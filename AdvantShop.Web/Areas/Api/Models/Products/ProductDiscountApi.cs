using AdvantShop.Catalog;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class ProductDiscountApi
    {
        public float Percent { get; }
        public float Amount { get; }
        public string Type { get; }

        public ProductDiscountApi(float percent, float amount, string type)
        {
            Percent = percent;
            Amount = amount;
            Type = type;
        }

        public ProductDiscountApi(Discount discount)
        {
            Percent = discount.Percent;
            Amount = discount.Amount;
            Type = discount.Type.ToString();
        }

        public static explicit operator Discount(ProductDiscountApi discount)
        {
            return new Discount(discount.Percent, discount.Amount);
        }
    }
}