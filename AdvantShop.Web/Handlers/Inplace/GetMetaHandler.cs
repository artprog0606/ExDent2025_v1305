using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.SEO;

namespace AdvantShop.Handlers.Inplace
{
    public class GetMetaHandler
    {
        public object Execute(int id, MetaType type)
        {
            var metaInfo = MetaInfoService.GetMetaInfo(id, type);

            string globalVaribles = string.Empty;
            string nameFieldTitle = string.Empty;
            bool hasNameField = true;
            string queryName = string.Empty;

            switch (type)
            {
                case MetaType.Brand:
                    globalVaribles = LocalizationService.GetResource("UseGlobalVariables") + " #STORE_NAME#, #BRAND_NAME#, #PAGE#, #GEO_NAME#";
                    nameFieldTitle = LocalizationService.GetResource("Admin.BrandName");
                    queryName = "SELECT BrandName FROM Catalog.Brand Where BrandID=@objId";
                    break;
                case MetaType.Category:
                    globalVaribles = LocalizationService.GetResource("UseGlobalVariables") + "#STORE_NAME#, #CATEGORY_NAME#, #PAGE#, #TAGS#, #GEO_NAME#";
                    nameFieldTitle = LocalizationService.GetResource("Admin.CategoryName");
                    queryName = "SELECT [Name] FROM [Catalog].[Category] Where CategoryID=@objId";
                    break;
                case MetaType.News:
                    globalVaribles = LocalizationService.GetResource("UseGlobalVariables") + " #STORE_NAME#, #NEWS_NAME#, #GEO_NAME#";
                    nameFieldTitle = LocalizationService.GetResource("Admin.NewsTitle");
                    queryName = "SELECT [Title] FROM [Settings].[News] Where NewsID=@objId";
                    break;
                case MetaType.Product:
                    globalVaribles = LocalizationService.GetResource("UseGlobalVariables") + " #STORE_NAME#, #PRODUCT_NAME#, #CATEGORY_NAME#, #BRAND_NAME#, #PRICE#, #TAGS#, #GEO_NAME#";
                    nameFieldTitle = LocalizationService.GetResource("Admin.ProductName");
                    queryName = "SELECT [Name] FROM [Catalog].[Product] Where [ProductID]=@objId";
                    break;
				case MetaType.Tag:
					globalVaribles = LocalizationService.GetResource("UseGlobalVariables") + " #STORE_NAME#, #TAG_NAME#, #GEO_NAME#";
                    nameFieldTitle = LocalizationService.GetResource("Admin.TagName");
					queryName = "SELECT [Name] FROM [Catalog].[Tag] Where [Id]=@objId";
					break;
                case MetaType.StaticPage:
                    globalVaribles = LocalizationService.GetResource("UseGlobalVariables") + " #STORE_NAME#, #PAGE_NAME#, #GEO_NAME#";
                    nameFieldTitle = LocalizationService.GetResource("Admin.StaticPageName");
                    queryName = "SELECT [PageName] FROM [CMS].[StaticPage] Where [StaticPageID]=@objId";
                    break;
				case MetaType.MainPageProducts:
					globalVaribles = LocalizationService.GetResource("UseGlobalVariables") + " #STORE_NAME#,  #PAGE_NAME#, #PAGE#, #GEO_NAME#";
                    nameFieldTitle = LocalizationService.GetResource("Admin.ProductListsName");
                    hasNameField = id > 0;
					queryName = "SELECT [Name] FROM [Catalog].[ProductList] Where [Id]=@objId";
					break;
                case MetaType.MainPageBrands:
                    globalVaribles = LocalizationService.GetResource("UseGlobalVariables") + " #STORE_NAME#,  #PAGE#, #GEO_NAME#";
                    hasNameField = false;
                    break;
                case MetaType.PageWarehouses:
                    globalVaribles = LocalizationService.GetResource("UseGlobalVariables") + " #STORE_NAME#, #GEO_NAME#";
                    hasNameField = false;
                    break;
            }

            return metaInfo != null
                ? new
                {
                    metaInfo.H1,
                    metaInfo.MetaDescription,
                    metaInfo.MetaKeywords,
                    metaInfo.Title,
                    globalVaribles = globalVaribles,
                    HasNameField = hasNameField,
                    Name = hasNameField 
                        ? SQLDataAccess.ExecuteScalar(queryName, System.Data.CommandType.Text,
                            new System.Data.SqlClient.SqlParameter("@objId", id))
                        : string.Empty,
                    NameFieldTitle = nameFieldTitle,
                    UseDefaultMeta = false
                }
                : new
                {
                    H1 = string.Empty,
                    MetaDescription = string.Empty,
                    MetaKeywords = string.Empty,
                    Title = string.Empty,
                    globalVaribles = globalVaribles,
                    HasNameField = hasNameField,
                    Name = hasNameField
                        ? SQLDataAccess.ExecuteScalar(queryName, System.Data.CommandType.Text,
                            new System.Data.SqlClient.SqlParameter("@objId", id))
                        : string.Empty,
                    NameFieldTitle = nameFieldTitle,
                    UseDefaultMeta = true
                };
        }
    }
}