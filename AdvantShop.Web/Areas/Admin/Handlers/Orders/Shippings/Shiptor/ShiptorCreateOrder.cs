using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Shipping;
using AdvantShop.Shipping.Shiptor;
using AdvantShop.Shipping.Shiptor.Api;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Shiptor
{
    public class ShiptorCreateOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public ShiptorCreateOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId, Shipping.Shiptor.Shiptor.KeyNameOrderShiptorIdInOrderAdditionalData);
            if (!string.IsNullOrEmpty(orderAdditionalData))
            {
                Errors.Add("Заказ уже передан");
                return false;
            }
            
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
            {
                Errors.Add("Заказ не найден");
                return false;
            }

            if (order.ShippingMethod == null || order.ShippingMethod.ShippingType != ((ShippingKeyAttribute)
                typeof(Shipping.Shiptor.Shiptor).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                Errors.Add("Неверный метод доставки");
                return false;
            }

            var calculationParameters =
                ShippingCalculationConfigurator.Configure()
                                               .ByOrder(order)
                                               .FromAdminArea()
                                               .Build();

            var shiptorShipMethod = new Shipping.Shiptor.Shiptor(order.ShippingMethod, calculationParameters);
            if (order.OrderCustomer == null)
            {
                Errors.Add("Отсутствуют данные пользователя");
                return false;
            }

            var paymentsCash = new[]
            {
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.CashOnDelivery)),
            };
            var shiptorEventWidgetData = order.OrderPickPoint != null &&
                                         order.OrderPickPoint.AdditionalData.IsNotEmpty()
                ? JsonConvert.DeserializeObject<ShiptorEventWidgetData>(order.OrderPickPoint.AdditionalData)
                : null;

            if (order.OrderPickPoint == null && shiptorEventWidgetData == null)
            {
                Errors.Add("Отсутствуют дополнительные данные доставки");
                return false;
            }

            var orderSum = order.Sum;
            var shippingCost = order.ShippingCostWithDiscount;
            var shippingCurrency = order.ShippingMethod.ShippingCurrency;

            if (shippingCurrency != null)
            {
                // Конвертируем в валюту доставки
                shippingCost = shippingCost.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                orderSum = orderSum.ConvertCurrency(order.OrderCurrency, shippingCurrency);
            }

            var orderItems = order.GetOrderItemsWithDiscountsAndFee()
                                  .AcceptableDifference(0.1f)
                                  .WithCurrency(shippingCurrency)
                                  .CeilingAmountToInteger()
                                  .GetItems();

            AddProducts(orderItems as List<OrderItem>, shiptorShipMethod);

            var dimensions = shiptorShipMethod.GetDimensions(10);

            var addOrderParams = new AddOrderParams
            {
                OrderNumber = order.Number,
                Height = dimensions[0],
                Width = dimensions[1],
                Length = dimensions[2],
                Weight = shiptorShipMethod.GetTotalWeight(),
                ProductsPrice = orderSum - shippingCost,
                DeclaredCost = shiptorShipMethod.WithInsure ? orderSum - shippingCost : 10f,
                ShippingCost = shippingCost,
                IsPayed = order.Payed,
                Departure = new AddOrderDeparture
                {
                    Comment = order.CustomerComment,
                    ShippingMethodId = shiptorEventWidgetData.ShippingMethod
                },
                Products = orderItems.Select(x => new AddOrderProduct
                {
                    ArtNo = x.ArtNo,
                    Count = (int) x.Amount,
                    Price = x.Price
                }).ToList()
            };

            if (!order.Payed &&
                (order.PaymentMethod != null &&
                    paymentsCash.Contains(order.PaymentMethod.PaymentKey)))
            {
                addOrderParams.CashOnDelivery = true;
                addOrderParams.DeclaredCost = orderSum - shippingCost;
                addOrderParams.PaymentType =
                    order.PaymentMethod.PaymentMethodId == shiptorShipMethod.PaymentCodCardId
                        ? EnPaymentType.Card
                        : EnPaymentType.Cash;
            }
            else
                addOrderParams.CashOnDelivery = false;

            var pointId = order.OrderPickPoint.PickPointId.TryParseInt(true);

            addOrderParams.Departure.DeliveryPointId = pointId.HasValue
                ? pointId.Value.ToString()
                : null;

            // делаем замену только для поиска, сами данные пользователя не изменяем
            string outCountry, outRegion, outDistrict, outCity, outZip;
            ShippingReplaceGeoService.ReplaceGeo(
                    order.ShippingMethod.ShippingType,
                    order.OrderCustomer.Country, order.OrderCustomer.Region, order.OrderCustomer.District, order.OrderCustomer.City, order.OrderCustomer.Zip,
                    out outCountry, out outRegion, out outDistrict, out outCity, out outZip);

            string countryIso2 = "RU";
            if (outCountry.IsNotEmpty())
            {
                var country = CountryService.GetCountryByName(outCountry);
                if (country != null)
                    countryIso2 = country.Iso2;
            }

            var suggests =
                shiptorShipMethod.ShiptorCheckoutApiService.SuggestSettlement(string.Format("{0} {1}", outRegion, outCity), countryIso2);
            var suggestCity = suggests != null && suggests.Count > 0
                ? suggests.FirstOrDefault(
                    x => x.Name.IndexOf(outCity, StringComparison.OrdinalIgnoreCase) != -1)
                : null;

            var kladrKodCity = suggestCity != null
                ? suggestCity.KladrId
                : null;

            addOrderParams.Departure.Address = new AddOrderAddress
            {
                FistName = order.OrderRecipient.FirstName,
                LastName = order.OrderRecipient.LastName,
                Patronymic = order.OrderRecipient.Patronymic,
                Email = order.OrderCustomer.Email,
                Phone = order.OrderRecipient.StandardPhone.ToString(),
                PostalCode = order.OrderCustomer.Zip,
                CountryIso2 = countryIso2,
                Settlement = order.OrderCustomer.City,
                KladrId = kladrKodCity,
                Street = !pointId.HasValue ? order.OrderCustomer.Street : null,
                House = !pointId.HasValue
                    ? string.Format("{0}{2}{1}",
                        order.OrderCustomer.House,
                        order.OrderCustomer.Structure,
                        order.OrderCustomer.Structure.IsNotEmpty() ? "/" : string.Empty)
                    : null,
                Apartment = !pointId.HasValue ? order.OrderCustomer.Apartment : null,
            };

            var resultShiptorAddOrder =
                shiptorShipMethod.ShiptorCheckoutApiService.AddOrder(addOrderParams);

            if (resultShiptorAddOrder != null)
            {
                OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                    Shipping.Shiptor.Shiptor.KeyNameOrderShiptorIdInOrderAdditionalData,
                    resultShiptorAddOrder.Id.ToString());

                var packageShiptor =
                    shiptorShipMethod.ShiptorCheckoutApiService.GetPackage(new GetPackageParams
                    {
                        Id = resultShiptorAddOrder.Id
                    });

                if (packageShiptor != null && packageShiptor.ExternalTrackingNumber.IsNotEmpty())
                {
                    order.TrackNumber = packageShiptor.ExternalTrackingNumber;
                    OrderService.UpdateOrderMain(order);
                }

                Track.TrackService.TrackEvent(
                    Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService,
                    order.ShippingMethod.ShippingType);

                return true;
            }
            else if (shiptorShipMethod.ShiptorCheckoutApiService.LastActionErrors != null)
                Errors.AddRange(shiptorShipMethod.ShiptorCheckoutApiService.LastActionErrors);
            else
                Errors.Add("Не удалось добавить заказ");

            return false;
        }

        private void AddProducts(List<OrderItem> orderItems, Shipping.Shiptor.Shiptor shiptorShipMethod)
        {
            foreach(var orderItem in orderItems)
            {
                var product = orderItem.ProductID.HasValue ? ProductService.GetProduct(orderItem.ProductID.Value) : null;
                shiptorShipMethod.ShiptorCheckoutApiService.SetProduct(new SetProductParams
                {
                    ShopArtNo = orderItem.ArtNo,
                    ArtNo = orderItem.ArtNo,
                    Name = string.Join(" ", new[] { orderItem.Name, orderItem.Color, orderItem.Size }),
                    RetailPrice = orderItem.Price,
                    SupplyPrice = orderItem.SupplyPrice,
                    Height = orderItem.Height,
                    Width = orderItem.Width,
                    Length = orderItem.Length,
                    Weight = orderItem.Weight,
                    Barcode = orderItem.BarCode,
                    Brand = product != null && product.Brand != null ? product.Brand.Name : null,
                    Adult = product != null ? product.ExportOptions.Adult : (bool?)null,
                    Url = product != null ? Core.UrlRewriter.UrlService.GetLink(Core.UrlRewriter.ParamType.Product, product.UrlPath, product.ProductId) : null
                });

                if (shiptorShipMethod.ShiptorCheckoutApiService.LastActionErrors != null)
                    Errors.AddRange(shiptorShipMethod.ShiptorCheckoutApiService.LastActionErrors);
            }
        }
    }
}
