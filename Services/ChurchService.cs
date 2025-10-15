using Microsoft.EntityFrameworkCore;
using Ekklesia.Api.Data;
using Ekklesia.Api.Models;
using Ekklesia.Api.Models.DTOs;
using Ekklesia.Api.Services.Interfaces;
using System.Text.Json;
using CsvHelper;
using System.Globalization;

namespace Ekklesia.Api.Services
{
    public class ChurchService : IChurchService
    {
        private readonly EkklesiaDbContext _context;
        private readonly ILogger<ChurchService> _logger;

        public ChurchService(EkklesiaDbContext context, ILogger<ChurchService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ChurchDto>> GetAllChurchesAsync(int page = 1, int pageSize = 20, 
            string? status = null, string? city = null, string? state = null, string? denomination = null)
        {
            var query = _context.Churches.Where(c => c.IsActive);

            // Apply filters
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ChurchStatus>(status, true, out var statusEnum))
            {
                query = query.Where(c => c.Status == statusEnum);
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(c => c.City != null && c.City.ToLower().Contains(city.ToLower()));
            }

            if (!string.IsNullOrEmpty(state))
            {
                query = query.Where(c => c.State != null && c.State.ToLower().Contains(state.ToLower()));
            }

            if (!string.IsNullOrEmpty(denomination))
            {
                query = query.Where(c => c.Denomination != null && c.Denomination.ToLower().Contains(denomination.ToLower()));
            }

            var churches = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return churches.Select(MapToDto);
        }

        public async Task<ChurchDto?> GetChurchByIdAsync(int id)
        {
            var church = await _context.Churches
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            return church != null ? MapToDto(church) : null;
        }

        public async Task<ChurchDto> CreateChurchAsync(CreateChurchDto createChurchDto, string createdBy)
        {
            var church = new Church
            {
                Name = createChurchDto.Name,
                Address = createChurchDto.Address,
                City = createChurchDto.City,
                State = createChurchDto.State,
                ZipCode = createChurchDto.ZipCode,
                Phone = createChurchDto.Phone,
                Email = createChurchDto.Email,
                Website = createChurchDto.Website,
                Denomination = createChurchDto.Denomination,
                Latitude = createChurchDto.Latitude,
                Longitude = createChurchDto.Longitude,
                Description = createChurchDto.Description,
                ServiceSchedule = createChurchDto.ServiceSchedule != null 
                    ? JsonSerializer.Serialize(createChurchDto.ServiceSchedule) 
                    : null,
                Status = ChurchStatus.Pending,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Churches.Add(church);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Church {ChurchName} created by {User}", church.Name, createdBy);

            return MapToDto(church);
        }

        public async Task<ChurchDto?> UpdateChurchAsync(int id, UpdateChurchDto updateChurchDto, string updatedBy)
        {
            var church = await _context.Churches.FindAsync(id);
            if (church == null || !church.IsActive)
                return null;

            // Update fields if provided
            if (!string.IsNullOrEmpty(updateChurchDto.Name))
                church.Name = updateChurchDto.Name;
            
            if (updateChurchDto.Address != null)
                church.Address = updateChurchDto.Address;
            
            if (updateChurchDto.City != null)
                church.City = updateChurchDto.City;
            
            if (updateChurchDto.State != null)
                church.State = updateChurchDto.State;
            
            if (updateChurchDto.ZipCode != null)
                church.ZipCode = updateChurchDto.ZipCode;
            
            if (updateChurchDto.Phone != null)
                church.Phone = updateChurchDto.Phone;
            
            if (updateChurchDto.Email != null)
                church.Email = updateChurchDto.Email;
            
            if (updateChurchDto.Website != null)
                church.Website = updateChurchDto.Website;
            
            if (updateChurchDto.Denomination != null)
                church.Denomination = updateChurchDto.Denomination;
            
            if (updateChurchDto.Latitude.HasValue)
                church.Latitude = updateChurchDto.Latitude;
            
            if (updateChurchDto.Longitude.HasValue)
                church.Longitude = updateChurchDto.Longitude;
            
            if (updateChurchDto.Description != null)
                church.Description = updateChurchDto.Description;
            
            if (updateChurchDto.ServiceSchedule != null)
                church.ServiceSchedule = JsonSerializer.Serialize(updateChurchDto.ServiceSchedule);

            church.UpdatedBy = updatedBy;
            church.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Church {ChurchId} updated by {User}", id, updatedBy);

            return MapToDto(church);
        }

        public async Task<bool> DeleteChurchAsync(int id)
        {
            var church = await _context.Churches.FindAsync(id);
            if (church == null)
                return false;

            // Soft delete
            church.IsActive = false;
            church.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Church {ChurchId} deleted", id);

            return true;
        }

        public async Task<bool> UpdateChurchStatusAsync(int id, ChurchStatusUpdateDto statusUpdate, string updatedBy)
        {
            var church = await _context.Churches.FindAsync(id);
            if (church == null || !church.IsActive)
                return false;

            church.Status = statusUpdate.Status;
            church.UpdatedBy = updatedBy;
            church.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Church {ChurchId} status updated to {Status} by {User}. Reason: {Reason}", 
                id, statusUpdate.Status, updatedBy, statusUpdate.Reason);

            return true;
        }

