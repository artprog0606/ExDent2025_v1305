using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Models;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Handlers.Brands;
using AdvantShop.Models.Brand;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Core.Services.SEO.MetaData;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class BrandController : BaseClientController
    {
        [AccessByChannel(EProviderSetting.StoreActive)]
        public ActionResult Index(string letter, string countryId, string q, int? page)
        {
            var model = new BrandHandler().GetBrandList(letter, countryId, q, page);
            if (model == null)
                return Error404();

            model.BrandLogoWidth = SettingsPictureSize.BrandLogoWidth;
            model.BrandLogoHeight = SettingsPictureSize.BrandLogoHeight;

            model.Pager = new Pager()
            {
                CurrentPage = page != null && page > 0 ? (int) page : 1,
                TotalPages = (int) (Math.Ceiling((double) model.Brands.Count/ SettingsCatalog.BrandsPerPage))
            };

            if (model.Brands.Count > 0)
            {
                if ((model.Pager.TotalPages < model.Pager.CurrentPage && model.Pager.CurrentPage > 1) ||
                    model.Pager.CurrentPage < 0)
                {
                    return Error404();
                }

                model.Brands =
                    model.Brands.Skip((model.Pager.CurrentPage - 1)* SettingsCatalog.BrandsPerPage).Take(SettingsCatalog.BrandsPerPage).ToList();
            }

            SetMetaInformation(model.Meta, page: model.Pager.CurrentPage, totalPages: model.Pager.TotalPages);

            var meta = new OpenGraphModel();
            if (model.Brands.Count > 0)
                meta.Image = model.Brands[0].BrandLogo.ImageSrc();

            MetaDataContext.CurrentObject = meta;

            SetNgController(NgControllers.NgControllersTypes.BrandCtrl);

            return View(model);
        }


        public ActionResult BrandItem(string url, BrandModel brandModel)
        {
            var brand = BrandService.GetBrand(url);
            if (brand == null || !brand.Enabled)
                return Error404();

            var model = new BrandHandler().GetBrandItem(brand, brandModel);
            if (model == null)
                return Error404();

            if (model.ProductsList != null)
            {
                SetMetaInformation(brand.Meta, brand.Name, page: model.ProductsList.Pager.CurrentPage, totalPages: model.ProductsList.Pager.TotalPages);
            }
            else
            {
                SetMetaInformation(brand.Meta, brand.Name);
            }

            SetNgController(NgControllers.NgControllersTypes.BrandCtrl);

            var meta = new OpenGraphModel
            {
                Image = brand.BrandLogo.ImageSrc()
            };

            MetaDataContext.CurrentObject = meta;

            WriteLog(brand.Name, Url.AbsoluteRouteUrl("Brand", brand.UrlPath), ePageType.brand);

            return View(model);
        }

        [HttpGet]
        public ActionResult GetBrandItem(string url, BrandModel brandModel)
        {
            var brand = BrandService.GetBrand(url);
            if (brand == null || !brand.Enabled)
                return Error404();

            var model = new BrandHandler().GetBrandItem(brand, brandModel);
            if (model == null)
                return Error404();

            if (model.ProductsList != null)
            {
                return PartialView("_ProductView", model.ProductsList.Products);
            }

            return new EmptyResult();

        }

        public ActionResult BrandCarousel(int count)
        {
            if (!SettingsDesign.BrandCarouselVisibility)
                return null;

            var brands = BrandService.GetBrandsOnLimit(count);

            return brands.Count > 0 ? PartialView(brands) : null;
        }
    }
}