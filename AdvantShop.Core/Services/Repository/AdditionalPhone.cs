using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Repository
{
    public class AdditionalPhone
    {
        public string Phone { get; set; }
        public string StandardPhone { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public EAdditionalPhoneType Type { get; set; }

        [JsonIgnore]
        public string PhoneLinkByType
        {
            get
            {
                if (Type == EAdditionalPhoneType.WhatsApp)
                    return "https://wa.me/" + StandardPhone;

                if (Type == EAdditionalPhoneType.Viber)
                    return "viber://chat?number=" + StandardPhone;

                if (Type == EAdditionalPhoneType.Telegram)
                    return "https://t.me/" + Icon;                
                
                if (Type == EAdditionalPhoneType.Skype)
                    return "Skype:" + StandardPhone + "?call";

                return "tel:" + StandardPhone;
            }
        }

        [JsonIgnore]
        public string PhoneTextByType
        {
            get
            {
                if (Type == EAdditionalPhoneType.Telegram)
                    return Description;

                return Phone;
            }
        }
    }

    public enum EAdditionalPhoneType
    {
        [Localize("Телефон")]
        Phone = 0,

        [Localize("Своя SVG-иконка")]
        Svg = 1,

        [Localize("WhatsApp")]
        WhatsApp = 2,

        [Localize("Viber")]
        Viber = 3,

        [Localize("Telegram")]
        Telegram = 4,

        [Localize("Skype")]
        Skype = 5
    }
}
