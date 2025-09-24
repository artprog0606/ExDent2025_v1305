using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.SalesFunnels;

namespace AdvantShop.Core.Services.Configuration.Settings
{
    public class SettingsCrm
    {
        public static bool CrmActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["CrmActive"]);
            set => SettingProvider.Items["CrmActive"] = value.ToString();
        }
        
        /// <summary>
        /// Статус зазказа, если заказ создается из лида
        /// </summary>
        public static int OrderStatusIdFromLead
        {
            get => SettingProvider.Items["OrderStatusIdFromLead"].TryParseInt();
            set => SettingProvider.Items["OrderStatusIdFromLead"] = value.ToString();
        }

        /// <summary>
        /// Id воронки по умолчанию
        /// </summary>
        public static int DefaultSalesFunnelId
        {
            get
            {
                var id = Convert.ToInt32(SettingProvider.Items["DefaultSalesFunnelId"]);

                var funnels = SalesFunnelService.GetList();
                if (funnels.Find(x => x.Id == id) == null)
                {
                    var funnel = funnels.FirstOrDefault();
                    if (funnel != null)
                        DefaultSalesFunnelId = id = funnel.Id;
                }
                return id;
            }
            set => SettingProvider.Items["DefaultSalesFunnelId"] = value.ToString();
        }

        public static int DefaultFinalDealStatusId
        {
            get
            {
                var status = DealStatusService.GetList(DefaultSalesFunnelId).FirstOrDefault(x => x.Status == SalesFunnelStatusType.FinalSuccess);
                return status != null ? status.Id : 0;
            }
        }


        public static int DefaultVkSalesFunnelId
        {
            get
            {
                var id = Convert.ToInt32(SettingProvider.Items["DefaultVkSalesFunnelId"]);

                var funnels = SalesFunnelService.GetList();
                if (funnels.Find(x => x.Id == id) == null)
                {
                    var funnel = funnels.FirstOrDefault();
                    if (funnel != null)
                        DefaultVkSalesFunnelId = id = funnel.Id;
                }
                return id;
            }
            set => SettingProvider.Items["DefaultVkSalesFunnelId"] = value.ToString();
        }

        public static int VkFinalDealStatusId
        {
            get
            {
                var status = DealStatusService.GetList(DefaultVkSalesFunnelId).FirstOrDefault(x => x.Status == SalesFunnelStatusType.FinalSuccess);
                return status != null ? status.Id : 0;
            }
        }

        public static int DefaultInstagramSalesFunnelId
        {
            get
            {
                var id = Convert.ToInt32(SettingProvider.Items["DefaultInstagramSalesFunnelId"]);

                var funnels = SalesFunnelService.GetList();
                if (funnels.Find(x => x.Id == id) == null)
                {
                    var funnel = funnels.FirstOrDefault();
                    if (funnel != null)
                        DefaultInstagramSalesFunnelId = id = funnel.Id;
                }
                return id;
            }
            set => SettingProvider.Items["DefaultInstagramSalesFunnelId"] = value.ToString();
        }

        public static List<int> InstagramFinalDealStatusIds
        {
            get
            {
                return DealStatusService.GetList(DefaultInstagramSalesFunnelId)
                    .Where(x => x.Status == SalesFunnelStatusType.FinalSuccess || x.Status == SalesFunnelStatusType.Canceled)
                    .Select(x => x.Id)
                    .ToList();
            }
        }

        public static int DefaultFacebookSalesFunnelId
        {
            get
            {
                var id = Convert.ToInt32(SettingProvider.Items["DefaultFacebookSalesFunnelId"]);

                var funnels = SalesFunnelService.GetList();
                if (funnels.Find(x => x.Id == id) == null)
                {
                    var funnel = funnels.FirstOrDefault();
                    if (funnel != null)
                        DefaultFacebookSalesFunnelId = id = funnel.Id;
                }
                return id;
            }
            set => SettingProvider.Items["DefaultFacebookSalesFunnelId"] = value.ToString();
        }

        public static int FacebookFinalDealStatusId
        {
            get
            {
                var status = DealStatusService.GetList(DefaultFacebookSalesFunnelId).FirstOrDefault(x => x.Status == SalesFunnelStatusType.FinalSuccess);
                return status != null ? status.Id : 0;
            }
        }


        public static int DefaultBuyInOneClickSalesFunnelId
        {
            get
            {
                var id = Convert.ToInt32(SettingProvider.Items["DefaultBuyInOneClickSalesFunnelId"]);

                var funnels = SalesFunnelService.GetList();
                if (funnels.Find(x => x.Id == id) == null)
                {
                    var funnel = funnels.FirstOrDefault();
                    if (funnel != null)
                        DefaultBuyInOneClickSalesFunnelId = id = funnel.Id;
                }
                return id;
            }
            set => SettingProvider.Items["DefaultBuyInOneClickSalesFunnelId"] = value.ToString();
        }


        public static int DefaultTelegramSalesFunnelId
        {
            get
            {
                var id = Convert.ToInt32(SettingProvider.Items["DefaultTelegramSalesFunnelId"]);

                var funnels = SalesFunnelService.GetList();
                if (funnels.Find(x => x.Id == id) == null)
                {
                    var funnel = funnels.FirstOrDefault();
                    if (funnel != null)
                        DefaultTelegramSalesFunnelId = id = funnel.Id;
                }
                return id;
            }
            set => SettingProvider.Items["DefaultTelegramSalesFunnelId"] = value.ToString();
        }

        public static int TelegramFinalDealStatusId
        {
            get
            {
                var status = DealStatusService.GetList(DefaultTelegramSalesFunnelId).FirstOrDefault(x => x.Status == SalesFunnelStatusType.FinalSuccess);
                return status != null ? status.Id : 0;
            }
        }

        public static bool CreateLeadFromCall
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsCrm.CreateLeadFromCall"]);
            set => SettingProvider.Items["SettingsCrm.CreateLeadFromCall"] = value.ToString();
        }

        public static int DefaultCallsSalesFunnelId
        {
            get
            {
                var id = Convert.ToInt32(SettingProvider.Items["DefaultCallsSalesFunnelId"]);
                // если не задана - воронка по умолчанию
                return id != 0 ? id : DefaultSalesFunnelId;
            }
            set => SettingProvider.Items["DefaultCallsSalesFunnelId"] = value.ToString();
        }

        public static int DefaultOkSalesFunnelId
        {
            get
            {
                var id = Convert.ToInt32(SettingProvider.Items["DefaultOkSalesFunnelId"]);
                return id != 0 ? id : DefaultSalesFunnelId;
            }
            set => SettingProvider.Items["DefaultOkSalesFunnelId"] = value.ToString();
        }
    }
}
