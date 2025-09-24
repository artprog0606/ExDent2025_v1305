using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;

namespace AdvantShop.App.Landing.Domain.SubBlocks
{
    public interface ILpSubBlock
    {
        string GetContent(LpConfiguration configuration);

        dynamic GetSettings(LpBlock currentBlock, LpConfiguration configuration, dynamic settings);
    }

    public class BaseLpSubBlock : ILpSubBlock
    {
        public virtual string GetContent(LpConfiguration configuration)
        {
            return null;
        }

        public virtual dynamic GetSettings(LpBlock currentBlock, LpConfiguration configuration, dynamic settings)
        {
            return settings;
        }
    }
}
