using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Core.Services.Crm.OzonSeller.Api
{
    public class GetProductInfoParams
    {
        public string OfferId { get; set; }
        public int? ProductId { get; set; }
        public int? Sku { get; set; }
    }

    public class ProductInfo
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор товара в системе продавца
        /// </summary>
        public string OfferId { get; set; }

        /// <summary>
        /// Идентификатор категории товара
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Штрихкод
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Цена главного предложения на Ozon
        /// </summary>
        public string BuyboxPrice { get; set; }

        /// <summary>
        /// Цена на товар с учетом всех акций. Это значение будет указано на витрине Ozon
        /// </summary>
        public string MarketingPrice { get; set; }

        /// <summary>
        /// Минимальная цена на аналогичный товара на Ozon
        /// </summary>
        public string MinOzonPrice { get; set; }

        /// <summary>
        /// Цена до учета скидок, на карточке товара отображается зачеркнутой
        /// </summary>
        public string OldPrice { get; set; }

        /// <summary>
        /// Цена для клиентов с подпиской Ozon Premium
        /// </summary>
        public string PremiumPrice { get; set; }

        /// <summary>
        /// Информация о цене товара
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Цена на товар, рекомендованная системой на основании схожих предложений
        /// </summary>
        public string RecommendedPrice { get; set; }

        /// <summary>
        /// Дата и время создания товара
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Статус добваления товара с систему
        /// </summary>
        public StatusImportProduct State { get; set; }

        /// <summary>
        /// Информация об ошибках валидации товара
        /// </summary>
        public List<ProductInfoError> Errors { get; set; }

        /// <summary>
        /// Массив url для изображений товара
        /// </summary>
        public List<Uri> Images { get; set; }

        /// <summary>
        /// Информация о SKU Ozon
        /// </summary>
        public List<ProductInfoSource> Sources { get; set; }

        /// <summary>
        /// Информация о количестве товара
        /// </summary>
        public ProductInfoStocks Stocks { get; set; }

        /// <summary>
        /// Ставка НДС для товара:
        /// <para>0 — не облагается НДС;</para>
        /// <para>0.1 — 10%;</para>
        /// <para>0.2 — 20%.</para>
        /// </summary>
        public string Vat { get; set; }

        /// <summary>
        /// Параметры видимости товара
        /// </summary>
        public ProductInfoVisibilityDetails VisibilityDetails { get; set; }

        /// <summary>
        /// Товар доступен на Ozon для покупки
        /// </summary>
        public bool Visible { get; set; }
    }

    public class ProductInfoError
    {
        /// <summary>
        /// Поле, в котором возникла ошибка
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Идентификатор параметра с ошибкой
        /// </summary>
        public int AttributeId { get; set; }

        /// <summary>
        /// Код ошибки
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Тип ошибки
        /// </summary>
        public ProductInfoErrorType Level { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter), converterParameters: true /*camelCaseText*/)]
    public enum ProductInfoErrorType
    {
        /// <summary>
        /// запрос выполнится, но есть ошибка в данных
        /// </summary>
        Warning,

        /// <summary>
        /// прерывается обработка запроса: проверьте данные и повторите запрос
        /// </summary>
        Error
    }

    public class ProductInfoSource
    {
        /// <summary>
        /// Видимость SKU товара в системе Ozon
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Номер Ozon-SKU
        /// </summary>
        public int Sku { get; set; }

        /// <summary>
        /// Тип SKU Ozon
        /// <para>fbo — товар продается со склада Ozon</para>
        /// <para>fbs — товар продается со склада продавца</para>
        /// <para>crossborder — трансграничная торговля</para>
        /// </summary>
        public string Source { get; set; }
    }

    public class ProductInfoStocks
    {
        /// <summary>
        /// Товары, которые ожидают поставки
        /// </summary>
        public int Coming { get; set; }

        /// <summary>
        /// Товары в наличии
        /// </summary>
        public int Present { get; set; }

        /// <summary>
        /// Товары в резерве
        /// </summary>
        public int Reserved { get; set; }
    }

    public class ProductInfoVisibilityDetails
    {
        /// <summary>
        /// Товар активирован
        /// </summary>
        public bool ActiveProduct { get; set; }

        /// <summary>
        /// У товара есть цена
        /// </summary>
        public bool HasPrice { get; set; }

        /// <summary>
        /// Товар доступен на складе
        /// </summary>
        public bool HasStock { get; set; }
    }

    public class GetProductsInfoAttributesParam
    {
        public GetProductsInfoAttributesFilter Filter { get; set; }

        /// <summary>
        /// Номер страницы, возвращаемой в запросе
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Количество элементов на странице
        /// </summary>
        public int PageSize { get; set; }
    }

    public class GetProductsInfoAttributesFilter
    {
        /// <summary>
        /// Идентификатор товара в системе продавца — артикул
        /// </summary>
        public List<string> OfferId { get; set; }

        /// <summary>
        /// Идентификатор товара
        /// </summary>
        public List<int> ProductId { get; set; }
    }

    public class ProductsInfoAttribute
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор товара в системе продавца
        /// </summary>
        public string OfferId { get; set; }

        /// <summary>
        /// Штрихкод товара
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Идентификатор категории товара
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Высота
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Глубина
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Ширина
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Единицы измерения
        /// </summary>
        public DimensionUnit DimensionUnit { get; set; }

        /// <summary>
        /// Вес
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Единица измерения веса
        /// </summary>
        public WeightUnit WeightUnit { get; set; }

        /// <summary>
        /// Массив характеристик товара
        /// </summary>
        public List<ProductsInfoAttributeAttribute> Attributes { get; set; }

        /// <summary>
        /// Массив вложенных характеристик
        /// </summary>
        public List<ComplexAttribute> ComplexAttributes { get; set; }

        /// <summary>
        /// Идентификатор группы изображений
        /// </summary>
        public string ImageGroupId { get; set; }

        /// <summary>
        /// Массив url для изображений товара
        /// </summary>
        public List<ProductsInfoAttributeImage> Images { get; set; }

        /// <summary>
        /// Массив изображений 360
        /// </summary>
        public List<ProductsInfoAttributeImages360> Images360 { get; set; }

        /// <summary>
        /// Массив pdf-файлов
        /// </summary>
        public List<ProductsInfoAttributePdfList> PdfList { get; set; }
    }

    public class ProductsInfoAttributeAttribute
    {
        /// <summary>
        /// Идентификатор характеристики
        /// </summary>
        public int AttributeId { get; set; }

        /// <summary>
        /// Массив значений характеристик
        /// </summary>
        public List<ProductsInfoAttributeValue> Values { get; set; }
    }

    public class ProductsInfoAttributeValue
    {
        /// <summary>
        /// Идентификатор характеристики в словаре
        /// </summary>
        public int DictionaryValueId { get; set; }

        /// <summary>
        /// Значение характеристики товара
        /// </summary>
        public string Value { get; set; }
    }

    public class ComplexAttribute
    {
        /// <summary>
        /// Массив характеристик товара
        /// </summary>
        public List<ComplexAttributeAttribute> Attributes { get; set; }
    }

    public class ComplexAttributeAttribute
    {
        /// <summary>
        /// Идентификатор характеристики
        /// </summary>
        public int AttributeId { get; set; }

        /// <summary>
        /// Идентификатор характеристики, которая поддерживает вложенные свойства
        /// </summary>
        public int ComplexId { get; set; }
        public List<ProductsInfoAttributeValue> Values { get; set; }
    }

    public class ProductsInfoAttributeImage
    {
        /// <summary>
        /// Ссылка на изображение формата
        /// </summary>
        public Uri FileName { get; set; }

        /// <summary>
        /// изображение главное
        /// </summary>
        public bool Default { get; set; }
        public int Index { get; set; }
    }

    public class ProductsInfoAttributeImages360
    {
        public Uri FileName { get; set; }
        public int Index { get; set; }
    }

    public class ProductsInfoAttributePdfList
    {
        public Uri FileName { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
    }
    
    public class GetProductsStocksParam
    {
        public GetProductsStocksParam()
        {
            PageSize = 1000;
        }
        /// <summary>
        /// Номер страницы, возвращаемой в запросе
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Количество элементов на странице
        /// </summary>
        public int PageSize { get; set; }
    }

    public class ProductStocks
    {
        /// <summary>
        /// Идентификатор товара
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Идентификатор товара в системе продавца
        /// </summary>
        public string OfferId { get; set; }

        /// <summary>
        /// Количество товара в наличии
        /// </summary>
        public ProductInfoStocks Stock { get; set; }
    }
}
