using AdvantShop.Core.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Web.Admin.ViewModels.Analytics
{
    public class ExportProductsModel
    {
        public int? OfferId { get; set; }

        public string Encoding { get; set; }

        public string ColumnSeparator { get; set; }

        public EExportProductsType ExportProductsType { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }


        public List<int> SelectedCategories { get; set; }

        public Dictionary<string, string> Encodings { get; set; }

        public Dictionary<string, string> Separators { get; set; }

    }
    
    public enum EExportProductsType
    {
        [Localize("Admin.Js.ExportProducts.AllProducts")]
        AllProducts = 0,
        [Localize("Admin.Js.ExportProducts.Categories")]
        Categories = 1,
        [Localize("Admin.Js.ExportProducts.OneProduct")]
        OneProduct = 2
    }
}
