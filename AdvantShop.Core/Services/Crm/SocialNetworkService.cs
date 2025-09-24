using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Saas;

namespace AdvantShop.Core.Services.Crm
{
    public class SocialNetworkService
    {
        public static bool SaveAvatar(Customer customer, string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl) || !photoUrl.StartsWith("http"))
                return false;

            if (customer == null || !string.IsNullOrEmpty(customer.Avatar))
                return false;

            try
            {
                var ext = Path.GetExtension(photoUrl.Split('?')[0]);
                if (string.IsNullOrEmpty(ext))
                    ext = ".jpg";

                var newFileName = customer.Id + "_" + DateTime.Now.ToString("yyMMddhhmmss") + ext;
                var newFilePath = FoldersHelper.GetPathAbsolut(FolderType.Avatar, newFileName);

                if (FileHelpers.DownloadRemoteImageFile(photoUrl, newFilePath))
                {
                    customer.Avatar = newFileName;
                    CustomerService.UpdateCustomer(customer);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return false;
        }

        public static bool IntegrationsLimitRiched()
        {
            var integrationsLimit = SaasDataService.CurrentSaasData.CRMIntegrationsCount;
            if (!SaasDataService.IsSaasEnabled || integrationsLimit <= 0)
                return false;
            var count = new List<bool>
            {
                Instagram.Instagram.Instance.IsActive(),
                new Facebook.FacebookApiService().IsActive(),
                VkApiService.IsVkActive(),
                new Telegram.TelegramApiService().IsActive(),
                Ok.OkApiService.IsActive(),
            }.Count(x => x);

            return count >= integrationsLimit;
        }

        public static int GetIntegrationsCount()
        {
            if (!SaasDataService.IsSaasEnabled)
                return 0;
            return new List<bool>
            {
                Instagram.Instagram.Instance.IsActive(),
                new Facebook.FacebookApiService().IsActive(),
                VkApiService.IsVkActive(),
                new Telegram.TelegramApiService().IsActive(),
                Ok.OkApiService.IsActive(),
            }.Count(x => x);
        }
    }
}
