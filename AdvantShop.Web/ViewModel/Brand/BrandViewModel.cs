using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Models;
using AdvantShop.SEO;

namespace AdvantShop.ViewModel.Brand
{
    public class BrandViewModel
    {
        public List<AdvantShop.Catalog.Brand> Brands { get; set; }

        public Pager Pager { get; set; }

        public List<SelectListItem> Countries { get; set; }

        public List<BrandLetter> EnLetters { get; set; }

        public List<BrandLetter> RuLetters { get; set; }

        public string SearchBrand { get; set; }

        public List<BreadCrumbs> BreadCrumbs { get; set; }

        public int CurentCountyId { get; set; }

        public BrandViewModel()
        {
            Brands = new List<AdvantShop.Catalog.Brand>();
            Countries = new List<SelectListItem>();
        }

        public int BrandLogoHeight { get; set; }

        public int BrandLogoWidth { get; set; }

        private MetaInfo _meta;

        public MetaInfo Meta
        {
            get
            {
                return _meta ??
                       (_meta =
                           MetaInfoService.GetMetaInfo(0, MetaType) ??
                           new MetaInfo
                           {
                               Type = MetaType.MainPageBrands,
                               Title = SettingsSEO.BrandMetaTitle,
                               MetaKeywords = SettingsSEO.BrandMetaKeywords,
                               MetaDescription = SettingsSEO.BrandMetaDescription,
                               H1 = SettingsSEO.BrandMetaH1
                           });
            }
            set { _meta = value; }
        }

        public MetaType MetaType
        {
            get { return MetaType.MainPageBrands; }
        }
    }

    public class BrandLetter
    {
        public string Name { get; set; }

        public string Url { get; set; }
        
        public bool Active { get; set; }

        public bool Selected { get; set; }
    }
}