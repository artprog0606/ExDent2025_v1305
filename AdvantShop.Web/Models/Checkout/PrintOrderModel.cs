using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdvantShop.Models.Checkout
{
    public class PrintOrderModel
    {
        public string Code { get; set; }
        public bool ShowMap { get; set; }
        public string Sorting { get; set; }
        public string SortingType { get; set; }
    }
}