namespace Ekklesia.Api.Models.DTOs
{
    public class ChurchDto
    {
        public int Id { get; set; }
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
        public ChurchStatus Status { get; set; }
        public string? Description { get; set; }
        public ServiceScheduleDto? ServiceSchedule { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateChurchDto
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
        public ServiceScheduleDto? ServiceSchedule { get; set; }
    }

    public class UpdateChurchDto
    {
        public string? Name { get; set; }
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
        public ServiceScheduleDto? ServiceSchedule { get; set; }
    }

    public class ServiceScheduleDto
    {
        public List<ServiceTimeDto> Services { get; set; } = new();
    }

    public class ServiceTimeDto
    {
        public string Day { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // e.g., "Sunday Service", "Bible Study", etc.
    }

    public class ChurchStatusUpdateDto
    {
        public ChurchStatus Status { get; set; }
        public string? Reason { get; set; }
    }
}