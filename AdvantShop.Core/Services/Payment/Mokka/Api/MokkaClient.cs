namespace AdvantShop.Core.Services.Payment.Mokka.Api
{
    public class MokkaClient : MokkaApiService, IMokkaClient, IDeserialize
    {
        private MokkaClient(string storeId, string secretKey, bool sandbox) : base(storeId, secretKey, sandbox) { }

        public static IMokkaClient Create(string storeId, string secretKey, bool sandbox) => new MokkaClient(storeId, secretKey, sandbox);

        public IDeserialize Deserializer => this;
    }

    public interface IMokkaClient
    {
        RegistrationResponse Registration(RegistrationParameters registrationParameters);
        CheckoutResponse Checkout(CheckoutParameters checkoutParameters);
        ScheduleResponse Schedule(ScheduleParameters scheduleParameters);
        StatusResponse GetStatus(StatusParameters statusParameters);
        CancelResponse Cancel(CancelParameters cancelParameters);
        FinishResponse Finish(FinishParameters finishParameters);
        FinishResponse Finish(FinishParameters finishParameters, System.IO.Stream checkStream);
        ReturnResponse Return(ReturnParameters returnParameters);
        LimitResponse Limit(LimitParameters limitParameters);
        IDeserialize Deserializer { get; }
    }

    public interface IDeserialize
    {
        T DeserializeObject<T>(string payload);
    }
}