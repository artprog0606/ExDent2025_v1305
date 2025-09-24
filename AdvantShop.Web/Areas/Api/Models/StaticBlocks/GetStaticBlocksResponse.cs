using System.Collections.Generic;
using AdvantShop.CMS;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.StaticBlocks
{
    public class GetStaticBlocksResponse : List<StaticBlockItem>, IApiResponse
    {
        public GetStaticBlocksResponse(List<StaticBlockItem> items)
        {
            this.AddRange(items);
        }
    }

    public class StaticBlockItem
    {
        public string Key { get; }
        public string Value { get; }

        public StaticBlockItem(StaticBlock sb)
        {
            Key = sb.Key;
            Value = !string.IsNullOrWhiteSpace(sb.Content) ? sb.Content : null;
        }
    }
}