using System;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.SalesChannels;

namespace AdvantShop.Configuration
{
    public enum EFeature
    {
        //[Localize("Новый дашборд")]
        //[DescriptionKey("Показывать \"Мои сайты\" и новый дашбоард")]
        //NewDashboard,

        [Localize("Обязательная маркировка")]
        [DescriptionKey("Если настройка включена, и для товара задана обязательность маркировки \"Честный знак\", то в заказе у товара будет показываться и храниться маркировка.")]
        MarkingRequired,

        [Localize("Изображения в дополнительных опциях")]
        [DescriptionKey("Возможность добавлять изображения в дополнительных опциях товара")]
        CustomOptionPicture,

        [Localize("Расширенные настройки дополнительных опций")]
        [DescriptionKey("Возможность выводить минимальное и максимальное количество опций, а также количество по умолчанию для доп. опций товара.\nНастройка влияет на отображение доп.опций на витрине (только для шаблона Sushi).")]
        CustomOptionCombo,

        [Localize("Функционал складов в типах цен")]
        [DescriptionKey("Возможность добавлять тип цен со складом")]
        PriceTypeWithWarehouse,
    }

    public class FeaturesService
    {
        public static bool IsEnabled(EFeature feature)
        {
            return SettingsFeatures.EnableExperimentalFeatures && SettingsFeatures.IsFeatureEnabled(feature);
        }
    }

    public class SettingsFeatures
    {
        public static bool EnableExperimentalFeatures
        {
            get => Convert.ToBoolean(SettingProvider.Items["Features.EnableExperimentalFeatures"]);
            set
            {
                SettingProvider.Items["Features.EnableExperimentalFeatures"] = value.ToString();
                CacheManager.RemoveByPattern(SalesChannelService.CacheKey);
            }
        }

        public static bool IsFeatureEnabled(EFeature feature)
        {
            return Convert.ToBoolean(SettingProvider.Items["Features.Enable" + feature.ToString()]);
        }

        public static void SetFeatureEnabled(EFeature feature, bool enabled)
        {
            SettingProvider.Items["Features.Enable" + feature.ToString()] = enabled.ToString();
            CacheManager.RemoveByPattern(SalesChannelService.CacheKey);
        }
    }
}