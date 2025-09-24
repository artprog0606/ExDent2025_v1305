namespace AdvantShop.ViewModel.Checkout
{
    public class CheckoutBonusViewModel
    {
        public float AppliedBonuses { get; set; }

        public float BonusPlus { get; set; }
        public bool HasCard { get; set; }
        public bool WantBonusCard { get; set; }
        public bool AllowSpecifyBonusAmount { get; set; }
        public bool ProhibitAccrualAndSubstractBonuses { get; set; }
    }
}