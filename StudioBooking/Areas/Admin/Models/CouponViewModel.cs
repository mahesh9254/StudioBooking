namespace StudioBooking.Areas.Admin.Models
{
    public class CouponViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public double Discount { get; set; }
        public bool IsActive { get; set; }
    }
}
