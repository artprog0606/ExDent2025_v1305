using System.Web.Mvc;
using AdvantShop.Shipping;

namespace AdvantShop.Models
{
    public class CalculationVariantsModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(CalculationVariants)
                && bindingContext.ModelType != typeof(CalculationVariants?))
            {
                return base.BindModel(controllerContext, bindingContext);
            }
            
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == null) return null;

            var attemptedValue = valueProviderResult.AttemptedValue;
            if (string.Equals(attemptedValue, "courier"))
                return CalculationVariants.Courier;
            
            if (string.Equals(attemptedValue, "self-delivery"))
                return CalculationVariants.PickPoint;
            
            return base.BindModel(controllerContext, bindingContext);
        }
    }
}