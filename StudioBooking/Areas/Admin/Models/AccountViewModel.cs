using Microsoft.AspNetCore.Mvc.Rendering;
using StudioBooking.DTO;

namespace StudioBooking.Areas.Admin.Models
{
    public class AccountViewModel
    {
        public UserDTO User { get; set; }
        public CustomerDTO? Customer { get; set; }
        public List<WalletDTO> Wallets { get; set; }
        public bool Result { get; set; }
        public string? ErrorMessage { get; set; }
        public SelectList? Roles { get; internal set; }

        public AccountViewModel()
        {
            User = new UserDTO();
            Customer = new CustomerDTO();
            Wallets = new List<WalletDTO>();
        }
    }
}
