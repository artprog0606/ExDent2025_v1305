using AdvantShop.Catalog;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.SQL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvantShop.Core.Services.FullSearch
{
    public interface IModuleProductSearchProvaider : IModule, IProductSearchProvaider
    {
    }

    public interface IProductSearchProvaider
    {
        SearchResult Find(string searchTerm);        
    }
}
