using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;

namespace AdvantShop.Web.Admin.ViewModels.Catalog.Import
{
    public abstract class BaseImportModel
    {
        public BaseImportModel()
        {
            Encodings = Enum.GetValues(typeof(EncodingsEnum)).Cast<EncodingsEnum>()
                .Select(enumItem => new SelectListItem { Value = enumItem.StrName(), Text = enumItem.StrName() }).ToList();
            ColumnSeparators = Enum.GetValues(typeof(SeparatorsEnum)).Cast<SeparatorsEnum>()
                .Select(enumItem => new SelectListItem { Value = enumItem.StrName(), Text = enumItem.Localize() }).ToList();
        }

        public string ColumnSeparator { get; set; }
        public string CustomColumnSeparator { get; set; }
        public string Encoding { get; set; }
        public bool HaveHeader { get; set; }

        public List<SelectListItem> Encodings { get; set; }
        public List<SelectListItem> ColumnSeparators { get; set; }

        public List<string> SelectedFields { get; set; }
    }
}
