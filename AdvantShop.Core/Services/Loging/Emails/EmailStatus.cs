using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Loging.Emails
{
    public enum EmailStatus
    {
        None = 0,

        [Localize("Core.Loging.EmailStatus.PrepareSend")]
        [DescriptionKey("Core.Loging.EmailStatus.PrepareSend.Description")]
        PrepareSend = 1,

        [Localize("Core.Loging.EmailStatus.Sending")]
        [DescriptionKey("Core.Loging.EmailStatus.Sending.Description")]
        Sending = 2,

        [Localize("Core.Loging.EmailStatus.Sent")]
        [DescriptionKey("Core.Loging.EmailStatus.Sent.Description")]
        Sent = 3,               

        [Localize("Core.Loging.EmailStatus.Delivered")]
        [DescriptionKey("Core.Loging.EmailStatus.Delivered.Description")]
        Delivered = 4,

        [Localize("Core.Loging.EmailStatus.Opened")]
        [DescriptionKey("Core.Loging.EmailStatus.Opened.Description")]
        Opened = 5,

        [Localize("Core.Loging.EmailStatus.Clicked")]
        [DescriptionKey("Core.Loging.EmailStatus.Clicked.Description")]
        Clicked = 6,

        [Localize("Core.Loging.EmailStatus.Unsubscribed")]
        [DescriptionKey("Core.Loging.EmailStatus.Unsubscribed.Description")]
        Unsubscribed = 7,

        [Localize("Core.Loging.EmailStatus.SoftBounced")]
        [DescriptionKey("Core.Loging.EmailStatus.SoftBounced.Description")]
        SoftBounced = 9,

        [Localize("Core.Loging.EmailStatus.HardBounced")]
        [DescriptionKey("Core.Loging.EmailStatus.HardBounced.Description")]
        HardBounced = 9,

        [Localize("Core.Loging.EmailStatus.Spam")]
        [DescriptionKey("Core.Loging.EmailStatus.Spam.Description")]
        Spam = 10,

        [Localize("Core.Loging.EmailStatus.Error")]
        [DescriptionKey("Core.Loging.EmailStatus.Error.Description")]
        Error = 11,
    }


    public enum AdvantShopEmailErrorStatus
    {
        None,

        [Localize("Core.Loging.AdvantShopEmailErrorStatus.Unsubscribed")]
        Unsubscribed,

        [Localize("Core.Loging.AdvantShopEmailErrorStatus.Invalid")]
        Invalid,

        [Localize("Core.Loging.AdvantShopEmailErrorStatus.Duplicate")]
        Duplicate,

        [Localize("Core.Loging.AdvantShopEmailErrorStatus.TemporaryUnavailable")]
        TemporaryUnavailable,

        [Localize("Core.Loging.AdvantShopEmailErrorStatus.PermanentUnavailable")]
        PermanentUnavailable,

        [Localize("Core.Loging.AdvantShopEmailErrorStatus.InternalServerError")]
        InternalServerError,
    }
}
