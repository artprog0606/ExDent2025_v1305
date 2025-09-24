using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Core.Services.Catalog
{
    public class CategoryHistoryService
    {
        public static void NewCategory(Category category, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = category.CategoryId,
                ObjType = ChangeHistoryObjType.Category,
                ParameterName = LocalizationService.GetResource("Core.CategoryHistory.CategoryCreated")
            });
        }

        public static void DeleteCategory(int categoryId, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = categoryId,
                ObjType = ChangeHistoryObjType.Category,
                ParameterName = LocalizationService.GetResource("Core.CategoryHistory.CategoryDeleted")
            });
        }

        public static void TrackCategoryChanges(Category category, Category oldCategory, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            var history = ChangeHistoryService.GetChanges(category.CategoryId, ChangeHistoryObjType.Category, oldCategory, category, changedBy);

            ChangeHistoryService.Add(history);
        }

        public static void CategoryChanged(Category category, ChangedBy changedBy)
        {
            if (!SettingsMain.TrackProductChanges)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = category.CategoryId,
                ObjType = ChangeHistoryObjType.Category,
                ParameterName = LocalizationService.GetResource("Core.CategoryHistory.CategoryChanged")
            });
        }
    }
}
