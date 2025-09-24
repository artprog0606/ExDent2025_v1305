namespace AdvantShop.ViewModel.Shared
{
    public class RatingViewModel
    {
        public RatingViewModel(double rating, bool readOnly = true)
        {
            Rating = rating;
            ReadOnly = readOnly;
        }

        public int ObjId { get; set; }

        public string Url { get; set; }

        public double Rating { get; private set; }

        public bool ReadOnly { get; set; }

        public string Binding { get; set; }
    }
}