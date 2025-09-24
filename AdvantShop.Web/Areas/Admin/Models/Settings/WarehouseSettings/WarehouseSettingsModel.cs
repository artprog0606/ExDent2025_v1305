using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Services.Catalog.Warehouses;

namespace AdvantShop.Web.Admin.Models.Settings.WarehouseSettings
{
    public class WarehouseSettingsModel : IValidatableObject
    {
        public int DefaultWarehouseId { get; set; }
        public string DefaultWarehouseName { get; set; }
        
        public bool SetCookieOnMainDomain { get; set; }
        public bool ShowSetCookieOnMainDomain { get; set; }
        public string StoreUrl { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DefaultWarehouseId is 0
                || !WarehouseService.Exists(DefaultWarehouseId))
                yield return new ValidationResult("Необходимо указать запасной склад", new[] { "DefaultWarehouseId" });
        }
    }
}