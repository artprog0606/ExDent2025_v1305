using System;

namespace AdvantShop.Core
{
    public class BlException: Exception
    {
        public string Property { get; protected set; }
        public BlException(string message, string prop) : base(message)
        {
            Property = prop;
        }

        public BlException(string message) : base(message)
        {
            Property = string.Empty;
        }
    }
}
