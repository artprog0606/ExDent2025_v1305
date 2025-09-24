//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Xml.Serialization;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.SEO;
using AdvantShop.Repository.Currencies;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.ChangeHistories;
using System.Linq;

namespace AdvantShop.Catalog
{
    public enum RelatedType
    {
        Related = 0,
        Alternative = 1
    }
    
    public class Product
    {
        public Product()
        {
            Discount = new Discount();
            AccrueBonuses = true;
        }

        public int ProductId { get; set; }

        [Compare("Core.Catalog.Product.ArtNo")]
        public string ArtNo { get; set; }

        [Compare("Core.Catalog.Product.Name")]
        public string Name { get; set; }


        public string Photo { get; set; }
        public string PhotoDesc { get; set; } // убрать или сделать lazy

        [Compare("Core.Catalog.Product.Ratio")]
        public double Ratio { get; set; } // убрать или сделать lazy

        [Compare("Core.Catalog.Product.Ratio")]
        public double? ManualRatio { get; set; }

        [Compare("Core.Catalog.Product.Discount")]
        public Discount Discount { get; set; }

        /// <summary>
        /// Не применять другие скидки (кроме скидки товара), бонусы, купоны 
        /// </summary>
        [Compare("Core.Catalog.Product.DoNotApplyOtherDiscounts")]
        public bool DoNotApplyOtherDiscounts { get; set; }

        [Compare("Core.Catalog.Product.BriefDescription", true)]
        public string BriefDescription { get; set; }

        [Compare("Core.Catalog.Product.Description", true)]
        public string Description { get; set; }

        [Compare("Core.Catalog.Product.Enabled")]
        public bool Enabled { get; set; }

        public bool Hidden { get; set; }

        [Compare("Core.Catalog.Product.Recomended")]
        public bool Recomended { get; set; }

        [Compare("Core.Catalog.Product.New")]
        public bool New { get; set; }

        [Compare("Core.Catalog.Product.BestSeller")]
        public bool BestSeller { get; set; }

        [Compare("Core.Catalog.Product.OnSale")]
        public bool OnSale { get; set; }

        [Compare("Core.Catalog.Product.AllowPreOrder")]
        public bool AllowPreOrder { get; set; }

        public bool CategoryEnabled { get; set; }

        [Compare("Core.Catalog.Product.Unit", ChangeHistoryParameterType.Unit)]
        public int? UnitId { get; set; }
        
        private Unit _unit;
        public Unit Unit => _unit ?? (_unit = UnitId.HasValue ? UnitService.Get(UnitId.Value) : null);

        [Compare("Core.Catalog.Product.ShippingPrice")]
        public float? ShippingPrice { get; set; }

        [Compare("Core.Catalog.Product.MinAmount")]
        public float? MinAmount { get; set; }

        [Compare("Core.Catalog.Product.MaxAmount")]
        public float? MaxAmount { get; set; }

        [Compare("Core.Catalog.Product.Multiplicity")]
        public float Multiplicity { get; set; }

        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }        

        public bool ActiveView360 { get; set; }
        
        [Compare("Core.Catalog.Product.DownloadLink")]
        public string DownloadLink { get; set; }

        /// <summary>
        /// Начислять бонусы за покупку этого товара
        /// </summary>
        [Compare("Core.Catalog.Product.AccrueBonuses")]
        public bool AccrueBonuses { get; set; }
        
        [Compare("Core.Catalog.Product.Tax", ChangeHistoryParameterType.Tax)]
        public int? TaxId { get; set; }

        private ePaymentSubjectType _paymentSubjectType;

        [Compare("Core.Catalog.Product.PaymentSubjectType")]
        public ePaymentSubjectType PaymentSubjectType
        {
            get => (int) _paymentSubjectType != 0 ? _paymentSubjectType : ePaymentSubjectType.commodity;
            set => _paymentSubjectType = value;
        }

        private ePaymentMethodType _paymentMethodType;

        [Compare("Core.Catalog.Product.PaymentMethodType")]
        public ePaymentMethodType PaymentMethodType
        {
            get => _paymentMethodType != 0 ? _paymentMethodType : ePaymentMethodType.full_prepayment;
            set => _paymentMethodType = value;
        }

        [Compare("Core.Catalog.Product.Brand", ChangeHistoryParameterType.Brand)]
        public int BrandId { get; set; }

        private Brand _brand;
        public Brand Brand => _brand ?? (_brand = BrandService.GetBrandById(BrandId));

        private string _urlPath;

        [Compare("Core.Catalog.Product.UrlPath")]
        public string UrlPath
        {
            get => _urlPath;
            set => _urlPath = value.ToLower();
        }

