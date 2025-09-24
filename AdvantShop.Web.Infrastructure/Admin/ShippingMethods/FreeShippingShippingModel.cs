using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.FreeShipping;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("FreeShipping")]
    public class FreeShippingShippingAdminModel : ShippingMethodAdminModel
    {
        public string DeliveryTime
        {
            get { return Params.ElementOrDefault(FreeShippingTemplate.DeliveryTime); }
            set { Params.TryAddValue(FreeShippingTemplate.DeliveryTime, value.DefaultOrEmpty()); }
        }
    }
}
