using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCAvenue
{
    public class PaymentRequest
    {
        public string merchant_id { get; set; }
        public string order_id { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string redirect_url { get; set; }
        public string cancel_url { get; set; }
        public string? billing_name { get; set; }
        public string? billing_address { get; set; }
        public string? billing_city { get; set; }
        public string? billing_state { get; set; }
        public string? billing_zip { get; set; }
        public string? billing_country { get; set; }
        public string? billing_tel { get; set; }
        public string? billing_email { get; set; }
        public string? delivery_name { get; set; }
        public string? delivery_address { get; set; }
        public string? delivery_city { get; set; }
        public string? delivery_state { get; set; }
        public string? delivery_zip { get; set; }
        public string? delivery_country { get; set; }
        public string? delivery_tel { get; set; }
        public string? merchant_param1 { get; set; }
        public string? merchant_param2 { get; set; }
        public string? merchant_param3 { get; set; }
        public string? merchant_param4 { get; set; }
        public string? merchant_param5 { get; set; }
        public string? promo_code { get; set; }
        public string? customer_identifier { get; set; }
    }
}
