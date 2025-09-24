using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.Web.Admin.Models.Landings
{
    public class AddingLpModel
    {
        public string Name { get; set; }

        public LpFunnelType LpType { get; set; }

        public string Template { get; set; }

        public int? ProductId { get; set; }

        public int? AdditionalSalesProductId { get; set; }

        public int? UpsellProductIdFirst { get; set; }

        public int? UpsellProductIdSecond { get; set; }

        public int? DownSellProductId { get; set; }

        public List<int> ProductIds { get; set; }

        public List<int> CategoryIds { get; set; }

        public List<int> OfferIds { get; set; }

        private string _url;
        public string Url
        {
            get
            {
                if (_url != null)
                    return _url;

                if (AdditionalSalesProductId != null)
                {
                    var p = ProductService.GetProduct(AdditionalSalesProductId.Value);
                    if (p != null)
                        return "cross-" + p.Name;
                }

                return Name;
            }
            set { _url = value; }
        }


        public PostAction PostAction { get; set; }

    }

    public class AddingLpResult
    {
        public string LpUrl { get; set; }
        public string AdminUrl { get; set; }
    }

    public class PostAction
    {
        public int PostActionType { get; set; }
        public int? PostActionCategoryId { get; set; }
        public int? PostActionFunnelSiteId { get; set; }
        public string PostActionUrl { get; set; }
    }
}
