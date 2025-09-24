using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Letters;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Customers;
using AdvantShop.Web.Admin.Handlers.Customers.Export;
using AdvantShop.Web.Admin.Handlers.Shared;
using AdvantShop.Web.Admin.Models.Customers;
using AdvantShop.Web.Admin.Models.Customers.Export;
using AdvantShop.Web.Admin.ViewModels.Common;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Customers
{
    public partial class CustomersCrmController : CustomersController { }

    [Auth(RoleAction.Customers)]
    [AccessBySettings(ETypeRedirect.AdminPanel, EProviderSetting.CrmActive, EProviderSetting.StoreActive)]
    public partial class CustomersController : BaseAdminController
    {
        #region List

        public ActionResult Index(CustomersFilterModel filter)
        {
            if (!string.IsNullOrEmpty(filter.Search))
            {
                var customer = CustomerService.GetCustomerByEmail(filter.Search);
                if (customer != null)
                    return RedirectToAction("Edit", new { id = customer.Id });

                var customerByCode = ClientCodeService.GetCustomerByCode(filter.Search, Guid.Empty);
                if (customerByCode != null && customerByCode.Id != Guid.Empty)
                {
                    return RedirectToAction("Edit", new { id = customerByCode.Id, code = filter.Search });
                }
            }
            
            SetMetaInformation(T("Admin.Customers.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.CustomersCtrl);

            return View("~/Areas/Admin/Views/Customers/Index.cshtml");
        }


        public ActionResult GetCustomers(CustomersFilterModel model)
        {
            var exportToCsv = model.OutputDataType == FilterOutputDataType.Csv;

            var result = new GetCustomersPaging(model, exportToCsv).Execute();

            if (exportToCsv)
            {
                if (!CustomerContext.CurrentCustomer.HasRoleAction(RoleAction.Analytics))
                    return JsonAccessDenied();
                
                var fileName = "export_grid_customers.csv".FileNamePlusDate();
                var items = result?.DataItems?.Select(x => (AdminCustomerExportToCsvModel) x).ToList();
                
                var fullFilePath = new ExportCustomersFromGrid(items, fileName).Execute();
                return FileDeleteOnUpload(fullFilePath, "application/octet-stream", fileName);
            }

            return Json(result);
        }


        public JsonResult GetCustomerIds(CustomersFilterModel command)
        {
            var customerIds = new List<Guid>();
            Command(command, (id, c) =>
            {
                customerIds.Add(id);
                return true;
            });
            return Json(new { customerIds });
        }

        #region Commands

        private bool Command(CustomersFilterModel command, Func<Guid, CustomersFilterModel, bool> func)
        {
            bool result = true;

            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                {
                    if (func(id, command) == false)
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                var handler = new GetCustomersPaging(command);
                var ids = handler.GetItemsIds("[Customer].[CustomerID]");

                foreach (var id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                    {
                        if (func(id, command) == false)
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCustomers(CustomersFilterModel command)
        {
            bool result = Command(command, (id, c) => DeleteCustomerById(id));
            return result ? JsonOk(result) : JsonError(T("Admin.Customers.ErrorWhileDeletingUsers"));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddTagsToCustomers(CustomersFilterModel command, List<string> tags)
        {
            foreach (var tag in tags)
            {
                if (TagService.Get(tag) == null)
                {
                    TagService.Add(new Tag
                    {
                        Name = tag,
                        Enabled = true
                    });
                }
            }

            bool result = Command(command, (id, c) =>
            {
                try
                {
                    var customer = CustomerService.GetCustomer(id);
                    var prevTags = TagService.Gets(customer.Id).Select(x => x.Name).ToList();
                    var newTags = tags.Where(x => !prevTags.Contains(x)).ToList();

                    customer.Tags = prevTags.Concat(newTags).Select(x => new Tag
                    {
                        Name = x,
                        Enabled = true,
                    }).ToList();

                    CustomerService.UpdateCustomer(customer);
                }
                catch(Exception ex)
                {
                    Debug.Log.Error("AddTagsToCustomers", ex);
                    return false;
                }
                
                return true;
            });

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_AddTagToCustomer);

            return result ? JsonOk() : JsonError();
        }
        #endregion


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCustomer(Guid customerid)
        {
            var result = DeleteCustomerById(customerid);
            return Json(result);
        }

        private bool DeleteCustomerById(Guid customerid)
        {
            if (!CustomerService.CanDelete(customerid))
            {
                // add message this customer can be deleted
                return false;
            }

            try
            {
                CustomerService.DeleteCustomer(customerid);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("DeleteCustomerById", ex);
                return false;
            }

            return true;
        }

        #endregion

        #region Add | Edit customer

        // Редирект на PopupAdd
        public ActionResult Add(AddCustomerModel model)
        {
            return Redirect(
                UrlService.GetAdminUrl(model.OrderId != null
                    ? "customers?orderid=" + model.OrderId + "&customerIdInfo="
                    : "customers?customerIdInfo="));
        }

        // Редирект на Popup
        public ActionResult Edit(Guid id, string code)
        {
            return Redirect(UrlService.GetAdminUrl("customers?customerIdInfo=" + id));
        }

        [Auth(EAuthErrorType.PartialView, RoleAction.Customers)]
        public ActionResult PopupAdd(AddCustomerModel addCustomerModel)
        {
            var model = new GetCustomer(addCustomerModel).Execute();
            if (model == null)
                return Error404(partial: true);

            return PartialView("~/Areas/Admin/Views/Customers/AddEditPopup.cshtml", model);
        }

        [Auth(EAuthErrorType.PartialView, RoleAction.Customers)]
        public ActionResult Popup(Guid id, string code)
        {
            var model = new GetCustomer(id, code).Execute();
            if (model == null)
                return Error404(partial: true);

            return PartialView("~/Areas/Admin/Views/Customers/AddEditPopup.cshtml", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult SavePopup(CustomersModel model, bool notSaveContact = false)
        {
            if (ModelState.IsValid)
            {
                var result = new AddUpdateCustomer(model).Execute(notSaveContact);
                if (result)
                    return JsonOk(model.CustomerId);
            }
            return JsonError();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddEdit(CustomersModel model)
        {
            if (ModelState.IsValid)
            {
                var result = new AddUpdateCustomer(model).Execute();
                if (result)
                {
                    ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                    return Redirect(UrlService.GetAdminUrl("customers?customerIdInfo=" + model.CustomerId));
                }
            }

            ShowErrorMessages();

            SetMetaInformation(T("Admin.Customers.Edit.Title"));
            SetNgController(NgControllers.NgControllersTypes.CustomerCtrl);

            if (model.IsEditMode)
                Redirect(UrlService.GetAdminUrl("customers?customerIdInfo=" + model.CustomerId));

            return RedirectToAction("Add");
        }

        //[HttpGet]
        //public JsonResult GetOrders(Guid customerId)
        //{
        //    var model = OrderService.GetCustomerOrderHistory(customerId).Select(x => new
        //    {
        //        OrderId = x.OrderID,
        //        x.OrderNumber,
        //        x.Status,
        //        x.Payed,
        //        x.ArchivedPaymentName,
        //        x.ShippingMethodName,
        //        Sum = PriceFormatService.FormatPrice(x.Sum),
        //        OrderDate = Culture.ConvertDate(x.OrderDate),
        //        x.ManagerId,
        //        x.ManagerName
        //    });
        //    return Json(new { DataItems = model });
        //}


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangePassword(Guid customerId, string pass, string pass2)
        {
            if (string.IsNullOrWhiteSpace(pass) || pass != pass2 || pass.Length < 6)
                return Json(new { result = false, error = T("Admin.Customers.Password6Chars") });

            var customer = CustomerService.GetCustomer(customerId);
            if (customer == null)
                return Json(new { result = false, error = T("Admin.Customers.UserIsNotFound") });

            CustomerService.ChangePassword(customerId, pass, false);

            return Json(new { result = true });
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ProcessCustomerContact(SuggestAddressQueryModel address)
        {
            if (address == null || address.City.IsNullOrEmpty())
                return JsonError();

            IpZone zone;
            if ((address.ByCity || address.Region.IsNullOrEmpty()) && (zone = IpZoneService.GetZoneByCity(address.City, null)) != null)
            {
                address.District = zone.District;
                address.Region = zone.Region;
                address.Country = zone.CountryName;
                address.Zip = zone.Zip;
            }
            ModulesExecuter.ProcessAddress(address, true);

            return JsonOk(address);
        }

        #endregion

        #region View

        public ActionResult View(Guid id, string code)
        {
            var model = new GetCustomerView(id, code, true).Execute();
            if (model == null)
                return RedirectToAction("Index");

            SetMetaInformation(T("Admin.Customers.Edit.Title"));
            SetNgController(NgControllers.NgControllersTypes.CustomerViewCtrl);

            return View("View", model);
        }

        [HttpGet]
        public JsonResult GetView(Guid id, string code)
        {
            var model = new GetCustomerView(id, code, false).Execute();
            return Json(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateAdminComment(Guid id, string comment)
        {
            var customer = CustomerService.GetCustomer(id);
            if (customer == null)
                return JsonError();

            customer.AdminComment = comment;
            CustomerService.UpdateCustomer(customer);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateClientStatus(Guid id, CustomerClientStatus clientStatus)
        {
            var customer = CustomerService.GetCustomer(id);
            if (customer == null)
                return JsonError();

            customer.ClientStatus = clientStatus;
            CustomerService.UpdateCustomer(customer);

            if (clientStatus != CustomerClientStatus.None)
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_StatusChanged, clientStatus.ToString());

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeCustomerManager(Guid customerId, int? managerId)
        {
            CustomerService.ChangeCustomerManager(customerId, managerId);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DisableDesktopAppNotification(string appName)
        {
            if (string.Equals(appName, "viber", StringComparison.OrdinalIgnoreCase))
            {
                SettingsAdmin.ShowViberDesktopAppNotification = false;
            }
            if (string.Equals(appName, "whatsapp", StringComparison.OrdinalIgnoreCase))
            {
                SettingsAdmin.ShowWhatsAppDesktopAppNotification = false;
            }
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendMobileAppNotification(Guid customerId, string body, string title)
        {
            var error = MobileApp.NotificationService.SendNotification(new MobileApp.Notification
            {
                CustomerId = customerId,
                Title = title,
                Body = body
            });
            if (error.IsNullOrEmpty())
                return JsonOk();
            return JsonError(error);
        }

        #endregion

        #region Get | Send letter

        public JsonResult GetLetterToCustomer(GetLetterToCustomerModel model)
        {
            var result = new GetLetterToCustomerResult
            {
                Error = MailService.ValidateMailSettingsBeforeSending()
            };

            Customer customer = null;
            
            if (!string.IsNullOrEmpty(model.CustomerId) && string.IsNullOrEmpty(model.FirstName) &&
                string.IsNullOrEmpty(model.LastName) && string.IsNullOrEmpty(model.Patronymic))
            {
                customer = CustomerService.GetCustomer(model.CustomerId.TryParseGuid());
            }

            if (customer is null)
            {
                customer = new Customer
                {
                    EMail = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = model.Patronymic,
                };
            }

            var manager = CustomerContext.CurrentCustomer.IsManager ? ManagerService.GetManager(CustomerContext.CurrentCustomer.Id) : null;
            var managerName = manager != null ? manager.FullName : string.Empty;
            var managerSign = manager != null ? manager.Sign : string.Empty;

            if (model.TemplateId != -1)
            {
                var notFormatted = model.CustomerId == null &&
                                   string.IsNullOrEmpty(model.FirstName) &&
                                   string.IsNullOrEmpty(model.LastName) &&
                                   string.IsNullOrEmpty(model.Patronymic);

                
                
                var service = new MailAnswerTemplateService();

                var template = service.Get(model.TemplateId, true);
                if (template != null)
                {
                    var letterBuilder = new LetterBuilder()
                        .Add(new CustomerLetterBuilder(customer));

                    if (model.OrderId.HasValue)
                    {
                        var order = OrderService.GetOrder(model.OrderId.Value);
                        letterBuilder.Add(new OrderLetterBuilder(order));
                    }

                    if (model.LeadId.HasValue)
                    {
                        var lead = LeadService.GetLead(model.LeadId.Value);
                        letterBuilder.Add(new LeadLetterBuilder(lead));
                    }

                    result.Subject = letterBuilder.FormatText(template.Subject);
                    result.Text = letterBuilder.FormatText(template.Body);
                }
            }
            else
            {
                var mail =
                    new SendToCustomerTemplate(model.FirstName ?? "", model.LastName ?? "", model.Patronymic ?? "", "",
                            "", managerName, model.OrderId, model.LeadId)
                        .BuildMail();

                result.Text = mail.Body;

                if (!string.IsNullOrEmpty(model.ReId))
                {
                    var email = CustomerService.GetEmailImap(model.ReId, null);
                    if (email != null)
                    {
                        result.Text +=
                            " <br><br><br> <blockquote style=\"margin:0px 0px 0px 0.8ex;border-left:1px solid rgb(204,204,204);padding-left:1ex\"> " +
                            (email.HtmlBody ?? email.TextBody ?? "").Replace("<html>", "").Replace("<HTML>", "").Replace("<body>", "").Replace("<BODY>", "").Replace("</body>", "").Replace("</BODY>", "").Replace("</HTML>", "") +
                            "</blockquote>";
                    }
                }
            }

            if (SettingsDesign.ShowUserAgreementForPromotionalNewsletter)
            {
                if (Guid.Empty.Equals(customer.Id))
                {
                    var customerId = default(Guid?);

                    if (!string.IsNullOrEmpty(model.CustomerId))
                        customerId = model.CustomerId.TryParseGuid();
                    else if (model.OrderId != null)
                        customerId = OrderService.GetOrderCustomer(model.OrderId.Value)?.CustomerID;
                    else if (model.LeadId != null)
                        customerId = LeadService.GetLead(model.LeadId.Value)?.CustomerId;
                    
                    if (customerId != null)
                        customer = CustomerService.GetCustomer(customerId.Value);
                }
                
                if (customer != null)
                    result.UserNotAgree = !customer.IsAgreeForPromotionalNewsletter;
            }

            return Json(result);
        }

        public JsonResult GetAnswerTemplates()
        {
            var templates = new List<MailAnswerTemplate>
            {
                new MailAnswerTemplate { TemplateId = -1, Name = T("Admin.Customers.Empty") }
            };

            templates.AddRange(new MailAnswerTemplateService().Gets(true));
            return JsonOk(templates);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendLetterToCustomer(SendLetterToCustomerModel model)
        {
            return ProcessJsonResult(new SendLetterToCustomersHandler(model));
        }

        #endregion

        [HttpGet]
        public JsonResult GetCustomerWithContact(Guid customerId, string code)
        {
            var customer = CustomerService.GetCustomer(customerId);
            if (customer == null && code.IsNotEmpty())
            {
                customer = ClientCodeService.GetCustomerByCode(code, customerId);
                customer.Code = code;
            }

            if (customer == null)
                return Json(null);

            var customerFields = CustomerFieldService.GetCustomerFieldsWithValue(customerId);
            
            return Json(new
            {
                customer.Id,
                customer.FirstName,
                customer.LastName,
                customer.Patronymic,
                Email = customer.EMail,
                customer.Phone,
                customer.StandardPhone,
                customer.IsAgreeForPromotionalNewsletter,
                customer.BonusCardNumber,
                customer.CustomerGroup,
                customer.Code,
                customer.RegistredUser,
                customer.Contacts,
                customer.Organization,
                customer.BirthDay,
                CustomerType = customer.CustomerType.ToString(),
                customerFields
            });
        }

        [HttpGet]
        public JsonResult GetCustomersAutocomplete(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(null);

            var customers = CustomerService.GetCustomersForAutocomplete(q, 10).Select(x => new
            {
                label = string.Format("{0} {1}{2}, {3} {4}", x.LastName, x.FirstName, x.Patronymic.IsNotEmpty() ? " " + x.Patronymic : string.Empty, x.EMail, x.Phone),
                value = x.Id,
                CustomerId = x.Id,
                x.FirstName,
                x.LastName,
                x.Patronymic,
                x.Organization,
                Email = x.EMail,
                x.Phone,
                x.StandardPhone,
                CustomerType = x.CustomerType.ToString()
            });

            return Json(customers);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetCustomersAutocompleteByFullNameParts(string firstName, string lastName, string patronymic)
        {
            if (string.IsNullOrWhiteSpace(firstName)
                && string.IsNullOrWhiteSpace(lastName)
                && string.IsNullOrWhiteSpace(patronymic))
                return Json(null);

            var customers =
                CustomerService.GetCustomersForAutocomplete(firstName, lastName, patronymic, 10)
                               .Select(x => new
                                {
                                    label =
                                        $"{x.LastName} {x.FirstName}{(x.Patronymic.IsNotEmpty() ? " " + x.Patronymic : string.Empty)}, {x.EMail} {x.Phone}",
                                    value = x.Id,
                                    CustomerId = x.Id,
                                    x.FirstName,
                                    x.LastName,
                                    x.Patronymic,
                                    x.Organization,
                                    Email = x.EMail,
                                    x.Phone,
                                    x.StandardPhone,
                                    CustomerType = x.CustomerType.ToString()
                                });

            return Json(customers);
        }
        
        [HttpGet]
        public JsonResult GetCustomersByPhoneAutocomplete(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(null);

            var customers = CustomerService.GetCustomersByPhoneForAutocomplete(q, 10).Select(x => new
            {
                label = string.Format("{4}, {0} {1}{2}, {3}", x.LastName, x.FirstName, x.Patronymic.IsNotEmpty() ? " " + x.Patronymic : string.Empty, x.EMail, x.Phone.TrimStart('+')),
                value = x.Id,
                CustomerId = x.Id,
                x.FirstName,
                x.LastName,
                x.Patronymic,
                x.Organization,
                Email = x.EMail,
                x.Phone,
                x.StandardPhone,
                CustomerType = x.CustomerType.ToString()
            });

            return Json(customers);
        }

        [HttpGet]
        public JsonResult GetCustomerFields()
        {
            return Json(CustomerFieldService.GetCustomerFields());
        }

        [HttpPost]
        [Auth(RoleAction.Customers, RoleAction.Crm, RoleAction.Booking)]
        public ActionResult CustomerFieldsForm(List<CustomerFieldWithValue> customerFields, Guid? customerId, CustomerType? customerType, bool IgnoreRequired)
        {
            ViewBag.CustomerFieldModelPrefix = "$ctrl";
            var fields = customerFields != null && customerType.HasValue && (!customerId.HasValue || customerId == Guid.Empty)
                ? customerFields
                : CustomerFieldService.GetCustomerFieldsWithValue(customerId ?? Guid.Empty);
            var viewModel = new CustomerFieldsViewModel(fields, ngModelName: "$ctrl", onSelectFunc: "$ctrl.processCompanyName($item)", ngVariableVisible: "$ctrl.customerType", ignoreRequired: IgnoreRequired);
            if (customerType.HasValue)
            {
                viewModel.FilteredCustomerFields = fields?
                    .Where(x => x.CustomerType == customerType || x.CustomerType == CustomerType.All)
                    .ToList();
            }
            return PartialView("_CustomerFields", viewModel);
        }

        [HttpGet]
        public JsonResult GetCustomerFieldValues(int id)
        {
            return Json(CustomerFieldService.GetCustomerFieldValues(id).Select(x => new { label = x.Value, value = x.Value }));
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetStandartPhone(string phone)
        {
            return JsonOk(StringHelper.ConvertToStandardPhone(phone, true, true));
        }

        #region contacts
        public JsonResult GetCustomerContacts(Guid customerId)
        {
            var customer = CustomerService.GetCustomer(customerId);

            if (customer == null || !customer.RegistredUser)
                return Json("");

            var customerContacts =
                from item in customer.Contacts
                select new
                {
                    customer.CustomerCompanyName,
                    item.ContactId,
                    item.Country,
                    item.City,
                    item.District,
                    item.Name,
                    customer.FirstName,
                    customer.LastName,
                    customer.Patronymic,
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
                    
                    SettingsCheckout.IsShowFullAddress,
                    AggregatedAddress = new[] { SettingsCustomers.IsRegistrationAsLegalEntity ? customer.CustomerCompanyName: item.Name, item.Zip, item.Country, item.Region, item.City, item.District, !string.IsNullOrEmpty(item.Street) ? LocalizationService.GetResource("Core.Orders.OrderContact.Street") + " " + item.Street : "", !string.IsNullOrEmpty(item.House) ? LocalizationService.GetResource("Admin.Js.CustomerView.House") + item.House : "", !string.IsNullOrEmpty(item.Structure) ? LocalizationService.GetResource("Admin.Js.CustomerView.Struct") + item.Structure : "", !string.IsNullOrEmpty(item.Entrance) ? LocalizationService.GetResource("Core.Orders.OrderContact.Entrance") + " " + item.Entrance : "", !string.IsNullOrEmpty(item.Floor) ? LocalizationService.GetResource("Admin.Customers.Customer.Floor").ToLower() + " " + item.Floor : "", !string.IsNullOrEmpty(item.Apartment) ? LocalizationService.GetResource("Admin.Js.CustomerView.Ap") + item.Apartment : "" }.
                                        Where(str => str.IsNotEmpty()).AggregateString(", ")

                };

            return Json(customerContacts.ToList());
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
        public JsonResult AddUpdateCustomerContact(Guid customerId, CustomerAccountModel account)
        {
            var contact = new AddUpdateContactHandler().Execute(customerId, account);
            return Json(contact);
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
        public JsonResult DeleteCustomerContact(Guid customerId, string contactId)
        {
            var customer = CustomerService.GetCustomer(customerId);
            if (customer == null || !customer.RegistredUser)
                return Json(false);

            var id = contactId.TryParseGuid();
            if (id != Guid.Empty && customer.Contacts.Any(contact => contact.ContactId == id))
            {
                CustomerService.DeleteContact(id);
            }
            return Json(true);
        }

        #endregion


        #region Tags
        public JsonResult GetAutocompleteTags()
        {
            return Json(new
            {
                tags = TagService.GetAutocompleteTags().Select(x => new { value = x.Name })
            });
        }

        public JsonResult GetTags(Guid customerId)
        {
            return Json(new
            {
                tags = TagService.GetAutocompleteTags().Select(x => new { value = x.Name }),
                selectedTags = TagService.Gets(customerId, onlyEnabled: false).Select(x => new { value = x.Name })
            });
        }
        #endregion
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeSocialCustomer(Guid fromCustomerId, Guid toCustomerId, string type)
        {
            return ProcessJsonResult(new ChangeSocialCustomer(fromCustomerId, toCustomerId, type));
        }

        public JsonResult GetCustomerTypes()
        {
            return Json(CustomerService.GetCustomerTypesSelectOptions());
        }
        
        [HttpGet]
        public JsonResult GetCustomerCountries()
        {
            var countries = SQLDataAccess
                .Query<string>("Select distinct Country from [Customers].[Contact] Order By Country")
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => new {label = x, value = x})
                .ToList();
            return Json(countries);
        }
        
        #region ExportCustomers

        [HttpPost, ValidateJsonAntiForgeryToken]
        [Auth(RoleAction.Customers)]
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        public JsonResult GetExportCustomersSettings()
        {
            return JsonOk(new GetExportCustomersSettings().Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        [Auth(RoleAction.Customers)]
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        public JsonResult ExportCustomers(ExportCustomersSettings settings)
        {
            new ExportCustomers(settings).Execute();
            return Json(new { result = true });
        }

        #endregion
    }
}
