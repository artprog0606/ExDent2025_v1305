using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.Vk.VkMarket;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Models;

namespace AdvantShop.Web.Admin.Models.VkMarkets
{
    public class VkCategoryModel
    {
        public int Id { get; set; }

        public long VkId { get; set; }

        public long VkCategoryId { get; set; }

        public string Name { get; set; }

        public string Categories { get; set; }
        public List<int> CategoryIds { get; set; }

        public int SortOrder { get; set; }

        public VkCategoryModel()
        {
            
        }

        public VkCategoryModel(VkCategory vkCategory)
        {
            Id = vkCategory.Id;
            VkId = vkCategory.VkId;
            VkCategoryId = vkCategory.VkCategoryId;
            Name = vkCategory.Name;
            SortOrder = vkCategory.SortOrder;

            var cats = new VkCategoryService().GetLinkedCategories(Id);

            var categories = new List<string>();
            foreach (var category in cats)
            {
                var subCats = CategoryService.GetParentCategories(category.CategoryId);
                var path = String.Join(" - ", subCats.Where(x => x.CategoryId != 0).Select(x => x.Name));
                
                categories.Add(path.Length > 200 ? path.Reduce(200) + ".." : path);
            }

            Categories = String.Join(",<br> ", categories);
            CategoryIds = cats.Select(x => x.CategoryId).ToList();
        }
    }
}
