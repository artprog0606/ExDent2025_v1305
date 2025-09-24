namespace AdvantShop.App.Landing.Domain.Settings
{
    public class LpSiteSettings
    {
        public int Id { get; set; }
        public int LandingSiteId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public LpSiteSettings()
        {
        }

        public LpSiteSettings(int landingSiteId, string name, string value)
        {
            LandingSiteId = landingSiteId;
            Name = name;
            Value = value;
        }
    }
}
