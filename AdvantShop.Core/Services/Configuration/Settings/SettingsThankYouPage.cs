using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Configuration.Settings
{
    public enum EThankYouPageActionType
    {
        [Localize("Core.Configuration.EThankYouPageActionType.None")]
        None,
        [Localize("Core.Configuration.EThankYouPageActionType.JoinGroup")]
        JoinGroup,
        [Localize("Core.Configuration.EThankYouPageActionType.ShowProducts")]
        ShowProducts
    }

    public enum ESocialNetworkType
    {
        [Localize("Core.Configuration.ESocialNetworkType.Vk")]
        Vk,
        [Localize("Core.Configuration.ESocialNetworkType.Facebook")]
        Facebook,
        [Localize("Core.Configuration.ESocialNetworkType.Youtube")]
        Youtube,
        [Localize("Core.Configuration.ESocialNetworkType.Twitter")]
        Twitter,
        [Localize("Core.Configuration.ESocialNetworkType.Instagram")]
        Instagram,
        [Localize("Core.Configuration.ESocialNetworkType.Telegram")]
        Telegram,
        [Localize("Core.Configuration.ESocialNetworkType.Odnoklassniki")]
        Odnoklassniki,
        [Localize("Core.Configuration.ESocialNetworkType.YandexZen")]
        YandexZen,
        [Localize("Core.Configuration.ESocialNetworkType.Rutube")]
        Rutube
    }

    public enum ESocialNetworkUrlPlaceholder
    {
        [Localize("https://vk.com/<ID>")]
        Vk,
        [Localize("https://www.facebook.com/<ID>")]
        Facebook,
        [Localize("https://www.youtube.com/c/<Channel>")]
        Youtube,
        [Localize("https://www.twitter.com/<Username>")]
        Twitter,
        [Localize("https://www.instagram.com/<Username>")]
        Instagram,
        [Localize("https://t.me/<Username>")]
        Telegram,
        [Localize("https://ok.ru/messages/group/<GROUP_ID>")]
        Odnoklassniki,
        [Localize("https://zen.yandex.ru/<Username>")]
        YandexZen,
        [Localize("https://rutube.ru/channel/<Channel>")]
        Rutube
    }

    public class SocialNetworkGroup
    {
        public ESocialNetworkType Type { get; set; }
        public bool Enabled { get; set; }
        public string Link { get; set; }
    }



    public class SettingsThankYouPage
    {
        public static EThankYouPageActionType ActionType
        {
            get => SettingProvider.Items["SettingsThankYouPage.ActionType"].TryParseEnum<EThankYouPageActionType>();
            set => SettingProvider.Items["SettingsThankYouPage.ActionType"] = value.ToString();
        }

        public static List<SocialNetworkGroup> SocialNetworks
        {
            get
            {
                var serialized = SettingProvider.Items["SettingsThankYouPage.SocialNetworks"];
                var groups = (serialized.IsNotEmpty()
                    ? JsonConvert.DeserializeObject<List<SocialNetworkGroup>>(serialized)
                    : null) ?? new List<SocialNetworkGroup>();

                return Enum.GetValues(typeof(ESocialNetworkType)).Cast<ESocialNetworkType>()
                    .Select(type => groups.FirstOrDefault(x => x.Type == type) ?? new SocialNetworkGroup { Type = type }).ToList();
            }
            set => SettingProvider.Items["SettingsThankYouPage.SocialNetworks"] = value != null ? JsonConvert.SerializeObject(value) : null;
        }

        public static string NameOfBlockProducts
        {
            get => SettingProvider.Items["SettingsThankYouPage.NameOfBlockProducts"];
            set => SettingProvider.Items["SettingsThankYouPage.NameOfBlockProducts"] = value;
        }

        public static bool ShowReletedProducts
        {
            get => SettingProvider.Items["SettingsThankYouPage.ShowReletedProducts"].TryParseBool();
            set => SettingProvider.Items["SettingsThankYouPage.ShowReletedProducts"] = value.ToString();
        }

        public static RelatedType ReletedProductsType
        {
            get => SettingProvider.Items["SettingsThankYouPage.ReletedProductsType"].TryParseEnum<RelatedType>();
            set => SettingProvider.Items["SettingsThankYouPage.ReletedProductsType"] = value.ToString();
        }

        public static bool ShowProductsList
        {
            get => SettingProvider.Items["SettingsThankYouPage.ShowProductsList"].TryParseBool();
            set => SettingProvider.Items["SettingsThankYouPage.ShowProductsList"] = value.ToString();
        }

        public static EProductOnMain ProductsListType
        {
            get => SettingProvider.Items["SettingsThankYouPage.ProductsListType"].TryParseEnum<EProductOnMain>();
            set => SettingProvider.Items["SettingsThankYouPage.ProductsListType"] = value.ToString();
        }

        public static int? ProductsListId
        {
            get => SettingProvider.Items["SettingsThankYouPage.ProductsListId"].TryParseInt(true);
            set => SettingProvider.Items["SettingsThankYouPage.ProductsListId"] = value.ToString();
        }

        public static bool ShowSelectedProducts
        {
            get => SettingProvider.Items["SettingsThankYouPage.ShowSelectedProducts"].TryParseBool();
            set => SettingProvider.Items["SettingsThankYouPage.ShowSelectedProducts"] = value.ToString();
        }

        public static List<int> SelectedProductIds
        {
            get
            {
                return SettingProvider.Items["SettingsThankYouPage.SelectedProductIds"].DefaultOrEmpty().Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.TryParseInt()).ToList();
            }
            set => SettingProvider.Items["SettingsThankYouPage.SelectedProductIds"] = value != null ? value.AggregateString(",") : null;
        }

        public static List<int> ExcludedPaymentIds
        {
            get
            {
                var serialized = SettingProvider.Items["SettingsThankYouPage.ExcludedPaymentIds"].DefaultOrEmpty();
                return (serialized.IsNotEmpty()
                    ? JsonConvert.DeserializeObject<List<int>>(serialized)
                    : null) ?? new List<int>();
            }
            set => SettingProvider.Items["SettingsThankYouPage.ExcludedPaymentIds"] = value != null ? JsonConvert.SerializeObject(value) : null;
        }

    }
}
