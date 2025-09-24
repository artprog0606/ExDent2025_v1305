using AdvantShop.Core.Services.Auth.Calls;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IAuthCallService : IModule
    {
        bool FlashCallEnable();
        bool VoiceCallEnable();
    }
    
    public interface IAuthCallWithoutCodeGenService : IAuthCallService
    {
        string GetAuthCode(long phone, EAuthCall type);
    }

    public interface IAuthCallWithCodeGenService : IAuthCallService
    {
        string GetAuthCode(long phone, string code, EAuthCall type);
    }
}