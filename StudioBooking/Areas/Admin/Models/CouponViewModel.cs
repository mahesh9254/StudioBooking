namespace StudioBooking.Areas.Admin.Models
{
    public class CouponViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }       
        public int Discount { get; set; }
        public bool IsActive { get; set; }
    }
}
