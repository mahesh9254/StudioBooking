namespace StudioBooking.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int ApprovalPending { get; set; }
        public int PendingBookings { get; set; }
        public int AdvanceBookings { get; set; }
        public double TotalBookedHours { get; set; }
    }
}
