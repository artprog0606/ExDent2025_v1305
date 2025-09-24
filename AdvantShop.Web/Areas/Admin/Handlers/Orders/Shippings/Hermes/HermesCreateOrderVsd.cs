using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Shipping.Hermes;
using AdvantShop.Shipping.Hermes.Api;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Hermes
{
    public class HermesCreateOrderVsd : HermesCreateOrder
    {
        public HermesCreateOrderVsd(int orderId) : base(orderId)
        {
        }

        public override string CreateOrder(Order order, Shipping.Hermes.Hermes hermesMethod, HermesCalculateOption hermesCalculateOption, OrderPickPoint orderPickPoint)
        {
            var paymentsCash = new[]
            {
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.CashOnDelivery)),
            };

            var businessUnit = hermesMethod.GetBusinessUnit(hermesCalculateOption.BusinessUnitCode);
            var dimensions = hermesMethod.GetDimensionsHermesInCentimeters();

            var client = new RestApiClient(hermesMethod.SecuredToken, hermesMethod.PublicToken);
            var parcel = new CreateVsdParcelParams
            {
                // Тут еще обязательные параметры
                //ActNumber = ,
                //VsdActDate = ,
                //VsdDocumentDate = ,
                PickupDcCode = hermesCalculateOption.SourcePointCode,
                BusinessUnitCode = hermesCalculateOption.BusinessUnitCode,
                ParcelBarcode = !businessUnit.Services.Contains("CLIENT_USE_AUTOGENERATE_BARCODE_ORDER_BY_HERMES") ? order.Number : null,
                ClientParcelNumber = order.Number.Length > 40 ? order.OrderID.ToString() : order.Number,
                ClientOrderNumber = order.Number.Length > 40 ? order.OrderID.ToString() : order.Number,
                ParcelShopCode = orderPickPoint.PickPointId,
                CashOnDeliveryValue = !order.Payed &&
                    (order.PaymentMethod != null &&
                        paymentsCash.Contains(order.PaymentMethod.PaymentKey))
                        ? order.Sum
                        : 0f,
                InsuranceValue = hermesCalculateOption.WithInsure
                    ? order.Sum
                    : 0f,
                ParcelMeasurements = new ParcelMeasurements
                {
                    WeightInGrams = (int)hermesMethod.GetTotalWeight(1000),
                    HeightInCentimeters = dimensions[2],
                    WidthInCentimeters = dimensions[1],
                    LengthInCentimeters = dimensions[0]
                },
                Customer = new Customer
                {
                    FirstName = order.OrderRecipient.FirstName.Reduce(32),
                    LastName = order.OrderRecipient.LastName.Reduce(32),
                    MiddleName = order.OrderRecipient.Patronymic.Reduce(32),
                    EMail = order.OrderCustomer.Email.IsNotEmpty()
                        ? order.OrderCustomer.Email
                        : null,
                    PhoneNumbers = order.OrderRecipient.Phone.IsNotEmpty() 
                        ? new List<string> { order.OrderRecipient.Phone }
                        : null,
                }
            };

            var result = client.CreateVsdParcel(parcel);

            if (result != null && result.IsSuccess != true && result.ErrorMessage.IsNotEmpty())
                Errors.Add(result.ErrorMessage);

            return result != null && result.IsSuccess == true ? result.ParcelBarcode : null;
        }
    }
}
