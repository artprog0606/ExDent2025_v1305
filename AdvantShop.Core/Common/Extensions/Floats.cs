namespace AdvantShop.Core.Common.Extensions
{
    public static class Floats
    {
        public static string ToFormatString(this float number)
        {
            return string.Format("{0:#0.####}", number).Replace('.', ',');
        }
    }
}