using System.Collections.Generic;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Models;

namespace AdvantShop.Web.Admin.Models.Crm.Vk
{
    public class FilterMarketCategory
    {
        public long Id { get; }
        public string Name { get; }
        public string SubCategories { get; }
        
        public FilterMarketCategory(VkMarketCategoryItem category)
        {
            Id = category.Id;
            Name = category.Name;

            var subCategories = category.View?.RootPath ?? new List<string>();
            subCategories.Reverse();
            SubCategories = string.Join(" • ", subCategories);
        }
    }
}