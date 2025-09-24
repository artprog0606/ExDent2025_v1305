using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Repository;
using AdvantShop.Web.Admin.Models.Crm.Leads;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class SaveLead
    {
        private readonly LeadModel _model;

        public SaveLead(LeadModel model)
        {
            _model = model;
        }

        public bool Execute()
        {
            var lead = LeadService.GetLead(_model.Id);

            if (lead == null || _model.Lead == null || LeadService.CheckAccess(lead) == false)
                return false;

            try
            {
                lead.Title = _model.Lead.Title;
                lead.Description = _model.Lead.Description;
                lead.Sum = _model.Lead.Sum;
                lead.FirstName = _model.Lead.FirstName.EncodeOrEmpty();
                lead.LastName = _model.Lead.LastName.EncodeOrEmpty();
                lead.Patronymic = _model.Lead.Patronymic.EncodeOrEmpty();
                lead.Organization = _model.Lead.Organization.EncodeOrEmpty();
                lead.Phone = _model.Lead.Phone.EncodeOrEmpty();
                lead.Email = _model.Lead.Email.EncodeOrEmpty();
                lead.CustomerType = _model.Lead.CustomerType;

                lead.CustomerId = _model.Lead.CustomerId;

                if (_model.Lead.CustomerId == null || _model.Lead.Customer == null)
                {
                    var phone = _model.Lead.Phone.EncodeOrEmpty();

                    if (lead.Customer == null)
                        lead.Customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                        {
                            FirstName = _model.Lead.FirstName.EncodeOrEmpty(),
                            LastName = _model.Lead.LastName.EncodeOrEmpty(),
                            Patronymic = _model.Lead.Patronymic.EncodeOrEmpty(),
                            Organization = _model.Lead.Organization.EncodeOrEmpty(),
                            Phone = phone,
                            StandardPhone = !string.IsNullOrEmpty(phone) ? StringHelper.ConvertToStandardPhone(phone, true, true) : null,
                            EMail = _model.Lead.Email.EncodeOrEmpty(),
                            CustomerType = _model.Lead.CustomerType,
                        };
                }
                else if (_model.Lead.Customer != null)
                {
                    if (lead.Customer == null)
                        lead.Customer = new Customer(CustomerGroupService.DefaultCustomerGroup);

                    var phone = _model.Lead.Customer.Phone.EncodeOrEmpty();

                    lead.CustomerId = _model.Lead.CustomerId;
                    lead.Customer.FirstName = _model.Lead.Customer.FirstName.EncodeOrEmpty();
                    lead.Customer.LastName = _model.Lead.Customer.LastName.EncodeOrEmpty();
                    lead.Customer.Patronymic = _model.Lead.Customer.Patronymic.EncodeOrEmpty();
                    lead.Customer.Organization = _model.Lead.Customer.Organization.EncodeOrEmpty();
                    lead.Customer.Phone = phone;
                    lead.Customer.StandardPhone = !string.IsNullOrEmpty(phone) ? StringHelper.ConvertToStandardPhone(phone, true, true) : null;
                    lead.Customer.EMail = _model.Lead.Customer.EMail.EncodeOrEmpty();
                    lead.Customer.CustomerType = _model.Lead.Customer.CustomerType;
                }


                if (lead.Customer.Contacts == null || lead.Customer.Contacts.Count == 0)
                {
                    var country = CountryService.GetCountry(SettingsMain.SellerCountryId);

                    lead.Customer.Contacts = new List<CustomerContact>()
                    {
                        new CustomerContact()
                        {
                            CustomerGuid = lead.Customer.Id,
                            Country = country != null ? country.Name : "",
                            City = SettingsMain.City
                        }
                    };
                }

                if (_model.Lead.Customer != null && _model.Lead.Customer.Contacts != null && _model.Lead.Customer.Contacts.Count > 0)
                {
                    lead.Customer.Contacts[0].CustomerGuid = lead.Customer.Id;
                    lead.Customer.Contacts[0].City = _model.Lead.Customer.Contacts[0].City;
                }

                lead.ManagerId = _model.Lead.ManagerId;
                lead.OrderSourceId = _model.Lead.OrderSourceId;

                var funnel = SalesFunnelService.Get(_model.Lead.SalesFunnelId);
                if (funnel != null)
                {
                    lead.SalesFunnelId = funnel.Id;
                }

                var statusChanged = lead.DealStatusId != _model.Lead.DealStatusId;
                var statuses = DealStatusService.GetList(lead.SalesFunnelId);
                var status = statuses.FirstOrDefault(x => x.Id == _model.Lead.DealStatusId);

                if (status != null)
                    lead.DealStatusId = status.Id;
                else if (statuses.Count > 0)
                    lead.DealStatusId = statuses[0].Id;

                LeadService.UpdateLead(lead, leadFields: _model.LeadFieldsJs != null ? _model.LeadFieldsJs.Select(x => x.Value).ToList() : null);

                if (_model.CustomerFieldsJs != null)
                {
                    foreach (var customerField in _model.CustomerFieldsJs)
                    {
                        LeadsHistoryService.TrackLeadCustomerFieldChanges(lead.Id, lead.Customer.Id, customerField.Value.Id, customerField.Value.Value, null);

                        CustomerFieldService.AddUpdateMap(lead.Customer.Id, customerField.Value.Id, customerField.Value.Value ?? "");
                    }
                }
                else if (_model.CustomerFields != null && lead.Customer != null)
                {
                    foreach (var customerField in _model.CustomerFields)
                    {
                        LeadsHistoryService.TrackLeadCustomerFieldChanges(lead.Id, lead.Customer.Id, customerField.Id, customerField.Value, null);

                        CustomerFieldService.AddUpdateMap(lead.Customer.Id, customerField.Id, customerField.Value ?? "");
                    }
                }

                TrackEvents(statusChanged);

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return false;
        }

        private void TrackEvents(bool statusChanged)
        {
            if (statusChanged)
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_DealStatusChanged);
            else
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_EditLead);
        }
    }
}
