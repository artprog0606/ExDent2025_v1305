using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Web.Admin.Models.Catalog.Units
{
    public class UnitModel
    {
        public static UnitModel CreateByUnit(Unit unit) => new UnitModel()
        {
            Id = unit.Id,
            Name = unit.Name,
            DisplayName = unit.DisplayName,
            MeasureType = unit.MeasureType.HasValue
                ? (byte) unit.MeasureType
                : (byte?) null,
            SortOrder = unit.SortOrder,
        };
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public byte? MeasureType { get; set; }
        public int SortOrder { get; set; }
        public int ProductsCount { get; set; }
        public bool CanBeDeleted => ProductsCount == 0;
    }
}