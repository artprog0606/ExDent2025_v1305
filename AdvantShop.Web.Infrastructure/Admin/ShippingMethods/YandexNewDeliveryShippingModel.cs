using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Orders;
using AdvantShop.Shipping.ShippingYandexNewDelivery;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("YandexNewDelivery")]
    public class YandexNewDeliveryShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string AuthorizationToken
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.AuthorizationToken); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.AuthorizationToken, value); }
        }

        public string ShopId
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.ShopId); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.ShopId, value); }
        }

        public string WarehouseId
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.WarehouseId); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.WarehouseId, value); }
        }

        public string CityFrom
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.CityFrom); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.CityFrom, value); }
        }

        public string WidgetApiKey
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.WidgetApiKey); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.WidgetApiKey, value); }
        }

        public string TypeViewPoints
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.TypeViewPoints, ((int)AdvantShop.Shipping.ShippingYandexNewDelivery.TypeViewPoints.WidgetYandexDelivery).ToString()); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.TypeViewPoints, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ListTypesViewPoints
        {
            get
            {
                return Enum.GetValues(typeof(TypeViewPoints))
                    .Cast<TypeViewPoints>()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = ((int)x).ToString()
                    })
                    .ToList();
            }
        }

        public string YaMapsApiKey
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        public string PriceDisplayType
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.PriceDisplayType, ((int)AdvantShop.Shipping.ShippingYandexNewDelivery.PriceDisplayType.ForCustomer).ToString()); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.PriceDisplayType, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ListPriceDisplayType
        {
            get
            {
                return Enum.GetValues(typeof(PriceDisplayType))
                    .Cast<PriceDisplayType>()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = ((int)x).ToString()
                    })
                    .ToList();
            }
        }

        public string ExportOrderExternalIdType
        {
            get
            {
                return Params.ElementOrDefault(YandexNewDeliveryTemplate.ExportOrderExternalIdType,
                    ((int) EExportOrderExternalIdType.OrderId)
                    .ToString());
            }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.ExportOrderExternalIdType, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ListExportOrderExternalIdType
        {
            get
            {
                return Enum.GetValues(typeof(EExportOrderExternalIdType)).Cast<EExportOrderExternalIdType>().Select(x =>
                    new SelectListItem() {Text = x.Localize(), Value = ((int) x).ToString()}).ToList();
            }
        }

        public string CourierOptionName
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.CourierOptionName, "(Курьер)");}
            set { Params.TryAddValue(YandexNewDeliveryTemplate.CourierOptionName, value); }
        }
        public string PickupOptionName
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.PickupOptionName, "(Самовывоз)");}
            set { Params.TryAddValue(YandexNewDeliveryTemplate.PickupOptionName, value); }
        }
        
        #region Statuses

        public List<SelectListItem> ListStatuses
        {
            get
            {
                var statuses = OrderStatusService.GetOrderStatuses()
                    .Select(x => new SelectListItem() { Text = x.StatusName, Value = x.StatusID.ToString() }).ToList();

                statuses.Insert(0, new SelectListItem() { Text = "", Value = "" });

                return statuses;
            }
        }

        public bool StatusesSync
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.StatusesSync).TryParseBool(); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.StatusesSync, value.ToString()); }
        }
        public string Status_DRAFT
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DRAFT); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DRAFT, value.DefaultOrEmpty()); }
        }
        public string Status_VALIDATING
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_VALIDATING); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_VALIDATING, value.DefaultOrEmpty()); }
        }
        public string Status_VALIDATING_ERROR
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_VALIDATING_ERROR); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_VALIDATING_ERROR, value.DefaultOrEmpty()); }
        }
        public string Status_CREATED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_CREATED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_CREATED, value.DefaultOrEmpty()); }
        }
        public string Status_SENDER_SENT
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SENDER_SENT); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SENDER_SENT, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_LOADED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_LOADED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_LOADED, value.DefaultOrEmpty()); }
        }
        public string Status_SENDER_WAIT_FULFILMENT
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SENDER_WAIT_FULFILMENT); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SENDER_WAIT_FULFILMENT, value.DefaultOrEmpty()); }
        }
        public string Status_SENDER_WAIT_DELIVERY
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SENDER_WAIT_DELIVERY); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SENDER_WAIT_DELIVERY, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_AT_START
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_AT_START); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_AT_START, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_AT_START_SORT
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_AT_START_SORT); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_AT_START_SORT, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_TRANSPORTATION
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_TRANSPORTATION); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_TRANSPORTATION, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_ARRIVED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_ARRIVED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_ARRIVED, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_TRANSPORTATION_RECIPIENT
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_TRANSPORTATION_RECIPIENT); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_TRANSPORTATION_RECIPIENT, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_CUSTOMS_ARRIVED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_CUSTOMS_ARRIVED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_CUSTOMS_ARRIVED, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_CUSTOMS_CLEARED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_CUSTOMS_CLEARED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_CUSTOMS_CLEARED, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_STORAGE_PERIOD_EXTENDED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_STORAGE_PERIOD_EXTENDED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_STORAGE_PERIOD_EXTENDED, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_STORAGE_PERIOD_EXPIRED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_STORAGE_PERIOD_EXPIRED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_STORAGE_PERIOD_EXPIRED, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_UPDATED_BY_SHOP
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_UPDATED_BY_SHOP); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_UPDATED_BY_SHOP, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_UPDATED_BY_RECIPIENT
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_UPDATED_BY_RECIPIENT); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_UPDATED_BY_RECIPIENT, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_UPDATED_BY_DELIVERY
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_UPDATED_BY_DELIVERY); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_UPDATED_BY_DELIVERY, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_ARRIVED_PICKUP_POINT
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_ARRIVED_PICKUP_POINT); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_ARRIVED_PICKUP_POINT, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_TRANSMITTED_TO_RECIPIENT
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_TRANSMITTED_TO_RECIPIENT); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_TRANSMITTED_TO_RECIPIENT, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_DELIVERED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_DELIVERED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_DELIVERED, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_ATTEMPT_FAILED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_ATTEMPT_FAILED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_ATTEMPT_FAILED, value.DefaultOrEmpty()); }
        }
        public string Status_DELIVERY_CAN_NOT_BE_COMPLETED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_CAN_NOT_BE_COMPLETED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_DELIVERY_CAN_NOT_BE_COMPLETED, value.DefaultOrEmpty()); }
        }
        public string Status_RETURN_STARTED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_RETURN_STARTED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_RETURN_STARTED, value.DefaultOrEmpty()); }
        }
        public string Status_RETURN_PREPARING
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_RETURN_PREPARING); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_RETURN_PREPARING, value.DefaultOrEmpty()); }
        }
        public string Status_RETURN_ARRIVED_DELIVERY
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_RETURN_ARRIVED_DELIVERY); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_RETURN_ARRIVED_DELIVERY, value.DefaultOrEmpty()); }
        }
        public string Status_RETURN_TRANSMITTED_FULFILLMENT
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_RETURN_TRANSMITTED_FULFILLMENT); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_RETURN_TRANSMITTED_FULFILLMENT, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_CREATED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_CREATED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_CREATED, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_LOADED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_LOADED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_LOADED, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_AT_START
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_AT_START); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_AT_START, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_OUT_OF_STOCK
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_OUT_OF_STOCK); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_OUT_OF_STOCK, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_AWAITING_CLARIFICATION
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_AWAITING_CLARIFICATION); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_AWAITING_CLARIFICATION, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_PREPARED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_PREPARED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_PREPARED, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_TRANSMITTED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_TRANSMITTED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_TRANSMITTED, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_RETURN_PREPARING
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_PREPARING); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_PREPARING, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_RETURN_RFF_PREPARING_FULFILLMENT
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_RFF_PREPARING_FULFILLMENT); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_RFF_PREPARING_FULFILLMENT, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_RETURN_RFF_TRANSMITTED_FULFILLMENT
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_RFF_TRANSMITTED_FULFILLMENT); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_RFF_TRANSMITTED_FULFILLMENT, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_RETURN_RFF_ARRIVED_FULFILLMENT
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_RFF_ARRIVED_FULFILLMENT); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_RFF_ARRIVED_FULFILLMENT, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_RETURN_ARRIVED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_ARRIVED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_ARRIVED, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_RETURN_PREPARING_SENDER
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_PREPARING_SENDER); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_PREPARING_SENDER, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_RETURN_TRANSFERRED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_TRANSFERRED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_TRANSFERRED, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_RETURN_RETURNED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_RETURNED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_RETURNED, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_CANCELED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_CANCELED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_CANCELED, value.DefaultOrEmpty()); }
        }
        public string Status_SORTING_CENTER_ERROR
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_ERROR); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_SORTING_CENTER_ERROR, value.DefaultOrEmpty()); }
        }
        public string Status_LOST
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_LOST); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_LOST, value.DefaultOrEmpty()); }
        }
        public string Status_CANCELLED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_CANCELLED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_CANCELLED, value.DefaultOrEmpty()); }
        }
        public string Status_CANCELLED_USER
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_CANCELLED_USER); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_CANCELLED_USER, value.DefaultOrEmpty()); }
        }
        public string Status_FINISHED
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_FINISHED); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_FINISHED, value.DefaultOrEmpty()); }
        }
        public string Status_ERROR
        {
            get { return Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_ERROR); }
            set { Params.TryAddValue(YandexNewDeliveryTemplate.Status_ERROR, value.DefaultOrEmpty()); }
        }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(AuthorizationToken))
                yield return new ValidationResult("Введите токен авторизации");

            if (string.IsNullOrWhiteSpace(ShopId))
                yield return new ValidationResult("Введите идентификатор магазина");

            if (string.IsNullOrWhiteSpace(WarehouseId))
                yield return new ValidationResult("Введите идентификатор склада");

            if (ShopId.TryParseInt() == 0)
                yield return new ValidationResult("Не верно указан идентификатор магазина");

            if (TypeViewPoints == ((int)AdvantShop.Shipping.ShippingYandexNewDelivery.TypeViewPoints.WidgetYandexDelivery).ToString() && string.IsNullOrWhiteSpace(WidgetApiKey))
                yield return new ValidationResult("Введите api ключ виджета");

            if (TypeViewPoints == ((int)AdvantShop.Shipping.ShippingYandexNewDelivery.TypeViewPoints.YaWidget).ToString() && string.IsNullOrWhiteSpace(YaMapsApiKey))
                yield return new ValidationResult("Введите api ключ яндекс.карт");

            if (string.IsNullOrWhiteSpace(CityFrom))
                yield return new ValidationResult("Укажите город из которого производится доставка");
        }
    }
}
