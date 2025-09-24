//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;

namespace AdvantShop.Catalog
{
    [Serializable]
    public class Offer
    {
        public Offer(){}
        
        public Offer(float amount)
        {
            Amount = amount;
        }
        public int OfferId { get; set; }

        public int ProductId { get; set; }

        public float Amount { get; private set; }

        [Obsolete("Значение будет присвоено объекту, но сохраняться в БД не будет. Регулирование остатков теперь осуществляется через склады (WarehouseStocksService).")]
        public void SetAmount(float amount) => Amount = amount;

        public void SetAmountByWarehouse(int warehouseId) => 
            SetAmountByWarehouse(Core.Services.Catalog.Warehouses.WarehouseStocksService.Get(OfferId, warehouseId));
        
        public void SetAmountByWarehouse(Core.Services.Catalog.Warehouses.WarehouseStock stock) => 
            Amount = stock?.Quantity ?? 0f;
        
        public void SetAmountByStocks(IEnumerable<Core.Services.Catalog.Warehouses.WarehouseStock> stocks) => 
            Amount = stocks?.Sum(s => s.Quantity) ?? 0f;

        /// <summary>
        /// Price from db
        /// </summary>
        [Compare("Core.Catalog.Offer.BasePrice")]
        public float BasePrice { get; set; }

        private float? _roundedPrice;

        public float RoundedPrice =>
            _roundedPrice ??
            (float) (_roundedPrice = PriceService.RoundPrice(BasePrice, null, Product.Currency.Rate));

        [NonSerialized]
        private OfferPriceRule _priceRule;

        [XmlIgnore]
        public OfferPriceRule PriceRule => _priceRule;
        
        public Offer SetOfferPriceRule(float amount, int customerGroupId, int? paymentMethodId = null, int? shippingMethodId = null)
        {
            var priceRule = PriceRuleService.GetPriceRule(OfferId, amount, customerGroupId, paymentMethodId, shippingMethodId);

            // if (paymentMethodId != null && priceRule == null)
            //     return this;

            _priceRule = priceRule;

            if (_priceRule?.PriceByRule == null)
                return this;

            BasePrice = _priceRule.PriceByRule.Value;
            _roundedPrice = null;

            return this;
        }
    
     
        [Compare("Core.Catalog.Offer.SupplyPrice")]
        public float SupplyPrice { get; set; }

        [Compare("Core.Catalog.Offer.Color", ChangeHistoryParameterType.Color)]
        public int? ColorID { get; set; }

        [Compare("Core.Catalog.Offer.Size", ChangeHistoryParameterType.Size)]
        public int? SizeID { get; set; }

        [Compare("Core.Catalog.Offer.Main")]
        public bool Main { get; set; }

        [Compare("Core.Catalog.Offer.ArtNo")]
        public string ArtNo { get; set; }

        [NonSerialized] private Color _color;

        [XmlIgnore]
        public Color Color => _color ?? (_color = ColorService.GetColor(ColorID));

        [NonSerialized] 
        private Size _size;

        [XmlIgnore]
        public Size Size => _size ?? (_size = SizeService.GetSize(SizeID));

        [NonSerialized]
        private SizeForCategory _sizeForCategory;

        [XmlIgnore]
        public SizeForCategory SizeForCategory
        {
            get
            {
                _sizeForCategory = _sizeForCategory ?? SizeService.GetSizeForCategory(SizeID, Product.CategoryId);
                _size = _sizeForCategory;
                return _sizeForCategory;
            }
        }

        [NonSerialized] private Product _product;

        [XmlIgnore]
        public Product Product => _product ?? (_product = ProductService.GetProduct(ProductId));

        [NonSerialized] 
        private ProductPhoto _photo;

        [XmlIgnore]
        public ProductPhoto Photo => _photo ?? (_photo = PhotoService.GetMainProductPhoto(ProductId, ColorID));

        [Compare("Core.Catalog.Offer.Weight")]
        public float? Weight { get; set; }

        [Compare("Core.Catalog.Offer.Length")]
        public float? Length { get; set; }

        [Compare("Core.Catalog.Offer.Width")]
        public float? Width { get; set; }

        [Compare("Core.Catalog.Offer.Height")]
        public float? Height { get; set; }

        [Compare("Core.Catalog.Offer.BarCode")]
        public string BarCode { get; set; }
    }
}
