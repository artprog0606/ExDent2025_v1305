using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Payment;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Security;
using AdvantShop.Shipping;
using AdvantShop.Trial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Taxes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Saas;
using AdvantShop.FilePath;
using AdvantShop.Core.Services.Attachments;
using System.IO;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Partners;
using AdvantShop.GeoModes;

namespace AdvantShop.Orders
{
    public class MyCheckout
    {
        private Guid? _linkedCustomerId;
        
        public CheckoutData Data { get; set; }
        public ShoppingCart Cart { get; private set; }

        public MyCheckout(ShoppingCart cart)
        {
            Cart = cart;
        }

        public List<BaseShippingOption> AvailableShippingOptions(
            List<PreOrderItem> preorderList = null,
            CalculationVariants? typeCalculationVariants = null)
        {
            var ignoreCartItems = ModulesExecuter.GetIgnoreShippingCartItems();
            if (ignoreCartItems.Count > 0)
            {
                foreach (var item in ignoreCartItems)
                    Cart.Remove(item);
            }

            var configuratorShippingCalculation =
                ShippingCalculationConfigurator.Configure()
                                               .ByMyCheckout(this)
                                               .WithCurrency(CurrencyService.CurrentCurrency);
            if (preorderList != null)
                configuratorShippingCalculation
                   .ByShoppingCart(null)
                   .WithPreOrderItems(preorderList);
            
            var shippingManager = new ShippingManager(configuratorShippingCalculation.Build());
            return typeCalculationVariants.HasValue
                ? shippingManager.GetOptions(typeCalculationVariants.Value)
                : shippingManager.GetOptions();
        }

        public void UpdateSelectShipping(
            List<PreOrderItem> preorderList, BaseShippingOption shipping, 
            List<BaseShippingOption> shippingOptions = null,
            CalculationVariants? typeCalculationVariants = null,
            int? warehouseId = null)
        {
            this.Data.SelectShipping = shipping;
            this.Data.WarehouseId = warehouseId;

            var ignoreCartItems = ModulesExecuter.GetIgnoreShippingCartItems();
            if (ignoreCartItems.Count > 0)
            {
                foreach (var item in ignoreCartItems)
                    Cart.Remove(item);
            }

            List<PreOrderItem> items = null;
            BaseShippingOption option = null;

            if (shippingOptions is null)
            {

                var configuratorShippingCalculation =
                    ShippingCalculationConfigurator.Configure()
                                                   .ByMyCheckout(this)
                                                   .WithCurrency(CurrencyService.CurrentCurrency);
                if (preorderList != null)
                    configuratorShippingCalculation
                       .ByShoppingCart(null)
                       .WithPreOrderItems(preorderList);

                var shippingManager = new ShippingManager(configuratorShippingCalculation.Build()).PreferShippingOptionFromParameters();
                shippingOptions = typeCalculationVariants.HasValue
                    ? shippingManager.GetOptions(typeCalculationVariants.Value)
                    : shippingManager.GetOptions();
            }
            
            option = (shipping != null ? shippingOptions.FirstOrDefault(x => x.Id == shipping.Id) : null) ??
                     shippingOptions.FirstOrDefault();

            if (option != null)
            {
                option.UpdateFromBase(shipping);
                if (this.Data.SelectPayment != null)
                    if (option.ApplyPay(this.Data.SelectPayment))
                    {
                        var totalPrice = (Cart.TotalPrice - Cart.TotalDiscount).RoundPrice(CurrencyService.CurrentCurrency.Rate, CurrencyService.CurrentCurrency);
                        var options = new List<BaseShippingOption> { option.DeepCloneJson() };
                        if (items == null)
                            items = preorderList ?? Cart.Select(shpItem => new PreOrderItem(shpItem)).ToList();
                        
                        var modules = AttachedModules.GetModuleInstances<IShippingCalculator>();
                        if (modules != null && modules.Count != 0)
                        {
                            foreach (var module in modules)
                                module.ProcessOptions(options, items, totalPrice);
                        }

                        // Если модуль делает доставку бесплатной
                        if (Math.Abs(options[0].Rate - option.Rate) > 0.001)
                            option.Rate = options[0].Rate;
                    }
            }

            this.Data.SelectShipping = option;
            this.Data.TypeCalculationVariants = typeCalculationVariants;
            this.SetPriceRule(shippingOptions);
            this.Update();
        }

