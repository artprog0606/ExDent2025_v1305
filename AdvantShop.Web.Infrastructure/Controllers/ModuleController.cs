using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Modules;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Infrastructure.Controllers
{
    [Warehouse]
    public abstract class ModuleController : BaseController
    {
        protected ActionResult Error404()
        {
            System.Web.HttpContext.Current.Server.TransferRequest("/error/notfound");
            return new EmptyResult();
        }

        protected void ShowErrorMessages()
        {
            foreach (var modelState in ViewData.ModelState.Values)
                foreach (var error in modelState.Errors)
                    ShowMessage(NotifyType.Error, error.ErrorMessage);
        }
    }

    [AdminAuth]
    public abstract class ModuleAdminController : ModuleController
    {
        public string GetModuleVersion(string moduleId)
        {
            var version = "";

            var module = ModulesService.GetModules().Items.FirstOrDefault(x => x.StringId == moduleId);
            if (module != null)
                version = module.CurrentVersion;

            if (version.TryParseFloat() == 0)
                version = "rnd" + new Random().Next(1000);

            return version;
        }
    }
}