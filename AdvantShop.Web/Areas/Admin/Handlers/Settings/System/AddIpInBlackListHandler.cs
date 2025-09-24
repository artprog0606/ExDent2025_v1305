using AdvantShop.Configuration;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Saas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    public class AddIpInBlackListHandler
    {
        private readonly List<string> _ipWithMaskList;

        public AddIpInBlackListHandler(List<string> ipWithMaskList)
        {
            _ipWithMaskList = ipWithMaskList;
        }

        public List<string> Execute()
        {
            var errors = new List<string>(); 
            var ipWhiteList = SaasDataService.CurrentSaasData.IpWhiteListValues; 
            var patternIpV4OrIpV6 = "^(((([0-1]?[0-9]{1,2}|2[0-4][0-9]|25[0-5])\\.){3}([0-1]?[0-9]{1,2}|2[0-4][0-9]|25[0-5])|(((([0-9a-fA-F]){1,4})\\:){7}([0-9a-fA-F]){1,4}))(/([1-2]?[0-9]|3[0-2]))?)$";
            var ipRegex = new Regex(patternIpV4OrIpV6);
            var validIps = new List<string>();
            var currentIp = HttpContext.Current.Request != null ? HttpContext.Current.TryGetIp() : null;
            foreach (var ipWithMask in _ipWithMaskList)
            {
                if (ipRegex.IsMatch(ipWithMask))
                    if (currentIp != null && UrlService.IsInSubnet(currentIp, ipWithMask))
                        errors.Add(LocalizationService.GetResource("Admin.SettingsSystem.IpBlacklist.CannotBanCurrentIp") + " - " + currentIp);
                    else
                        validIps.Add(ipWithMask);
                else
                    errors.Add(LocalizationService.GetResource("Admin.SettingsSystem.IpBlacklist.InvalidIp") + " - " + ipWithMask);
            }

            if (validIps.Count == 0)
                return errors;
            
            if (ipWhiteList != null)
                foreach (var ipWhite in ipWhiteList)
                {
                    var ipWithMaskInWhiteList = validIps.FirstOrDefault(ipWithMask => UrlService.IsInSubnet(ipWhite.Split('/')[0], ipWithMask) || UrlService.IsInSubnet(ipWithMask.Split('/')[0], ipWhite));
                    if (ipWithMaskInWhiteList == null)
                        continue;
                    validIps.Remove(ipWithMaskInWhiteList);
                    errors.Add(LocalizationService.GetResource("Admin.SettingsSystem.IpBlacklist.IpContainedInWhiteList") + " - " + ipWithMaskInWhiteList);
                }

            var ipList = SettingsGeneral.BannedIpList;
            foreach (var ip in ipList)
            {
                var existsIp = validIps.FirstOrDefault(ipWithMask => ipWithMask.Equals(ip, StringComparison.OrdinalIgnoreCase));
                if (existsIp == null)
                    continue;
                validIps.Remove(existsIp);
                errors.Add(LocalizationService.GetResource("Admin.SettingsSystem.IpBlacklist.AlreadyExistIp") + " - " + existsIp);
            }

            if (validIps.Count > 0)
            {
                ipList.AddRange(validIps);
                SettingsGeneral.BannedIpList = ipList;
            }
            return errors.Count == 0 ? null : errors;
        }
    }
}
