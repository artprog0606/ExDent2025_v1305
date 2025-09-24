namespace AdvantShop.Core.Services.Crm.OzonSeller.Api
{
    public class CategoryAttribute
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool IsCollection { get; set; }
        public bool IsRequired { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int DictionaryId { get; set; }
    }
}
