using AdvantShop.Catalog;

namespace AdvantShop.Areas.Partners.ViewModels.Home
{
    public class HomeViewModel
    {
        public Coupon Coupon { get; set; }

        public string ReferralRequestParam { get; set; }

        public int CustomersCount { get; set; }
        public string Balance { get; set; }
        public string RewardPercent { get; set; }
        /// <summary>
        /// сумма товаров в оплаченных заказах, с которых начислено вознаграждение
        /// </summary>
        public string OrderItemsSum { get; set; }
        public string RewardsSum { get; set; }
        public bool AllowPayoutRewards { get; set; }
    }
}
