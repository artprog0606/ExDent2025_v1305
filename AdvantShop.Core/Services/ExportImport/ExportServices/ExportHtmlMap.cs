//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;

namespace AdvantShop.ExportImport
{
    public class ExportHtmlMap
    {
        private static readonly object SyncObject = new object();
        private static readonly List<string> _filesExported = new List<string>();

        private static List<string> FilesExported
        {
            get { lock (SyncObject) { return _filesExported; }}
        }

        private readonly string _fileNameAndPath;
        private string _prefUrl;
        private string _fileNameAndPathForDomain;
        private string _prefUrlForDomain;
        private StreamWriter _sw;

        private class ExportHtmlItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string UrlPath { get; set; }
        }
        
        private class ExportHtmlNewsItem : ExportHtmlItem
        {
            public DateTime Date { get; set; }
        }
        
        private class ExportHtmlItemcategoryTag
        {
            public int CategoryId { get; set; }
            public string TagName { get; set; }
            public string TagUrlPath { get; set; }
            public string CategoryName { get; set; }
            public string CategoryUrlPath { get; set; }
        }

        public ExportHtmlMap()
        {
            _fileNameAndPath = SettingsGeneral.AbsolutePath + "sitemap.html";
            _prefUrl = SettingsMain.SiteUrl + "/";
            _fileNameAndPathForDomain = null;
            _prefUrlForDomain = string.Empty;
        }

        public string Create()
        {
            try
            {
                if (FilesExported.Contains(_fileNameAndPath, StringComparer.OrdinalIgnoreCase))
                    return null;
                FilesExported.Add(_fileNameAndPath);

                var path = Path.GetDirectoryName(_fileNameAndPath);
                if (path == null) return null;

                FileHelpers.CreateDirectory(path);
                FileHelpers.DeleteFile(_fileNameAndPath);

                _prefUrl = _prefUrl.Contains("http://") || _prefUrl.Contains("https://") ? _prefUrl : "http://" + _prefUrl;

                using (var fs = new FileStream(_fileNameAndPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (_sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        _sw.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>" + _prefUrl + " - " +
                                      LocalizationService.GetResource("Core.ExportImport.ExportHtmlMap.SiteMapGenerateHeader") +
                                      "</title></head><body><div>");

                        _sw.WriteLine("<b><a href='{0}'>{0}</a></b><br/><br/>", SettingsMain.SiteUrl);
                        CreateStaticPages();
                        GetCategories();
                        CreateNews();
                        CreateBrands();
                        
                        if (!SaasDataService.IsSaasEnabled ||
                            (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.HaveTags))
                        {
                            CreateCategoryTags();
                        }

                        CreateModules();

                        _sw.WriteLine("</div></body></html>");

                        _sw.Close();
                    }
                }

                FilesExported.Remove(_fileNameAndPath);
            }
            catch (Exception ex)
            {
                if (FilesExported.Contains(_fileNameAndPath, StringComparer.OrdinalIgnoreCase))
                    FilesExported.Remove(_fileNameAndPath);

                Debug.Log.Error("ExportHtmlMap", ex);
                return null;
            }
            return _fileNameAndPath;
        }
        
