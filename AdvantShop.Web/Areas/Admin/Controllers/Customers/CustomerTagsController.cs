using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Web.Admin.Handlers.Customers.CustomerTags;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Customers.CustomerTags;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;

namespace AdvantShop.Web.Admin.Controllers.Customers
{
    [Auth(RoleAction.Customers)]
    public class CustomerTagsController : BaseAdminController
    {
        public JsonResult GetTags(CustomerTagsFilterModel model)
        {
            return Json(new GetCustomerTagsHandler(model).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceTag(CustomerTagsModel model)
        {
            var tag = TagService.Get(model.Id);
            if (tag == null)
                return JsonError();

            tag.Enabled = model.Enabled;
            tag.SortOrder = model.SortOrder;

            TagService.Update(tag);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteTag(int id)
        {
            TagService.Delete(id);
            return Json(true);
        }

        private void Command(CustomerTagsFilterModel command, Action<int, CustomerTagsFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetCustomerTagsHandler(command).GetItemsIds("Id");
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteTags(CustomerTagsFilterModel model)
        {
            Command(model, (id, c) => TagService.Delete(id));
            return Json(true);
        }

        public ActionResult Add()
        {
            var model = new CustomerTagModel()
            {
                IsEditMode = false,
                Enabled = true,
                SortOrder = 0
            };

            SetMetaInformation(T("Admin.CustomerTags.AddEdit.Title"));
            SetNgController(NgControllers.NgControllersTypes.CustomerTagsCtrl);

            return View("AddEdit", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(CustomerTagModel model)
        {
            if (ModelState.IsValid)
            {
                var tagId = new AddUpdateCustomerTagHandler(model).Execute();

                if(tagId != 0)
                {
                    return JsonOk(T("Admin.ChangesSuccessfullySaved"));
                }
            }

            return JsonError("Ошибка при добавлении тега");
        }

        public ActionResult Edit(int id)
        {
            var tag = TagService.Get(id);
            if (tag == null)
                return RedirectToAction("Index", "SettingsCustomers", new { tab = "customerSegments" });

            var model = new CustomerTagModel()
            {
                IsEditMode = true,

                Id = tag.Id,
                Name = tag.Name,
                Enabled = tag.Enabled,
                SortOrder = tag.SortOrder
            };


            SetMetaInformation(T("Admin.CustomerTags.AddEdit.Title") + " - " + tag.Name);
            SetNgController(NgControllers.NgControllersTypes.CustomerTagsCtrl);

            return View("AddEdit", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Edit(CustomerTagModel model)
        {
            if (ModelState.IsValid)
            {
                var tagId = new AddUpdateCustomerTagHandler(model).Execute();

                if(tagId != 0)
                {
                    return JsonOk(T("Admin.ChangesSuccessfullySaved"));
                }
            }

            return JsonError("Ошибка при обновлении тега");
        }

        public JsonResult GetCustomerTagsSelectOptions()
        {
            var tags = TagService.GetAllTags(onlyEnabled: true).Select(x => new SelectItemModel(x.Name, x.Id)).ToList();
            
            tags.Insert(0, new SelectItemModel(T("Admin.CustomerTags.SelectWithoutTags"), -1));
            
            return Json(tags);
        }
    }
}
