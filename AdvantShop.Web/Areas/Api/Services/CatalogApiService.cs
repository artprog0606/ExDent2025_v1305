using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Catalog;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;

namespace AdvantShop.Areas.Api.Services
{
    public class CatalogApiService
    {
        public void SetMarkers(List<CatalogProductItem> products)
        {
            if (products == null || products.Count == 0)
                return;
            
            foreach (var type in AttachedModules.GetModules<IMarker>())
            {
                var module = (IMarker)Activator.CreateInstance(type);
                var markers = module.GetMarkersByProductIds(products.Select(x => x.ProductId).ToList());
                if (markers == null || markers.Count == 0) 
                    continue;
                
                foreach (var product in products)
                {
                    var productMarkers = markers.FirstOrDefault(x => x.ProductId == product.ProductId);
                    if (productMarkers != null && 
                        productMarkers.Markers != null && productMarkers.Markers.Count > 0)
                    {
                        if (product.Markers == null)
                            product.Markers = new List<ProductMarker>();
                        
                        product.Markers.AddRange(productMarkers.Markers);
                    }
                }
            }
        }
    }
}