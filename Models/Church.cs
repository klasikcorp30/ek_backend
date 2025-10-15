using System.ComponentModel.DataAnnotations;

namespace Ekklesia.Api.Models
{
    public class Church
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(20)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? ZipCode { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        [StringLength(50)]
        public string? Denomination { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public ChurchStatus Status { get; set; } = ChurchStatus.Pending;

        [StringLength(500)]
        public string? Description { get; set; }

        // Service schedule information
        public string? ServiceSchedule { get; set; } // JSON string storing service times

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public enum ChurchStatus
    {
        Pending = 0,
        Verified = 1,
        Rejected = 2
    }
}