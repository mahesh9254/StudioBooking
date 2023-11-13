namespace StudioBooking.Data.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public double Discount { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public Category Category { get; set; }
        public Service Service { get; set; }        
    }
}
