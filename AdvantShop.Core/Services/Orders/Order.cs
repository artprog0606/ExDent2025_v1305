//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Payment;
using AdvantShop.Shipping;
using AdvantShop.Taxes;
using AdvantShop.Repository.Currencies;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Helpers;
using System.Text.RegularExpressions;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Configuration;
using Newtonsoft.Json;

namespace AdvantShop.Orders
{
    public class Order : IOrder, IBizObject, ITriggerObject
    {
        public int OrderID { get; set; }

        [Compare("Core.Orders.Order.Number")]
        public string Number { get; set; }

        public Guid Code { get; set; }


        private PaymentDetails _paymentDetails;
        public PaymentDetails PaymentDetails
        {
            get => _paymentDetails ?? (_paymentDetails = OrderService.GetPaymentDetails(OrderID));
            set => _paymentDetails = value;
        }

        public bool Payed => PaymentDate != null;

        private List<OrderItem> _orderItems;
        public List<OrderItem> OrderItems
        {
            get => _orderItems ?? (_orderItems = OrderService.GetOrderItems(OrderID));
            set => _orderItems = value;
        }

        private List<GiftCertificate> _orderCertificates;
        public List<GiftCertificate> OrderCertificates
        {
            get => _orderCertificates ?? (_orderCertificates = GiftCertificateService.GetOrderCertificates(OrderID));
            set => _orderCertificates = value;
        }

        //-------------------------------
        private OrderCustomer _orderCustomer;
        public OrderCustomer OrderCustomer
        {
            get => _orderCustomer ?? (_orderCustomer = OrderService.GetOrderCustomer(OrderID));
            set => _orderCustomer = value;
        }

        public IOrderCustomer GetOrderCustomer()
        {
            return OrderCustomer;
        }

        private OrderCurrency _orderCurrency;
        public OrderCurrency OrderCurrency
        {
            get
            {
                if (_orderCurrency != null)
                    return _orderCurrency;

                return _orderCurrency = OrderService.GetOrderCurrency(OrderID) ?? CurrencyService.CurrentCurrency;
            }
            set => _orderCurrency = value;
        }

        private OrderPickPoint _orderPickPoint;
        public OrderPickPoint OrderPickPoint
        {
            get => _orderPickPoint ?? (_orderPickPoint = OrderService.GetOrderPickPoint(OrderID));
            set => _orderPickPoint = value;
        }

        private List<OrderTax> _taxes;
        public List<OrderTax> Taxes
        {
            get
            {
                if (_taxes == null)
                {
                    if (PaymentMethodTax != null && PaymentMethodTax.TaxType != TaxType.None)
                    {
                        _taxes = new List<OrderTax>();
                        // стоимость товара / (100 + ндс) * ндс
                        var vat = PaymentMethodTax.TaxType == TaxType.VatWithout ? null : (float?)Math.Round(Sum / (100 + PaymentMethodTax.Rate) * PaymentMethodTax.Rate, 2);
                        _taxes.Add(
                            new OrderTax
                            {
                                TaxId = PaymentMethodTax.TaxId,
                                ShowInPrice = PaymentMethodTax.ShowInPrice,
                                Name = PaymentMethodTax.Name,
                                Rate = PaymentMethodTax.TaxType == TaxType.VatWithout ? null : (float?)PaymentMethodTax.Rate,
                                Sum = vat,
                            });
                    }
                    else
                    {
                        _taxes = TaxService.GetOrderTaxes(OrderItems, Sum, ShippingCostWithDiscount, ShippingTaxType);
                    }
                }
                return _taxes;
            }
            set => _taxes = value;
        }

        private int _shippingMethodId;
        public int ShippingMethodId
        {
            get => _shippingMethodId;
            set
            {
                if (_shippingMethodId != value)
                    _shippingMethod = null;

                _shippingMethodId = value;
            }
        }

        private ShippingMethod _shippingMethod;
        public ShippingMethod ShippingMethod => _shippingMethod ?? (_shippingMethod = ShippingMethodService.GetShippingMethod(ShippingMethodId));

        [Compare("Core.Orders.Order.ShippingName")]
        public string ArchivedShippingName { get; set; }

