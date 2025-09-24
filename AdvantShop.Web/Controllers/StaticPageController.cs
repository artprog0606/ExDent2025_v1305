using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.CMS;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.InplaceEditor;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Customers;
using AdvantShop.SEO;
using AdvantShop.ViewModel.StaticPage;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class StaticPageController : BaseClientController
    {
        [AccessByChannel(EProviderSetting.StoreActive)]
        public ActionResult Index(string url)
        {
            if (string.IsNullOrEmpty(url))
                return Error404();

            var staticPage = StaticPageService.GetStaticPage(url);
            if (staticPage == null || !staticPage.Enabled)
                return Error404();

            MetaInfo metaInformation = SetMetaInformation(staticPage.Meta, staticPage.PageName);

            var model = new StaticPageViewModel()
            {
                Id = staticPage.StaticPageId,
                Title = staticPage.PageName,
                Text = staticPage.PageText,
                UrlPath = staticPage.UrlPath,
                H1 = metaInformation.H1,
                BreadCrumbs = new List<BreadCrumbs>()
                {
                    new BreadCrumbs(T("MainPage"), Url.AbsoluteRouteUrl("Home"))
                }
            };

            SetMobileTitle(staticPage.PageName);

            if (!string.IsNullOrEmpty(model.Text) && InplaceEditorService.CanUseInplace(RoleAction.Store))
            {
                model.Text = InplaceEditorService.PrepareContent(model.Text);
            }

            var parentPages = StaticPageService.GetParentStaticPages(staticPage.StaticPageId).Select(StaticPageService.GetStaticPage).Reverse().ToList();

            model.BreadCrumbs.AddRange(parentPages.Select(stPage => new BreadCrumbs
            {
                Name = stPage.PageName,
                Url = Url.AbsoluteRouteUrl("StaticPage", new { url = stPage.UrlPath })
            }));

            model.SubPages = StaticPageService.GetChildStaticPages(staticPage.StaticPageId, true).Select(subPage => new StaticPageViewModel
            {
                Id = subPage.StaticPageId,
                Title = subPage.PageName,
                Text = subPage.PageText,
                UrlPath = subPage.UrlPath
            }).ToList();

            if (!model.SubPages.Any())
            {
                if (staticPage.ParentId != 0)
                    model.SubPages = StaticPageService.GetChildStaticPages(staticPage.ParentId, true).Select(subPage => new StaticPageViewModel
                    {
                        Id = subPage.StaticPageId,
                        Title = subPage.PageName,
                        Text = subPage.PageText,
                        UrlPath = staticPage.StaticPageId != subPage.StaticPageId ? subPage.UrlPath : null
                    }).ToList();

                model.ParentPages = parentPages.Where(stPage => stPage.StaticPageId != staticPage.StaticPageId)
                    .Select(stPage => new StaticPageViewModel
                    {
                        Id = stPage.StaticPageId,
                        Title = stPage.PageName,
                        UrlPath = stPage.UrlPath
                    }).ToList();
            }
            else
            {
                model.ParentPages.Add(new StaticPageViewModel
                {
                    Id = staticPage.StaticPageId,
                    Title = staticPage.PageName,
                });
            }

            var tagManager = GoogleTagManagerContext.Current;
            if (tagManager.Enabled)
            {
                tagManager.PageType = ePageType.info;
            }

            SetNgController(NgControllers.NgControllersTypes.StaticPageCtrl);

            return View(model);
        }
    }
}