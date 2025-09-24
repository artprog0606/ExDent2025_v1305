using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.SEO;

namespace AdvantShop.ExportImport
{
    public class ExportFeedCsvCategoryService
    {
        private const string Separator = ";";

        public static ExportFeedCsvCategory GetCsvCategoriesFromReader(SqlDataReader reader, List<CategoryFields> fieldMapping, string propertySeparator, string nameSameProductProperty, string nameNotSameProductProperty)
        {
            var category = CategoryService.GetCategoryFromReader(reader);

            var categoryCsv = new ExportFeedCsvCategory()
            {
                CategoryId = category.CategoryId.ToString(),
                ExternalId = category.ExternalId.ToString(),
                Name = category.Name,
                Slug = category.UrlPath,
                ParentCategory = category.ParentCategoryId.ToString(),
                SortOrder = category.SortOrder.ToString(),
                Enabled = category.Enabled ? "+" : "-",
                Hidden = category.Hidden ? "+" : "-",
                BriefDescription = category.BriefDescription,
                Description = category.Description,
                DisplayStyle = category.DisplayStyle.ToString(),
                Sorting = category.Sorting.ToString(),
                DisplayBrandsInMenu = category.DisplayBrandsInMenu ? "+" : "-",
                DisplaySubCategoriesInMenu = category.DisplaySubCategoriesInMenu ? "+" : "-",
                ShowOnMainPage = category.ShowOnMainPage ? "+" : "-",
                SizeChart = category.SizeChart?.Name
            };

            if (fieldMapping.Contains(CategoryFields.Tags))
            {
                categoryCsv.Tags = String.Join(Separator, category.Tags.Select(x => x.Name));
            }

            if (fieldMapping.Contains(CategoryFields.Picture))
            {
                categoryCsv.Picture = category.Picture.PhotoName;
            }

            if (fieldMapping.Contains(CategoryFields.MiniPicture))
            {
                categoryCsv.MiniPicture = category.MiniPicture.PhotoName;
            }

            if (fieldMapping.Contains(CategoryFields.Icon))
            {
                categoryCsv.Icon = category.Icon.PhotoName;
            }
            
            if (fieldMapping.Contains(CategoryFields.Title) ||
                fieldMapping.Contains(CategoryFields.H1) ||
                fieldMapping.Contains(CategoryFields.MetaKeywords) ||
                fieldMapping.Contains(CategoryFields.MetaDescription))
            {
                var meta = MetaInfoService.GetMetaInfo(category.CategoryId, MetaType.Category) ??
                           new MetaInfo(0, 0, MetaType.Category, string.Empty, string.Empty, string.Empty, string.Empty);

                categoryCsv.Title = meta.Title;
                categoryCsv.H1 = meta.H1;
                categoryCsv.MetaKeywords = meta.MetaKeywords;
                categoryCsv.MetaDescription = meta.MetaDescription;
            }

            if (fieldMapping.Contains(CategoryFields.PropertyGroups))
            {
                var groups = PropertyGroupService.GetListByCategory(category.CategoryId);
                categoryCsv.PropertyGroups = String.Join(Separator, groups.Select(x => x.Name));
            }

            if (fieldMapping.Contains(CategoryFields.RelatedCategories))
            {
                categoryCsv.RelatedCategories = string.Join(";", CategoryService.GetRelatedCategoryIds(category.CategoryId, RelatedType.Related));
            }

            if (fieldMapping.Contains(CategoryFields.SimilarCategories))
            {
                categoryCsv.SimilarCategories = string.Join(";", CategoryService.GetRelatedCategoryIds(category.CategoryId, RelatedType.Alternative));
            }

            if (fieldMapping.Contains(CategoryFields.RelatedProperties))
            {
                categoryCsv.RelatedProperties = CategoryService.GetRelatedPropertyValues(category.CategoryId, RelatedType.Related).GroupBy(x => x.Property.Name)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Value).AggregateString(propertySeparator));
                foreach (var property in CategoryService.GetRelatedProperties(category.CategoryId, RelatedType.Related))
                {
                    if (categoryCsv.RelatedProperties.ContainsKey(property.Name))
                        categoryCsv.RelatedProperties[property.Name] += property.IsSame ? propertySeparator + nameSameProductProperty : propertySeparator + nameNotSameProductProperty;
                    else
                        categoryCsv.RelatedProperties.Add(property.Name, property.IsSame ? nameSameProductProperty : nameNotSameProductProperty);
                }
                categoryCsv.RelatedPropertyNames = categoryCsv.RelatedProperties.Select(x => x.Key).Distinct();
            }

            if (fieldMapping.Contains(CategoryFields.SimilarProperties))
            {
                categoryCsv.SimilarProperties = CategoryService.GetRelatedPropertyValues(category.CategoryId, RelatedType.Alternative).GroupBy(x => x.Property.Name)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Value).AggregateString(propertySeparator));
                foreach (var property in CategoryService.GetRelatedProperties(category.CategoryId, RelatedType.Alternative))
                {
                    if (categoryCsv.SimilarProperties.ContainsKey(property.Name))
                        categoryCsv.SimilarProperties[property.Name] += property.IsSame ? propertySeparator + nameSameProductProperty : propertySeparator + nameNotSameProductProperty;
                    else
                        categoryCsv.SimilarProperties.Add(property.Name, property.IsSame ? nameSameProductProperty : nameNotSameProductProperty);
                }
                categoryCsv.SimilarPropertyNames = categoryCsv.SimilarProperties.Select(x => x.Key).Distinct();
            }

            return categoryCsv;
        }

        public static IEnumerable<ExportFeedCsvCategory> GetCsvCategories(List<CategoryFields> fieldMapping, string propertySeparator, string nameSameProductProperty, string nameNotSameProductProperty)
        {
            return SQLDataAccess.ExecuteReadIEnumerable(
                   "Select * From [Catalog].[Category]", //  Where CategoryId <> 0
                    CommandType.Text,
                    reader => GetCsvCategoriesFromReader(reader, fieldMapping, propertySeparator, nameSameProductProperty, nameNotSameProductProperty));
        }

        public static int GetCsvCategoriesCount()
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "Select Count(CategoryId) From [Catalog].[Category]",
                CommandType.Text);
        }
    }
}