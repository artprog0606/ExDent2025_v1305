using System;

namespace AdvantShop.Core.Common.Attributes
{
    public class EnumIgnoreAttribute : Attribute, IAttribute<bool>
    {
        public bool Value { get { return true; } }
    }
}
