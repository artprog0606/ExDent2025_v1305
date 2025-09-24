using System.Collections.Generic;
using AdvantShop.Areas.Partners.Models.Customers;
using AdvantShop.Core.Models;

namespace AdvantShop.Areas.Partners.ViewModels.Customers
{
    public class CustomersViewModel
    {
        public List<CustomerModel> Customers { get; set; }

        public Pager Pager { get; set; }
    }
}