using System.Web.Mvc;
using AdvantShop.CMS;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Shared.AdminComments;
using AdvantShop.Web.Admin.Handlers.Shared.AdminNotifications;
using AdvantShop.Web.Admin.Models.Shared.AdminComments;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    [Auth(RoleAction.Customers, RoleAction.Orders, RoleAction.Crm, RoleAction.Tasks)]
    public partial class AdminCommentsController : BaseAdminController
    {
        public JsonResult GetComments(int objId, AdminCommentType type)
        {
            return Json(new GetAdminCommentsHandler(objId, type).Execute());
        }

        public JsonResult GetComment(int id)
        {
            return Json(AdminCommentService.GetAdminComment(id));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(AdminCommentModel model)
        {
            if (model == null)
                return Json(new { Result = false });

            return Json(new AddAdminCommentHandler(model).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Update(int id, string text)
        {
            AdminComment comment;
            if ((comment = AdminCommentService.GetAdminComment(id)) == null)
                return Json(new { Result = false });

            var customer = CustomerContext.CurrentCustomer;

            if ((comment.CustomerId != null && comment.CustomerId == customer.Id) || customer.IsAdmin)
            {
                comment.Text = text ?? "";
                AdminCommentService.UpdateAdminComment(comment);
                new AdminNotificationsHandler().UpdateAdminComment(comment, "changed");
                return Json(new { Result = true });
            }

            return Json(new { Result = false });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var comment = AdminCommentService.GetAdminComment(id);
            if (comment == null)
                return Json(new { Result = false });

            var customer = CustomerContext.CurrentCustomer;

            if ((comment.CustomerId != null && comment.CustomerId == customer.Id) || customer.IsAdmin)
            {
                new AdminNotificationsHandler().UpdateAdminComment(comment, "deleted");
                AdminCommentService.DeleteAdminComment(id);
                return Json(new { Result = true });
            }

            return Json(new { Result = false });
        }

        [HttpGet]
        public JsonResult GetChangeHistory(ChangeHistoryFilter filter)
        {
            return Json(new GetChangeHistory(filter).Execute());
        }
    }
}
