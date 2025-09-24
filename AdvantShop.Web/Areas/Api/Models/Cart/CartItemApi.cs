using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Products;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Cart
{
    public class CartItemApi
    {
        public int Index { get; set; }
        public string ValidationError { get; }
        
        public int OfferId { get; }
        public string Name { get; }
        public string ArtNo { get; }
        public string Link { get; }
        public string PhotoSmall { get; }
        public string PhotoMiddle { get; }
        public string Color { get; }
        public string Size { get; }
        public List<SelectedCartCustomOptionItemApi> SelectedCustomOptions { get; }
        public string PreparedOldPrice { get; }
        public string PreparedPrice { get; }
        public ProductDiscountApi Discount { get; }
        public string PreparedDiscount { get; }
        public float Amount { get; }
        public string PreparedCost { get; }
        
        public float AvailableAmount { get; }
        public float MinAmount { get; }
        public float MaxAmount { get; }
        public bool CanChangeAmount { get; }
        public float Multiplicity { get; }
        
        public bool IsGift { get; }
        public string Unit { get; }
        
        [JsonIgnore]
        public string AttributesXml { get; }

        public CartItemApi(AdvantShop.Models.Cart.CartItemModel item)
        {
            ValidationError = !string.IsNullOrEmpty(item.Avalible) ? item.Avalible : null;
            
            OfferId = item.OfferId;
            Name = item.Name;
            ArtNo = item.Sku;
            Link = item.Link;
            PreparedOldPrice = item.Price != item.PriceWithDiscount ? item.Price : null;
            PreparedPrice = item.PriceWithDiscount;
            Discount = new ProductDiscountApi(item.Discount);
            PreparedDiscount = item.DiscountText;
            SelectedCustomOptions = item.SelectedOptions?.Select(x => new SelectedCartCustomOptionItemApi(x)).ToList();
            Amount = item.Amount;
            PreparedCost = item.Cost;

            PhotoSmall = item.PhotoSmallPath;
            PhotoMiddle = item.PhotoMiddlePath;
            Color = item.ColorName;
            Size = item.SizeName;
            
            AvailableAmount = item.AvailableAmount;
            MinAmount = item.MinAmount;
            MaxAmount = item.MaxAmount;
            CanChangeAmount = !item.FrozenAmount;
            Multiplicity = item.Multiplicity;
            IsGift = item.IsGift;
            Unit = !string.IsNullOrEmpty(item.Unit) ? item.Unit : null;

            AttributesXml = item.AttributesXml;
        }
    }
}