using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Shipping;

namespace AdvantShop.Shipping
{
    public abstract class BaseShippingWithCargoAndCache : BaseShippingWithCargo
    {
        protected BaseShippingWithCargoAndCache(ShippingMethod method, ShippingCalculationParameters calculationParameters)
            : base(method, calculationParameters)
        {
        }

        protected virtual int GetHashForCache()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<BaseShippingOption> CalculateOptions(CalculationVariants calculationVariants)
        {
            if (!NeedToCalc(calculationVariants))
                return null;
            
            ShippingCacheRepositiry.Delete();
            var hash = GetHashForCache() ^ (int)calculationVariants;

            var cached = ShippingCacheRepositiry.Get(_method.ShippingMethodId, hash);
            if (cached != null)
                return cached.Options;

            var calcOptions = base.CalculateOptions(calculationVariants);
            IList<BaseShippingOption> options = calcOptions as IList<BaseShippingOption> ?? calcOptions?.ToList();

            var model = new ShippingCache
            {
                Options = options,
                ShippingMethodId = _method.ShippingMethodId,
                ParamHash = hash
            };


            if (ShippingCacheRepositiry.Exist(_method.ShippingMethodId, hash))
                ShippingCacheRepositiry.Update(model);
            else
                ShippingCacheRepositiry.Add(model);

            return options;
        }
    }
}