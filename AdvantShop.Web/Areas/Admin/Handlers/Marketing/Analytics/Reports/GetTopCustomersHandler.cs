using System;
using System.Linq;
using System.Collections.Generic;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics
{
    public class TopCustomerItemModel
    {
        public Guid CustomerId { get; set; }
        public string Fio { get; set; }
        public int OrdersCount { get; set; }
        public int PaidOrdersCount { get; set; }
        public string PaidOrdersSum
        {
            get
            {
                return PaidOrdersSumNumber != 0
                    ? PriceFormatService.FormatPrice(PaidOrdersSumNumber < 0 ? 0 : PaidOrdersSumNumber)
                    : PaidOrdersSumNumber.FormatPrice();
            }
        }
        public float PaidOrdersSumNumber { get; set; }
    }

    public class GetTopCustomersHandler
    {
        private readonly DateTime _from;
        private readonly DateTime _to;

        public GetTopCustomersHandler(DateTime from, DateTime to)
        {
            _from = from;
            _to = to;
        }

        public List<TopCustomerItemModel> Execute()
        {
            var customers = GetTopCustomersByOrderCount(_from, _to);

            return
                customers
                    .OrderByDescending(i => i.PaidOrdersSumNumber)
                    .ThenByDescending(i => i.OrdersCount)
                    .Take(10)
                    .ToList();
        }

        public static List<TopCustomerItemModel> GetTopCustomersByOrderCount(DateTime from, DateTime to)
        {
            string sql =
                "Select [customer].customerid, ([customer].FirstName + ' ' + [customer].lastName + ' ' + [customer].patronymic) as fio, " +
                "Count([order].OrderId) as ordersCount, " +
                "SUM(CASE WHEN [order].paymentdate is not null THEN 1 ELSE 0 END) as ordersPaidCount, " +
                "SUM(CASE WHEN [order].paymentdate is not null THEN [order].sum ELSE 0 END) as sumPaidOrdersCount " +
                "from [Order].[Order] " +
                "inner join[order].[ordercustomer] on[order].orderid = ordercustomer.orderid " +
                "inner join[customers].[customer] on[ordercustomer].customerid = [customer].customerid " +
                "where [order].orderdate >= @dateFrom and [order].orderdate <= @dateTo " +
                "group by[customer].customerid, [customer].FirstName, [customer].lastName, [customer].patronymic";
            return SQLDataAccess.ExecuteReadList(
                sql,
                CommandType.Text,
                (reader) => new TopCustomerItemModel
                {
                    CustomerId = SQLDataHelper.GetGuid(reader, "customerid"),
                    Fio = SQLDataHelper.GetString(reader, "fio"),
                    OrdersCount = SQLDataHelper.GetInt(reader, "ordersCount"),
                    PaidOrdersCount = SQLDataHelper.GetInt(reader, "ordersPaidCount"),
                    PaidOrdersSumNumber = SQLDataHelper.GetFloat(reader, "sumPaidOrdersCount"),
                },
                new SqlParameter("@dateFrom", from),
                new SqlParameter("@dateTo", to));

        }
    }
}
