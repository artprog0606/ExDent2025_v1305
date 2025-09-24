﻿using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.MyAccount;
using AdvantShop.Customers;
using AdvantShop.Handlers.MyAccount;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Models.MyAccount;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Security;
using AdvantShop.ViewModel.MyAccount;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Diagnostics;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Catalog;
using AdvantShop.Handlers.Common;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;

namespace AdvantShop.Controllers
{
    public partial class MyAccountController : BaseClientController
    {
        public ActionResult Index()
        {
            //if (SettingsDesign.IsMobileTemplate)
            //    return RedirectToRoute("Home");

            var customer = CustomerContext.CurrentCustomer;
            if (!customer.RegistredUser)
                return RedirectToAction("Login", "User");

            var model = new GetMyAccount(customer, TempData["IsRegisteredNow"]).Execute();

            SetMetaInformation(T("MyAccount.Index.MyAccountTitle"));
            SetNgController(NgControllers.NgControllersTypes.MyAccountCtrl);

            ModulesExecuter.ViewMyAccount(customer);

            return View(model);
        }

        public ActionResult LpMyAccount()
        {
            var customer = CustomerContext.CurrentCustomer;
            if (!customer.RegistredUser)
                return new EmptyResult();

            var model = new GetMyAccount(customer, TempData["IsRegisteredNow"]).Execute();
            model.IsLanding = true;

            return PartialView("~/Views/MyAccount/Index.cshtml", model);
        }

        #region Order product history

        public JsonResult GetCustomerOrderProductHistory()
        {
            if (!CustomerContext.CurrentCustomer.RegistredUser)
                return Json(null);

            var orderedProducts = OrderService.GetAllOrderedProducts(CustomerContext.CurrentCustomer.Id)
                .Select(
                    orderedProduct => 
                        new
                        {
                            Name = orderedProduct.Name,
                            ArtNo = orderedProduct.ArtNo,
                            Price = PriceFormatService.FormatPrice(orderedProduct.Price),
                            LastOrderDate = orderedProduct.LastOrderDate,
                            LastOrderTime = orderedProduct.LastOrderTime,
                            LastOrderNumber = orderedProduct.LastOrderNumber,
                            Url = !string.IsNullOrEmpty(orderedProduct.Url) 
                                ? UrlService.GetLink(ParamType.Product, orderedProduct.Url)
                                : null,
                            PhotoSmallUrl = !string.IsNullOrEmpty(orderedProduct.PhotoName) 
                                ? FoldersHelper.GetImageProductPath(ProductImageType.Small, orderedProduct.PhotoName , false) 
                                : null,
                        })
                .ToList();
            
            return Json(new
            {
                OrderedProducts = orderedProducts,
            });
        }

        #endregion

        #region Customer contacts

        public JsonResult GetCustomerContacts()
        {
            if (!CustomerContext.CurrentCustomer.RegistredUser)
                return Json("");

            var customerContacts =
                from item in CustomerContext.CurrentCustomer.Contacts
                select new
                {
                    CustomerContext.CurrentCustomer.CustomerCompanyName,
                    item.ContactId,
                    item.Country,
                    item.City,
                    item.District,
                    item.Name,
                    CustomerContext.CurrentCustomer.FirstName,
                    CustomerContext.CurrentCustomer.LastName,
                    CustomerContext.CurrentCustomer.Patronymic,
                    item.CountryId,
                    item.RegionId,
                    item.Region,
                    item.Zip,

                    item.Street,
                    item.House,
                    item.Apartment,
                    item.Structure,
                    item.Entrance,
                    item.Floor,
                    item.IsMain,

                    IsShowFullAddress = SettingsCheckout.IsShowFullAddress,
                    AggregatedAddress = new[]
                    {
                        SettingsCustomers.IsRegistrationAsLegalEntity
                            ? CustomerContext.CurrentCustomer.CustomerCompanyName
                            : item.Name,
                        item.Zip, item.Country, item.Region, item.City, item.District,
                        !string.IsNullOrEmpty(item.Street)
                            ? LocalizationService.GetResource("Core.Orders.OrderContact.Street") + " " + item.Street
                            : "",
                        !string.IsNullOrEmpty(item.House)
                            ? LocalizationService.GetResource("Admin.Js.CustomerView.House") + item.House
                            : "",
                        !string.IsNullOrEmpty(item.Structure)
                            ? LocalizationService.GetResource("Admin.Js.CustomerView.Struct") + item.Structure
                            : "",
                        !string.IsNullOrEmpty(item.Entrance)
                            ? LocalizationService.GetResource("Core.Orders.OrderContact.Entrance") + " " + item.Entrance
                            : "",
                        !string.IsNullOrEmpty(item.Floor)
                            ? LocalizationService.GetResource("Admin.Customers.Customer.Floor").ToLower() + " " +
                              item.Floor
                            : "",
                        !string.IsNullOrEmpty(item.Apartment)
                            ? LocalizationService.GetResource("Admin.Js.CustomerView.Ap") + item.Apartment
                            : ""
                    }.Where(str => str.IsNotEmpty()).AggregateString(", ")

                };

            return Json(customerContacts.ToList());
        }

