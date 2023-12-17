
namespace CCAvenue
{
	public static class CCAvenueConfig
	{
		////Prod
		//public const string URL = "https://secure.ccavenue.com/transaction.do?command=initiateTransaction";
		//public const string WorkingKey = "0C1FD3FA1D2559BA3F7F8C33000C1E05";
		////public const string AccessCode = "AVNH29KC81BH53HNHB";
		//public const string AccessCode = "AVEY04KD81BL78YELB";
		//public const string MerchantId = "2164114";

		//Test
		public const string URL = "https://test.ccavenue.com/transaction/transaction.do?command=initiateTransaction";
		//public const string WorkingKey = "273AC536E5A5CE7EDC45438BEB610042";
		public const string WorkingKey = "0C1FD3FA1D2559BA3F7F8C33000C1E05";
		public const string AccessCode = "AVNH29KC81BH53HNHB";
		public const string MerchantId = "2164114";
	}

	public enum PaymentStatusCode
	{
		Failed,
		Success
	}

	public enum OrderStatus
	{
		Success,
		Failure,
		Aborted,
		Invalid
	}

	public enum PaymentMode
	{
		IVRS,
		EMI,
		CreditCard,
		NetBanking,
		DebitCard,
		CashCard,
		UPI,
		Wallet
	}

	public enum Currency
	{
		INR,
		USD,
		SGD,
		GBP,
		EUR
	}
}
