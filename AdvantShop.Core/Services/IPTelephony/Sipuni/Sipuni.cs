using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Core.Services.IPTelephony.Sipuni
{
    public class Sipuni : IPTelephonyOperator
    {
        private string _apiKey;

        public override EOperatorType Type
        {
            get { return EOperatorType.Sipuni; }
        }

        public Sipuni()
        {
            _apiKey = SettingsTelephony.SipuniApiKey;
        }

        public Sipuni(string apiKey)
        {
            _apiKey = apiKey;
        }

        public override CallBack.CallBack CallBack
        {
            get { return new SipuniCallBack(); }
        }

        public override string GetRecordLink(int callId)
        {
            var call = CallService.GetCall(callId);
            if (call == null || call.RecordLink.IsNullOrEmpty())
                return string.Empty;

            return call.RecordLink;
        }
    }
}
