namespace AdvantShop.Core.Services.Crm.OzonSeller.Api
{
    public class GetCategoryAttributeValuesParams
    {
        public int CategoryId { get; set; }
        public int AttributeId { get; set; }
        public string Language { get; set; }
        public int LastValueId { get; set; }
        public int Limit { get; set; }
    }
}
