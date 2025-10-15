using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ekklesia.Api.Models.DTOs;
using Ekklesia.Api.Services.Interfaces;
using System.Security.Claims;

namespace Ekklesia.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChurchesController : ControllerBase
    {
        private readonly IChurchService _churchService;
        private readonly ILogger<ChurchesController> _logger;

        public ChurchesController(IChurchService churchService, ILogger<ChurchesController> logger)
        {
            _churchService = churchService;
            _logger = logger;
        }

        /// <summary>
        /// Get all churches with optional filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChurchDto>>> GetChurches(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null,
            [FromQuery] string? city = null,
            [FromQuery] string? state = null,
            [FromQuery] string? denomination = null)
        {
            try
            {
                var churches = await _churchService.GetAllChurchesAsync(page, pageSize, status, city, state, denomination);
                return Ok(churches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving churches");
                return StatusCode(500, "An error occurred while retrieving churches");
            }
        }

        /// <summary>
        /// Get a specific church by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ChurchDto>> GetChurch(int id)
        {
            try
            {
                var church = await _churchService.GetChurchByIdAsync(id);
                
                if (church == null)
                    return NotFound($"Church with ID {id} not found");

                return Ok(church);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving church {ChurchId}", id);
                return StatusCode(500, "An error occurred while retrieving the church");
            }
        }

        /// <summary>
        /// Create a new church (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ChurchDto>> CreateChurch([FromBody] CreateChurchDto createChurchDto)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "system";
                var church = await _churchService.CreateChurchAsync(createChurchDto, userEmail);
                
                return CreatedAtAction(nameof(GetChurch), new { id = church.Id }, church);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating church");
                return StatusCode(500, "An error occurred while creating the church");
            }
        }

        /// <summary>
        /// Update a church (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ChurchDto>> UpdateChurch(int id, [FromBody] UpdateChurchDto updateChurchDto)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "system";
                var church = await _churchService.UpdateChurchAsync(id, updateChurchDto, userEmail);
                
                if (church == null)
                    return NotFound($"Church with ID {id} not found");

                return Ok(church);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating church {ChurchId}", id);
                return StatusCode(500, "An error occurred while updating the church");
            }
        }

        /// <summary>
        /// Delete a church (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteChurch(int id)
        {
            try
            {
                var result = await _churchService.DeleteChurchAsync(id);
                
                if (!result)
                    return NotFound($"Church with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting church {ChurchId}", id);
                return StatusCode(500, "An error occurred while deleting the church");
            }
        }

        /// <summary>
        /// Update church verification status (Admin only)
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateChurchStatus(int id, [FromBody] ChurchStatusUpdateDto statusUpdate)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "system";
                var result = await _churchService.UpdateChurchStatusAsync(id, statusUpdate, userEmail);
                
                if (!result)
                    return NotFound($"Church with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating church status for {ChurchId}", id);
                return StatusCode(500, "An error occurred while updating the church status");
            }
        }

        /// <summary>
        /// Search churches
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ChurchDto>>> SearchChurches(
            [FromQuery] string searchTerm,
            [FromQuery] double? latitude = null,
            [FromQuery] double? longitude = null,
            [FromQuery] double? radiusKm = null)
        {
            try
            {
                var churches = await _churchService.SearchChurchesAsync(searchTerm, latitude, longitude, radiusKm);
                return Ok(churches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching churches");
                return StatusCode(500, "An error occurred while searching churches");
            }
        }

        /// <summary>
        /// Import churches from CSV (Data Curator or Admin only)
        /// </summary>
        [HttpPost("import")]
        [Authorize(Roles = "DataCurator,Admin")]
        public async Task<ActionResult> ImportChurches(IFormFile csvFile)
        {
            try
            {
                if (csvFile == null || csvFile.Length == 0)
                    return BadRequest("No file uploaded");

                if (!csvFile.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    return BadRequest("File must be a CSV file");

                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "system";
                
                using var stream = csvFile.OpenReadStream();
                var importedCount = await _churchService.ImportChurchesFromCsvAsync(stream, userEmail);

                return Ok(new { ImportedCount = importedCount, Message = $"Successfully imported {importedCount} churches" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing churches from CSV");
                return StatusCode(500, "An error occurred while importing churches");
            }
        }
    }
}
