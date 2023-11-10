using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;

namespace StudioBooking.DTO
{
    public class AddOnDTO
    {
        public long Id { get; set; }
        public long? BookingId { get; set; }
        public string Name { get; set; }
        public double? BookingTotal { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
        public int AdjustmentType { get; set; }
        public DateTime CreatedDate { get; set; }

        public static async Task<List<AddOnDTO>> GetBookingAddOns(ApplicationDbContext context, long id)
        {
            return await context.Addons.Where(a => a.BookingId == id && a.IsActive).Select(a => new AddOnDTO { Id = a.Id, BookingId = a.BookingId, Name = a.Name, Description = a.Description, Amount = a.Amount, CreatedDate = a.CreatedDate,AdjustmentType=a.AdjustmentType }).ToListAsync();
        }
    }
}
