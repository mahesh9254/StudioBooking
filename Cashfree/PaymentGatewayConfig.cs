using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashfree
{
    public static class PaymentGatewayConfig
    {
        public const string Test = "https://sandbox.cashfree.com/pg";
        public const string Production = "https://api.cashfree.com/pg";
        public const string APIVersion = "2022-09-01";
        public const string TestClientAppID = "3277265a77409eb29cdb14420a627723";
        public const string TestClientSecret = "817dbe082afae8673e494e8b87d7180a1d2af901";
        public const string ProductionClientAppID = "3277265a77409eb29cdb14420a627723";
        public const string ProductionClientSecret = "817dbe082afae8673e494e8b87d7180a1d2af901";
    }
}
