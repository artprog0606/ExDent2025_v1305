using AdvantShop.Core.Services.Messengers;

namespace AdvantShop.Core.Modules.Interfaces
{ 
    public interface IMessengerService : IModule
    {
        string SendMessage(Message message);
    }
}