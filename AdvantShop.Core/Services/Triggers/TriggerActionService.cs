using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Repository;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Triggers
{
    public class TriggerActionService
    {
        public static List<TriggerAction> GetTriggerActions(int triggerRuleId)
        {
            return CacheManager.Get(TriggerCacheKeys.ActionsListCachePrefix + triggerRuleId,
                () =>
                    SQLDataAccess.ExecuteReadList(
                        "Select * From CRM.TriggerAction Where TriggerRuleId=@TriggerRuleId Order by SortOrder asc, Id asc",
                        CommandType.Text, GetReader,
                        new SqlParameter("@TriggerRuleId", triggerRuleId)));
        }

        public static TriggerAction GetTriggerAction(int id)
        {
            return CacheManager.Get(TriggerCacheKeys.ActionCachePrefix + id,
                () =>
                    SQLDataAccess.ExecuteReadOne("Select * From CRM.TriggerAction Where Id=@id",
                        CommandType.Text, GetReader,
                        new SqlParameter("@id", id)));
        }

        public static TriggerAction GetTriggerAction(Guid emailingId)
        {
            return SQLDataAccess.ExecuteReadOne("Select * From CRM.TriggerAction Where EmailingId=@EmailingId",
                CommandType.Text, GetReader,
                new SqlParameter("@EmailingId", emailingId));
        }

        private static TriggerAction GetReader(SqlDataReader reader)
        {
            var requestParams = SQLDataHelper.GetString(reader, "RequestParams");
            var requestHeaderParams = SQLDataHelper.GetString(reader, "RequestHeaderParams");
            var actionId = SQLDataHelper.GetInt(reader, "Id");
            var notificationRequestParams = SQLDataHelper.GetString(reader, "NotificationRequestParams");
            var paramsJson = SQLDataHelper.GetString(reader, "ParamsJson");
            var actionType = (ETriggerActionType)SQLDataHelper.GetInt(reader, "ActionType");

            return new TriggerAction()
            {
                Id = actionId,
                TriggerRuleId = SQLDataHelper.GetInt(reader, "TriggerRuleId"),
                ActionType = (ETriggerActionType)SQLDataHelper.GetInt(reader, "ActionType"),
                SendEmailData = new TriggerActionSendEmailData
                {
                    EmailSubject = SQLDataHelper.GetString(reader, "EmailSubject"),
                    EmailBody = SQLDataHelper.GetString(reader, "EmailBody"),
                    Params = paramsJson.IsNullOrEmpty() || actionType != ETriggerActionType.Email
                                ? new TriggerActionSendEmailAdditionalParams() { RecipientIsCustomer = true }
                                : JsonConvert.DeserializeObject<TriggerActionSendEmailAdditionalParams>(paramsJson)
                },
                SendSmsData = new TriggerActionSendSmsData
                {
                    SmsText = SQLDataHelper.GetString(reader, "SmsText"),
                    SmsTemplateId = SQLDataHelper.GetNullableInt(reader, "SmsTemplateId"),
                    Params = paramsJson.IsNullOrEmpty() || actionType != ETriggerActionType.Sms
                                ? new TriggerActionSendSmsDataAdditionalParams { RecipientIsCustomer = true }
                                : JsonConvert.DeserializeObject<TriggerActionSendSmsDataAdditionalParams>(paramsJson)
                },
                MessageText = SQLDataHelper.GetString(reader, "MessageText"),
                TimeDelay = JsonConvert.DeserializeObject<TimeInterval>(SQLDataHelper.GetString(reader, "TimeDelay")),
                EditField = new EditField()
                {
                    Type = SQLDataHelper.GetNullableInt(reader, "EditFieldType"),
                    ObjId = SQLDataHelper.GetNullableInt(reader, "ObjId"),
                    EditFieldValue = SQLDataHelper.GetString(reader, "EditFieldValue"),
                    DealStatusId = SQLDataHelper.GetNullableInt(reader, "DealStatusId"),
                    Params = paramsJson.IsNullOrEmpty() || actionType != ETriggerActionType.Edit
                                ? null
                                : JsonConvert.DeserializeObject<EditFieldParams>(paramsJson)
                },
                EmailingId = SQLDataHelper.GetGuid(reader, "EmailingId"),
                SendRequestData = new TriggerActionSendRequestData()
                {
                    RequestMethod = (TriggerActionSendRequestMethod)SQLDataHelper.GetInt(reader, "RequestMethod", 0),
                    RequestUrl = SQLDataHelper.GetString(reader, "RequestUrl"),
                    RequestParams =
                        !string.IsNullOrEmpty(requestParams)
                            ? JsonConvert.DeserializeObject<List<TriggerActionSendRequestParam>>(requestParams)
                            : null,
                    RequestHeaderParams =
                        !string.IsNullOrEmpty(requestHeaderParams)
                            ? JsonConvert.DeserializeObject<List<TriggerActionSendRequestParam>>(requestHeaderParams)
                            : null,
                    RequestParamsType = (TriggerActionSendRequestParamsType)SQLDataHelper.GetInt(reader, "RequestParamsType"),
                    RequestParamsJson = SQLDataHelper.GetString(reader, "RequestParamsJson")
                },
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                NotificationTitle = SQLDataHelper.GetString(reader, "NotificationTitle"),
                NotificationBody = SQLDataHelper.GetString(reader, "NotificationBody"),
                NotificationRequestParams = notificationRequestParams.IsNullOrEmpty()
                                                ? null
                                                : JsonConvert.DeserializeObject<List<TriggerPushNotification>>(notificationRequestParams),
                SendToShippingServiceData = paramsJson.IsNullOrEmpty() || actionType != ETriggerActionType.SendToShippingService 
                    ? new TriggerActionSendToShippingServiceData
                    {
                        EmailBody = LocalizationService.GetResource("Core.Triggers.Action.ErrorSendToShippingService.DefaultEmailBody"),
                        EmailSubject = LocalizationService.GetResource("Core.Triggers.Action.ErrorSendToShippingService.DefaultEmailSubject")
                    }
                    : JsonConvert.DeserializeObject<TriggerActionSendToShippingServiceData>(paramsJson)
            };
        }

        public static int Add(TriggerAction action)
        {
            var paramsJson = GetParamsJson(action);
            action.Id =
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert Into CRM.TriggerAction (TriggerRuleId, ActionType, TimeDelay, EmailSubject, EmailBody, SmsText, EditFieldType, EditFieldValue, ObjId, EmailingId, DealStatusId, ParamsJson, RequestMethod, RequestUrl, RequestParams, RequestHeaderParams, SortOrder, RequestParamsType, MessageText, NotificationTitle, NotificationBody, NotificationRequestParams, SmsTemplateId) " +
                    "Values (@TriggerRuleId, @ActionType, @TimeDelay, @EmailSubject, @EmailBody, @SmsText, @EditFieldType, @EditFieldValue, @ObjId, @EmailingId, @DealStatusId, @ParamsJson, @RequestMethod, @RequestUrl, @RequestParams, @RequestHeaderParams, @SortOrder, @RequestParamsType, @MessageText, @NotificationTitle, @NotificationBody, @NotificationRequestParams, @SmsTemplateId); Select scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@TriggerRuleId", action.TriggerRuleId),
                    new SqlParameter("@ActionType", action.ActionType),
                    new SqlParameter("@TimeDelay",
                        action.TimeDelay != null ? JsonConvert.SerializeObject(action.TimeDelay) : ""),
                    new SqlParameter("@EmailSubject", action.SendEmailData?.EmailSubject ?? (object) DBNull.Value),
                    new SqlParameter("@EmailBody", action.SendEmailData?.EmailBody ?? (object) DBNull.Value),
                    new SqlParameter("@SmsText", action.SendSmsData?.SmsText ?? (object) DBNull.Value),
                    new SqlParameter("@SmsTemplateId", action.SendSmsData?.SmsTemplateId ?? (object) DBNull.Value),
                    new SqlParameter("@MessageText", action.MessageText ?? (object)DBNull.Value),
                    new SqlParameter("@EditFieldType", (action.EditField?.Type) ?? (object) DBNull.Value),
                    new SqlParameter("@ObjId", (action.EditField?.ObjId) ?? (object) DBNull.Value),
                    new SqlParameter("@EditFieldValue", (action.EditField?.EditFieldValue) ?? (object) DBNull.Value),
                    new SqlParameter("@DealStatusId", (action.EditField?.DealStatusId) ?? (object) DBNull.Value),
                    new SqlParameter("@EmailingId", Guid.NewGuid()),

                    new SqlParameter("@RequestMethod", action.SendRequestData != null ? (int) action.SendRequestData.RequestMethod : 0),
                    new SqlParameter("@RequestUrl", action.SendRequestData?.RequestUrl ?? (object) DBNull.Value),
                    new SqlParameter("@RequestParams",
                        action.SendRequestData?.RequestParams != null
                            ? JsonConvert.SerializeObject(action.SendRequestData.RequestParams)
                            : (object) DBNull.Value),
                    new SqlParameter("@RequestHeaderParams",
                        action.SendRequestData?.RequestHeaderParams != null
                            ? JsonConvert.SerializeObject(action.SendRequestData.RequestHeaderParams)
                            : (object) DBNull.Value),

                    new SqlParameter("@RequestParamsType", action.SendRequestData != null ? (int)action.SendRequestData.RequestParamsType : 0),
                    new SqlParameter("@RequestParamsJson", action.SendRequestData?.RequestParamsJson ?? (object)DBNull.Value),

                    new SqlParameter("@SortOrder", action.SortOrder),
                    new SqlParameter("@NotificationBody", action.NotificationBody ?? (object)DBNull.Value),
                    new SqlParameter("@NotificationTitle", action.NotificationTitle ?? (object)DBNull.Value),
                    new SqlParameter("@NotificationRequestParams", 
                        action.NotificationRequestParams != null 
                            ? JsonConvert.SerializeObject(action.NotificationRequestParams)
                            : (object)DBNull.Value),
                    new SqlParameter("@ParamsJson", paramsJson ?? (object)DBNull.Value)
                );
            
            CacheManager.RemoveByPattern(TriggerCacheKeys.ActionsListCachePrefix + action.TriggerRuleId);
            
            return action.Id;
        }

        public static void Update(TriggerAction action)
        {
            var paramsJson = GetParamsJson(action);
            SQLDataAccess.ExecuteNonQuery(
                "Update CRM.TriggerAction " +
                "Set TriggerRuleId=@TriggerRuleId, ActionType=@ActionType, TimeDelay=@TimeDelay, EmailSubject=@EmailSubject, EmailBody=@EmailBody, SmsText=@SmsText, EditFieldType=@EditFieldType, EditFieldValue=@EditFieldValue, ObjId=@ObjId, " +
                "DealStatusId=@DealStatusId, ParamsJson=@ParamsJson, RequestMethod=@RequestMethod, RequestUrl=@RequestUrl, RequestParams=@RequestParams, RequestHeaderParams=@RequestHeaderParams, SortOrder=@SortOrder, " +
                "RequestParamsType=@RequestParamsType, RequestParamsJson=@RequestParamsJson, MessageText=@MessageText, NotificationBody = @NotificationBody, NotificationTitle = @NotificationTitle, NotificationRequestParams = @NotificationRequestParams," +
                "SmsTemplateId=@SmsTemplateId " +
                "Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", action.Id),
                new SqlParameter("@TriggerRuleId", action.TriggerRuleId),
                new SqlParameter("@ActionType", action.ActionType),
                new SqlParameter("@TimeDelay",
                    action.TimeDelay != null ? JsonConvert.SerializeObject(action.TimeDelay) : ""),
                new SqlParameter("@EmailSubject", action.SendEmailData?.EmailSubject ?? (object) DBNull.Value),
                new SqlParameter("@EmailBody", action.SendEmailData?.EmailBody ?? (object) DBNull.Value),
                new SqlParameter("@SmsText", action.SendSmsData?.SmsText ?? (object) DBNull.Value),
                new SqlParameter("@SmsTemplateId", action.SendSmsData?.SmsTemplateId ?? (object) DBNull.Value),
                new SqlParameter("@MessageText", action.MessageText ?? (object)DBNull.Value),
                new SqlParameter("@EditFieldType",
                    (action.EditField?.Type) ?? (object) DBNull.Value),
                new SqlParameter("@ObjId",
                    (action.EditField?.ObjId) ?? (object) DBNull.Value),
                new SqlParameter("@EditFieldValue",
                    (action.EditField?.EditFieldValue) ?? (object) DBNull.Value),
                new SqlParameter("@DealStatusId",
                    (action.EditField?.DealStatusId) ?? (object) DBNull.Value),
                
                new SqlParameter("@RequestMethod", action.SendRequestData != null ? (int) action.SendRequestData.RequestMethod : 0),
                new SqlParameter("@RequestUrl", action.SendRequestData?.RequestUrl ?? (object) DBNull.Value),
                new SqlParameter("@RequestParams",
                    action.SendRequestData?.RequestParams != null
                        ? JsonConvert.SerializeObject(action.SendRequestData.RequestParams)
                        : (object) DBNull.Value),
                new SqlParameter("@RequestHeaderParams",
                    action.SendRequestData?.RequestHeaderParams != null
                        ? JsonConvert.SerializeObject(action.SendRequestData.RequestHeaderParams)
                        : (object) DBNull.Value),

                new SqlParameter("@RequestParamsType", action.SendRequestData != null ? (int) action.SendRequestData.RequestParamsType : 0),
                new SqlParameter("@RequestParamsJson", action.SendRequestData?.RequestParamsJson ?? (object) DBNull.Value),

                new SqlParameter("@SortOrder", action.SortOrder),
                new SqlParameter("@NotificationTitle", action.NotificationTitle ?? (object)DBNull.Value),
                new SqlParameter("@NotificationBody", action.NotificationBody ?? (object)DBNull.Value),
                new SqlParameter("@NotificationRequestParams", 
                    action.NotificationRequestParams != null
                        ? JsonConvert.SerializeObject(action.NotificationRequestParams)
                        : (object)DBNull.Value),
                new SqlParameter("@ParamsJson", paramsJson ?? (object)DBNull.Value)
            );
            
            CacheManager.RemoveByPattern(TriggerCacheKeys.ActionCachePrefix);
            CacheManager.RemoveByPattern(TriggerCacheKeys.ActionsListCachePrefix + action.TriggerRuleId);
        }

        public static void Delete(int id)
        {
            var action = GetTriggerAction(id);
            
            SQLDataAccess.ExecuteNonQuery("Delete From CRM.TriggerAction Where Id=@id", CommandType.Text, new SqlParameter("@id", id));
            AdditionalOptionsService.Delete(id, EnAdditionalOptionObjectType.Trigger);
            
            CacheManager.RemoveByPattern(TriggerCacheKeys.ActionCachePrefix + id);
            if (action != null)
                CacheManager.RemoveByPattern(TriggerCacheKeys.ActionsListCachePrefix + action.TriggerRuleId);
        }

        private static string GetParamsJson(TriggerAction action)
        {
            if (action.ActionType == ETriggerActionType.Edit)
                return JsonConvert.SerializeObject(action.EditField.Params);
            if (action.ActionType == ETriggerActionType.Email && action.SendEmailData != null)
                return JsonConvert.SerializeObject(action.SendEmailData.Params);
            if (action.ActionType == ETriggerActionType.Sms)
                return JsonConvert.SerializeObject(action.SendSmsData.Params);
            if (action.ActionType == ETriggerActionType.SendToShippingService)
                return JsonConvert.SerializeObject(action.SendToShippingServiceData);
            return null;
        }
    }
}
