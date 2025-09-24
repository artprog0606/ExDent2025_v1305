using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Models.Orders
{
    public partial class OrderItemsModel
    {
        public int OrderId { get; set; }
        public string Number { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderDateFormatted => Culture.ConvertDate(OrderDate);
        public string PaymentDate { get; set; }
        public bool IsPaid => PaymentDate != null;
        public string AdminOrderComment { get; set; }
        public float Sum { get; set; }
        public string SumFormatted => 
            PriceFormatService.FormatPrice(Sum, CurrencyValue, CurrencySymbol, CurrencyCode, IsCodeBefore);


        public int OrderStatusId { get; set; }
        public string StatusName { get; set; }
        public string Color { get; set; }

        public string PaymentMethodName { get; set; }
        public string ShippingMethodName { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingMethod { get; set; }
        
        
        public bool ManagerConfirmed { get; set; }
        public string ManagerId { get; set; }
        
        public string CurrencyCode { get; set; }
        public float CurrencyValue { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsCodeBefore { get; set; }
        public bool EnablePriceRounding { get; set; }
        public float RoundNumbers { get; set; }
        public int CurrencyNumCode { get; set; }

        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DeliveryAddress => StringHelper.AggregateStrings(", ", Zip, Street, House, Structure, Apartment);
        public string Zip { get; set; }
        public string Apartment { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Structure { get; set; }
        public string CompanyName { get; set; }

        public string BuyerName
        {
            get
            {
                if (!string.IsNullOrEmpty(CompanyName))
                    return CompanyName;

                return StringHelper.AggregateStrings(" ", FirstName, LastName);
            }
        }
        
        public Guid ManagerCustomerId { get; set; }
        public string ManagerName { get; set; }

        public bool LoadOrderItems { get; set; }

        private List<OrderItemsPhotoInfo> _orderItemsPhoto;

        public List<OrderItemsPhotoInfo> OrderItemsPhoto
        {
            get
            {
                if (!LoadOrderItems)
                    return null;

                if (_orderItemsPhoto != null)
                    return _orderItemsPhoto;

                _orderItemsPhoto = new List<OrderItemsPhotoInfo>();

                var orderItemsPhotos = OrderService.GetOrderItems(OrderId);

                foreach (var item in orderItemsPhotos)
                {
                    _orderItemsPhoto.Add(new OrderItemsPhotoInfo() 
                    { 
                        isGift = false, 
                        PhotoPath = FoldersHelper.GetImageProductPath(ProductImageType.XSmall, item.Photo.PhotoNameSize2.IsNotEmpty() ? item.Photo.PhotoNameSize2 : item.Photo.PhotoName, false) 
                    });
                }

                if (orderItemsPhotos.Count == 0)
                {
                    var certificates = GiftCertificateService.GetOrderCertificates(OrderId);
                    if (certificates != null && certificates.Count > 0)
                    {
                        foreach (var certificate in certificates)
                        {
                            string path = UrlService.GetUrl() + "Areas/Admin/Content/images/icon/giftIconOrder.png";
                            _orderItemsPhoto.Add(new OrderItemsPhotoInfo() { isGift = true, PhotoPath = path });
                        }
                    }
                }


                return _orderItemsPhoto;
            }
        }

        private List<string> _orderItems;
        public List<string> OrderItems
        {
            get
            {
                if (!LoadOrderItems)
                    return null;
                
                if (_orderItems != null)
                    return _orderItems;

                _orderItems = new List<string>();

                var orderItems = OrderService.GetOrderItems(OrderId);
                var currency = new Currency()
                {
                    Iso3 = CurrencyCode,
                    NumIso3 = CurrencyNumCode,
                    Rate = CurrencyValue,
                    Symbol = CurrencySymbol,
                    IsCodeBefore = IsCodeBefore,
                    EnablePriceRounding = EnablePriceRounding,
                    RoundNumbers = RoundNumbers
                };

                foreach (var item in orderItems)
                {
                    _orderItems.Add(
                        string.Format("{0} {1} - {2}, {3} шт.", 
                            item.Name, item.ArtNo,
                            PriceService.SimpleRoundPrice(item.Price, currency).FormatPrice(currency),
                            item.Amount));
                }

                if (orderItems.Count == 0)
                {
                    var certificates = GiftCertificateService.GetOrderCertificates(OrderId);
                    if (certificates != null && certificates.Count > 0)
                    {
                        foreach (var certificate in certificates)
                        {
                            _orderItems.Add(
                                $"{LocalizationService.GetResource("Admin.Orders.OrderItemsModel.Certificate")} {certificate.CertificateCode}");
                        }
                    }
                }

                return _orderItems;
            }
        }

        public partial class OrderItemsPhotoInfo
        {
            public bool isGift { get; set; }
            public string PhotoPath { get; set; }
        }

        public string GroupName { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public string DeliveryDateTimeFormatted
        {
            get
            {
                string deliveryDateTimeStr = null;
                if (DeliveryDate.HasValue)
                    deliveryDateTimeStr = DeliveryDate.Value.ToShortDateTime();
                if (DeliveryTime.IsNotEmpty())
                    deliveryDateTimeStr += deliveryDateTimeStr.IsNullOrEmpty() 
                        ? new DeliveryInterval(DeliveryTime).ReadableString
                        : $"<br />{new DeliveryInterval(DeliveryTime).ReadableString}";
                return deliveryDateTimeStr;
            }
        }
    }
}
