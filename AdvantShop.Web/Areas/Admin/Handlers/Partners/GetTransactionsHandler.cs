using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Partners;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class GetTransactionsHandler : AbstractHandler<PartnerTransactionsFilterModel, int, PartnerTransactionModel>
    {
        public GetTransactionsHandler(PartnerTransactionsFilterModel filterModel) : base(filterModel)
        {
        }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select(
                "t.Id",
                "t.Amount",
                "t.Balance",
                "t.Basis",
                "t.DateCreated",
                "t.CustomerId",
                "tc.CurrencySymbol",
                "tc.CurrencyCode",
                "tc.CurrencyValue",
                "tc.IsCodeBefore".AsSqlField("CurrencyIsCodeBefore"),
                "LTRIM(case when c.Organization is null or c.Organization = '' then c.LastName + ' ' + c.FirstName + ' ' + ISNULL(c.Patronymic,'') else c.Organization end)".AsSqlField("CustomerName"),
                "t.OrderId",
                "o.Number".AsSqlField("OrderNumber"),
                "t.DetailsJson"
                );

            paging.From("[Partners].[Transaction] t");
            paging.Inner_Join("Partners.TransactionCurrency tc ON tc.TransactionId = t.Id");
            paging.Left_Join("Customers.Customer c ON c.CustomerId = t.CustomerId");
            paging.Left_Join("[Order].[Order] o ON o.OrderId = t.OrderId");

            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            paging.Where("t.PartnerId = {0}", FilterModel.PartnerId);

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