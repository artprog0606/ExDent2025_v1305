using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Diagnostics;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Shipping.Yandex;
using AdvantShop.Shipping.Yandex.Api;
using AdvantShop.Taxes;
using AdvantShop.Web.Admin.Models.Orders.Yandex;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Yandex
{
    public class CreateYandexOrder
    {
        private readonly int _orderId;
        private readonly string _additionalAction;
        private readonly string _additionalActionData;

        public List<string> Errors { get; set; }

        public CreateYandexOrder(int orderId, string additionalAction, string additionalActionData)
        {
            _orderId = orderId;
            _additionalActionData = additionalActionData;
            _additionalAction = additionalAction;
            Errors = new List<string>();
        }

        public bool Execute(out string additionalActionResult, out object additionalActionDataResult)
        {
            var yandexRequestId = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Yandex.YandexDelivery.KeyNameYandexRequestIdInOrderAdditionalData);

            additionalActionResult = null;
            additionalActionDataResult = null;

            if (!string.IsNullOrEmpty(yandexRequestId))
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
                typeof(Shipping.Yandex.YandexDelivery).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                Errors.Add("Неверный метод доставки");
                return false;
            }

            var calculationParameters =
                ShippingCalculationConfigurator.Configure()
                                               .ByOrder(order)
                                               .FromAdminArea()
                                               .Build();
            var yandexMethod = new Shipping.Yandex.YandexDelivery(order.ShippingMethod, calculationParameters);
            
            ChangeDeliveryDateDto dto = null;
            if (_additionalAction == Shipping.Yandex.YandexDelivery.KeySelectValidDeliveryDate 
                && _additionalActionData.IsNotEmpty())
            {
                var data = JsonConvert.DeserializeObject<ChangeDeliveryDateModel>(_additionalActionData);

                dto = data?.ToDto();
            }

            bool? modifyArtNo = null;
            if (_additionalAction == Shipping.Yandex.YandexDelivery.KeyConfirmModifyArtNo ) 
                modifyArtNo = _additionalActionData.TryParseBool();
        
            var result = yandexMethod.UnloadOrder(order, _additionalAction, (object)dto ?? modifyArtNo);
            
            
            if (result.Success)
            {
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService,
                    order.ShippingMethod.ShippingType);

                return true;
            }
            
            
            if (result.ErrorCode.IsNotEmpty())
            {
                additionalActionResult = result.ErrorCode;
                additionalActionDataResult = result.Object;
            }
            
            Errors.AddRange(result.Message.Split("\n"));
            
            return false;
        }
    }
}