using System;
using AdvantShop.Areas.Partners.Models.Rewards;
using AdvantShop.Areas.Partners.ViewModels.Rewards;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Models;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.SQL2;

namespace AdvantShop.Areas.Partners.Handlers.Rewards
{
    public class GetRewardsHandler
    {
        private readonly int _currentPage;
        private readonly int? _itemsPerPage;

        private readonly Partner _partner;

        public GetRewardsHandler(int? page = null, int? itemsPerPage = null)
        {
            _currentPage = page ?? 1;
            _itemsPerPage = itemsPerPage;
            _partner = PartnerContext.CurrentPartner;
        }

        public RewardsViewModel Get()
        {
            var paging = new SqlPaging();
            var model = new RewardsViewModel
            {
                Partner = _partner
            };
            if (_partner.NaturalPerson != null)
            {
                model.PaymentTypeId = _partner.NaturalPerson.PaymentTypeId;
                model.PaymentAccountNumber = _partner.NaturalPerson.PaymentAccountNumber;
            }

            paging.Select(
                "[Transaction].Id",
                "(-[Transaction].Amount * TransactionCurrency.CurrencyValue)".AsSqlField("RewardSum"),
                "[Transaction].DateCreated",
                "[Transaction].Basis"
                );

            paging.From("Partners.[Transaction]");
            paging.Inner_Join("Partners.TransactionCurrency ON TransactionCurrency.TransactionId = [Transaction].Id");

            paging.Where("[Transaction].PartnerId = {0}", _partner.Id);
            paging.Where("[Transaction].IsRewardPayout = {0}", true);

            paging.OrderByDesc("[Transaction].DateCreated");

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

            model.Rewards = paging.PageItemsList<RewardModel>();

            return model;
        }
    }
}