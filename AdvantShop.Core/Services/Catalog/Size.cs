using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Catalog
{
    public class Size
    {
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public int SortOrder { get; set; }

        public override string ToString()
        {
            return SizeName;
        }
    }

    public class SizeForCategory : Size
    {
        public int CategoryId { get; set; }

        public string SizeNameForCategory { get; set; }

        public string GetFullName()
        {
            if (SizeNameForCategory.IsNotEmpty())
                return SizeName + $" ({SizeNameForCategory})";
            return SizeName;
        }
    }
}