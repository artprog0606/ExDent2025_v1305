namespace AdvantShop.Catalog
{
    public class DiscountByTimeCategory
    {
        public int DiscountByTimeId { get; set; }
        public int CategoryId { get; set; }
        public bool ApplyDiscount {  get; set; }
        public bool ActiveByTime { get; set; }
    }
}
