using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping.ShippingYandexDelivery;
using Newtonsoft.Json;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("YandexDelivery")]
    public class YandexDeliveryShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        private bool _isActiveNow;
        public bool IsActive
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.IsActive).TryParseBool(); }
            set
            {
                _isActiveNow = true;
                Params.TryAddValue(YandexDeliveryTemplate.IsActive, value.ToString());
            }
        }

        private bool _isApiKeysAdded;

        public string ApiKeys
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.ApiKeys); }
            set
            {
                var apiKeys = value.DefaultOrEmpty();

                if (!string.IsNullOrWhiteSpace(apiKeys))
                {
                    try
                    {
                        var keys = JsonConvert.DeserializeObject<Dictionary<string, string>>(apiKeys);

                        SecretKeyDelivery = keys["searchDeliveryList"];
                        SecretKeyCreateOrder = keys["createOrder"];
                                              

                        _isApiKeysAdded = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                }

                Params.TryAddValue(YandexDeliveryTemplate.ApiKeys, apiKeys);
            }
        }

        //private bool _isApiDataAdded;
        public string ApiData
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.ApiData); }
            set
            {
                var apiData = value.DefaultOrEmpty();

                if (!string.IsNullOrWhiteSpace(apiData))
                {
                    try
                    {
                        var apiDataKeys = JsonConvert.DeserializeObject<YaDeliveryJsonConfigParams>(apiData);

                        ClientId = apiDataKeys.client.id;

                        var warehouse = apiDataKeys.warehouses.FirstOrDefault();
                        WarehouseId = warehouse != null ? warehouse.id : "";

                        var sender = apiDataKeys.senders.FirstOrDefault();
                        SenderId = sender != null ? sender.id : "";

                        var requisite = apiDataKeys.requisites.FirstOrDefault();
                        RequisiteId = requisite != null ? requisite.id : "";

                        CityFrom = "Москва";

                        //_isApiDataAdded = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                }

                Params.TryAddValue(YandexDeliveryTemplate.ApiData, apiData);
            }
        }
        
        public string SecretKeyDelivery
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.SecretKeyDelivery); }
            set { Params.TryAddValue(YandexDeliveryTemplate.SecretKeyDelivery, value.DefaultOrEmpty()); }
        }

        public string SecretKeyCreateOrder
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.SecretKeyCreateOrder); }
            set { Params.TryAddValue(YandexDeliveryTemplate.SecretKeyCreateOrder, value.DefaultOrEmpty()); }
        }
        
        public string WidgetCode
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.WidgetCode); }
            set
            {
                var widgetCode = value.DefaultOrEmpty();

                if (!string.IsNullOrWhiteSpace(widgetCode))
                {
                    // Save only url of widget like https://delivery.yandex.ru/widget/loader?resource_id=..&sid=..&key=..

                    var index = widgetCode.IndexOf("https://delivery.yandex.ru/widget/");
                    if (index != -1)
                        widgetCode = widgetCode.Substring(index).Replace("\"></script>", "");
                }

                Params.TryAddValue(YandexDeliveryTemplate.WidgetCode, widgetCode);
            }
        }

        public bool ShowAssessedValue
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.ShowAssessedValue).TryParseBool(); }
            set { Params.TryAddValue(YandexDeliveryTemplate.ShowAssessedValue, value.ToString()); }
        }

        public string ClientId
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.ClientId); }
            set { Params.TryAddValue(YandexDeliveryTemplate.ClientId, value.DefaultOrEmpty()); }
        }

        public string SenderId
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.SenderId); }
            set { Params.TryAddValue(YandexDeliveryTemplate.SenderId, value.DefaultOrEmpty()); }
        }

        public string WarehouseId
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.WarehouseId); }
            set { Params.TryAddValue(YandexDeliveryTemplate.WarehouseId, value.DefaultOrEmpty()); }
        }
        
        public string RequisiteId
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.RequisiteId); }
            set { Params.TryAddValue(YandexDeliveryTemplate.RequisiteId, value.DefaultOrEmpty()); }
        }

        public string CityFrom
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.CityFrom, "Москва"); }
            set { Params.TryAddValue(YandexDeliveryTemplate.CityFrom, value.DefaultOrEmpty()); }
        }

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
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.StatusesSync).TryParseBool(); }
            set { Params.TryAddValue(YandexDeliveryTemplate.StatusesSync, value.ToString()); }
        }

        public string Status_DRAFT
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_DRAFT); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_DRAFT, value.DefaultOrEmpty()); }
        }

        public string Status_CREATED
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_CREATED); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_CREATED, value.DefaultOrEmpty()); }
        }

        public string Status_SENDER_SENT
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_SENDER_SENT); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_SENDER_SENT, value.DefaultOrEmpty()); }
        }

        public string Status_DELIVERY_LOADED
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_LOADED); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_DELIVERY_LOADED, value.DefaultOrEmpty()); }
        }

        public string Status_SENDER_WAIT_FULFILMENT
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_SENDER_WAIT_FULFILMENT); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_SENDER_WAIT_FULFILMENT, value.DefaultOrEmpty()); }
        }

        public string Status_SENDER_WAIT_DELIVERY
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_SENDER_WAIT_DELIVERY); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_SENDER_WAIT_DELIVERY, value.DefaultOrEmpty()); }
        }

        public string Status_FULFILMENT_LOADED
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_FULFILMENT_LOADED); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_FULFILMENT_LOADED, value.DefaultOrEmpty()); }
        }

        public string Status_FULFILMENT_ARRIVED
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_FULFILMENT_ARRIVED); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_FULFILMENT_ARRIVED, value.DefaultOrEmpty()); }
        }

        public string Status_FULFILMENT_PREPARED
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_FULFILMENT_PREPARED); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_FULFILMENT_PREPARED, value.DefaultOrEmpty()); }
        }

        public string Status_FULFILMENT_TRANSMITTED
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_FULFILMENT_TRANSMITTED); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_FULFILMENT_TRANSMITTED, value.DefaultOrEmpty()); }
        }

        public string Status_DELIVERY_AT_START
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_AT_START); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_DELIVERY_AT_START, value.DefaultOrEmpty()); }
        }

        public string Status_DELIVERY_TRANSPORTATION
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_TRANSPORTATION); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_DELIVERY_TRANSPORTATION, value.DefaultOrEmpty()); }
        }

        public string Status_DELIVERY_ARRIVED
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_ARRIVED); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_DELIVERY_ARRIVED, value.DefaultOrEmpty()); }
        }

        public string Status_DELIVERY_TRANSPORTATION_RECIPIENT
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_TRANSPORTATION_RECIPIENT); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_DELIVERY_TRANSPORTATION_RECIPIENT, value.DefaultOrEmpty()); }
        }

        public string Status_DELIVERY_ARRIVED_PICKUP_POINT
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_ARRIVED_PICKUP_POINT); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_DELIVERY_ARRIVED_PICKUP_POINT, value.DefaultOrEmpty()); }
        }

        public string Status_DELIVERY_DELIVERED
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_DELIVERY_DELIVERED); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_DELIVERY_DELIVERED, value.DefaultOrEmpty()); }
        }

        public string Status_RETURN_PREPARING
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_RETURN_PREPARING); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_RETURN_PREPARING, value.DefaultOrEmpty()); }
        }

        public string Status_RETURN_ARRIVED_DELIVERY
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_RETURN_ARRIVED_DELIVERY); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_RETURN_ARRIVED_DELIVERY, value.DefaultOrEmpty()); }
        }

        public string Status_RETURN_ARRIVED_FULFILMENT
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_RETURN_ARRIVED_FULFILMENT); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_RETURN_ARRIVED_FULFILMENT, value.DefaultOrEmpty()); }
        }

        public string Status_RETURN_PREPARING_SENDER
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_RETURN_PREPARING_SENDER); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_RETURN_PREPARING_SENDER, value.DefaultOrEmpty()); }
        }

        public string Status_RETURN_RETURNED
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_RETURN_RETURNED); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_RETURN_RETURNED, value.DefaultOrEmpty()); }
        }

        public string Status_RETURN_TRANSMITTED_FULFILMENT
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_RETURN_TRANSMITTED_FULFILMENT); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_RETURN_TRANSMITTED_FULFILMENT, value.DefaultOrEmpty()); }
        }

        public string Status_LOST
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_LOST); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_LOST, value.DefaultOrEmpty()); }
        }

        public string Status_UNEXPECTED
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_UNEXPECTED); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_UNEXPECTED, value.DefaultOrEmpty()); }
        }

        public string Status_CANCELED
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_CANCELED); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_CANCELED, value.DefaultOrEmpty()); }
        }

        public string Status_ERROR
        {
            get { return Params.ElementOrDefault(YandexDeliveryTemplate.Status_ERROR); }
            set { Params.TryAddValue(YandexDeliveryTemplate.Status_ERROR, value.DefaultOrEmpty()); }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (_isActiveNow)
            {
                if (string.IsNullOrWhiteSpace(ApiKeys) || string.IsNullOrWhiteSpace(ApiData))
                    yield return new ValidationResult("Введите ключи");

                if (!_isApiKeysAdded)
                    IsActive = false;
            }
        }
    }
}
