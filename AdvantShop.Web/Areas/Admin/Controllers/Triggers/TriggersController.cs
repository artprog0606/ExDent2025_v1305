using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Triggers;
using AdvantShop.Web.Admin.Handlers.Triggers.Category;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Triggers;
using AdvantShop.Web.Admin.Models.Triggers.Category;
using AdvantShop.Web.Admin.ViewModels.Triggers;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Triggers
{
    [Auth(RoleAction.Triggers)]
    [SaasFeature(ESaasProperty.HaveTriggers)]
    [SalesChannel(ESalesChannelType.Triggers)]
    public partial class TriggersController : BaseAdminController
    {
        [ChildActionOnly]
        public ActionResult NavMenu()
        {
            return PartialView();
        }

        #region List

        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Triggers.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.TriggersCtrl);

            var saasData = SaasDataService.CurrentSaasData;

            var model = new TriggersViewModel
            {
                TriggersCount = TriggerRuleService.GetTriggersCount(),
                TriggersLimitation = SaasDataService.IsSaasEnabled,
                TriggersLimit = SaasDataService.IsSaasEnabled ? saasData.TriggersCount : 0,
            };

            model.DisableAddition = SaasDataService.IsSaasEnabled && model.TriggersCount >= model.TriggersLimit;

            if (TriggerRuleService.GetTriggersCount(false) == 0)
                return View("Preview");

            return View(model);
        }

        public JsonResult GetTriggersPlugged(TriggerFilterModel filter)
        {
            return Json(new GetTriggerRules(filter).Execute());
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult InplaceTriggerPlugged(int id, bool? enabled)
        {
            if (enabled.HasValue)
                TriggerRuleService.SetActive(id, enabled.Value);

            return Json(new { result = true });
        }

        private void Command(TriggerFilterModel command, Action<int, TriggerFilterModel> func)
        {
            var exceptions = new ConcurrentQueue<Exception>();

            if (command.SelectMode == SelectModeCommand.None)
            {
                Parallel.ForEach(command.Ids, new ParallelOptions { MaxDegreeOfParallelism = SettingsMain.UseMultiThreads ? 10 : 1 }, (id) =>
                {
                    try
                    {
                        func(id, command);
                    }
                    catch (Exception e)
                    {
                        exceptions.Enqueue(e);
                    }
                });

                //foreach (var id in command.Ids)
                //    func(id, command);
            }
            else
            {
                var ids = new GetTriggerRules(command).GetItemsIds<int>("[TriggerRule].[Id]");

                Parallel.ForEach(ids, new ParallelOptions { MaxDegreeOfParallelism = SettingsMain.UseMultiThreads ? 10 : 1 }, (id) =>
                {
                    try
                    {
                        if (command.Ids == null || !command.Ids.Contains(id))
                            func(id, command);
                    }
                    catch (Exception e)
                    {
                        exceptions.Enqueue(e);
                    }
                });
                //foreach (int id in ids)
                //{
                //    if (command.Ids == null || !command.Ids.Contains(id))
                //        func(id, command);
                //}
            }

            if (exceptions.Any())
            {
                Debug.Log.Error(exceptions.AggregateString("<br/>^^^<br/>"));
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteTriggers(TriggerFilterModel command)
        {
            Command(command, (id, c) => TriggerRuleService.Delete(id));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ActivateTriggers(TriggerFilterModel command)
        {
            Command(command, (id, c) => TriggerRuleService.SetActive(id, true));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DisableTriggers(TriggerFilterModel command)
        {
            Command(command, (id, c) => TriggerRuleService.SetActive(id, false));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteTrigger(int id)
        {
            TriggerRuleService.Delete(id);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeEnabled(int id, bool enabled)
        {
            var trigger = TriggerRuleService.GetTrigger(id);
            if (trigger == null)
                return JsonError("Триггер не существует");

            trigger.Enabled = enabled;
            TriggerRuleService.Update(trigger);

            return JsonOk();
        }

        #endregion

        #region Add | Edit

        public ActionResult Add(TriggerEditModel model)
        {
            SetMetaInformation(T("Admin.Triggers.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.TriggersCtrl);

            return View("AddEdit", model);
        }

        public ActionResult Edit(int id)
        {
            SetMetaInformation(T("Admin.Triggers.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.TriggersCtrl);

            return View("AddEdit", new TriggerEditModel() { Id = id });
        }

        public JsonResult GetTriggerFormData(ETriggerEventType? eventType, ETriggerObjectType[] objectTypes)
        {
            return Json(new GetTriggerFormData(eventType, objectTypes).Execute());
        }

        public JsonResult GetTrigger(int id)
        {
            return Json(new GetTrigger(id).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Save(TriggerModel model)
        {
            return ProcessJsonResult(new SaveTrigger(model));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(TriggerModel model)
        {
            return ProcessJsonResult(new AddTrigger(model));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveName(int id, string name)
        {
            return ProcessJsonResult(new SaveTriggerName(id, name));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CopyTrigger(int id)
        {
            return ProcessJsonResult(new CopyTrigger(id));
        }


        // Filters in modal window

        public JsonResult GetFilterRuleParamValues(ETriggerEventType eventType, int fieldType, int? fieldObjId)
        {
            var values = new GetTriggerRuleParamValues(eventType, fieldType, fieldObjId).Execute();
            return Json(new { values });
        }

        public JsonResult GetFilterRuleFormData(ETriggerEventType eventType)
        {
            var fields = new GetTriggerRuleFields(eventType).Execute();
            return Json(new { fields });
        }

        public JsonResult GetEditActionFormData(ETriggerEventType eventType)
        {
            var fields = new GetTriggerEditFields(eventType).Execute();
            return Json(new { fields });
        }

        public JsonResult GetActionEditFieldValues(ETriggerEventType eventType, int fieldType, int? fieldObjId)
        {
            var values = new GetTriggerRuleParamValues(eventType, fieldType, fieldObjId).Execute();
            return Json(new { values });
        }

        #endregion

        #region Category

        public JsonResult GetCategories(CategoriesFilterModel model)
        {
            return Json(new GetCategoriesHandler(model).Execute());
        }

        public JsonResult GetCategoriesList()
        {
            var list = TriggerCategoryService.GetList().Select(x => new SelectItemModel(x.Name, x.Id.ToString())).ToList();
            list.Insert(0, new SelectItemModel("Общая", "0"));
            return Json(list);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceCategory(CategoryModel model)
        {
            if (ModelState.IsValid && model.Id != 0)
            {
                var category = TriggerCategoryService.Get(model.Id);

                if (category != null)
                {
                    category.Name = model.Name;
                    category.SortOrder = model.SortOrder;

                    TriggerCategoryService.Update(category);

                    return JsonOk();
                }
            }

            return JsonError();
        }

        public JsonResult GetCategory(int id)
        {
            var category = TriggerCategoryService.Get(id);
            if (category == null)
                return JsonError("Указанная категория отсутствует");

            return JsonOk((CategoryModel)category);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddCategory(CategoryModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            var id = TriggerCategoryService.Add(new TriggerCategory()
            {
                Name = model.Name,
                SortOrder = model.SortOrder
            });

            return JsonOk((CategoryModel)TriggerCategoryService.Get(id));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateCategory(CategoryModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            var category = TriggerCategoryService.Get(model.Id);
            if (category == null)
                return JsonError("Категория не найдена");

            category.Name = model.Name;
            category.SortOrder = model.SortOrder;

            TriggerCategoryService.Update(category);

            return JsonOk((CategoryModel)category);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCategory(int id)
        {
            if (TriggerRuleService.HasTriggerRulesOfCategory(id))
                return JsonError("Нельзя удалить категорию, так как категория имеет триггеры");

            TriggerCategoryService.Delete(id);

            return JsonOk();
        }

        #region Commands

        private void Command(CategoriesFilterModel model, Action<int, CategoriesFilterModel> action)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                    action(id, model);
            }
            else
            {
                var handler = new GetCategoriesHandler(model);
                var ids = handler.GetItemsIds("Id");

                foreach (int id in ids)
                    if (model.Ids == null || !model.Ids.Contains(id))
                        action(id, model);
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult DeleteCategories(CategoriesFilterModel command)
        {
            Command(command, (id, c) =>
            {
                if (!TriggerRuleService.HasTriggerRulesOfCategory(id))
                    TriggerCategoryService.Delete(id);
            });

            return Json(true);
        }

        #endregion

        #endregion
    }
}
