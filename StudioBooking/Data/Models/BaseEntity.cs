using System.ComponentModel.DataAnnotations;

namespace StudioBooking.Data.Models
{
    public class BaseEntity
    {
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(100)]
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
