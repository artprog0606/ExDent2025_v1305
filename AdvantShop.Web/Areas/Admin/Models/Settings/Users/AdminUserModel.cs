using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Web.Admin.Handlers.Settings.Users;

namespace AdvantShop.Web.Admin.Models.Settings.Users
{
    public class AdminUserModel
    {
        // customer fields
        public Guid CustomerId { get; set; }
        public Role CustomerRole { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public bool Enabled { get; set; }
        public Guid? HeadCustomerId { get; set; }
        public DateTime? BirthDay { get; set; }
        public string City { get; set; }
        public int SortOrder { get; set; }

        // manager fields
        public int? AssociatedManagerId { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Position { get; set; }
        public string FullName { get; set; }
        public bool EditHimself { get; set; }
        public string Sign { get; set; }

        public string PhotoEncoded { get; set; }
        public string PhotoSrc
        {
            get
            {
                return Avatar.IsNotEmpty()
                    ? string.Format("{0}?rnd={1}", FoldersHelper.GetPath(FolderType.Avatar, Avatar, false), new Random().Next())
                    : UrlService.GetAdminStaticUrl() + "images/no-avatar.jpg";
            }
        }

        private List<UserRoleActionModel> _roleActionKeys;
        public List<UserRoleActionModel> RoleActionKeys
        {
            get { return _roleActionKeys ?? (_roleActionKeys = new GetUserRoleActions(CustomerId).Execute()); }
            set { _roleActionKeys = value; }
        }

        private List<ManagerRole> _managerRoles;
        public List<ManagerRole> ManagerRoles
        {
            get { return _managerRoles ?? (_managerRoles = CustomerId != Guid.Empty ? ManagerRoleService.GetManagerRoles(CustomerId) : new List<ManagerRole>()); }
        }

        private List<int> _rolesIds;
        public List<int> ManagerRolesIds
        {
            get { return _rolesIds ?? (_rolesIds = ManagerRoles.Select(x => x.Id).ToList()); }
            set { _rolesIds = value; }
        }

        private List<CustomerFieldWithValue> _customerFields;

        public List<CustomerFieldWithValue> CustomerFields
        {
            get
            {
                return _customerFields 
                    ?? (_customerFields = CustomerFieldService.GetCustomerFieldsWithValue(CustomerId));
            }
            set { _customerFields = value; }
        }

        public string Permissions
        {
            get { return CustomerRole.Localize(); }
        }

        public string Roles
        {
            get { return String.Join(", ", ManagerRoles.Select(x => x.Name)); }
        }

        public bool HasTasksAccess
        {
            get { return SettingsTasks.TasksActive && RoleActionKeys != null && RoleActionKeys.Any(x => x.Key.ToString() == RoleAction.Tasks.ToString() && x.Enabled); }
        }

        private bool _canDeleteInited;
        private void InitCanDelete()
        {
            if (_canDeleteInited)
                return;
            if (CustomerId != Guid.Empty)
            {
                List<string> messages;
                _canBeDeleted = CustomerService.CanDelete(CustomerId, out messages);
                _cantDeleteMessage = messages.AggregateString("<br/>");
            }
            _canDeleteInited = true;
        }

        public string _cantDeleteMessage;
        public string CantDeleteMessage
        {
            get
            {
                InitCanDelete();
                return _cantDeleteMessage;
            }
        }

        public bool _canBeDeleted;
        public bool CanBeDeleted
        {
            get
            {
                InitCanDelete();
                return _canBeDeleted;
            }
        }

        public string FcmToken { get; set; }
        public bool AdminAppNotificationsEnabled { get; set; }
    }
}
