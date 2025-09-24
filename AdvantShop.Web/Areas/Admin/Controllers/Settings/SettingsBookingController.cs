using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Booking;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Admin.Models.Settings.BookingTags;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Booking)]
    public partial class SettingsBookingController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Settings.Booking.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsBookingCtrl);

            var model = new GetBookingSettingsHandler().Execute();
            return View("index", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(BookingSettingsModel model)
        {
            if (ModelState.IsValid)
            {
                new SaveBookingSettingsHandler(model).Execute();
                ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
            }
            else
            {
                foreach (var modelState in ViewData.ModelState.Values)
                    foreach (var error in modelState.Errors)
                    {
                        ShowMessage(NotifyType.Error, error.ErrorMessage);
                    }
            }

            var isMobile = SettingsDesign.IsMobileTemplate;

            var referrerParams = new System.Web.Routing.RouteValueDictionary();

            if (HttpContext.Request.UrlReferrer != null)
            {
                var nv = HttpUtility.ParseQueryString(HttpContext.Request.UrlReferrer.Query); //Преобразует урлхвост в NameValueCollection

                foreach (var nvKey in nv.AllKeys)
                {
                    if (referrerParams.ContainsKey(nvKey) is false)
                        referrerParams.Add(nvKey, nv.Get(nvKey));
                }
            }

            if (isMobile)
            {
                return RedirectToAction("Index", referrerParams);
            }
            else
            {
                return Index();
            }
        }

        [HttpPost]
        public JsonResult IsBookingActive()
        {
            return Json(Configuration.SettingsMain.BookingActive);
        }

        #region Tags

        public JsonResult GetTags(TagsFilterModel model)
        {
            var handler = new GetTagsHandler(model);
            var result = handler.Execute();

            return Json(result);
        }

        #region Inplace Tag

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceTag(TagModel model)
        {
            var dbModel = TagService.Get(model.Id);
            dbModel.Name = model.Name;
            dbModel.SortOrder = model.SortOrder;
            TagService.Update(dbModel);

            return Json(new { result = true });
        }

        #endregion

        #region Commands

        private void Command(TagsFilterModel command, Func<int, TagsFilterModel, bool> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                {
                    func(id, command);
                }
            }
            else
            {
                var handler = new GetTagsHandler(command);
                var ids = handler.GetItemsIds("Id");

                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteTags(TagsFilterModel command)
        {
            Command(command, (id, c) =>
            {
                TagService.Delete(id);
                return true;
            });
            return Json(true);
        }

        #endregion

        #region CRUD

        public JsonResult GetTag(int id)
        {
            var dbModel = TagService.Get(id);
            if (dbModel == null)
                return Json(new { result = false });

            var result = new
            {
                Id= dbModel.Id,
                Name= dbModel.Name,
                SortOrder = dbModel.SortOrder
            };

            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddTag(TagModel model)
        {
            if (model.Name.IsNullOrEmpty())
                return Json(new { result = false });

            var dbModel = new Tag
            {
                Name = model.Name.DefaultOrEmpty().Trim(),
                SortOrder = model.SortOrder,
            };
            TagService.Add(dbModel);

            return Json(new { result = true });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateTag(TagModel model)
        {
            if (model.Name.IsNullOrEmpty())
                return Json(new { result = false });

            var dbModel = TagService.Get(model.Id);
            if (dbModel == null)
                return Json(new { result = false });

            dbModel.Name = model.Name.DefaultOrEmpty();
            dbModel.SortOrder = model.SortOrder;

            TagService.Update(dbModel);

            return Json(new { result = true });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteTag(int id)
        {
            TagService.Delete(id);
            return Json(new { result = true });
        }

        #endregion

        #endregion

    }
}
