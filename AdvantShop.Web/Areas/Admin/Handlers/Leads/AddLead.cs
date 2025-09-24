using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Web.Admin.Models.Crm.Leads;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class AddLead
    {
        private readonly AddLeadModel _model;

        public AddLead(AddLeadModel model)
        {
            _model = model;
        }

        public int Execute()
        {
            try
            {
                var orderSource = !_model.CallId.HasValue 
                    ? OrderSourceService.GetOrderSource(OrderType.None)
                    : IPTelephonyService.GetOrderSource(_model.CallId.Value) ?? OrderSourceService.GetOrderSource(OrderType.Phone);

                var lead = new Lead()
                {
                    FirstName = _model.FirstName.DefaultOrEmpty(),
                    LastName = _model.LastName.DefaultOrEmpty(),
                    Patronymic = _model.Patronymic.DefaultOrEmpty(),
                    Organization = _model.Organization.DefaultOrEmpty(),
                    Phone = _model.Phone.DefaultOrEmpty(),
                    Email = _model.Email.DefaultOrEmpty(),
                    Title = _model.Title.DefaultOrEmpty(),
                    Description = _model.Description.DefaultOrEmpty(),
                    Sum = _model.Sum,
                    ManagerId = _model.ManagerId,
                    OrderSourceId = orderSource.Id,
                    IsFromAdminArea = true,
                    CustomerType = _model.CustomerType
                };

                var salesFunnel = SalesFunnelService.Get(_model.SalesFunnelId);
                lead.SalesFunnelId = salesFunnel != null ? salesFunnel.Id : SettingsCrm.DefaultSalesFunnelId;

                var status = DealStatusService.Get(_model.DealStatusId);
                var statuses = DealStatusService.GetList(lead.SalesFunnelId);

                if (status == null || !statuses.Any(x => x.Id == _model.DealStatusId))
                {
                    if (statuses.Count > 0)
                        lead.DealStatusId = statuses[0].Id;
                }
                else
                {
                    lead.DealStatusId = status.Id;
                }

                if (_model.CustomerId != null && _model.CustomerId != Guid.Empty)
                {
                    lead.CustomerId = _model.CustomerId.Value;

                    var customer = CustomerService.GetCustomer(_model.CustomerId.Value);
                    if (customer != null)
                    {
                        lead.Customer.FirstName = lead.FirstName;
                        lead.Customer.LastName = lead.LastName;
                        lead.Customer.Patronymic = lead.Patronymic;
                        lead.Customer.Organization = lead.Organization;
                        lead.Customer.Phone = lead.Phone;
                        lead.Customer.StandardPhone = StringHelper.ConvertToStandardPhone(lead.Phone, true, true);
                        lead.Customer.CustomerType = lead.CustomerType;

                        if (string.IsNullOrWhiteSpace(lead.Customer.EMail))
                            lead.Customer.EMail = lead.Email;

                        var contacts = CustomerService.GetCustomerContacts(_model.CustomerId.Value);
                        if (contacts.Count > 0)
                        {
                            var contactsItem = contacts.FirstOrDefault(x => x.IsMain);
                            if (contactsItem == null)
                            {
                                contactsItem = contacts[0];
                            }
                            lead.City = contactsItem.City;
                            lead.Country = contactsItem.Country;
                            lead.District = contactsItem.District;
                            lead.Region = contactsItem.Region;
                            lead.Zip = contactsItem.Zip;
                        }
                    }
                    else
                    {
                        lead.Customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                        {
                            Id = lead.CustomerId.Value,
                            FirstName = lead.FirstName,
                            LastName = lead.LastName,
                            Patronymic = lead.Patronymic,
                            Organization = lead.Organization,
                            Phone = lead.Phone,
                            StandardPhone = StringHelper.ConvertToStandardPhone(lead.Phone, true, true),
                            EMail = lead.Email,
                            CustomerRole = Role.User,
                            CustomerType = lead.CustomerType
                        };
                    }
                }
                else
                {
                    lead.Customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                    {
                        FirstName = lead.FirstName,
                        LastName = lead.LastName,
                        Patronymic = lead.Patronymic,
                        Organization = lead.Organization,
                        Phone = lead.Phone,
                        StandardPhone = StringHelper.ConvertToStandardPhone(lead.Phone, true, true),
                        EMail = lead.Email,
                        CustomerRole = Role.User,
                        CustomerType = lead.CustomerType
                    };
                }

                if (_model.Products != null)
                {
                    var items = new List<LeadItem>();
                    
                    foreach (var productModel in _model.Products)
                    {
                        var offer = OfferService.GetOffer(productModel.OfferId);
                        if (offer == null)
                            continue;
                        
                        items.Add(new LeadItem(offer, productModel.Amount)
                        {
                            Price = PriceService.GetFinalPrice(offer, lead.Customer.CustomerGroup)
                        });
                    }

                    lead.LeadItems = items;
                }

                LeadService.AddLead(lead, true, changedBy: null, trackChanges: true, customerFields: _model.CustomerFields, leadFields: _model.LeadFields);
                
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_LeadCreated_AdminArea);

                return lead.Id;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return 0;
        }
    }
}

