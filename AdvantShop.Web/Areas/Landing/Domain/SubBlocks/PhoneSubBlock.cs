using AdvantShop.Configuration;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.App.Landing.Domain.SubBlocks
{
    public class PhoneSubBlock : BaseLpSubBlock
    {
        public override string GetContent(LpConfiguration configuration)
        {
            return SettingsMain.Phone;
        }
    }

    public class PhoneMobileSubBlock : BaseLpSubBlock
    {
        public override string GetContent(LpConfiguration configuration)
        {
            return SettingsMain.MobilePhone;
        }
    }
}
