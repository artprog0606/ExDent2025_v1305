namespace AdvantShop.Core.Common.Extensions
{
    public static class Booleans
    {
        public static string ToLowerString(this bool value)
        {
            return value ? "true" : "false";
        }

        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }
    }
}
