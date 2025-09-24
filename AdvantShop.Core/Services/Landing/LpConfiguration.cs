using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Templates;

namespace AdvantShop.Core.Services.Landing
{
    /// <summary>
    /// Предустановленные настройки
    /// </summary>
    public class LpConfiguration
    {
        public string Template { get; set; }

        public LpFunnelType Type { get; set; }

        public int? ProductId { get; set; }

        public int? UpsellProductIdFirst { get; set; }

        public int? UpsellProductIdSecond { get; set; }

        public int? DownSellProductId { get; set; }

        private Product _product = null;
        public Product Product
        {
            get
            {
                return _product ?? (ProductIdByType != null ? (_product = ProductService.GetProduct(ProductIdByType.Value)) : null);
            }
        }

        public void ClearProduct()
        {
            _product = null;
        }

        public Offer ProductOffer
        {
            get { return Product != null ? OfferService.GetMainOffer(Product.Offers, true) : null; }
        }

        public LpTemplatePageType PageType { get; set; }


        private int? ProductIdByType
        {
            get
            {
                if (PageType == LpTemplatePageType.None)
                    return null;

                if (PageType == LpTemplatePageType.Main)
                {
                    if (Offers != null && Offers.Count > 0)
                        return Offers[0].ProductId;

                    return ProductId;
                }

                if (PageType == LpTemplatePageType.UpsellFirst)
                    return UpsellProductIdFirst;

                if (PageType == LpTemplatePageType.UpsellSecond)
                    return UpsellProductIdSecond;

                if (PageType == LpTemplatePageType.Downsell)
                    return DownSellProductId;

                return ProductId;
            }
        }

        public List<int> ProductIds { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<int> OfferIds { get; set; }

        private List<Offer> _offers = null;
        public List<Offer> Offers
        {
            get
            {
                return _offers ?? 
                       (_offers = OfferIds != null 
                           ? OfferIds.Select(OfferService.GetOffer).Where(x => x != null).ToList() 
                           : new List<Offer>());
            }
        }

        public string PostActionUrl { get; set; }
    }

    public class LpConfigurationAfter
    {
        public int? MainLpId { get; set; }

        public int? MainSecondLpId { get; set; }
        public int? MainThirdLpId { get; set; }
        public int? MainFourLpId { get; set; }
        public int? MainFiveLpId { get; set; }
        public int? MainSixLpId { get; set; }

        public int? UpsellFirstLpId { get; set; }

        public int? UpsellSecondLpId { get; set; }

        public int? DownsellLpId { get; set; }

        public int? ThankYouPageLpId { get; set; }

        public List<LpBlock> AddedBlocks { get; set; }

        public int LandingId { get; set; }
        public string PostActionUrl { get; set; }
    }
}
