using System.Collections.Generic;
using AdvantShop.Areas.Partners.Models.Accruals;
using AdvantShop.Core.Models;

namespace AdvantShop.Areas.Partners.ViewModels.Accruals
{
    public class AccrualsViewModel
    {
        public List<AccrualModel> Accruals { get; set; }

        public Pager Pager { get; set; }
    }
}