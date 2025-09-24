using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Partners;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class GetBindedCustomersHandler : AbstractHandler<PartnerCustomersFilterModel, int, PartnerCustomerModel>
    {
        public GetBindedCustomersHandler(PartnerCustomersFilterModel filterModel) : base(filterModel)
        {
        }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select(
                "bc.CustomerId",
                "bc.DateCreated",
                "LTRIM(case when Organization is null or Organization = '' then LastName + ' ' + FirstName + ' ' + ISNULL(Patronymic,'') else Organization end)".AsSqlField("FullName"),
                "c.Phone",
                "c.Email",
                ("(select ISNULL(SUM(o.[Sum] * oCurr.CurrencyValue),0) From [Order].[Order] o " +
                    "LEFT JOIN [Order].[OrderCustomer] oCust ON o.[OrderID] = oCust.[OrderId] " +
                    "INNER JOIN [order].[OrderCurrency] oCurr on oCurr.OrderID = o.OrderID " +
                    "where oCust.CustomerID = c.[CustomerId] and o.[PaymentDate] is not null)").AsSqlField("PaidOrdersSum"),
                ("(Select COUNT(o.[OrderId]) From [Order].[Order] o " +
                    "LEFT JOIN [Order].[OrderCustomer] oCust ON o.[OrderID] = oCust.[OrderId] " +
                    "WHERE oCust.[CustomerId] = c.[CustomerId] and o.[PaymentDate] is not null)").AsSqlField("PaidOrdersCount"),
                "(Select Top(1) [City] From [Customers].[Contact] Where [Contact].[CustomerID] = c.[CustomerID])".AsSqlField("Location"),
                "bc.VisitDate",
                "bc.CouponCode",
                "bc.Url",
                "bc.UrlReferrer",
                "bc.UtmSource",
                "bc.UtmMedium",
                "bc.UtmCampaign",
                "bc.UtmTerm",
                "bc.UtmContent"
                );

            paging.From("[Partners].[BindedCustomer] bc");
            paging.Inner_Join("Customers.Customer c ON c.CustomerId = bc.CustomerId");

            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            paging.Where("bc.PartnerId = {0}", FilterModel.PartnerId);

            return paging;
        }

        protected override SqlPaging Sorting(SqlPaging paging)
        {
            if (string.IsNullOrEmpty(FilterModel.Sorting) || FilterModel.SortingType == FilterSortingType.None)
            {
                paging.OrderByDesc("DateCreated");
                return paging;
            }

            var sorting = FilterModel.Sorting.ToLower().Replace("formatted", "");

            var field = paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (FilterModel.SortingType == FilterSortingType.Asc)
                {
                    paging.OrderBy(sorting);
                }
                else
                {
                    paging.OrderByDesc(sorting);
                }
            }

            return paging;
        }
    }
}