using System;
using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Categories
{
    public class DeleteCategory : AbstractCommandHandler<DeleteCategoryResponse>
    {
        private readonly int _id;
        private Category _category;

        public DeleteCategory(int id)
        {
            _id = id;
        }

        protected override void Load()
        {
            _category = CategoryService.GetCategory(_id);
        }

        protected override void Validate()
        {
            if (_category == null)
                throw new BlException("Категории не существует");
            
            if (_category.CategoryId == 0)
                throw new BlException("Нельзя удалить корневую категорию");
        }

        protected override DeleteCategoryResponse Handle()
        {
            try
            {
                CategoryService.DeleteCategoryAndPhotos(_id);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                throw new BlException(ex.Message);
            }
            return new DeleteCategoryResponse();
        }
    }
}