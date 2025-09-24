using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Crm.LeadFields;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Crm.SalesFunnels;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Crm
{
    [Auth(RoleAction.Crm)]
    public class LeadFieldsController : BaseAdminController
    {
        public JsonResult GetAllLeadFields()
        {
            var leadFields = LeadFieldService.GetLeadFields().Select(x => new LeadFieldModel(x)).ToList();
            return JsonOk(leadFields);
        }

        public JsonResult GetLeadFields(int salesFunnelId, bool onlyEnabled = true)
        {
            var leadFields = LeadFieldService.GetLeadFields(salesFunnelId, onlyEnabled).Select(x => new LeadFieldModel(x)).ToList();
            return JsonOk(leadFields);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeLeadFieldSorting(int salesFunnelId, int id, int? prevId, int? nextId)
        {
            return ProcessJsonResult(new ChangeLeadFieldSortingHandler(salesFunnelId, id, prevId, nextId));
        }

        public ActionResult LeadFieldsForm(int? leadId, int? salesFunnelId)
        {
            return PartialView("~/Areas/Admin/Views/Leads/_LeadFields.cshtml", LeadFieldService.GetLeadFieldsWithValue(leadId, salesFunnelId));
        }

        public JsonResult GetLeadFieldValues(int id)
        {
            return Json(LeadFieldService.GetLeadFieldValues(id).Select(x => new SelectItemModel(x.Value, x.Value)));
        }

        #region CRUD LeadField

        public JsonResult Get(int id)
        {
            var field = LeadFieldService.GetLeadField(id);
            if (field == null)
                return JsonError();

            return JsonOk(new LeadFieldModel(field, true));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(LeadFieldModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();
            return ProcessJsonResult(new AddEditLeadFieldHandler(model, false));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Update(LeadFieldModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();
            return ProcessJsonResult(new AddEditLeadFieldHandler(model, true));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(LeadFieldModel model)
        {
            var field = LeadFieldService.GetLeadField(model.Id);
            if (field == null)
                return JsonError();
            
            field.Required = model.Required;
            field.Enabled = model.Enabled;
            LeadFieldService.UpdateLeadField(field);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            LeadFieldService.DeleteLeadField(id);
            return JsonOk();
        }

        #endregion

        public JsonResult GetLeadFieldTypes()
        {
            return Json(Enum.GetValues(typeof(LeadFieldType)).Cast<LeadFieldType>()
                .Select(x => new SelectItemModel(x.Localize(), (int)x)).ToList());
        }

        public JsonResult GetFormData()
        {
            var fieldTypes = Enum.GetValues(typeof(LeadFieldType)).Cast<LeadFieldType>()
                .Select(x => new SelectItemModel(x.Localize(), (int)x)).ToList();
            return Json(new
            {
                fieldTypes
            });
        }
    }
}