        // почему-то в интерфейсей два поля с названием доставки (выпиливать не стал, т.к. модули используют этот интерфейс)
        // в классе, поле ShippingMethod нужно для возврата самого метода
        string IOrder.ShippingMethod => ShippingMethodName;

        public string ShippingMethodName => ShippingMethod != null ? ShippingMethod.Name : ArchivedShippingName;

        private int _paymentMethodId;
        public int PaymentMethodId
        {
            get => _paymentMethodId;
            set
            {
                if (_paymentMethodId != value)
                    _paymentMethod = null;

                _paymentMethodId = value;
            }
        }

        private PaymentMethod _paymentMethod;
        public PaymentMethod PaymentMethod => _paymentMethod ?? (_paymentMethod = PaymentService.GetPaymentMethod(PaymentMethodId));

        private TaxElement _paymentMethodTax;
        public TaxElement PaymentMethodTax => _paymentMethodTax ?? (_paymentMethodTax = PaymentMethod != null && PaymentMethod.TaxId.HasValue ? TaxService.GetTax(PaymentMethod.TaxId.Value) : null);

        [Compare("Core.Orders.Order.PaymentName")]
        public string ArchivedPaymentName { get; set; }

        public string PaymentMethodName => PaymentMethod != null ? PaymentMethod.Name : ArchivedPaymentName;

        private OrderStatus _orderStatus;
        public OrderStatus OrderStatus
        {
            get => _orderStatus ?? (_orderStatus = OrderStatusService.GetOrderStatus(OrderStatusId));
            set => _orderStatus = value;
        }

        public IOrderStatus GetOrderStatus()
        {
            return OrderStatus;
        }

        public string PreviousStatus { get; set; }

        [Compare("Core.Orders.Order.AffiliateID")]
        public int AffiliateID { get; set; }

        
        public int? ManagerId { get; set; }

        /// <summary>
        /// Скидка (процент)
        /// </summary>
        [Compare("Core.Orders.Order.OrderDiscount")]
        public float OrderDiscount { get; set; }

        /// <summary>
        /// Скидка (число)
        /// </summary>
        [Compare("Core.Orders.Order.OrderDiscountValue")]
        public float OrderDiscountValue { get; set; }

        [Compare("Core.Orders.Order.OrderDate")]
        public DateTime OrderDate { get; set; }

        [Compare("Core.Orders.Order.PaymentDate")]
        public DateTime? PaymentDate { get; set; }

        [Compare("Core.Orders.Order.CustomerComment")]
        public string CustomerComment { get; set; }

        [Compare("Core.Orders.Order.StatusComment")]
        public string StatusComment { get; set; }

        public string AdditionalTechInfo { get; set; }

        [Compare("Core.Orders.Order.AdminOrderComment")]
        public string AdminOrderComment { get; set; }
        
        public bool Decremented { get; set; }

        [Compare("Core.Orders.Order.ShippingCost")]
        public float ShippingCost { get; set; }

        // Сертификат может покрываеть стоимость доставки
        public float ShippingCostWithDiscount
        {
            get
            {
                var shippingCost = Math.Max( // если скидка больше стоимости заказа, то получим отрицательное,а это значит, что скидка покрывает сумму доставки
                           ShippingCost - Math.Max( // если скидка маленькая, то получим отрицательное, а это значит, что скидка не распространяется на доставку
                               TotalDiscount - OrderItems.Sum(item => item.Price * item.Amount) - PaymentCost,
                               0),
                           0);
                if (shippingCost > 0f)
                    shippingCost -= Math.Max( // если скидка маленькая, то получим отрицательное, а это значит, что скидка не распространяется на доставку
                        BonusCost - OrderItems.Where(item => item.DoNotApplyOtherDiscounts is false).Sum(item => item.Price * item.Amount),
                        0f);
                return Math.Max(shippingCost, 0f).RoundPrice(OrderCurrency.CurrencyValue, OrderCurrency);
            }
        }

        public TaxType ShippingTaxType { get; set; }
        
        public ePaymentMethodType ShippingPaymentMethodType { get; set; } = ePaymentMethodType.full_prepayment;
        public ePaymentSubjectType ShippingPaymentSubjectType { get; set; } = ePaymentSubjectType.payment;

        [Compare("Core.Orders.Order.PaymentCost")]
        public float PaymentCost { get; set; }

