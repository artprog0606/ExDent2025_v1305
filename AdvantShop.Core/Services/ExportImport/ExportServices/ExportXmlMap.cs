//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.SEO;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;

namespace AdvantShop.ExportImport
{
    public class ExportXmlMap
    {
        private static readonly object SyncObject = new object();
        private static readonly List<string> _filesExported = new List<string>();

        private static List<string> FilesExported
        {
            get { lock (SyncObject) { return _filesExported; } }
        }

        private readonly string _filenameAndPath;
        private readonly string _prefUrl;
        private readonly bool _allowTags;

        private const string DefaultChangeFreq = "daily";

        private const float DefaultPriority = 0.5f;
        private const int MaxUrlCount = 5_000;
        private const int StepFileLength = 5_000;

        private List<SiteMapData> _moduleSiteMaps;
        
        private string _fileNameAndPathForDomain;

        public ExportXmlMap()
        {
            _filenameAndPath = SettingsGeneral.AbsolutePath + "sitemap.xml";;
            _prefUrl = SettingsMain.SiteUrl + "/";
            _allowTags = !SaasDataService.IsSaasEnabled ||
                          (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.HaveTags);
            _fileNameAndPathForDomain = null;
        }

        public string Create()
        {
            try
            {
                if (FilesExported.Contains(_filenameAndPath, StringComparer.OrdinalIgnoreCase))
                    return null;

                FilesExported.Add(_filenameAndPath);

                var path = Path.GetDirectoryName(_filenameAndPath);
                if (path == null) return null;
                FileHelpers.CreateDirectory(path);

                DeleteOldFiles(path);
                GenerateSiteMap();

                FilesExported.Remove(_filenameAndPath);
            }
            catch (Exception ex)
            {
                if (FilesExported.Contains(_filenameAndPath, StringComparer.OrdinalIgnoreCase))
                    FilesExported.Remove(_filenameAndPath);

                Debug.Log.Error(TaskManager.TaskManagerInstance().GetTasks(), ex);
                return null;
            }
            return _filenameAndPath;
        }
        
        public string Create(int domainId, string domain)
        {
            _fileNameAndPathForDomain = SettingsGeneral.AbsolutePath + $"{domain.Split('.')[0]}_sitemap.xml";
            
            try
            {
                if (FilesExported.Contains(_fileNameAndPathForDomain, StringComparer.OrdinalIgnoreCase))
                    return null;

                FilesExported.Add(_fileNameAndPathForDomain);

                var path = Path.GetDirectoryName(_fileNameAndPathForDomain);
                if (path == null) return null;
                FileHelpers.CreateDirectory(path);

                GenerateSiteMap(domainId, _fileNameAndPathForDomain);

                FilesExported.Remove(_fileNameAndPathForDomain);
            }
            catch (Exception ex)
            {
                if (FilesExported.Contains(_fileNameAndPathForDomain, StringComparer.OrdinalIgnoreCase))
                    FilesExported.Remove(_fileNameAndPathForDomain);

                Debug.Log.Error(TaskManager.TaskManagerInstance().GetTasks(), ex);
                return null;
            }
            return _fileNameAndPathForDomain;
        }

        private void DeleteOldFiles(string path)
        {
            var dir = new DirectoryInfo(path);
            foreach (var item in dir.GetFiles())
            {
                if (item.Name.Contains("sitemap") && item.Name.Contains(".xml"))
                {
                    FileHelpers.DeleteFile(item.FullName);
                }
            }
        }

