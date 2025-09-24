using System;

namespace AdvantShop.Core.Services.Shipping.PickPoint.Api
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PickPointErrorCodeAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Property)]
    public class PickPointErrorMessageAttribute : Attribute { }
}
