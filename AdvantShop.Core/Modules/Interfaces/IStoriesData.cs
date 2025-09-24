using System.Collections.Generic;
using AdvantShop.Core.Services.Stories;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IStoriesData
    { 
        IList<StoryData> GetData();
    }
}
