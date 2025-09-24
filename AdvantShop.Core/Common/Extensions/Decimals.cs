using System;

namespace AdvantShop
{
    public static class Decimals
    {
        public static string ToInvatiant(this decimal value)
        {
            return value.ToString(value%1 == 0 ? "### ### ##0.##" : "### ### ##0.00").TrimStart();
        }
    }
}
