using System.Collections.Generic;

namespace AdvantShop.Core.Services.Triggers
{
    public class TriggerActionSendRequestData
    {
        public TriggerActionSendRequestMethod RequestMethod { get; set; }
        public string RequestUrl { get; set; }
        public List<TriggerActionSendRequestParam> RequestParams { get; set; }
        public List<TriggerActionSendRequestParam> RequestHeaderParams { get; set; }
        public TriggerActionSendRequestParamsType RequestParamsType { get; set; }
        public string RequestParamsJson { get; set; }
    }

    public class TriggerActionSendRequestParam
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public enum TriggerActionSendRequestMethod
    {
        Get = 0,
        Post = 1
    }

    public enum TriggerActionSendRequestParamsType
    {
        Parameters = 0,
        Json = 1
    }
}
