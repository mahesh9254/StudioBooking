using System.ComponentModel.DataAnnotations;

namespace StudioBooking.Data.Models
{
    public class Service : BaseEntity
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public int ServiceType { get; set; }
        public bool EnableTwoStepBooking { get; set; }
        public ICollection<ServicePrice> ServicePrices { get; set; }
        public Service()
        {
            Name = string.Empty;
            ServicePrices = new List<ServicePrice>();
        }
    }
}
