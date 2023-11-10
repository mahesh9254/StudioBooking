using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashfree
{
    public class OrderResponse
    {
        public long cf_order_id { get; set; }
        public string order_id { get; set; }
        public string entity { get; set; }
        public string order_currency { get; set; }
        public double order_amount { get; set; }
        public string order_status { get; set; }
        public string payment_session_id { get; set; }
        public DateTime order_expiry_time { get; set; }
        public string order_note { get; set; }
        public DateTime created_at { get; set; }
        public CustomerDetail customer_details { get; set; }
        public Payments payments { get; set; }
        public Refunds refunds { get; set; }
        public Settlements settlements { get; set; }
        public OrderResponse()
        {
            payments = new Payments();
            refunds = new Refunds();
            settlements = new Settlements();
        }
    }

    public class Payments
    {
        public string? url { get; set; }
    }
    public class Refunds
    {
        public string? url { get; set; }
    }
    public class Settlements
    {
        public string? url { get; set; }
    }
}
