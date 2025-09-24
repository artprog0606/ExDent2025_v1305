namespace AdvantShop.Web.Admin.Models.Catalog.PropertyValues
{
    public class PropertyValueModel
    {
        public int PropertyValueId { get; set; }
        public int PropertyId { get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }
        public int ProductsCount { get; set; }
        public string Name { get; set; }
    }
}
