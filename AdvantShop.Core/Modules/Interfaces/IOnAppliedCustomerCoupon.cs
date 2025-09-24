using AdvantShop.Catalog;
using System;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IOnAppliedCustomerCoupon : IModule
    {
        void AddedCustomerCoupon(int couponId, Guid customerId);
        void DeletedCustomerCoupon(int couponId, Guid customerId);
    }
}
