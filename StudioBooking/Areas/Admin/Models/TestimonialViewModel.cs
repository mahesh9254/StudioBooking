using StudioBooking.DTO;

namespace StudioBooking.Areas.Admin.Models
{
    public class TestimonialViewModel
    {
        public TestimonialDTO Testimonial { get; set; }
        public bool Result { get; set; }
        public string? ErrorMessage { get; set; }
        public TestimonialViewModel()
        {
            Testimonial = new TestimonialDTO();
        }
    }
}
