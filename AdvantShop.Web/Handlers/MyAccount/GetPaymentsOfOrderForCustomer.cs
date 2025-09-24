using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping;

namespace AdvantShop.Handlers.MyAccount
{
    public class GetPaymentsOfOrderForCustomer
    {
        private readonly Order _order;

        public GetPaymentsOfOrderForCustomer(Order order)
        {
            _order = order ?? throw new ArgumentNullException(nameof(order));
        }

        public List<BasePaymentOption> Get()
        {
            BaseShippingOption shipping = null;

            var calculationParameters =
                ShippingCalculationConfigurator.Configure()
                                               .ByOrder(_order, actualizeShipping: true)
                                               .Build();

            if (_order.ShippingMethodId.IsNotDefault())
            {
                var shippingManager = new ShippingManager(calculationParameters);
                shippingManager.PreferShippingOptionFromParameters();

                shipping = shippingManager.GetOptions().FirstOrDefault();
            }

            shipping = shipping
                       ?? calculationParameters.ShippingOption
                       // мало вероятно, но на всякий
                       ?? new BaseShippingOption()
                       {
                           Name = _order.ArchivedShippingName, 
                           Rate = _order.ShippingCost,
                           IsAvailablePaymentCashOnDelivery = _order.AvailablePaymentCashOnDelivery,
                           IsAvailablePaymentPickPoint = _order.AvailablePaymentPickPoint
                       };
            
            if (_order.OrderPickPoint != null)
                shipping?.UpdateFromOrderPickPoint(_order.OrderPickPoint);

            var manager = new PaymentManager(
                config => config
                         .ByOrder(_order/*, actualizeShippingAndPayment: true*/)
                         .WithShippingOption(shipping)
                         .Build());
            var options = manager.GetOptions();

            if (_order.PaymentDetails != null)
            {
                foreach (var option in options)
                    option.SetDetails(_order.PaymentDetails);
            }

            return options;
        }
    }
}