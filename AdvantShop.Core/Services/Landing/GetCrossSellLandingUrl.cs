using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Landing.Templates;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Track;

namespace AdvantShop.Core.Services.Landing
{
    /// <summary>
    /// Получаем landingId
    /// 
    /// Берем воронки товаров из заказа и ищем те, страницы которых не содержат товары из заказа. 
    /// Берем landingId с самым дорогим товаром.
    /// </summary>
    public class GetCrossSellLandingUrl
    {
        private readonly Lead _lead;

        #region Ctor

        private readonly List<int> _productIds;
        private readonly Order _order;
        private readonly int? _lpUpId;
        private readonly bool _isLanding;

        private readonly LpSiteService _siteService;
        private readonly LpService _lpService;

        public GetCrossSellLandingUrl()
        {
            _siteService = new LpSiteService();
            _lpService = new LpService();
        }

        public GetCrossSellLandingUrl(Order order) : this()
        {
            _productIds = order.OrderItems.Where(x => x.ProductID != null).Select(x => x.ProductID.Value).ToList();
            _order = order;
        }

        public GetCrossSellLandingUrl(int? lpUpId, Order order, bool isLanding) : this(order)
        {
            _lpUpId = lpUpId;
            _isLanding = isLanding;
        }

        public GetCrossSellLandingUrl(Lead lead) : this()
        {
            _lead = lead;
            _productIds = lead.LeadItems.Where(x => x.ProductId != null).Select(x => x.ProductId.Value).ToList();
        }

        #endregion

        public string Execute()
        {
            if (_lpUpId != null)
                return GetUrl(_lpUpId.Value, null, true);

            if (!SettingsLandingPage.ActiveLandingPage || !SettingsLandingPage.UseCrossSellLandingsInCheckout)
                return null;

            if (_productIds.Count == 0)
                return null;

            var lpSites =
                _productIds
                    .Select(productId => _siteService.GetByAdditionalSalesProductId(productId))
                    .Where(site => site != null && site.Enabled)
                    .ToList();

            if (lpSites.Count == 0)
                return null;

            var landings = new List<CrossSellLandingModel>();

            foreach (var site in lpSites)
            {
                var pages = _lpService.GetList(site.Id);

                if (pages.Count == 0)
                    continue;

                var crossSellProductId = pages[0].ProductId;

                if (crossSellProductId != null && CheckCrossSellShowMode(crossSellProductId.Value))
                {
                    var offer = OfferService.GetProductOffers(crossSellProductId.Value).FirstOrDefault(x => x.Main);
                    if (offer != null)
                    {
                        var downsellProductId = pages.Count >= 2 ? pages[1].ProductId : null;

                        landings.Add(new CrossSellLandingModel()
                        {
                            CrossSellLandingId = pages[0].Id,
                            CrossSellProductPrice = offer.RoundedPrice,
                            IgnoredDownSellLandingId =
                                SettingsLandingPage.CrossSellShowMode == ECrossSellShowMode.ProductNotInCart &&
                                downsellProductId != null && _productIds.Contains(downsellProductId.Value)
                                    ? pages[1].Id
                                    : default(int?)
                        });
                    }
                }
            }

            var lp = landings.OrderByDescending(x => x.CrossSellProductPrice).FirstOrDefault();
            if (lp != null)
                return GetUrl(lp.CrossSellLandingId, lp.IgnoredDownSellLandingId, _isLanding);

            return null;
        }

        private bool CheckCrossSellShowMode(int crossSellProductId)
        {
            return
                SettingsLandingPage.CrossSellShowMode == ECrossSellShowMode.ProductNotInCart
                    ? !_productIds.Contains(crossSellProductId)
                    : true;
        }

        private string GetUrl(int landingId, int? ignoredDownSellLandingId, bool showMode = false)
        {
            TrackService.TrackEvent(ETrackEvent.Core_Orders_OrderCreated_SalesFunnel);

            var lp = _lpService.Get(landingId);

            if (lp != null && lp.Enabled)
            {
                var url = _lpService.GetLpLink(HttpContext.Current.Request.Url.Host, lp);
                if (!string.IsNullOrEmpty(url))
                    return url + "?" + GetUrlParam() + (ignoredDownSellLandingId != null ? "&without=" + ignoredDownSellLandingId : "") + (showMode ? "&mode=lp" : "");
            }

            return UrlService.GetUrl("checkout/success?" + GetUrlParam() + (showMode ? "&mode=lp" : ""));
        }

        private string GetUrlParam()
        {
            if (_order != null)
                return "code=" + _order.Code;

            if (_lead != null)
                return "lid=" + _lead.Id;

            return null;
        }
    }

    public class CrossSellLandingModel
    {
        public int CrossSellLandingId { get; set; }
        public float CrossSellProductPrice { get; set; }
        public int? IgnoredDownSellLandingId { get; set; }
    }
}