using System;
using AdvantShop.Catalog;

namespace AdvantShop.Letters
{
    public class TriggerLetterBuilderOptions
    {
        public Coupon GeneratedCouponTemplate { get; }
        
        public string TriggerCouponCode { get; }

        public TriggerLetterBuilderOptions(Coupon generatedCouponTemplate, string triggerCouponCode)
        {
            GeneratedCouponTemplate = generatedCouponTemplate;
            TriggerCouponCode = triggerCouponCode;
        }

        public bool IsEmpty => 
            GeneratedCouponTemplate == null && string.IsNullOrEmpty(TriggerCouponCode);
    }
    
    public class TriggerLetterBuilder : BaseLetterTemplateBuilder<TriggerLetterBuilderOptions, TriggerLetterTemplateKey>
    {
        public TriggerLetterBuilder(TriggerLetterBuilderOptions order) : base(order, null) {  }

        public TriggerLetterBuilder(TriggerLetterBuilderOptions order, TriggerLetterTemplateKey[] availableKeys) : base(order, availableKeys)
        {
        }
        
        protected override string GetValue(TriggerLetterTemplateKey key)
        {
            var options = _entity;
            
            switch (key)
            {
                case TriggerLetterTemplateKey.GeneratedCouponCode:
                {
                    if (options.GeneratedCouponTemplate == null) 
                        return null;
                    
                    var generatedCoupon = CouponService.GenerateCoupon(options.GeneratedCouponTemplate);
                    return generatedCoupon?.Code;
                }

                case TriggerLetterTemplateKey.TriggerCouponCode:
                    return options.TriggerCouponCode;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }
    }
}