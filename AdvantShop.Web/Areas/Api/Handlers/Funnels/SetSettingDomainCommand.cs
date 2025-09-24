using AdvantShop.Areas.Api.Model.Funnels;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.ExportImport;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Funnels
{
    public class SetSettingDomainCommand : ICommandHandler<SetSettingDomainDto, bool>
    {
        public bool Execute(SetSettingDomainDto model)
        {
            if (SettingsLic.LicKey != model.Lickey)
                throw new BlException("wrong lickey");

            bool needUpdate = SettingsMain.SiteUrl != model.Domain;
            SettingsMain.SiteUrl = model.Domain;
            if (needUpdate) {
                new ExportHtmlMap().Create();
                new ExportXmlMap().Create();
            }

            return true;
        }
    }
}