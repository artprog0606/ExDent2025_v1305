//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;

namespace AdvantShop.Orders
{
    public enum TypeOrderItem
    {
        Product,
        BookingService
    }

    [Serializable]
    public class OrderItem : IOrderItem
    {
        public OrderItem()
        {
            AccrueBonuses = true;
        }

        public int OrderItemID { get; set; }

        public int OrderID { get; set; }

        public int? ProductID { get; set; }
        
        public int? BookingServiceId { get; set; }
        public TypeOrderItem TypeItem { get; set; }

        [Compare("Core.Orders.OrderItem.ArtNo")]
        public string ArtNo { get; set; }

        [Compare("Core.Orders.OrderItem.BarCode")]
        public string BarCode { get; set; }

        [Compare("Core.Orders.OrderItem.Name")]
        public string Name { get; set; }

        [Compare("Core.Orders.OrderItem.Price")]
        public float Price { get; set; }

        [Compare("Core.Orders.OrderItem.Amount")]
        public float Amount { get; set; }

        public float DecrementedAmount { get; set; }

        [Compare("Core.Orders.OrderItem.Color")]
        public string Color { get; set; }

        [Compare("Core.Orders.OrderItem.Size")]
        public string Size { get; set; }

        [Compare("Core.Orders.OrderItem.IsCouponApplied")]
        public bool IsCouponApplied { get; set; }

        public float SupplyPrice { get; set; }

        [Compare("Core.Orders.OrderItem.Weight")]
        public float Weight { get; set; }
        
        public bool IsMarkingRequired { get; set; }

        public int? PhotoID { get; set; }

        public bool IgnoreOrderDiscount { get; set; }
        public bool AccrueBonuses { get; set; }

        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public ePaymentSubjectType PaymentSubjectType { get; set; }
        public ePaymentMethodType PaymentMethodType { get; set; }

        public MeasureType? MeasureType { get; set; }
        public string Unit { get; set; }
        
        public string DownloadLink { get; set; }

        [NonSerialized]
        private ProductPhoto _photo;

        [XmlIgnore]
        public ProductPhoto Photo
        {
            get
            {
                if (_photo != null)
                    return _photo;

                _photo = PhotoID != null
                            ? PhotoService.GetPhoto<ProductPhoto>(PhotoID.Value, PhotoType.Product)
                            : null;

                if (_photo == null)
                    _photo = ProductID != null
                            ? PhotoService.GetPhotoByObjId<ProductPhoto>(ProductID.Value, PhotoType.Product)
                            : null;

                if (_photo == null)
                    _photo = new ProductPhoto(0, PhotoType.Product, "");

                return _photo;
            }
        }

        public int? TaxId { get; set; }
        public string TaxName { get; set; }
        public TaxType? TaxType { get; set; }
        public float? TaxRate { get; set; }
        public bool? TaxShowInPrice { get; set; }

        private List<EvaluatedCustomOptions> _selectedOptions;

        private bool _selectedOptionsLoaded;
        public List<EvaluatedCustomOptions> SelectedOptions
        {
            get
            {
                if (_selectedOptionsLoaded)
                    return _selectedOptions;

                _selectedOptionsLoaded = true;
                return _selectedOptions = OrderService.GetOrderCustomOptionsByOrderItemId(OrderItemID);
            }
            set
            {
                _selectedOptions = value;
                _selectedOptionsLoaded = true;
            }
        }

        public bool IsGift { get; set; }


        /// <summary>
        /// Изменяли цену?
        /// </summary>
        public bool IsCustomPrice { get; set; }
        
        /// <summary>
        /// Цена товара на момент заказа (без скидок)
        /// </summary>
        public float? BasePrice { get; set; }
        
        /// <summary>
        /// Скидка в процентах
        /// </summary>
        public float DiscountPercent { get; set; }
        
        /// <summary>
        /// Скидка в валюте
        /// </summary>
        public float DiscountAmount { get; set; }
        
        /// <summary>
        /// Не применять другие скидки, купоны, бонусы и тд
        /// </summary>
        public bool DoNotApplyOtherDiscounts { get; set; }

        /// <summary>
        /// Товар по купону
        /// </summary>
        public bool IsByCoupon { get; set; }

