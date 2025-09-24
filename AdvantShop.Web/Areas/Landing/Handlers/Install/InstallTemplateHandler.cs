using AdvantShop.App.Landing.Domain;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Handlers.Inplace;
using AdvantShop.App.Landing.Models.Inplace;
using System.Linq;
using AdvantShop.App.Landing.Models.LandingAdmin;
using AdvantShop.Core;

namespace AdvantShop.App.Landing.Handlers.Install
{
    public class InstallTemplateHandler
    {
        #region Ctor
        
        private readonly LandingAdminIndexPostModel _model;

        private readonly LpService _lpService;
        private readonly LpSiteService _siteService;
        private readonly LpTemplateService _templateService;

        public InstallTemplateHandler(LandingAdminIndexPostModel model)
        {
            _model = model;

            _lpService = new LpService();
            _siteService = new LpSiteService();
            _templateService = new LpTemplateService();
        }

        #endregion

        public Lp Execute()
        {
            var lp = new Lp()
            {
                Name = _model.Name,
                Template = _model.Template,
                Enabled = true,
                IsMain = _model.SiteId == null,
                LandingSiteId = _model.SiteId ?? 0
            };

            if (!string.IsNullOrEmpty(_model.Template))
            {
                if (_templateService.GetTemplate(lp.Template) == null)
                    throw new BlException("Шаблон не существует");
            }
            else
            {
                var t = _templateService.GetTemplates().FirstOrDefault();
                if (t != null)
                    lp.Template = t.Key;
            }

            //var сonfiguration = new LpConfiguration()
            //{
            //    Type = _model.Type,
            //    Goal = _model.Goal
            //};

            //if (model.Type == LpType.OneProduct)
            //{
            //    var product = ProductService.GetProduct(model.ProductIds.TryParseInt());
            //    if (product != null)
            //        сonfiguration.Products = new List<Product>() { product };
            //}
            //else if (model.Type == LpType.FewProducts)
            //{
            //    var ids = model.ProductIds != null ? model.ProductIds.Split(new[] { ',' }).Select(x => x.TryParseInt()).ToList() : new List<int>();
            //    сonfiguration.Products = new List<Product>();
            //    foreach (var id in ids)
            //    {
            //        var product = ProductService.GetProduct(id);
            //        if (product != null)
            //            сonfiguration.Products.Add(product);
            //    }
            //}

            if (lp.LandingSiteId == 0)
            {
                lp.LandingSiteId = _siteService.Add(new LpSite()
                {
                    Name = lp.Name,
                    Template = lp.Template,
                    Enabled = true,
                    Url = _siteService.GetAvailableUrl(lp.Name)
                });
            }

            lp.Url = _lpService.GetAvailableUrl(lp.LandingSiteId, lp.Name);

            _lpService.Add(lp);

            // TODO: Uncomment it!
            for (var i = 0; i < template.Blocks.Count; i++)
            {
                try
                {
                    var installBlockHandler = new InstallBlockHandler(template.Blocks[i], template.Key, _lp.Id, i * 100, сonfiguration);
                    var result = installBlockHandler.Execute();
                    //if (!result.Result)
                    //    Debug.Log.Error("Can't install landing block " + template.Blocks[i] + " for template " + template.Key);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }

            new SaveLpPageSettings(lp.Id, new InplaceSettingsModel()
            {
                PageTitle = lp.Name,
                FontMain = LPageSettings.GetDefaultFonts()[0].Name
            }).Execute();

            return lp;
        }

    }
}
