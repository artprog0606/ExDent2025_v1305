using AdvantShop.Configuration;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Saas;
using AdvantShop.Trial;
using System.Linq;

namespace AdvantShop.Core.Services.Domains
{
    public interface IDomainService
    {
        void Add(string domain);
        void Remove(string domain);
    }

    public class DomainService : IDomainService
    {
        private const string Host = "http://modules.advantshop.net/";

        public void Add(string domain)
        {
            if (!SaasDataService.IsSaasEnabled && !TrialService.IsTrialEnabled)
                return;

            var model = new DomainShopDto
            {
                LicKey = SettingsLic.LicKey,
                Domain = domain,
            };

            var result = RequestHelper.MakeRequest<string>(Host + "domain/add", model);

            if (result != null)
                result = result.Trim('"');

            if (result != "ok")
                throw new BlException(result);
        }

        public void Remove(string domain)
        {
            if (!SaasDataService.IsSaasEnabled && !TrialService.IsTrialEnabled)
                return;

            var model = new DomainShopDto
            {
                LicKey = SettingsLic.LicKey,
                Domain = domain,
            };

            var result = RequestHelper.MakeRequest<string>(Host + "domain/remove", model);

            if (result != null)
                result = result.Trim('"');

            if (result != "ok")
                throw new BlException(result);
        }

        public static bool IsAvalable()
        {
            return (SaasDataService.IsSaasEnabled || TrialService.IsTrialEnabled || Demo.IsDemoEnabled) 
                    && CustomerContext.CurrentCustomer.IsAdmin 
                    && SalesChannelService.GetList()
                        .Any(x => x.Enabled
                            && (x.Type == ESalesChannelType.Dashboard
                            || x.Type == ESalesChannelType.Partners
                            || x.Type == ESalesChannelType.Module 
                            && x.ModuleStringId == "MobileApp"));
        }

        public class DomainShopDto
        {
            public string LicKey { get; set; }
            public string Domain { get; set; }
        }
    }
}
