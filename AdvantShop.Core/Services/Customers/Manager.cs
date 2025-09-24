//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.FilePath;
using Newtonsoft.Json;

namespace AdvantShop.Customers
{
    public class Manager
    {
        public int ManagerId { get; set; }
        public Guid CustomerId { get; set; }
        public int? DepartmentId { get; set; }
        public string Position { get; set; }
        public string Sign { get; set; }

        private Customer _customer;
        
        [JsonIgnore]
        public Customer Customer => _customer ?? (_customer = CustomerService.GetCustomer(CustomerId));

        public string FullName => $"{FirstName} {LastName}";

        public string FirstName => Customer != null ? Customer.FirstName : string.Empty;

        public string LastName => Customer != null ? Customer.LastName : string.Empty;

        public long? StandardPhone => Customer != null ? Customer.StandardPhone : null;

        public string Email => Customer != null ? Customer.EMail : string.Empty;

        public bool Enabled => Customer != null && Customer.Enabled;
        
        public string AvatarSrc =>
            Customer != null && Customer.Avatar.IsNotEmpty() 
                ? FoldersHelper.GetPath(FolderType.Avatar, Customer.Avatar, false) 
                : string.Empty;

        public bool HasRoleAction(RoleAction key)
        {
            return Customer != null && Customer.HasRoleAction(key);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Manager;
            if (other == null)
                return false;

            return ManagerId == other.ManagerId &&
                   CustomerId == other.CustomerId &&
                   DepartmentId == other.DepartmentId &&
                   Position == other.Position;
        }

        public override int GetHashCode()
        {
            return ManagerId.GetHashCode() ^
                   CustomerId.GetHashCode() ^
                   DepartmentId.GetHashCode() ^
                   (Position ?? "").GetHashCode();
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
