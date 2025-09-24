using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Shipping.Shiptor.Api
{
    public class ShiptorBaseObject<T>
        where T : class, new()
    {
        public ShiptorBaseObject(EnMethod method)
        {
            Method = method;
            Params = new T();
        }

        public string Id { get { return "AdvantShop"; } }

        [JsonProperty("jsonrpc")]
        public string JsonRpc
        {
            get { return "2.0"; }
        }

        [JsonProperty("wk")]
        public string ApiKey { get; set; }

        public EnMethod Method { get; private set; }

        public T Params { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnMethod
    {
        /// <summary>
        /// Метод, добавляющий или обновляющий существующий товар
        /// </summary>
        [EnumMember(Value = "setProduct")] SetProduct,
        /// <summary>
        /// Расчет стоимости доставки с настройками кабинета чекаута
        /// </summary>
        [EnumMember(Value = "simpleCalculate")] SimpleCalculate,
        /// <summary>
        /// Расчет стоимости доставки
        /// </summary>
        [EnumMember(Value = "calculateShipping")] CalculateShipping,
        /// <summary>
        /// Получение статуса посылки
        /// </summary>
        [EnumMember(Value = "getPackage")] GetPackage,
        /// <summary>
        /// Получение справочника населенных пунктов
        /// </summary>
        [EnumMember(Value = "getSettlements")] GetSettlements,
        /// <summary>
        /// Метод, добавляющий новый заказ в кабинет чекаута
        /// </summary>
        [EnumMember(Value = "addOrder")] AddOrder,
        /// <summary>
        /// Получение статусов посылок с их описанием
        /// </summary>
        [EnumMember(Value = "getStatusList")] GetStatusList,
        /// <summary>
        /// Поиск населенного пункта
        /// </summary>
        [EnumMember(Value = "suggestSettlement")] SuggestSettlement,
        /// <summary>
        /// Поиск населенного пункта
        /// </summary>
        [EnumMember(Value = "simpleSuggestSettlement")] SimpleSuggestSettlement,
    }
}
