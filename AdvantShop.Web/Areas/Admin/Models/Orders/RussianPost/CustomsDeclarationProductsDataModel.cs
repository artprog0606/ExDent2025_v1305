using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Orders.RussianPost
{
    public class CustomsDeclarationProductsDataModel
    {
        public List<CustomsDeclarationProductData> Products { get; set; }
    }

    public class CustomsDeclarationProductData
    {
        public int ItemId { get; set; }
        public int? ProductId { get; set; }
        public string Name { get; set; }
        public string ArtNo { get; set; }
        public string CountryCode { get; set; }
        public string TnvedCode { get; set; }
    }
}
