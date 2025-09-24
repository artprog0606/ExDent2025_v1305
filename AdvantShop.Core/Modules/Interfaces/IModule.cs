//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Web.Routing;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IModuleBase
    {
        
    }

    public interface IModule : IModuleBase
    {
        string ModuleStringId { get; }

        string ModuleName { get; }

        bool CheckAlive();

        bool InstallModule();

        bool UpdateModule();

        bool UninstallModule();
    }
}
