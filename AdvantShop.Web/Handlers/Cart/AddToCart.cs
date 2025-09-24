using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Extensions;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.Models.Cart;
using AdvantShop.Models.Checkout;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvantShop.Handlers.Cart
{
    public class AddToCart
    {
        #region Ctor

        private readonly CartItemAddingModel _itemModel;
        private readonly Controller _controller;

        public AddToCart(CartItemAddingModel item, Controller controller)
        {
            _itemModel = item;
            _controller = controller;
        }

        #endregion Ctor

        public AddToCartResult Execute()
        {
            var cartId = 0;
            ShoppingCartItem cartItem = null;

            if (_itemModel.Mode == "lp" && _itemModel.OfferIds != null)
            {
                var offers = _itemModel.OfferIds.Select(OfferService.GetOffer).Where(o => o != null).ToList();
                if (offers.Count == 0)
                    return new AddToCartResult("fail");

                var button = GetLpButton(_itemModel.LpBlockId);

                if (_itemModel.LpEntityId != null && _itemModel.LpEntityId != 0)
                {
                    if (_itemModel.LpEntityType == "order")
                    {
                        var order = OrderService.GetOrder(_itemModel.LpEntityId.Value);
                        if (order != null)
                        {
                            AddToCartResult result = null;
                            foreach (var offer in offers)
                                result = AddItemToOrder(GetCartItem(offer, button: button), order);

                            return result;
                        }
                    }
                    else if (_itemModel.LpEntityType == "lead")
                    {
                        var lead = LeadService.GetLead(_itemModel.LpEntityId.Value);
                        if (lead != null)
                        {
                            AddToCartResult result = null;
                            foreach (var offer in offers)
                                result = AddItemToLead(lead, offer);

                            return result;
                        }
                    }
                }
                else
                {
                    ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart);

                    foreach (var offer in offers)
                        cartId = ShoppingCartService.AddShoppingCartItem(GetCartItem(offer, button: button));
                }
            }
            else
            {
                Offer offer;

                if (_itemModel.OfferId == 0 && _itemModel.ProductId != 0)
                {
                    var product = ProductService.GetProduct(_itemModel.ProductId);

                    if (product == null || product.Offers.Count == 0)
                        return new AddToCartResult("fail");

                    if (product.Offers.Count != 1)
                        return new AddToCartResult("redirect");

                    offer = product.Offers.First();
                }
                else
                {
                    offer = OfferService.GetOffer(_itemModel.OfferId);
                }

                if (offer == null)
                    return new AddToCartResult("fail");

                if ((!offer.Product.Enabled || !offer.Product.CategoryEnabled) && _itemModel.Mode != "lp")
                    return new AddToCartResult("fail");

                List<EvaluatedCustomOptions> listOptions = null;
                var selectedOptions = !string.IsNullOrWhiteSpace(_itemModel.AttributesXml) && _itemModel.AttributesXml != "null"
                                        ? HttpUtility.UrlDecode(_itemModel.AttributesXml)
                                        : null;

                if (selectedOptions != null)
                {
                    try
                    {
                        listOptions = CustomOptionsService.DeserializeFromXml(selectedOptions, offer.Product.Currency.Rate);
                    }
                    catch (Exception)
                    {
                        listOptions = null;
                    }
                }

                if (CustomOptionsService.DoesProductHaveRequiredCustomOptions(offer.ProductId) && listOptions == null)
                    return new AddToCartResult("redirect");

                cartItem = GetCartItem(offer, listOptions, selectedOptions, GetLpButton(_itemModel.LpBlockId));

                if (CustomOptionsService.IsValidCustomOptions(cartItem) is false)
                    return new AddToCartResult("fail");

                if (_itemModel.Mode == "lp")
                {
                    if (_itemModel.LpEntityId != null && _itemModel.LpEntityId != 0)
                    {
                        if (_itemModel.LpEntityType == "order")
                        {
                            var order = OrderService.GetOrder(_itemModel.LpEntityId.Value);
                            if (order != null)
                                return AddItemToOrder(cartItem, order);
                        }
                        else if (_itemModel.LpEntityType == "lead")
                        {
                            var lead = LeadService.GetLead(_itemModel.LpEntityId.Value);
                            if (lead != null)
                                return AddItemToLead(lead, offer);
                        }
                    }

                    if (_itemModel.LpId != null && LPageSettings.ShowShoppingCart(_itemModel.LpId.Value))
                        _itemModel.Mode = "";
                    else
                        ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart);
                }

                cartId = ShoppingCartService.AddShoppingCartItem(cartItem);


                ProductGiftService.AddGiftByOfferToCart(offer);

                if (_itemModel.Payment != 0)
                    CommonHelper.SetCookie("payment", _itemModel.Payment.ToString());

                ModulesExecuter.AddToCart(cartItem, _controller.Url.AbsoluteRouteUrl("Product", new { url = offer.Product.UrlPath }));
            }

            var url = "";

            if (_itemModel.Mode == "lp")
            {
                var queryString = _itemModel.LpId != null ? "?lpid=" + _itemModel.LpId : "";

                if (_itemModel.LpUpId != null)
                    queryString += (string.IsNullOrEmpty(queryString) ? "?" : "&") + "lpupid=" + _itemModel.LpUpId;

                if (_itemModel.HideShipping.HasValue && _itemModel.HideShipping.Value)
                    queryString += (string.IsNullOrEmpty(queryString) ? "?" : "&") + "mode=" + (int)CheckoutLpMode.WithoutShipping;

                url = "checkout/lp" + queryString;
            }
            
            return new AddToCartResult(_itemModel.Mode == "lp" ? "redirect" : "success")
            {
                Url = url,
                CartId = cartId,
                TotalItems = ShoppingCartService.CurrentShoppingCart.TotalItems,
                CartItem = cartItem
            };
        }

        private ShoppingCartItem GetCartItem(Offer offer, List<EvaluatedCustomOptions> listOptions = null, string selectedOptions = null, LpButton button = null)
        {
            if (Single.IsNaN(_itemModel.Amount) || _itemModel.Amount == 0)
            {
                var prodMinAmount = offer.Product.GetMinAmount();

                _itemModel.Amount = prodMinAmount > 0 ? prodMinAmount : 1;
            }

            var item = new ShoppingCartItem()
            {
                OfferId = offer.OfferId,
                Amount = _itemModel.Amount,
                AttributesXml = listOptions != null ? selectedOptions : string.Empty,
            };

            if (button != null)
            {
                if (button.Discount != null)
                {
                    var customOptionsPrice = CustomOptionsService.GetCustomOptionPrice(offer.RoundedPrice, selectedOptions, offer.Product.Currency.Rate);

                    item.CustomPrice = PriceService.GetFinalPrice(offer.RoundedPrice + customOptionsPrice, button.Discount);
                }

                var actionOfferItem = GetActionOfferItem(button, offer);
                if (actionOfferItem != null)
                {
                    if (!string.IsNullOrEmpty(actionOfferItem.OfferPrice))
                        item.CustomPrice = actionOfferItem.OfferPrice.TryParseFloat();

                    if (!string.IsNullOrEmpty(actionOfferItem.OfferAmount))
                        item.Amount = actionOfferItem.OfferAmount.TryParseFloat(1);
                }
            }

            return item;
        }

        private AddToCartResult AddItemToOrder(ShoppingCartItem cartItem, Order order)
        {
            var orderItems = order.OrderItems;
            var orderItem = (OrderItem)cartItem;

            var button = GetLpButton(_itemModel.LpBlockId);
            if (button != null && !string.IsNullOrEmpty(button.ActionOfferPrice) && (button.UseManyOffers == null || !button.UseManyOffers.Value))
                orderItem.Price = button.ActionOfferPrice.TryParseFloat();

            orderItems.Add(orderItem);

            OrderService.AddUpdateOrderItems(orderItems, OrderService.GetOrderItems(order.OrderID), order, null, false, true);

            var lpUrl = "";
            if (_itemModel.LpUpId != null)
            {
                lpUrl = new LpService().GetLpLink(_controller.Request.Url.Host, _itemModel.LpUpId.Value) + "?code=" + order.Code;
            }
            else
            {
                var lpSite = _itemModel.LpBlockId != null ? new LpSiteService().GetByLandingBlockId(_itemModel.LpBlockId.Value) : null;
                var showMode = lpSite == null ||
                               lpSite.Template != LpFunnelType.ProductCrossSellDownSell.ToString() ||
                               (lpSite.Template == LpFunnelType.ProductCrossSellDownSell.ToString() && _itemModel.ModeFrom == "lp");

                _controller.TempData["orderid"] = order.OrderID;

                lpUrl = UrlService.GetUrl("checkout/success?code=" + order.Code + (showMode ? "&mode=lp" : ""));
            }

            return new AddToCartResult("redirect") { Url = lpUrl };
        }

        private AddToCartResult AddItemToLead(Lead lead, Offer offer)
        {
            float? actionOfferPrice = null;

            var button = GetLpButton(_itemModel.LpBlockId);
            if (button != null)
            {
                if (!string.IsNullOrEmpty(button.ActionOfferPrice) &&
                    (button.UseManyOffers == null || !button.UseManyOffers.Value))
                {
                    actionOfferPrice = button.ActionOfferPrice.TryParseFloat();
                }
                else
                {
                    var actionOfferItem = GetActionOfferItem(button, offer);
                    if (actionOfferItem != null)
                    {
                        if (!string.IsNullOrEmpty(actionOfferItem.OfferPrice))
                            actionOfferPrice = actionOfferItem.OfferPrice.TryParseFloat();

                        if (!string.IsNullOrEmpty(actionOfferItem.OfferAmount))
                            _itemModel.Amount = actionOfferItem.OfferAmount.TryParseFloat(1);
                    }
                }

                var shippingPrice = button.ActionShippingPrice.TryParseFloat(true);
                if (shippingPrice != null)
                {
                    lead.ShippingName = LocalizationService.GetResource("Lead.LaningShippingName");
                    lead.ShippingCost = shippingPrice.Value;
                }
            }

            lead.LeadItems.Add(new LeadItem(offer, _itemModel.Amount, actionOfferPrice));
            LeadService.UpdateLead(lead, false, trackChanges: false);

            var lpUrl = _itemModel.LpUpId != null
                ? new LpService().GetLpLink(_controller.Request.Url.Host, _itemModel.LpUpId.Value) + "?lid=" + lead.Id
                : UrlService.GetUrl("checkout/buyinoneclicksuccess/" + lead.Id + "?mode=lp");

            return new AddToCartResult("redirect") { Url = lpUrl };
        }

        private LpButton GetLpButton(int? blockId)
        {
            if (blockId == null)
                return null;

            var block = new LpBlockService().Get(blockId.Value);
            var buttonName = !string.IsNullOrEmpty(_itemModel.LpButtonName) ? _itemModel.LpButtonName : "button";

            return block != null ? block.TryGetSetting<LpButton>(buttonName) : null;
        }

        private LpButtonOfferItem GetActionOfferItem(LpButton button, Offer offer)
        {
            if (button == null)
                return null;

            if (button.ActionOfferItems != null && button.ActionOfferItems.Count > 0)
                return button.ActionOfferItems.Find(x => x.OfferId == offer.OfferId.ToString());

            return null;
        }
    }
}