namespace AdvantShop.Core.Services.Payment.Modulbank
{
    public class BaseResponseObject
    {
        public string Status { get; set; }
    }

    public class TransactionContainer : BaseResponseObject
    {
        public Transaction Transaction { get; set; }
    }

    public class Transaction
    {
        public string State { get; set; }
        public string OrderId { get; set; }
        public float Amount { get; set; }
    }
}
