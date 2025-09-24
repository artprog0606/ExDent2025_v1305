using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.WishList
{
    public class RemoveFromWishList : AbstractCommandHandler<ApiResponse>
    {
        private readonly int _offerId;
        private Offer _offer;

        public RemoveFromWishList(int offerId)
        {
            _offerId = offerId;
        }
        
        protected override void Load()
        {
            _offer = OfferService.GetOffer(_offerId);
        }

        protected override void Validate()
        {
            if (!SettingsApiAuth.ShowWishList)
                throw new BlException("Настройка \"Избранное\" не активна");
            
            if (_offer == null)
                throw new BlException("Товар не найден");
        }
        
        protected override ApiResponse Handle()
        {
            var item = ShoppingCartService.CurrentWishlist.Find(x => x.OfferId == _offerId);
            if (item != null)
                ShoppingCartService.DeleteShoppingCartItem(item);
            
            return new ApiResponse();
        }
    }
}