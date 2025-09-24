using System.Collections.Generic;
using AdvantShop.Payment;

namespace AdvantShop.Web.Admin.Handlers.Settings.PaymentMethods
{
    public class GetPaymentMethods
    {
        public GetPaymentMethods()
        {
            
        }

        public List<PaymentMethod> Execute()
        {
            return PaymentService.GetAllPaymentMethods(false);
        }

    }
}