        public async Task<IEnumerable<ChurchDto>> SearchChurchesAsync(string searchTerm, 
            double? latitude = null, double? longitude = null, double? radiusKm = null)
        {
            var query = _context.Churches.Where(c => c.IsActive && c.Status == ChurchStatus.Verified);

            // Text search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(c => 
                    (c.Name != null && c.Name.ToLower().Contains(term)) ||
                    (c.City != null && c.City.ToLower().Contains(term)) ||
                    (c.State != null && c.State.ToLower().Contains(term)) ||
                    (c.Denomination != null && c.Denomination.ToLower().Contains(term)) ||
                    (c.Description != null && c.Description.ToLower().Contains(term)));
            }

            var churches = await query.ToListAsync();

            // Location-based filtering (simple distance calculation)
            if (latitude.HasValue && longitude.HasValue && radiusKm.HasValue)
            {
                churches = churches.Where(c => 
                    c.Latitude.HasValue && c.Longitude.HasValue &&
                    CalculateDistance(latitude.Value, longitude.Value, c.Latitude.Value, c.Longitude.Value) <= radiusKm.Value)
                    .ToList();
            }

            return churches.Select(MapToDto);
        }

        public async Task<int> ImportChurchesFromCsvAsync(Stream csvStream, string importedBy)
        {
            var importedCount = 0;

            try
            {
                using var reader = new StreamReader(csvStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecordsAsync<ChurchCsvRecord>();
                
                await foreach (var record in records)
                {
                    try
                    {
                        // Check if church with same name and address already exists
                        var existingChurch = await _context.Churches
                            .FirstOrDefaultAsync(c => c.Name == record.Name && c.Address == record.Address);

                        if (existingChurch != null)
                        {
                            // Update existing church
                            existingChurch.Phone = record.Phone;
                            existingChurch.Email = record.Email;
                            existingChurch.Website = record.Website;
                            existingChurch.Denomination = record.Denomination;
                            existingChurch.UpdatedBy = importedBy;
                            existingChurch.UpdatedAt = DateTime.UtcNow;
                        }
                        else
                        {
                            // Create new church
                            var newChurch = new Church
                            {
                                Name = record.Name,
                                Address = record.Address,
                                City = record.City,
                                State = record.State,
                                ZipCode = record.ZipCode,
                                Phone = record.Phone,
                                Email = record.Email,
                                Website = record.Website,
                                Denomination = record.Denomination,
                                Latitude = record.Latitude,
                                Longitude = record.Longitude,
                                Description = record.Description,
                                Status = ChurchStatus.Pending,
                                CreatedBy = importedBy,
                                UpdatedBy = importedBy,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            _context.Churches.Add(newChurch);
                        }

                        importedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to import church record: {ChurchName}", record.Name);
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully imported {Count} churches by {User}", importedCount, importedBy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import churches from CSV");
                throw;
            }

            return importedCount;
        }

        private static ChurchDto MapToDto(Church church)
        {
            ServiceScheduleDto? serviceSchedule = null;
            if (!string.IsNullOrEmpty(church.ServiceSchedule))
            {
                try
                {
                    serviceSchedule = JsonSerializer.Deserialize<ServiceScheduleDto>(church.ServiceSchedule);
                }
                catch
                {
                    // Ignore JSON deserialization errors
                }
            }

            return new ChurchDto
            {
                Id = church.Id,
                Name = church.Name,
                Address = church.Address,
                City = church.City,
                State = church.State,
                ZipCode = church.ZipCode,
                Phone = church.Phone,
                Email = church.Email,
                Website = church.Website,
                Denomination = church.Denomination,
                Latitude = church.Latitude,
                Longitude = church.Longitude,
                Status = church.Status,
                Description = church.Description,
                ServiceSchedule = serviceSchedule,
                CreatedAt = church.CreatedAt,
                UpdatedAt = church.UpdatedAt,
                IsActive = church.IsActive
            };
        }

        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula for calculating distance between two points on Earth
            const double R = 6371; // Earth's radius in kilometers

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }

    public class ChurchCsvRecord
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Denomination { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Description { get; set; }
    }
}