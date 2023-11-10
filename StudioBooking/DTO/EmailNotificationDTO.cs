namespace StudioBooking.DTO
{
    public class EmailNotificationDTO
    {
        public string? TransactionId { get; set; }
        public string? Name { get; set; }
        public string? EmailId { get; set; }
        public string? EmailFrom { get; set; }
        public string? EmailBcc { get; set; }
        public string? EmailCc { get; set; }
        public string? Meassage { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Refund { get; set; }
        public string? Year { get; set; }
        public string? EmailBody { get; set; }
        public string? EmailNotification { get; set; }
        public string? Subject { get; set; }
        public List<string> Docs { get; set; }
        public EmailNotificationDTO()
        {
            Docs = new List<string>();
        }

    }
}
