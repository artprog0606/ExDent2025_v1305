using System;

namespace AdvantShop.Core.Modules.Interfaces
{
    public sealed class ModuleKeyValue
    {
        public string Key { get; set; }
        public Func<string> GetValue { get; set; }
    }
}