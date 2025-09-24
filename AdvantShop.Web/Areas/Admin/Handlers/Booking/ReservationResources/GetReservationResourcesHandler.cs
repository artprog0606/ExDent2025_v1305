using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Booking.ReservationResources;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Booking.ReservationResources
{
    public class GetReservationResourcesHandler
    {
        private readonly ReservationResourcesFilterModel _filterModel;
        private readonly bool _showByAccess;
        private readonly Customer _currentCustomer;
        private readonly Manager _currentManager;
        private SqlPaging _paging;

        public GetReservationResourcesHandler(ReservationResourcesFilterModel filterModel, bool showByAccess = true)
        {
            _filterModel = filterModel;
            _showByAccess = showByAccess;
            _currentCustomer = CustomerContext.CurrentCustomer;
            _currentManager = _currentCustomer.IsManager ? ManagerService.GetManager(_currentCustomer.Id) : null;
        }

        public ReservationResourcesFilterResult Execute()
        {
            var model = new ReservationResourcesFilterResult();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено ресурсов бронирования: {0}", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<ReservationResourceGridModel>();

            return model;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("ReservationResource.Id");
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "ReservationResource.Id",
                "ReservationResource.Name",
                "ReservationResource.Description",
                "ReservationResource.Enabled",
                "ReservationResource.SortOrder",
                "ReservationResource.Image",
                string.Format("ISNULL(AffiliateReservationResource.AffiliateId, {0})", _filterModel.AffiliateFilterId).AsSqlField("AffiliateId"),
                "AffiliateReservationResource.BookingIntervalMinutes",
                "ISNULL(AffiliateReservationResource.AffiliateId, 0)".AsSqlField("BindAffiliate")
                );

            _paging.From("Booking.ReservationResource");
            _paging.Left_Join("Booking.AffiliateReservationResource ON ReservationResource.Id = AffiliateReservationResource.ReservationResourceId AND AffiliateReservationResource.AffiliateId = {0}", _filterModel.AffiliateFilterId);

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (_filterModel.Search.IsNotEmpty())
            {
                _paging.Where("ReservationResource.Name LIKE '%'+{0}+'%'", _filterModel.Search);
            }
            if (_filterModel.Enabled.HasValue)
            {
                _paging.Where("ReservationResource.Enabled = {0}", _filterModel.Enabled.Value);
            }
            if (_filterModel.HasPhoto.HasValue)
            {
                _paging.Where(_filterModel.HasPhoto.Value ? "(Image is not null and Image <> '')" : "(Image is null or Image = '')");
            }
            if (_filterModel.SortingFrom.HasValue)
            {
                _paging.Where("ReservationResource.SortOrder >= {0}", _filterModel.SortingFrom.Value);
            }
            if (_filterModel.SortingTo.HasValue)
            {
                _paging.Where("ReservationResource.SortOrder <= {0}", _filterModel.SortingTo.Value);
            }
            if (_filterModel.HasAffiliate.HasValue)
            {
                if (_filterModel.HasAffiliate.Value)
                    _paging.Where("AffiliateReservationResource.AffiliateId IS NOT NULL");
                else
                    _paging.Where("AffiliateReservationResource.AffiliateId IS NULL");
            }

            if (_showByAccess && !_currentCustomer.IsAdmin && _currentCustomer.IsManager && _currentManager != null)
            {
                _paging.Inner_Join("Booking.Affiliate ON Affiliate.Id = AffiliateReservationResource.AffiliateId");

                _paging.Where(
                    "(Affiliate.AccessForAll = {0} OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = {1}) OR ReservationResource.ManagerId = {1})",
                    true, _currentManager.ManagerId);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("ReservationResource.SortOrder");
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
