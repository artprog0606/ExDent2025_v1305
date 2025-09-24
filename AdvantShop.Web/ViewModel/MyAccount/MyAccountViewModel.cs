using System.Collections.Generic;
using AdvantShop.Core.Services.MyAccount;

namespace AdvantShop.ViewModel.MyAccount
{
    public class MyAccountViewModel
    {
        public bool DisplayBonuses { get; set; }
        public string BonusesAmount { get; set; }

        public bool DisplayChangeEmail { get; set; }

        public List<MyAccountTab> Tabs { get; set; }
        public bool IsRegisteredNow { get; set; }

        public bool IsLanding { get; set; }
    }
}