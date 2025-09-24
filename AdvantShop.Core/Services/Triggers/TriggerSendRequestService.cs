using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Triggers
{
    public static class TriggerSendRequestService
    {
        public static bool Send(TriggerRule trigger, TriggerAction action, ITriggerObject triggerObject)
        {
            var data = action.SendRequestData;

            if (data == null)
                return false;

            if (string.IsNullOrWhiteSpace(data.RequestUrl))
                return false;

            try
            {
                var url = data.RequestUrl.Trim();

                if (!url.StartsWith("http"))
                    url = "http://" + url;

                var method = data.RequestMethod == TriggerActionSendRequestMethod.Get
                    ? ERequestMethod.GET
                    : ERequestMethod.POST;

                var headers = data.RequestHeaderParams != null
                    ? data.RequestHeaderParams
                        .Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value))
                        .ToDictionary(x => x.Key, x => x.Value)
                    : null;

                var parameters = new Dictionary<string, object>();
                var parametersJson = "";

                switch (trigger.ObjectType)
                {
                    case ETriggerObjectType.Lead:
                        var lead = triggerObject as Lead;

                        if (data.RequestParamsType == TriggerActionSendRequestParamsType.Parameters)
                        {
                            if (data.RequestParams != null)
                                foreach (var param in data.RequestParams)
                                    if (!parameters.ContainsKey(param.Key))
                                        parameters.Add(param.Key, ReplaceParamsForLead(param.Value, lead));
                        }
                        else if (!string.IsNullOrEmpty(data.RequestParamsJson))
                        {
                            parametersJson = (string) ReplaceParamsForLead(data.RequestParamsJson, lead, true);
                        }

                        url = (string) ReplaceParamsForLead(url, lead);
                        break;

                    case ETriggerObjectType.Order:
                        var order = triggerObject as Order;

                        if (data.RequestParamsType == TriggerActionSendRequestParamsType.Parameters)
                        {
                            if (data.RequestParams != null)
                                foreach (var param in data.RequestParams)
                                    if (!parameters.ContainsKey(param.Key))
                                        parameters.Add(param.Key, ReplaceParamsForOrder(param.Value, order));
                        }
                        else if (!string.IsNullOrEmpty(data.RequestParamsJson))
                        {
                            parametersJson = (string) ReplaceParamsForOrder(data.RequestParamsJson, order, true);
                        }

                        url = (string) ReplaceParamsForOrder(url, order);
                        break;

                    case ETriggerObjectType.Customer:
                        var customer = triggerObject as Customer;

                        if (data.RequestParamsType == TriggerActionSendRequestParamsType.Parameters)
                        {
                            if (data.RequestParams != null)
                                foreach (var param in data.RequestParams)
                                    if (!parameters.ContainsKey(param.Key))
                                        parameters.Add(param.Key, ReplaceParamsForCustomer(param.Value, customer));
                        }
                        else if (!string.IsNullOrEmpty(data.RequestParamsJson))
                        {
                            parametersJson = (string) ReplaceParamsForCustomer(data.RequestParamsJson, customer, true);
                        }

                        url = (string) ReplaceParamsForCustomer(url, customer);
                        break;
                }

                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        var result =
                            RetryHelper.Do(
                                () => method == ERequestMethod.GET
                                    ? RequestHelper.MakeRequest<string>(url, method: method)
                                    : RequestHelper.MakeRequest<string>(url,
                                        data: data.RequestParamsType == TriggerActionSendRequestParamsType.Parameters
                                            ? (object) parameters
                                            : (object) JsonConvert.DeserializeObject(parametersJson),
                                        method: method,
                                        headers: headers),
                                TimeSpan.FromSeconds(60), 3);

                        Debug.Log.Info(
                            string.Format("Trigger {0} send request: {1} to {2} {3} Response: {4}",
                                trigger.Id, method, url,
                                data.RequestParamsType == TriggerActionSendRequestParamsType.Parameters
                                    ? parameters.Count > 0 ? JsonConvert.SerializeObject(parameters) : ""
                                    : parametersJson,
                                result));
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error($"Error on trigger {trigger.Id} send request: {method} to {url}. {ex.Message}", ex);
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return false;
        }

        private static object ReplaceParamsForLead(string value, Lead lead, bool isJson = false)
        {
            var customer = lead.Customer;

            var result =
                value.Replace("#LeadId#", lead.Id.ToString())
                    .Replace("#Description#", lead.Description.ReplaceQuote())
                    .Replace("#Sum#", lead.Sum.ToString("#.##"))
                    .Replace("#Status#", lead.DealStatus.ToString().ReplaceQuote())
                    .Replace("#ShippingMethod#", lead.ShippingName.ReplaceQuote())

                    .Replace("#ClientId#", customer != null ? lead.Customer.Id.ToString() : "")
                    .Replace("#ClientLastName#", customer != null ? lead.Customer.LastName.ReplaceQuote() : "")
                    .Replace("#ClientFirstName#", customer != null ? lead.Customer.FirstName.ReplaceQuote() : "")
                    .Replace("#ClientPatronymic#", customer != null ? lead.Customer.Patronymic.ReplaceQuote() : "")
                    .Replace("#ClientFullName#", customer != null ? lead.Customer.GetFullName().ReplaceQuote() : "")
                    .Replace("#ClientPhone#", customer != null ? lead.Customer.Phone.ReplaceQuote() : "")
                    .Replace("#ClientEmail#", customer != null ? lead.Customer.EMail.ReplaceQuote() : "")
                    .Replace("#ClientAddress#", customer != null ? (lead.Country + " " + lead.Region + " " + lead.District + " " + lead.City).Trim() : "");

            var additionalFields = customer != null ? GetAdditionalFieldsValue(lead.Customer.Id) : null;
            var items =
                lead.LeadItems != null && lead.LeadItems.Count > 0
                    ? lead.LeadItems.Select(x => new
                    {
                        Artno = x.ArtNo.ReplaceQuote(),
                        Name = x.Name.ReplaceQuote(),
                        Price = x.Price,
                        Amount = x.Amount,
                        Color = x.Color,
                        Size = x.Size,
                        Length = x.Length,
                        Width = x.Width,
                        Height = x.Height,
                        ProductId = x.ProductId
                    })
                    : null;

            if (value == "#AdditionalFields#")
                return additionalFields;

            if (value == "#Products#")
                return items;

            if (isJson)
            {
                result = result
                    .Replace("#AdditionalFields#", JsonConvert.SerializeObject(additionalFields))
                    .Replace("#Products#", JsonConvert.SerializeObject(items));
            }

            return result;
        }

        private static object ReplaceParamsForOrder(string value, Order order, bool isJson = false)
        {
            var customer = order.OrderCustomer;

            var result =
                value.Replace("#OrderId#", order.OrderID.ToString())
                    .Replace("#Number#", order.Number)
                    .Replace("#IsPaid#", order.Payed.ToLowerString())
                    .Replace("#Sum#", order.Sum.ToInvariantString("F2"))
                    .Replace("#Status#", order.OrderStatus.StatusName.ReplaceQuote())
                    .Replace("#ShippingMethod#",
                        (order.ArchivedShippingName +
                        (order.OrderPickPoint != null ? " " + order.OrderPickPoint.PickPointAddress : "")).ReplaceQuote())
                    .Replace("#PaymentMethod#", order.ArchivedPaymentName.ReplaceQuote())

                    .Replace("#ClientId#", customer != null ? customer.CustomerID.ToString() : "")
                    .Replace("#ClientFirstName#", customer != null ? customer.FirstName.ReplaceQuote() : "")
                    .Replace("#ClientOrganization#", customer != null ? customer.Organization.ReplaceQuote() : "")
                    .Replace("#ClientLastName#", customer != null ? customer.LastName.ReplaceQuote() : "")
                    .Replace("#ClientPatronymic#", customer != null ? customer.Patronymic.ReplaceQuote() : "")
                    .Replace("#ClientPhone#", customer != null ? customer.Phone.ReplaceQuote() : "")
                    .Replace("#ClientEmail#", customer != null ? customer.Email.ReplaceQuote() : "")

                    .Replace("#ClientCountry#", customer != null ? customer.Country.ReplaceQuote() : "")
                    .Replace("#ClientRegion#", customer != null ? customer.Region.ReplaceQuote() : "")
                    .Replace("#ClientCity#", customer != null ? customer.City.ReplaceQuote() : "")
                    .Replace("#ClientDistrict#", customer != null ? customer.District.ReplaceQuote() : "")
                    .Replace("#ClientZip#", customer != null ? customer.Zip.ReplaceQuote() : "")
                    .Replace("#ClientStreet#", customer != null ? customer.Street.ReplaceQuote() : "")
                    .Replace("#ClientHouse#", customer != null ? customer.House.ReplaceQuote() : "")
                    .Replace("#ClientApartment#", customer != null ? customer.Apartment.ReplaceQuote() : "")
                    .Replace("#ClientStructure#", customer != null ? customer.Structure.ReplaceQuote() : "")
                    .Replace("#ClientEntrance#", customer != null ? customer.Entrance.ReplaceQuote() : "")
                    .Replace("#ClientFloor#", customer != null ? customer.Floor.ReplaceQuote() : "")
                    
                    .Replace("#ClientAddress#", customer != null ? customer.GetCustomerAddress().ReplaceQuote() : "")
                    .Replace("#ClientCustomField1#", customer != null ? customer.CustomField1.ReplaceQuote() : "")
                    .Replace("#ClientCustomField2#", customer != null ? customer.CustomField2.ReplaceQuote() : "")
                    .Replace("#ClientCustomField3#", customer != null ? customer.CustomField3.ReplaceQuote() : "")
                    
                    .Replace("#ShippingCost#", order.ShippingCost.ToInvariantString("F2"))
                    .Replace("#PaymentCost#", order.PaymentCost.ToInvariantString("F2"))
                    .Replace("#BonusCost#", order.BonusCost.ToInvariantString("F2"))
                    .Replace("#OrderDiscountPercent#", order.OrderDiscount.ToInvariantString("F2"))
                    .Replace("#OrderDiscountValue#", order.OrderDiscountValue.ToInvariantString("F2"))
                    .Replace("#BonusCardNumber#", order.BonusCardNumber?.ToString())
                    .Replace("#DeliveryDate#", order.DeliveryDate?.ToString())
                    .Replace("#DeliveryTime#", order.DeliveryTime)
                    
                    .Replace("#TotalWeight#", order.TotalWeight != null ? order.TotalWeight.ToInvariantString() : "")
                    .Replace("#TotalLength#", order.TotalLength != null ? order.TotalLength.ToInvariantString() : "")
                    .Replace("#TotalWidth#", order.TotalWidth != null ? order.TotalWidth.ToInvariantString() : "")
                    .Replace("#TotalHeight#", order.TotalHeight != null ? order.TotalHeight.ToInvariantString() : "")
                    
                    .Replace("#TrackNumber#", order.TrackNumber)
                    .Replace("#Comments#", order.CustomerComment.DefaultOrEmpty().ReplaceQuote())
                    .Replace("#StatusComment#", order.StatusComment.DefaultOrEmpty().ReplaceQuote())
                    .Replace("#AdminComment#", order.AdminOrderComment.DefaultOrEmpty().ReplaceQuote());

            if (value.Contains("#OrderSource#"))
                result = result.Replace("#OrderSource#", order.OrderSource?.Name.ReplaceQuote());
            
            if (value.Contains("#Currency#"))
                result = result.Replace("#Currency#", order.OrderCurrency?.CurrencyCode.ReplaceQuote());
            
            var additionalFields = 
                value.Contains("#AdditionalFields#") && customer != null 
                    ? GetAdditionalFieldsValue(customer.CustomerID) 
                    : null;

            var items =
                value.Contains("#Products#")
                && order.OrderItems != null
                && order.OrderItems.Count > 0
                    ? order.OrderItems.Select(x => new
                    {
                        Artno = x.ArtNo.ReplaceQuote(),
                        Name = x.Name.ReplaceQuote(),
                        Price = x.Price,
                        Amount = x.Amount,
                        Color = x.Color,
                        Size = x.Size,
                        Length = x.Length,
                        Width = x.Width,
                        Height = x.Height,
                        ProductId = x.ProductID
                    })
                    : null;

            if (value == "#AdditionalFields#")
                return additionalFields;

            if (value == "#Products#")
                return items;

            if (isJson)
            {
                result = result
                    .Replace("#AdditionalFields#", JsonConvert.SerializeObject(additionalFields))
                    .Replace("#Products#", JsonConvert.SerializeObject(items));
            }

            return result;
        }

        private static object ReplaceParamsForCustomer(string value, Customer customer, bool isJson = false)
        {
            var result =
                value.Replace("#ClientId#", customer.Id.ToString())
                    .Replace("#ClientFirstName#", customer.FirstName.ReplaceQuote())
                    .Replace("#ClientLastName#", customer.LastName.ReplaceQuote())
                    .Replace("#ClientPatronymic#", customer.Patronymic.ReplaceQuote())
                    .Replace("#ClientPhone#", customer.Phone.ReplaceQuote())
                    .Replace("#ClientEmail#", customer.EMail.ReplaceQuote())
                    .Replace("#ClientOrganization#", customer.Organization.ReplaceQuote());

            if (value == "#AdditionalFields#")
                return GetAdditionalFieldsValue(customer.Id);

            if (isJson)
            {
                result = result
                    .Replace("#AdditionalFields#", JsonConvert.SerializeObject(GetAdditionalFieldsValue(customer.Id)));
            }

            return result;
        }

        private static object GetAdditionalFieldsValue(Guid customerId)
        {
            var customerFields = CustomerFieldService.GetCustomerFieldsWithValue(customerId);
            return
                customerFields != null && customerFields.Count > 0
                    ? customerFields.Select(x => new {name = x.Name, value = x.Value})
                    : null;
        }

        private static string ReplaceQuote(this string val)
        {
            return string.IsNullOrEmpty(val) ? val : val.Replace("\"", "\\\"");
        }
    }
}
