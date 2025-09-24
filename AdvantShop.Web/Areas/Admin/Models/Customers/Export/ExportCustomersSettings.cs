using System.Collections.Generic;
using AdvantShop.Core.Services.ExportImport.ExportServices.ExportCustomers;

namespace AdvantShop.Web.Admin.Models.Customers.Export
{
    public class ExportCustomersSettings : CustomersExportSettings
    {
        public Dictionary<string, string> Encodings { get; set; }

        public Dictionary<string, string> Separators { get; set; }

        public Dictionary<int, string> Groups { get; set; }
        public List<SelectItemModel> Managers { get; set; }
        public List<SelectItemModel<int>> CustomerTypes { get; set; }
        
        public List<SelectItemModel> AllFields { get; set; }
    }
}
