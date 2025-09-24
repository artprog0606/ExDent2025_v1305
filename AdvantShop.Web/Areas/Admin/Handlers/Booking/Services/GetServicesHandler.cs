using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Booking.Services;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Booking.Services
{
    public class GetServicesHandler
    {
        private readonly ServicesFilterModel _filterModel;
        private SqlPaging _paging;

        public GetServicesHandler(ServicesFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public ServicesFilterResult Execute()
        {
            var model = new ServicesFilterResult();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено услуг: {0}", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<ServiceModel>();

            return model;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("Service.Id");
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "Service.Id",
                "Service.CategoryId",
                "Service.CurrencyId",
                "Service.ArtNo",
                "Service.Name",
                "Service.Price",
                "Service.Image",
                "Service.SortOrder",
                "Service.Enabled"
                );

            _paging.From("Booking.Service");

            if (_filterModel.CategoryFilterId > 0)
            {
                _paging.Where("CategoryId = {0}", _filterModel.CategoryFilterId);
            }

            if (_filterModel.AffiliateId.HasValue)
            {
                _paging.Select(
                    "AffiliateService.AffiliateId",
                    "ISNULL(AffiliateService.AffiliateId, 0)".AsSqlField("BindAffiliate")
                    );
                _paging.Inner_Join("[Booking].[AffiliateService] ON [Service].[Id] = [AffiliateService].[ServiceId]");
                _paging.Where("AffiliateService.AffiliateId = {0}", _filterModel.AffiliateId.Value);

                if (_filterModel.ReservationResourceId.HasValue)
                {
                    _paging.Inner_Join("[Booking].[ReservationResourceService] ON [Service].[Id] = [ReservationResourceService].[ServiceId]");
                    _paging.Where("ReservationResourceService.AffiliateId = {0}", _filterModel.AffiliateId.Value);
                    _paging.Where("ReservationResourceService.ReservationResourceId = {0}", _filterModel.ReservationResourceId.Value);
                }
            }
            else if(_filterModel.LeftJoinAffiliateId.HasValue)
            {
                _paging.Select(
                    string.Format("{0}", _filterModel.LeftJoinAffiliateId.Value).AsSqlField("AffiliateId"),
                    "ISNULL(AffiliateService.AffiliateId, 0)".AsSqlField("BindAffiliate")
                    );
                _paging.Left_Join("[Booking].[AffiliateService] ON [Service].[Id] = [AffiliateService].[ServiceId] AND AffiliateService.AffiliateId = {0}", _filterModel.LeftJoinAffiliateId.Value);
            }

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (_filterModel.Search.IsNotEmpty())
            {
                _paging.Where(
                    "(Service.Name LIKE '%'+{0}+'%' OR Service.ArtNo LIKE '%'+{0}+'%')",
                    _filterModel.Search);
            }
            if (_filterModel.ArtNo.IsNotEmpty())
            {
                _paging.Where("Service.ArtNo = {0}", _filterModel.ArtNo);
            }
            if (_filterModel.Name.IsNotEmpty())
            {
                _paging.Where("Service.Name LIKE '%'+{0}+'%'", _filterModel.Name);
            }
            if (_filterModel.Enabled.HasValue)
            {
                _paging.Where("Service.Enabled = {0}", _filterModel.Enabled.Value);
            }
            if (_filterModel.HasPhoto.HasValue)
            {
                _paging.Where(_filterModel.HasPhoto.Value ? "(Service.Image is not null and Service.Image <> '')" : "(Service.Image is null or Service.Image = '')");
            }
            if (_filterModel.SortingFrom.HasValue)
            {
                _paging.Where("Service.SortOrder >= {0}", _filterModel.SortingFrom.Value);
            }
            if (_filterModel.SortingTo.HasValue)
            {
                _paging.Where("Service.SortOrder <= {0}", _filterModel.SortingTo.Value);
            }
            if (!_filterModel.AffiliateId.HasValue && _filterModel.LeftJoinAffiliateId.HasValue && _filterModel.HasAffiliate.HasValue)
            {
                if (_filterModel.HasAffiliate.Value)
                    _paging.Where("AffiliateService.AffiliateId IS NOT NULL");
                else
                    _paging.Where("AffiliateService.AffiliateId IS NULL");
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("Service.SortOrder");
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
