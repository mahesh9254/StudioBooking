namespace StudioBooking.DTO
{
    public class CouponDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public double Discount { get; set; }
    }
}
