using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.Ok;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Crm.Telegram;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Loging.TrafficSource;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Crm.Leads;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class GetLead
    {
        private readonly int _id;

        public GetLead(int id)
        {
            _id = id;
        }

        public LeadModel Execute()
        {
            var lead = LeadService.GetLead(_id);

            var model = new LeadModel() {Id = _id, Lead = lead};

            if (lead == null)
                return model;

            if (!LeadService.CheckAccess(lead))
                return null;

            model.SalesFunnel = SalesFunnelService.Get(lead.SalesFunnelId);

            if (model.SalesFunnel != null && !SalesFunnelService.CheckAccess(model.SalesFunnel))
                return null;

            var statuses = DealStatusService.GetList(lead.SalesFunnelId);

            model.Statuses = new List<SelectItemModel>() {new SelectItemModel("-", "0")};
            model.Statuses.AddRange(statuses.Select(x => new SelectItemModel(x.Name, x.Id)));

            var finalStatus = statuses.FirstOrDefault(x => x.Status == SalesFunnelStatusType.FinalSuccess);
            model.FinalStatusId = finalStatus != null ? finalStatus.Id : 0;
            var canceledStatus = statuses.FirstOrDefault(x => x.Status == SalesFunnelStatusType.Canceled);
            model.CanceledStatusId = canceledStatus != null ? canceledStatus.Id : 0;
            
            if (model.Statuses.Find(x => x.value == lead.DealStatusId.ToString()) == null)
                lead.DealStatusId = 0;

            model.TrafficSource = OrderTrafficSourceService.Get(lead.Id, TrafficSourceType.Lead);

            if (lead.LeadItems != null)
            {
                var leadCurrency = lead.LeadCurrency != null
                    ? (Currency)lead.LeadCurrency
                    : CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3);

                var itemsSum = lead.LeadItems.Sum(x => x.Price * x.Amount);
                var totalDiscount = lead.GetTotalDiscount(leadCurrency);

                //todo refactor it because change data on get it's not clean
                var sum = itemsSum - totalDiscount + lead.ShippingCost;
                if (lead.Sum != sum && itemsSum > 0)
                {
                    lead.Sum = sum;
                    LeadService.UpdateLead(lead, false);
                }
            }

            if (lead.CustomerId != null)
            {
                model.VkUser = VkService.GetUser(lead.CustomerId.Value);
                model.InstagramUser = InstagramService.GetUserByCustomerId(lead.CustomerId.Value);
                model.FacebookUser = FacebookService.GetUser(lead.CustomerId.Value);
                model.TelegramUser = new TelegramService().GetUser(lead.CustomerId.Value);
                model.OkUser = OkService.GetUser(lead.CustomerId.Value);

                AdminInformerService.SetSeen(lead.CustomerId.Value);
            }

            var orderId = OrderService.GetOrderIdByLeadId(lead.Id);
            var order = orderId != 0 ? OrderService.GetOrder(orderId) : null;
            if (order != null)
                model.Order = order;

            return model;
        }
    }
}
