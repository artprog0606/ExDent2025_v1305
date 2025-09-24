using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Webhook.Models.Api
{
    public class OrderSourceModel
    {
        public static OrderSourceModel FromOrderSource(OrderSource orderSource)
        {
            if (orderSource == null)
                return null;

            return new OrderSourceModel
            {
                Id = orderSource.Id,
                Name = orderSource.Name,
                Main = orderSource.Main,
                Type = orderSource.Type.ToString()
            };
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Main { get; set; }
        public string Type { get; set; }
    }
}
