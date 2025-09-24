using AdvantShop.Areas.Api.Models.Leads;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Infrastructure.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Areas.Api.Handlers.Leads
{
    public class AddLead : AbstractCommandHandler<AddLeadResponse>
    {
        private readonly AddLeadModel _model;

        public AddLead(AddLeadModel model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_model.FirstName) && string.IsNullOrWhiteSpace(_model.Email) && string.IsNullOrWhiteSpace(_model.Phone))
            {
                throw new BlException("Заполните обязательное поле (имя, email или телефон)");
            }
        }

        private AddLeadResponse Add()
        {
            OrderSource orderSource = null;
            if (!string.IsNullOrWhiteSpace(_model.Source))
            {
                orderSource = OrderSourceService.GetOrderSource(_model.Source);
                if (orderSource == null)
                {
                    var orderSourceId = OrderSourceService.AddOrderSource(new OrderSource
                    {
                        Name = _model.Source,
                        SortOrder = 0,
                        Type = OrderType.None
                    });
                    orderSource = OrderSourceService.GetOrderSource(orderSourceId);
                }
            }

            if (orderSource == null)
                orderSource = OrderSourceService.GetOrderSource(OrderType.None);

            var lead = new Lead()
            {
                FirstName = _model.FirstName.EncodeOrEmpty(),
                LastName = _model.LastName.EncodeOrEmpty(),
                Patronymic = _model.Patronymic.EncodeOrEmpty(),
                Phone = _model.Phone.EncodeOrEmpty(),
                Email = _model.Email.EncodeOrEmpty(),
                Title = _model.Title.EncodeOrEmpty(),
                Description = _model.Description.EncodeOrEmpty(),
                Sum = _model.Sum,
                OrderSourceId = orderSource != null ? orderSource.Id : 0,
                DiscountValue = _model.DiscountValue,
                Discount = _model.DiscountValue != 0 ? 0 : _model.DiscountPercent,
                LeadCurrency = CurrencyService.CurrentCurrency,
                Comment = _model.Comment.EncodeOrEmpty()
            };

            if (_model.FunnelId > 0)
            {
                var salesFunnel = SalesFunnelService.Get(_model.FunnelId);
                if (salesFunnel != null)
                    lead.SalesFunnelId = salesFunnel.Id;
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
                    lead.Customer.Phone = lead.Phone;
                    lead.Customer.StandardPhone = StringHelper.ConvertToStandardPhone(lead.Phone);

                    if (string.IsNullOrWhiteSpace(lead.Customer.EMail))
                        lead.Customer.EMail = lead.Email;
                }
            }
            else
            {
                lead.Customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                {
                    FirstName = lead.FirstName,
                    LastName = lead.LastName,
                    Patronymic = lead.Patronymic,
                    Phone = lead.Phone,
                    StandardPhone = StringHelper.ConvertToStandardPhone(lead.Phone),
                    EMail = lead.Email,
                    CustomerRole = Role.User
                };
            }

            if (_model.Products != null)
            {
                var items = new List<LeadItem>();

                foreach (var productModel in _model.Products)
                {
                    var offer = OfferService.GetOffer(productModel.ArtNo);
                    if (offer == null)
                    {
                        var p = ProductService.GetProduct(productModel.ArtNo);
                        if (p != null && p.Offers.Count == 1)
                            offer = p.Offers[0];
                    }

                    if (offer != null)
                        items.Add(new LeadItem(offer, productModel.Amount));
                    else
                    {
                        items.Add(new LeadItem()
                        {
                            Name = productModel.Name,
                            ArtNo = productModel.ArtNo ?? "",
                            Amount = productModel.Amount,
                            Price = productModel.Price
                        });
                    }
                }

                lead.LeadItems = items;
                lead.Sum = lead.LeadItems.Sum(x => x.Price * x.Amount) - lead.GetTotalDiscount(lead.LeadCurrency);
            }

            LeadService.AddLead(lead, true);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_LeadCreated_Api);

            return new AddLeadResponse() { leadId = lead.Id };
        }

        protected override AddLeadResponse Handle()
        {
            try
            {
                return Add();
            }
            catch (BlException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message, ex);
                throw;
            }
        }
    }
}