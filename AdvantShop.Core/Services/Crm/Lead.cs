using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;

namespace AdvantShop.Core.Services.Crm
{
    public class Lead : IBizObject, ITriggerObject, ICloneable
    {
        public int Id { get; set; }


        [Compare("Core.Crm.Lead.Title")]
        public string Title { get; set; }

        [Compare("Core.Crm.Lead.Description")]
        public string Description { get; set; }

        [Compare("Core.Crm.Lead.Sum")]
        public float Sum { get; set; }

        [Compare("Core.Customers.Customer.FirstName")]
        public string FirstName { get; set; }

        [Compare("Core.Customers.Customer.LastName")]
        public string LastName { get; set; }

        [Compare("Core.Customers.Customer.Patronymic")]
        public string Patronymic { get; set; }

        [Compare("Core.Customers.Customer.Organization")]
        public string Organization { get; set; }

        [Compare("Core.Customers.Customer.Phone")]
        public string Phone { get; set; }

        [Compare("Core.Customers.Customer.Email")]
        public string Email { get; set; }

        [Compare("Core.Customers.Customer.CustomerType")]
        public CustomerType CustomerType { get; set; }

        public Guid? CustomerId { get; set; }

        private Customer _customer = null;
        
        public Customer Customer
        {
            get
            {
                if (_customer != null)
                    return _customer;

                _customer = CustomerId != null ? CustomerService.GetCustomer(CustomerId.Value) : null;

                return _customer;
            }
            set => _customer = value;
        }

        [Obsolete]
        public LeadStatus LeadStatus { get; set; }

        private DealStatus _dealStatus;

        [Compare("Core.Crm.Lead.DealStatus")]
        public DealStatus DealStatus => _dealStatus ?? (_dealStatus = DealStatusService.Get(DealStatusId));

        public int DealStatusId { get; set; }

        [Compare("Core.Crm.Lead.SalesFunnel", ChangeHistoryParameterType.SalesFunnel)]
        public int SalesFunnelId { get; set; }

        [Compare("Core.Crm.Lead.Comment")]
        public string Comment { get; set; }
        
        [Compare("Core.Crm.Lead.Manager", ChangeHistoryParameterType.Manager)]
        public int? ManagerId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        [Compare("Core.Crm.Lead.Discount")]
        public float Discount { get; set; }

        [Compare("Core.Crm.Lead.DiscountValue")]
        public float DiscountValue { get; set; }
        
        [Compare("Core.Crm.Lead.OrderSource", ChangeHistoryParameterType.OrderSource)]
        public int OrderSourceId { get; set; }

        public bool IsFromAdminArea { get; set; }

        private List<LeadItem> _leadItems;
        public List<LeadItem> LeadItems
        {
            get =>
                Id != 0 || _leadItems != null
                    ? _leadItems ?? (_leadItems = LeadService.GetLeadItems(Id))
                    : (_leadItems = new List<LeadItem>());
            set => _leadItems = value;
        }

        private LeadCurrency _leadCurrency;
        public LeadCurrency LeadCurrency
        {
            get =>
                Id != 0 || _leadCurrency != null
                    ? _leadCurrency ?? (_leadCurrency = LeadService.GetLeadCurrency(Id))
                    : _leadCurrency;
            set => _leadCurrency = value;
        }

        [Compare("Core.Crm.Lead.DeliveryDate")]
        public DateTime? DeliveryDate { get; set; }

        private string _deliveryTime;
        [Compare("Core.Crm.Lead.DeliveryTime")]
        public string DeliveryTime
        {
            get => _deliveryTime ?? (_deliveryTime = DeliveryInterval.ToString());
            set => DeliveryInterval = new DeliveryInterval(_deliveryTime = value);
        }
        private DeliveryInterval _deliveryInterval;
        public DeliveryInterval DeliveryInterval
        {
            get => _deliveryInterval ?? (_deliveryInterval = new DeliveryInterval(_deliveryTime));
            set => _deliveryInterval = value;
        }

        public int ShippingMethodId { get; set; }

        [Compare("Core.Crm.Lead.ShippingName")]
        public string ShippingName { get; set; }

