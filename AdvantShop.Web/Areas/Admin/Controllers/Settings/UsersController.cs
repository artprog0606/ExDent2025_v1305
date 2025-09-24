using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Saas;
using AdvantShop.Trial;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Core;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Users;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Settings.Users;
using AdvantShop.Web.Admin.ViewModels.Settings;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public class UsersController : BaseAdminController
    {
        public JsonResult GetUsers(UsersFilterModel model)
        {
            return Json(new GetUsersHandler(model).Execute());
        }

        #region Inplace Users

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult InplaceUser(AdminUserModel model)
        {
            var currentCustomer = CustomerContext.CurrentCustomer;
            if (model.CustomerId == currentCustomer.Id)
                return JsonError();

            var user = CustomerService.GetCustomer(model.CustomerId);
            if (user == null)
                return JsonError();

            if (model.CustomerRole == Role.Administrator && currentCustomer.IsModerator && user.Enabled != model.Enabled)
                return JsonError();

            user.Enabled = model.Enabled;
            user.SortOrder = model.SortOrder;
            CustomerService.UpdateCustomer(user);

            return JsonOk();
        }

        #endregion
        
        #region Commands

        private void Command(UsersFilterModel command, Func<Guid, UsersFilterModel, bool> func)
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
                var handler = new GetUsersHandler(command);
                var ids = handler.GetItemsIds("Customer.CustomerID");

                foreach (var id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteUsers(UsersFilterModel command)
        {
            Command(command, (id, c) => DeleteCustomerById(id));
            return Json(true);
        }

        #endregion
        
        #region CRUD User

        public JsonResult GetUser(Guid customerId)
        {
            var dbModel = CustomerService.GetCustomer(customerId);
            if (dbModel == null)
                return JsonError(T("Admin.Users.Validate.NotFound"));

            return JsonOk(new GetUserModel(dbModel).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddUser(AdminUserModel model)
        {
            return ProcessJsonResult(new AddEditUserHandler(model, false));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateUser(AdminUserModel model)
        {
            return ProcessJsonResult(new AddEditUserHandler(model, true));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteUser(Guid customerid)
        {
            List<string> messages;

            return DeleteCustomerById(customerid, out messages)
                ? JsonOk()
                : JsonError(messages.Any() ? messages.AggregateString("<br/>") : string.Empty);
        }

        private bool DeleteCustomerById(Guid customerid)
        {
            List<string> messages;
            return DeleteCustomerById(customerid, out messages);
        }

        private bool DeleteCustomerById(Guid customerid, out List<string> messages)
        {
            if (!CustomerService.CanDelete(customerid, out messages))
                return false;

            try
            {
                CustomerService.DeleteCustomer(customerid);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("DeleteCustomerById", ex);
                messages.Add(T("Admin.Users.ErrorOnDeleteUser"));
                return false;
            }

            return true;
        }

        #endregion

        // public JsonResult ChangeEnableManagersModuleState(bool state)
        // {
        //     SettingsCheckout.EnableManagersModule = state;
        //     return JsonOk();
        // }

        public JsonResult SendChangePasswordMail(Guid customerId)
        {
            return ProcessJsonResult(new SendChangePasswordEmailHandler(customerId));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangePassword(Guid customerId, string password, string passwordConfirm)
        {
            return ProcessJsonResult(new ChangePasswordHandler(customerId, password, passwordConfirm));
        }

        /// <summary>
        /// данные для формы редактирования/добавления сотрудника
        /// </summary>
        /// <param name="customerId">id редактируемого сотрудника</param>
        /// <returns></returns>
        public JsonResult GetUserFormData(Guid? customerId)
        {
            return ProcessJsonResult(new GetUserFormDataHandler(customerId));
        }

        public JsonResult GetSaasDataInformation()
        {
            var resultData = new UsersViewModel();

            try
            {
                var saasData = SaasDataService.CurrentSaasData;

                resultData.ManagersCount = ManagerService.GetManagersCount();
                resultData.ManagersLimitation = SaasDataService.IsSaasEnabled;
                resultData.ManagersLimit = resultData.ManagersLimitation ? saasData.EmployeesCount : 0;

                resultData.EmployeesCount = EmployeeService.GetEmployeeCount();
                resultData.EmployeesLimit = SaasDataService.IsSaasEnabled ? saasData.EmployeesCount : int.MaxValue;
                resultData.EnableEmployees = !SaasDataService.IsSaasEnabled || (SaasDataService.IsSaasEnabled && resultData.EmployeesCount < resultData.EmployeesLimit);

                resultData.EnableManagersModule = true;
            }
            catch (BlException e)
            {
                ModelState.AddModelError(e.Property, e.Message);
                return JsonError();
            }
            return JsonOk(resultData);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Invite(AdminUserModel[] users)
        {
            if (!TrialService.IsTrialEnabled)
                return JsonError();

            foreach (var user in users)
            {
                if (CustomerService.IsEmailExist(user.Email))
                    continue;

                var customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                {
                    EMail = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CustomerRole = Role.Moderator,
                    Password = StringHelper.GeneratePassword(8)
                };

                CustomerService.InsertNewCustomer(customer);
                if (customer.Id == Guid.Empty)
                    continue;

                foreach (RoleAction roleAction in Enum.GetValues(typeof(RoleAction)))
                {
                    if (roleAction == RoleAction.Settings || roleAction == RoleAction.None)
                        continue;
                    RoleActionService.UpdateOrInsertCustomerRoleAction(customer.Id, roleAction.ToString(), true);
                }

                customer.Password = SecurityHelper.GetPasswordHash(customer.Password);
                var mailTemplate = new UserRegisteredMailTemplate(customer.EMail, customer.FirstName, customer.LastName, Localization.Culture.ConvertDate(DateTime.Now),
                    ValidationHelper.DeleteSigns(SecurityHelper.GetPasswordHash(customer.Password)));
                mailTemplate.BuildMail();

                TrialService.SendMessage(customer.EMail, mailTemplate);
            }
            return JsonOk();
        }

        [HttpGet]
        public JsonResult GetPermissions()
        {
            return Json(new List<SelectItemModel>()
            {
                new SelectItemModel(Role.Administrator.Localize(), Role.Administrator.ToString()),
                new SelectItemModel(Role.Moderator.Localize(), Role.Moderator.ToString()),
            });
        }

        [HttpGet]
        public JsonResult GetRoles()
        {
            return Json(ManagerRoleService.GetManagerRoles().Select(x => new {label = x.Name, value = x.Id}));
        }

        public JsonResult GetRolesValidation(Guid customerId, List<int> rolesIds)
        {
            if (rolesIds == null)
                rolesIds = new List<int>();

            var manager = ManagerService.GetManager(customerId);
            if (manager == null)
                return JsonError();
            
            var errors = new List<string>();

            var taskGroupIds = TaskGroupService.GetTaskGroupIdsByManagerId(manager.ManagerId);
            foreach (var taskGroupId in taskGroupIds)
            {
                if (!TaskService.CheckAccessByGroup(manager.Customer, taskGroupId, rolesIds, null))
                    errors.Add(
                        String.Format("Сотрудник является участником группы {0}, но не имеет нужной роли ({1})",
                            TaskGroupService.GetTaskGroup(taskGroupId).Name,
                            String.Join(", ", TaskGroupService.GetTaskGroupManagerRoles(taskGroupId).Select(x => x.Name)))
                    );
            }

            if (errors.Count == 0)
            {
                var taskGroupIdsByTasks = TaskGroupService.GetTaskGroupIdsByManagerTasks(manager.ManagerId);
                foreach (var taskGroupId in taskGroupIdsByTasks)
                {
                    if (!TaskService.CheckAccessByGroup(manager.Customer, taskGroupId, rolesIds, null))
                        errors.Add(
                            String.Format("Сотрудник является участником группы {0}, но не имеет нужной роли ({1})",
                                TaskGroupService.GetTaskGroup(taskGroupId).Name,
                                String.Join(", ",
                                    TaskGroupService.GetTaskGroupManagerRoles(taskGroupId).Select(x => x.Name))));
                }
            }

            return errors.Count > 0 ? JsonError(errors.ToArray()) : JsonOk();
        }
    }
}
