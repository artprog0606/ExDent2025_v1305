using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Shipping.Sdek.Api;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Sdek
{
    public class SdekOrderPrintForm
    {
        private readonly int _orderId;
        public List<string> Errors;

        public SdekOrderPrintForm(int orderId)
        {
            _orderId = orderId;
        }

        public Tuple<string, string> Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return null;
            
            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != "Sdek")
                return null;
            
            try
            {
                var sdekOrderUuid = OrderService.GetOrderAdditionalData(order.OrderID, Shipping.Sdek.Sdek.KeyNameSdekOrderUuidInOrderAdditionalData);
                var sdekOrderNumber = OrderService.GetOrderAdditionalData(order.OrderID, Shipping.Sdek.Sdek.KeyNameDispatchNumberInOrderAdditionalData);
                var sdek = new Shipping.Sdek.Sdek(shippingMethod, null);
                
                var createPrintFormResult = sdek.SdekApiService20.CreatePrintForm(new CreatePrintForm()
                {
                    Orders = new List<CreatePrintFormOrder>()
                    {
                        new CreatePrintFormOrder()
                        {
                            OrderUuid = sdekOrderUuid.TryParseGuid(true),
                            CdekNumber = sdekOrderNumber.TryParseInt(true)
                        }
                    }
                });

                if (createPrintFormResult?.Entity?.Uuid != null)
                {
                    if (createPrintFormResult.Requests.First().State != "INVALID")
                    {
                        GetPrintFormResult getPrintFormResult;
                        int countRequests = 0;
                        do 
                        {
                            if (countRequests > 0)
                                System.Threading.Tasks.Task.Delay(countRequests > 4 ? 1000 : 500).Wait();
                            getPrintFormResult = sdek.SdekApiService20.GetPrintForm(createPrintFormResult.Entity.Uuid);
                            countRequests++;
                        }  while (getPrintFormResult != null && getPrintFormResult?.Entity?.Statuses?.All(x => x.Code != "INVALID" && x.Code != "READY") == true && countRequests < 10);

                        if (getPrintFormResult?.Entity?.Url.IsNotEmpty() == true)
                        {
                            var path = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
                            FileHelpers.CreateDirectory(path);
                            
                            var fullFilePath = sdek.SdekApiService20.DownloadFile(getPrintFormResult.Entity.Url, path);
                        
                            if (!File.Exists(fullFilePath))
                                return null;

                            return new Tuple<string, string>(fullFilePath, Path.GetFileName(fullFilePath));
                        } 
                        else if (getPrintFormResult?.Requests != null)
                        {
                            Errors = getPrintFormResult.Requests
                                .Where(x => x.Errors != null)
                                .SelectMany(request => request.Errors.Select(error => error.Message))
                                .ToList();
                        }
                    }
                }
                else if (sdek.SdekApiService20.LastActionErrors != null)
                {
                    Errors = sdek.SdekApiService20.LastActionErrors;
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }
    }
}