        public static explicit operator OrderItem(ShoppingCartItem item)
        {
            var orderItem = new OrderItem
            {
                ProductID = item.Offer.ProductId,
                Name = item.Offer.Product.Name,
                ArtNo = item.Offer.ArtNo,
                BarCode = item.Offer.BarCode,
                Price = item.PriceWithDiscount,
                Amount = item.Amount,
                SupplyPrice = item.Offer.SupplyPrice,
                SelectedOptions = CustomOptionsService.DeserializeFromXml(item.AttributesXml, item.Offer.Product.Currency.Rate),
                Weight = item.Offer.GetWeight(),
                IsCouponApplied = item.IsCouponApplied,
                Color = item.Offer.ColorID != null ? item.Offer.Color.ColorName : null,
                Size = item.Offer.SizeID != null ? item.Offer.SizeForCategory.GetFullName() : null,
                PhotoID = item.Offer.Photo != null ? item.Offer.Photo.PhotoId : (int?) null,
                IgnoreOrderDiscount = item.ModuleKey.IsNotEmpty() && item.Discount.HasValue,
                AccrueBonuses = item.Offer.Product.AccrueBonuses,
                IsMarkingRequired = item.Offer.Product.IsMarkingRequired,
                Width = item.Offer.GetWidth(),
                Length = item.Offer.GetLength(),
                Height = item.Offer.GetHeight(),
                PaymentMethodType = item.Offer.Product.PaymentMethodType,
                PaymentSubjectType = item.Offer.Product.PaymentSubjectType,
                MeasureType = item.Offer.Product.Unit?.MeasureType,
                Unit = item.Offer.Product.Unit?.DisplayName,
                IsGift = item.IsGift,
                TypeItem = TypeOrderItem.Product,
                IsByCoupon = item.IsByCoupon,
                IsCustomPrice = item.CustomPrice != null,
                BasePrice = item.IsGift ? 0 : (item.CustomPrice ?? item.Offer.RoundedPrice),
                DiscountPercent = item.Offer.Product.Discount.Percent, //item.Discount.Percent,
                DiscountAmount = item.Offer.Product.Discount.Amount.RoundPrice(item.Offer.Product.Currency.Rate, CurrencyService.CurrentCurrency),   //item.Discount.Amount
                DoNotApplyOtherDiscounts = item.Offer.Product.DoNotApplyOtherDiscounts,
                DownloadLink = item.Offer.Product.DownloadLink
            };

            orderItem.IgnoreOrderDiscount |= orderItem.DoNotApplyOtherDiscounts;
            
            var tax = item.Offer.Product.TaxId != null ? TaxService.GetTax(item.Offer.Product.TaxId.Value) : null;
            if (tax != null && tax.Enabled)
            {
                orderItem.TaxId = tax.TaxId;
                orderItem.TaxName = tax.Name;
                orderItem.TaxType = tax.TaxType;
                orderItem.TaxRate = tax.Rate;
                orderItem.TaxShowInPrice = tax.ShowInPrice;
            }

            return orderItem;
        }


        public static explicit operator OrderItem(LeadItem item)
        {
            var product = item.ProductId != null ? ProductService.GetProduct(item.ProductId.Value) : null;
            var offer = OfferService.GetOffer(item.ArtNo);

            var orderItem = new OrderItem
            {
                ProductID = item.ProductId,
                Name = item.Name,
                ArtNo = item.ArtNo,
                BarCode = item.BarCode,
                Price = item.Price,
                Amount = item.Amount,
                Weight = item.Weight,
                Color = item.Color,
                Size = item.Size,
                PhotoID = item.PhotoId,
                AccrueBonuses = product?.AccrueBonuses ?? true,
                IsMarkingRequired = product?.IsMarkingRequired ?? false,
                Width = item.Width,
                Length = item.Length,
                Height = item.Height,
                MeasureType = product?.Unit?.MeasureType,
                Unit = product?.Unit?.DisplayName,
                SupplyPrice = offer?.SupplyPrice ?? 0,
                TypeItem = TypeOrderItem.Product,
                SelectedOptions = CustomOptionsService.DeserializeFromJson(item.CustomOptionsJson, product != null ? product.Currency.Rate : CurrencyService.CurrentCurrency.Rate),
                
                IsCustomPrice = false,
                BasePrice = offer?.RoundedPrice ?? item.Price,
                DiscountPercent = product?.Discount.Percent ?? 0,
                DiscountAmount = product != null ? product.Discount.Amount.RoundPrice(product.Currency.Rate, CurrencyService.CurrentCurrency) : 0,
                DoNotApplyOtherDiscounts = product?.DoNotApplyOtherDiscounts ?? false
            };
            if (product != null)
            {
                orderItem.PaymentMethodType = product.PaymentMethodType;
                orderItem.PaymentSubjectType = product.PaymentSubjectType;
                orderItem.MeasureType = product.Unit?.MeasureType;
                orderItem.Unit = product.Unit?.DisplayName;
            }

            orderItem.IgnoreOrderDiscount |= orderItem.DoNotApplyOtherDiscounts;

            var tax = offer != null && offer.Product.TaxId != null ? TaxService.GetTax(offer.Product.TaxId.Value) : null;
            if (tax != null && tax.Enabled)
            {
                orderItem.TaxId = tax.TaxId;
                orderItem.TaxName = tax.Name;
                orderItem.TaxType = tax.TaxType;
                orderItem.TaxRate = tax.Rate;
                orderItem.TaxShowInPrice = tax.ShowInPrice;
            }

            return orderItem;
        }

