using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Customers
{
    public class GetTransactions : AbstractCommandHandler<GetTransactionsResponse>
    {
        private readonly long _cardId;
        private readonly TransactionsFilterModel _filter;
        private Card _bonusCard;
        private SqlPaging _paging;

        public GetTransactions(long cardId, TransactionsFilterModel filter)
        {
            _cardId = cardId;
            _filter = filter;
        }

        protected override void Validate()
        {
            if (_filter.ItemsPerPage > 100 || _filter.ItemsPerPage <= 0)
                _filter.ItemsPerPage = 100;

            if (_filter.Page < 0)
                throw new BlException("page can't less than 0");

            _bonusCard = BonusSystemService.GetCard(_cardId);
            
            if (_bonusCard == null)
                throw new BlException("Бонусная карта не найдена");
        }

        protected override GetTransactionsResponse Handle()
        {
            GetPaging();

            var model = new GetTransactionsResponse()
            {
                Pagination = new ApiPagination()
                {
                    CurrentPage = _paging.CurrentPageIndex,
                    TotalCount = _paging.TotalRowsCount,
                    TotalPageCount = _paging.PageCount()
                },
                Transactions = new List<GetTransactionItem>()
            };

            if (model.Pagination.TotalPageCount < _filter.Page && _filter.Page > 1)
                return model;

            model.Transactions = _paging.PageItemsList<GetTransactionItem>();
            model.Pagination.Count = model.Transactions.Count;

            return model;
        }

        private void GetPaging()
        {
            _paging = 
                new SqlPaging() {ItemsPerPage = _filter.ItemsPerPage, CurrentPageIndex = _filter.Page}
                .Select(
                    "Id",
                    "Balance",
                    "CreateOn",
                    "Amount",
                    "Basis",
                    "OperationType",
                    "PurchaseId"
                )
                .From("[Bonus].[Transaction]")
                .Where("CardId = {0}", _bonusCard.CardId);

            Sorting();            
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filter.Sorting) || _filter.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("CreateOn");
                return;
            }

            var sorting = _filter.Sorting.ToLower().Replace("formatted", "");

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filter.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(sorting);
                }
                else
                {
                    _paging.OrderByDesc(sorting);
                }
            }
        }
    }
}