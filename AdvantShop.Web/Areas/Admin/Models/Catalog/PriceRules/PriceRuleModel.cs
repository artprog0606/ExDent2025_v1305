using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Customers;
using AdvantShop.Payment;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.PriceRules
{
    public class PriceRuleModel : PriceRule
    {
        public string TypeStr
        {
            get
            {
                var str = "";

                var groupNames = new List<string>();
                
                foreach (var customerGroupId in CustomerGroupIds)
                {
                    var group = CustomerGroupService.GetCustomerGroup(customerGroupId);
                    if (group != null)
                        groupNames.Add(group.GroupName);
                    else
                        return $"Ошибка. Группы покупателей #{customerGroupId} не существует.";
                }
                
                if (groupNames.Count > 0)
                    str += $"{(groupNames.Count == 1 ? "Группа" : "Группы")} покупателей {string.Join(", ", groupNames.Select(x => "\"" + x + "\""))} ";

                if (Amount != 0)
                    str += "от " + Amount + " шт. ";

                if (PaymentMethodId != null && PaymentMethodId != 0)
                {
                    var paymentMethod = PaymentService.GetPaymentMethod(PaymentMethodId.Value);
                    if (paymentMethod != null)
                        str += "при оплате \"" + paymentMethod.Name + "\" ";
                    else
                        return "Ошибка. Метода оплаты не существует.";
                }

                if (ShippingMethodId != null && ShippingMethodId != 0)
                {
                    var shippingMethod = ShippingMethodService.GetShippingMethod(ShippingMethodId.Value);
                    if (shippingMethod != null)
                        str += "при доставке \"" + shippingMethod.Name + "\" ";
                    else
                        return "Ошибка. Метода доставки не существует.";
                }

                if (WarehouseIds.Count > 0)
                {
                    var warehouses = new List<string>();
                    
                    foreach (var warehouseId in WarehouseIds)
                    {
                        var warehouse = WarehouseService.Get(warehouseId);
                        if (warehouse != null)
                            warehouses.Add(warehouse.Name);
                        else
                            return $"Ошибка. Склад #{warehouseId} не существует.";
                    }
                    
                    if (warehouses.Count > 0)
                        str += $"{(warehouses.Count == 1 ? "Склад" : "Склады")} {string.Join(", ", warehouses.Select(x => "\"" + x + "\""))} ";
                }

                str += !ApplyDiscounts ? "(скидки не применяются)" : "(скидки применяются)";

                return str;
            }
        }

        public bool IsUsed { get; set; }
    }

    public class PriceRuleFilterModel : BaseFilterModel
    {
        public string Name { get; set; }
        
        public bool? Enabled { get; set; }
    }
}
