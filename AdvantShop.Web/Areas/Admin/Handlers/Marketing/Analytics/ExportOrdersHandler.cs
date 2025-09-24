using System;
using System.IO;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Statistic;
using AdvantShop.Web.Admin.ViewModels.Analytics;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics
{
    public class ExportOrdersHandler
    {
        private readonly ExportOrdersModel _settings;
        private readonly string _strFilePath;
        private readonly string _strFullPath;
        private const string _filePrefix = "StatisticsOrders";
        private const string _fileExt = ".csv";
        private readonly string _fileName;

        public ExportOrdersHandler(ExportOrdersModel settings)
        {
            _settings = settings;

            _strFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(_strFilePath);
            _fileName = (_filePrefix + _fileExt).FileNamePlusDate();
            _strFullPath = Path.Combine(_strFilePath, _fileName);
        }

        public void Execute()
        {
            if (CommonStatistic.IsRun)
                return;
            
            var paging = new SqlPaging
            {
                TableName = "[Order].[Order] " +
                            "LEFT JOIN [Order].[OrderCustomer] ON [Order].OrderId = [OrderCustomer].[OrderId] " +
                            "LEFT JOIN [Order].[OrderCoupon] ON [Order].OrderId = [OrderCoupon].[OrderId] " +
                            "LEFT JOIN [Order].[OrderRecipient] ON [Order].OrderId = [OrderRecipient].[OrderId] "
            };
            paging.AddField(new Field { Name = "[Order].*" });

            paging.AddField(new Field
            {
                Name = "[Order].[IsDraft]",
                NotInQuery = true,
                Filter = new EqualFieldFilter { ParamName = "@IsDraft", Value = "0" }
            });

            if (_settings.UseStatus)
            {
                paging.AddField(new Field
                {
                    Name = "[Order].OrderStatusID",
                    NotInQuery = true,
                    Filter = new EqualFieldFilter { ParamName = "@OrderStatusID", Value = _settings.Status.ToString() }
                });
            }

            if (_settings.UseSource)
            {
                paging.AddField(new Field
                {
                    Name = "[Order].OrderSourceId",
                    NotInQuery = true,
                    Filter = new EqualFieldFilter { ParamName = "@OrderSourceId", Value = _settings.Source.ToString() }
                });
            }

            if (_settings.UsePaid)
            {
                paging.AddField(new Field
                {
                    Name = "[Order].PaymentDate",
                    NotInQuery = true,
                    Filter = new NullFieldFilter { ParamName = "@PaymentDate", Null = !_settings.Paid }
                });
            }

            if (_settings.UseShipping)
            {
                paging.AddField(new Field
                {
                    Name = "[Order].ShippingMethodID",
                    NotInQuery = true,
                    Filter = new EqualFieldFilter { ParamName = "@ShippingMethodID", Value = _settings.Shipping.ToString() }
                });
            }
            
            if (_settings.UsePayment)
            {
                paging.AddField(new Field
                {
                    Name = "[Order].PaymentMethodID",
                    NotInQuery = true,
                    Filter = new EqualFieldFilter { ParamName = "@PaymentMethodID", Value = _settings.Payment.ToString() }
                });
            }

            if (_settings.UseCity)
            {
                paging.AddField(new Field
                {
                    Name = "[OrderCustomer].City",
                    NotInQuery = true,
                    Filter = new EqualFieldFilter { ParamName = "@City", Value = _settings.City }
                });
            }

            if (_settings.UseSum)
            {
                var filter = new RangeFieldFilter
                {
                    ParamName = "@Sum",
                    From = _settings.OrderSumFrom,
                    To = _settings.OrderSumTo
                };
                paging.AddField(new Field { Name = "[Order].Sum", NotInQuery = true, Filter = filter });
            }

            if (_settings.UseDate)
            {
                var filter = new DateTimeRangeFieldFilter { ParamName = "@RDate" };
                var dateFrom = _settings.DateFrom;
                filter.From = dateFrom != DateTime.MinValue ? dateFrom : new DateTime(2000, 1, 1);

                var dateTo = _settings.DateTo;
                filter.To = dateTo != DateTime.MinValue ? dateTo : new DateTime(3000, 1, 1);
                paging.AddField(new Field { Name = "[Order].OrderDate", NotInQuery = true, Filter = filter });
            }

            if (_settings.UseShippingDate)
            {
                var filter = new DateTimeRangeFieldFilter { ParamName = "@RShippingDate" };
                var dateFrom = _settings.ShippingDateFrom;
                filter.From = dateFrom != DateTime.MinValue ? dateFrom : new DateTime(2000, 1, 1);

                var dateTo = _settings.ShippingDateTo;
                filter.To = dateTo != DateTime.MinValue ? dateTo : new DateTime(3000, 1, 1);
                paging.AddField(new Field { Name = "[Order].DeliveryDate", NotInQuery = true, Filter = filter });
            }

            if (_settings.UseBonusCost)
            {
                paging.AddField(new Field
                {
                    Name = "[Order].BonusCost",
                    NotInQuery = true,
                    Filter = new NotEqualFieldFilter { ParamName = "@BonusCost", Value = "0" }
                });
            }

            if (_settings.UseCouponCode)
            {
                paging.AddField(new Field
                {
                    Name = "OrderCoupon.Code",
                    NotInQuery = true,
                    Filter = new EqualFieldFilter { ParamName = "@CouponCode", Value = _settings.CouponCode }
                });
            }

            var ordersCount = paging.TotalRowsCount;
            
            try
            {
                if (!Directory.Exists(_strFilePath))
                    Directory.CreateDirectory(_strFilePath);

                foreach (var item in Directory.GetFiles(_strFilePath).Where(f => f.Contains(_filePrefix)))
                    FileHelpers.DeleteFile(item);
                

                CommonStatistic.StartNew(() =>
                    {
                        CommonStatistic.TotalRow = ordersCount;
                        Save(paging);
                    },
                    "analytics?analyticsReportTab=exportOrders",
                    LocalizationService.GetResource("Admin.Analytics.ExportOrders.ProcessName"),
                    UrlService.GetUrl(FoldersHelper.PhotoFoldersPath[FolderType.PriceTemp] + _fileName));
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message, ex);
            }
        }

        protected void Save(SqlPaging paging)
        {
            var orders = paging.GetCustomData("[Order].*", "", OrderService.GetOrderFromReader);
            new OrderCsvExport().MultiOrder(orders, _strFullPath, _settings.Encoding, _settings.UseOrderItemsInString, _settings.CustomOptionOptionsSeparator);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_ExportOrders);
        }
    }
}
