//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Repository.Currencies;
using Newtonsoft.Json;

namespace AdvantShop.Catalog
{
    public enum OptionPriceType
    {
        [Localize("Core.Catalog.OptionPriceType.Fixed")]
        Fixed = 0,

        [Localize("Core.Catalog.OptionPriceType.Percent")]
        Percent = 1
    }

    public enum OptionField
    {
        Title = 1,
        PriceBc = 2,
        SortOrder = 4
    }

    [Serializable]
    public class OptionItem : IDentable
    {
        public int ID => OptionId;
        
        public int OptionId { get; set; }
        public int CustomOptionsId { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Title { get; set; }

        [JsonIgnore]
        public float CurrencyRate { get; set; }

        //[JsonIgnore]
        public float BasePrice { get; set; }

        //[JsonIgnore]
        public OptionPriceType PriceType { get; set; }

        //[JsonIgnore]
        public int SortOrder { get; set; }


        public OptionItemPhoto PictureData { get; set; }

        public int? ProductId { get; set; }
        public int? OfferId { get; set; }
        public float? MinQuantity { get; set; }
        public float? MaxQuantity { get; set; }
        public float? DefaultQuantity { get; set; }
        public string Description { get; set; }

        private ProductPhoto _productPhoto;
        public ProductPhoto ProductPhoto
        {
            get
            {
                if (_productPhoto != null)
                    return _productPhoto;

                if (ProductId == null)
                    return null;

                _productPhoto = PhotoService.GetMainProductPhoto(ProductId.Value);
                return _productPhoto;
            }
            set
            {
                _productPhoto = value;
            }
        }

        // для клиентской части
        public string OptionText { get; set; }
        

        private float? _roundedPrice;

        public float GetRoundedPrice(Currency renderingCurrency)
        {
            if (PriceType == OptionPriceType.Fixed)
            {
                return _roundedPrice ??
                       (float) (_roundedPrice = PriceService.RoundPrice(BasePrice, renderingCurrency, CurrencyRate));
            }
            return BasePrice;
        }
        
        public string GetOptionText(Currency renderingCurrency = null)
        {
            var result = Title.Trim();

            var price = GetRoundedPrice(renderingCurrency);

            if (price != 0)
            {
                result = Title + GetOptionPrice(renderingCurrency);
            }

            return result;
        }
        
        public string GetOptionPrice(Currency renderingCurrency = null)
        {
            var result = "";
            var price = GetRoundedPrice(renderingCurrency);

            if (price != 0)
            {
                var symbol = price > 0 ? " +" : price < 0 ? " -" : "";
                if (PriceType == OptionPriceType.Fixed)
                {
                    result = symbol + Math.Abs(price).FormatPrice(renderingCurrency ?? CurrencyService.CurrentCurrency);
                }
                else if (PriceType == OptionPriceType.Percent)
                {
                    result = symbol + Math.Abs(price).ToString("#,0.##") + "%";
                }
            }

            return result;
        }
    }
}