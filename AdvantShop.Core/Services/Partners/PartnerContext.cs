using System.Web;

namespace AdvantShop.Core.Services.Partners
{
    public static class PartnerContext
    {
        private const string PartnerContextKey = "PartnerContext";
        
        public static Partner CurrentPartner
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;

                var cachedPartner = HttpContext.Current.Items[PartnerContextKey] as Partner;
                if (cachedPartner != null)
                    return cachedPartner;

                var partner = PartnerAuthService.GetAuthenticatedPartner();
                HttpContext.Current.Items[PartnerContextKey] = partner;

                return partner;
            }
        }

        public static int PartnerId
        {
            get { return CurrentPartner.Id; }
        }
    }
}