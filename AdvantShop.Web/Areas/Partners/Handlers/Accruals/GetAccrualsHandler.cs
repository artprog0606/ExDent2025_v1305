using AdvantShop.Areas.Partners.Models.Accruals;
using AdvantShop.Areas.Partners.ViewModels.Accruals;
using AdvantShop.Core.Models;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.SQL2;

namespace AdvantShop.Areas.Partners.Handlers.Accruals
{
    public class GetAccrualsHandler
    {
        private readonly int _currentPage;
        private readonly int? _itemsPerPage;

        private readonly Partner _partner;

        public GetAccrualsHandler(int? page = null, int? itemsPerPage = null)
        {
            _currentPage = page ?? 1;
            _itemsPerPage = itemsPerPage;
            _partner = PartnerContext.CurrentPartner;
        }

        public AccrualsViewModel Get()
        {
            var paging = new SqlPaging();
            var model = new AccrualsViewModel();

            paging.Select(
                "t.Id",
                "c.Email",
                "c.Phone",
                "t.Amount",
                "t.DateCreated",
                "t.DetailsJson",
                "tc.CurrencySymbol",
                "tc.CurrencyCode",
                "tc.CurrencyValue",
                "tc.IsCodeBefore".AsSqlField("CurrencyIsCodeBefore")
                );

            paging.From("[Partners].[Transaction] t");
            paging.Inner_Join("Partners.TransactionCurrency tc ON tc.TransactionId = t.Id");
            paging.Left_Join("Customers.Customer c ON c.CustomerId = t.CustomerId");

            paging.Where("t.PartnerId = {0}", _partner.Id);
            paging.Where("t.Amount > 0");

            paging.OrderByDesc("t.DateCreated");

            paging.ItemsPerPage = _itemsPerPage.HasValue ? _itemsPerPage.Value : 10;
            paging.CurrentPageIndex = _currentPage != 0 ? _currentPage : 1;

            var totalCount = paging.TotalRowsCount;
            var totalPages = paging.PageCount(totalCount);

            model.Pager = new Pager()
            {
                TotalItemsCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = _currentPage
            };

            if ((totalPages < _currentPage && _currentPage > 1) || _currentPage < 0)
            {
                return model;
            }

            model.Accruals = paging.PageItemsList<AccrualModel>();

            return model;
        }
    }
}