using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ekklesia.Api.Data;
using System.Diagnostics;

namespace Ekklesia.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly EkklesiaDbContext _context;
        private readonly ILogger<HealthController> _logger;

        public HealthController(EkklesiaDbContext context, ILogger<HealthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Basic health check endpoint
        /// </summary>
        [HttpGet]
        public ActionResult<HealthResponse> Get()
        {
            try
            {
                var response = new HealthResponse
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(500, new HealthResponse
                {
                    Status = "Unhealthy",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                    Error = "Internal server error"
                });
            }
        }

        /// <summary>
        /// Database health check endpoint
        /// </summary>
        [HttpGet("database")]
        public async Task<ActionResult<DatabaseHealthResponse>> GetDatabaseHealth()
        {
            try
            {
                var startTime = DateTime.UtcNow;
                
                // Test database connection
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    return StatusCode(503, new DatabaseHealthResponse
                    {
                        Status = "Unhealthy",
                        Timestamp = DateTime.UtcNow,
                        DatabaseType = _context.Database.ProviderName,
                        CanConnect = false,
                        Error = "Cannot connect to database"
                    });
                }

                // Get some basic stats
                var userCount = await _context.Users.CountAsync();
                var churchCount = await _context.Churches.CountAsync();
                var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                var response = new DatabaseHealthResponse
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    DatabaseType = _context.Database.ProviderName,
                    CanConnect = true,
                    ResponseTimeMs = responseTime,
                    Statistics = new DatabaseStatistics
                    {
                        TotalUsers = userCount,
                        TotalChurches = churchCount,
                        ActiveChurches = await _context.Churches.CountAsync(c => c.IsActive),
                        VerifiedChurches = await _context.Churches.CountAsync(c => c.Status == Models.ChurchStatus.Verified)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return StatusCode(503, new DatabaseHealthResponse
                {
                    Status = "Unhealthy",
                    Timestamp = DateTime.UtcNow,
                    DatabaseType = _context.Database.ProviderName,
                    CanConnect = false,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Detailed system information
        /// </summary>
        [HttpGet("info")]
        public ActionResult<SystemInfoResponse> GetSystemInfo()
        {
            try
            {
                var response = new SystemInfoResponse
                {
                    ApplicationName = "Ekklesia API",
                    Version = "1.0.0",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                    MachineName = Environment.MachineName,
                    OperatingSystem = Environment.OSVersion.ToString(),
                    ProcessorCount = Environment.ProcessorCount,
                    WorkingSet = Environment.WorkingSet,
                    DotNetVersion = Environment.Version.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "System info check failed");
                return StatusCode(500, "Failed to retrieve system information");
            }
        }
    }

    public class HealthResponse
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Version { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string? Error { get; set; }
    }

    public class DatabaseHealthResponse
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? DatabaseType { get; set; }
        public bool CanConnect { get; set; }
        public double? ResponseTimeMs { get; set; }
        public DatabaseStatistics? Statistics { get; set; }
        public string? Error { get; set; }
    }

    public class DatabaseStatistics
    {
        public int TotalUsers { get; set; }
        public int TotalChurches { get; set; }
        public int ActiveChurches { get; set; }
        public int VerifiedChurches { get; set; }
    }

    public class SystemInfoResponse
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public int ProcessorCount { get; set; }
        public long WorkingSet { get; set; }
        public string DotNetVersion { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public TimeSpan Uptime { get; set; }
    }
}