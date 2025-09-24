using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Orders;

namespace AdvantShop.Shipping
{
    public class PreOrderItem
    {
        public PreOrderItem()
        {
        }

        public PreOrderItem(ShoppingCartItem cartItem)
        {
            var offer = cartItem.Offer;
            var product = offer.Product;
            

            Id = cartItem.ShoppingCartItemId;
            ArtNo = offer.ArtNo;
            ProductId = product.ProductId;
            Name = product.Name;
            Price = cartItem.PriceWithDiscount;
            Amount = cartItem.Amount;

            ShippingPrice = product.ShippingPrice ?? 0;
            
            Weight = offer.GetWeight();
            Width = offer.GetWidth();
            Height = offer.GetHeight();
            Length = offer.GetLength();

            IsDigital = product.IsDigital;
        }

        public PreOrderItem(OrderItem orderItem)
        {
            Id = orderItem.OrderItemID;
            ArtNo = orderItem.ArtNo;
            ProductId = orderItem.ProductID;

            Name = orderItem.Name;
            Price = orderItem.Price;
            Amount = orderItem.Amount;

            Weight = orderItem.Weight;
            Width = orderItem.Width;
            Height = orderItem.Height;
            Length = orderItem.Length;

            var product = orderItem.ProductID != null ? ProductService.GetProduct((int)orderItem.ProductID) : null;
            if (product != null)
            {
                ShippingPrice = product.ShippingPrice ?? 0;

                if (Weight == 0 || (Width == 0 && Height == 0 && Length == 0))
                {
                    var offer = product.Offers.FirstOrDefault(x => x.ArtNo == ArtNo);

                    if (Weight == 0)
                    {
                        Weight = offer != null ? offer.GetWeight() : 0;
                    }

                    if (Width == 0 && Height == 0 && Length == 0)
                    {
                        Width = offer != null ? offer.GetWidth() : 0;
                        Height = offer != null ? offer.GetHeight() : 0;
                        Length = offer != null ? offer.GetLength() : 0;
                    }
                }

                IsDigital = product.IsDigital;
            }
        }

        public PreOrderItem(LeadItem item)
        {
            Id = item.LeadItemId;
            ArtNo = item.ArtNo;
            ProductId = item.ProductId;

            Name = item.Name;
            Price = item.Price;
            Amount = item.Amount;

            Weight = item.Weight;
            Width = item.Width;
            Height = item.Height;
            Length = item.Length;

            var product = item.ProductId != null ? ProductService.GetProduct((int)item.ProductId) : null;
            if (product != null)
            {
                ShippingPrice = product.ShippingPrice ?? 0;

                if (Weight == 0 || (Width == 0 && Height == 0 && Length == 0))
                {
                    var offer = product.Offers.FirstOrDefault(x => x.ArtNo == ArtNo);

                    if (Weight == 0)
                    {
                        Weight = offer != null ? offer.GetWeight() : 0;
                    }

                    if (Width == 0 && Height == 0 && Length == 0)
                    {
                        Width = offer != null ? offer.GetWidth() : 0;
                        Height = offer != null ? offer.GetHeight() : 0;
                        Length = offer != null ? offer.GetLength() : 0;
                    }
                }

                IsDigital = product.IsDigital;
            }
        }

        public int Id { get; set; }
        public string ArtNo { get; set; }
        public int? ProductId { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public float ShippingPrice { get; set; }
        public float Amount { get; set; }
        public float Weight { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Length { get; set; }
        public bool IsDigital { get; set; }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^
                   Price.GetHashCode() ^
                   ShippingPrice.GetHashCode() ^
                   Amount.GetHashCode() ^
                   Weight.GetHashCode() ^
                   Width.GetHashCode() ^
                   Height.GetHashCode() ^
                   Length.GetHashCode() ^
                   IsDigital.GetHashCode();
        }
    }
}