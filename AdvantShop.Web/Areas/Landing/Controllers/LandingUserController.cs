using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Models.Users;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Customers;
using AdvantShop.Helpers;

namespace AdvantShop.App.Landing.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class LandingUserController : LandingBaseController
    {
        #region Ctor

        private readonly LpService _lpService;

        public LandingUserController()
        {
            _lpService = new LpService();
        }

        #endregion

        private Lp GetLp(int id)
        {
            var lp = _lpService.Get(id);
            if (lp != null)
                LpService.CurrentLanding = lp;

            return lp;
        }
        
        public ActionResult Auth(int id)
        {
            var lp = GetLp(id);
            if (lp == null)
                return Error404();
            
            var customer = CustomerContext.CurrentCustomer;

            SetMetaInformation(T("Авторизация"));

            if (customer.RegistredUser)
                return View("AuthNotAccess");

            var authRegUrl = LPageSettings.AuthRegUrl.IsNotEmpty() ? LPageSettings.AuthRegUrl : LSiteSettings.AuthRegUrl;
            var model = new AuthViewModel()
            {
                From = _lpService.GetLpLinkRelative(lp.Id),
                RegistrationUrl = authRegUrl
            };

            return View(model);
        }

        public ActionResult AuthNotAccess(int id)
        {
            var lp = GetLp(id);
            if (lp == null)
                return Error404();
            
            SetMetaInformation(T("Авторизация"));

            return View();
        }

        public ActionResult Redirect(int id)
        {
            var lp = _lpService.Get(id);
            if (lp != null)
            {
                var url = _lpService.GetLpLink(lp.Id);
                return Redirect(url);
            }

            return Error404();
        }
    }
}
