using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;
using AdvantShop.Shipping;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("PickPoint")]
    public class PickPointPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string ShippingMethodTemplate
        {
            get { return Parameters.ElementOrDefault(PickPoint.ShippingMethodTemplate); }
            set { Parameters.TryAddValue(PickPoint.ShippingMethodTemplate, value.DefaultOrEmpty()); }
        }

        public string AvailableShippings
        {
            get
            {
                var availableShippings = new List<string>();
                var listShippings = AdvantshopConfigService.GetDropdownShippings();
                var listModules = Core.Modules.ModulesExecuter.GetDropdownShippings();
                foreach (var shippingType in PickPoint.GetAvailableShippingKeys())
                {
                    var type = listShippings.FirstOrDefault(x => x.Value.Equals(shippingType, StringComparison.OrdinalIgnoreCase));
                    if (type == null)
                        type = listModules.FirstOrDefault(x => x.Value.Equals(shippingType, StringComparison.OrdinalIgnoreCase));
                    if (type != null)
                        availableShippings.Add(type != null ? type.Text : shippingType);
                }
                return string.Join(", ", availableShippings);
            }
        }

        public List<SelectListItem> ShippingMethodTemplates
        {
            get
            {
                var keys = PickPoint.GetAvailableShippingKeys();
                var methods = new List<ShippingMethod>();

                foreach (var key in keys)
                {
                    methods.AddRange(ShippingMethodService.GetShippingMethodByType(key, active: false));
                }

                var shippingKeys = new List<SelectListItem>() {new SelectListItem() {Text = "Не выбрано", Value = ""}};

                foreach (var method in methods)
                {
                    shippingKeys.Add(new SelectListItem { Text = method.Name, Value = method.ShippingMethodId.ToString() });
                }
                
                var shippingKey = shippingKeys.Find(x => x.Value == ShippingMethodTemplate);
                if (shippingKey != null)
                    shippingKey.Selected = true;
                
                return shippingKeys;
            }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
