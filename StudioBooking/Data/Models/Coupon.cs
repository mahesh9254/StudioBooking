namespace StudioBooking.Data.Models
{
    public class Coupon : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }       
        public int Discount { get; set; }       

    }
}
