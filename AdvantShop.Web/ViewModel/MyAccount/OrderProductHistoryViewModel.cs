using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.MyAccount;

namespace AdvantShop.ViewModel.MyAccount
{
    public class OrderProductHistoryViewModel
    {
        public OrderProductHistoryViewModel()
        {
            ShowProductArtNo = SettingsCatalog.ShowProductArtNo;
        }
        public bool ShowProductArtNo { get; set; }
    }
}