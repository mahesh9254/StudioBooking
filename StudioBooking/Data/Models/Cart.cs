using System.ComponentModel.DataAnnotations.Schema;

namespace StudioBooking.Data.Models
{
    public class Cart : BaseEntity
    {
        public long Id { get; set; }
        [ForeignKey("Customer")]
        public long? CustomerId { get; set; }
        public int ServicePriceId { get; set; }
        //public long TransactionId { get; set; }
        public string? BookingDate { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public bool? IsCheckedOut { get; set; }
        public Customer Customer { get; set; }
        public Cart()
        {
            Customer= new Customer();
        }
    }
}