        [Compare("Core.Orders.Order.BonusCost")]
        public float BonusCost { get; set; }

        [Compare("Core.Orders.Order.BonusCardNumber")]
        public long? BonusCardNumber { get; set; }

        /// <summary>
        /// Total order discount
        /// </summary>
        public float DiscountCost { get; set; }
        
        public float TaxCost { get; set; }
        
        public float SupplyTotal { get; set; }
        
        public int OrderStatusId { get; set; }

        public float Sum { get; set; }

        [Compare("Core.Orders.Order.GroupName")]
        public string GroupName { get; set; }

        [Compare("Core.Orders.Order.GroupDiscount")]
        public float GroupDiscount { get; set; }
        
        [Compare("Core.Orders.Order.Certificate")]
        public OrderCertificate Certificate { get; set; }

        private OrderCoupon _coupon;
        private bool _couponLoaded;
        
        [Compare("Core.Orders.Order.Coupon")]
        public OrderCoupon Coupon
        {
            get
            {
                if (_couponLoaded)
                    return _coupon;

                _couponLoaded = true;
                return _coupon = OrderService.GetOrderCoupon(OrderID);
            }
            set
            {
                _coupon = value;
                _couponLoaded = true;
            }
        }

        public string GroupDiscountString => GroupName + (GroupDiscount != 0 ? " (" + GroupDiscount + "%)" : "");


        public float TotalDiscount
        {
            get
            {
                float discount = 0;
                
                discount += OrderDiscount > 0
                    ? (OrderDiscount * OrderItems.Where(item => !item.IgnoreOrderDiscount).Sum(item => item.Price * item.Amount) / 100)
                        .RoundPrice(OrderCurrency.CurrencyValue, OrderCurrency)
                    : 0;
                discount += OrderDiscountValue != 0 
                    ? OrderDiscountValue.RoundPrice(OrderCurrency.CurrencyValue, OrderCurrency) 
                    : 0;

                if (Certificate != null)
                {
                    discount += Certificate.Price != 0 ? Certificate.Price.RoundPrice(OrderCurrency.CurrencyValue, OrderCurrency) : 0;
                }

                if (Coupon != null)
                {
                    switch (Coupon.Type)
                    {
                        case CouponType.Fixed:
                            var productsPrice = OrderItems.Where(p => p.IsCouponApplied)
                                                              .Sum(p => p.Price * p.Amount);
                            var couponFixedPrice = productsPrice >= Coupon.Value ? Coupon.Value : productsPrice;
                            discount += couponFixedPrice.RoundPrice(OrderCurrency.CurrencyValue, OrderCurrency);
                            break;
                        case CouponType.Percent:
                            var couponPercentPrice = OrderItems.Where(p => p.IsCouponApplied)
                                                                   .Sum(p => Coupon.Value * p.Price / 100 * p.Amount);
                            discount += couponPercentPrice.RoundPrice(OrderCurrency.CurrencyValue, OrderCurrency);
                            break;
                    }
                }

                return discount.RoundPrice(OrderCurrency.CurrencyValue, OrderCurrency);
            }
        }

        [Compare("Core.Orders.Order.UseIn1C")]
        public bool UseIn1C { get; set; }

        public DateTime ModifiedDate { get; set; }

        [Compare("Core.Orders.Order.ManagerConfirmed")]
        public bool ManagerConfirmed { get; set; }
        
        public int OrderSourceId { get; set; }

        private OrderSource _orderSource { get; set; }

        [Compare("Core.Orders.Order.OrderSourceId")]
        public OrderSource OrderSource { get { return _orderSource ?? OrderSourceService.GetOrderSource(OrderSourceId); } }

        [Compare("Core.Orders.Order.CustomData")]
        public string CustomData { get; set; }

        public bool IsDraft { get; set; }

        public DateTime? DeliveryDate { get; set; }
        private string _deliveryTime;
        public string DeliveryTime
        {
            get => _deliveryTime ?? (_deliveryTime = DeliveryInterval.ToString());
            set => DeliveryInterval = new DeliveryInterval(_deliveryTime = value);
        }
        private DeliveryInterval _deliveryInterval;
        public DeliveryInterval DeliveryInterval
        {
            get => _deliveryInterval ?? (_deliveryInterval = new DeliveryInterval(_deliveryTime));
            set => _deliveryInterval = value;
        }

