using System;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Booking
{
    [SaasFeature(ESaasProperty.HaveBooking)]
    [Auth(RoleAction.Booking)]
    [AccessBySettings(Core.Services.Configuration.EProviderSetting.BookingActive, ETypeRedirect.AdminPanel)]
    public class BaseBookingController : BaseAdminController
    {
        private const string SelectedAffiliateCookieName = "affiliate_selected";
        private const string AffiliateContext = "AffiliateContext";

        protected bool SelectAffiliate(int id)
        {
            var affiliate = AffiliateService.Get(id);

            if (AffiliateService.CheckAccess(affiliate))
            {
                CommonHelper.SetCookie(SelectedAffiliateCookieName, id.ToString(), new TimeSpan(365, 0, 0, 0), false);

                _selectedAffiliate = affiliate;
                System.Web.HttpContext.Current.Items[AffiliateContext] = _selectedAffiliate;
                _loadedSelectedAffiliate = true;
                return true;
            }

            return false;
        }

        private Affiliate _selectedAffiliate;
        private bool _loadedSelectedAffiliate = false;
        protected Affiliate SelectedAffiliate
        {
            get
            {
                if (_selectedAffiliate == null && !_loadedSelectedAffiliate && System.Web.HttpContext.Current != null)
                {
                    Affiliate affiliate = System.Web.HttpContext.Current.Items[AffiliateContext] as Affiliate;

                    if (affiliate == null)
                    {
                        var cookieAffiliateId = CommonHelper.GetCookieString(SelectedAffiliateCookieName).TryParseInt(true);
                        affiliate = cookieAffiliateId.HasValue ? AffiliateService.Get(cookieAffiliateId.Value) : null;

                        if (affiliate == null || !AffiliateService.CheckAccess(affiliate))
                            affiliate = AffiliateService.GetList().Where(AffiliateService.CheckAccess).FirstOrDefault();


                        if (affiliate != null && cookieAffiliateId != affiliate.Id)
                        {
                            if (SelectAffiliate(affiliate.Id))
                                return _selectedAffiliate;
                        }
                    }

                    _selectedAffiliate = affiliate;
                    System.Web.HttpContext.Current.Items[AffiliateContext] = _selectedAffiliate;
                    _loadedSelectedAffiliate = true;
                }
                return _selectedAffiliate;
            }
        }
    }
}
