using System.Linq;
using AdvantShop.Areas.Api.Models.WishList;
using AdvantShop.Core;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.WishList
{
    public class GetWishListApi : AbstractCommandHandler<GetWishListResponse>
    {
        protected override void Validate()
        {
            if (!SettingsApiAuth.ShowWishList)
                throw new BlException("Настройка \"Избранное\" не активна");
        }
        
        protected override GetWishListResponse Handle()
        {
            var items = 
                ShoppingCartService.CurrentWishlist
                    .Where(x => x.Offer != null && x.Offer.Product != null)
                    .Select(x => new WishListItem(x))
                    .ToList();

            return new GetWishListResponse(items);
        }
    }
}