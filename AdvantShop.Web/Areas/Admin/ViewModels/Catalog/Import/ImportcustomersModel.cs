using AdvantShop.Customers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.ViewModels.Catalog.Import
{
    public class ImportCustomersModel : BaseImportModel
    {
        public ImportCustomersModel()
        {
            CustomerGroups = CustomerGroupService.GetCustomerGroupList().Select(x => new SelectListItem { Value = x.CustomerGroupId.ToString(), Text = x.GroupName }).ToList();
            DefaultCustomerGroupId = CustomerGroupService.GetDefaultCustomerGroup().CustomerGroupId;
        }

        public int DefaultCustomerGroupId { get; set; }

        public List<SelectListItem> CustomerGroups { get; set; }
    }
}
