using AdvantShop.Configuration;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.App.Landing.Domain.SubBlocks
{
    public class EmailSubBlock : BaseLpSubBlock
    {
        public override string GetContent(LpConfiguration configuration)
        {
            return SettingsMail.EmailForLeads;
        }
    }
}
