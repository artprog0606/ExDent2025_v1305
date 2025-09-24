using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Core.Services.Crm.OzonSeller.Api
{
    public class ProductsImport
    {
        /// <summary>
        /// Список товаров
        /// <para>required</para>
        /// </summary>
        public List<ProductImport> Items { get; set; }
    }

    public class ProductImport
    {
        /// <summary>
        /// Штрихкод товара.
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Идентификатор категории
        /// <para>required</para>
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Называние товара
        /// <para>required</para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор товара в системе продавца
        /// <para>required</para>
        /// </summary>
        public string OfferId { get; set; }

        /// <summary>
        /// Цена товара с учетом скидок, отображается на карточке товара
        /// <para>required</para>
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Цена до скидок (будет зачеркнута на карточке товара). Указывается в рублях. Разделитель дробной части — точка, до двух знаков после точки.
        /// </summary>
        public string OldPrice { get; set; }

        /// <summary>
        /// Цена для клиентов с подпиской Ozon Premium
        /// </summary>
        public string PremiumPrice { get; set; }

        /// <summary>
        /// Ставка НДС для товара:
        /// <para>0 — не облагается НДС;</para>
        /// <para>0.1 — 10%;</para>
        /// <para>0.2 — 20%.</para>
        /// <para>required</para>
        /// </summary>
        public string Vat { get; set; }

        /// <summary>
        /// Вес товара в упаковке
        /// <para>required</para>
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Единицы измерения веса. Доступные варианты: g (граммы), kg (килограммы), lb (фунты).
        /// </summary>
        /// <para>required</para>
        public WeightUnit WeightUnit { get; set; }

        /// <summary>
        /// Глубина упаковки
        /// <para>required</para>
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Высота упаковки
        /// <para>required</para>
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Ширина упаковки
        /// <para>required</para>
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Единица измерения габаритов. Доступные варианты: mm (миллиметры), cm (сантиметры), in (дюймы).
        /// <para>required</para>
        /// </summary>
        public DimensionUnit DimensionUnit { get; set; }

        /// <summary>
        /// Идентификатор группы изображений
        /// </summary>
        public string ImageGroupId { get; set; }

        /// <summary>
        /// Массив с url изображений, не больше 10
        /// <para>required</para>
        /// </summary>
        public List<Uri> Images { get; set; }

        /// <summary>
        /// Масиив изображений 360
        /// </summary>
        public List<Uri> Images360 { get; set; }

        /// <summary>
        /// Массив характеристик товара
        /// <para>required</para>
        /// </summary>
        public List<ProductImportAttribute> Attributes { get; set; }

        /// <summary>
        /// Массив характеристик, у которых есть вложенные аттрибуты
        /// </summary>
        public List<ProductImportComplexAttribute> ComplexAttributes { get; set; }

        /// <summary>
        /// Список pdf-файлов
        /// </summary>
        public List<ProductImportPdfList> PdfList { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WeightUnit
    {
        [EnumMember(Value = "g")]
        Grams,

        [EnumMember(Value = "kg")]
        Kilograms,

        [EnumMember(Value = "lb")]
        Pounds
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DimensionUnit
    {
        [EnumMember(Value = "mm")]
        Millimeters,

        [EnumMember(Value = "cm")]
        Centimeters,

        [EnumMember(Value = "in")]
        Inches
    }

    public class ProductImportAttribute
    {
        /// <summary>
        /// Идентификатор словаря
        /// </summary>
        public int ComplexId { get; set; }

        /// <summary>
        /// Идентификатор характеристики
        /// <para>required</para>
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Массив вложенных значений характеристики
        /// </summary>
        public List<ProductImportAttributeValue> Values { get; set; }
    }

    public class ProductImportAttributeValue
    {
        /// <summary>
        /// Идентификатор справочника
        /// </summary>
        public int DictionaryValueId { get; set; }

        /// <summary>
        /// Значение из справочника
        /// </summary>
        public string Value { get; set; }
    }

    public class ProductImportComplexAttribute
    {
        public List<ProductImportAttribute> Attributes { get; set; }
    }

    public class ProductImportPdfList
    {
        public int Index { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// URL для pdf-файла
        /// </summary>
        public string SrcUrl { get; set; }
    }

    public class ImportProductsResult
    {
        /// <summary>
        /// Код задачи на импорт товаров
        /// </summary>
        public int TaskId { get; set; }
    }

    public class StatusImportProducts
    {
        public List<StatusImportProductItem> Items { get; set; }
        public int Total { get; set; }
    }

    public class StatusImportProductItem
    {
        /// <summary>
        /// Идентификатор товара в системе продавца
        /// </summary>
        public string OfferId { get; set; }

        /// <summary>
        /// Идентификатор товара
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Статус создания товара
        /// </summary>
        public StatusImportProduct Status { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter), converterParameters: true /*camelCaseText*/)]
    public enum StatusImportProduct
    {
        /// <summary>
        /// информация о товаре добавляется в систему, ожидайте
        /// </summary>
        Processing,

        /// <summary>
        /// информация обновлена
        /// </summary>
        Processed,

        /// <summary>
        /// товар проходит модерацию, ожидайте
        /// </summary>
        Moderating,

        /// <summary>
        ///  товар не прошел модерацию
        /// </summary>
        FailedModeration,

        /// <summary>
        /// товар не прошел валидацию
        /// </summary>
        FailedValidation,

        /// <summary>
        /// возникла ошибка
        /// </summary>
        Failed
    }

    public class ProductSetPrepaymentParams
    {
        /// <summary>
        /// Флаг обязательной предоплаты для товара
        /// </summary>
        public bool IsPrepayment { get; set; }

        /// <summary>
        /// Массив идентификаторов товаров в системе продавца
        /// </summary>
        public List<string> OffersIds { get; set; }

        /// <summary>
        /// Массив идентификаторов товаров в системе Ozon
        /// </summary>
        public List<int> ProductsIds { get; set; }
    }

    public class ProductSetPrepaymentResult
    {
        public List<PrepaymentItemUpdated> ItemUpdated { get; set; }
    }

    public partial class PrepaymentItemUpdated
    {
        /// <summary>
        /// Результат запроса
        /// <para>true — изменения сохранены</para>
        ///<para>false — изменения не применились.</para>
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Идентификатор товара
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Идентификатор товара в системе продавца
        /// </summary>
        public string OfferId { get; set; }
    }

}
