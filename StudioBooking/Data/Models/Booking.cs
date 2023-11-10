using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudioBooking.Data.Models
{
    public class Booking : BaseEntity
    {
        public long Id { get; set; }
        [ForeignKey("ServicePrice")]
        public int ServicePriceId { get; set; }
        [ForeignKey("Customer")]
        public long? CustomerId { get; set; }
        public DateTime BookingDate { get; set; }
        [MaxLength(50)]
        public string StartTime { get; set; }
        [MaxLength(50)]
        public string EndTime { get; set; }
        public decimal RatePerHour { get; set; }
        public double Total { get; set; }
        public int BookingStatus { get; set; }
        public int PaymentStatus { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
        public bool IsAddonRequested { get; set; }
        [ForeignKey("CustomerAddress")]
        public long? BillingAddressId { get; set; }
        [MaxLength(100)]
        public string? CalenderEventId { get; set; }
        public ServicePrice ServicePrice { get; set; }
        public Customer? Customer { get; set; }
        public CustomerAddress? CustomerAddress { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<PaymentGatewayResponse> PaymentGatewayResponses { get; set; }
        public ICollection<ScheduleRequest> ScheduleRequests { get; set; }
        public ICollection<Addon> Addons { get; set; }        

        public Booking()
        {
            Transactions = new List<Transaction>();
            PaymentGatewayResponses = new List<PaymentGatewayResponse>();
            ScheduleRequests = new List<ScheduleRequest>();
            Addons = new List<Addon>();
        }
    }
}