        /// <summary>
        /// Подлежит обязательной маркировке «Честный знак»
        /// </summary>
        [Compare("Core.Catalog.Product.IsMarkingRequired")]
        public bool IsMarkingRequired { get; set; }
        
        [Compare("Core.Catalog.Product.IsDigital")]
        public bool IsDigital { get; set; }

        public bool HasMultiOffer { get; set; }

        private bool? _gifts;
        public bool HasGifts()
        {
            if (_gifts.HasValue)
                return _gifts.Value;
            _gifts = ProductService.HasGifts(ProductId);
            return _gifts.Value;
        }

        [Compare("Core.Catalog.Product.Currency", ChangeHistoryParameterType.Currency)]
        public int CurrencyID { get; set; }

        private Currency _currency;
        public Currency Currency => _currency ?? (_currency = CurrencyService.GetCurrency(CurrencyID, true));

        [Compare("Core.Catalog.Product.Comment")]
        public string Comment { get; set; }

        private bool _sizeChartLoaded;
        private SizeChart _sizeChart;
        public SizeChart SizeChart
        {
            get
            {
                if (_sizeChartLoaded)
                    return _sizeChart;
                _sizeChartLoaded = true;
                return _sizeChart ?? (_sizeChart = SizeChartService.Get(ProductId, ESizeChartEntityType.Product).FirstOrDefault());
            }
            set
            {
                _sizeChart = value;
                _sizeChartLoaded = true;
            }
        }

        private List<Offer> _offers;

        public List<Offer> Offers
        {
            get => _offers ?? (_offers = OfferService.GetProductOffers(ProductId));
            set => _offers = value;
        }

        public MetaType MetaType => MetaType.Product;

        private bool _isMetaLoaded;
        private MetaInfo _meta;

        public MetaInfo Meta
        {
            get
            {
                if (_isMetaLoaded)
                    return _meta;

                _isMetaLoaded = true;

                return _meta ??
                       (_meta =
                           MetaInfoService.GetMetaInfo(ProductId, MetaType) ??
                           MetaInfoService.GetDefaultMetaInfo(MetaType, string.Empty));
            }
            set
            {
                _meta = value;
                _isMetaLoaded = true;
            }
        }

        private List<ProductPhoto> _productPhotos;
        public List<ProductPhoto> ProductPhotos => _productPhotos ?? 
                                                   (_productPhotos = PhotoService.GetPhotos<ProductPhoto>(ProductId, PhotoType.Product));

        private List<ProductPhoto> _productPhotos360;

        public List<ProductPhoto> ProductPhotos360 => _productPhotos360 ??
                                                      (_productPhotos360 = PhotoService.GetPhotos<ProductPhoto>(ProductId, PhotoType.Product360));

        private List<ProductVideo> _productVideos;
        public List<ProductVideo> ProductVideos => _productVideos ?? 
                                                   (_productVideos = ProductVideoService.GetProductVideos(ProductId));

        private List<PropertyValue> _productPropertyValues;
        public List<PropertyValue> ProductPropertyValues =>
            _productPropertyValues ??
            (_productPropertyValues = PropertyService.GetPropertyValuesByProductId(ProductId));

        private int _categoryId;

        public int CategoryId => _categoryId == 0 || _categoryId == CategoryService.DefaultNonCategoryId
            ? _categoryId = ProductService.GetFirstCategoryIdByProductId(ProductId)
            : _categoryId;

        public void SetMainCategoryId(int? mainCategoryId)
        {
            _categoryId = mainCategoryId == null || mainCategoryId == 0
                ? CategoryService.DefaultNonCategoryId
                : mainCategoryId.Value;
        }

        private Category _mainCategory;
        public Category MainCategory => _mainCategory ?? (_mainCategory = CategoryService.GetCategory(CategoryId));

        private List<Category> _productCategories;

        [SoapIgnore]
        [XmlIgnore]
        public List<Category> ProductCategories => _productCategories ??
                                                   (_productCategories = ProductService.GetCategoriesByProductId(ProductId));


        private bool _tagsLoaded;
        private List<Tag> _tags;
        public List<Tag> Tags
        {
            get
            {
                if (_tagsLoaded)
                    return _tags;
                _tagsLoaded = true;
                return _tags ?? (_tags = TagService.Gets(ProductId, ETagType.Product, true));
            }
            set
            {
                _tags = value;
                _tagsLoaded = true;
            }
        }

        private int? _ratioCount;
        public int? RatioCount => _ratioCount ?? (_ratioCount = RatingService.GetProductRatioCount(ProductId));

        private ProductExportOptions _exportOptions;
        public ProductExportOptions ExportOptions
        {
            get => _exportOptions ?? (_exportOptions = ProductExportOptionsService.Get(ProductId));
            set => _exportOptions = value;
        }
    }
}