        private int GetCount(int? domainId = null)
        {
            var totalCount = 0;
            totalCount += SQLDataAccess.ExecuteScalar<int>(
                "SELECT Count([CategoryId]) FROM [Catalog].[Category] WHERE [Enabled] = 1 and HirecalEnabled=1 and CategoryID <> 0",
                CommandType.Text);
            totalCount += SQLDataAccess.ExecuteScalar<int>(
                "SELECT Count([Product].[ProductID]) " +
                " FROM [Catalog].[Product] INNER JOIN [Catalog].[ProductCategories] ON [Catalog].[Product].[ProductID] = [Catalog].[ProductCategories].[ProductID]" +
                " INNER JOIN [Catalog].[Category] ON [Catalog].[Category].[CategoryID] = [Catalog].[ProductCategories].[CategoryID] AND [Catalog].[Category].[Enabled] = 1 and Main=1 " +
                " WHERE [Product].[Enabled] = 1 and [Product].[CategoryEnabled]=1 and (select Count(CategoryID) from Catalog.ProductCategories where ProductID=[Product].[ProductID]) <> 0;",
                CommandType.Text);
            totalCount += SQLDataAccess.ExecuteScalar<int>(
                "SELECT Count([NewsID]) FROM [Settings].[News] where AddingDate >= GetDate() AND Enabled = 1",
                CommandType.Text);
            totalCount += SQLDataAccess.ExecuteScalar<int>(
                "SELECT Count([StaticPageID]) FROM [CMS].[StaticPage] where IndexAtSiteMap=1 and enabled=1",
                CommandType.Text);
            totalCount += SQLDataAccess.ExecuteScalar<int>(
                "SELECT Count([BrandID]) FROM [Catalog].[Brand] where enabled=1",
                CommandType.Text);
            totalCount += _allowTags ? SQLDataAccess.ExecuteScalar<int>(
                @"SELECT COUNT(CategoryId)
                    FROM [Catalog].[Tag] inner join [Catalog].[TagMap] on [TagMap].[TagId] = [Tag].[Id]
                    inner join Catalog.Category on Category.CategoryId = TagMap.ObjId
                    where TagMap.Type = 'Category' AND Tag.Enabled = 1 AND Category.Enabled = 1 AND Category.HirecalEnabled = 1",
                CommandType.Text) : 0;

            totalCount += GetModuleSiteMaps().Count;

            return totalCount;
        }

