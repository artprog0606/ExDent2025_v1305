using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Landing.Templates
{
    public class LpTemplateHelperService 
    {
        public List<Category> GetCategoriesForProductView()
        {
            var categories =
                SQLDataAccess.Query<Category>(
                    "Select top(3) c.CategoryId, c.Name From Catalog.Category c Where c.Enabled = 1 and (Select Count(*) From Catalog.ProductCategories pc Where pc.CategoryId = c.CategoryId) > 6 and c.CategoryId <> 0")
                    .ToList();

            if (categories.Count == 0)
            {
                categories =
                    SQLDataAccess.Query<Category>(
                        "Select top(3) c.CategoryId, c.Name From Catalog.Category c Where c.Enabled = 1 and c.CategoryId <> 0")
                        .ToList();
            }

            return categories;
        }

        public Dictionary<Category, List<int>> GetCategoriesByProductIds(List<int> productIds)
        {
            var list = new Dictionary<Category, List<int>>();

            foreach (var productId in productIds)
            {
                var categoryId = ProductService.GetFirstCategoryIdByProductId(productId);
                if (categoryId != CategoryService.DefaultNonCategoryId)
                {
                    var category = CategoryService.GetCategory(categoryId);
                    if (category != null)
                    {
                        if (list.ContainsKey(category))
                        {
                            if (list[category] == null)
                                list[category] = new List<int>();

                            list[category].Add(productId);
                        }
                        else
                        {
                            list.Add(category, new List<int>() {productId});
                        }
                    }
                }
                else
                {
                    var category = new Category(){Name = "Новая коллекция"};
                    if (list.ContainsKey(category))
                    {
                        if (list[category] == null)
                            list[category] = new List<int>();

                        list[category].Add(productId);
                    }
                    else
                    {
                        list.Add(category, new List<int>() {productId});
                    }
                }
            }

            return list;
        }

        public List<int> GetProductIdsForProductView(int count, int categoryId)
        {
            return 
                SQLDataAccess.Query<int>(
                    "Select top(@count) p.ProductId " +
                    "From Catalog.Product p " +
                    "Left Join Catalog.ProductCategories pc on p.ProductId = pc.ProductId " +
                    "Where pc.CategoryId = @categoryId " +
                    "Order by p.Enabled desc, pc.SortOrder",
                    new {count, categoryId})
                    .ToList();
        }
    }
}
