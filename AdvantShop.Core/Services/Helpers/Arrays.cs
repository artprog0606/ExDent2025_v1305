namespace AdvantShop.Helpers
{
    public static class Arrays
    {
        public static bool Contains(this string[] array, string value)
        {
            for (int index = 0; index < array.Length; ++index)
            {
                if (array[index] == value)
                    return true;
            }
            return false;
        }
    }
}