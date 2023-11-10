using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;
using System.Globalization;

namespace StudioBooking.DTO
{
    public class CartDTO
    {
        public long Id { get; set; }
        public int ServiceId { get; set; }
        public int CategoryId { get; set; }
        public int ServicePriceId { get; set; }
        public long TransactionId { get; set; }
        public string? BookingDate { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public bool? IsCheckedOut { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public static Cart GetCart(CartDTO cart, long customerId, string userId)
        {            
            return new Cart
            {
                BookingDate = cart.BookingDate,
                ServicePriceId = cart.ServicePriceId,
                StartTime = TimeOnly.Parse(cart.StartTime).ToString(),
                EndTime = TimeOnly.Parse(cart.EndTime).ToString(),
                CustomerId = customerId,
                IsActive = true,
                CreatedBy = userId,
                CreatedDate = Defaults.GetDateTime(),
                ModifiedBy = userId,
                ModifiedDate = Defaults.GetDateTime(),
            };
        }
    }
}
