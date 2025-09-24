namespace AdvantShop.ViewModel.PaymentStatus
{
    public class PaymentStatusViewModel
    {
        public string Result { get; }
        public bool IsEmptyLayout { get; }

        public PaymentStatusViewModel(string result, bool isEmptyLayout)
        {
            Result = result;
            IsEmptyLayout = isEmptyLayout;
        }
    }

    public class PaymentStatusResponse
    {
        public bool IsEmptyLayout { get; }

        public PaymentStatusResponse(bool isEmptyLayout)
        {
            IsEmptyLayout = isEmptyLayout;
        }
    }
}