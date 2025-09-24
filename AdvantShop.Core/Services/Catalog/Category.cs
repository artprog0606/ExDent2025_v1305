//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Helpers;
using AdvantShop.SEO;
using Newtonsoft.Json;

namespace AdvantShop.Catalog
{
    [Serializable]
    public class Category
    {
        public Category()
        {
            CategoryId = CategoryService.DefaultNonCategoryId;
            Enabled = true;
        }

        public int CategoryId { get; set; }

        [Compare("Core.Catalog.Category.ExternalId")]
        public string ExternalId { get; set; }

        [Compare("Core.Catalog.Category.Name")]
        public string Name { get; set; }

        [Compare("Core.Catalog.Category.Description", true)]
        public string Description { get; set; }

        [Compare("Core.Catalog.Category.BriefDescription", true)]
        public string BriefDescription { get; set; }

        [Compare("Core.Catalog.Category.Enabled")]
        public bool Enabled { get; set; }

        [Compare("Core.Catalog.Category.Hidden")]
        public bool Hidden { get; set; }


        public bool HasChild { get; set; }

        [Compare("Core.Catalog.Category.SortOrder")]
        public int SortOrder { get; set; }


        /// <summary>
        /// Кол-во активных не скрытых с учетом подкатегорий
        /// </summary>
        public int ProductsCount { get; set; }
        
        /// <summary>
        /// Кол-во всех товаров с учетом подкатегорий
        /// </summary>
        public int TotalProductsCount { get; set; }

        [Compare("Core.Catalog.Category.ParentCategoryId")]
        public int ParentCategoryId { get; set; }

        [Compare("Core.Catalog.Category.DisplayStyle")]
        public ECategoryDisplayStyle DisplayStyle { get; set; }

        [Compare("Core.Catalog.Category.DisplayChildProducts")]
        public bool DisplayChildProducts { get; set; }

        [Compare("Core.Catalog.Category.DisplayBrandsInMenu")]
        public bool DisplayBrandsInMenu { get; set; }

        [Compare("Core.Catalog.Category.DisplaySubCategoriesInMenu")]
        public bool DisplaySubCategoriesInMenu { get; set; }

        public bool ParentsEnabled { get; set; }

        [Compare("Core.Catalog.Category.Sorting")]
        public ESortOrder Sorting { get; set; }

        [Compare("Core.Catalog.Category.AutomapAction")]
        public ECategoryAutomapAction AutomapAction { get; set; }

        /// <summary>
        /// Кол-во товаров доступных к покупке с учетом подкатегорий (активных, не скрытых, есть кол-во или под заказ)
        /// </summary>
        public int Available_Products_Count { get; set; }
        
        /// <summary>
        /// Кол-во активных не скрытых в данной категории
        /// </summary>
        public int Current_Products_Count { get; set; }

        private CategoryPhoto _picture;
        public CategoryPhoto Picture
        {
            get => _picture ?? (_picture = PhotoService.GetPhotoByObjId<CategoryPhoto>(CategoryId, PhotoType.CategoryBig));
            set => _picture = value;
        }

        private CategoryPhoto _minipicture;
        public CategoryPhoto MiniPicture
        {
            get => _minipicture ?? (_minipicture = PhotoService.GetPhotoByObjId<CategoryPhoto>(CategoryId, PhotoType.CategorySmall));
            set => _minipicture = value;
        }

        private CategoryPhoto _icon;
        public CategoryPhoto Icon
        {
            get => _icon ?? (_icon = PhotoService.GetPhotoByObjId<CategoryPhoto>(CategoryId, PhotoType.CategoryIcon));
            set => _icon = value;
        }

        private Category _parentcategory;

        [JsonIgnore]
        public Category ParentCategory => _parentcategory ?? (_parentcategory = CategoryService.GetCategory(ParentCategoryId));

        private string _urlPath;

        [Compare("Core.Catalog.Category.UrlPath")]
        public string UrlPath
        {
            get => _urlPath;
            set => _urlPath = value.ToLower();
        }

        public MetaType MetaType => MetaType.Category;

        private bool _metaLoaded;
        private MetaInfo _meta;
        public MetaInfo Meta
        {
            get
            {
                if (_metaLoaded)
                    return _meta;

                _metaLoaded = true;
                return _meta ??
                       (_meta =
                           MetaInfoService.GetMetaInfo(CategoryId, MetaType) ??
                           MetaInfoService.GetDefaultMetaInfo(MetaType, Name));
            }
            set
            {
                _meta = value;
                _metaLoaded = true;
            }
        }

        public override string ToString()
        {
            return $"{Name} {CategoryId}";
        }

        public int ID => CategoryId;

        private bool _tagsLoaded;
        private List<Tag> _tags;

        public List<Tag> Tags
        {
            get
            {
                if (_tagsLoaded)
                    return _tags;
                
                _tagsLoaded = true;
                return _tags = TagService.Gets(CategoryId, ETagType.Category, true);
            }
            set
            {
                _tags = value;
                _tagsLoaded = true;
            }
        }

        public string ModifiedBy { get; set; }

        [Compare("Core.Catalog.Category.ShowOnMainPage")]
        public bool ShowOnMainPage { get; set; }

        private bool _sizeChartLoaded;
        private SizeChart _sizeChart;
        public SizeChart SizeChart
        {
            get
            {
                if (_sizeChartLoaded)
                    return _sizeChart;
                
                _sizeChartLoaded = true;
                return _sizeChart ?? (_sizeChart = SizeChartService.Get(CategoryId, ESizeChartEntityType.Category).FirstOrDefault());
            }
            set
            {
                _sizeChart = value;
                _sizeChartLoaded = true;
            }
        }
    }
}
