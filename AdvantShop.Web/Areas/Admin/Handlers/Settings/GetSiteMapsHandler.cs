using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.ExportImport;
using AdvantShop.SiteMaps;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Admin.Handlers.Settings
{
    public class GetSiteMapsHandler
    {
        public CommandResult Execute()
        {
            var result = new CommandResult { Result = false };
            
            var resultList = new List<SitemapInfoModel>();

            var htmlMapPath =  SettingsGeneral.AbsolutePath + "sitemap.html";
            var xmlMapPath = SettingsGeneral.AbsolutePath + "sitemap.xml";
            
            var LastWriteTime = string.Empty;

            if (File.Exists(xmlMapPath) && File.Exists(htmlMapPath))
            {
                LastWriteTime = File.GetLastWriteTime(xmlMapPath).ToString("yyyy-MM-dd HH:mm");
                
                resultList.Add(new SitemapInfoModel(SettingsMain.SiteUrl + "/sitemap.html", SettingsMain.SiteUrl + "/sitemap.xml"));
            }
            
            var domains = DomainSiteMapsService.GetSitemapDomainModels();
            
            foreach (var domain in domains)
            {
                var htmlMapPathDomain =  SettingsGeneral.AbsolutePath + $"{domain.Url.Split('.')[0]}_sitemap.html";
                var xmlMapPathDomain = SettingsGeneral.AbsolutePath + $"{domain.Url.Split('.')[0]}_sitemap.xml";

                var url = Uri.EscapeDataString(domain.Url).Split('.')[0];

                if (File.Exists(xmlMapPathDomain) && File.Exists(htmlMapPathDomain))
                {
                    LastWriteTime = File.GetLastWriteTime(xmlMapPathDomain).ToString("yyyy-MM-dd HH:mm");
                    
                    resultList.Add(new SitemapInfoModel("http://" + Uri.EscapeDataString(domain.Url) + $"/{url}_sitemap.html", "http://" + Uri.EscapeDataString(domain.Url) + $"/{url}_sitemap.xml"));
                }
            }
            
            result.Obj = resultList;

            if (resultList.Count > 0)
            {
                result.Message = LastWriteTime;
            }

            return result;
        }
    }
}