        public List<BasePaymentOption> AvailablePaymentOptions(List<PreOrderItem> preorderList = null)
        {
            var configuratorPaymentCalculation =
                PaymentCalculationConfigurator.Configure()
                                              .ByMyCheckout(this);
            if (preorderList != null)
                configuratorPaymentCalculation
                   .ByShoppingCart(null)
                   .WithPreOrderItems(preorderList);

            var manager = new PaymentManager(configuratorPaymentCalculation.Build());
            var result = manager.GetOptions();
            return result;
        }

        public bool UpdateSelectPayment(List<PreOrderItem> preorderList, BasePaymentOption payment, List<BasePaymentOption> paymentOptions = null)
        {
            this.Data.SelectPayment = payment;

            var configuratorPaymentCalculation =
                PaymentCalculationConfigurator.Configure()
                                              .ByMyCheckout(this);
            if (preorderList != null)
                configuratorPaymentCalculation
                   .ByShoppingCart(null)
                   .WithPreOrderItems(preorderList);

            BasePaymentOption option = null;
            PaymentManager manager = null;
            var paymentCalculationParameters = configuratorPaymentCalculation.Build();

            if (paymentOptions == null)
            {
                manager = new PaymentManager(paymentCalculationParameters);
                paymentOptions = manager.PreferPaymentOptionFromParameters().GetOptions();
                
                if (payment != null && paymentOptions.Count == 0)
                    paymentOptions = manager.WithoutPreferPaymentOptionFromParameters().GetOptions();
            }

            option = (payment != null ? paymentOptions.FirstOrDefault(x => x.Id == payment.Id) : null) ??
                         paymentOptions.FirstOrDefault();

            if (option != null)
                option.Update(payment);

            this.Data.SelectPayment = option;

            this.SetPriceRule(paymentOptions);

            var prevFinalRateShipping = this.Data.SelectShipping != null
                ? this.Data.SelectShipping.FinalRate
                : 0f;

            if (this.Data.SelectShipping != null && this.Data.SelectShipping.ApplyPay(this.Data.SelectPayment))
            {
                // стоимость доставки изменилась, нужно обновить оплату, 
                // т.к. ее нацена зависит и от стоимости доставки (наценка метода оплаты)
                if (prevFinalRateShipping != this.Data.SelectShipping.FinalRate && manager != null)
                    //UpdatePaymentByNewShipping
                    this.Data.SelectPayment = (BasePaymentOption) Activator.CreateInstance(
                        this.Data.SelectPayment.GetType(), 
                        PaymentService.GetPaymentMethod(this.Data.SelectPayment.Id),
                        paymentCalculationParameters.ItemsTotalPriceWithDiscounts + this.Data.SelectShipping.FinalRate);

                var totalPrice = (Cart.TotalPrice - Cart.TotalDiscount).RoundPrice(CurrencyService.CurrentCurrency.Rate, CurrencyService.CurrentCurrency);
                
                var options = new List<BaseShippingOption> { this.Data.SelectShipping };
                
                var modules = AttachedModules.GetModuleInstances<IShippingCalculator>();
                if (modules != null && modules.Count != 0)
                {
                    foreach (var module in modules)
                        module.ProcessOptions(options, paymentCalculationParameters.PreOrderItems, totalPrice);
                }
            }

            this.Update();
            return this.Data.SelectPayment == null;
        }

        public void UpdateDeliveryInterval(BaseShippingOption shipping)
        {
            this.Data.SelectShipping = shipping;

            BaseShippingOption option = null;
            var configuratorShippingCalculation =
                ShippingCalculationConfigurator.Configure()
                                               .ByMyCheckout(this)
                                               .WithCurrency(CurrencyService.CurrentCurrency);
            var shippingManager = new ShippingManager(configuratorShippingCalculation.Build()).PreferShippingOptionFromParameters();
            var shippingOptions = this.Data.TypeCalculationVariants.HasValue
                ? shippingManager.GetOptions(this.Data.TypeCalculationVariants.Value)
                : shippingManager.GetOptions();

            option = shippingOptions.FirstOrDefault(x => x.Id == shipping.Id);

            option?.UpdateDeliveryInterval(shipping);

            this.Data.SelectShipping = option ?? shippingOptions.FirstOrDefault();
            this.Update();
        }

