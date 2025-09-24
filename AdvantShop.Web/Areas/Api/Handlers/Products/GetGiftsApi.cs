using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Products
{
    public class GetGiftsApi : AbstractCommandHandler<GetGiftsResponse>
    {
        private readonly int _id;
        private readonly int? _offerId;

        public GetGiftsApi(int id, int? offerId)
        {
            _id = id;
            _offerId = offerId;
        }

        protected override void Validate()
        {
            if (!ProductService.IsExists(_id))
                throw new BlException("Товар не найден");
        }

        protected override GetGiftsResponse Handle()
        {
            var gifts = ProductGiftService.GetGiftsByProductIdWithPrice(_id);

            if (_offerId != null)
                gifts = gifts.Where(x => x.ProductOfferId == _offerId.Value).ToList();

            var items =
                gifts != null && gifts.Count > 0
                    ? gifts.Where(x => x.Product != null).Select(x => new ProductGiftApi(x)).ToList()
                    : new List<ProductGiftApi>();

            return new GetGiftsResponse(items);
        }
    }
}