//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.DownloadableContent.Interfaces;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Core.Modules
{
    [Serializable]
    public class Module : IDownloadableContent
    {
        public int Id { get; set; }
        public string StringId { get; set; }
        public string Version { get; set; }
        public bool Active { get; set; }

        public string Name { get; set; }
        public float Price { get; set; }
        public string Currency { get; set; }
        public bool IsFree { get; set; }

        public string DetailsLink { get; set; }
        public string BriefDescription { get; set; }

        public int SortOrder { get; set; }
        public string Icon { get; set; }

        public string CurrentVersion { get; set; }
        public bool IsLocalVersion { get; set; }
        public bool IsCustomVersion { get; set; }
        public bool IsPersonalModule { get; set; }
        public bool IsInstall { get; set; }

        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }

        public bool Enabled { get; set; }

        public bool Popular { get; set; }
        public bool New { get; set; }

        public string InstructionTitle { get; set; }
        public string InstructionLink { get; set; }

        public bool NeedUpdate { get; set; }

        public bool DissalowInstall { get; set; }

        public string PriceString
        {
            get
            {
                return Price != 0 ? String.Format("{0:##,##0.##}", Price) + " " + Currency : LocalizationService.GetResource("Core.Modules.FreeCost");
            }
        }


        public bool ShowInSalesChannels { get; set; }
        public string TitleInSalesChannels { get; set; }
        public string MenuNameInSalesChannels { get; set; }
        public string MenuIconInSalesChannels { get; set; }
        public string IconInSalesChannels { get; set; }
        public string BriefDescriptionInSalesChannels { get; set; }
        public string DescriptionInSalesChannels { get; set; }
        public string ImageInSalesChannels { get; set; }

        public bool ShowPreview { get; set; }
        public bool ShowInstalledAndPreviewInSalesChannel { get; set; }
        public string PreviewRightTextInSalesChannel { get; set; }
        public string PreviewLeftTextInSalesChannel { get; set; }
        public string PreviewButtonTextInSalesChannel { get; set; }
        public string CategoryName { get; set; }
        public int CategorySortOrder { get; set; }

        public bool IsPayment { get; set; }
        public string PaymentName { get; set; }
        public string PaymentKey { get; set; }
        public string PaymentIcon { get; set; }

        public bool IsShipping { get; set; }
        public string ShippingName { get; set; }
        public string ShippingKey { get; set; }
        public string ShippingIcon { get; set; }

        public bool IsMobileAdminReady { get; set; }
    }
}