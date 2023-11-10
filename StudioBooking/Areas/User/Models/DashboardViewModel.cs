using StudioBooking.DTO;

namespace StudioBooking.Areas.User.Models
{
    public class DashboardViewModel
    {
        public UserDTO? User { get; set; }
        public double Coins { get; set; }
        public decimal Hours { get; set; }
        public List<BookingDTO> Bookings { get; set; }
        public List<TransactionDTO> Transactions { get; set; }
        public DashboardViewModel()
        {            
            Bookings = new List<BookingDTO>();
            Transactions = new List<TransactionDTO>();
        }
    }
}
