using System;
using System.Web.Mvc;
using AdvantShop.Areas.Api.Handlers.Cart;
using AdvantShop.Areas.Api.Models.Cart;
using AdvantShop.Areas.Api.Models.Checkout;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Checkout
{
    public class GetCheckoutApi : AbstractCommandHandler<GetCheckoutResponse>
    {
        private readonly CheckoutApiModel _model;

        public GetCheckoutApi(CheckoutApiModel model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            var cart = new GetCartApi(_model).Execute();

            if (!string.IsNullOrEmpty(cart.ValidationError))
                throw new BlException(cart.ValidationError);
            
            if (!ShoppingCartService.CurrentShoppingCart.CanOrder)
                throw new BlException("В корзине есть товары недоступные к оформлению");
        }

        protected override GetCheckoutResponse Handle()
        {
            var customer = CustomerContext.CurrentCustomer;
            var checkoutData = MyCheckout.Factory(customer.Id);

            var needUpdate = UpdateAddress(checkoutData);
            
            if (_model.ShippingId != null)
            {
                checkoutData.Data.PreSelectedShippingId = _model.ShippingId;
                checkoutData.Data.PreSelectedShippingPointId = _model.ShippingPointId;
                needUpdate = true;
            }

            if (_model.ContactId != null && _model.ContactId != Guid.Empty)
            {
                checkoutData.Data.Contact.ContactId = _model.ContactId;
                if (SettingsCheckout.SplitShippingByType)
                    checkoutData.Data.TypeCalculationVariants = CalculationVariants.Courier;
                
                needUpdate = true;
            }
            
            if (needUpdate)
                checkoutData.Update();

            return new GetCheckoutResponse()
            {
                Url = UrlService.GetUrl("checkout/apiauth")
            };
        }

        private bool UpdateAddress(MyCheckout checkoutData)
        {
            var address = _model.Address;
            var needUpdate = false;
            
            if (address != null &&
                (!string.IsNullOrEmpty(address.Country) || !string.IsNullOrEmpty(address.Region) ||
                 !string.IsNullOrEmpty(address.City) || !string.IsNullOrEmpty(address.District) ||
                 !string.IsNullOrEmpty(address.PostCode)))
            {
                var contact = checkoutData.Data.Contact;

                if (contact == null)
                    contact = new CheckoutAddress();

                if (!string.IsNullOrEmpty(address.Country) && string.IsNullOrEmpty(contact.Country))
                {
                    contact.Country = address.Country;
                    needUpdate = true;
                }

                if (!string.IsNullOrEmpty(address.Region) && string.IsNullOrEmpty(contact.Region))
                {
                    contact.Region = address.Region;
                    needUpdate = true;
                }

                if (!string.IsNullOrEmpty(address.City) && string.IsNullOrEmpty(contact.City))
                {
                    contact.City = address.City;
                    needUpdate = true;
                }
                
                if (!string.IsNullOrEmpty(address.District) && string.IsNullOrEmpty(contact.District))
                {
                    contact.District = address.District;
                    needUpdate = true;
                }
                
                if (!string.IsNullOrEmpty(address.PostCode) && string.IsNullOrEmpty(contact.Zip))
                {
                    contact.Zip = address.PostCode;
                    needUpdate = true;
                }
            }

            return needUpdate;
        }
    }
}