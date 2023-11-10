using CCA.Util;

namespace CCAvenue
{
    public class PaymentResponse
    {
        public string order_id { get; set; }
        public string tracking_id { get; set; }
        public string bank_ref_no { get; set; }
        public string order_status { get; set; }
        public string? failure_message { get; set; }
        public string? payment_mode { get; set; }
        public string? card_name { get; set; }
        public int status_code { get; set; }
        public string? status_message { get; set; }
        public string currency { get; set; }
        public string amount { get; set; }
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
        public string? vault { get; set; }
        public string? offer_type { get; set; }
        public string? offer_code { get; set; }
        public string? discount_value { get; set; }
        public string? mer_amount { get; set; }
        public string? eci_value { get; set; }
        public string? retry { get; set; }
        public string? response_code { get; set; }
        public string? billing_notes { get; set; }
        public string? trans_date { get; set; }
        public string? bin_country { get; set; }

        public static PaymentResponse GetPaymentResponse(string encResp, string workingKey)
        {
            var paymentResponse = new PaymentResponse();
            var ccaCrypto = new CCACrypto();
            var encResponse = ccaCrypto.Decrypt(encResp, workingKey);
            string[] segments = encResponse.Split('&');
            foreach (string seg in segments)
            {
                string[] parts = seg.Split('=');
                if (parts.Length > 0)
                {
                    string Key = parts[0].Trim();
                    string Value = parts[1].Trim();
                    if (Key == "order_id")
                        paymentResponse.order_id = Value;
                    if (Key == "tracking_id")
                        paymentResponse.tracking_id = Value;
                    if (Key == "bank_ref_no")
                        paymentResponse.bank_ref_no = Value;
                    if (Key == "order_status")
                        paymentResponse.order_status = Value;
                    if (Key == "failure_message")
                        paymentResponse.failure_message = Value;
                    if (Key == "payment_mode")
                        paymentResponse.payment_mode = Value;
                    if (Key == "card_name")
                        paymentResponse.card_name = Value;
                    if (Key == "status_code")
                        paymentResponse.status_code = string.IsNullOrEmpty(Value) ? (int)PaymentStatusCode.Failed : (int)PaymentStatusCode.Success;
                    if (Key == "status_message")
                        paymentResponse.status_message = Value;
                    if (Key == "currency")
                        paymentResponse.currency = Value;
                    if (Key == "amount")
                        paymentResponse.amount = Value;
                    if (Key == "billing_name")
                        paymentResponse.billing_name = Value;
                    if (Key == "billing_address")
                        paymentResponse.billing_address = Value;
                    if (Key == "billing_city")
                        paymentResponse.billing_city = Value;
                    if (Key == "billing_state")
                        paymentResponse.billing_state = Value;
                    if (Key == "billing_zip")
                        paymentResponse.billing_zip = Value;
                    if (Key == "billing_country")
                        paymentResponse.billing_country = Value;
                    if (Key == "billing_tel")
                        paymentResponse.billing_tel = Value;
                    if (Key == "billing_email")
                        paymentResponse.billing_email = Value;
                    if (Key == "delivery_name")
                        paymentResponse.delivery_name = Value;
                    if (Key == "delivery_address")
                        paymentResponse.delivery_address = Value;
                    if (Key == "delivery_city")
                        paymentResponse.delivery_city = Value;
                    if (Key == "delivery_state")
                        paymentResponse.delivery_state = Value;
                    if (Key == "delivery_zip")
                        paymentResponse.delivery_zip = Value;
                    if (Key == "delivery_country")
                        paymentResponse.delivery_country = Value;
                    if (Key == "delivery_tel")
                        paymentResponse.delivery_tel = Value;
                    if (Key == "merchant_param1")
                        paymentResponse.merchant_param1 = Value;
                    if (Key == "merchant_param2")
                        paymentResponse.merchant_param2 = Value;
                    if (Key == "merchant_param3")
                        paymentResponse.merchant_param3 = Value;
                    if (Key == "merchant_param4")
                        paymentResponse.merchant_param4 = Value;
                    if (Key == "merchant_param5")
                        paymentResponse.merchant_param5 = Value;
                    if (Key == "vault")
                        paymentResponse.vault = Value;
                    if (Key == "offer_type")
                        paymentResponse.offer_type = Value;
                    if (Key == "offer_type")
                        paymentResponse.offer_code = Value;
                    if (Key == "discount_value")
                        paymentResponse.discount_value = Value;
                    if (Key == "mer_amount")
                        paymentResponse.mer_amount = Value;
                    if (Key == "eci_value")
                        paymentResponse.eci_value = Value;
                    if (Key == "retry")
                        paymentResponse.retry = Value;
                    if (Key == "response_code")
                        paymentResponse.response_code = Value;
                    if (Key == "billing_notes")
                        paymentResponse.billing_notes = Value;
                    if (Key == "trans_date")
                        paymentResponse.trans_date = Value;
                    if (Key == "bin_country")
                        paymentResponse.bin_country = Value;
                }
            }
            return paymentResponse;
        }
    }
}