        public JsonResult GetFieldsForCustomerContacts(bool isShowName = false)
        {
            var suggestionsModule = AttachedModules.GetModules<ISuggestions>().Select(x => (ISuggestions)Activator.CreateInstance(x)).FirstOrDefault();
            return Json(new
            {
                SettingsCheckout.IsShowCountry,
                SettingsCheckout.IsRequiredCountry,
                SettingsCheckout.IsShowState,
                SettingsCheckout.IsRequiredState,
                SettingsCheckout.IsShowCity,
                SettingsCheckout.IsRequiredCity,
                SettingsCheckout.IsShowDistrict,
                SettingsCheckout.IsRequiredDistrict,
                SettingsCheckout.IsShowAddress,
                SettingsCheckout.IsRequiredAddress,
                SettingsCheckout.IsShowZip,
                SettingsCheckout.IsRequiredZip,
                SettingsCheckout.IsShowFullAddress,
                UseAddressSuggestions = suggestionsModule != null && suggestionsModule.SuggestAddressInClient,
                SuggestAddressUrl = suggestionsModule != null ? suggestionsModule.SuggestAddressUrl : null,
                IsShowFirstName = isShowName,
                SettingsCheckout.CustomerFirstNameField,
                IsShowLastName = isShowName && SettingsCheckout.IsShowLastName,
                SettingsCheckout.IsRequiredLastName,
                IsShowPatronymic = isShowName && SettingsCheckout.IsShowPatronymic,
                SettingsCheckout.IsRequiredPatronymic
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ProcessAddress(SuggestAddressQueryModel address)
        {
            if (address == null || address.City.IsNullOrEmpty())
                return JsonError();

            IpZone zone;
            if ((address.ByCity || address.Region.IsNullOrEmpty()) && (zone = IpZoneService.GetZoneByCity(address.City, null)) != null)
            {
                address.Region = zone.Region;
                address.Country = zone.CountryName;
                address.District = zone.District;
                address.Zip = zone.Zip;
            }
            ModulesExecuter.ProcessAddress(address);

            return JsonOk(address);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddUpdateCustomerContact(CustomerAccountModel account)
        {
            var contact = new AddUpdateContactHandler().Execute(account);
            return Json(contact);
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetMainContact(Guid contactId)
        {
            if (!CustomerContext.CurrentCustomer.RegistredUser)
            {
                return JsonError("Unregistered user");
            }
            CustomerService.SetMainContact(true, CustomerContext.CurrentCustomer.Id, contactId);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCustomerContact(string contactId)
        {
            if (!CustomerContext.CurrentCustomer.RegistredUser)
                return Json(false);

            var id = contactId.TryParseGuid();
            if (id != Guid.Empty && CustomerContext.CurrentCustomer.Contacts.Any(contact => contact.ContactId == id))
            {
                CustomerService.DeleteContact(id);
            }
            return Json(true);
        }

        #endregion

        #region Order history

        public JsonResult GetOrderDetails(string ordernumber)
        {
            if (!CustomerContext.CurrentCustomer.RegistredUser)
                return Json(null);

            var order = OrderService.GetOrderByNumber(ordernumber);

            if (order.OrderCustomer == null || order.OrderCustomer.CustomerID != CustomerContext.CurrentCustomer.Id)
                return Json(null);

            return Json(new GetOrderDetailsHandler(order).Get());
        }

        public JsonResult GetCustomerOrderHistory()
        {
            if (!CustomerContext.CurrentCustomer.RegistredUser)
                return Json(null);

            var orders = OrderService.GetCustomerOrderHistory(CustomerContext.CurrentCustomer.Id);

            var customerOrders = from item in orders
                                 select new
                                 {
                                     item.ArchivedPaymentName,
                                     Status = OrderStatusService.GetOrderStatus(item.StatusID).Hidden ? item.PreviousStatus : item.Status,
                                     item.ShippingMethodName,
                                     OrderDate = item.OrderDate.ToString(SettingsMain.ShortDateFormat),
                                     OrderTime = item.OrderDate.ToString("HH:mm"),
                                     Sum = PriceFormatService.FormatPrice(item.Sum, item.CurrencyValue, item.CurrencySymbol, item.CurrencyCode, item.IsCodeBefore, null),
                                     item.OrderNumber,
                                     item.Payed,
                                     item.TrackNumber
                                 };
            var totalPrice = orders.Where(item => item.Payed).Sum(item => item.Sum * item.CurrencyValue);
            totalPrice = totalPrice / CurrencyService.CurrentCurrency.Rate;

            return Json(new
            {
                Orders = customerOrders,
                TotalSum = PriceFormatService.FormatPrice(totalPrice, CurrencyService.CurrentCurrency)
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CancelOrder(string ordernumber)
        {
            return ProcessJsonResult(new CancelOrder(ordernumber));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangePaymentMethod(string paymentId, string orderNumber)
        {
            if (!CustomerContext.CurrentCustomer.RegistredUser && paymentId.IsNullOrEmpty() && orderNumber.IsNullOrEmpty())
                return Json(null);

            var order = OrderService.GetOrderByNumber(orderNumber);
            if (order == null)
                return Json(null);

            var paymentIdInt = paymentId.TryParseInt();
            var manager = new PaymentManager(
                config => config
                         .ByOrder(order, actualizeShippingAndPayment: true)
                         .WithPaymentOption(new BasePaymentOption() { Id = paymentIdInt })
                         .Build());

            var payment = manager.PreferPaymentOptionFromParameters().GetOptions().FirstOrDefault();

            if (payment == null)
                return Json(null);

            order.PaymentMethodId = payment.Id;
            order.ArchivedPaymentName = payment.Name;
            order.PaymentCost = payment.Rate;
            order.PaymentDetails = payment.GetDetails(order);

            OrderService.UpdatePaymentDetails(order.OrderID, order.PaymentDetails);
            OrderService.UpdateOrderMain(order);
            OrderService.RefreshTotal(order);

            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeOrderComment(string orderNumber, string customerComment)
        {
            if (orderNumber.IsNullOrEmpty())
                return Json(null);

            var order = OrderService.GetOrderByNumber(orderNumber);
            if (order == null)
                return Json(null);

            customerComment = HttpUtility.HtmlEncode(customerComment);

            if (order.CustomerComment == customerComment) 
                return Json(true);
            
            try
            {
                var changeUserCommentTemplate = 
                    new ChangeUserCommentTemplate(order.OrderID.ToString(), customerComment, order.Number, 
                        order.Manager != null ? order.Manager.FullName : string.Empty);
                
                MailService.SendMailNow(order.OrderCustomer?.CustomerID ?? Guid.Empty, SettingsMail.EmailForOrders, changeUserCommentTemplate);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message, ex);
            }

            OrderService.UpdateCustomerComment(order.OrderID, customerComment);

            return Json(true);
        }

        [HttpGet]
        public JsonResult GetOrderReview(string orderNumber)
        {
            if (orderNumber.IsNotEmpty())
            {
                var orderId = OrderService.GetOrderIdByNumber(orderNumber);
                var orderReview = OrderService.GetOrderReview(orderId);
                return Json(orderReview ?? new OrderReview { OrderId = orderId });
            }
            return Json(null);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddOrderReview(string orderNumber, float ratio, string text)
        {
            return ProcessJsonResult(() =>
                new AddOrderReviewHandler(orderNumber, ratio, text, CustomerContext.CustomerId).Execute());
        }

        [HttpGet]
        public ActionResult GetOrderFile(int attachmentId)
        {
            var attachment = AttachmentService.GetAttachment<CustomerOrderAttachment>(attachmentId);
            if (attachment == null)
                return Error404();

            var customer = CustomerContext.CurrentCustomer;
            var orderCustomer = OrderService.GetOrderCustomer(attachment.ObjId);
            if (orderCustomer == null || customer.Id != orderCustomer.CustomerID)
                return Error404();

            if (!new FileExtensionContentTypeHelper().TryGetContentType(attachment.Path, out var contentType))
                contentType = "text/plain";

            if (System.IO.File.Exists(attachment.PathAbsolut))
                return File(attachment.PathAbsolut, contentType, attachment.FileName);
            return Error404();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadAttachments(int orderId)
        {
            if (!SettingsCheckout.AllowUploadFiles)
                return JsonError(T("Checkout.Attachments.ProhibitUploadFiles"));
            try
            {
                var oldAttachments = AttachmentService.GetAttachments<CustomerOrderAttachment>(orderId);
                var results = new UploadAttachmentsWithResizePhotoHandler<CustomerOrderAttachment>(orderId, SettingsCheckout.CheckoutImageWidth, SettingsCheckout.CheckoutImageHeight).Execute();
                var newAttachments = AttachmentService.GetAttachments<CustomerOrderAttachment>(orderId);
                OrderHistoryService.ChangingCustomerOrderFiles(
                    orderId,
                    oldAttachments,
                    newAttachments,
                    new OrderChangedBy(CustomerContext.CurrentCustomer));

                return JsonOk(results);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return JsonError(ex.Message);
            }
        }

        #endregion

        public JsonResult CheckEmailBusy(string email)
        {
            return Json(ValidationHelper.IsValidEmail(email) && !CustomerService.ExistsEmail(email));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateCustomerEmail(string email)
        {
            if (!ValidationHelper.IsValidEmail(email)
                || CustomerService.ExistsEmail(email)
                || (!string.IsNullOrWhiteSpace(CustomerContext.CurrentCustomer.EMail)
                    && !CustomerContext.CurrentCustomer.EMail.Contains("@temp")))
            {
                return Json(false);
            }

            CustomerService.UpdateCustomerEmail(CustomerContext.CustomerId, email);
            AuthorizeService.SignIn(email, CustomerContext.CurrentCustomer.Password, true, true);

            return Json(true);
        }

        [ChildActionOnly]
        public ActionResult CommonInfo()
        {
            var model = new CommonInfoHandler().Get();
            return PartialView(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveUserInfo(UserInfoModel userInfo)
        {
            return ProcessJsonResult(new SaveUserInfo(userInfo));
        }

        [ChildActionOnly]
        public ActionResult AddressBook()
        {
            var selectedCountryId = IpZoneContext.CurrentZone.CountryId;

            var model = new AddressBookViewModel()
            {
                Countries = CountryService.GetAllCountries()
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.CountryId.ToString(), Selected = x.CountryId == selectedCountryId })
                    .ToList(),
            };

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ChangePassword()
        {
            return PartialView(new ChangePasswordViewModel());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            return ProcessJsonResult(() =>
            {
                if (!ModelState.IsValid)
                    throw new BlException(T("User.Registration.Error"));

                if (model.NewPassword.Length < 6)
                    throw new BlException(T("User.Registration.PasswordLenght"));

                if (string.IsNullOrWhiteSpace(model.NewPassword) ||
                    string.IsNullOrWhiteSpace(model.NewPasswordConfirm) ||
                    model.NewPassword != model.NewPasswordConfirm)
                {
                    throw new BlException(T("User.Registration.ErrorPasswordNotMatch"));
                }

                CustomerService.ChangePassword(CustomerContext.CurrentCustomer.Id, model.NewPassword, false);
                AuthorizeService.SignIn(CustomerContext.CurrentCustomer.EMail, model.NewPassword, false, true);
            });
        }

        [ChildActionOnly]
        public ActionResult BonusCard()
        {
            var model = new MyAccountBonusSystemHandler(CustomerContext.CurrentCustomer).Get();
            return PartialView("BonusCard", model);
        }
    }
}