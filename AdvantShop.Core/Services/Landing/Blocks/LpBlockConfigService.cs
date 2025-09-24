using System;
using System.IO;
using System.Web.Hosting;
using AdvantShop.Core.Caching;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Landing.Blocks
{
    public class LpBlockConfigService
    {
        public LpBlockConfig Get(string blockKey, string templateName)
        {
            return CacheManager.Get(LpConstants.LpBlockConfigCachePrefix + blockKey + templateName, LpConstants.LpCacheTime,
                () =>
                {
                    try
                    {
                        var path = string.Format("{0}/{1}/Blocks/{2}/", LpFiles.TepmlateFolder, templateName, blockKey);
                        var configPath = HostingEnvironment.MapPath(path + "config.json");

                        if (!File.Exists(configPath))
                        {
                            path = string.Format("{0}/Blocks/{1}/", LpFiles.ViewsFolder, blockKey);
                            configPath = HostingEnvironment.MapPath(path + "config.json");
                            if (!File.Exists(configPath))
                                return null;
                        }

                        var configContent = "";

                        using (var sr = new StreamReader(configPath))
                            configContent = sr.ReadToEnd();

                        var blockConfig = JsonConvert.DeserializeObject<LpBlockConfig>(configContent);
                        if (blockConfig != null)
                            blockConfig.BlockPath = path + blockKey + ".cshtml";

                        return blockConfig;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }

                    return null;
                });
        }
    }
}
