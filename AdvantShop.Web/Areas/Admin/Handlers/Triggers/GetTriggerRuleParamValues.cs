using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.BusinessProcesses.Customers;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.CustomerSegments;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Models;

namespace AdvantShop.Web.Admin.Handlers.Triggers
{
    public class GetTriggerRuleParamValues
    {
        private readonly ETriggerEventType _eventType;
        private readonly int _fieldType;
        private readonly int? _fieldObjId;

        public GetTriggerRuleParamValues(ETriggerEventType eventType, int fieldType, int? fieldObjId)
        {
            _eventType = eventType;
            _fieldType = fieldType;
            _fieldObjId = fieldObjId;
        }

        public List<SelectItemModel> Execute()
        {
            switch (_eventType)
            {
                case ETriggerEventType.OrderCreated:
                case ETriggerEventType.OrderStatusChanged:
                case ETriggerEventType.OrderPaied:
                    switch (_fieldType)
                    {
                        case (int)EOrderFieldType.CustomerGroup:
                            return GetCustomerGroupsList();

                        case (int)EOrderFieldType.CustomerField:
                            return GetCustomerFieldValuesList();

                        case (int)EOrderFieldType.OrderSource:
                            return GetOrderSourcesList();

                        case (int)EOrderFieldType.PaymentMethod:
                            return PaymentService.GetAllPaymentMethods(false).Select(x => new SelectItemModel(x.Name, x.PaymentMethodId)).ToList();

                        case (int)EOrderFieldType.ShippingMethod:
                            return ShippingMethodService.GetAllShippingMethods().Select(x => new SelectItemModel(x.Name, x.ShippingMethodId)).ToList();

                        case (int)EOrderFieldType.Manager:
                        case (int)EOrderFieldType.CustomerManager:
                            return GetManagersList();

                        case (int)EOrderFieldType.CustomerSegment:
                            return GetCustomerSegments();

                        case (int)EOrderFieldType.OpenLeadSalesFunnels:
                            return GetSalesFunnels();

                        case (int)EOrderFieldType.OrderStatus:
                            return GetOrderStatuses();

                        case (int)EOrderFieldType.CustomerType:
                            return GetCustomerTypes();

                        default:
                            return null;
                    }

                case ETriggerEventType.LeadCreated:
                case ETriggerEventType.LeadStatusChanged:
                    switch (_fieldType)
                    {
                        case (int)ELeadFieldType.CustomerGroup:
                            return GetCustomerGroupsList();

                        case (int)ELeadFieldType.CustomerField:
                            return GetCustomerFieldValuesList();

                        case (int)ELeadFieldType.Source:
                            return GetOrderSourcesList();

                        case (int)ELeadFieldType.SalesFunnel:
                        case (int)ELeadFieldType.OpenLeadSalesFunnels:
                            return GetSalesFunnels();

                        case (int)ELeadFieldType.Manager:
                        case (int)ELeadFieldType.CustomerManager:
                            return GetManagersList();

                        case (int)ELeadFieldType.CustomerSegment:
                            return GetCustomerSegments();

                        case (int)ELeadFieldType.DealStatus:
                            return GetSalesFunnels();

                        case (int)ELeadFieldType.LeadField:
                            return GetLeadFieldValuesList();

                        case (int)ELeadFieldType.CustomerType:
                            return GetCustomerTypes();

                        default:
                            return null;
                    }

                case ETriggerEventType.CustomerCreated:
                case ETriggerEventType.TimeFromLastOrder:
                case ETriggerEventType.SignificantDate:
                case ETriggerEventType.SignificantCustomerDate:
                    switch (_fieldType)
                    {
                        case (int)ECustomerFieldType.CustomerGroup:
                            return GetCustomerGroupsList();

                        case (int)ECustomerFieldType.CustomerField:
                            return GetCustomerFieldValuesList();

                        case (int)ECustomerFieldType.Manager:
                            return GetManagersList();

                        case (int)ECustomerFieldType.CustomerSegment:
                            return GetCustomerSegments();

                        case (int)ECustomerFieldType.OpenLeadSalesFunnels:
                            return GetSalesFunnels();

                        case (int)ECustomerFieldType.CustomerType:
                            return GetCustomerTypes();

                        default:
                            return null;
                    }

                default:
                    return null;
            }
        }

        private List<SelectItemModel> GetCustomerGroupsList()
        {
            return CustomerGroupService.GetCustomerGroupList().Select(x => new SelectItemModel(x.GroupName, x.CustomerGroupId.ToString())).ToList();
        }

        private List<SelectItemModel> GetCustomerFieldValuesList()
        {
            var customerField = _fieldObjId.HasValue ? CustomerFieldService.GetCustomerField(_fieldObjId.Value) : null;
            return customerField != null && customerField.FieldType == CustomerFieldType.Select
                    ? CustomerFieldService.GetCustomerFieldValues(customerField.Id).Select(x => new SelectItemModel(x.Value, x.Value)).ToList()
                    : null;
        }

        private List<SelectItemModel> GetOrderSourcesList()
        {
            return OrderSourceService.GetOrderSources().Select(x => new SelectItemModel(x.Name, x.Id)).ToList();
        }

        private List<SelectItemModel> GetManagersList()
        {
            return ManagerService.GetManagersList().Select(x => new SelectItemModel(x.FullName, x.ManagerId)).ToList();
        }

        private List<SelectItemModel> GetCustomerSegments()
        {
            return CustomerSegmentService.GetList().Select(x => new SelectItemModel(x.Name, x.Id)).ToList();
        }

        private List<SelectItemModel> GetSalesFunnels()
        {
            return SalesFunnelService.GetList().Select(x => new SelectItemModel(x.Name, x.Id)).ToList();
        }

        private List<SelectItemModel> GetOrderStatuses()
        {
            return OrderStatusService.GetOrderStatuses().Select(x => new SelectItemModel(x.StatusName, x.StatusID)).ToList();
        }

        private List<SelectItemModel> GetLeadFieldValuesList()
        {
            var leadField = _fieldObjId.HasValue ? LeadFieldService.GetLeadField(_fieldObjId.Value) : null;
            return leadField != null && leadField.FieldType == LeadFieldType.Select
                    ? LeadFieldService.GetLeadFieldValues(leadField.Id).Select(x => new SelectItemModel(x.Value, x.Value)).ToList()
                    : null;
        }

        private List<SelectItemModel> GetCustomerTypes()
        {
            return Enum.GetValues(typeof(CustomerType)).Cast<CustomerType>().Where(x => x != CustomerType.All).Select(x => new SelectItemModel
            {
                label = x.Localize(),
                value = ((int)x).ToString(),
            }).ToList();
        }
    }
}