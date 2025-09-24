using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Models.Attachments;

namespace AdvantShop.Web.Admin.Models.Orders.OrdersEdit
{
    public class OrderItemsSummaryModel
    {
        public OrderCurrency OrderCurrency { get; set; }

        public Card BonusCard { get; set; }
        public Purchase BonusCardPurchase { get; set; }
        public float BonusCost { get; set; }
        public float BonusesAvailable { get; set; }
        public bool CanChangeBonusAmount { get; set; }


        public float ProductsCost { get; set; }
        public string ProductsCostStr { get { return ProductsCost.FormatPrice(OrderCurrency); } }

        public float ProductsDiscountPrice { get; set; }
        public string ProductsDiscountPriceStr { get { return ProductsDiscountPrice.FormatPrice(OrderCurrency); } }
        public float OrderDiscount { get; set; }

        public float CouponPrice { get; set; }

        public string CouponPriceStr
        {
            get { return CouponPrice.FormatPrice(OrderCurrency); }
        }

        public string Coupon { get; set; }

        public float CertificatePrice { get; set; }
        public string CertificatePriceStr { get { return CertificatePrice.FormatPrice(OrderCurrency); } }
        public OrderCertificate Certificate { get; set; }


        public string ShippingName { get; set; }
        public int ShippingMethodId { get; set; }
        public string ShippingType { get; set; }
        public float ShippingCost { get; set; }
        public OrderPickPoint OrderPickPoint { get; set; }
        public string ShippingCostStr { get { return ShippingCost.FormatPrice(OrderCurrency); } }

        public string DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public DeliveryInterval DeliveryInterval { get; set; } 

        public string PaymentName { get; set; }
        public int PaymentMethodId { get; set; }
        public float PaymentCost { get; set; }
        public string PaymentCostStr { get { return PaymentCost.FormatPrice(OrderCurrency); } }
        public PaymentDetails PaymentDetails { get; set; }
        public string PaymentKey { get; set; }

        public bool ShowSendBillingLink { get; set; }
        public bool ShowPrintPaymentDetails { get; set; }
        public string PrintPaymentDetailsText { get; set; }
        public string PrintPaymentDetailsLink { get; set; }
        public bool ShowInnPaymentDetails { get; set; }
        public bool ShowCompanyNamePaymentDetails { get; set; }

        public List<KeyValuePair<string, string>> Taxes { get; set; }

        public float Sum { get; set; }
        public string SumStr { get { return Sum.FormatPrice(OrderCurrency); } }
        public float OrderDiscountValue { get; set; }
        public bool Paid { get; set; }

        public bool ShippingUseWeight { get; set; }
        public float TotalWeight { get; set; }
        public float TotalWeightFromShipping { get; set; }

        public bool ShippingUseDemensions { get; set; }

        public string TotalDemensions { get; set; }
        public float TotalHeight { get; set; }
        public float TotalWidth { get; set; }
        public float TotalLength { get; set; }

        public string TotalDemensionsFromShipping { get; set; }
        public float TotalHeightFromShipping { get; set; }
        public float TotalWidthFromShipping { get; set; }
        public float TotalLengthFromShipping { get; set; }

        public string CustomerComment { get; set; }

        public bool IsNotEditedDimensions { get; set; }
        public bool IsNotEditedWeight { get; set; }
        public bool IsDraft { get; set; }

        public bool DontCallBack { get; set; }
        public List<AttachmentModel> CustomerOrderAttachments { get; set; }
        public string FilesHelpText { get; set; }
        public EnTypeOfReceivingMethod? ReceivingMethod { get; set; }
        public int? CountDevices { get; set; }
        public bool ShowCountDevices { get; set; }
    }


    public class LeadItemsSummaryModel
    {
        public Currency LeadCurrency { get; set; }

        public float ProductsCost { get; set; }
        public string ProductsCostStr { get { return ProductsCost.FormatPrice(LeadCurrency); } }

        public float ProductsDiscountPrice { get; set; }
        public string ProductsDiscountPriceStr { get { return ProductsDiscountPrice.FormatPrice(LeadCurrency); } }
        public float Discount { get; set; }
        public float DiscountValue { get; set; }

        public float Sum { get; set; }
        public string SumStr { get { return Sum.FormatPrice(LeadCurrency); } }
        public string SumValueFormat { get { return Sum.ToString(); } }

        public string ShippingName { get; set; }
        public float ShippingCost { get; set; }
        public string ShippingCostStr { get { return ShippingCost.FormatPrice(LeadCurrency); } }
        public OrderPickPoint ShippingPickPoint { get; set; }
        public string DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public int LeadItemsCount { get; set; }

        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Apartment { get; set; }
        public string Structure { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
        public string Zip { get; set; }
    }
}
