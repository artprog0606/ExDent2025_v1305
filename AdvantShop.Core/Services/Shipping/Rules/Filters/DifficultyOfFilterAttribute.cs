using System;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Shipping.Rules
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class DifficultyOfFilterAttribute: Attribute, IAttribute<int>
    {
        public DifficultyOfFilterAttribute(ECostFilter cost)
        {
            Value = (int)cost;
        }

        public int Value { get; }
    }

    public enum ECostFilter : int
    {
        Easy = 0,
        Medium = 1,
        Hard = 2,
    }
}