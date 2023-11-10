using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashfree
{
    public class OrderRequest
    {
        public string order_id { get; set; }
        public double order_amount { get; set; }
        public string order_currency { get; set; } = "INR";
        public DateTime? order_expiry_time { get; set; }
        public string? order_note { get; set; }
        public CustomerDetail customer_details { get; set; }
        public OrderMeta order_meta { get; set; }
        public List<string> order_tags { get; set; }
    }
}
