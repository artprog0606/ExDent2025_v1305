using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Marketing.Certificates;
using AdvantShop.Web.Admin.Handlers.Marketing.DiscountsPriceRanges;
using AdvantShop.Web.Admin.Models.Marketing.DiscountsByTime;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Controllers.Marketing
{
    [Auth(RoleAction.CouponsAndDiscounts)]
    public partial class DiscountsByTimeController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.DiscountsByTime.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.DiscountsPriceRangeCtrl);

            return View();
        }

        public JsonResult GetItems(DiscountsByTimeFilterModel model)
        {
            return Json(new GetDiscountsByTime(model).Execute());
        }

        #region Commands

        private void Command(DiscountsByTimeFilterModel model, Func<int, DiscountsByTimeFilterModel, bool> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                {
                    func(id, model);
                }
            }
            else
            {
                var handler = new GetDiscountsByTime(model);
                var ids = handler.GetItemsIds<int>();

                foreach (int id in ids)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteItems(DiscountsByTimeFilterModel model)
        {
            Command(model, (id, c) =>
            {
                DiscountByTimeService.Delete(id);
                return true;
            });
            return Json(true);
        }

        #endregion

        #region CRUD

        [HttpGet]
        public JsonResult GetFormData()
        {
            return JsonOk(new 
            {
                GMT = $"GMT {DateTime.Now:zzz}",
                DaysOfWeek = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = T("Admin.DiscountsByTime.Monday"),
                        Value = ((int)DayOfWeek.Monday).ToString()
                    },
                    new SelectListItem
                    {
                        Text = T("Admin.DiscountsByTime.Tuesday"),
                        Value = ((int)DayOfWeek.Tuesday).ToString()
                    },
                    new SelectListItem
                    {
                        Text = T("Admin.DiscountsByTime.Wednesday"),
                        Value = ((int)DayOfWeek.Wednesday).ToString()
                    },
                    new SelectListItem
                    {
                        Text = T("Admin.DiscountsByTime.Thursday"),
                        Value = ((int)DayOfWeek.Thursday).ToString()
                    },
                    new SelectListItem
                    {
                        Text = T("Admin.DiscountsByTime.Friday"),
                        Value = ((int)DayOfWeek.Friday).ToString()
                    },
                    new SelectListItem
                    {
                        Text = T("Admin.DiscountsByTime.Saturday"),
                        Value = ((int)DayOfWeek.Saturday).ToString()
                    },
                    new SelectListItem
                    {
                        Text = T("Admin.DiscountsByTime.Sunday"),
                        Value = ((int)DayOfWeek.Sunday).ToString()
                    }
                }
            });
        }

        [HttpGet]
        public JsonResult Get(int id)
        {
            var discountByTime = DiscountByTimeService.Get(id);
            if (discountByTime == null)
                return JsonError(T("Admin.DiscountsByTime.DiscountNotFound"));
            var discountCategories = DiscountByTimeService.GetDiscountCategories(id);
            return JsonOk(new DiscountsByTimeModel
            {
                Id = id,
                Enabled = discountByTime.Enabled,
                DateFrom = discountByTime.TimeFrom.ToString("hh\\:mm"),
                DateTo = discountByTime.TimeTo.ToString("hh\\:mm"),
                Percent = discountByTime.Discount,
                PopupText = discountByTime.PopupText,
                ShowPopup = discountByTime.ShowPopup,
                SelectedDays = discountByTime.DaysOfWeek,
                SortOrder = discountByTime.SortOrder,
                DiscountCategories = discountCategories.Where(x => x.ApplyDiscount).Select(x => x.CategoryId).ToList(),
                ActiveByTimeCategories = discountCategories.Where(x => x.ActiveByTime).Select(x => x.CategoryId).ToList()
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(DiscountsByTimeGridModel model)
        {
            var discountByTime = DiscountByTimeService.Get(model.Id);
            if (discountByTime == null)
                return JsonError("Не существующая скидка");
            //если изменили активность, то обновляем через handler, чтобы обновить/удалить задачи и изменить активность у категорий, также как при обновлении через модалку
            if (model.Enabled != discountByTime.Enabled)
            {
                var discountsByTimeModel = new DiscountsByTimeModel
                {
                    Percent = model.Discount,
                    SortOrder = model.SortOrder,
                    Enabled = model.Enabled,
                    Id = discountByTime.Id,
                    DateFrom = discountByTime.TimeFrom.ToString(),
                    DateTo = discountByTime.TimeTo.ToString(),
                    ActiveByTimeCategories = discountByTime.ActiveByTimeCategories.Select(x => x.CategoryId).ToList(),
                    DiscountCategories = discountByTime.ApplyDiscountCategories.Select(x => x.CategoryId).ToList(),
                    PopupText = discountByTime.PopupText,
                    SelectedDays = discountByTime.DaysOfWeek,
                    ShowPopup = discountByTime.ShowPopup
                };
                return ProcessJsonResult(new AddEditDiscountByTimeHandler(discountsByTimeModel));
            }
            discountByTime.SortOrder = model.SortOrder;
            discountByTime.Discount = model.Discount;
            DiscountByTimeService.Update(discountByTime);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            DiscountByTimeService.Delete(id);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddEdit(DiscountsByTimeModel model) => ProcessJsonResult(new AddEditDiscountByTimeHandler(model));

        #endregion
    }
}
