using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Model.Orders;
using AdvantShop.Core.Services.Webhook.Models.Api;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Infrastructure.Api;

namespace AdvantShop.Areas.Api.Handlers.Orders
{
    public class GetOrders : EntitiesHandler<FilterOrdersModel, OrderModel>
    {
        public GetOrders(FilterOrdersModel filterModel) : base(filterModel) { }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select(
                    "[Order].[OrderID]",
                    "[Order].[Number]",
                    "[Order].[Sum]",
                    "[Order].[OrderDate]",
                    "[Order].[Code]",

                    "[Order].[CustomerComment]",
                    "[Order].[AdminOrderComment]",

                    "[Order].[PaymentMethodName]",
                    "[Order].[PaymentCost]",

                    "[Order].[ShippingMethodName]",
                    "[Order].[ShippingCost]",
                    "[Order].[ShippingTaxType]",
                    "[Order].[TrackNumber]",
                    "[Order].[DeliveryDate]",
                    "[Order].[DeliveryTime]",

                    "[Order].[OrderDiscount]",
                    "[Order].[OrderDiscountValue]",

                    "[Order].[BonusCardNumber]",
                    "[Order].[BonusCost]",

                    "[Order].[LpId]",

                    "[Order].[PaymentDate]",
                    "[Order].[ModifiedDate]",
                    "[Order].[IsDraft]",

                    "[OrderCurrency].[CurrencyCode]",
                    "[OrderCurrency].[CurrencyValue]",
                    "[OrderCurrency].[CurrencySymbol]",
                    "[OrderCurrency].[IsCodeBefore]",
                    "[OrderCurrency].[RoundNumbers]",
                    "[OrderCurrency].[EnablePriceRounding]",

                    "[OrderCustomer].[CustomerID]",
                    "[OrderCustomer].[FirstName]",
                    "[OrderCustomer].[LastName]",
                    "[OrderCustomer].[Patronymic]",
                    "[OrderCustomer].[Organization]",
                    "[OrderCustomer].[Email]",
                    "[OrderCustomer].[Phone]",
                    "[OrderCustomer].[Country]",
                    "[OrderCustomer].[Region]",
                    "[OrderCustomer].[District]",
                    "[OrderCustomer].[City]",
                    "[OrderCustomer].[Zip]",
                    "[OrderCustomer].[CustomField1]",
                    "[OrderCustomer].[CustomField2]",
                    "[OrderCustomer].[CustomField3]",
                    "[OrderCustomer].[Street]",
                    "[OrderCustomer].[House]",
                    "[OrderCustomer].[Apartment]",
                    "[OrderCustomer].[Structure]",
                    "[OrderCustomer].[Entrance]",
                    "[OrderCustomer].[Floor]",

                    "[Order].[OrderStatusID]",
                    "[OrderStatus].[StatusName]",
                    "[OrderStatus].[Color]".AsSqlField("StatusColor"),
                    "[OrderStatus].[IsCanceled]".AsSqlField("StatusIsCanceled"),
                    "[OrderStatus].[IsCompleted]".AsSqlField("StatusIsCompleted"),
                    "[OrderStatus].[Hidden]".AsSqlField("StatusHidden"),
                    "[OrderStatus].[CancelForbidden]".AsSqlField("StatusIsCancellationForbidden"),

                    "[Order].[OrderSourceId]",
                    "[OrderSource].[Name]".AsSqlField("SourceName"),
                    "[OrderSource].[Main]".AsSqlField("SourceMain"),
                    "[OrderSource].[Type]".AsSqlField("SourceType")
                )
                .From("[Order].[Order]")
                .Left_Join("[Order].[OrderCustomer] ON [Order].[OrderID]=[OrderCustomer].[OrderID]")
                .Left_Join("[Order].[OrderStatus] ON [OrderStatus].[OrderStatusID]=[Order].[OrderStatusID]")
                .Left_Join("[Order].[OrderCurrency] ON [Order].[OrderID] = [OrderCurrency].[OrderID]")
                .Left_Join("[Order].[OrderSource] on [Order].[OrderSourceId] = [OrderSource].[Id]");

            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            paging.Where("[Order].[IsDraft] != 1");

            if (FilterModel.CustomerId.HasValue)
                paging.Where("[OrderCustomer].[CustomerID] = {0}", FilterModel.CustomerId.Value);

            if (FilterModel.StatusId.HasValue)
                paging.Where("[Order].[OrderStatusID] = {0}", FilterModel.StatusId.Value);

            if (FilterModel.SumFrom.HasValue)
            {
                paging.Where("[Order].[Sum] >= {0}", FilterModel.SumFrom.Value);
            }
            if (FilterModel.SumTo.HasValue)
            {
                paging.Where("[Order].[Sum] <= {0}", FilterModel.SumTo.Value);
            }

            if (FilterModel.IsPaid.HasValue)
            {
                paging.Where(FilterModel.IsPaid.Value ? "PaymentDate is not null" : "PaymentDate is null");
            }

            if (FilterModel.IsCompleted.HasValue)
            {
                paging.Where("[OrderStatus].[IsCompleted] = {0}", FilterModel.IsCompleted.Value ? 1 : 0);
            }

            if (!string.IsNullOrWhiteSpace(FilterModel.DateFrom) && DateTime.TryParse(FilterModel.DateFrom, out var from))
            {
                paging.Where("OrderDate >= {0}", from);
            }
            if (!string.IsNullOrWhiteSpace(FilterModel.DateTo) && DateTime.TryParse(FilterModel.DateTo, out var to))
            {
                paging.Where("OrderDate <= {0}", to);
            }
            
            if (!string.IsNullOrWhiteSpace(FilterModel.ModifiedDateFrom) && DateTime.TryParse(FilterModel.ModifiedDateFrom, out var modifiedFrom))
            {
                paging.Where("ModifiedDate >= {0}", modifiedFrom);
            }
            if (!string.IsNullOrWhiteSpace(FilterModel.ModifiedDateTo) && DateTime.TryParse(FilterModel.ModifiedDateTo, out var modifiedTo))
            {
                paging.Where("ModifiedDate <= {0}", modifiedTo);
            }

            return paging;
        }

        protected override SqlPaging Sorting(SqlPaging paging)
        {
            if (string.IsNullOrEmpty(FilterModel.Sorting) || FilterModel.SortingType == FilterSortingType.None)
            {
                paging.OrderBy("[Order].[OrderID]");

                return paging;
            }

            var sorting = FilterModel.Sorting;

            var field = paging.SelectFields().FirstOrDefault(x => x.FieldName.Equals(sorting, StringComparison.OrdinalIgnoreCase));
            if (field != null)
            {
                if (FilterModel.SortingType == FilterSortingType.Asc)
                    paging.OrderBy(sorting);
                else
                    paging.OrderByDesc(sorting);
            }

            return paging;
        }

        protected override List<OrderModel> FillItems(SqlPaging paging)
        {
            return paging.PageItemsList(reader =>
                OrderModel.FromReader(reader, FilterModel.LoadItems, 
                                        FilterModel.LoadSource ?? true,
                                        FilterModel.LoadCustomer ?? true,
                                        FilterModel.PreparedPrices ?? false,
                                        FilterModel.LoadReview ?? false,
                                        FilterModel.LoadBillingApiLink ?? false));
        }
    }
}