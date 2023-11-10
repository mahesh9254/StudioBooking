using StudioBooking.DTO;

namespace StudioBooking.Areas.Admin.Models
{
    public class ClientViewModel
    {
        public ClientDTO Client { get; set; }
        public bool Result { get; set; }
        public string? ErrorMessage { get; set; }        
        public ClientViewModel()
        {
            Client = new ClientDTO();
        }
    }
}
