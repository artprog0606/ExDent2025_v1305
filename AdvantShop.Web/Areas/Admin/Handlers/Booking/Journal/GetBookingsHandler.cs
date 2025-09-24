using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Booking.Journal;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class GetBookingsHandler
    {
        private readonly BookingsFilterModel _filterModel;
        private readonly bool _showByAccess;
        private readonly Customer _currentCustomer;
        private readonly Manager _currentManager;
        private SqlPaging _paging;

        public GetBookingsHandler(BookingsFilterModel filterModel, bool showByAccess = true)
        {
            _filterModel = filterModel;
            _showByAccess = showByAccess;
            _currentCustomer = CustomerContext.CurrentCustomer;
            _currentManager = _currentCustomer.IsManager ? ManagerService.GetManager(_currentCustomer.Id) : null;
        }

        public BookingsFilterResult Execute()
        {
            var model = new BookingsFilterResult();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено броней: {0} на сумму {1}", model.TotalItemsCount,
                PriceFormatService.FormatPrice(
                    _paging.GetCustomData("Sum ([Booking].[Sum]*[BookingCurrency].[CurrencyValue]) as totalPrice", "", reader => SQLDataHelper.GetFloat(reader, "totalPrice"), true).FirstOrDefault(),
                    SettingsCatalog.DefaultCurrency));

            if (_filterModel.AffiliateFilterId.HasValue)
            {
                Dictionary<BookingStatus, int> statuses = null;
                var affiliate = AffiliateService.Get(_filterModel.AffiliateFilterId.Value);

                if (_showByAccess && !_currentCustomer.IsAdmin && !affiliate.AccessForAll && _currentCustomer.IsManager && _currentManager != null && !affiliate.ManagerIds.Contains(_currentManager.ManagerId))
                {
                    statuses = BookingService.GetStatusesWithCountByManager(_filterModel.AffiliateFilterId.Value,
                        _currentManager.ManagerId);
                }

                if (statuses == null)
                    statuses = BookingService.GetStatusesWithCount(_filterModel.AffiliateFilterId.Value);

                foreach (var status in statuses)
                    model.BookingsCount.Add((int) status.Key, status.Value);
            }

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<BookingFilteredResultModel>();

            return model;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("Id");
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "Booking.Id",
                "Booking.AffiliateId",
                "Booking.Status",
                "Affiliate.Name".AsSqlField("AffiliateName"),
                "Booking.ReservationResourceId",
                "isNull([Booking].LastName, '')  + ' ' + isNull([Booking].FirstName, '')  + ' ' + isNull([Booking].Patronymic, '')".AsSqlField("CustomerName"),
                "isNull([ReservationResource].Name, '')".AsSqlField("ReservationResourceName"),
                "Booking.PaymentDate",
                "Booking.Sum",
                "BookingCurrency.CurrencyValue",
                "BookingCurrency.CurrencyCode",
                "BookingCurrency.CurrencySymbol",
                "BookingCurrency.IsCodeBefore",
                "Booking.BeginDate",
                "Booking.EndDate",
                "Booking.DateAdded"
                );

            _paging.From("Booking.Booking");
            _paging.Inner_Join("Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId");
            _paging.Inner_Join("[Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id]");
            _paging.Left_Join("[Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id]");
            _paging.Left_Join("[Order].[PaymentMethod] ON [Booking].[PaymentMethodID] = [Order].[PaymentMethod].[PaymentMethodID]");


            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (_filterModel.BookingId.HasValue)
                _paging.Where("Booking.Id = {0}", _filterModel.BookingId.Value);

            if (_filterModel.AffiliateFilterId.HasValue)
                _paging.Where("Booking.AffiliateId = {0}", _filterModel.AffiliateFilterId.Value);

            if (_filterModel.Status.HasValue)
                _paging.Where("Booking.Status = {0}", (int)_filterModel.Status.Value);

            if (_filterModel.NoStatus.HasValue)
                _paging.Where("Booking.Status != {0}", (int)_filterModel.NoStatus.Value);

            if (_filterModel.ReservationResourceId.HasValue)
                _paging.Where("Booking.ReservationResourceId = {0}", _filterModel.ReservationResourceId.Value);

            if (_filterModel.CustomerId.HasValue)
                _paging.Where("Booking.CustomerId = {0}", _filterModel.CustomerId.Value);

            if (_filterModel.OrderSourceId.HasValue)
                _paging.Where("Booking.OrderSourceId = {0}", _filterModel.OrderSourceId.Value);

            if (_filterModel.PaymentMethodId.HasValue)
            {
                if (_filterModel.PaymentMethodId.Value > 0)
                    _paging.Where("Booking.PaymentMethodID = {0}", _filterModel.PaymentMethodId.Value);
                else
                    _paging.Where("Booking.PaymentMethodID IS NULL");
            }

            if (_filterModel.IsPaid.HasValue)
            {
                if (_filterModel.IsPaid.Value)
                    _paging.Where("Booking.PaymentDate IS NOT NULL");
                else
                    _paging.Where("Booking.PaymentDate IS NULL");
            }

            if (_filterModel.SumFrom.HasValue)
                _paging.Where("Booking.Sum >= {0}", _filterModel.SumFrom.Value);
            if (_filterModel.SumTo.HasValue)
                _paging.Where("Booking.Sum <= {0}", _filterModel.SumTo.Value);

            if (_filterModel.BeginDateFrom.HasValue)
                _paging.Where("Booking.BeginDate >= {0}", _filterModel.BeginDateFrom.Value);
            if (_filterModel.BeginDateTo.HasValue)
                _paging.Where("Booking.BeginDate <= {0}", _filterModel.BeginDateTo.Value);

            if (_filterModel.EndDateFrom.HasValue)
                _paging.Where("Booking.EndDate >= {0}", _filterModel.EndDateFrom.Value);
            if (_filterModel.EndDateTo.HasValue)
                _paging.Where("Booking.EndDate <= {0}", _filterModel.EndDateTo.Value);

            if (_filterModel.DateAddedFrom.HasValue)
                _paging.Where("Booking.DateAdded >= {0}", _filterModel.DateAddedFrom.Value);
            if (_filterModel.DateAddedTo.HasValue)
                _paging.Where("Booking.DateAdded <= {0}", _filterModel.DateAddedTo.Value);

            if (!string.IsNullOrWhiteSpace(_filterModel.Search))
            {
                _paging.Left_Join("[Customers].[Customer] as CustomerCard on [Booking].[CustomerId] = [CustomerCard].[CustomerId]");

                _paging.Where(
                    "([Booking].Id LIKE '%'+{0}+'%' OR " +
                    //"[Lead].FirstName LIKE '%'+{0}+'%' OR " +
                    //"[Lead].LastName LIKE '%'+{0}+'%' OR " +
                    //"[Lead].Patronymic LIKE '%'+{0}+'%' OR " +
                    "[CustomerCard].Phone LIKE '%'+{0}+'%' OR " +
                    "[CustomerCard].Email LIKE '%'+{0}+'%' OR " +
                    "(isNull([Booking].LastName, '') + ' ' + isNull([Booking].FirstName, '') + ' ' + isNull([Booking].Patronymic, '')) LIKE '%'+{0}+'%' OR " +
                    "(isNull([CustomerCard].LastName, '') + ' ' + isNull([CustomerCard].FirstName, '') + ' ' + isNull([CustomerCard].Patronymic, '')) LIKE '%'+{0}+'%' OR " +
                    "[ReservationResource].Name LIKE '%'+{0}+'%' OR " +
                    "[Booking].Phone LIKE '%'+{0}+'%' OR " +
                    "[Booking].Email LIKE '%'+{0}+'%') ",
                    _filterModel.Search);
            }

            if (_showByAccess && !_currentCustomer.IsAdmin && _currentCustomer.IsManager && _currentManager != null)
                _paging.Where(
                    "(Affiliate.AccessForAll = {0} OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = {1}) OR ReservationResource.ManagerId = {1})",
                    true, _currentManager.ManagerId);
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("Booking.DateAdded");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower().Replace("formatted", "");

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
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
