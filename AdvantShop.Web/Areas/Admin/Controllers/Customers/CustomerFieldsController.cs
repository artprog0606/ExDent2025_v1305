using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Customers.CustomerFields;
using AdvantShop.Web.Admin.Models.Customers.CustomerFields;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Customers
{
    [Auth(RoleAction.Customers)]
    public class CustomerFieldsController : BaseAdminController
    {
        public JsonResult GetPaging(CustomerFieldsFilterModel model)
        {
            var result = new GetCustomerFieldsHandler(model).Execute();
            return Json(result);
        }

        #region Inplace CustomerFields

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(AdminCustomerFieldModel model)
        {
            var dbModel = CustomerFieldService.GetCustomerField(model.Id);
            if (dbModel == null)
                return Json(new {result = false});

            dbModel.SortOrder = model.SortOrder;
            dbModel.Required = model.Required;
            dbModel.Enabled = model.Enabled;
            dbModel.ShowInRegistration = model.ShowInRegistration;
            dbModel.ShowInCheckout = model.ShowInCheckout;
            dbModel.ShowInUserEditing = model.ShowInUserEditing;

            CustomerFieldService.UpdateCustomerField(dbModel);

            return Json(new {result = true});
        }

        #endregion

        #region Commands

        private void Command(CustomerFieldsFilterModel command, Func<int, CustomerFieldsFilterModel, bool> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                {
                    func(id, command);
                }
            }
            else
            {
                var handler = new GetCustomerFieldsHandler(command);
                var ids = handler.GetItemsIds("Id");

                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteItems(CustomerFieldsFilterModel command)
        {
            Command(command, (id, c) =>
            {
                CustomerFieldService.DeleteCustomerField(id);
                return true;
            });
            return Json(true);
        }

        #endregion
        
        #region CRUD CustomerField

        public JsonResult Get(int id)
        {
            var dbModel = CustomerFieldService.GetCustomerField(id);
            if (dbModel == null)
                return Json(new { result = false });

            var handler = new GetCustomerFieldModel(dbModel);
            var result = handler.Execute();

            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(AdminCustomerFieldModel model)
        {
            if (model.Name.IsNullOrEmpty())
                return Json(new { result = false });
            if (model.FieldAssignment != CustomerFieldAssignment.None)
            {
                var field = CustomerFieldService.GetCustomerFieldByFieldAssignment(model.FieldAssignment, false);
                if (field != null)
                    return Json(new { result = false, 
                        error = $"Предназначение \"{model.FieldAssignment.Localize()}\" уже настроено в поле \"{field.Name}\"" });
            }
            var customerField = new AddEditCustomerFieldHandler(model, false).Execute();

            return Json(new { result = customerField != null });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Update(AdminCustomerFieldModel model)
        {
            if (model.Name.IsNullOrEmpty())
                return Json(new { result = false });
            if (model.FieldAssignment != CustomerFieldAssignment.None)
            {
                var field = CustomerFieldService.GetCustomerFieldByFieldAssignment(model.FieldAssignment, false);
                if (field != null && field.Id != model.Id)
                    return Json(new
                    {
                        result = false,
                        error = $"Предназначение \"{model.FieldAssignment.Localize()}\" уже настроено в поле {field.Name}"
                    });
            }

            var customerField = new AddEditCustomerFieldHandler(model, true).Execute();

            return Json(new { result = customerField != null });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            CustomerFieldService.DeleteCustomerField(id);
            return Json(new { result = true });
        }

        #endregion

        public JsonResult GetCustomerFieldTypes()
        {
            return Json(CustomerFieldService.GetCustomerFieldTypesSelectOptions());
        }

        public JsonResult GetCustomerFieldCustomerTypes()
        {
            return Json(CustomerFieldService.GetCustomerFieldCustomerTypesSelectOptions());
        }

        public JsonResult GetCustomerFieldFormData()
        {
            return Json(new
            {
                fieldTypes = CustomerFieldService.GetCustomerFieldTypesSelectOptions(),
                customerTypes = CustomerFieldService.GetCustomerFieldCustomerTypesSelectOptions(),
                isRegistrationAsLegalEntity = SettingsCustomers.IsRegistrationAsLegalEntity,
                isRegistrationAsPhysicalEntity = SettingsCustomers.IsRegistrationAsPhysicalEntity,
                fieldAssignments = CustomerFieldService.GetCustomerFieldFieldAssignmentsSelectOptions()
            });
        }
    }
}
