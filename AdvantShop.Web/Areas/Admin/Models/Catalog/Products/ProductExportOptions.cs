using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Web.Admin.Models.Catalog.Products
{
    public class ProductExportOptionsModel
    {
        public ProductExportOptionsModel()
        {
            YandexDiscountConditions = Enum.GetValues(typeof(EYandexDiscountCondition)).Cast<EYandexDiscountCondition>()
                .Select(x => new SelectItemModel(x.Localize(), (int)x)).ToList();
            YandexProductQualityTypes = Enum.GetValues(typeof(EYandexProductQuality)).Cast<EYandexProductQuality>()
                .Select(x => new SelectItemModel(x.Localize(), (int)x)).ToList();
            
            ShowMarkingRequiredInOrderHint = FeaturesService.IsEnabled(EFeature.MarkingRequired);
        }

        public int ProductId { get; set; }
        public List<SelectItemModel> YandexDiscountConditions { get; private set; }
        public List<SelectItemModel> YandexProductQualityTypes { get; private set; }
        public AdvantShop.Catalog.ProductExportOptions ExportOptions { get; set; }
        public bool IsMarkingRequired { get; set; }
        public bool ShowMarkingRequiredInOrderHint { get; }
    }
}
