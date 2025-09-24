using AdvantShop.Areas.Api.Model.Orders;
using AdvantShop.Areas.Api.Models.Orders;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Api;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Orders
{
    public class GetOrdersMe : ICommandHandler<GetOrdersMeResponse>
    {
        private readonly OrdersMeFilterModel _filter;

        public GetOrdersMe(OrdersMeFilterModel filter)
        {
            _filter = filter;
        }
        
        public GetOrdersMeResponse Execute()
        {
            var model = new FilterOrdersModel()
            {
                CustomerId = CustomerContext.CustomerId,
                LoadItems = _filter.LoadItems ?? false,
                LoadCustomer = _filter.LoadCustomer ?? false,
                LoadSource = _filter.LoadSource ?? false,
                LoadReview = _filter.LoadReview ?? true,
                LoadBillingApiLink = _filter.LoadBillingApiLink ?? true,
                PreparedPrices = true,
                IsPaid = _filter.IsPaid,
                StatusId = _filter.StatusId,
                IsCompleted = _filter.IsCompleted,
                SumFrom = _filter.SumFrom,
                SumTo = _filter.SumTo,
                DateFrom = _filter.DateFrom,
                DateTo = _filter.DateTo,

                ItemsPerPage = _filter.ItemsPerPage,
                Page = _filter.Page,
                SortingType = FilterSortingType.Desc,
                Sorting = "[OrderID]"
            };
            
            var result = new GetOrders(model).Execute();
            
            return new GetOrdersMeResponse(result);
        }
    }
}