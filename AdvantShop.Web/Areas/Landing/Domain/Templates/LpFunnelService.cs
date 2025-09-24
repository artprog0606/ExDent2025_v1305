using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop.App.Landing.Domain.Templates
{
    public class LpFunnelService
    {
        public List<LpFunnelModel> GetFunnelTypes()
        {
            return CacheManager.Get("funnelTypes", () =>
            {
                var funnelTypes = new List<LpFunnelModel>();

                try
                {
                    var filePath = HostingEnvironment.MapPath(LpFiles.LpStaticPath + "funnelTypes.json");
                    if (!File.Exists(filePath))
                        return funnelTypes;

                    var content = "";

                    using (var sr = new StreamReader(filePath))
                        content = sr.ReadToEnd();

                    funnelTypes = JsonConvert.DeserializeObject<List<LpFunnelModel>>(content);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }

                return funnelTypes;
            });
        }
    }
}
