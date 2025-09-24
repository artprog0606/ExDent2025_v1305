using System;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Triggers
{
    public interface ITriggerParams
    {
    }

    public enum TriggerParamsDateSince
    {
        [Localize("за")]
        Before = 0,

        [Localize("через")]
        After = 1
    }

    public class TriggerParamsDate : ITriggerParams
    {
        public DateTime DateTime { get; set; }
        public bool IgnoreYear { get; set; }

        public TriggerParamsDateSince Since { get; set; }
        public int? Days { get; set; }
    }

    public class TriggerParamsDaysBeforeDate : ITriggerParams
    {
        public bool IsCustomField { get; set; }
        public string CustomFieldId { get; set; }
        public bool IgnoreYear { get; set; }

        public TriggerParamsDateSince Since { get; set; }
        public int Days { get; set; }
    }
}
