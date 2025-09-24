using System;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Catalog
{
    public enum EYandexDiscountCondition
    {
        [Localize("Core.Catalog.EYandexDiscountCondition.None")]
        None = 0,
        
        [Localize("Core.Catalog.EYandexDiscountCondition.ShowcaseSample")]
        [StringName("showcasesample")]
        LikeNew,
        
        [Localize("Core.Catalog.EYandexDiscountCondition.Preowned")]
        [StringName("preowned")]
        Used,

        [Localize("Core.Catalog.EYandexDiscountCondition.Reduction")]
        [StringName("reduction")]
        Reduction,

        [Localize("Core.Catalog.EYandexDiscountCondition.Refurbished")]
        [StringName("refurbished")]
        Refurbished
    }

    public enum EYandexProductQuality
    {
        [Localize("Core.Catalog.EYandexProductQuality.None")]
        None = 0,

        [Localize("Core.Catalog.EYandexProductQuality.Perfect")]
        [StringName("perfect")]
        Perfect,

        [Localize("Core.Catalog.EYandexProductQuality.Excellent")]
        [StringName("excellent")]
        Excellent,

        [Localize("Core.Catalog.EYandexProductQuality.Good")]
        [StringName("good")]
        Good
    }

    public class ProductExportOptions
    {
        public ProductExportOptions()
        {
            IsChanged = false;
        }

        private bool _isChanged;

        public bool IsChanged
        {
            get => _isChanged;
            set => _isChanged = value;
        }

        private string _gtin;

        [Compare("Core.Catalog.Product.Gtin")]
        public string Gtin
        {
            get => _gtin;
            set
            {
                _isChanged |= _gtin != value;
                _gtin = value;
            }
        }
        
        private string _mpn;

        [Compare("Core.Catalog.Product.Mpn")]
        public string Mpn
        {
            get => _mpn;
            set
            {
                _isChanged |= _mpn != value;
                _mpn = value;
            }
        }

        private string _googleProductCategory;

        [Compare("Core.Catalog.Product.GoogleProductCategory")]
        public string GoogleProductCategory
        {
            get => _googleProductCategory;
            set
            {
                _isChanged |= _googleProductCategory != value;
                _googleProductCategory = value;
            }
        }

        private string _yandexSalesNote;
        
        [Compare("Core.Catalog.Product.SalesNote")]
        public string YandexSalesNote 
        {
            get => _yandexSalesNote;
            set
            {
                _isChanged |= _yandexSalesNote != value;
                _yandexSalesNote = value;
            }
        }
        
        private string _yandexTypePrefix;
        [Compare("Core.Catalog.Product.YandexTypePrefix")]
        public string YandexTypePrefix
        {
            get => _yandexTypePrefix;
            set
            {
                _isChanged |= _yandexTypePrefix != value;
                _yandexTypePrefix = value;
            }
        }
        
        private string _yandexModel;
        [Compare("Core.Catalog.Product.YandexModel")]
        public string YandexModel
        {
            get => _yandexModel;
            set
            {
                _isChanged |= _yandexModel != value;
                _yandexModel = value;
            }
        }
        
        private string _yandexName;
        [Compare("Core.Catalog.Product.YandexName")]
        public string YandexName
        {
            get => _yandexName;
            set
            {
                _isChanged |= _yandexName != value;
                _yandexName = value;
            }
        }
        
        private string _yandexDeliveryDays;
        [Compare("Core.Catalog.Product.YandexDeliveryDays")]
        public string YandexDeliveryDays 
        {
            get => _yandexDeliveryDays;
            set
            {
                _isChanged |= _yandexDeliveryDays != value;
                _yandexDeliveryDays = value;
            }
        }  
        
        private float _bid;
        [Compare("Core.Catalog.Product.Bid")]
        public float Bid
        {
            get => _bid;
            set
            {
                _isChanged |= _bid != value;
                _bid = value;
            }
        }

        private string _yandexSizeUnit;

        [Compare("Core.Catalog.Product.YandexSizeUnit")]
        public string YandexSizeUnit
        {
            get => _yandexSizeUnit;
            set
            {
                _isChanged |= _yandexSizeUnit != value;
                _yandexSizeUnit = value != null && ProductExportOptionsService.YandexSizeUnitValidate.Contains(value)
                    ? value
                    : null;
            }
        }

        private bool _adult;
        [Compare("Core.Catalog.Product.Adult")]
        public bool Adult 
        {
            get => _adult;
            set
            {
                _isChanged |= _adult != value;
                _adult = value;
            }
        }
        
        private bool _manufacturerWarranty;
        [Compare("Core.Catalog.Product.ManufacturerWarranty")]
        public bool ManufacturerWarranty 
        {
            get => _manufacturerWarranty;
            set
            {
                _isChanged |= _manufacturerWarranty != value;
                _manufacturerWarranty = value;
            }
        }
        
        private bool _yandexProductDiscounted;
        [Compare("Core.Catalog.Product.YandexProductDiscounted")]
        public bool YandexProductDiscounted 
        {
            get => _yandexProductDiscounted;
            set
            {
                _isChanged |= _yandexProductDiscounted != value;
                _yandexProductDiscounted = value;
            }
        }
        
        private EYandexDiscountCondition _yandexProductDiscountCondition;
        [Compare("Core.Catalog.Product.YandexProductDiscountCondition")]
        public EYandexDiscountCondition YandexProductDiscountCondition 
        {
            get => _yandexProductDiscountCondition;
            set
            {
                _isChanged |= _yandexProductDiscountCondition != value;
                _yandexProductDiscountCondition = value;
            }
        }

        private string _yandexProductDiscountReason;
        [Compare("Core.Catalog.Product.YandexProductDiscountReason")]
        public string YandexProductDiscountReason 
        {
            get => _yandexProductDiscountReason;
            set
            {
                _isChanged |= _yandexProductDiscountReason != value;
                _yandexProductDiscountReason = value;
            }
        }


        private EYandexProductQuality _yandexProductQuality;
        [Compare("Core.Catalog.Product.YandexProductQuality")]
        public EYandexProductQuality YandexProductQuality
        {
            get => _yandexProductQuality;
            set
            {
                _isChanged |= _yandexProductQuality != value;
                _yandexProductQuality = value;
            }
        }

        private string _yandexMarketCategory;
        [Compare("Core.Catalog.Product.YandexMarketCategory")]
        public string YandexMarketCategory 
        {
            get => _yandexMarketCategory;
            set
            {
                _isChanged |= _yandexMarketCategory != value;
                _yandexMarketCategory = value;
            }
        }
        
        
        
        private string _yandexMarketExpiry;
        [Compare("Core.Catalog.Product.YandexMarketExpiry")]
        public string YandexMarketExpiry
        {
            get => _yandexMarketExpiry;
            set
            {
                _isChanged |= _yandexMarketExpiry != value;
                _yandexMarketExpiry = value;
            }
        }
        
        private string _yandexMarketWarrantyDays;
        [Compare("Core.Catalog.Product.YandexMarketWarrantyDays")]
        public string YandexMarketWarrantyDays
        {
            get => _yandexMarketWarrantyDays;
            set
            {
                _isChanged |= _yandexMarketWarrantyDays != value;
                _yandexMarketWarrantyDays = value;
            }
        }
        
        private string _yandexMarketCommentWarranty;
        [Compare("Core.Catalog.Product.YandexMarketCommentWarranty")]
        public string YandexMarketCommentWarranty
        {
            get => _yandexMarketCommentWarranty;
            set
            {
                _isChanged |= _yandexMarketCommentWarranty != value;
                _yandexMarketCommentWarranty = value;
            }
        }
        
        private string _yandexMarketPeriodOfValidityDays;
        [Compare("Core.Catalog.Product.YandexMarketPeriodOfValidityDays")]
        public string YandexMarketPeriodOfValidityDays
        {
            get => _yandexMarketPeriodOfValidityDays;
            set
            {
                _isChanged |= _yandexMarketPeriodOfValidityDays != value;
                _yandexMarketPeriodOfValidityDays = value;
            }
        }
        
        private string _yandexMarketServiceLifeDays;
        [Compare("Core.Catalog.Product.YandexMarketServiceLifeDays")]
        public string YandexMarketServiceLifeDays
        {
            get => _yandexMarketServiceLifeDays;
            set
            {
                _isChanged |= _yandexMarketServiceLifeDays != value;
                _yandexMarketServiceLifeDays = value;
            }
        }
        
        private string _yandexMarketTnVedCode;
        [Compare("Core.Catalog.Product.YandexMarketTnVedCode")]
        public string YandexMarketTnVedCode
        {
            get => _yandexMarketTnVedCode;
            set
            {
                _isChanged |= _yandexMarketTnVedCode != value;
                _yandexMarketTnVedCode = value;
            }
        }
        
        private int? _yandexMarketStepQuantity;
        [Compare("Core.Catalog.Product.YandexMarketStepQuantity")]
        public int? YandexMarketStepQuantity
        {
            get => _yandexMarketStepQuantity;
            set
            {
                _isChanged |= _yandexMarketStepQuantity != value;
                _yandexMarketStepQuantity = value;
            }
        }
        
        private int? _yandexMarketMinQuantity;
        [Compare("Core.Catalog.Product.YandexMarketMinQuantity")]
        public int? YandexMarketMinQuantity
        {
            get => _yandexMarketMinQuantity;
            set
            {
                _isChanged |= _yandexMarketMinQuantity != value;
                _yandexMarketMinQuantity = value;
            }
        }
        
        private long? _yandexMarketCategoryId;
        [Compare("Core.Catalog.Product.YandexMarketCategoryId")]
        public long? YandexMarketCategoryId
        {
            get => _yandexMarketCategoryId;
            set
            {
                _isChanged |= _yandexMarketCategoryId != value;
                _yandexMarketCategoryId = value;
            }
        }
        
        private DateTime? _googleAvailabilityDate;
        [Compare("Core.Catalog.Product.GoogleAvailabilityDate")]
        public DateTime? GoogleAvailabilityDate
        {
            get => _googleAvailabilityDate;
            set
            {
                _isChanged |= _googleAvailabilityDate != value;
                _googleAvailabilityDate = value;
            }
        }
    }
}