using System.Drawing;
using System.Web;
using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Core;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Api;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Areas.Api.Handlers.Categories
{
    public class DeletePicture : AbstractCommandHandler<ApiResponse>
    {
        private readonly int _id;
        private readonly PhotoType _type;
        private Category _category;

        public DeletePicture(int id, PhotoType type)
        {
            _id = id;
            _type = type;
        }

        protected override void Load()
        {
            _category = CategoryService.GetCategory(_id);
        }

        protected override void Validate()
        {
            if (_category == null)
                throw new BlException("Категория не найдена");
        }
        
        protected override ApiResponse Handle()
        {
            var photo = PhotoService.GetPhoto<CategoryPhoto>(_id, _type);
            if (photo != null)
            {
                PhotoService.DeletePhotos(_id, _type);
                CategoryService.ClearCategoryCache(_id);
            }
            
            return new ApiResponse();
        }
    }
}