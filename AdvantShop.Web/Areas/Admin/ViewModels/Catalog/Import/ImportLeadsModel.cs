using AdvantShop.Core.Services.Crm.SalesFunnels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.ViewModels.Catalog.Import
{
    public class ImportLeadsModel : BaseImportModel
    {
        public ImportLeadsModel()
        {
            SalesFunnels = SalesFunnelService.GetList().Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name }).ToList();
        }

        public string PropertySeparator { get; set; }

        public string PropertyValueSeparator { get; set; }

        public bool UpdateCustomer { get; set; }
        
        public string BasicSalesFunnelId { get; set; }

        public List<SelectListItem> SalesFunnels { get; set; }

        /// <summary>
        /// Не добавлять лид если у пользователя уже есть лид в этой воронке
        /// </summary>
        public bool DoNotDuplicate { get; set; }
    }
}