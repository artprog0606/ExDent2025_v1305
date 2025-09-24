using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.GeoModes;
using AdvantShop.Helpers;
using AdvantShop.Models.Location;

namespace AdvantShop.Handlers.Home
{
    public sealed class GetGeoMode
    {
        public GeoModeModel Execute()
        {
            var customer = CustomerContext.CurrentCustomer;
            var mainCustomerContact = customer.Contacts.Find(x => x.IsMain) ?? customer.Contacts.FirstOrDefault();
            
            var shippingType = CommonHelper.GetCookieString(GeoModeConfig.ShippingTypeCookieName);

            var isShowSelfDelivery = TemplateSettingsProvider.Items["ShowSelfDeliveryInDeliveryWidgetOnMain"].TryParseBool();
            var isShowPickPoint = TemplateSettingsProvider.Items["ShowDeliveryInDeliveryWidgetOnMain"].TryParseBool();
            
            shippingType = GeoModeConfig.PickPointType == shippingType
                ? isShowSelfDelivery ? GeoModeConfig.PickPointType : GeoModeConfig.CourierType
                : isShowPickPoint ? GeoModeConfig.CourierType : GeoModeConfig.PickPointType;

            var model = new GeoModeModel
            {
                IsPointSelected = 
                    CommonHelper.GetCookie(GeoModeConfig.PointCookieName) != null || 
                    CommonHelper.GetCookie(GeoModeConfig.PreviousShippingIdCookieName) != null,
                CurrentContact = mainCustomerContact,
                IsUserRegistered = customer.RegistredUser,
                ShippingType = shippingType,
                ShowSelfDelivery = isShowSelfDelivery,
                ShowPickPoint = isShowPickPoint
            };
            
            CommonHelper.SetCookie(GeoModeConfig.ShippingTypeCookieName, shippingType);

            return model;
        }
    }
}