using AdvantShop.Core.Services.Landing;
using Newtonsoft.Json;
using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Landing.Pictures;

namespace AdvantShop.App.Landing.Models
{
    public class ProductShippingVariantsModel
    {
        public int OfferId { get; set; }
        public float MinAmount { get; set; }
        public SettingsDesign.eShowShippingsInDetails ShowShippingsMethods { get; set; }
        public string Zip { get; set; }
        
        public string NgOfferId { get; set; }
        public string NgAmount { get; set; }
        public string NgSvCustomOptions { get; set; }
        public string NgInitFn { get; set; }

        public ProductShippingVariantsModel()
        {
            ShowShippingsMethods =
                Helpers.BrowsersHelper.IsBot()
                    ? SettingsDesign.eShowShippingsInDetails.ByClick
                    : SettingsDesign.ShowShippingsMethodsInDetails;

            Zip = AdvantShop.Repository.IpZoneContext.CurrentZone.Zip;

            NgOfferId = "product.offerSelected.OfferId";
            NgAmount = "product.offerSelected.AmountBuy";
            NgSvCustomOptions = "product.customOptions.xml";
            NgInitFn = "product.addShippingVariants(shippingVariants)";
        }
    }
}
