using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.TemplatesDocx;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Payment;
using AdvantShop.Web.Admin.Handlers.Attachments;
using AdvantShop.Web.Admin.Handlers.Booking.Journal;
using AdvantShop.Web.Admin.Handlers.Booking.NavMenu;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Attachments;
using AdvantShop.Web.Admin.Models.Booking.Journal;
using AdvantShop.Web.Admin.Models.Settings.TemplatesDocxSettings;
using AdvantShop.Web.Admin.Models.Tasks;
using AdvantShop.Web.Admin.ViewModels.Booking;
using AdvantShop.Web.Admin.ViewModels.Booking.Journal;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Booking
{
    public partial class BookingController : BaseBookingController
    {
        public ActionResult Index(int? id)
        {
            var isMobile = SettingsDesign.IsMobileTemplate;

            if (id.HasValue && !SelectAffiliate(id.Value))
                return RedirectToAction("Index", "Booking", new {id = (int?)null});

            if (SelectedAffiliate != null)
                SetMetaInformation(SelectedAffiliate.Name);

            SetNgController(NgControllers.NgControllersTypes.BookingJournalCtrl);

            var currentCustomer = CustomerContext.CurrentCustomer;
            Manager currentManager = currentCustomer.IsManager ? ManagerService.GetManager(currentCustomer.Id) : null;

            var model = new JournalModel()
            {
                SelectedAffiliate = SelectedAffiliate,
                AccessToEditing = SelectedAffiliate != null && AffiliateService.CheckAccessToEditing(SelectedAffiliate, currentManager),
                AccessToViewBooking = SelectedAffiliate != null && (AffiliateService.CheckAccess(SelectedAffiliate, currentManager, false) || SelectedAffiliate.AccessToViewBookingForResourceManagers),
            };

            var cookiesValueViewMode = Helpers.CommonHelper.GetCookieString("bookingjournal_viewmode");

            var enumValues = Enum.GetValues(typeof (JournalViewMode)).Cast<JournalViewMode>().ToList();

            if(isMobile)
            {
                Helpers.CommonHelper.SetCookie("bookingjournal_viewmode", "sheduler-short");
            }

            model.ViewMode = 
                enumValues.Any(view => model.GetViewModeValue(view) == cookiesValueViewMode)
                ? enumValues.First(view => model.GetViewModeValue(view) == cookiesValueViewMode)
                : JournalViewMode.ShedulerCompact;


            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Booking_ViewBookingList);



            return View(model);
        }

        [ChildActionOnly]
        public ActionResult AffiliatesMenu()
        {
            var model = new GetAffiliatesMenuHandler(SelectedAffiliate).Execute();

            return PartialView("_AffiliatesMenu", model);
        }

        public ActionResult SubsidiaryListMenu()
        {
            var model = new GetAffiliatesMenuHandler(SelectedAffiliate).Execute();

            return PartialView("_SubsidiaryListMenu", model);
        }

        [ChildActionOnly]
        public ActionResult NavMenu()
        {
            var model = new GetNavMenuHandler(SelectedAffiliate).Execute();

            return PartialView("_NavMenu", model);
        }

        [ChildActionOnly]
        public ActionResult MenuJson(bool isOpen)
        {
            var model = new GetMenuJsonHandler(SelectedAffiliate,  isOpen: isOpen).Execute();

            if (!model.IsOpen)
            {
                var currentAction = Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
                var currentController = Request.RequestContext.RouteData.Values["controller"].ToString().ToLower();

                var items = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("index", "booking"),
                    new Tuple<string, string>("index", "bookingcategory"),
                    new Tuple<string, string>("index", "bookingresources"),
                    new Tuple<string, string>("index", "bookinganalytics"),
                    new Tuple<string, string>("settings", "bookingaffiliate"),
                    new Tuple<string, string>("index", "settingsbooking")
                };

                if (items.Any(x => x.Item1 == currentAction && x.Item2 == currentController))
                    model.IsOpen = true;
            }

            return PartialView("_MenuJson", model);
        }

        #region Add Edit Delete

        public JsonResult Get(int id)
        {
            var handler = new GetBookingHandler(id);
            var model = handler.Execute();

            if (model != null)
            {
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Booking_ViewBooking);
                return JsonOk(model);
            }

            return JsonError(handler.Errors.ToArray());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(AddUpdateBookingModel model)
        {
            if (ModelState.IsValid)
            {
                var handler = new AddUpdateBookingHandler(model);
                if (handler.Execute())
                    return JsonOk();

                if (handler.UserConfirmIsRequired)
                    return JsonOk(new
                    {
                        UserConfirmIsRequired = true,
                        ConfirmButtomText = handler.ConfirmButtomText,
                        ConfirmMessage = handler.ConfirmMessage
                    });

                return JsonError(handler.Errors.ToArray());
            }

            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Update(AddUpdateBookingModel model)
        {
            if (ModelState.IsValid)
            {
                var handler = new AddUpdateBookingHandler(model);
                if (handler.Execute())
                    return JsonOk();

                if (handler.UserConfirmIsRequired)
                    return JsonOk(new
                    {
                        UserConfirmIsRequired = true,
                        ConfirmButtomText = handler.ConfirmButtomText,
                        ConfirmMessage = handler.ConfirmMessage
                    });

                return JsonError(handler.Errors.ToArray());
            }

            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var booking = BookingService.Get(id);
            booking.IsFromAdminArea = true;

            if (BookingService.CheckAccessToEditing(booking))
            {
                BookingService.Delete(booking);
                BizProcessExecuter.BookingDeleted(booking);
                return JsonOk();
            }
            else
                return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeStatus(int id, BookingStatus status)
        {
            var booking = BookingService.Get(id);

            if (booking != null)
            {
                if (BookingService.CheckAccessToEditing(booking))
                {
                    BookingService.ChangeStatus(id, status);

                    BizProcessExecuter.BookingChanged(booking);
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Booking_BookingStatusChanged);
                    return JsonOk();
                }
                else
                    return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
            }
            else
                return JsonError("Бронь не найдена");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateOrder(int id)
        {
            var booking = BookingService.Get(id);
            if (booking == null || BookingService.CheckAccessToEditing(booking) == false)
                return Json(new { result = false });

            var order = AdvantShop.Orders.OrderService.CreateOrder(booking);
            if (order == null)
                return Json(new { result = false });

            return Json(new { result = true, orderId = order.OrderID, code = order.Code });
        }

        #endregion

        #region Modal

        public JsonResult GetBookingFormData(GetBookingFormModel model)
        {
            var handler = new GetBookingFormDataHandler(model);
            var data = handler.Execute();

            return data != null
                ? JsonOk(data)
                : JsonError(handler.Errors.ToArray());
        }

        public JsonResult GetAttachmentHelpText()
        {
            return JsonOk(new { AttachmentHelpText = FileHelpers.GetFilesHelpText(AttachmentService.FileTypes[AttachmentType.Booking], "15MB")});
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddUpdateBookingItems(AddUpdateBookingItemsModel model)
        {
            var handler = new AddUpdateBookingItemsHandler(model);
            var newItems = handler.Execute();
            if (newItems != null)
                return JsonOk(new
                {
                    NewItems = newItems
                });

            return JsonError(handler.Errors.ToArray());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateSummary(UpdateSummaryModel model)
        {
            var handler = new UpdateSummaryHandler(model);
            return JsonOk(new
            {
                NewSummary = handler.Execute()
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangePayment(ChangePaymentModel model)
        {
            var handler = new ChangePaymentHandler(model);
            return JsonOk(new
            {
                NewSummary = handler.Execute()
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetPayments(GetPayments model)
        {
            var handler = new GetPaymentsHandler(model);
            return JsonOk(new
            {
                payments = handler.Execute()
            });
        }

        public JsonResult GetPaymentMethods()
        {
            var methods = PaymentService.GetAllPaymentMethods(false);
            var listMethods =
                methods.Select(method => new SelectItemModel(method.Name, method.PaymentMethodId.ToString())).ToList();
            listMethods.Insert(0, new SelectItemModel("Без метода оплаты", "0"));

            return Json(listMethods);
        }

        //public JsonResult GetCustomerFields(Guid? customerId)
        //{
        //    var id = customerId != null ? customerId.Value : Guid.Empty;
        //    var fields = CustomerFieldService.GetCustomerFieldsWithValue(id) ?? new List<CustomerFieldWithValue>();
        //    return Json(fields);
        //}

        public JsonResult GetCustomerSocial(Guid? customerId)
        {
            if (customerId.HasValue)
            {
                var handler = new GetCustomerSocialHandler(customerId.Value);
                return Json(handler.Execute());
            }

            return null;
        }

        public JsonResult GetMonthFreeDays(GetMonthFreeDaysModel model)
        {
            var handler = new GetMonthFreeDaysHandler(model);
            var data = handler.Execute();

            return data != null
                ? JsonOk(new
                {
                    PrevMonth = data.PrevMonth,
                    CurrentMonth = data.CurrentMonth,
                    NextMonth = data.NextMonth
                })
                : JsonError(handler.Errors.ToArray());
        }

        public JsonResult GetBookingCustomerFormData()
        {
            return Json(new
            {
                customerTypes = Enum.GetValues(typeof(CustomerType)).Cast<CustomerType>().Where(x => x != CustomerType.All).Select(x => (object)new
                {
                    label = x.Localize(),
                    value = x.ToString(),
                }),
                isRegistrationAsLegalEntity = SettingsCustomers.IsRegistrationAsLegalEntity,
                isRegistrationAsPhysicalEntity = SettingsCustomers.IsRegistrationAsPhysicalEntity
            });
        }

        #endregion

        #region Grid

        public JsonResult GetList(BookingsFilterModel filter)
        {
            return Json(new GetBookingsHandler(filter).Execute());
        }

        public JsonResult GetListForCustomer(BookingsFilterModel filter)
        {
            return Json(new GetBookingsHandler(filter, showByAccess: false).Execute());
        }

        private void Command(BookingsFilterModel model, Action<int, BookingsFilterModel> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                    func(id, model);
            }
            else
            {
                var ids = new GetBookingsHandler(model).GetItemsIds();
                foreach (int id in ids)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteBookings(BookingsFilterModel model)
        {
            Command(model, (id, m) =>
            {
                var booking = BookingService.Get(id);
                booking.IsFromAdminArea = true;

                if (BookingService.CheckAccessToEditing(booking))
                {
                    BookingService.Delete(booking);
                    BizProcessExecuter.BookingDeleted(booking);
                }
            });
            return JsonOk();
        }

        #endregion

        #region Journal

        public JsonResult GetJournal(BookingsJournalFilterModel model)
        {
            var handler = new GetBookingsJournalHandler(model);
            var result = handler.Execute();
            return Json(result);
        }

        public JsonResult GetJournalColumn(BookingsJournalFilterModel model)
        {
            var handler = new GetBookingsJournalHandler(model);
            var result = handler.GetEvents();
            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateAfterDrag(UpdateAfterDragBookingModel model)
        {
            if (ModelState.IsValid)
            {
                var handler = new UpdateAfterDragBookingHandler(model);
                if (handler.Execute())
                    return JsonOk();

                if (handler.UserConfirmIsRequired)
                    return JsonOk(new
                    {
                        UserConfirmIsRequired = true,
                        ConfirmButtomText = handler.ConfirmButtomText,
                        ConfirmMessage = handler.ConfirmMessage
                    });

                return JsonError(handler.Errors.ToArray());
            }

            return JsonError();
        }

        #endregion

        #region JournalDays

        public JsonResult GetJournalDays(BookingsJournalDaysFilterModel model)
        {
            var handler = new GetBookingsJournalDaysHandler(model);
            var result = handler.Execute();
            return Json(result);
        }
        #endregion

        #region ReservationResource Journal

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetReservationResourceJournal(BookingsReservationResourceJournalFilterModel model)
        {
            if (ModelState.IsValid)
            {
                var handler = new GetBookingJournalReservationResourceHandler(model);
                var result = handler.Execute();
                return result != null
                    ? JsonOk(new
                    {
                        MinTime = result.MinTime,
                        MaxTime = result.MaxTime,
                        Events = result.Events
                    })
                    : JsonError(handler.Errors.ToArray());
            }

            return JsonError();
        }


        #endregion

        #region Attachments

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadAttachments(int bookingId)
        {
            var booking = BookingService.Get(bookingId);
            if (booking == null)
                return Json(new[] { new UploadAttachmentsResult() {Error = "Бронь не найдена" }});
            if (!BookingService.CheckAccessToEditing(booking))
                return Json(new[] { new UploadAttachmentsResult() {Error = LocalizationService.GetResource("Admin.Booking.NoAccess") }});

            var handler = new UploadAttachmentsHandler(bookingId);
            var result = handler.Execute<BookingAttachment>();

            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteAttachment(int id, int bookingId)
        {
            var booking = BookingService.Get(bookingId);
            if (booking == null)
                return JsonError("Бронь не найдена");
            if (!BookingService.CheckAccessToEditing(booking))
                return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));

            var result = AttachmentService.DeleteAttachment<BookingAttachment>(id);
            return Json(new { result = result });
        }

        public JsonResult GetAttachments(int bookingId)
        {
            var booking = BookingService.Get(bookingId);
            if (booking == null || BookingService.CheckAccess(booking) == false)
                return Json(null);

            return Json(AttachmentService.GetAttachments<BookingAttachment>(bookingId)
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

        #region TemplatesDocx

        public JsonResult GetTemplatesByType()
        {
            return JsonOk(new
            {
                Templates = TemplatesDocxServices.GetList<BookingTemplateDocx>().Select(x => (TemplatesDocxModel)x)
            });
        }

        public ActionResult GenerateTemplates(GenerateTemplatesDocxModel model)
        {
            var handler = new GenerateTemplatesDocx(model);
            var result = handler.Execute();

            if (model.Attach)
                return result != null ? Json(result) : JsonError(handler.Errors.ToArray());
            else
            {
                if (result != null)
                {
                    var resultData = (Tuple<string, string>) result;
                    return FileDeleteOnUpload(resultData.Item1, "application/octet-stream", Path.GetFileName(resultData.Item1), () => Helpers.FileHelpers.DeleteDirectory(resultData.Item2));
                }

                return JsonError(handler.Errors.ToArray());
            }
        }

        #endregion

        #region Statuses 

        public JsonResult GetStatusesList()
        {
            return Json(
                Enum.GetValues(typeof (BookingStatus))
                    .Cast<BookingStatus>()
                    .Select(x => new SelectItemModel(x.Localize(), (int) x)));
        }

        #endregion
    }
}
