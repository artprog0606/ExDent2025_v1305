using System;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface ITwoFactorAuthentication : IModule
    {
        bool CheckCodeValid(string key, string code);
        bool HasUserEnabledAuthentication(Guid userId);
        void SaveUserAuthenticationEnabled(Guid userId, bool isTfa);
        ITwoFactorAuthenticationOptions GetCodes(Guid userId, string email);
    }
    public interface ITwoFactorAuthenticationOptions
    {
        string SecretKey { get; }
        string QrCode { get; }
    }
}