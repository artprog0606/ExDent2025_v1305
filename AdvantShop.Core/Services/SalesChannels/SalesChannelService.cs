using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Web.Hosting;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Core.Services.Landing;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.SalesChannels
{
    public static class SalesChannelService
    {
        public const string CacheKey = "SalesChannels_list";

        public static List<SalesChannel> GetList()
        {
            var items = CacheManager.Get(CacheKey + ModulesRepository.ModulesCachePrefix + Thread.CurrentThread.CurrentUICulture.Name, () =>
            {
                var result = new List<SalesChannel>();

                try
                {
                    var configContent = "";

                    using (var sr = new StreamReader(HostingEnvironment.MapPath("~/areas/admin/salesChannels.json")))
                        configContent = sr.ReadToEnd();

                    var configSalesChannels = JsonConvert.DeserializeObject<List<SalesChannelConfigModel>>(configContent);

                    foreach (var configItem in configSalesChannels)
                        result.Add(new SalesChannel(configItem));


                    // load from modules
                    var moduleChannels = ModulesService.GetModuleSalesChannels();
                    result.AddRange(moduleChannels);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }

                return result;
            });

            return items;
        }

        public static SalesChannel GetByType(ESalesChannelType type, string moduleStringId = null)
        {
            return GetList().FirstOrDefault(x =>
                x.Type == type &&
                (x.ModuleStringId == null && moduleStringId == null ||
                 (x.ModuleStringId != null && moduleStringId != null && x.ModuleStringId.ToLower() == moduleStringId.ToLower())));
        }

        public static void SetNotShowInstalled(string moduleStringId, bool value)
        {
            SettingProvider.Items["SalesChannel_NotShowInstalled_" + moduleStringId.ToLower()] = value.ToString();
        }

        public static bool GetNotShowInstalled(string moduleStringId)
        {
            return SettingProvider.Items["SalesChannel_NotShowInstalled_" + moduleStringId.ToLower()].TryParseBool();
        }

        public static void SetExcludedProductSalesChannel(string key, int productId)
        {
            SQLDataAccess.ExecuteNonQuery(
               "if (select count([ProductId]) from [CRM].[SalesChannelExcludedProduct] where ProductId=@ProductId and SalesChannelKey=@SalesChannelKey) = 0 " +
               "begin insert into [CRM].[SalesChannelExcludedProduct] ([ProductId], [SalesChannelKey]) values (@ProductId, @SalesChannelKey) end",
               CommandType.Text,
               new SqlParameter("@SalesChannelKey", key.ToString()),
               new SqlParameter("@ProductId", productId));
        }

        public static List<string> GetExcludedProductSalesChannelList(int productId)
        {
            return SQLDataAccess.ExecuteReadColumn<string>(
                  "select [SalesChannelKey] from [CRM].[SalesChannelExcludedProduct] Where [ProductId] = @ProductId",
                  CommandType.Text,
                  "SalesChannelKey",
                  new SqlParameter("@ProductId", productId));
        }

        public static void DeleteExcludedProductSalesChannels(int productId)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "delete from [CRM].[SalesChannelExcludedProduct] Where [ProductId] = @ProductId",
                 CommandType.Text,
                 new SqlParameter("@ProductId", productId));
        }

        public static void DeleteExcludedProductSalesChannels(int productId, List<string> channels)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Delete from [CRM].[SalesChannelExcludedProduct] Where [ProductId] = @ProductId and SalesChannelKey in (" + String.Join(",", "'" + channels + "'") + ")",
                 CommandType.Text,
                 new SqlParameter("@ProductId", productId));
        }

        public static Dictionary<string, int> GetExcludedSalesChannelsByProducts(List<int> ids)
        {
            return
                SQLDataAccess.ExecuteReadDictionary<string, int>(
                    "Select [SalesChannelKey], Count([SalesChannelKey]) as NumberEnable from Crm.SalesChannelExcludedProduct Where ProductId in (" + String.Join(",", ids) + ") Group by [SalesChannelKey]", CommandType.Text, "SalesChannelKey",
                    "NumberEnable");
        }

        public static bool IsFirstTimeCreateStore()
        {
            var store = SalesChannelService.GetByType(ESalesChannelType.Store);
            return !store.Enabled && new LpSiteService().GetList().Count == 0;
        }
    }
}