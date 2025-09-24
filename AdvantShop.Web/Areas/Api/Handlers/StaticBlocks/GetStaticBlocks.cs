using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.StaticBlocks;
using AdvantShop.CMS;
using AdvantShop.Core;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.StaticBlocks
{
    public class GetStaticBlocks : AbstractCommandHandler<GetStaticBlocksResponse>
    {
        private readonly string _keys;

        public GetStaticBlocks(string keys)
        {
            _keys = keys;
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_keys))
                throw new BlException("Укажите ключи");
        }

        protected override GetStaticBlocksResponse Handle()
        {
            var items = new List<StaticBlockItem>();
            
            var keys = _keys.Split(new [] {","}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            foreach (var key in keys)
            {
                var sb = StaticBlockService.GetPagePartByKeyWithCache(key);
                if (sb != null && sb.Enabled)
                    items.Add(new StaticBlockItem(sb));
            }

            return new GetStaticBlocksResponse(items);
        }
    }
}