        public string Create(int domainId, string domain)
        {
            _fileNameAndPathForDomain = SettingsGeneral.AbsolutePath + $"{domain.Split('.')[0]}_sitemap.html";
            _prefUrlForDomain = domain + $"/{domain.Split('.')[0]}_sitemap.html";
            
            try
            {
                if (FilesExported.Contains(_fileNameAndPathForDomain, StringComparer.OrdinalIgnoreCase))
                    return null;
                FilesExported.Add(_fileNameAndPathForDomain);

                var path = Path.GetDirectoryName(_fileNameAndPathForDomain);
                if (path == null) return null;

                FileHelpers.CreateDirectory(path);
                FileHelpers.DeleteFile(_fileNameAndPathForDomain);

                _prefUrlForDomain = _prefUrlForDomain.Contains("http://") || _prefUrlForDomain.Contains("https://") ? _prefUrlForDomain : "http://" + _prefUrlForDomain;

                using (var fs = new FileStream(_fileNameAndPathForDomain, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (_sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        _sw.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>" + _prefUrlForDomain + " - " +
                                      LocalizationService.GetResource("Core.ExportImport.ExportHtmlMap.SiteMapGenerateHeader") +
                                      "</title></head><body><div>");

                        _sw.WriteLine("<b><a href='{0}'>{0}</a></b><br/><br/>", _prefUrlForDomain);
                        CreateStaticPages();
                        GetCategories();
                        CreateNews();
                        CreateBrands();
                        
                        if (!SaasDataService.IsSaasEnabled ||
                            (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.HaveTags))
                        {
                            CreateCategoryTags();
                        }

                        CreateModules();

                        _sw.WriteLine("</div></body></html>");

                        _sw.Close();
                    }
                }

                FilesExported.Remove(_fileNameAndPathForDomain);
            }
            catch (Exception ex)
            {
                if (FilesExported.Contains(_fileNameAndPathForDomain, StringComparer.OrdinalIgnoreCase))
                    FilesExported.Remove(_fileNameAndPathForDomain);

                Debug.Log.Error("ExportHtmlMap", ex);
                return null;
            }
            return _fileNameAndPathForDomain;
        }

        private void CreateStaticPages(int? domainId = null)
        {
            var staticPages = SQLDataAccess.Query<ExportHtmlItem>(
                    "SELECT [PageName] as Name, [UrlPath] FROM [CMS].[StaticPage] WHERE [IndexAtSiteMap] = 1 and enabled=1 ORDER BY [SortOrder]")
                .ToList();
            
            if (staticPages.Count == 0)
                return;
            
            _sw.WriteLine("<b>" + LocalizationService.GetResource("Core.ExportImport.ExportHtmlMap.StaticPages") + " </b> <ul>");

            foreach (var staticPage in staticPages)
            {
                _sw.WriteLine("<li><a href='{0}{1}'>{2}</a></li>",
                    _prefUrl, UrlService.GetLink(ParamType.StaticPage, staticPage.UrlPath), 
                    staticPage.Name);
            }
            
            _sw.WriteLine("</ul>");
        }
        
        private void GetCategories(int categoryId = 0, int level = 0)
        {
            if (level >= 15) // something wrong, mb stackoverflow ex
                return;

            level++;

            var categories =
                SQLDataAccess.Query<ExportHtmlItem>(
                    "SELECT [CategoryID] as Id, [Name], [UrlPath] FROM [Catalog].[Category] " + 
                    "WHERE [Enabled] = 1 and HirecalEnabled=1 and ParentCategory=@categoryId and CategoryID<>0 AND [Products_Count] > 0 " +
                    "ORDER BY [SortOrder]",
                    new {categoryId}).ToList();

            if (categoryId == 0 && categories.Count > 0)
                _sw.WriteLine("<b>" + LocalizationService.GetResource("Core.ExportImport.ExportHtmlMap.Catalog") + "</b>");

            if (categories.Count == 0) 
                return;
            
            _sw.WriteLine("<ul>");

            foreach (var category in categories)
            {
                _sw.WriteLine("<li>");
                _sw.WriteLine("<a href='{0}{1}'>{2}</a>",
                    _prefUrl, UrlService.GetLink(ParamType.Category, category.UrlPath),
                    HttpUtility.HtmlEncode(category.Name));

                GetCategories(category.Id, level);
                GetProducts(category.Id);

                _sw.WriteLine("</li>");
            }

            _sw.WriteLine("</ul>");
        }

        private void GetProducts(int categoryId)
        {
            var products = SQLDataAccess.Query<ExportHtmlItem>(
                "SELECT Product.[Name] as Name, [UrlPath] " +
                "FROM [Catalog].[ProductCategories] " +
                "INNER JOIN [Catalog].[Product] ON [Product].ProductID = ProductCategories.ProductID " +
                "WHERE CategoryEnabled =1 and [Enabled] = 1 and ProductCategories.Main = 1 and CategoryID=@categoryId",
                new {categoryId})
                .ToList();
            
            if (products.Count == 0)
                return;
            
            _sw.WriteLine("<ul>");

            foreach (var product in products)
            {
                _sw.WriteLine("<li><a href='{0}{1}'>{2}</a></li>",
                    _prefUrl, UrlService.GetLink(ParamType.Product, product.UrlPath), 
                    HttpUtility.HtmlEncode(product.Name));
            }
            
            _sw.WriteLine("</ul>");
        }

