using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.ExportImport;
using AdvantShop.SiteMaps;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    public class UpdateSiteMapsHandler
    {
        public CommandResult Execute()
        {
            var result = new CommandResult { Result = false };

            var htmlMapPath =  new ExportHtmlMap().Create();
            var xmlMapPath = new ExportXmlMap().Create();

            var LastWriteTime = string.Empty;

            if (File.Exists(xmlMapPath))
            {
                LastWriteTime = Localization.Culture.ConvertDate(new FileInfo(xmlMapPath).LastWriteTime);
                result.Result = true;
            }

            if (File.Exists(htmlMapPath))
            {
                LastWriteTime = Localization.Culture.ConvertDate(new FileInfo(htmlMapPath).LastWriteTime);
                result.Result = true;
            }
            
            var resultList = new List<SitemapInfoModel>();
            
            resultList.Add(new SitemapInfoModel( SettingsMain.SiteUrl + "/sitemap.html", SettingsMain.SiteUrl + "/sitemap.xml"));

            var domains = DomainSiteMapsService.GetSitemapDomainModels();
            
            foreach (var domain in domains)
            {
                var htmlMapPathDomain =  new ExportHtmlMap().Create(domain.Id, domain.Url);
                var xmlMapPathDomain = new ExportXmlMap().Create(domain.Id, domain.Url);

                var url = Uri.EscapeDataString(domain.Url).Split('.')[0];

                if (File.Exists(xmlMapPathDomain))
                {
                    LastWriteTime = Localization.Culture.ConvertDate(new FileInfo(xmlMapPathDomain).LastWriteTime);
                }

                if (File.Exists(htmlMapPathDomain))
                {
                    LastWriteTime = Localization.Culture.ConvertDate(new FileInfo(htmlMapPathDomain).LastWriteTime);
                }
            
                resultList.Add(new SitemapInfoModel("http://" + Uri.EscapeDataString(domain.Url) + $"/{url}_sitemap.html", "http://" + Uri.EscapeDataString(domain.Url) + $"/{url}_sitemap.xml"));
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
