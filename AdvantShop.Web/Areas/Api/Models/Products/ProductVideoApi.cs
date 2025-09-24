using AdvantShop.Catalog;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class ProductVideoApi
    {
        public string Name { get; }

        public string PlayerCode { get; }

        public string Description { get; }

        public ProductVideoApi(ProductVideo video)
        {
            Name = video.Name;
            PlayerCode = video.PlayerCode;
            Description = video.Description;
        }
    }
}