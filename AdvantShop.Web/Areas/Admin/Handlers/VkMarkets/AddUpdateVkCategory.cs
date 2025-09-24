using System;
using AdvantShop.Core;
using AdvantShop.Core.Services.Crm.Vk.VkMarket;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Models;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Models.VkMarkets;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.VkMarkets
{
    public class AddUpdateVkCategory : AbstractCommandHandler<bool>
    {
        private readonly VkCategoryModel _category;
        private readonly VkCategoryService _categoryService;
        private readonly VkMarketApiService _apiService;

        public AddUpdateVkCategory(VkCategoryModel category)
        {
            _category = category;
            _categoryService = new VkCategoryService();
            _apiService = new VkMarketApiService();
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_category.Name))
                throw new BlException("Укажите Название");
            
            if (_category.VkCategoryId == 0)
                throw new BlException("Укажите Категорию ВКонтакте");
            
            if (_category.CategoryIds == null || _category.CategoryIds.Count == 0)
                throw new BlException("Укажите Категории магазина");
        }

        protected override bool Handle()
        {
            try
            {
                var cat = new VkCategory()
                {
                    Id = _category.Id,
                    VkId = _category.VkId,
                    Name = _category.Name,
                    VkCategoryId = _category.VkCategoryId,
                    SortOrder = _category.SortOrder
                };

                var updateSortOrder = true;

                if (_category.Id == 0)
                {
                    _categoryService.Add(cat);
                }
                else
                {
                    var c = _categoryService.Get(cat.Id);
                    updateSortOrder = c != null && c.SortOrder != cat.SortOrder;

                    _categoryService.Update(cat);
                    _categoryService.RemoveLinks(cat.Id);
                }

                if (_category.CategoryIds != null)
                {
                    foreach (var categoryId in _category.CategoryIds)
                    {
                        _categoryService.AddLink(categoryId, cat.Id);
                    }
                }

                // change sortorder
                if (updateSortOrder)
                {
                    var categories = _categoryService.GetList();
                    if (categories.Count > 1)
                    {
                        var index = categories.FindIndex(x => x.Id == _category.Id);
                        
                        var before = index > 0 ? categories[index - 1].VkId : default(long?);
                        var after = index + 1 < categories.Count ? categories[index + 1].VkId : default(long?);
                    
                        _apiService.ReorderAlbums(cat.VkId, before, after);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);

                if (ex.Message != null)
                {
                    if (ex.Message == "Access denied")
                        throw new BlException("У пользователя нет прав на создание категорий или в группе не активирована опция \"Товары в ВКонтакте\"");

                    throw new BlException(ex.Message);
                }

                return false;
            }

            return true;
        }

    }
}
