using System;
using System.Xml.Serialization;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Crm
{
    public class LeadItem
    {
        public int LeadItemId { get; set; }

        public int LeadId { get; set; }

        public int? ProductId { get; set; }
        
        public string Name { get; set; }
        
        public string ArtNo { get; set; }

        [Compare("Core.Crm.LeadItem.Price", ChangeHistoryParameterType.LeadItemField)]
        public float Price { get; set; }

        [Compare("Core.Crm.LeadItem.Amount", ChangeHistoryParameterType.LeadItemField)]
        public float Amount { get; set; }

        public string Color { get; set; }

        public string Size { get; set; }

        public int? PhotoId { get; set; }

        public string BarCode { get; set; }

        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        [NonSerialized]
        private ProductPhoto _photo;

        [XmlIgnore]
        public ProductPhoto Photo
        {
            get
            {
                if (_photo != null)
                    return _photo;

                _photo = PhotoId != null
                            ? PhotoService.GetPhoto<ProductPhoto>(PhotoId.Value, PhotoType.Product)
                            : null;

                if (_photo == null)
                    _photo = ProductId != null
                            ? PhotoService.GetPhotoByObjId<ProductPhoto>(ProductId.Value, PhotoType.Product)
                            : null;

                if (_photo == null)
                    _photo = new ProductPhoto(0, PhotoType.Product, "");

                return _photo;
            }
        }
        
        public float Weight { get; set; }

        public string CustomOptionsJson { get; set; }

        public LeadItem()
        {
            
        }

        public LeadItem(Offer offer, float amount, float? price)
        {
            ProductId = offer.ProductId;
            Name = offer.Product.Name;
            ArtNo = offer.ArtNo;
            Amount = amount != 0 ? amount : 1;
            Price = price != null
                ? price.Value
                : PriceService.GetFinalPrice(offer.RoundedPrice, offer.Product.Discount);
            Color = offer.Color != null ? offer.Color.ColorName : null;
            Size = offer.Size != null ? offer.Size.SizeName : null;
            PhotoId = offer.Photo != null ? offer.Photo.PhotoId : (int?) null;
            Weight = offer.GetWeight();
            Width = offer.GetWidth();
            Length = offer.GetLength();
            Height = offer.GetHeight();
            BarCode = offer.BarCode;
        }

        public LeadItem(Offer offer, float amount) : this(offer, amount, null)
        {
        }

        public static explicit operator LeadItem(OrderItem item)
        {
            return new LeadItem
            {
                LeadItemId = item.OrderItemID,
                ProductId = item.ProductID,
                Name = item.Name,
                ArtNo = item.ArtNo,
                Price = item.Price,
                Amount = item.Amount,
                Weight = item.Weight,
                Color = item.Color,
                Size = item.Size,
                PhotoId = item.PhotoID,
                Width = item.Width,
                Length = item.Length,
                Height = item.Height,
                BarCode = item.BarCode,
                CustomOptionsJson = CustomOptionsService.SerializeToJson(item.SelectedOptions)
            };
        }
    }
}
