using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Triggers
{
    public class CopyTrigger : ICommandHandler<int>
    {
        private readonly int _id;

        public CopyTrigger(int id)
        {
            _id = id;
        }

        public int Execute()
        {
            var trigger = TriggerRuleService.GetTrigger(_id);

            trigger.Name += " - Копия";

            var id = TriggerRuleService.Add(trigger);

            foreach (var action in TriggerActionService.GetTriggerActions(_id))
            {
                var sourceActionId = action.Id;

                action.TriggerRuleId = id;
                TriggerActionService.Add(action);

                // copy action coupon
                foreach (var couponSource in CouponService.GetCouponsByTriggerAction(sourceActionId))
                {
                    var coupon = couponSource.DeepClone();
                    coupon.TriggerActionId = action.Id;

                    CopyCoupon(coupon, couponSource);
                }
            }

            // copy trigger coupon
            var triggerCoupon = CouponService.GetCouponByTrigger(_id);
            if (triggerCoupon != null)
            {
                var newTriggerCoupon = triggerCoupon.DeepClone();
                newTriggerCoupon.TriggerId = trigger.Id;

                CopyCoupon(newTriggerCoupon, triggerCoupon);
            }

            return id;
        }


        private void CopyCoupon(Coupon coupon, Coupon couponSource)
        {
            coupon.Code += " - copy";

            CouponService.AddCoupon(coupon);

            foreach (var categoryId in couponSource.CategoryIds)
                CouponService.AddCategoryToCoupon(coupon.CouponID, categoryId);

            foreach (var productId in couponSource.ProductsIds)
                CouponService.AddProductToCoupon(coupon.CouponID, productId);

            foreach (var customerGroupId in couponSource.CustomerGroupIds)
                CouponService.AddCustomerGroupToCoupon(coupon.CouponID, customerGroupId);
        }
    }
}
