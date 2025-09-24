using System;
using System.Linq;
using AdvantShop.Areas.Api.Models.Settings;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Configuration;
using AdvantShop.Core;

namespace AdvantShop.Areas.Api.Handlers.Settings
{
    public class GetSettings : AbstractCommandHandler<SettingsResponse>
    {
        private readonly string _keys;

        public GetSettings(string keys)
        {
            _keys = keys;
        }

        protected override void Validate()
        {
            if (string.IsNullOrEmpty(_keys))
                throw new BlException("Укажите список настроек");
        }

        protected override SettingsResponse Handle()
        {
            var items =
                _keys.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => new SettingItemResponse(x, SettingProvider.Items[x]))
                    .ToList();

            return new SettingsResponse(items);
        }
    }
}