        [Compare("Core.Orders.Order.TrackNumber")]
        public string TrackNumber { get; set; }

        public bool IsFromAdminArea { get; set; }

        public int? LeadId { get; set; }

        public int? LpId { get; set; }

        public bool IsSendedToGA { get; set; }

        public string PayCode { get; set; }

        [Compare("Core.Orders.Order.TotalWeight")]
        public float? TotalWeight { get; set; }

        [Compare("Core.Orders.Order.TotalLength")]
        public float? TotalLength { get; set; }

        [Compare("Core.Orders.Order.TotalWidth")]
        public float? TotalWidth { get; set; }

        [Compare("Core.Orders.Order.TotalHeight")]
        public float? TotalHeight { get; set; }

        private Manager _manager;

        [Compare("Core.Orders.Order.ManagerId")]
        public Manager Manager => _manager ?? (_manager = ManagerId.HasValue ? ManagerService.GetManager(ManagerId.Value) : null);

        public bool AvailablePaymentCashOnDelivery { get; set; }

        public bool AvailablePaymentPickPoint { get; set; }
        
        /// <summary>
        /// Id покупателя, который мб связан с заказом (в случаях когда незарегестрированный покупатель указал его почту или телефон)
        /// </summary>
        public Guid? LinkedCustomerId { get; set; }

        public bool DontCallBack { get; set; }

        public EnTypeOfReceivingMethod? ReceivingMethod { get; set; }

        public int? CountDevices { get; set; }

        private OrderRecipient _orderRecipient;
        public OrderRecipient OrderRecipient
        {
            get => _orderRecipient ?? (_orderRecipient = OrderService.GetOrderRecipient(OrderID));
            set => _orderRecipient = value;
        }

        private OrderReview _orderReview;
        public OrderReview OrderReview
        {
            get => _orderReview ?? (_orderReview = OrderService.GetOrderReview(OrderID));
            set => _orderReview = value;
        }

        private List<int> _warehouseIds;

        /// <summary>
        /// Склады на момент заказа
        /// </summary>
        public List<int> WarehouseIds
        {
            get
            {
                if (_warehouseIds != null)
                    return _warehouseIds;

                return _warehouseIds = !string.IsNullOrEmpty(WarehouseIdsJson)
                    ? JsonConvert.DeserializeObject<List<int>>(WarehouseIdsJson)
                    : new List<int>();
            }
            set
            {
                _warehouseIds = value ?? new List<int>();
                WarehouseIdsJson = value != null && value.Count > 0 ? JsonConvert.SerializeObject(value) : null;
            }
        }
        
        public string WarehouseIdsJson { get; set; }

        public TriggerProcessObject GetTriggerProcessObject()
        {
            if (OrderCustomer == null)
                return null;

            return new TriggerProcessObject()
            {
                EntityId = OrderID,
                EventObjId = OrderStatusId,
                Email = OrderCustomer.Email,
                Phone = OrderCustomer.StandardPhone ?? StringHelper.ConvertToStandardPhone(OrderCustomer.Phone) ?? 0,
                CustomerId = OrderCustomer.CustomerID
            };
        }
    }

    public class OrderAutocomplete
    {
        public int OrderID { get; set; }
        public string Number { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string MobilePhone { get; set; }

        public DateTime OrderDate { get; set; }

        public float Sum { get; set; }

        public string StatusName { get; set; }
    }

    public enum OrderContactType
    {
        ShippingContact,
        BillingContact
    }


    [Serializable]
    [Obsolete("OrderContact is deprecated, please use OrderCustomer instead.")]
    public class OrderContact
    {
        public int OrderContactId { get; set; }

        [Compare("Core.Orders.OrderContact.Name")]
        public string Name { get; set; }

        [Compare("Core.Orders.OrderContact.Country")]
        public string Country { get; set; }

        [Compare("Core.Orders.OrderContact.Zone")]
        public string Zone { get; set; }

        [Compare("Core.Orders.OrderContact.City")]
        public string City { get; set; }

        [Compare("Core.Orders.OrderContact.Zip")]
        public string Zip { get; set; }

        [Compare("Core.Orders.OrderContact.Address")]
        public string Address { get; set; }

