using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashfree
{
    public class CustomerDetail
    {
        public string customer_id { get; set; }
        public string? customer_name { get; set; }
        public string customer_email { get; set; }
        public string customer_phone { get; set; }
        public string? customer_bank_account_number { get; set; }
        public string? customer_bank_ifsc { get; set; }
        public string? customer_bank_code { get; set; }
    }
}
