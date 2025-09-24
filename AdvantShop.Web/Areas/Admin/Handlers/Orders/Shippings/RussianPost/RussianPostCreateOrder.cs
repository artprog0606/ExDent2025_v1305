using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Shipping.RussianPost;
using AdvantShop.Web.Admin.Models.Orders.RussianPost;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.RussianPost
{
    public class RussianPostCreateOrder
    {
        private readonly int _orderId;
        private readonly string _additionalAction;
        private readonly string _additionalActionData;

        public List<string> Errors { get; set; }

        public RussianPostCreateOrder(int orderId, string additionalAction, string additionalActionData)
        {
            _orderId = orderId;
            _additionalAction = additionalAction;
            _additionalActionData = additionalActionData;
            Errors = new List<string>();
        }

        public bool Execute(out string additionalActionResult, out object additionalActionDataResult)
        {
            additionalActionResult = null;
            additionalActionDataResult = null;
            
            var order = OrderService.GetOrder(_orderId);
            if (order is null)
            {
                Errors.Add("Заказ не найден");
                return false;
            }

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod is null
                || shippingMethod.ShippingType !=
                ((ShippingKeyAttribute) typeof(Shipping.RussianPost.RussianPost)
                                       .GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                Errors.Add("Неверный метод доставки");
                return false;
            }

            var calculationParameters =
                ShippingCalculationConfigurator.Configure()
                                               .ByOrder(order)
                                               .FromAdminArea()
                                               .Build();
            var russianPostMethod = new Shipping.RussianPost.RussianPost(shippingMethod, calculationParameters);
            
            List<CustomsDeclarationDto> declarationDtos = null;
            if (_additionalAction == Shipping.RussianPost.RussianPost.CustomsDeclarationProductsDataActionName 
                && _additionalActionData.IsNotEmpty())
            {
                var data = JsonConvert
                   .DeserializeObject<List<Models.Orders.RussianPost.CustomsDeclarationProductData>>(
                        _additionalActionData);

                if (data != null)
                {
                    declarationDtos =
                        data
                           .Select(customsDeclarationItem =>
                                new CustomsDeclarationDto
                                {
                                    OrderItemId = customsDeclarationItem.ItemId,
                                    ProductId = customsDeclarationItem.ProductId,
                                    Name = customsDeclarationItem.Name,
                                    ArtNo = customsDeclarationItem.ArtNo,
                                    CountryCode = customsDeclarationItem.CountryCode,
                                    TnvedCode = customsDeclarationItem.TnvedCode
                                })
                           .ToList();
                }
            }

            var result = russianPostMethod.UnloadOrder(
                order,
                _additionalAction.IsNotEmpty()
                    ? _additionalAction
                    : Shipping.RussianPost.RussianPost.FromAdminFirstActionName,
                declarationDtos);

            if (result.Success)
            {
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService,
                    shippingMethod.ShippingType);

                return true;
            }
            
            
            if (result.ErrorCode.IsNotEmpty())
            {
                additionalActionResult = result.ErrorCode;

                if (result.Object is IList<CustomsDeclarationDto> requestData)
                {
                    additionalActionDataResult = new CustomsDeclarationProductsDataModel
                    {
                        Products = requestData
                                  .Select(item =>
                                       new Models.Orders.RussianPost.CustomsDeclarationProductData
                                       {
                                           ItemId = item.OrderItemId,
                                           ProductId = item.ProductId,
                                           ArtNo = item.ArtNo,
                                           Name = item.Name,
                                           CountryCode = item.CountryCode,
                                           TnvedCode = item.TnvedCode
                                       })
                                  .ToList()
                    };
                }
                else
                    additionalActionDataResult = result.Object;
            }
            
            Errors.AddRange(result.Message.Split("\n"));
            
            return false;

        }

    }
}
