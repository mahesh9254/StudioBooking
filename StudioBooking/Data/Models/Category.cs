namespace StudioBooking.Data.Models
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public ICollection<ServicePrice> ServicePrices { get; set; }
        public ICollection<CategoryDetail> CategoryDetails { get; set; }
    }
}
