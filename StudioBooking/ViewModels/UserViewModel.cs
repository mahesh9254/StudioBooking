using StudioBooking.DTO;

namespace StudioBooking.ViewModels
{
    public class UserViewModel
    {
        public UserDTO User { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
