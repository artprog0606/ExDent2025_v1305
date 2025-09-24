using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Areas.Partners.Models.Rewards;
using AdvantShop.Core.Models;
using AdvantShop.Core.Services.Partners;

namespace AdvantShop.Areas.Partners.ViewModels.Rewards
{
    public class RewardsViewModel
    {
        public RewardsViewModel()
        {
            PaymentTypes = new List<SelectListItem> { new SelectListItem() { Text = "Не указан", Value = string.Empty } };
            PaymentTypes.AddRange(PaymentTypeService.GetPaymentTypes().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));
        }

        public Partner Partner { get; set; }

        public List<RewardModel> Rewards { get; set; }

        public Pager Pager { get; set; }

        public int? PaymentTypeId { get; set; }
        public string PaymentAccountNumber { get; set; }
        public List<SelectListItem> PaymentTypes { get; private set; }
    }
}