        [Compare("Core.Orders.OrderContact.CustomField1")]
        public string CustomField1 { get; set; }

        [Compare("Core.Orders.OrderContact.CustomField2")]
        public string CustomField2 { get; set; }

        [Compare("Core.Orders.OrderContact.CustomField3")]
        public string CustomField3 { get; set; }
    }

    public class OrderCurrency
    {
        public static implicit operator OrderCurrency(Currency cur)
        {
            return new OrderCurrency
            {
                CurrencyCode = cur.Iso3,
                CurrencyNumCode = cur.NumIso3,
                CurrencyValue = cur.Rate,
                CurrencySymbol = cur.Symbol,
                IsCodeBefore = cur.IsCodeBefore,
                EnablePriceRounding = cur.EnablePriceRounding,
                RoundNumbers = cur.RoundNumbers
            };
        }

        public static implicit operator Currency(OrderCurrency cur)
        {
            return new Currency
            {
                Iso3 = cur.CurrencyCode,
                NumIso3 = cur.CurrencyNumCode,
                Rate = cur.CurrencyValue,
                Symbol = cur.CurrencySymbol,
                IsCodeBefore = cur.IsCodeBefore,
                EnablePriceRounding = cur.EnablePriceRounding,
                RoundNumbers = cur.RoundNumbers
            };
        }

        [Compare("Core.Orders.OrderCurrency.CurrencyCode")]
        public string CurrencyCode { get; set; }
        public int CurrencyNumCode { get; set; }

        [Compare("Core.Orders.OrderCurrency.CurrencyValue")]
        public float CurrencyValue { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsCodeBefore { get; set; }
        
        public float RoundNumbers { get; set; }
        public bool EnablePriceRounding { get; set; }
    }

    public class OrderCoupon
    {
        public string Code { get; set; }
        public CouponType Type { get; set; }
        public float Value { get; set; }
        public string CurrencyIso3 { get; set; }
        public float MinimalOrderPrice { get; set; }
        public bool IsMinimalOrderPriceFromAllCart { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as OrderCoupon;
            if (other == null)
                return false;

            return other.Code == this.Code && 
                   other.Type == this.Type && 
                   other.Value == this.Value &&
                   other.CurrencyIso3 == this.CurrencyIso3 &&
                   other.MinimalOrderPrice == this.MinimalOrderPrice &&
                   other.IsMinimalOrderPriceFromAllCart == this.IsMinimalOrderPriceFromAllCart;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ Type.GetHashCode() ^ Value.GetHashCode() ^ (CurrencyIso3 ?? "").GetHashCode() ^
                   MinimalOrderPrice.GetHashCode() ^ IsMinimalOrderPriceFromAllCart.GetHashCode();
        }

        public override string ToString()
        {
            return $"'{Code}' ({Value}{(Type == CouponType.Percent ? "%" : "")})";
        }

        public OrderCoupon()
        {
        }

        public OrderCoupon(Coupon coupon)
        {
            Code = coupon.Code;
            Type = coupon.Type;
            Value = coupon.Value;
            CurrencyIso3 = coupon.CurrencyIso3;
            MinimalOrderPrice = coupon.MinimalOrderPrice;
            IsMinimalOrderPriceFromAllCart = coupon.IsMinimalOrderPriceFromAllCart;
        }
        
        public static explicit operator OrderCoupon(Coupon coupon)
        {
            return new OrderCoupon
            {
                Code = coupon.Code,
                Type = coupon.Type,
                Value = coupon.GetRate(),
                CurrencyIso3 = coupon.CurrencyIso3,
                MinimalOrderPrice = coupon.MinimalOrderPrice,
                IsMinimalOrderPriceFromAllCart = coupon.IsMinimalOrderPriceFromAllCart
            };
        } 
    }

    public class OrderCertificate
    {
        public string Code { get; set; }
        public float Price { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as OrderCertificate;
            if (other == null)
                return false;

            return other.Code == this.Code && other.Price == this.Price;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ Price.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Сертификат '{0}' ({1})", Code, Price);
        }
    }

    public enum BuyInOneclickPage
    {
        None,
        Product,
        Cart,
        Checkout,
        LandingPage,
        PreOrder
    }

