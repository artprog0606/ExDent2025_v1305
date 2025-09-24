using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Settings.ShippingMethods.Warehouse;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Settings.ShippingMethods.Warehouse
{
    public class GetWarehousesHandler: ICommandHandler<List<WarehouseModel>>
    {
        private readonly int _shippingMethodId;

        public GetWarehousesHandler(int shippingMethodId)
        {
            _shippingMethodId = shippingMethodId;
        }
        public List<WarehouseModel> Execute()
        {
           return SQLDataAccess.ExecuteReadList(
                @"SELECT [Warehouse].[Id], [Warehouse].[Name]
                FROM [Catalog].[Warehouse] 
                    INNER JOIN [Order].[ShippingWarehouse] ON [ShippingWarehouse].[WarehouseId] = [Warehouse].[Id]
                WHERE [ShippingWarehouse].[MethodId] = @MethodId",
                CommandType.Text,
                reader => new WarehouseModel
                {
                    Id = SQLDataHelper.GetInt(reader, "Id"),
                    Name = SQLDataHelper.GetString(reader, "Name"),
                },
                new SqlParameter("@MethodId", _shippingMethodId));
        }
    }
}