        public static explicit operator OrderItem(BookingItem item)
        {
            var orderItem = new OrderItem
            {
                ProductID = null,
                Name = item.Name,
                ArtNo = item.ArtNo,
                Price = item.Price,
                Amount = item.Amount,
                PaymentMethodType = ePaymentMethodType.full_prepayment,
                PaymentSubjectType = ePaymentSubjectType.payment,
                MeasureType = Core.Services.Catalog.MeasureType.Piece,
                BookingServiceId = item.ServiceId,
                TypeItem = TypeOrderItem.BookingService
            };
            //var tax = item.Service.TaxId != null ? TaxService.GetTax(item.Offer.Product.TaxId.Value) : null;
            //if (tax != null && tax.Enabled)
            //{
            //    orderItem.TaxId = tax.TaxId;
            //    orderItem.TaxName = tax.Name;
            //    orderItem.TaxType = tax.TaxType;
            //    orderItem.TaxRate = tax.Rate;
            //    orderItem.TaxShowInPrice = tax.ShowInPrice;
            //}

            return orderItem;
        }

        public static explicit operator OrderItem(Offer offer)
        {
            var orderItem = new OrderItem
            {
                ProductID = offer.ProductId,
                Name = offer.Product.Name,
                ArtNo = offer.ArtNo,
                BarCode = offer.BarCode,
                Price = offer.BasePrice,
                Amount = offer.Product.MinAmount ?? offer.Product.Multiplicity,
                SupplyPrice = offer.SupplyPrice,
                Weight = offer.Weight ?? 0,
                IsCouponApplied = true,
                Color = offer.ColorID != null ? offer.Color.ColorName : null,
                Size = offer.SizeID != null ? offer.SizeForCategory.GetFullName() : null,
                PhotoID = offer.Photo?.PhotoId ?? (int?)null,
                AccrueBonuses = offer.Product.AccrueBonuses,
                IsMarkingRequired = offer.Product.IsMarkingRequired,
                Width = offer.Width ?? 0,
                Length = offer.Length ?? 0,
                Height = offer.Height ?? 0,
                PaymentMethodType = offer.Product.PaymentMethodType,
                PaymentSubjectType = offer.Product.PaymentSubjectType,
                MeasureType = offer.Product.Unit?.MeasureType,
                Unit = offer.Product.Unit?.DisplayName,
                TypeItem = TypeOrderItem.Product,

                BasePrice = offer.BasePrice,
                DiscountPercent = offer.Product.Discount.Percent,
                DiscountAmount = offer.Product.Discount.Amount.RoundPrice(offer.Product.Currency.Rate, CurrencyService.CurrentCurrency),
                DoNotApplyOtherDiscounts = offer.Product.DoNotApplyOtherDiscounts
            };

            orderItem.IgnoreOrderDiscount |= orderItem.DoNotApplyOtherDiscounts;

            var tax = offer.Product.TaxId != null ? TaxService.GetTax(offer.Product.TaxId.Value) : null;
            if (tax != null && tax.Enabled)
            {
                orderItem.TaxId = tax.TaxId;
                orderItem.TaxName = tax.Name;
                orderItem.TaxType = tax.TaxType;
                orderItem.TaxRate = tax.Rate;
                orderItem.TaxShowInPrice = tax.ShowInPrice;
            }

            return orderItem;
        }

        public static bool operator ==(OrderItem first, OrderItem second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (((object)first == null) || ((object)second == null))
            {
                return false;
            }

            return first.ProductID == second.ProductID && first.ArtNo == second.ArtNo && 
                   first.Color == second.Color && first.Size == second.Size && 
                   first.SelectedOptions.SequenceEqual(second.SelectedOptions) && 
                   first.IsGift == second.IsGift && first.TypeItem == second.TypeItem &&
                   first.IsCustomPrice == second.IsCustomPrice && first.IsCouponApplied == second.IsCouponApplied &&
                   first.IsByCoupon == second.IsByCoupon;
        }

        public static bool operator !=(OrderItem first, OrderItem second)
        {
            return !(first == second);
        }

        public bool Equals(OrderItem other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other.ArtNo == ArtNo &&
                other.Name == Name &&
                other.ProductID == ProductID &&
                other.Amount == Amount &&
                other.Price == Price &&
                other.SupplyPrice == SupplyPrice &&
                other.Color == Color &&
                other.Size == Size &&
                Equals(other.SelectedOptions, SelectedOptions) &&
                other.OrderItemID == OrderItemID &&
                other.IsGift == IsGift &&
                other.TypeItem == TypeItem &&
                other.IsByCoupon == IsByCoupon)
            {
                return true;
            }

            //WARNING !!!!!! Equals() is same shit as == operator !!!!!!!!!!!
            return other == this;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj.GetType() == typeof(OrderItem) && Equals((OrderItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ProductID ?? 1 * 397 ^ Amount.GetHashCode() * 397 ^ (SelectedOptions != null ? SelectedOptions.AggregateHash() : 1);
            }
        }
    }
}