using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Quartz;

namespace AdvantShop.Shipping
{
    [DisallowConcurrentExecution]
    public class MaintenanceShippingsJob : IJob
    {
        private static readonly object Sync = new object();

        public void Execute(IJobExecutionContext context)
        {
            lock (Sync)
            {
                try
                {
                    foreach (var shipping in GetSupportedShipping())
                    {
                        try
                        {
                            ((IShippingWithBackgroundMaintenance)shipping).ExecuteJob();
                        }
                        catch (Exception ex)
                        {
                            Debug.Log.Error(ex);
                        }
                    }
                }
                catch (BlException ex)
                {
                    Debug.Log.Error(ex);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
        }

        private static List<BaseShipping> GetSupportedShipping()
        {
            var supportedTypes = ReflectionExt.GetTypesWith<ShippingKeyAttribute>(false)
                .Where(x => x.GetInterfaces().Contains(typeof(IShippingWithBackgroundMaintenance)) && x.IsSubclassOf(typeof(BaseShipping)))
                .Select(AttributeHelper.GetAttributeValue<ShippingKeyAttribute, string>)
                .ToList();

            return ShippingMethodService.GetAllShippingMethods()
                .Where(method => method.Enabled && supportedTypes.Contains(method.ShippingType))
                .Select(method =>
                {
                    var type = ReflectionExt.GetTypeByAttributeValue<ShippingKeyAttribute>(typeof (BaseShipping), atr => atr.Value, method.ShippingType);
                    return (BaseShipping) Activator.CreateInstance(type, method, null);
                })
                //.Where(x => ((IShippingWithBackgroundMaintenance)x.Value).StatusesSync)
                .ToList();
        }
    }
}
