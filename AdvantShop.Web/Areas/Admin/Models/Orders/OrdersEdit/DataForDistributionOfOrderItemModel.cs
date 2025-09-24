using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Orders.OrdersEdit
{
    public class DataForDistributionOfOrderItemModel
    {
        public List<WarehouseDistributionInfoModel> Warehouses { get; set; }
    }

    public class WarehouseDistributionInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float AvailableAmount { get; set; }
    }
}