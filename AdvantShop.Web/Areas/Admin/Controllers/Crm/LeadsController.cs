using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Saas;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Attachments;
using AdvantShop.Web.Admin.Handlers.Catalog.Import;
using AdvantShop.Web.Admin.Handlers.Leads;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Attachments;
using AdvantShop.Web.Admin.Models.Crm.Leads;
using AdvantShop.Web.Admin.Models.Tasks;
using AdvantShop.Web.Admin.ViewModels.Crm.Leads;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Controllers.Crm
{
    [Auth(RoleAction.Crm)]
    [SaasFeature(ESaasProperty.HaveCrm)]
    [AccessBySettings(Core.Services.Configuration.EProviderSetting.CrmActive, ETypeRedirect.AdminPanel)]
    public partial class LeadsController : BaseAdminController
    {
        #region Leads list

        public ActionResult Index(int? salesFunnelId, bool? useKanban, bool? useDefault)
        {
            if (salesFunnelId == null || salesFunnelId == 0)
            {
                var salesFunnel = SalesFunnelService.GetList().FirstOrDefault(SalesFunnelService.CheckAccess);
                if (salesFunnel != null)
                    salesFunnelId = salesFunnel.Id;
            }

            if (useDefault != null && useDefault.Value)
            {
                salesFunnelId = SettingsCrm.DefaultSalesFunnelId;
            }

            var funnel = SalesFunnelService.Get(salesFunnelId ?? 0);
            if (funnel == null && salesFunnelId != -1)
                return RedirectToAction("Index", new { salesFunnelId = -1, useKanban = false });

            if (funnel != null && !SalesFunnelService.CheckAccess(funnel))
            {
                funnel = SalesFunnelService.GetList().FirstOrDefault(SalesFunnelService.CheckAccess);
                if (funnel != null)
                    return Redirect("leads?salesFunnelId=" + funnel.Id + (useKanban != null ? "&useKanban=" + useKanban : "") + "#");

                return Redirect("leads?salesFunnelId=-1&useKanban=false#");
            }

            var customer = CustomerContext.CurrentCustomer;

            var model = new LeadsListViewModel()
            {
                SalesFunnelId = funnel != null ? funnel.Id : 0,
                SalesFunneName = funnel != null ? funnel.Name : null,
                UseKanban = useKanban != null ? useKanban.Value : CommonHelper.GetCookieString("leads_viewmode") != "grid",

                IsFullAccess = customer.IsAdmin || (customer.IsModerator &&
                                                    (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.All || customer.HasRoleAction(RoleAction.Settings)))
            };


            SetMetaInformation(T("Admin.Leads.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.LeadsCtrl);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Trial_VisitCRM);

            SalesFunnelService.SetLastAdminFunnel(salesFunnelId);

            return View(model);
        }

        public ActionResult LastFunnel()
        {
            var salesFunnelId = SalesFunnelService.GetLastAdminFunnel() ?? SettingsCrm.DefaultSalesFunnelId;

            return RedirectToAction("Index", new { salesFunnelId });
        }


        public ActionResult DefaultLeadsList()
        {
            return RedirectToAction("Index", new { useDefault = true });
        }

        public ActionResult GetLeads(LeadsFilterModel model)
        {
            var exportToCsv = model.OutputDataType == FilterOutputDataType.Csv;

            var result = new GetLeads(model, exportToCsv).Execute();

            if (exportToCsv)
            {
                var fileName = "export_leads_" + DateTime.Now.ToString("ddMMyyhhmmss") + ".csv";
                var fullFilePath = new ExportLeadsHandler(result, model.SalesFunnelId, fileName).Execute();
                return File(fullFilePath, "application/octet-stream", fileName);
            }

            return Json(result);
        }

        public JsonResult GetLeadCustomerIds(LeadsFilterModel model, List<Guid> ids = null, int? itemsPerPage = 1000000)
        {
            if (itemsPerPage != null)
                model.ItemsPerPage = itemsPerPage.Value;

            var customerIds = new GetLeads(model).GetCustomerIds();

            return
                Json(ids == null || ids.Count == 0 || model.SelectMode == SelectModeCommand.None
                    ? customerIds
                    : customerIds.Where(x => x != null && !ids.Contains(x.Value)).ToList());
        }
        
        public JsonResult GetLetterRecipients(LeadsFilterModel model, List<int> ids = null, int? itemsPerPage = 1000000)
        {
            if (itemsPerPage != null)
                model.ItemsPerPage = itemsPerPage.Value;

            var recipients = new GetLeads(model, false, true).GetRecipients(ids, model.SelectMode, true);
            return Json(recipients);
        }
        
        public JsonResult GetSmsRecipients(LeadsFilterModel model, List<int> ids = null, int? itemsPerPage = 1000000)
        {
            if (itemsPerPage != null)
                model.ItemsPerPage = itemsPerPage.Value;

            var recipients = new GetLeads(model, false, true).GetRecipients(ids, model.SelectMode, false);
            return Json(recipients);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeBuyInOneClickCreateOrder()
        {
            SettingsCheckout.BuyInOneClickCreateOrder = false;
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult HideOrderFromBuyInOneClickMsg()
        {
            SettingsNotifications.HideOrderFromBuyInOneClickAdminMsg = true;
            return JsonOk();
        }

        #region Kanban

        public JsonResult GetKanban(LeadsKanbanFilterModel model)
        {
            return Json(new GetLeadsKanbanHandler(model).Execute());
        }

        public JsonResult GetKanbanCustomerIds(LeadsKanbanFilterModel model)
        {
            return Json(new GetLeadsKanbanHandler(model, true).GetCustomerIds());
        }
        
        public JsonResult GetKanbanLetterRecipients(LeadsKanbanFilterModel model)
        {
            return Json(new GetLeadsKanbanHandler(model, onlyEmails: true).GetRecipients(true));
        }
        
        public JsonResult GetKanbanSmsRecipients(LeadsKanbanFilterModel model)
        {
            return Json(new GetLeadsKanbanHandler(model, onlyEmails: true).GetRecipients(false));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult KanbanExport(LeadsKanbanFilterModel model)
        {
            var leadIds = new GetLeadsKanbanHandler(model).GetLeadIds();

            var fileName = "export_leads_" + DateTime.Now.ToString("ddMMyyhhmmss") + ".csv";
            var fullFilePath = new ExportLeadsHandler(leadIds, model.SalesFunnelId, fileName).GetFileUrl();
            return Json(new { url = fullFilePath });
        }

        public JsonResult GetKanbanColumn(LeadsKanbanFilterModel model)
        {
            return Json(new GetLeadsKanbanHandler(model).GetCards());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeSorting(int id, int? prevId, int? nextId)
        {
            return Json(new { result = new ChangeLeadSorting(id, prevId, nextId).Execute() });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeLeadDealStatus(int leadId, int dealStatusId)
        {
            var model = new GetLead(leadId).Execute();
            if (model == null || model.Lead == null)
                return JsonError();

            var salesFunnel = SalesFunnelService.Get(model.Lead.SalesFunnelId);
            var dealStatus = DealStatusService.Get(dealStatusId);
            if (salesFunnel == null || dealStatus == null)
                return JsonError();

            if (dealStatus.Status == SalesFunnelStatusType.FinalSuccess && salesFunnel.FinalSuccessAction == SalesFunnelFinalSuccessAction.CreateOrder)
            {
                var order = OrderService.CreateOrder(model.Lead);
                return order == null ? JsonError() : JsonOk(new { orderId = order.OrderID, orderNumber = order.Number });
            }

            model.Lead.DealStatusId = dealStatusId;
            return new SaveLead(model).Execute() ? JsonOk() : JsonError();
        }

        #endregion

        #region Command

        private void Command(LeadsFilterModel command, Action<int, LeadsFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetLeads(command).GetItemsIds();
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [Auth, HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteLeads(LeadsFilterModel command)
        {
            Command(command, (id, c) => LeadService.DeleteLead(id));
            return JsonOk();
        }

        public JsonResult GetOrderSources()
        {
            return Json(OrderSourceService.GetOrderSources().Select(x => new SelectItemModel(x.Name, x.Id)));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeManager(LeadsFilterModel command, int? newManagerId)
        {
            Command(command, (id, c) => LeadService.UpdateLeadManager(id, newManagerId));

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeSalesFunnelAndDealStatus(LeadsFilterModel command, int newSalesFunnelId, int newDealStatusId)
        {
            var funnel = SalesFunnelService.Get(newSalesFunnelId);
            if (funnel == null)
                return JsonError();

            var status = DealStatusService.GetList(funnel.Id).FirstOrDefault(x => x.Id == newDealStatusId);
            if (status == null)
                return JsonError();

            var createOrderOnFinalSuccess = 
                command.CreateOrderOnFinalSuccess.HasValue && 
                command.CreateOrderOnFinalSuccess.Value && 
                funnel.FinalSuccessAction == SalesFunnelFinalSuccessAction.CreateOrder && 
                status.Status == SalesFunnelStatusType.FinalSuccess;

            Command(command, (id, c) =>
            {
                var lead = LeadService.GetLead(id);
                if (lead == null)
                    return;
                
                if (lead.SalesFunnelId == newSalesFunnelId && lead.DealStatusId == newDealStatusId)
                    return;

                lead.SalesFunnelId = newSalesFunnelId;
                lead.DealStatusId = newDealStatusId;

                LeadService.UpdateLead(lead, false);

                if (createOrderOnFinalSuccess && OrderService.GetOrderIdByLeadId(lead.Id) == 0)
                {
                    OrderService.CreateOrder(lead, null, false);
                }
            });

            return JsonOk();
        }

        #endregion

        #endregion

        #region Add | Edit Lead

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(AddLeadModel model)
        {
            foreach (var key in new List<string>() { "Sum" })
            {
                if (ModelState.ContainsKey(key))
                    ModelState[key].Errors.Clear();
            }

            if (ModelState.IsValid)
            {
                var id = new AddLead(model).Execute();
                return Json(new { result = id != 0, leadId = id });
            }

            var errors =
                (from modelState in ViewData.ModelState.Values
                 from error in modelState.Errors
                 select error.ErrorMessage).ToList();

            return Json(new { result = false, errors = String.Join(", ", errors) });
        }

        public ActionResult Edit(int id)
        {
            var model = new GetLead(id).Execute();
            if (model == null)
                return RedirectToAction("Index");

            SetMetaInformation(T("Admin.Leads.Edit.Title") + " " + model.Id);
            SetNgController(NgControllers.NgControllersTypes.LeadCtrl);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(LeadModel model)
        {
            if (ModelState.IsValid)
            {
                var result = new SaveLead(model).Execute();
                if (result)
                {
                    ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                    return RedirectToAction("Edit", new { id = model.Id });
                }
            }

            ShowErrorMessages();

            SetMetaInformation(T("Admin.Leads.Edit.Title") + " " + model.Id);
            SetNgController(NgControllers.NgControllersTypes.LeadCtrl);

            return RedirectToAction("Edit", new { id = model.Id });
        }

        [Auth(EAuthErrorType.PartialView, RoleAction.Crm)]
        [SaasFeature(ESaasProperty.HaveCrm, partial: true)]
        public ActionResult Popup(int id)
        {
            var model = new GetLead(id).Execute();
            if (model == null || model.Lead == null)
                return Error404(partial: true);

            return PartialView("_Popup", model);
        }

        [HttpPost]
        public JsonResult SaveLead(LeadModel model)
        {
            if (ModelState.IsValid)
            {
                var result = new SaveLead(model).Execute();
                return result ? JsonOk(true) : JsonError();
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult DeleteLead(int leadId)
        {
            LeadService.DeleteLead(leadId);
            return JsonOk();
        }

        public JsonResult GetLeadInfo(int id)
        {
            var model = new GetLead(id).Execute();
            if (model == null || model.Lead == null)
                return Json(null);

            return Json(model);
        }

        public JsonResult GetLeadForm(Guid? customerId, bool fromCart, int? salesFunnelId)
        {
            var manager = ManagerService.GetManager(CustomerContext.CurrentCustomer.Id);

            List<LeadTempProduct> products = null;
            float sum = 0;

            if (fromCart && customerId.HasValue)
            {
                var cart = ShoppingCartService.GetShoppingCartFromDb(ShoppingCartType.ShoppingCart, customerId.Value, false);
                products = cart.Select(x => new LeadTempProduct()
                {
                    OfferId = x.OfferId,
                    ArtNo = x.Offer.ArtNo,
                    Amount = x.Amount,
                    Name = x.Offer.Product.Name +
                        (x.Offer.Size != null ? ", " + x.Offer.Size.SizeName : "") +
                        (x.Offer.Color != null ? ", " + x.Offer.Color.ColorName : ""),
                    Price = x.Offer.RoundedPrice,
                    PreparedPrice = x.Offer.RoundedPrice.FormatPrice()
                }).ToList();
                sum = products.Sum(x => x.Price);
            }

            var funnels = SalesFunnelService.GetList();
            var funnelId = salesFunnelId != null && salesFunnelId > 0 ? salesFunnelId.Value : SettingsCrm.DefaultSalesFunnelId;
            var showCustomerType = SettingsCustomers.IsRegistrationAsPhysicalEntity && SettingsCustomers.IsRegistrationAsLegalEntity;
            var customerTypes = Enum.GetValues(typeof(CustomerType)).Cast<CustomerType>().Where(x => x != CustomerType.All).Select(x => (object)new
            {
                label = x.Localize(),
                value = x.ToString(),
            }).ToList();

            return Json(new
            {
                currencySymbol = SettingsCatalog.DefaultCurrency.Symbol,
                managers = ManagerService.GetManagers(RoleAction.Crm).Select(x => new SelectItemModel(x.FullName, x.ManagerId)),
                managerId = manager != null ? manager.ManagerId.ToString() : "",
                salesFunnels = funnels.Select(x => new SelectItemModel(x.Name, x.Id)),
                salesFunnelId = funnelId,
                statuses = DealStatusService.GetList(funnelId).Select(x => new SelectItemModel(x.Name, x.Id)),
                products,
                sum,
                customerTypes = customerTypes,
                customerType = SettingsCustomers.IsRegistrationAsPhysicalEntity ? customerTypes[0] : customerTypes[1],
                showCustomerType = showCustomerType
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateOrder(int leadId, bool? force)
        {
            var lead = LeadService.GetLead(leadId);
            if (lead == null || LeadService.CheckAccess(lead) == false)
                return Json(new { result = false });

            if (force == null || !force.Value)
            {
                var funnel = SalesFunnelService.Get(lead.SalesFunnelId);
                if (funnel != null && funnel.FinalSuccessAction == SalesFunnelFinalSuccessAction.None)
                {
                    var status = DealStatusService.GetList(funnel.Id).FirstOrDefault(x => x.Status == SalesFunnelStatusType.FinalSuccess);
                    if (status != null)
                    {
                        lead.DealStatusId = status.Id;
                        LeadService.UpdateLead(lead, false);

                        Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_DealStatusChanged);
                    }
                    return Json(new { result = true });
                }
            }

            var order = OrderService.CreateOrder(lead, null, false);
            if (order == null)
                return Json(new { result = false });

            return Json(new { result = true, orderId = order.OrderID, code = order.Code });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CancelLead(int leadId)
        {
            var model = new GetLead(leadId).Execute();
            DealStatus cancelStatus;
            if (model == null || model.Lead == null || !LeadService.CheckAccess(model.Lead) ||
                (cancelStatus = DealStatusService.GetList(model.Lead.SalesFunnelId).FirstOrDefault(x => x.Status == SalesFunnelStatusType.Canceled)) == null)
                return JsonError();

            model.Lead.DealStatusId = cancelStatus.Id;
            var result = new SaveLead(model).Execute();

            return Json(new { result });
        }

        public JsonResult GetCompleteLeadForm(int leadId)
        {
            var lead = LeadService.GetLead(leadId);
            if (lead == null || !LeadService.CheckAccess(lead))
                return JsonError();
            var dealStatuses = DealStatusService.GetList(lead.SalesFunnelId);
            return JsonOk(new
            {
                successStatus = dealStatuses.FirstOrDefault(x => x.Status == SalesFunnelStatusType.FinalSuccess),
                cancelStatus = dealStatuses.FirstOrDefault(x => x.Status == SalesFunnelStatusType.Canceled)
            });
        }

        #region Lead Items 

        [HttpGet]
        public JsonResult GetLeadItems(LeadItemsFilterModel model) => Json(new GetLeadItems(model).Execute());

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddLeadItems(int leadId, List<int> offerIds)
        {
            var lead = LeadService.GetLead(leadId);

            if (lead == null || offerIds == null || offerIds.Count == 0)
                return Json(new { result = false });

            var result = new AddLeadItems(lead, offerIds).Execute();

            return Json(new { result });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateLeadItem(LeadItemModel model)
        {
            var lead = LeadService.GetLead(model.LeadId);
            if (lead == null)
                return Json(new { result = false });

            var leadItem = lead.LeadItems.Find(x => x.LeadItemId == model.LeadItemId);
            if (leadItem == null)
                return Json(new { result = false });

            leadItem.Price = model.Price;
            leadItem.Amount = model.Amount;

            var history = ChangeHistoryService.GetChanges(lead.Id, ChangeHistoryObjType.Lead, LeadService.GetLeadItem(leadItem.LeadItemId), leadItem, null, leadItem.LeadItemId);

            LeadService.UpdateLeadItem(lead.Id, leadItem);

            lead = LeadService.GetLead(model.LeadId);

            LeadService.OnLeadChanged(lead, history);

            return Json(new { result = true });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteLeadItem(int leadId, int leadItemId)
        {
            var lead = LeadService.GetLead(leadId);
            if (lead == null)
                return JsonError();

            var leadItem = lead.LeadItems.Find(x => x.LeadItemId == leadItemId);
            if (leadItem == null)
                return JsonError();

            var history = new List<ChangeHistory>();

            LeadService.DeleteLeadItem(leadItem);

            history.Add(new ChangeHistory(null)
            {
                ObjId = lead.Id,
                ObjType = ChangeHistoryObjType.Lead,
                ParameterName =
                    "Удален товар " + leadItem.Name +
                    (!string.IsNullOrEmpty(leadItem.ArtNo) ? " (" + leadItem.ArtNo + ")" : ""),
                ParameterId = leadItem.ProductId,
            });

            lead = LeadService.GetLead(leadId);

            LeadService.OnLeadChanged(lead, history);

            if (lead.LeadItems.Count == 0)
            {
                lead.Sum = 0;
                LeadService.UpdateLead(lead, false);
            }

            return JsonOk();
        }

        public JsonResult GetLeadItemsSummary(int leadId)
        {
            return Json(new GetLeadItemsSummary(leadId).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeDiscount(int leadId, float discount, bool isValue)
        {
            if (!isValue && (discount < 0 || discount > 100))
                return JsonError();

            var lead = LeadService.GetLead(leadId);
            if (lead == null)
                return JsonError();

            if (!isValue)
            {
                lead.Discount = discount;
                lead.DiscountValue = 0;
            }
            else
            {
                lead.Discount = 0;
                lead.DiscountValue = discount;
            }

            LeadService.UpdateLead(lead, false);

            return JsonOk();
        }

        #endregion

        #region Add Lead Modal

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetTempProducts(List<LeadTempProduct> model, Guid? customerId)
        {
            if (model == null)
                return Json(null);

            var ids = model.Select(x => x.OfferId).Distinct();

            var offers = new List<Offer>();

            foreach (var offerId in ids)
            {
                var offer = OfferService.GetOffer(offerId);
                if (offer == null)
                    continue;

                offers.Add(offer);
            }

            var customer = customerId != null ? CustomerService.GetCustomer(customerId.Value) : null;
            var customerGroup = customer != null
                ? customer.CustomerGroup
                : new CustomerGroup() {CustomerGroupId = CustomerGroupService.DefaultCustomerGroup};

            var products = new List<LeadTempProduct>();
            
            foreach (var offer in offers)
            {
                var modelOffer = model.Find(m => m.OfferId == offer.OfferId);

                var price = PriceService.GetFinalPrice(offer, customerGroup);

                var p = new LeadTempProduct()
                {
                    OfferId = offer.OfferId,
                    ArtNo = offer.ArtNo,
                    Amount = modelOffer != null ? modelOffer.Amount : 1,
                    Name =
                        offer.Product.Name + (offer.Size != null ? ", " + offer.Size.SizeName : "") +
                        (offer.Color != null ? ", " + offer.Color.ColorName : ""),
                    Price = price,
                    PreparedPrice = price.FormatPrice(),
                    PhotoName = offer.Photo.PhotoName,
                };
                products.Add(p);
            }

            return Json(new
            {
                products = products,
                sum = products.Sum(x => x.Price * x.Amount)
            });
        }

        //public JsonResult GetCustomerFields(Guid? customerId)
        //{
        //    var id = customerId != null ? customerId.Value : Guid.Empty;
        //    var fields = CustomerFieldService.GetCustomerFieldsWithValue(id) ?? new List<CustomerFieldWithValue>();
        //    return Json(fields);
        //}

        #endregion
        
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeCustomer(int leadId, Guid newCustomerId)
        {
            return ProcessJsonResult(() =>
            {
                var lead = LeadService.GetLead(leadId);
                if (lead == null)
                    throw new BlException("Лид не найден");
                lead.CustomerId = newCustomerId;
                LeadService.UpdateLead(lead, updateCustomer: false);
            });
        }

        #endregion

        #region Deal Statuses

        public JsonResult GetAllDealStatuses()
        {
            var dealStatuses = DealStatusService.GetList();
            return JsonOk(dealStatuses);
        }

        public JsonResult GetDealStatuses(int salesFunnelId)
        {
            return Json(GetSeparateDealStatuses(salesFunnelId));
        }

        public JsonResult GetDefaultDealStatuses()
        {
            return Json(GetSeparateDealStatuses(null));
        }

        private object GetSeparateDealStatuses(int? salesFunnelId)
        {
            var dealStatuses = salesFunnelId.HasValue ? DealStatusService.GetList(salesFunnelId.Value) : DealStatusService.GetDefaultDealStatuses();
            return new
            {
                items = dealStatuses.Where(x => x.Status == SalesFunnelStatusType.None),
                systemItems = dealStatuses.Where(x => x.Status != SalesFunnelStatusType.None)
            };
        }

        public JsonResult GetDealStatusesByLead(int leadId)
        {
            var lead = LeadService.GetLead(leadId);
            if (lead == null)
                return JsonError();
            return Json(DealStatusService.GetList(lead.SalesFunnelId).Select(x => new SelectItemModel(x.Name, x.Id)));
        }

        public JsonResult GetDealStatus(int id)
        {
            return Json(DealStatusService.Get(id));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddDealStatus(DealStatus status, int salesFunnelId)
        {
            if (string.IsNullOrWhiteSpace(status.Name))
                return JsonError();

            var list = DealStatusService.GetList(salesFunnelId);
            status.SortOrder = (list.Count > 0 ? list.Max(x => x.SortOrder) : 0) + 10;
            status.Color = status.Color.DefaultOrEmpty().Replace("#", "");

            DealStatusService.Add(status);
            SalesFunnelService.AddDealStatus(salesFunnelId, status.Id);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_DealStatusCUD);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateDealStatus(DealStatus status)
        {
            var s = DealStatusService.Get(status.Id);
            if (s == null || string.IsNullOrWhiteSpace(status.Name))
                return JsonError();

            s.Name = status.Name;
            s.SortOrder = status.SortOrder;
            s.Color = status.Color.DefaultOrEmpty().Replace("#", "");
            DealStatusService.Update(s);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_DealStatusCUD);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeDealStatusSorting(int id, int? prevId, int? nextId)
        {
            var status = DealStatusService.Get(id);
            if (status == null)
                return JsonError();

            var statuses = DealStatusService.GetList().Where(x => x.Id != status.Id).ToList();

            if (prevId != null)
            {
                var index = statuses.FindIndex(x => x.Id == prevId);
                statuses.Insert(index + 1, status);
            }
            else if (nextId != null)
            {
                var index = statuses.FindIndex(x => x.Id == nextId);
                statuses.Insert(index > 0 ? index - 1 : 0, status);
            }

            for (int i = 0; i < statuses.Count; i++)
            {
                statuses[i].SortOrder = i * 10 + 10;
                DealStatusService.Update(statuses[i]);
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteDealStatus(int id)
        {
            DealStatusService.Delete(id);
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_DealStatusCUD);
            return JsonOk();
        }

        #endregion

        #region Shippings

        public JsonResult GetShippings(int id, string country, string city, string district, string region, string zip,
            string street, string house, string structure, string apartment, string entrance, string floor)
        {
            return Json(new GetLeadShippings(id, country, city, district, region, zip, street, house, structure, apartment, entrance, floor).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CalculateShipping(int id, string country, string city, string district, string region, string zip,
            string street, string house, string structure, string apartment, string entrance, string floor, BaseShippingOption shipping)
        {
            var model = new GetLeadShippings(id, country, city, district, region, zip, street, house, structure, apartment, entrance, floor, shipping, false).Execute();

            var option = model.Shippings != null ? model.Shippings.FirstOrDefault(x => x.Id == shipping.Id) : null;

            if (option != null)
                option.UpdateFromBase(shipping);

            if (option == null && shipping.MethodId == 0)
                option = shipping;

            return Json(new { selectShipping = option });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveShipping(int id, string country, string city, string district, string region, string zip,
            string street, string house, string structure, string apartment, string entrance, string floor, BaseShippingOption shipping)
        {
            var lead = LeadService.GetLead(id);
            if (lead == null || shipping == null)
                return JsonError();
            
            BaseShippingOption option;

            if (shipping.IsCustom == false)
            {
                var model = new GetLeadShippings(id, country, city, district, region, zip, street, house, structure, apartment, entrance, floor, shipping, false).Execute();

                option = model.Shippings != null ? model.Shippings.FirstOrDefault(x => x.Id == shipping.Id) : null;

                if (option == null)
                    return JsonError("Выбранный метод не найден");

                option.UpdateFromBase(shipping);
                
                option.ManualRate = shipping.ManualRate;
            }
            else
            {
                option = shipping;
            }

            lead.ShippingMethodId = option.MethodId;
            lead.ShippingName = option.Name ?? option.NameRate;
            lead.ShippingCost = option.ManualRate;
            
            var shippingTax = option.TaxId.HasValue ? Taxes.TaxService.GetTax(option.TaxId.Value) : null;
            lead.ShippingTaxType = shippingTax?.TaxType ?? Taxes.TaxType.None;
            lead.ShippingPaymentMethodType = option.PaymentMethodType;
            lead.ShippingPaymentSubjectType = option.PaymentSubjectType;

            var pickPoint = option.GetOrderPickPoint();
            lead.ShippingPickPoint = pickPoint != null ? JsonConvert.SerializeObject(pickPoint) : null;

            lead.AvailablePaymentCashOnDelivery = option.IsAvailablePaymentCashOnDelivery;
            lead.AvailablePaymentPickPoint = option.IsAvailablePaymentPickPoint;
                
            if (option.DateOfDelivery != null)
                lead.DeliveryDate = option.DateOfDelivery;
            if (option.TimeOfDelivery.IsNotEmpty())
                lead.DeliveryTime = option.TimeOfDelivery;

            LeadService.UpdateLead(lead, false);

            return JsonOk();
        }

        [HttpGet]
        public JsonResult GetDeliveryTime(int id)
        {
            var lead = LeadService.GetLead(id);
            if (lead == null)
                return JsonError();

            decimal? timeZoneOffset = null;
            if (lead.DeliveryInterval.TimeZoneOffset.HasValue)
                timeZoneOffset = (decimal)lead.DeliveryInterval.TimeZoneOffset.Value.TotalHours;

            return Json(new
            {
                DeliveryDate = lead.DeliveryDate?.ToString("dd.MM.yyyy") ?? string.Empty,
                DeliveryTime = lead.DeliveryTime,
                DeliveryInterval = new
                {
                    TimeFrom = lead.DeliveryInterval.TimeFrom.HasValue ? lead.DeliveryInterval.TimeFrom.Value.ToString(@"hh\:mm") : null,
                    TimeTo = lead.DeliveryInterval.TimeTo.HasValue ? lead.DeliveryInterval.TimeTo.Value.ToString(@"hh\:mm") : null,
                    TimeZoneOffset = timeZoneOffset,
                    ReadableString = lead.DeliveryInterval.ReadableString,
                },
                UseInterval = lead.DeliveryInterval.TimeFrom.HasValue && lead.DeliveryInterval.TimeTo.HasValue
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveDeliveryTime(int id, string deliveryDate, string deliveryTime)
        {
            var lead = LeadService.GetLead(id);
            if (lead == null)
                return JsonError();

            lead.DeliveryDate = !string.IsNullOrWhiteSpace(deliveryDate) ? deliveryDate.TryParseDateTime() : default(DateTime?);
            lead.DeliveryTime = deliveryTime;

            LeadService.UpdateLead(lead, false);

            return JsonOk();
        }

        [HttpGet]
        public JsonResult GetShippingCity(int leadId)
        {
            var lead = LeadService.GetLead(leadId);
            if (lead == null)
                return JsonError();

            if (!string.IsNullOrEmpty(lead.Country) || !string.IsNullOrEmpty(lead.Region) || !string.IsNullOrEmpty(lead.City))
                return Json(new
                {
                    Country = lead.Country,
                    Region = lead.Region,
                    District = lead.District,
                    City = lead.City,
                    Street = "",
                    Zip = lead.Zip
                });

            var customer = lead.Customer;
            if (customer != null)
            {
                var contact = customer.Contacts.FirstOrDefault();
                if (contact != null)
                    return Json(new
                    {
                        Country = contact.Country,
                        Region = contact.Region,
                        District = contact.District,
                        City = contact.City,
                        Street = contact.Street,
                        Zip = contact.Zip
                    });
            }

            var country = CountryService.GetCountry(SettingsMain.SellerCountryId);
            var region = RegionService.GetRegion(SettingsMain.SellerRegionId);
            var city = CityService.GetCityByName(SettingsMain.City ?? "");

            return Json(new
            {
                Country = country != null ? country.Name : "",
                Region = region != null ? region.Name : "",
                District = city != null ? city.District : "",
                City = SettingsMain.City,
                Street = "",
                Zip = city != null ? city.Zip : ""
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveShippingCity(int leadId, string country, string region, string district, string city, string street, string zip)
        {
            var lead = LeadService.GetLead(leadId);
            var isContactsEmpty = false;
            if (lead == null)
                return JsonError();

            if (lead.Customer == null)
                lead.Customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                {
                    FirstName = lead.FirstName ?? "",
                    LastName = lead.LastName ?? "",
                    Patronymic = lead.Patronymic ?? "",
                    EMail = lead.Email ?? "",
                    Phone = lead.Phone ?? ""
                };

            if (lead.Customer.Contacts == null)
                lead.Customer.Contacts = new List<CustomerContact>();

            if (lead.Customer.Contacts.Count == 0)
            {
                isContactsEmpty = true;
                lead.Customer.Contacts.Add(new CustomerContact() { CustomerGuid = lead.Customer.Id });
            }

            lead.Country = country.DefaultOrEmpty();
            lead.Region =  region.DefaultOrEmpty();
            lead.District =  district.DefaultOrEmpty();
            lead.City = city.DefaultOrEmpty();
            lead.Zip = zip.DefaultOrEmpty();

            if (isContactsEmpty)
            {
                var contact = lead.Customer.Contacts[0];
                contact.Country = country.DefaultOrEmpty();
                contact.Region = region.DefaultOrEmpty();
                contact.District = district.DefaultOrEmpty();
                contact.City = city.DefaultOrEmpty();
                contact.Zip = zip.DefaultOrEmpty();
                contact.Street = street.DefaultOrEmpty();                
            }
            
            LeadService.UpdateLead(lead);

            return JsonOk(new { lead.Country, lead.Region, lead.District, lead.City, Street = street, lead.Zip });
        }

        #endregion

        [ChildActionOnly]
        public ActionResult CrmNavMenu(string selected)
        {
            var model = new CrmNavMenuModel()
            {
                Selected = selected,
                SaleFunnels = SalesFunnelService.GetList().Where(SalesFunnelService.CheckAccess).ToList()
            };
            return PartialView(model);
        }

        public ActionResult SalesFunnelsMenu(int? excludeLeadListId)
        {
            var customer = CustomerContext.CurrentCustomer;

            var model = new CrmNavMenuModel()
            {
                ExcludeLeadListId = excludeLeadListId,
                SaleFunnels = SalesFunnelService.GetList().Where(x => x.Enable && x.Id != excludeLeadListId).Where(SalesFunnelService.CheckAccess).ToList(),
                IsFullAccess = customer.IsAdmin || (customer.IsModerator &&
                                                    (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.All || customer.HasRoleAction(RoleAction.Settings)))
            };
            return PartialView(model);
        }

        public IHtmlString SalesFunnelsMenuDirective()
        {
            return new HtmlString("<ul role=\"menu\" class=\"dropdown-menu submenu\" data-submenu><li leads-list></li></ul>");
        }

        #region Import to crm

        public ActionResult ImportLeads()
        {

            SetMetaInformation("Импорт лидов");
            SetNgController(NgControllers.NgControllersTypes.ImportCtrl);

            var model = new GetImportCustomersModel().Execute();

            return View(model);
        }

        public ActionResult Import(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            SetMetaInformation("Импорт лидов");

            switch (id.ToLower())
            {
                case "amocrm":
                    return View("Import/AmoCrm");
            }

            return View("Import/AmoCrm");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ImportAmoCrm(HttpPostedFileBase file)
        {
            try
            {
                var result = new ImportAmoCrm().Execute();
                if (result)
                    ShowMessage(NotifyType.Success, "Импорт успешно закончен");
                else
                    ShowMessage(NotifyType.Error, "Ошибка при импорте");
            }
            catch (BlException ex)
            {
                ShowMessage(NotifyType.Error, "Ошибка при импорте: " + ex.Message);
            }
            return RedirectToAction("Import", new { id = "amocrm" });
        }

        #endregion

        #region LeadsAnalytics

        public ActionResult LeadsAnalyticsPartial(int leadsListId)
        {
            var model = new GetLeadsListAnalytics(leadsListId).Execute();
            return PartialView("~/Areas//Admin/Views/Leads/Analytics/_LeadsAnalytics.cshtml", model);
        }
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult LeadsAnalyticsChartPartial(int leadsListId, DateTime? dateFrom, DateTime? dateTo)
        {
            var model = new GetLeadsGraph(leadsListId, dateFrom, dateTo).Execute();
            return JsonOk(model);
        }

        public JsonResult GetLeadsListSources(int leadsListId)
        {
            var model = new GetLeadsListSources(leadsListId).Execute();
            return Json(new { DataItems = model });
        }

        #endregion

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

    }

    [Auth(RoleAction.Crm, RoleAction.Customers)]
    public partial class LeadsExtController : BaseAdminController
    {
        #region Attachments

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadAttachments(int leadId)
        {
            var lead = LeadService.GetLead(leadId);
            if (lead == null || LeadService.CheckAccess(lead) == false)
                return Json(new UploadAttachmentsResult());

            var handler = new UploadAttachmentsHandler(leadId);
            var result = handler.Execute<LeadAttachment>();
            foreach (var item in result)
            {
                if (item.Result)
                    LeadsHistoryService.AddAttachment(leadId, item.Attachment.OriginFileName, null);
            }
            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteAttachment(int id, int leadId)
        {
            var attachment = AttachmentService.GetAttachment<LeadAttachment>(id);

            var result = AttachmentService.DeleteAttachment<LeadAttachment>(id);
            if (result)
            {
                LeadsHistoryService.RemoveAttachment(leadId, attachment.OriginFileName, null);
            }
            return Json(new { result = result });
        }

        public JsonResult GetAttachments(int leadId)
        {
            return Json(AttachmentService.GetAttachments<LeadAttachment>(leadId)
                .Select(x => new AttachmentModel
                {
                    Id = x.Id,
                    ObjId = x.ObjId,
                    FileName = x.FileName,
                    FilePath = x.Path,
                    FilePathAdmin = x.PathAdmin,
                    FileSize = x.FileSizeFormatted,
                    OriginFileName = x.OriginFileName.IsNullOrEmpty()
                        ? x.FileName
                        : x.OriginFileName
                })
            );
        }

        #endregion
    }
}
