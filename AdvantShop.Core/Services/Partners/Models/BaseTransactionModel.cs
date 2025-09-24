namespace AdvantShop.Core.Services.Partners
{
    public abstract class BaseTransactionModel
    {
        protected int Id { get; set; }

        protected string CurrencySymbol { get; set; }
        protected string CurrencyCode { get; set; }
        protected float CurrencyValue { get; set; }
        protected bool CurrencyIsCodeBefore { get; set; }

        protected TransactionCurrency Currency
        {
            get
            {
                return new TransactionCurrency
                {
                    CurrencySymbol = CurrencySymbol,
                    CurrencyCode = CurrencyCode,
                    CurrencyValue = CurrencyValue,
                    IsCodeBefore = CurrencyIsCodeBefore
                };
            }
        }

        protected string DetailsJson { get; set; }
        protected TransactionDetailsModel _details;
        public TransactionDetailsModel Details
        {
            get
            {
                return _details ?? (_details = new TransactionDetailsModel(DetailsJson, Currency, Id));
            }
        }
    }
}
