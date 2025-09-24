using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket
{
    public class VkOrderService
    {
        public int GetOrderId(int vkOrderId)
        {
            return SQLDataAccess
                .Query<int>("Select top(1) OrderId From Vk.VkOrder_Order Where VkOrderId=@vkOrderId", new { vkOrderId })
                .FirstOrDefault();
        }

        public void Add(int orderId, int vkOrderId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Vk.VkOrder_Order (OrderId, VkOrderId) Values (@OrderId, @VkOrderId)", CommandType.Text,
                new SqlParameter("@OrderId", orderId),
                new SqlParameter("@VkOrderId", vkOrderId));
        }
    }
}
