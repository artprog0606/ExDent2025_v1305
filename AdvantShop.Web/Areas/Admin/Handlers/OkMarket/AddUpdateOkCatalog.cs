using AdvantShop.Core;
using AdvantShop.Core.Services.Crm.Ok.OkMarket;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Models.OkMarket;
using AdvantShop.Web.Infrastructure.Handlers;
using System;

namespace AdvantShop.Web.Admin.Handlers.OkMarket
{
    public class AddUpdateOkCatalog : AbstractCommandHandler<bool>
    {
        private readonly OkMarketApiService _apiService;
        private readonly OkMarketCatalogModel _catalog;

        public AddUpdateOkCatalog(OkMarketCatalogModel catalog)
        {
            _catalog = catalog;
            _apiService = new OkMarketApiService();
        }

        protected override bool Handle()
        {
            try
            {
                var cat = new OkMarketCatalog
                {
                    Name = _catalog.Name,
                    CategoryIds = _catalog.CategoryIds,
                    Id = _catalog.Id,
                    OkCatalogId = _catalog.OkCatalogId
                };

                if (cat.OkCatalogId != 0)
                {
                    _apiService.EditCatalog(_catalog.OkCatalogId, cat.Name);
                    OkMarketService.UpdateCatalog(cat);
                    OkMarketService.RemoveLinkedCategories(cat.Id);
                }
                else
                {
                    var catalogId = _apiService.AddCatalog(cat.Name);
                    if (!catalogId.HasValue)
                    {

                        return false;
                    }
                    cat.OkCatalogId = catalogId.Value;
                    OkMarketService.AddCatalog(cat);
                }

                foreach (var categoryId in cat.CategoryIds)
                {
                    OkMarketService.AddCatalogLink(categoryId, cat.Id);
                }
                return true;
            }
            catch (BlException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return false;
            }
            
        }
    }
}