        public static MyCheckout Factory(Guid customerId)
        {
            var data = OrderConfirmationService.Get(customerId);
            var cart = ShoppingCartService.GetShoppingCart(ShoppingCartType.ShoppingCart, false,
                                                            data?.SelectPayment?.Id, 
                                                            data?.SelectShipping?.MethodId);
            
            
            var model = new MyCheckout(cart) { Data = data };

            if (model.Data == null)
            {
                var customer = CustomerContext.CurrentCustomer;
                model.Data = new CheckoutData
                {
                    ShopCartHash = model.Cart.GetHashCode(),
                    User = { Id = customerId, CustomerType = customer.CustomerType }
                };
                if (BonusSystem.IsActive)
                {
                    var bonusCard = BonusSystemService.GetCard(customerId);
                    if (bonusCard != null)
                        model.Data.Bonus.AppliedBonuses = (float)Math.Truncate(bonusCard.BonusesTotalAmount).SimpleRoundPrice();
                }
                OrderConfirmationService.Add(customer.Id, model.Data);
            }
            
            return model;
        }

        public void Update()
        {
            OrderConfirmationService.Update(CustomerContext.CustomerId, Data);
        }

        public void ProcessUser()
        {
            var customer = CustomerContext.CurrentCustomer;

            if (customer.RegistredUser)
            {
                ProcessRegisteredUser(customer);
                return;
            }

            if (!string.IsNullOrEmpty(Data.User.Email))
            {
                var customerByEmail = CustomerService.GetCustomerByEmail(Data.User.Email);
                if (customerByEmail != null)
                    _linkedCustomerId = Data.User.Id = customerByEmail.Id;
            }

            if (!string.IsNullOrEmpty(Data.User.Phone) && _linkedCustomerId == null)
            {
                var customerByPhone =
                    CustomerService.GetCustomerByPhone(Data.User.Phone, StringHelper.ConvertToStandardPhone(Data.User.Phone));

                if (customerByPhone != null)
                    _linkedCustomerId = Data.User.Id = customerByPhone.Id;
            }

            if (_linkedCustomerId != null)
            {
                if (BonusSystem.IsActive)
                {
                    var bonusCard = BonusSystemService.GetCard(_linkedCustomerId);
                    if (bonusCard != null)
                        Data.User.BonusCardId = bonusCard.CardId;
                }
                return;
            }

            ProcessUnRegisteredUser();
        }

