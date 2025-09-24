using AdvantShop.Core.Services.Catalog;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Catalog.Products
{
    public class AdminOfferModel
    {
        public int OfferId { get; set; }

        public int ProductId { get; set; }

        public float Amount { get; set; }

        public float BasePrice { get; set; }

        public float SupplyPrice { get; set; }

        public int? ColorId { get; set; }

        public string Color { get; set; }

        public int? SizeId { get; set; }

        public string Size { get; set; }

        public bool Main { get; set; }

        public string ArtNo { get; set; }

        public float? Weight { get; set; }

        public float? Length { get; set; }

        public float? Width { get; set; }

        public float? Height { get; set; }

        public string BarCode { get; set; }

        public List<OfferPriceRule> OfferPriceRules { get; set; }
        public int UsedWarehouses { get; set; }
        public int CountWarehouses { get; set; }
    }
}
