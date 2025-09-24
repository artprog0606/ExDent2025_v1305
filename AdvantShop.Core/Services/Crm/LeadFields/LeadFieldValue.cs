namespace AdvantShop.Core.Services.Crm.LeadFields
{
    public class LeadFieldValue
    {
        public int Id { get; set; }
        public int LeadFieldId { get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }
    }
}
