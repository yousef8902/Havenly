using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities
{
    public class PropertyBlockedDate
    {
        [Key]
        public long BlockedDateID { get; set; }

        public long PropertyID { get; set; }

        [ForeignKey(nameof(PropertyID))]
        public Property? Property { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [MaxLength(200)]
        public string Reason { get; set; } = "Maintenance / Personal Use";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
