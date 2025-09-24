using System.Collections.Generic;
using System.Web;
using AdvantShop.Configuration;

namespace AdvantShop.Core.Services.Catalog.Warehouses
{
    public class WarehouseContext
    {
        public static List<int> CurrentWarehouseIds
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;

                var ids = HttpContext.Current.Items["CurrentWarehouseIds"] != null
                    ? HttpContext.Current.Items["CurrentWarehouseIds"] as List<int>
                    : null;
                
                return ids;
            }
            set
            {
                if (HttpContext.Current == null)
                    return;

                HttpContext.Current.Items["CurrentWarehouseIds"] = value != null && value.Count > 0 ? value : null;
            }
        }
        
        public static List<int> GetAvailableWarehouseIds() => SettingsCatalog.ShowOnlyAvalible ? CurrentWarehouseIds : null;
    }
}