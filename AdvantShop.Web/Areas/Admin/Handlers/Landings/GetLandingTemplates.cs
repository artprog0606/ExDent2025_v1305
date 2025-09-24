using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Templates;
using AdvantShop.Web.Admin.Models.Landings;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class GetLandingTemplates
    {
        private readonly LpFunnelCategory _category;
        private readonly LpTemplateService _templateService;

        public GetLandingTemplates(LpFunnelCategory category)
        {
            _category = category;
            _templateService = new LpTemplateService();
        }

        public List<LpTemplateModel> Execute()
        {
            return
                _templateService.GetTemplates().Where(x => x.Category == _category)
                    .Select(x => new LpTemplateModel
                    {
                        Key = x.Key,
                        Name = x.Name,
                        Description = x.Description,
                        Scheme = x.Scheme,
                        Video = x.Video,
                        ImageSrc =
                            !string.IsNullOrEmpty(x.Picture)
                                ? x.Picture
                                : "./../areas/admin/content/images/landing/templates/" + _category + ".jpg",
                        //ImageSrc =
                        //    !string.IsNullOrEmpty(x.Picture)
                        //        ? x.Picture
                        //        : "./../areas/admin/content/images/landing/templates/" + _category + ".jpg",
                        TemplateType = x.TemplateType.ToString()
                    })
                    .ToList();
        }
    }
}
