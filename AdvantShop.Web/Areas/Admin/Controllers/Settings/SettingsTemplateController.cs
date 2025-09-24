using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Templates;
using AdvantShop.Web.Admin.Models.Settings.Templates;
using AdvantShop.Web.Admin.ViewModels.Settings;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    [SalesChannel(ESalesChannelType.Store)]
    public partial class SettingsTemplateController : BaseAdminController
    {
        public ActionResult Index()
        {
            var model = new GetTemplateSettings().Execute();

            SetMetaInformation(T("Admin.Settings.Template.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsTemplateCtrl);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(SettingsTemplateModel model)
        {
            if (ModelState.IsValid)
            {
                new SaveTemplateSettings(model).Execute();
                ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
            }

            ShowErrorMessages();

            return RedirectToAction("Index");
        }
        
        [ChildActionOnly]
        public ActionResult OtherSettings(ETemplateSettingSection? section, List<TemplateSettingSection> settings)
        {
            if(section != null){
                var enumStr = section.StrName();
                settings = settings.Where(x => x.Key.ToLower() == enumStr).ToList();
            }else{
                settings = settings.Where(x => !Enum.IsDefined(typeof(ETemplateSettingSection), x.Key)).ToList();
            }
            
            return PartialView("_Other", new OtherSettings()
            {
                Section = section,
                Settings = settings
            });
        }
    }
}
