namespace AdvantShop.Core.Services.Crm
{
    public class LeadsListSourceItem
    {
        public int OrderSourceId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public int LeadsCount { get; set; }
        public int PercentLeads { get; set; }
    }
}
