using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Leads;
using AdvantShop.Web.Admin.Handlers.Shared.AdminComments;
using AdvantShop.Web.Admin.Models.Crm.Leads;
using AdvantShop.Web.Admin.Models.Shared.AdminComments;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Crm
{
    [Auth(RoleAction.Crm, RoleAction.Customers, RoleAction.Booking)]
    public partial class CrmEventsController : BaseAdminController
    {
        public JsonResult GetEvents(int? objId, string objType, Guid? customerId)
        {
            return Json(new GetCrmEvents(objId, objType, customerId).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddEvent(Guid? customerId, int? objId, string objType, LeadEventType? type, string message, string pageType)
        {
            if (string.IsNullOrWhiteSpace(message))
                return JsonError();

            var model = new AdminCommentModel() { Text = message };

            if (objId != null)
            {
                if (objType == "lead")
                {
                    var lead = LeadService.GetLead(objId.Value);
                    if (lead == null)
                        return JsonError();

                    model.ObjId = objId.Value;
                    model.Type = AdminCommentType.Lead;

                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_AddComment_Events);
                }
                else if (objType == "booking")
                {
                    var booking = BookingService.Get(objId.Value);
                    if (booking == null)
                        return JsonError("Бронь не найдена");
                    if (!BookingService.CheckAccessToEditing(booking))
                        return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));

                    model.ObjId = objId.Value;
                    model.Type = AdminCommentType.Booking;
                }
                else if (objType == "order")
                {
                    var o = OrderService.GetOrder(objId.Value);
                    if (o == null)
                        return JsonError("Заказ не найден");

                    model.ObjId = objId.Value;
                    model.Type = AdminCommentType.Order;
                }
            }
            else
            {
                var customer = customerId != null ? CustomerService.GetCustomer(customerId.Value) : null;
                if (customer == null)
                    return JsonError();

                model.ObjId = customer.InnerId;
                model.Type = AdminCommentType.Customer;

                if (pageType == "order")
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_AddComment_Events);
                else if (pageType == "customer")
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_AddComment_Events);
            }

            var result = new AddAdminCommentHandler(model).Execute();

            return result.Result ? JsonOk() : JsonError();
        }

        public JsonResult GetEmail(string id, string folder)
        {
            return Json(CustomerService.GetEmailImap(id, folder));
        }

        public JsonResult GetSendedEmail(LeadEventEmailDataModel model)
        {
            if (model == null || model.CustomerId == Guid.Empty || string.IsNullOrEmpty(model.CustomerEmail))
                return Json(null);

            var emails = CustomerService.GetEmails(model.CustomerId, model.CustomerEmail);
            if (emails != null)
            {
                var email = emails.FirstOrDefault(x => x.Id == model.Id) ?? 
                            emails.FirstOrDefault(x => x.CreateOn == model.CreateOn);

                if (email != null)
                    return Json(new EmailImap()
                    {
                        Subject = email.Subject,
                        HtmlBody = email.Body,
                        Date = email.CreateOn,
                        From = SettingsMail.From,
                        FromEmail = SettingsMail.From,
                        To = email.EmailAddress,
                    });
            }

            return Json(null);
        }
    }
}
