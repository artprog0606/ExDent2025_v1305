using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    public class BasePaymentOption : AbstractPaymentOption
    {
        public BasePaymentOption()
        {
        }

        public BasePaymentOption(PaymentMethod method, float preCoast)
        {
            Id = method.PaymentMethodId;
            Name = method.Name;
            IconName = PaymentIcons.GetPaymentIcon(method.PaymentKey, method.IconFileName?.PhotoName, method.Name);
            Desc = method.Description;
            Rate = method.GetExtracharge(preCoast);
        }

        public PaymentDetails GetDetails(Order order)
        {
            return GetDetailsByOrder(order) ?? GetDetails();
        }
        
        protected virtual PaymentDetails GetDetailsByOrder(Order order)
        {
            return null;
        }
        
        public virtual PaymentDetails GetDetails()
        {
            return null;
        }

        public virtual void SetDetails(PaymentDetails details)
        {
        }

        public virtual bool Update(BasePaymentOption temp)
        {            
            return true;
        }
    }
}