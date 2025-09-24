using System;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings.Users
{
    public class UsersFilterModel : BaseFilterModel<Guid>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int? DepartmentId { get; set; }
        public bool? Enabled { get; set; }
        public bool? HasPhoto { get; set; }

        public int? Role { get; set; }
        public Role? Permission { get; set; }
    }
}
