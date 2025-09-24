using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Partners;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Partners;
using AdvantShop.Web.Admin.ViewModels.Partners;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Partners
{
    [Auth(RoleAction.Partners)]
    [SaasFeature(ESaasProperty.Partners)]
    [SalesChannel(ESalesChannelType.Partners)]
    public partial class PartnersProgrammController : BaseAdminController
    {
       
        public ActionResult Index()
        {
            var isMobile = SettingsDesign.IsMobileTemplate;

            SetMetaInformation(T("Admin.Partners.Index.Title"));
           

            if (isMobile) {
                SetNgController(NgControllers.NgControllersTypes.ExportFeedsCtrl);
                return View();
            } 
            else
            {
                return Redirect(UrlService.GetAdminUrl("partners"));
            }
        }
    }
}
