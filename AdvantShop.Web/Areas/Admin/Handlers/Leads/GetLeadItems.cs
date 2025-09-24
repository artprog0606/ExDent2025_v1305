using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.SQL2;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Models.Crm.Leads;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class GetLeadItems : ICommandHandler<FilterResult<LeadItemModel>>
    {
        private readonly LeadItemsFilterModel _filterModel;
        private readonly UrlHelper _urlHelper;
        private SqlPaging _paging;

        public GetLeadItems(LeadItemsFilterModel filterModel)
        {
            _filterModel = filterModel;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public FilterResult<LeadItemModel> Execute()
        {
            var model = new FilterResult<LeadItemModel>();

            GetPaging();
            
            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено товаров: {0}", model.TotalItemsCount);
            
            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
                return model;
            
            model.DataItems = new List<LeadItemModel>(); 
            
            var currency = LeadService.GetLeadCurrency(_filterModel.LeadId) ?? 
                           CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3);
            var pageItems = _paging.PageItemsList<LeadItem>();
            
            foreach (var item in pageItems)
            {
                var p = item.ProductId != null ? ProductService.GetProduct(item.ProductId.Value) : null;

                var leadItem = new LeadItemModel()
                {
                    LeadItemId = item.LeadItemId,
                    LeadId = item.LeadId,
                    Enabled = p != null && p.Enabled,
                    ImageSrc = item.Photo.ImageSrcSmall(),
                    ArtNo = item.ArtNo,
                    Name = item.Name,
                    ProductLink = p != null ? _urlHelper.AbsoluteActionUrl("Edit", "Product", new { id = p.ProductId }) : null,

                    Color = !string.IsNullOrEmpty(item.Color) ? SettingsCatalog.ColorsHeader + ": " + item.Color : "",
                    Size = !string.IsNullOrEmpty(item.Size) ? SettingsCatalog.SizesHeader + ": " + item.Size : "",
                    Price = item.Price,
                    Amount = item.Amount,
                    Cost = (item.Price*item.Amount).FormatPrice(currency),
                    Width = item.Width,
                    Length = item.Length,
                    Height = item.Height,
                    Weight = item.Weight,

                    BarCode = item.BarCode,

                    CustomOptions = RenderCustomOptions(CustomOptionsService.DeserializeFromJson(item.CustomOptionsJson, currency.CurrencyValue), currency)
                };
                model.DataItems.Add(leadItem);
            }

            return model;
        }

        public static string RenderCustomOptions(List<EvaluatedCustomOptions> evlist, Currency currency)
        {
            if (evlist == null || !evlist.Any())
                return string.Empty;

            var html = new StringBuilder();
            foreach (var ev in evlist)
            {
                html.AppendFormat(
                    "<div class=\"orderitem-option\"><span class=\"orderitem-option-name\">{0}:</span> <span class=\"orderitem-option-value\">{1} {3}</span></div>",
                    ev.CustomOptionTitle, ev.OptionTitle, ev.GetFormatPrice(currency), ev.OptionAmount > 1 ? "x " + ev.OptionAmount.ToString() : "");
            }

            return html.ToString();
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "LeadItem.LeadItemId"
                ,"LeadItem.LeadId"
                ,"LeadItem.PhotoId"
                ,"LeadItem.ArtNo"
                ,"LeadItem.Name"
                ,"LeadItem.ProductId"
                ,"LeadItem.Color" 
                ,"LeadItem.Size"
                ,"LeadItem.Price" 
                ,"LeadItem.Amount"
                ,"LeadItem.Width"
                ,"LeadItem.Length"
                ,"LeadItem.Height"
                ,"LeadItem.Weight"
                ,"LeadItem.BarCode"
                ,"LeadItem.CustomOptionsJson"
                
                ,"LeadItem.Price * LeadItem.Amount".AsSqlField("Cost")
            );

            _paging.From("[Order].[LeadItem]");
            _paging.Where("LeadId = {0}", _filterModel.LeadId);

            Sorting();
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
                return;
            
            var sorting = _filterModel.Sorting.ToLower();
            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);

            if (field == null) return;
            
            if (_filterModel.SortingType == FilterSortingType.Asc)
                _paging.OrderBy(sorting);
            else
                _paging.OrderByDesc(sorting);
        }
    }
}
