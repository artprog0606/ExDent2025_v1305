using System;
using System.Linq;
using AdvantShop.Design;
using AdvantShop.Web.Admin.Models.Dashboard;

namespace AdvantShop.Web.Admin.Handlers.Dashboard
{
    public class GetCreateTemplateModel
    {
        private readonly string _id;

        public GetCreateTemplateModel(string id)
        {
            _id = id;
        }

        public CreateTemplateModel Execute()
        {
            var template = TemplateService.GetTemplates().Items.FirstOrDefault(x => x.StringId.ToLower() == _id.ToLower());
            if (template == null)
                return null;

            var model = new CreateTemplateModel()
            {
                Id = template.Id,
                StringId = template.StringId,
                Description = template.Description,
                Name = template.Name,
                Icon = template.Icon,
                BriefDescription = template.BriefDescription,
                Active = template.Active,
                Currency = template.Currency,
                CurrentVersion = template.CurrentVersion,
                DetailsLink = template.DetailsLink,
                Developer = template.Developer,
                DateAdded = template.DateAdded,
                DateModified = template.DateModified,
                DcType = template.DcType,
                DeveloperSupport = template.DeveloperSupport,
                DeveloperWebSite = template.DeveloperWebSite,
                IsCustomVersion = template.IsCustomVersion,
                IsInstall = template.IsInstall,
                IsLocalVersion = template.IsLocalVersion,
                NeedUpdate = template.NeedUpdate,
                OnlineDemoLink = template.OnlineDemoLink,
                Price = template.Price,
                SortOrder = template.SortOrder,
                Version = template.Version,

                Colors = (template.BriefDescription ?? "").Split(new []{";"}, StringSplitOptions.RemoveEmptyEntries).ToList()
            };

            return model;
        }
    }
}
