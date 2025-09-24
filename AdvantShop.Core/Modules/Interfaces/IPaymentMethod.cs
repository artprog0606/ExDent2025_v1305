namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IPaymentMethod : IModule
    {
        string PaymentKey { get; }
        string PaymentName { get; }
    }
}