        [Compare("Core.Crm.Lead.ShippingCost")]
        public float ShippingCost { get; set; }

        //[Compare("Core.Crm.Lead.ShippingPickPoint")]
        public string ShippingPickPoint { get; set; }
        
        public TaxType ShippingTaxType { get; set; }
        
        public ePaymentMethodType ShippingPaymentMethodType { get; set; } = ePaymentMethodType.full_prepayment;
        public ePaymentSubjectType ShippingPaymentSubjectType { get; set; } = ePaymentSubjectType.payment;
        
        public bool AvailablePaymentCashOnDelivery { get; set; }
        public bool AvailablePaymentPickPoint { get; set; }

        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }

        private List<LeadAttachment> _attachments;
        public List<LeadAttachment> Attachments => _attachments ?? (_attachments = AttachmentService.GetAttachments<LeadAttachment>(Id));


        public TriggerProcessObject GetTriggerProcessObject()
        {
            if (Customer == null)
                return new TriggerProcessObject()
                {
                    EntityId = Id,
                    Email = Email,
                    Phone = StringHelper.ConvertToStandardPhone(Phone) ?? 0,
                    EventObjId = DealStatusId,
                    CustomerId = CustomerId ?? Guid.Empty
                };

            return new TriggerProcessObject()
            {
                EntityId = Id,
                Email = Customer.EMail,
                Phone = Customer.StandardPhone ?? StringHelper.ConvertToStandardPhone(Customer.Phone) ?? 0,
                EventObjId = DealStatusId,
                CustomerId = CustomerId ?? Guid.Empty
            };
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Copy with lazy fields
        /// </summary>
        /// <returns></returns>
        public Lead DeepClone()
        {
            var lead = (Lead)Clone();
            var customer = lead.Customer;
            var dealStatus = lead.DealStatus;
            var items = lead.LeadItems;
            var currency = lead.LeadCurrency;
            var attachments = lead.Attachments;
            return lead;
        }
    }

    public enum LeadStatus
    {
        [Localize("Core.Crm.LeadStatus.New")]
        [Color("#ff7f00")]
        New = 1,

        [Localize("Core.Crm.LeadStatus.Processing")]
        [Color("#77ff77")]
        Processing = 2,

        [Localize("Core.Crm.LeadStatus.ClosedDeal")]
        [Color("#ff9bb9")]
        ClosedDeal = 3,

        [Localize("Core.Crm.LeadStatus.NotClosedDeal")]
        [Color("#58bae9")]
        NotClosedDeal = 4
    }

    public class LeadCurrency
    {
        public string CurrencyCode { get; set; }
        public int CurrencyNumCode { get; set; }
        public float CurrencyValue { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsCodeBefore { get; set; }
        public float RoundNumbers { get; set; }
        public bool EnablePriceRounding { get; set; }

        public static implicit operator LeadCurrency(Currency cur)
        {
            return new LeadCurrency
            {
                CurrencyCode = cur.Iso3,
                CurrencyNumCode = cur.NumIso3,
                CurrencyValue = cur.Rate,
                CurrencySymbol = cur.Symbol,
                IsCodeBefore = cur.IsCodeBefore,
                EnablePriceRounding = cur.EnablePriceRounding,
                RoundNumbers = cur.RoundNumbers
            };
        }

        public static implicit operator Currency(LeadCurrency cur)
        {
            return new Currency
            {
                Iso3 = cur.CurrencyCode,
                NumIso3 = cur.CurrencyNumCode,
                Rate = cur.CurrencyValue,
                Symbol = cur.CurrencySymbol,
                IsCodeBefore = cur.IsCodeBefore,
                EnablePriceRounding = cur.EnablePriceRounding,
                RoundNumbers = cur.RoundNumbers
            };
        }
    }

    public static class LeadExtensions
    {
        public static float GetTotalDiscount(this Lead lead, Currency leadCurrency)
        {
            var totalDiscount = lead.Discount > 0
                ? (lead.Discount * lead.LeadItems.Sum(x => x.Price * x.Amount) / 100).RoundPrice(leadCurrency.Rate, leadCurrency)
                : 0;

            totalDiscount += lead.DiscountValue;

            return totalDiscount;
        }
    }
}
