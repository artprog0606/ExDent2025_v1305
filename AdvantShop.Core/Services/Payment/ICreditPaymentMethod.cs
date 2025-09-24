namespace AdvantShop.Payment
{
    public interface ICreditPaymentMethod
    {
        float MinimumPrice { get;  }
        float? MaximumPrice { get;  }
        EnTypePresentationOfCreditInformation TypePresentationOfCreditInformation { get; }
        float? GetFirstPayment(float finalPrice);
        (float AmountPyament, int NumberOfPayments) GetAmountAndNumberOfPayments(float finalPrice);
        // int PaymentMethodId { get; }
        bool ActiveCreditPayment { get; }
        string CreditButtonTextInProductCard { get; }
        bool ShowCreditButtonInProductCard { get; }
    }

    public enum EnTypePresentationOfCreditInformation
    {
        /// <summary>
        /// Сумма первого платежа
        /// </summary>
        FirstPayment = 0,
        
        /// <summary>
        /// Кол-во платежей и сумма платежа
        /// <example>1200₽ x 4</example>
        /// </summary>
        AmountAndNumberOfPayments= 1
    }
}