using System;
using System.Linq;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;

namespace AdvantShop.Handlers.Inplace
{
    public class InplaceModuleHandler
    {
        public object Execute(int id, string type, string field, string content)
        {
            var modules = AttachedModules.GetModuleInstances<IInplaceEditor>();
            if (modules != null && modules.Count != 0)
            {
                foreach (var module in modules)
                    module.Edit(id, type, field, content);
            }
            return true;
        }
    }
}