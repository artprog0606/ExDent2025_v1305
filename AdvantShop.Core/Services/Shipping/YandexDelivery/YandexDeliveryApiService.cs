using System.Web;
using System.Linq;
using System.Collections.Generic;

using AdvantShop.Shipping.ShippingYandexDelivery;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;

using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Shipping.YandexDelivery
{
    public class YandexDeliveryApiService
    {
        private const string Url = "https://delivery.yandex.ru/api/last";

        private readonly string _clientId;
        private readonly string _cityFrom;
        private readonly string _senderId;
        private readonly string _warehouseId;
        private readonly string _requisiteId;

        //private readonly string _secretKeyAutocomplete = "32b6b68cda51e6057cd57834ade60f474f70eb2e786634a168d5204e1dd1f097";

        private readonly Dictionary<string, string> _yandexDeliveryKeys;

        public YandexDeliveryApiService(string clientId, string senderId, string cityFrom, string warehouseId, string requisiteId, Dictionary<string, string> yandexDeliveryKeys)
        {
            _clientId = clientId;
            _senderId = senderId;
            _cityFrom = cityFrom;
            _warehouseId = warehouseId;
            _requisiteId = requisiteId;
            _yandexDeliveryKeys = yandexDeliveryKeys;
        }

        private enum EApiMethod
        {
            [StringName("getPaymentMethods")]
            GetPaymentMethods,
            [StringName("getSenderOrders")]
            GetSenderOrders,
            [StringName("getSenderOrderLabel")]
            GetSenderOrderLabel,
            [StringName("getSenderParcelDocs")]
            GetSenderParcelDocs,
            [StringName("autocomplete")]
            Autocomplete,
            [StringName("getIndex")]
            GetIndex,
            [StringName("createOrder")]
            CreateOrder,
            [StringName("updateOrder")]
            UpdateOrder,
            [StringName("deleteOrder")]
            DeleteOrder,
            [StringName("getSenderOrderStatus")]
            GetSenderOrderStatus,
            [StringName("getSenderOrderStatuses")]
            GetSenderOrderStatuses,
            [StringName("getSenderInfo")]
            GetSenderInfo,
            [StringName("getWarehouseInfo")]
            GetWarehouseInfo,
            [StringName("getRequisiteInfo")]
            GetRequisiteInfo,
            [StringName("getIntervals")]
            GetIntervals,
            [StringName("createWithdraw")]
            CreateWithdraw,
            [StringName("confirmSenderOrders")]
            ConfirmSenderOrders,
            [StringName("updateWithdraw")]
            UpdateWithdraw,
            [StringName("createImport")]
            CreateImport,
            [StringName("updateImport")]
            UpdateImport,
            [StringName("getDeliveries")]
            GetDeliveries,
            [StringName("getOrderInfo")]
            GetOrderInfo,
            [StringName("searchDeliveryList")]
            SearchDeliveryList,
            [StringName("confirmSenderParcels")]
            ConfirmSenderParcels,
            [StringName("searchSenderOrdersStatuses")]
            SearchSenderOrdersStatuses,
            [StringName("getImports")]
            GetImports,
            [StringName("getWithdraws")]
            GetWithdraws,
            [StringName("cancelImport")]
            CancelImport,
            [StringName("cancelWithdraw")]
            CancelWithdraw
        }

        private enum ETermType
        {
            [StringName("address")]
            Address,
            [StringName("locality")]
            Locality,
            [StringName("street")]
            Street,
            [StringName("house")]
            House,
            [StringName("index")]
            Index,
            [StringName("goods")]
            Goods,
        }

        public string GetCityGeoId(string cityName)
        {
            /*
             *  secret_key=<секретный ключ>
                &client_id=<идентификатор аккаунта в Яндекс.Доставке>
                &sender_id=<идентификатор магазина>
                &type=<объект дополнения>
                &term=<строка для дополнения>
                &locality_name=<название города>
                &street=<название улицы>
             */
            if (_yandexDeliveryKeys == null)
            {
                return string.Empty;
            }

            if (!_yandexDeliveryKeys.ContainsKey(EApiMethod.Autocomplete.StrName()))
            {
                return string.Empty;
            }

            var methodApiKey = _yandexDeliveryKeys[EApiMethod.Autocomplete.StrName()];
            
            var data = new Dictionary<string, object>()
            {
                {"client_id", _clientId},
                {"sender_id", _senderId},
                {"type", ETermType.Locality.StrName()},
                {"term", cityName}
            };

            data.Add("secret_key", YandexDeliveryService.GetSign(data, methodApiKey));
            var postDataString = data.Keys.Aggregate("", (current, t) => current + string.Format("&{0}={1}", t, HttpUtility.UrlEncode(data[t].ToString())));

            var responseData = YandexDeliveryService.MakeRequest(Url + "/autocomplete", postDataString);

            var response = JsonConvert.DeserializeObject<YandexDeliveryResponse<YandexDeliveryResponseAutocomplete>>(responseData);

            return response != null && response.Status == "ok" && response.Data != null && response.Data.Suggestions != null && response.Data.Suggestions.Count > 0
                ? response.Data.Suggestions[0].GeoId
                : string.Empty;
        }

        public string GetOrderStatus(string yaOrderId)
        {
            if (_yandexDeliveryKeys == null)
            {
                return null;
            }

            if (!_yandexDeliveryKeys.ContainsKey(EApiMethod.GetSenderOrderStatus.StrName()))
            {
                return null;
            }

            var methodApiKey = _yandexDeliveryKeys[EApiMethod.GetSenderOrderStatus.StrName()];

            var data = new Dictionary<string, object>()
            {
                {"client_id", _clientId},
                {"sender_id", _senderId},
                {"order_id", yaOrderId}
            };

            data.Add("secret_key", YandexDeliveryService.GetSign(data, methodApiKey));
            var postDataString = data.Keys.Aggregate("", (current, t) => current + string.Format("&{0}={1}", t, HttpUtility.UrlEncode(data[t].ToString())));

            var responseData = YandexDeliveryService.MakeRequest(Url + "/getSenderOrderStatus", postDataString);

            var response = JsonConvert.DeserializeObject<YandexDeliveryResponse<string>>(responseData);

            return response != null && response.Status == "ok" && response.Data != null
                ? response.Data
                : null;
        }

        public List<YandexOrderStatus> GetOrderStatuses(string yaOrderId)
        {
            if (_yandexDeliveryKeys == null)
            {
                return null;
            }

            if (!_yandexDeliveryKeys.ContainsKey(EApiMethod.GetSenderOrderStatuses.StrName()))
            {
                return null;
            }

            var methodApiKey = _yandexDeliveryKeys[EApiMethod.GetSenderOrderStatuses.StrName()];

            var data = new Dictionary<string, object>()
            {
                {"client_id", _clientId},
                {"sender_id", _senderId},
                {"order_id", yaOrderId}
            };

            data.Add("secret_key", YandexDeliveryService.GetSign(data, methodApiKey));
            var postDataString = data.Keys.Aggregate("", (current, t) => current + string.Format("&{0}={1}", t, HttpUtility.UrlEncode(data[t].ToString())));

            var responseData = YandexDeliveryService.MakeRequest(Url + "/getSenderOrderStatuses", postDataString);

            var response = JsonConvert.DeserializeObject<YandexDeliveryResponse<GetOrderStatuses>>(responseData);

            return response != null && response.Status == "ok" && response.Data != null && response.Data.Data != null
                ? response.Data.Data
                : null;
        }

        public List<YandexDeliveryDto> GetDeliveries()
        {
            if (_yandexDeliveryKeys == null)
            {
                return null;
            }

            if (!_yandexDeliveryKeys.ContainsKey(EApiMethod.GetDeliveries.StrName()))
            {
                return null;
            }

            var methodApiKey = _yandexDeliveryKeys[EApiMethod.GetDeliveries.StrName()];

            var data = new Dictionary<string, object>()
            {
                {"client_id", _clientId},
                {"sender_id", _senderId},
            };

            data.Add("secret_key", YandexDeliveryService.GetSign(data, methodApiKey));
            var postDataString = data.Keys.Aggregate("", (current, t) => current + string.Format("&{0}={1}", t, HttpUtility.UrlEncode(data[t].ToString())));

            var responseData = YandexDeliveryService.MakeRequest(Url + "/getDeliveries", postDataString);

            var response = JsonConvert.DeserializeObject<YandexDeliveryResponse<GetDeliveries>>(responseData);

            return response != null && response.Status == "ok" && response.Data != null && response.Data.Deliveries != null
                ? response.Data.Deliveries
                : null;
        }
    }
}