        private void CreateNews()
        {
            var news = SQLDataAccess.Query<ExportHtmlNewsItem>(
                    "SELECT [Title] as Name, [UrlPath], AddingDate as Date FROM [Settings].[News] where AddingDate <= GetDate() AND Enabled = 1 ORDER BY AddingDate DESC")
                .ToList();

            if (news.Count == 0) 
                return;
            
            _sw.WriteLine("<b>" + LocalizationService.GetResource("Core.ExportImport.ExportHtmlMap.News") + " </b> <ul>");

            foreach (var item in news)
            {
                _sw.WriteLine("<li><a href='{0}{1}'>{2} :: {3}</a></li>",
                    _prefUrl, UrlService.GetLink(ParamType.News, item.UrlPath), 
                    item.Date, 
                    item.Name);
            }
                
            _sw.WriteLine("</ul>");
        }

        private void CreateBrands()
        {
            var brands = SQLDataAccess.Query<ExportHtmlItem>(
                    "SELECT [BrandName] as Name, [UrlPath] FROM [Catalog].[Brand] Where enabled=1 ORDER BY BrandName")
                .ToList();

            if (brands.Count == 0) 
                return;
            
            _sw.WriteLine("<b>" + LocalizationService.GetResource("Core.ExportImport.ExportHtmlMap.Brands") + " </b> <ul>");

            foreach (var brand in brands)
            {
                _sw.WriteLine("<li><a href='{0}{1}'>{2}</a></li>",
                    _prefUrl, UrlService.GetLink(ParamType.Brand, brand.UrlPath),
                    brand.Name);
            }
            
            _sw.WriteLine("</ul>");
        }
        
        private void CreateCategoryTags()
        {
            var categoryTags = SQLDataAccess.Query<ExportHtmlItemcategoryTag>(
                    @"SELECT Tag.Name as TagName, Tag.UrlPath as TagUrlPath, Category.CategoryId, Category.Name as CategoryName, Category.UrlPath as CategoryUrlPath
                  FROM [Catalog].[Tag] 
                  inner join [Catalog].[TagMap] on [TagMap].[TagId] = [Tag].[Id] 
                  inner join Catalog.Category on Category.CategoryId = TagMap.ObjId 
                  where TagMap.Type = @tagType AND Tag.Enabled = 1 AND Category.Enabled = 1 AND Category.HirecalEnabled = 1",
                    new {tagType = ETagType.Category.ToString()})
                .ToList();

            if (categoryTags.Count == 0) 
                return;
            
            _sw.WriteLine("<b>" + LocalizationService.GetResource("Core.ExportImport.ExportHtmlMap.Tags") + " </b> <ul>");

            foreach (var categoryTag in categoryTags)
            {
                _sw.WriteLine("<li><a href='{0}{1}/tag/{2}'>{3}, {4}</a></li>",
                    _prefUrl, UrlService.GetLink(ParamType.Category, categoryTag.CategoryUrlPath, categoryTag.CategoryId),
                    categoryTag.TagUrlPath,
                    categoryTag.CategoryName, categoryTag.TagName);
            }
            
            _sw.WriteLine("</ul>");
        }


        private void CreateModules()
        {
            var modules = AttachedModules.GetModules<ISiteMap>();
            if (modules.Count == 0)
                return;
            
            foreach (var cls in modules)
            {
                var classInstance = (ISiteMap)Activator.CreateInstance(cls, null);

                var data = classInstance.GetData();
                if (data == null || data.Count == 0)
                    continue;
                
                _sw.WriteLine("<b>" + classInstance.ModuleName + " </b> <ul>");

                foreach (var item in data)
                {
                    _sw.WriteLine("<li><a href='{0}'>{1}</a></li>", item.Loc, item.Title);
                }
                _sw.WriteLine("</ul>");
            }
        }
    }
}