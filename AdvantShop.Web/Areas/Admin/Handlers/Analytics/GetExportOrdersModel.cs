using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.ViewModels.Analytics;

namespace AdvantShop.Web.Admin.Handlers.Analytics
{
    public class GetExportOrdersModel
    {
        public GetExportOrdersModel()
        {
        }

        public ExportOrdersModel Execute()
        {
            var model = new ExportOrdersModel();

            var encodings = new Dictionary<string, string>();
            foreach (var enumItem in (EncodingsEnum[])Enum.GetValues(typeof(EncodingsEnum)))
            {
                encodings.Add(enumItem.StrName(), enumItem.StrName());
            }

            var orderStatuses = new Dictionary<int, string>();
            foreach (var status in OrderStatusService.GetOrderStatuses())
            {
                orderStatuses.Add(status.StatusID, status.StatusName);
            }

            model.PaidStatuses = new Dictionary<bool, string>
            {
                {true, LocalizationService.GetResource("Admin.Marketing.Paid") },
                {false, LocalizationService.GetResource("Admin.Marketing.NotPaid") }
            };

            model.Shippings = new Dictionary<int, string>();
            foreach (var shipping in ShippingMethodService.GetAllShippingMethods())
            {
                model.Shippings.Add(shipping.ShippingMethodId, shipping.Name);
            }

            model.Paid = true;

            model.Encoding = EncodingsEnum.Windows1251.StrName();
            model.Encodings = encodings;
            model.OrderStatuses = orderStatuses;
            model.Status = orderStatuses != null && orderStatuses.Count > 0 ? orderStatuses.FirstOrDefault().Key : 0;

            return model;
        }
    }
}
