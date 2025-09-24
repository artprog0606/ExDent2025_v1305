using System;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Shared.Vk;
using AdvantShop.Web.Admin.Models.Crm;
using AdvantShop.Web.Admin.Models.Shared.Socials;
using AdvantShop.Web.Infrastructure.Filters;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Controllers.Crm
{
    [Auth(RoleAction.Vk)]
    [SalesChannel(ESalesChannelType.Vk)]
    public partial class VkController : BaseAdminController
    {
        #region Ctor

        private readonly VkApiService _vkService;

        public VkController()
        {
            _vkService = new VkApiService();
        }

        #endregion

        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Vk.Index.Title"));
            SetNgController(Infrastructure.Controllers.NgControllers.NgControllersTypes.VkMainCtrl);

            return View();
        }
        
        #region Settings 

        public JsonResult GetVkSettings()
        {
            var group = SettingsVk.Group;

            var isConfigured = SettingsVk.UserTokenData != null 
                               && SettingsVk.UserTokenData.access_token.IsNotEmpty();

            string codeChallenge = null;
            string state = null;
            
            if (!isConfigured)
            {
                state = SettingsVk.UserTokenState = Guid.NewGuid().ToString();
                SettingsVk.UserTokenCodeVerifier = Guid.NewGuid().ToString();
                
                codeChallenge = SettingsVk.UserTokenCodeVerifier.Sha256AndToBase64UrlEncode();
            }

            return Json(new
            {
                clientId = SettingsVk.ApplicationId,
                groups = isConfigured ? _vkService.GetUserGroups() : null,
                group = group,
                salesFunnels = SalesFunnelService.GetList(),
                salesFunnelId = SettingsCrm.DefaultVkSalesFunnelId,
                createLeadFromMessages = SettingsVk.CreateLeadFromMessages,
                createLeadFromComments = SettingsVk.CreateLeadFromComments,
                syncOrdersFromVk = SettingsVk.SyncOrdersFromVk,
                groupMessageErrorStatus = SettingsVk.GroupMessageErrorStatus,
                codeChallenge,
                state
            });
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult GetUserAccessToken(ExchangeCode exchangeCode)
        {
            var result = new VkApiService().GetAccessToken(exchangeCode);
            
            if (!result.IsSuccess)
            {
                SettingsVk.UserTokenData = null;
                
                return JsonError(result.Error.Message);
            }
            
            var exchangeCodeResponse = result.Value;
                
            SettingsVk.UserTokenData = new VkTokenData(exchangeCodeResponse, exchangeCode.device_id, exchangeCode.client_id);
            
            SettingsVk.ApplicationId = exchangeCode.client_id;
            SettingsVk.UserId = exchangeCodeResponse.user_id;
            
            Track.TrackService.TrackEvent(Track.ETrackEvent.SalesChannels_ConnectAttempt_Vk);

            return JsonOk();
        }

        public JsonResult GetVkGroups()
        {
            return Json(_vkService.GetUserGroups());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveAuthVkGroup(VkGroup group, string accessToken)
        {
            if (!VkApiService.IsVkActive() && Core.Services.Crm.SocialNetworkService.IntegrationsLimitRiched())
                return JsonError(T("Admin.SettingsCrm.SocialNetworks.LimitRiched"));
        
            if (group == null 
                || group.Id == 0 
                || string.IsNullOrWhiteSpace(group.Name) 
                || string.IsNullOrWhiteSpace(accessToken))
            {
                return JsonError();
            }
        
            SettingsVk.Group = group;
            SettingsVk.TokenGroup = accessToken;
            SettingsVk.TokenGroupErrorCount = 0;
        
            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Vk_GroupConnected);
        
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteGroup()
        {
            SettingsVk.Group = null;
            SettingsVk.TokenGroup = null;
            SettingsVk.UserTokenData = null;
            SettingsVk.TokenUserErrorCount = 0;
            SettingsVk.TokenGroupErrorCount = 0;
            SettingsVk.LastMessageId = null;
            SettingsVk.LastSendedMessageId = null;
            SettingsVk.IsMessagesLoaded = false;

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveSettings(SaveVkSettingsModel model)
        {
            SettingsCrm.DefaultVkSalesFunnelId = model.Id;
            SettingsVk.CreateLeadFromMessages = model.CreateLeadFromMessages;
            SettingsVk.CreateLeadFromComments = model.CreateLeadFromComments;
            SettingsVk.SyncOrdersFromVk = model.SyncOrdersFromVk;
            
            return JsonOk();
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeGroupMessageErrorStatus()
        {
            SettingsVk.GroupMessageErrorStatus = "";
            JobActivationManager.SettingUpdated();
            
            return JsonOk();
        }

        #endregion

        [HttpGet]
        public JsonResult GetCustomerMessages(Guid customerId)
        {
            if (!VkApiService.IsVkActive())
                return Json(null);

            return Json(VkService.GetCustomerMessages(customerId));
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendVkMessage(long userId, string message)
        {
            return ProcessJsonResult(() => _vkService.SendMessageByGroup(userId, message.Replace("\r\n", "<br>")));
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendVkMessageByCustomers(SocialSendMessageModel model)
        {
            var sendedCount = new SendVkMessage(model).Execute();
            return JsonOk(sendedCount);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendVkMessageToWall(int id, string message)
        {
            return ProcessJsonResult(() => _vkService.SendMessageToWall(id, message.Replace("\r\n", "<br>")));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddVkUser(Guid customerId,  string link)
        {
            try
            {
                var result = _vkService.AddVkUserByLink(customerId, link);
                return result ? JsonOk() : JsonError();
            }
            catch (BlException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return JsonError();
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteVkLink(Guid customerId)
        {
            _vkService.DeleteVkUser(customerId);
            return JsonOk();
        }
    }
}
