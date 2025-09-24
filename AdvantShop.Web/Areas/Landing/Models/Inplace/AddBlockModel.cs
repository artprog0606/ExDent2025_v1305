using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing.Blocks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models.Inplace
{
    public class AddBlockModel
    {
        public int LpId { get; set; }

        public string Name { get; set; }

        public int SortOrder { get; set; }

        public int? ProductId { get; set; }
    }

    public class AddAllBlocksModel
    {
        public int LpId { get; set; }

        public int SortOrder { get; set; }

        public int? ProductId { get; set; }

        public List<LpBlockItem> Blocks { get; set; }
    }

    public class AddBlockResultModel
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }

        [JsonProperty("block")]
        public LpBlock Block { get; set; }
    }
}
