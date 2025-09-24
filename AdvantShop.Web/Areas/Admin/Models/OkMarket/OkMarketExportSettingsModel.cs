using AdvantShop.Repository.Currencies;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Models.OkMarket
{
    public class OkMarketExportSettingsModel
    {
        public OkMarketExportSettingsModel()
        {
            Currencies = CurrencyService.GetAllCurrencies(true).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Iso3
            }).ToList();
        }

        public bool ExportPreorderProducts { get; set; }
        public bool ExportUnavailableProducts { get; set; }
        public bool ExportSizeAndColorInName { get; set; }
        public bool ExportUpdateProductPhotos { get; set; }
        public bool ExportSizeAndColorInDescription { get; set; }
        public bool ExportProperties { get; set; }
        public int ExportLinkToSite { get; set; }
        public int ExportDescription { get; set; }
        public List<SelectListItem> Currencies { get; set; }
        public string CurrencyIso3 { get; set; }
        public bool ExportOnShedule { get; set; }
        public bool IsExportRun { get; set; }
        public List<string> Reports { get; set; }
    }
}