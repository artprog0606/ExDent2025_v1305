using System.Collections.Generic;
using AdvantShop.App.Landing.Domain.Common;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Landing.Forms;

namespace AdvantShop.App.Landing.Models.Inplace
{
    public class BlockSettings
    {
        public int BlockId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Dictionary<string, object> Settings { get; set; }

        public List<SubBlockSettingsModel> Subblocks { get; set; }
        public LpForm Form { get; set; }

        public List<ReferenceBlockItem> ReferenceBlocks { get; set; }
    }

    public class BlockSettingsModel : BlockSettings
    {
        public List<AdvListItem> PostActions { get; set; }
        public List<LpFormField> CrmFields { get; set; }
        public List<SalesFunnel> SalesFunnels { get; set; }
        public List<AdvListItem> Landings { get; set; }
        public List<AdvListItem> LandingsForUrl { get; set; }
        public List<AdvListItem> PostMessageRedirectLps { get; set; }

        public bool IsStoreClosed => SettingsMain.IsStoreClosed;
    }

    public class SubBlockSettingsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Dictionary<string, object> Settings { get; set; }
        public string ContentHtml { get; set; }
    }

    public class ReferenceBlockItem
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }
}
