//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Core.Modules.Interfaces
{
    public abstract class BaseTab : ITab
    {
        public int ProductId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string TabGroup { get; set; }

        public bool Active { get; set; }

        public int SortOrder { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }

        public string TabId { get; set; }

    }

    public interface ITab { }
}