        private List<SiteMapData> GetData(int domainId = 0)
        {
            var sb = new StringBuilder();
            sb.Append("SELECT [CategoryId] as Id, [UrlPath], 'category' as Type, GetDate() as Lastmod FROM [Catalog].[Category] WHERE [Enabled] = 1 and HirecalEnabled =1");
            
            sb.Append(" union ");
            sb.Append(
                "SELECT [Product].[ProductID] as Id , [Product].[UrlPath], 'product' as Type ,[Product].[DateModified] as Lastmod FROM [Catalog].[Product] " +
                "INNER JOIN [Catalog].[ProductCategories] ON [Catalog].[Product].[ProductID] = [Catalog].[ProductCategories].[ProductID] and Main=1" +
                "INNER JOIN [Catalog].[Category] ON [Catalog].[Category].[CategoryID] = [Catalog].[ProductCategories].[CategoryID]" +
                "AND [Catalog].[Category].[Enabled] = 1 WHERE [Product].[Enabled] = 1 and [Product].[CategoryEnabled] = 1");

            sb.Append(" union ");
            sb.Append("SELECT [NewsID] as Id, News.[UrlPath], 'news' as Type , [AddingDate] as LastMod FROM [Settings].[News] where AddingDate <= GetDate() AND Enabled = 1");

            sb.Append(" union ");
            sb.Append("SELECT [StaticPageID] as Id, [UrlPath], 'page' as Type, [ModifyDate] as Lastmod FROM [CMS].[StaticPage] where IndexAtSiteMap=1 and enabled=1");

            sb.Append(" union ");
            sb.Append("SELECT [BrandID] as Id, [UrlPath], 'brand' as Type, GetDate() as Lastmod FROM [Catalog].[Brand] where enabled=1");

            if (SettingsCatalog.BestEnabled)
            {
                sb.Append(" union ");
                sb.Append("SELECT 0 as Id, 'productlist/best' as UrlPath, 'other' as Type, GetDate() as Lastmod");
            }

            if (SettingsCatalog.NewEnabled)
            {
                sb.Append(" union ");
                sb.Append("SELECT 0 as Id, 'productlist/new' as UrlPath, 'other' as Type, GetDate() as Lastmod");
            }

            if (SettingsCatalog.SalesEnabled)
            {
                sb.Append(" union ");
                sb.Append("SELECT 0 as Id, 'productlist/sale' as UrlPath, 'other' as Type, GetDate() as Lastmod");
            }

            sb.Append(" union ");
            sb.Append("SELECT Id as Id, 'productlist/list/' + CONVERT(nvarchar, Id) as UrlPath, 'other' as Type, GetDate() as Lastmod from Catalog.ProductList WHERE Enabled = 1");

            if (_allowTags)
            {
                sb.Append(" union ");
                sb.Append(
                    @"SELECT 
                        Category.CategoryId as Id, 
                        ('categories/' + Category.UrlPath + '/tag/' + Tag.UrlPath) as UrlPath,
                        'categorytag' as Type,
                        GetDate() as Lastmod
                    FROM [Catalog].[Tag] inner join [Catalog].[TagMap] on [TagMap].[TagId] = [Tag].[Id]
                    inner join Catalog.Category on Category.CategoryId = TagMap.ObjId
                    where TagMap.Type = 'Category' AND Tag.Enabled = 1 AND Category.Enabled = 1 AND Category.HirecalEnabled = 1");
            }

            var items = SQLDataAccess.ExecuteReadList(sb.ToString(), CommandType.Text, GetSiteMapDataFromReader);

            var moduleItems = GetModuleSiteMaps();
            if (moduleItems.Count > 0)
                items.AddRange(moduleItems);

            return items;
        }

        private void GenerateSiteMap()
        {
            var totalCount = GetCount();
            var data = GetData();
            if (totalCount > MaxUrlCount)
            {
                int intervals = totalCount / StepFileLength;
                if (totalCount % StepFileLength > 0)
                    intervals += 1;
                CreateMultipleXml(intervals, _filenameAndPath, data);
            }
            else
            {
                CreateSimpleXml(_filenameAndPath, data);
            }
        }
        
        private void GenerateSiteMap(int domainId, string fileNameAndPathForDomain)
        {
            var totalCount = GetCount(domainId);
            var data = GetData(domainId);
            if (totalCount > MaxUrlCount)
            {
                int intervals = totalCount / StepFileLength;
                if (totalCount % StepFileLength > 0)
                    intervals += 1;
                CreateMultipleXml(intervals, fileNameAndPathForDomain, data);
            }
            else
            {
                CreateSimpleXml(fileNameAndPathForDomain, data);
            }
        }

        private void CreateMultipleXml(int intervals, string strFinalFilePath, List<SiteMapData> data)
        {
            CreateXmlMap(intervals, strFinalFilePath);
            CreateXmlFiles(intervals, strFinalFilePath, data);
        }

        private void CreateXmlFiles(int intervals, string strFinalFilePath, List<SiteMapData> data)
        {
            var fileName = strFinalFilePath.Replace(".xml", "");
            for (int i = 0; i < intervals; i++)
            {
                var temp = data.Skip(StepFileLength * i).Take(StepFileLength);
                WriteFile($"{fileName}_{i}.xml", temp, i == 0);
            }
        }

        /// <summary>
        /// Файл с сылками на xml-файлы
        /// </summary>
        private void CreateXmlMap(int intervals, string filePath)
        {
            var fileName = filePath.Replace(".xml", "").Split('\\').Last();
            var now = DateTime.Now.ToString("yyyy-MM-dd");
            
            using (var outputFile = new StreamWriter(filePath, false, new UTF8Encoding(false)))
            {
                using (var writer = XmlWriter.Create(outputFile))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("sitemapindex", "http://www.sitemaps.org/schemas/sitemap/0.9");

                    for (int i = 0; i < intervals; i++)
                    {
                        writer.WriteStartElement("sitemap");

                        writer.WriteStartElement("loc");
                        writer.WriteString($"{_prefUrl}{fileName}_{i}.xml");
                        writer.WriteEndElement();

                        writer.WriteStartElement("lastmod");
                        writer.WriteString(now);
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        private void CreateSimpleXml(string fileName, List<SiteMapData> data)
        {
            WriteFile(fileName, data);
        }


        private void WriteFile(string filePath, IEnumerable<SiteMapData> data, bool isFirst = true)
        {
            var now = DateTime.Now.ToString("yyyy-MM-dd");
            
            using (var outputFile = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var writer = XmlWriter.Create(outputFile))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                    if (isFirst) // adding link to main page
                    {
                        WriteLine(writer,
                            new SiteMapData()
                            {
                                Loc = SettingsMain.SiteUrl,
                                Changefreq = DefaultChangeFreq,
                                Priority = 1
                            }, 
                            now);
                    }

                    foreach (var item in data)
                    {
                        WriteLine(writer, item, now);
                    }
                }
            }
        }

        private void WriteLine(XmlWriter writer, SiteMapData item, string now)
        {
            writer.WriteStartElement("url");
            // start url

            writer.WriteStartElement("loc");
            writer.WriteString(item.IsBaseUrl ? _prefUrl + item.Loc : item.Loc);
            writer.WriteEndElement();

            writer.WriteStartElement("lastmod");
            writer.WriteString(item.Lastmod != null && item.Lastmod != DateTime.MinValue
                ? item.Lastmod.Value.ToString("yyyy-MM-dd")
                : now);
            writer.WriteEndElement();

            writer.WriteStartElement("changefreq");
            writer.WriteString(item.Changefreq.IsNullOrEmpty() ? DefaultChangeFreq : item.Changefreq);
            writer.WriteEndElement();

            writer.WriteStartElement("priority");
            writer.WriteString(item.Priority.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // end url
            writer.WriteEndElement();
        }

        private SiteMapData GetSiteMapDataFromReader(SqlDataReader reader)
        {
            var siteMapData = new SiteMapData(true)
            {
                Changefreq = DefaultChangeFreq,
                Priority = DefaultPriority
            };

            switch (SQLDataHelper.GetString(reader, "Type"))
            {
                case "category":
                    siteMapData.Loc = SQLDataHelper.GetInt(reader, "Id") != 0 
                        ? UrlService.GetLink(ParamType.Category, SQLDataHelper.GetString(reader, "UrlPath")) 
                        : SQLDataHelper.GetString(reader, "UrlPath");
                    siteMapData.Priority = 0.9f;
                    break;
                case "product":
                    siteMapData.Loc = UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(reader, "UrlPath"));
                    siteMapData.Lastmod = SQLDataHelper.GetDateTime(reader, "Lastmod");
                    siteMapData.Priority = 0.8f;
                    break;
                case "news":
                    siteMapData.Loc = UrlService.GetLink(ParamType.News, SQLDataHelper.GetString(reader, "UrlPath"));
                    siteMapData.Lastmod = SQLDataHelper.GetDateTime(reader, "Lastmod");
                    break;
                case "page":
                    siteMapData.Loc = UrlService.GetLink(ParamType.StaticPage, SQLDataHelper.GetString(reader, "UrlPath"));
                    siteMapData.Lastmod = SQLDataHelper.GetDateTime(reader, "Lastmod");
                    siteMapData.Priority = 0.9f;
                    break;
                case "brand":
                    siteMapData.Loc = UrlService.GetLink(ParamType.Brand, SQLDataHelper.GetString(reader, "UrlPath"));
                    siteMapData.Priority = 0.9f;
                    break;
                case "categorytag":
                    siteMapData.Loc = SQLDataHelper.GetString(reader, "UrlPath");
                    siteMapData.Priority = 0.9f;
                    break;
                case "other":
                    siteMapData.Loc = SQLDataHelper.GetString(reader, "UrlPath");
                    break;
            }

            return siteMapData;
        }

        private List<SiteMapData> GetModuleSiteMaps()
        {
            if (_moduleSiteMaps != null)
                return _moduleSiteMaps;

            _moduleSiteMaps = new List<SiteMapData>();

            var modules = AttachedModules.GetModules<ISiteMap>();
            if (modules.Count > 0)
            {
                foreach (var cls in modules)
                {
                    var classInstance = (ISiteMap) Activator.CreateInstance(cls, null);
                    var moduleSiteMapItems = classInstance.GetData();

                    if (moduleSiteMapItems != null && moduleSiteMapItems.Count > 0)
                        _moduleSiteMaps.AddRange(moduleSiteMapItems);
                }
            }

            return _moduleSiteMaps;
        }
    }
}