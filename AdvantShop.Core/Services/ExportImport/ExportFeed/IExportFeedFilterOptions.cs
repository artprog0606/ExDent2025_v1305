using System.Collections.Generic;

namespace AdvantShop.ExportImport
{
    public interface IExportFeedFilterOptions : IExportFeedOptions
    {
        bool AllowPreOrderProducts { get; }
        bool OnlyMainOfferToExport { get; }

        int? PriceRuleId { get; }
        int? PriceRuleIdForOldPrice { get; }
        decimal? NotExportAmountCount { get; }
        bool DontExportProductsWithoutDimensionsAndWeight { get; }
        
        string Currency { get; }
        decimal? PriceFrom { get; }
        decimal? PriceTo { get; }
        
        bool ConsiderMultiplicityInPrice { get; }
        
        List<int> WarehouseIds { get; }
    }
}