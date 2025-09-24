using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Orders.OrderReviews;
using AdvantShop.Web.Admin.Models.Orders.OrderReview;
using AdvantShop.Web.Infrastructure.Controllers;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Controllers.Orders
{
    [Auth(RoleAction.Orders)]
    public partial class OrderReviewsController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.OrderReviews.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.OrderReviewsCtrl);

            return View();
        }
        
        public JsonResult GetOrderReviews(OrderReviewFilterModel filterModel) => Json(new GetOrderReviewsHandler(filterModel).Execute());
    }
}