    public class OrderChangedBy
    {
        public string Name { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime ModificationTime { get; set; }


        public OrderChangedBy(string name)
        {
            Name = name;
            ModificationTime = DateTime.Now;
        }
        
        public OrderChangedBy(string name, Guid? customerId)
        {
            Name = name;
            CustomerId = customerId;
            ModificationTime = DateTime.Now;
        }

        public OrderChangedBy(Customer customer)
        {
            if (customer != null)
            {
                Name = customer.FirstName + " " + customer.LastName;
                CustomerId = customer.Id;
            }
            ModificationTime = DateTime.Now;
        }

        public static explicit operator ChangedBy(OrderChangedBy orderChangedBy)
        {
            return
                orderChangedBy != null
                    ? new ChangedBy(orderChangedBy.Name, orderChangedBy.CustomerId, orderChangedBy.ModificationTime)
                    : null;
        }
    }

    public class LastOrdersItem
    {
        public int OrderId { get; set; }
        public string Number { get; set; }
        public DateTime OrderDate { get; set; }
        public float Sum { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        

        public string Color { get; set; }
        public string StatusName { get; set; }

        public string CurrencyCode { get; set; }
        public float CurrencyValue { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsCodeBefore { get; set; }
        public float RoundNumbers { get; set; }
        public bool EnablePriceRounding { get; set; }
    }

    public class DeliveryInterval
    {
        public DeliveryInterval() { }
        /// <summary>
        /// Заполнит интервалы и часовой пояс из строки с нужным форматом
        /// </summary>
        /// <param name="deliveryTime">Строка вида "hh:mm-hh:mm|zzz"</param>
        public DeliveryInterval(string deliveryTime)
        {
            DeliveryTimeStr = deliveryTime;
            if (deliveryTime.IsNotEmpty() && new Regex(@"^\d{2}:\d{2}(-\d{2}:\d{2})?(\|((-)?\d*[,.]??\d*))?$").IsMatch(deliveryTime))
            {
                var intervalArr = deliveryTime.Split('|');
                if (intervalArr.Length == 2)
                {
                    var offsetHour = intervalArr[1].TryParseDouble(true);
                    if (offsetHour.HasValue)
                        TimeZoneOffset = TimeSpan.FromHours(offsetHour.Value);
                }
                var timesArr = intervalArr[0].Split('-');
                if (timesArr.Length != 2)
                {
                    TimeFrom = timesArr[0].TryParseTimeSpan(true);
                }
                else if (timesArr.Length == 2)
                {
                    TimeFrom = timesArr[0].TryParseTimeSpan(true);
                    TimeTo = timesArr[1].TryParseTimeSpan(true);
                }
            }
        }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public TimeSpan? TimeZoneOffset { get; set; }
        public string DeliveryTimeStr { get; set; }
        private string _readableString;
        public string ReadableString
        {
            get
            {
                if (_readableString.IsNotEmpty())
                    return _readableString;
                if (!TimeFrom.HasValue && !TimeTo.HasValue)
                    return _readableString = DeliveryTimeStr;

                return _readableString = string.Concat(
                       TimeFrom.HasValue ? $"{TimeFrom.Value:hh\\:mm}" : string.Empty,
                       TimeFrom.HasValue && TimeTo.HasValue ? "-" : string.Empty,
                       TimeTo.HasValue ? $"{TimeTo.Value:hh\\:mm}" : string.Empty,
                       TimeZoneOffset.HasValue ? $" (GMT {(TimeZoneOffset.Value > TimeSpan.Zero ? "+" : "-")}{TimeZoneOffset.Value:hh\\:mm})" : string.Empty);
            }
        }

        public override string ToString()
        {
            if (!TimeFrom.HasValue && !TimeTo.HasValue)
                return DeliveryTimeStr;
            return string.Concat(
                       TimeFrom.HasValue ? $"{TimeFrom.Value:hh\\:mm}" : string.Empty,
                       TimeFrom.HasValue && TimeTo.HasValue ? "-" : string.Empty,
                       TimeTo.HasValue ? $"{TimeTo.Value:hh\\:mm}" : string.Empty,
                       TimeZoneOffset.HasValue ? $"|{TimeZoneOffset.Value.TotalHours}" : string.Empty);
        }
    }
}