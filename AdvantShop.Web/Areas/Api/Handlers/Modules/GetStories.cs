using AdvantShop.Areas.Api.Models.Inits;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Stories;
using AdvantShop.Web.Infrastructure.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Areas.Api.Handlers.Modules
{
    public class GetStories : AbstractCommandHandler<GetStoriesRespone>
    {
        protected override GetStoriesRespone Handle()
        {
            var modules = AttachedModules.GetModules<IStoriesData>();
            var result = new List<StoryData>();
            foreach (var cls in modules)
            {
                var isActive = cls != null;
                if (!isActive)
                    continue;

                var classInstance = (IStoriesData)Activator.CreateInstance(cls);
                var data = classInstance.GetData();
                if (data != null)
                    result.AddRange(data);
            }
            
            return new GetStoriesRespone(result);
        }
    }
}