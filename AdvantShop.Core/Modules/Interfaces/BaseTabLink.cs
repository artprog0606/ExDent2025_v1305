namespace AdvantShop.Core.Modules.Interfaces
{
    public abstract class BaseTabLink : ITabLink
    {
        public string TabId { get; set; }
        public string TitleOfLink { get; set; }
    }

    public interface ITabLink
    {
        string TabId { get; set; }
        string TitleOfLink { get; set; }
    }
}
