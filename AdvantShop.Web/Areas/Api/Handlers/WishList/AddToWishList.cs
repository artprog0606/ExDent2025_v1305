using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.WishList
{
    public class AddToWishList : AbstractCommandHandler<ApiResponse>
    {
        private readonly int _offerId;
        private Offer _offer;

        public AddToWishList(int offerId)
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
            
            var item = ShoppingCartService.CurrentWishlist.Find(x => x.OfferId == _offerId);
            if (item != null)
                throw new BlException("Товар уже в списке желаний");
        }
        
        protected override ApiResponse Handle()
        {
            ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
            {
                OfferId = _offer.OfferId,
                Amount = _offer.Product.GetMinAmount(),
                ShoppingCartType = ShoppingCartType.Wishlist,
            });
            return new ApiResponse();
        }
    }
}