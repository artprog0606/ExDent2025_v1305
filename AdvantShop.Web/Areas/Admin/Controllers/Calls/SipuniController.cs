using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Core.Services.IPTelephony.Sipuni;
using AdvantShop.Saas;
using Call = AdvantShop.Core.Services.IPTelephony.Call;

namespace AdvantShop.Web.Admin.Controllers.Calls
{
    public class SipuniController : BaseCallController
    {
        private static readonly Object SyncObject = new Object();

        public JsonResult PushNotification([ModelBinder(typeof(SipuniModelBinder))] SipuniNotificationDto model)
        {
            lock (SyncObject)
            {
                //AdvantShop.Statistic.CommonStatistic.WriteLog(DateTime.Now.ToString("HH:mm:ss:ffffff") + ": " + ControllerContext.RequestContext.HttpContext.Request.RawUrl);

                if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveTelephony)
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }

                LogNotification();

                if (model == null || SettingsTelephony.CurrentIPTelephonyOperatorType != EOperatorType.Sipuni)
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);

                var call = CallService.GetCall(model.CallId, EOperatorType.Sipuni)
                           ?? new Call
                           {
                               CallId = model.CallId,
                               SrcNum = model.SrcNumShort.ToString(),
                               DstNum = model.DstNumShort,
                               CallDate = model.Timestamp,
                               OperatorType = EOperatorType.Sipuni
                           };
                call.IsComplete = false;

                switch (model.Event)
                {
                    case ESipuniEvent.Call:
                        var callModel = (SipuniCallDto)model;
                        call.Type = callModel.SrcType == ESipuniAddressType.Outer ? ECallType.In : ECallType.Out;
                        call.Phone = callModel.SrcType == ESipuniAddressType.Outer ? callModel.SrcNumShort.ToString() : callModel.DstNumShort;

                        if (callModel.DstType == ESipuniAddressType.Inner)
                            call.Extension = callModel.DstNumShort;
                        else if (callModel.SrcType == ESipuniAddressType.Inner)
                            call.Extension = callModel.SrcNumShort;

                        if ((!callModel.IsInnerCall || SettingsTelephony.SipuniConsiderInnerCalls) && callModel.SrcType == ESipuniAddressType.Outer) 
                            SendIncomingCallNotify(call);
                        break;
                    case ESipuniEvent.Answer:
                        break;
                    case ESipuniEvent.HangUp:
                    //case ESipuniEvent.SecondaryHangUp:
                        var hangupModel = (SipuniHangupDto)model;
                        call.CallDate = hangupModel.CallStartTimestamp;
                        call.IsComplete = true;
                        if (hangupModel.CallAnswerTimestamp.HasValue)
                        {
                            call.CallAnswerDate = hangupModel.CallAnswerTimestamp.Value;
                            call.Duration = (int)(hangupModel.Timestamp - hangupModel.CallAnswerTimestamp.Value).TotalSeconds;
                        }
                        call.RecordLink = hangupModel.CallRecordLink;
                        if (call.Type != ECallType.Out && !hangupModel.CallAnswerTimestamp.HasValue)
                        {
                            call.Type = ECallType.Missed;
                            call.HangupStatus = hangupModel.Status.ToString().TryParseEnum<ECallHangupStatus>();
                            ProcessMissedCall(call);
                        }
                        SendCallEndedNotify(call);
                        break;
                }

                if (call.Id == 0)
                    call.Id = CallService.AddCall(call);
                else
                    CallService.UpdateCall(call);

                CallService.ProcessCall(call);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}