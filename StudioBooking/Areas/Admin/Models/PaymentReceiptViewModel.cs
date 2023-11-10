using Microsoft.AspNetCore.Mvc.Rendering;
using StudioBooking.DTO;

namespace StudioBooking.Areas.Admin.Models
{
    public class PaymentReceiptViewModel
    {
        public PaymentReceiptDTO PaymentReceipt { get; set; }        
        public bool Result { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
