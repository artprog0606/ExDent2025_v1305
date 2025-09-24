using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Localization;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class SalesFunnelsFilterModel : BaseFilterModel
    {
        public string Name { get; set; }        
        public int SortOrder { get; set; }        
        public bool? Enable { get; set; }
    }  
}