        private void ProcessRegisteredUser(Customer customer)
        {
            try
            {
                var user = Data.User;
                
                if (string.IsNullOrEmpty(user.Email))
                    user.Email = customer.EMail;

                var needUpdateCustomer = false;

                if (!string.IsNullOrWhiteSpace(user.FirstName) && customer.FirstName != user.FirstName)
                {
                    customer.FirstName = user.FirstName;
                    needUpdateCustomer = true;
                }

                if (!string.IsNullOrWhiteSpace(user.LastName) && customer.LastName != user.LastName)
                {
                    customer.LastName = user.LastName;
                    needUpdateCustomer = true;
                }

                if (!string.IsNullOrWhiteSpace(user.Patronymic) && customer.Patronymic != user.Patronymic)
                {
                    customer.Patronymic = user.Patronymic;
                    needUpdateCustomer = true;
                }

                if (!string.IsNullOrWhiteSpace(user.Phone) && customer.Phone != user.Phone)
                {
                    var standardPhone = !string.IsNullOrEmpty(user.Phone)
                        ? StringHelper.ConvertToStandardPhone(user.Phone)
                        : null;
                    
                    if (!CustomerService.IsPhoneExist(user.Phone, standardPhone))
                    {
                        customer.Phone = user.Phone;
                        customer.StandardPhone = standardPhone;

                        needUpdateCustomer = true;
                    }
                }

                if (user.IsAgreeForPromotionalNewsletter != null && user.IsAgreeForPromotionalNewsletter.Value &&
                    !customer.IsAgreeForPromotionalNewsletter)
                {
                    customer.IsAgreeForPromotionalNewsletter = true;
                    needUpdateCustomer = true;
                }

                if (SettingsCheckout.IsShowBirthDay && user.BirthDay != null && user.BirthDay != customer.BirthDay)
                {
                    customer.BirthDay = user.BirthDay;
                    needUpdateCustomer = true;
                }

                if (customer.BonusCardNumber == null && user.BonusCardId != null)
                {
                    var card = BonusSystemService.GetCard(user.BonusCardId);
                    if (card != null && !card.Blocked)
                    {
                        customer.BonusCardNumber = card.CardNumber;
                        needUpdateCustomer = true;
                    }
                }
                
                if (needUpdateCustomer)
                    CustomerService.UpdateCustomer(customer);

                if (customer.Contacts.Count == 0)
                {
                    var country = !string.IsNullOrWhiteSpace(Data.Contact.Country)
                        ? CountryService.GetCountryByName(Data.Contact.Country)
                        : null;

                    CustomerService.AddContact(new CustomerContact()
                    {
                        Name = StringHelper.AggregateStrings(" ", user.LastName, user.FirstName, user.Patronymic),
                        Country = Data.Contact.Country,
                        CountryId = country?.CountryId ?? 0,
                        Region = Data.Contact.Region,
                        City = Data.Contact.City,
                        District = Data.Contact.District,
                        Zip = Data.Contact.Zip,

                        Street = Data.Contact.Street,
                        House = Data.Contact.House,
                        Apartment = Data.Contact.Apartment,
                        Structure = Data.Contact.Structure,
                        Entrance = Data.Contact.Entrance,
                        Floor = Data.Contact.Floor
                    }, customer.Id);
                }

                if (user.CustomerFields != null)
                {
                    foreach (var customerField in user.CustomerFields)
                    {
                        CustomerFieldService.AddUpdateMap(customer.Id, customerField.Id, customerField.Value ?? "", true);
                    }
                }

                // если есть купон и пользователь зарегистрирован недавно, то проверяем партнерскую программу
                if (Cart.Coupon != null 
                    && customer.RegistrationDateTime >= DateTime.Now.AddDays(-7)
                    && OrderService.GetOrdersCountByCustomer(customer.Id) == 0)
                {
                    PartnerService.BindNewCustomer(customer);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private void ProcessUnRegisteredUser()
        {
            try
            {
                if (!Data.User.WantRegist)
                    Data.User.Password = StringHelper.GeneratePassword(8);

                var customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                {
                    Id = CustomerContext.CustomerId,
                    Password = Data.User.Password,
                    FirstName = Data.User.FirstName,
                    LastName = Data.User.LastName,
                    Patronymic = Data.User.Patronymic,
                    Phone = Data.User.Phone,
                    StandardPhone = StringHelper.ConvertToStandardPhone(Data.User.Phone),
                    EMail = Data.User.Email,
                    CustomerRole = Role.User,
                    BirthDay = SettingsCheckout.IsShowBirthDay ? Data.User.BirthDay : null,
                    CustomerType = Data.User.CustomerType,
                    IsAgreeForPromotionalNewsletter = Data.User.IsAgreeForPromotionalNewsletter ??
                                                      SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked
                };

                CustomerService.InsertNewCustomer(customer, Data.User.CustomerFields);
                if (customer.Id == Guid.Empty)
                    return;

                if (Data.User.WantRegist && BonusSystem.IsActive && 
                    (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.BonusSystem))
                {
                    CreateBonusCard(customer);
                }

                Data.User.Id = customer.Id;

                AuthorizeService.SignIn(customer.EMail, customer.Password, false, true);

                var country = !string.IsNullOrWhiteSpace(Data.Contact.Country)
                    ? CountryService.GetCountryByName(Data.Contact.Country)
                    : null;

                var contact = new CustomerContact()
                {
                    Name = customer.GetFullName(),
                    Country = Data.Contact.Country,
                    CountryId = country?.CountryId ?? 0,
                    Region = Data.Contact.Region,
                    District = Data.Contact.District,
                    City = Data.Contact.City,
                    Zip = Data.Contact.Zip,

                    Street = Data.Contact.Street,
                    House = Data.Contact.House,
                    Apartment = Data.Contact.Apartment,
                    Structure = Data.Contact.Structure,
                    Entrance = Data.Contact.Entrance,
                    Floor = Data.Contact.Floor
                };

                CustomerService.AddContact(contact, customer.Id);
                
                if (Data.User.WantRegist && !string.IsNullOrEmpty(customer.EMail))
                {
                    var mail = new RegistrationMailTemplate(customer);

                    MailService.SendMailNow(CustomerContext.CustomerId, customer.EMail, mail);
                    MailService.SendMailNow(SettingsMail.EmailForRegReport, mail, replyTo: customer.EMail);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        public Order ProcessOrder(string customData, OrderType? orderType, bool isLanding)
        {
            ProcessUser();

            var order = CreateOrder(Cart, customData, orderType, isLanding);

            var certificate = Cart.Certificate;
            if (certificate != null)
            {
                certificate.ApplyOrderNumber = order.Number;
                certificate.Used = true;
                certificate.Enable = true;

                GiftCertificateService.DeleteCustomerCertificate(certificate.CertificateId);
                GiftCertificateService.UpdateCertificateById(certificate);
            }

            var coupon = Cart.Coupon;
            if (coupon != null && Cart.TotalPrice >= coupon.MinimalOrderPrice)
            {
                CouponService.IncrementActualUses(coupon.CouponID);
                CouponService.DeleteCustomerCoupon(coupon.CouponID);
            }

            ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart, Data.User.Id);
            ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart, CustomerContext.CustomerId);

            OrderConfirmationService.Delete(CustomerContext.CustomerId);

            var geoModePointId = CommonHelper.GetCookieString(GeoModeConfig.PointCookieName);
            if (geoModePointId.IsNotEmpty())
            {
                CommonHelper.SetCookie(GeoModeConfig.PreviousShippingIdCookieName, geoModePointId, true);
                if (order.OrderPickPoint != null)
                    CommonHelper.SetCookie(GeoModeConfig.PreviousShippingPointIdCookieName, order.OrderPickPoint.PickPointId, true);
                
                CommonHelper.DeleteCookie(GeoModeConfig.PointCookieName);
            }

            return order;
        }

        private Order CreateOrder(ShoppingCart cart, string customData, OrderType? orderType, bool isLanding)
        {
            var currency = CurrencyService.CurrentCurrency;
            var orderSource = OrderSourceService.GetOrderSource(OrderType.ShoppingCart);

            if (orderType != null && orderType != OrderType.None)
                orderSource = OrderSourceService.GetOrderSource(orderType.Value);
            else if (Data.LpId != null)
            {
                var lp = new LpService().Get(Data.LpId.Value);
                LpSite site;
                if (lp != null && (site = new LpSiteService().Get(lp.LandingSiteId)) != null)
                    orderSource = OrderSourceService.GetOrderSource(OrderType.LandingPage, site.Id, site.Name);
                else
                    orderSource = OrderSourceService.GetOrderSource(OrderType.LandingPage);
            }
            else if (SettingsDesign.IsSocialTemplate)
                orderSource = OrderSourceService.GetOrderSource(OrderType.SocialNetworks);
            else if (SettingsDesign.IsMobileTemplate)
                orderSource = OrderSourceService.GetOrderSource(OrderType.Mobile);

            var customer = CustomerContext.CurrentCustomer;

            var order = new Order
            {
                OrderCustomer = new OrderCustomer
                {
                    CustomerIP = HttpContext.Current.Request.UserHostAddress,
                    CustomerID = Data.User.Id,
                    FirstName = Data.User.FirstName,
                    LastName = Data.User.LastName,
                    Patronymic = Data.User.Patronymic,
                    Organization = Data.User.Organization,
                    Email = Data.User.Email,
                    Phone = Data.User.Phone,
                    StandardPhone =
                        !string.IsNullOrWhiteSpace(Data.User.Phone)
                            ? StringHelper.ConvertToStandardPhone(Data.User.Phone)
                            : null,
                    CustomerType = Data.User.CustomerType,

                    Country = Data.Contact.Country,
                    Region = Data.Contact.Region,
                    District = Data.Contact.District,
                    City = Data.Contact.City,
                    Zip = Data.Contact.Zip,
                    CustomField1 = Data.Contact.CustomField1,
                    CustomField2 = Data.Contact.CustomField2,
                    CustomField3 = Data.Contact.CustomField3,

                    Street = Data.Contact.Street,
                    House = Data.Contact.House,
                    Apartment = Data.Contact.Apartment,
                    Structure = Data.Contact.Structure,
                    Entrance = Data.Contact.Entrance,
                    Floor = Data.Contact.Floor
                },
                OrderCurrency = currency,
                OrderStatusId = OrderStatusService.DefaultOrderStatus,
                AffiliateID = 0,
                OrderDate = DateTime.Now,
                CustomerComment = Data.CustomerComment,
                ManagerId = customer.ManagerId,

                GroupName = customer.CustomerGroup.GroupName,
                GroupDiscount = customer.CustomerGroup.GroupDiscount,
                OrderDiscount = cart.DiscountPercentOnTotalPrice,
                OrderSourceId = orderSource.Id,
                CustomData = customData,
                LpId = Data.LpId,
                LinkedCustomerId = _linkedCustomerId,
                DontCallBack = Data.DontCallBack,
                ReceivingMethod = Data.ReceivingMethod,
                WarehouseIds = WarehouseContext.GetAvailableWarehouseIds(),
                CountDevices = Data.CountDevices
            };

            foreach (var orderItem in cart.Select(item => (OrderItem)item))
            {
                order.OrderItems.Add(orderItem);
            }

            order.ShippingMethodId = Data.SelectShipping.MethodId;
            order.PaymentMethodId = Data.SelectPayment.Id;

            order.ArchivedShippingName = Data.SelectShipping.Name;
            order.ArchivedPaymentName = Data.SelectPayment.Name;

            order.OrderPickPoint = Data.SelectShipping.GetOrderPickPoint();

            order.PaymentDetails = Data.SelectPayment.GetDetails(order);

            order.AvailablePaymentCashOnDelivery = Data.SelectShipping.IsAvailablePaymentCashOnDelivery;
            order.AvailablePaymentPickPoint = Data.SelectShipping.IsAvailablePaymentPickPoint;

            if (Data.SelectShipping != null)
                if (Data.SelectShipping.DateOfDelivery != null)
                    order.DeliveryDate = Data.SelectShipping.DateOfDelivery;
                if (Data.SelectShipping.TimeOfDelivery == null && Data.SelectShipping.ShowSoonest)
                    order.DeliveryTime = "Как можно скорее";
                else if (Data.SelectShipping.TimeOfDelivery.IsNotEmpty())
                    order.DeliveryTime = string.Concat(
                        Data.SelectShipping.TimeOfDelivery,
                        Data.SelectShipping.TimeZoneOffset.HasValue 
                            ? $"|{Data.SelectShipping.TimeZoneOffset.Value}" 
                            : string.Empty);

            ProcessCertificate(order);
            ProcessCoupon(order);

            var shippingPrice = Data.SelectShipping.FinalRate;
            var paymentPrice = Data.SelectPayment.Rate;

            Card bonusCard = null;
            if (BonusSystem.IsActive)
            {
                bonusCard = BonusSystemService.GetCard(Data.User.BonusCardId);

                if (Data.Bonus.UseIt && bonusCard != null && bonusCard.BonusesTotalAmount > 0)
                {
                    order.BonusCost = BonusSystemService.GetBonusCost(bonusCard, cart, shippingPrice, Data.Bonus.AppliedBonuses).BonusPrice;
                }

                if (Data.User.WantBonusCard && bonusCard == null && customer.RegistredUser)
                {
                    CreateBonusCard(customer);
                    bonusCard = BonusSystemService.GetCard(customer.Id);
                }
            }

            order.BonusCardNumber = bonusCard != null && !bonusCard.Blocked ? bonusCard.CardNumber : default(long?);

            order.ShippingCost = shippingPrice;
            var shippingTax = Data.SelectShipping.TaxId.HasValue ? TaxService.GetTax(Data.SelectShipping.TaxId.Value) : null;
            order.ShippingTaxType = shippingTax == null ? TaxType.None : shippingTax.TaxType;
            order.ShippingPaymentMethodType = Data.SelectShipping.PaymentMethodType;
            order.ShippingPaymentSubjectType = Data.SelectShipping.PaymentSubjectType;
            order.PaymentCost = paymentPrice;
            if (SettingsCheckout.IsShowOrderRecipient && Data.User.IsAddRecipient)
                order.OrderRecipient = new OrderRecipient
                {
                    FirstName = Data.User.RecipientFirstName,
                    LastName = Data.User.RecipientLastName,
                    Patronymic = Data.User.RecipientPatronymic,
                    Phone = Data.User.RecipientPhone,
                    StandardPhone = StringHelper.ConvertToStandardPhone(Data.User.RecipientPhone)
                };

            order.OrderID = OrderService.AddOrder(order, new OrderChangedBy(customer));
            
            var customerIdHashCode = customer.Id.GetHashCode();
            var checkoutAttachments = AttachmentService.GetAttachments<CheckoutAttachment>(customerIdHashCode);
            if (checkoutAttachments != null)
            {
                foreach (var checkoutAttachment in checkoutAttachments)
                {
                    var customerOrderAttachment = new CustomerOrderAttachment
                    {
                        ObjId = order.OrderID,
                        FileName = checkoutAttachment.FileName,
                        OriginFileName = checkoutAttachment.OriginFileName,
                        FileSize = checkoutAttachment.FileSize,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                    };
                    customerOrderAttachment.Id = AttachmentService.AddAttachment(customerOrderAttachment);
                    if (customerOrderAttachment.Id != 0)
                    {
                        var filePath = checkoutAttachment.PathAbsolut;
                        if (File.Exists(filePath))
                           File.Move(filePath, customerOrderAttachment.PathAbsolut);
                    }
                }
                AttachmentService.DeleteAttachments<CheckoutAttachment>(customerIdHashCode);
            }

            OrderStatusService.ChangeOrderStatusForNewOrder(order.OrderID);

            if (BonusSystem.IsActive && bonusCard != null && !bonusCard.Blocked)
            {
                BonusSystemService.MakeBonusPurchase(bonusCard.CardNumber, cart, shippingPrice, order);
            }

            PostProcessOrder(order);


            var lpUrl = new GetCrossSellLandingUrl(Data.LpUpId, order, isLanding).Execute();

            if (string.IsNullOrEmpty(lpUrl))
            {
                OrderMailService.SendMail(order, cart.TotalDiscount, Data.Bonus.BonusPlus, Data.SelectShipping.ForMailTemplate());
            }
            else
            {
                LandingHelper.LandingRedirectUrl = lpUrl;
                DeferredMailService.Add(new DeferredMail(order.OrderID, DeferredMailType.Order));
            }

            TrialService.TrackEvent(
                order.OrderItems.Any(x => x.Name.Contains("SM-G900F"))
                    ? TrialEvents.BuyTheProduct
                    : TrialEvents.CheckoutOrder, string.Empty);

            return order;
        }

        private void ProcessCertificate(Order order)
        {
            var certificate = Cart.Certificate;

            if (certificate != null)
            {
                order.Certificate = new OrderCertificate()
                {
                    Code = certificate.CertificateCode,
                    Price = certificate.Sum
                };
            }
        }

        private void ProcessCoupon(Order order)
        {
            var coupon = Cart.Coupon;
            
            order.Coupon = coupon != null && Cart.TotalPrice >= coupon.MinimalOrderPrice 
                ? (OrderCoupon) coupon 
                : null;
        }

        private void CreateBonusCard(Customer customer)
        {
            try
            {
                customer.BonusCardNumber = BonusSystemService.AddCard(new Card { CardId = customer.Id });
                CustomerService.UpdateCustomer(customer);

                if (customer.BonusCardNumber != null)
                {
                    Data.User.BonusCardId = customer.Id;

                    if (HttpContext.Current != null)
                        HttpContext.Current.Session["BonusesForNewCard"] = BonusSystem.BonusesForNewCard;
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private void PostProcessOrder(Order order)
        {
            if (order.Sum == 0)
                OrderService.PayOrder(order.OrderID, true);
        }

        private float GetBonusDiscount(CheckoutData data, ShoppingCart cart)
        {
            if (data.Bonus != null && data.Bonus.UseIt)
            {
                var bonusCost = BonusSystemService.GetBonusCost(cart, 0, data.Bonus.AppliedBonuses);
                return bonusCost.BonusPrice;
            }

            return 0.0f;
        }

        public void SetCart(ShoppingCart cart)
        {
            Cart = cart;
        }
    }
    
    public static class MyChekoutExtensions
    {
        public static void SetPriceRule(this MyCheckout model)
        {
            if ((SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.PriceTypes)
                || (model.Data.SelectPayment == null 
                    && model.Data.SelectShipping is null))
                return;

            foreach (var item in model.Cart)
            {
                item.Offer.SetPriceRule(
                    item.Amount,
                    item.CustomerGroup.CustomerGroupId, 
                    model.Data.SelectPayment?.Id,
                    model.Data.SelectShipping?.MethodId);
            }
        }

        public static void SetPriceRule(this MyCheckout model, List<BasePaymentOption> payments)
        {
            if ((SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.PriceTypes) ||
                model.Data.SelectPayment == null)
                return;

            var offersCache = new Dictionary<int, Offer>();
            foreach(var payment in payments)
            {
                var discount = 0f;

                foreach (var item in model.Cart)
                {
                    var priceRule = PriceRuleService.GetPriceRule(item.OfferId, item.Amount, item.CustomerGroup.CustomerGroupId, payment.Id, model.Data.SelectShipping?.MethodId);

                    if (priceRule?.PriceByRule != null 
                        && priceRule.PaymentMethodId == payment.Id)
                    {
                        if (!offersCache.ContainsKey(item.OfferId))
                            offersCache.Add(
                                item.OfferId,
                                OfferService.GetOffer(item.OfferId)
                                             // применяем цену без учета метода оплаты, чтобы discount посчитать с уже примененной цены
                                            .SetOfferPriceRule(item.Amount, item.CustomerGroup.CustomerGroupId, null, model.Data.SelectShipping?.MethodId));
                        var offer = offersCache[item.OfferId];
                        if (offer != null && offer.BasePrice > priceRule.PriceByRule.Value)
                            discount += (offer.RoundedPrice - priceRule.PriceByRule.Value.RoundPrice(item.Offer.Product.Currency.Rate)) * item.Amount;
                    }
                }

                if (discount > 0)
                    payment.NamePostfix = LocalizationService.GetResourceFormat("Core.OrderConfirmation.DiscountByPaymentRule", discount.FormatPrice());
            }
        }

        public static void SetPriceRule(this MyCheckout model, List<BaseShippingOption> shippingOptions)
        {
            if ((SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.PriceTypes) ||
                model.Data.SelectShipping == null)
                return;

            var offersCache = new Dictionary<int, Offer>();
            foreach(var shipping in shippingOptions)
            {
                var discount = 0f;

                foreach (var item in model.Cart)
                {
                    var priceRule = PriceRuleService.GetPriceRule(item.OfferId, item.Amount, item.CustomerGroup.CustomerGroupId, model.Data.SelectPayment?.Id, shipping.MethodId);

                    if (priceRule?.PriceByRule != null
                        && priceRule.ShippingMethodId == shipping.MethodId)
                    {
                        if (!offersCache.ContainsKey(item.OfferId))
                            offersCache.Add(
                                item.OfferId,
                                OfferService.GetOffer(item.OfferId)
                                             // применяем цену без учета метода оплаты, чтобы discount посчитать с уже примененной цены
                                            .SetOfferPriceRule(item.Amount, item.CustomerGroup.CustomerGroupId, model.Data.SelectPayment?.Id, null));
                        var offer = offersCache[item.OfferId];
                        if (offer != null && offer.BasePrice > priceRule.PriceByRule.Value)
                            discount += (offer.RoundedPrice - priceRule.PriceByRule.Value.RoundPrice(item.Offer.Product.Currency.Rate)) * item.Amount;
                    }
                }

                if (discount > 0)
                    shipping.Hint = LocalizationService.GetResourceFormat("Core.OrderConfirmation.DiscountByShippingRule", discount.FormatPrice());
            }
        }
    }
}