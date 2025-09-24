using AdvantShop.Catalog;

namespace AdvantShop.ViewModel.ProductDetails
{
    public class SizePickerModel
    {
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        
        public SizePickerModel(Size size)
        {
            SizeId = size.SizeId;
            SizeName = size.ToString();
        }

        public SizePickerModel(SizeForCategory size)
        {
            SizeId = size.SizeId;
            SizeName = size.GetFullName();
        }

        public override bool Equals(object obj)
        {
            var size = obj as SizePickerModel;
            if (size == null)
                return false;

            return this.SizeId == size.SizeId &&
                   this.SizeName == size.SizeName;
        }

        public override int GetHashCode()
        {
            return SizeId.GetHashCode() ^ (SizeName ?? "").GetHashCode();
        }
    }
}