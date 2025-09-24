using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Core.Services.Catalog
{
    public class ProductHistoryService
    {
        #region Product

        public static void NewProduct(Product product, ChangedBy changedBy)
        {
            if(!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = product.ProductId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.ProductHistory.ProductCreated")
            });
        }

        public static void DeleteProduct(int productId, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = productId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.ProductHistory.ProductDeleted")
            });
        }

        public static void TrackProductChanges(Product product, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            var oldProduct = ProductService.GetProduct(product.ProductId);
            if (oldProduct == null)
                return;

            var history = ChangeHistoryService.GetChanges(product.ProductId, ChangeHistoryObjType.Product, oldProduct, product, changedBy);

            ChangeHistoryService.Add(history);
        }

        public static void TrackProductMainCategoryChanges(int productId, int categoryId, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = productId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.ProductHistory.ProductMainCategoryChanged") + " " + categoryId
            });
        }

        public static void ProductChanged(Product product, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = product.ProductId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.ProductHistory.ProductChanged")
            });
        }

        public static void TrackAmountByOrderItemDecrementIncrement(
                                int productId, string status, float oldAmount, float newAmount, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = productId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = status,
                OldValue = oldAmount.ToString(),
                NewValue = newAmount.ToString()
            });
        }

        public static void NewCategory(int productId, int categoryId, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            var category = CategoryService.GetCategory(categoryId);

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = productId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName =
                    LocalizationService.GetResource("Core.ProductHistory.CategoryAdded") +
                    (category != null ? category.Name : "")
            });
        }

        public static void DeleteCategory(int productId, int categoryId, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            var category = CategoryService.GetCategory(categoryId);

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = productId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName =
                    LocalizationService.GetResource("Core.ProductHistory.CategoryDeleted") +
                    (category != null ? category.Name : "")
            });
        }

        #endregion

        #region Offer

        public static void NewOffer(Offer offer, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = offer.ProductId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.ProductHistory.OfferCreated") + " " + offer.ArtNo
            });
        }

        public static void DeleteOffer(int offerId, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            var offer = OfferService.GetOffer(offerId);
            if (offer == null)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = offer.ProductId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.ProductHistory.OfferDeleted")
            });
        }

        public static void TrackStockChanges(WarehouseStock warehouseStock, WarehouseStock oldWarehouseStock,
            Offer offer, string warehouseName, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            List<ChangeHistory> history;
            if (oldWarehouseStock is null)
            {
                history = new List<ChangeHistory>
                {
                    new ChangeHistory(changedBy)
                    {
                        ObjId = offer.ProductId,
                        ObjType = ChangeHistoryObjType.Product,
                        NewValue = warehouseStock.Quantity.ToString(),
                        ParameterName = LocalizationService.GetResource("Core.Catalog.WarehouseStock.Quantity") + " "
                            + offer.ArtNo + " по складу " + warehouseName
                    }
                };
            }
            else
                history = ChangeHistoryService.GetChanges(offer.ProductId, ChangeHistoryObjType.Product,
                    oldWarehouseStock, warehouseStock, changedBy,
                    entityName: offer.ArtNo + " по складу " + warehouseName);
            
            ChangeHistoryService.Add(history);
        }

        public static void TrackOfferAmountChanges(float amount, Offer oldOffer, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            if (amount == oldOffer.Amount)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = oldOffer.ProductId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.Catalog.Offer.Amount") + " " + oldOffer.ArtNo,
                NewValue = amount.ToString(),
                OldValue = oldOffer.Amount.ToString()
            });
        }

        public static void TrackOfferChanges(Offer offer, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            var oldOffer = OfferService.GetOffer(offer.OfferId);
            if (oldOffer == null)
                return;

            var history = ChangeHistoryService.GetChanges(offer.ProductId, ChangeHistoryObjType.Product, oldOffer, offer, changedBy, entityName:offer.ArtNo);
            
            ChangeHistoryService.Add(history);
        }
        
        public static void OfferChanged(Offer offer, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = offer.ProductId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResource("Core.ProductHistory.OfferChanged")
            });
        }
        
        public static void OfferAmountChangedByOrder(int orderId, int productId, string artno, string amount, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            var newOffer = OfferService.GetOffer(artno);

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = productId,
                ObjType = ChangeHistoryObjType.Product,
                ParameterName = LocalizationService.GetResourceFormat("Core.ProductHistory.OfferAmountChangedByOrder",
                    orderId, artno, amount),
                NewValue = newOffer?.Amount.ToString() ?? null
            });
        }
        
        public static void TrackExportOptionsChanges(int productId, ProductExportOptions exportOptions, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            var old = ProductExportOptionsService.Get(productId);
            if (old == null)
                return;

            var history = ChangeHistoryService.GetChanges(productId, ChangeHistoryObjType.Product, old, exportOptions, changedBy);
            
            ChangeHistoryService.Add(history);
        }

        #endregion
    }
}
