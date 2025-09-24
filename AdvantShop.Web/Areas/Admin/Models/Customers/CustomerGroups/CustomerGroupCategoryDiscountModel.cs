namespace AdvantShop.Web.Admin.Models.Customers.CustomerGroups
{
    public class CustomerGroupCategoryDiscountModel
    {
        public int CustomerGroupId { get; set; }
        public int CategoryId { get; set; }
        public float? Discount { get; set; }
        public string Name { get; set; }
    }
}