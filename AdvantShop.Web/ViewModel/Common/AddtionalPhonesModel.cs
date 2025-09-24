using System.Collections.Generic;
using AdvantShop.Core.Services.Repository;
using AdvantShop.Web.Admin.Models;

namespace AdvantShop.ViewModel.Common
{
    public class AddtionalPhonesModel
    {
        public List<AdditionalPhone> Phones { get; set; }
        public List<SelectItemModel<int>> Types { get; set; }
    }
}