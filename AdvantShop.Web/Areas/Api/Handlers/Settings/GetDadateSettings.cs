using AdvantShop.Areas.Api.Models.Settings;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Core.Modules;

namespace AdvantShop.Areas.Api.Handlers.Settings
{
    public class GetDadataSettings : AbstractCommandHandler<GetDadataSettingsResponse>
    {
        private const string ModuleId = "DaData";

        protected override GetDadataSettingsResponse Handle()
        {
            var isActive = AttachedModules.GetModuleById(ModuleId, true) != null;
            return new GetDadataSettingsResponse()
            {
                IsActive = isActive,
                ApiKey = isActive
                    ? ModuleSettingsProvider.GetSettingValue<string>("ApiKey", ModuleId)
                    : null,
                SecretKey = isActive
                    ? ModuleSettingsProvider.GetSettingValue<string>("SecretKey", ModuleId)
                    : null
            };
        }
    }
}