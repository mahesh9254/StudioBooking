using StudioBooking.DTO;

namespace StudioBooking.Areas.Admin.Models
{
    public class TeamViewModel
    {
        public TeamDTO Team { get; set; }
        public bool Result { get; set; }
        public string? ErrorMessage { get; set; }
        public TeamViewModel()
        {
            Team = new TeamDTO();
        }
    }
}
