using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class APISettingsModel
    {
        public string SiteUrl { get; set; }
        public string Key { get; set; }
        public string KeyAuth { get; set; }
        
        public bool IsRus { get; set; }
        public bool _1CEnabled { get; set; }
        public bool _1CDisableProductsDecremention { get; set; }
        public bool _1CUpdateStatuses { get; set; }
        public bool ExportOrdersType { get; set; }
        public List<SelectListItem> ExportOrders { get; set; }
        public bool _1CUpdateProducts { get; set; }
        public List<SelectListItem> UpdateProducts { get; set; }
        public bool _1CSendProducts { get; set; }
        public List<SelectListItem> SendProducts { get; set; }
        public int? _1CStocksToWarehouseId { get; set; }
        public List<SelectListItem> Warehouses { get; set; }
        public string ImportPhotosUrl { get; set; }
        public string ImportProductsUrl { get; set; }
        public string ExportProductsUrl { get; set; }
        public string DeletedProducts { get; set; }
        public string ExportOrdersUrl { get; set; }
        public string ChangeOrderStatusUrl { get; set; }
        public string DeletedOrdersUrl { get; set; }

        public string LeadAddUrl { get; set; }
        public string VkUrl { get; set; }

        public bool ShowOneC { get; set; }

        public string OrderAddUrl { get; set; }
        public string OrderGetUrl { get; set; }
        public string OrderGetListUrl { get; set; }
        public string OrderChangeStatusUrl { get; set; }
        public string OrderSetPaidUrl { get; set; }

        public string OrderStatusGetListUrl { get; set; }

        public string WebhookAddOrderUrl { get; set; }
        public string WebhookChangeOrderStatusUrl { get; set; }
        public string WebhookUpdateOrderUrl { get; set; }
        public string WebhookDeleteOrderUrl { get; set; }
        
        public string WebhookAddLeadUrl { get; set; }
        public string WebhookChangeLeadStatusUrl { get; set; }
        public string WebhookUpdateLeadUrl { get; set; }
        public string WebhookDeleteLeadUrl { get; set; }
        

        private string _categoriesSorting;
        public string CategoriesSorting
        {
            get
            {
                if (_categoriesSorting != null)
                    return _categoriesSorting;

                _categoriesSorting += string.Join(", ",
                    Enum.GetValues(typeof(ESortOrder)).Cast<ESortOrder>()
                        .Where(x => !x.Ignore())
                        .Select(x => $"{x.ToString()} - {x.Localize()}"));

                return _categoriesSorting;
            }
        }

        public string GetApiUrl(string apiUrl) => $"{SiteUrl}{apiUrl}?apikey={Key}";
        
        public string GetApiAuthUrl(string apiUrl) => $"{SiteUrl}{apiUrl}?apikey={KeyAuth}";
    }
}