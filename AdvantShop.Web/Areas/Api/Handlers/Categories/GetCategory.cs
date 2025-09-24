using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Categories
{
    public class GetCategory : AbstractCommandHandler<GetCategoryResponse>
    {
        private readonly int _id;
        private Category _category;

        public GetCategory(int id)
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
        }

        protected override GetCategoryResponse Handle()
        {
            return new GetCategoryResponse(_category);
        }
    }
}