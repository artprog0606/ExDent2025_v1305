using System;

namespace AdvantShop.Core.Common.Attributes
{
    public class TrackEventAttribute : Attribute
    {
        public string EventKey { get; set; }
        public bool SendOnce { get; set; }
        public string TrialPrefix { get; set; }
        public string Delimiter { get; set; }
        //public bool OnceDaily { get; set; }

        public ModeConfigService.Modes? ShopMode { get; set; }

        public TrackEventAttribute(string eventKey)
        {
            EventKey = eventKey;
        }

        public TrackEventAttribute(ModeConfigService.Modes shopMode)
        {
            ShopMode = shopMode;
        }
    }
}