using System.Linq;
using System.Web.Mvc;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Tasks;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public class ManagersController : BaseAdminController
    {
        [Auth(RoleAction.Settings, RoleAction.Crm, RoleAction.Orders, RoleAction.Customers, RoleAction.Tasks)]
        public JsonResult GetManagersSelectOptions(bool includeEmpty = false, RoleAction[] roleActions = null)
        {
            var managers = ManagerService.GetManagers(roleActions).Select(x => new SelectItemModel(x.FullName, x.ManagerId)).ToList();
            if (includeEmpty)
                managers.Insert(0, new SelectItemModel("Не назначено", "-1"));
            return Json(managers);
        }

        [Auth(RoleAction.Settings, RoleAction.Crm, RoleAction.Orders, RoleAction.Customers, RoleAction.Tasks)]
        public JsonResult GetAllTaskManagers(int? taskGroupId = null, bool assigned = false, bool appointed = false, bool includeEmpty = false, TasksPreFilterType? filterBy = null, bool observed = false)
        {
            var managers = ManagerService.GetAllTaskManagers(
                taskGroupId: taskGroupId, 
                assigned: assigned, 
                appointed: appointed, 
                accepted: filterBy.HasValue && filterBy.Value == TasksPreFilterType.Accepted,
                observed: observed)
                .Select(x => new SelectItemModel(x.FullName, x.ManagerId)).ToList();
            if (includeEmpty)
                managers.Insert(0, new SelectItemModel("Не назначено", "-1"));
            return Json(managers);
        }
    }
}
