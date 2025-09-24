using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Landing.Templates;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Design;
using AdvantShop.Web.Admin.Models.Dashboard;

namespace AdvantShop.Web.Admin.Handlers.Dashboard
{
    public class GetSiteTemplates
    {
        private readonly LpSiteCategory _category;

        public GetSiteTemplates(LpSiteCategory category)
        {
            _category = category;
        }
        
        public List<SiteTemplateModel> GetTemplates()
        {
            var templates = TemplateService.GetTemplates().Items
                .Where(x => !x.IsInstall)
                .Select(x => new SiteTemplateModel()
                {
                    Name = x.Name,
                    Picture = x.Icon,
                    Price = x.Price,
                    PriceStr = x.Price > 0
                        ? x.Price + " " + x.Currency
                        : LocalizationService.GetResource("Admin.Design.Index.Free"),
                    EditLink = string.Format("dashboard/createTemplate?id={0}", x.StringId),
                    DemoLink = x.OnlineDemoLink,
                    IsPopular = x.Popular,
                    Id = x.Id,
                    StringId = x.StringId,
                    Version = x.Version,
                    Active = x.Active
                }).ToList();

            return templates;
        }

        public SiteTemplatesModel Execute()
        {
            var model = new SiteTemplatesModel();

            if (_category == LpSiteCategory.Store)
            {
                model.Templates = GetTemplates();
            }
            else
            {
                var lpTemplateService = new LpTemplateService();

                var templates = _category == LpSiteCategory.AllFunnels
                    ? lpTemplateService.GetTemplates()
                        // есть шаблоны воронок без категории (Store default)
                        .Where(x => x.SiteCategory != LpSiteCategory.Store)
                        .ToList()
                    : lpTemplateService.GetTemplates()
                        .Where(x => x.SiteCategory == _category)
                        .ToList();

                model.LpTemplates = templates.Select(x => new SiteTemplateModel
                {
                    Name = x.Name,
                    Picture =
                        !string.IsNullOrEmpty(x.Picture)
                            ? x.Picture
                            : "./../areas/admin/content/images/landing/templates/" + _category + ".jpg",
                    EditLink = $"dashboard/createFunnel?id={x.Key}",
                    DemoLink = x.DemoLink
                }).ToList();
            }

            return model;
        }


    }
}
