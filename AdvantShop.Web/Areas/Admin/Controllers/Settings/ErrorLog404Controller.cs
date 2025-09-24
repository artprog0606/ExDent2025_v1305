using System;
using System.Web.Mvc;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Handlers.Settings.System;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    public partial class ErrorLog404Controller : BaseAdminController
    {
        public JsonResult GetPage404(AdminErrorLog404FilterModel model)
        {
            return Json(new GetErrorLog404(model).Execute());
        }

        public JsonResult DeleteItems(AdminErrorLog404FilterModel model)
        {
            Command(model, (id, c) => Error404Service.DeleteError404(id));
            return Json(true);
        }

        #region Command

        private void Command(AdminErrorLog404FilterModel model, Action<int, AdminErrorLog404FilterModel> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                    func(id, model);
            }
            else
            {
                var Id404page = new GetErrorLog404(model).GetItemsIds();
                foreach (int id in Id404page)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        #endregion
    }
}
