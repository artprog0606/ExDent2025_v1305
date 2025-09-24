using System;
using System.Web.Mvc;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.ShippingReplaceGeo;
using AdvantShop.Web.Admin.Models.Settings.ShippingReplaceGeo;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public class ShippingReplaceGeoController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation("Правила замены адреса доставки для доставок");
            SetNgController(NgControllers.NgControllersTypes.ShippingReplaceGeoCtrl);

            return View();
        }

        public JsonResult GetListShippingReplaceGeo(ShippingReplaceGeoFilterModel filterModel)
        {
            return Json(new GetListShippingReplaceGeoHandler(filterModel).Execute());
        }

        #region Inplace

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult InplaceShippingReplaceGeo(ShippingReplaceGeoModel model)
        {
            if (ModelState.IsValid)
            {
                var replaceItem = Shipping.ShippingReplaceGeoService.Get(model.Id);
                if (replaceItem == null)
                    return JsonError();

                replaceItem.Enabled = model.Enabled;
                replaceItem.Sort = model.Sort;

                Shipping.ShippingReplaceGeoService.Update(replaceItem);

                return JsonOk();
            }

            return JsonError();
        }

        #endregion

        #region Commands

        private void Command(ShippingReplaceGeoFilterModel command, Action<int, ShippingReplaceGeoFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetListShippingReplaceGeoHandler(command).GetItemsIds();
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteShippingReplacesGeo(ShippingReplaceGeoFilterModel command)
        {
            Command(command, (id, c) => Shipping.ShippingReplaceGeoService.Delete(id));
            return Json(true);
        }

        #endregion

        #region Get Add Update Delete

        public JsonResult Get(int id)
        {
            var replace = Shipping.ShippingReplaceGeoService.Get(id);

            if (replace != null)
                return JsonOk(ShippingReplaceGeoModel.CreateFromShippingReplaceGeo(replace));

            return JsonError("Указанная замена не найдена");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(ShippingReplaceGeoModel model)
        {
            if (ModelState.IsValid)
            {
                var handler = new AddUpdateShippingReplaceGeoHandler(model);
                if (handler.Execute())
                    return JsonOk();

                return JsonError(handler.Errors.ToArray());
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Update(ShippingReplaceGeoModel model)
        {
            if (ModelState.IsValid)
            {
                if (Shipping.ShippingReplaceGeoService.Get(model.Id) == null)
                    return JsonError("Указанная замена не найдена");

                var handler = new AddUpdateShippingReplaceGeoHandler(model);
                if (handler.Execute())
                    return JsonOk();

                return JsonError(handler.Errors.ToArray());
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            Shipping.ShippingReplaceGeoService.Delete(id);
            return JsonOk();
        }

        #endregion
    }
}
