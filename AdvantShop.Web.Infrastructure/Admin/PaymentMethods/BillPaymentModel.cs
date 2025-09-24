using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Customers;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("Bill")]
    public class BillPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string CompanyName
        {
            get => Parameters.ElementOrDefault(BillTemplate.CompanyName);
            set => Parameters.TryAddValue(BillTemplate.CompanyName, value.DefaultOrEmpty());
        }

        public string Accountant
        {
            get => Parameters.ElementOrDefault(BillTemplate.Accountant);
            set => Parameters.TryAddValue(BillTemplate.Accountant, value.DefaultOrEmpty());
        }

        public string PosAccountant
        {
            get => Parameters.ElementOrDefault(BillTemplate.PosAccountant);
            set => Parameters.TryAddValue(BillTemplate.PosAccountant, value.DefaultOrEmpty());
        }

        public string TransAccount
        {
            get => Parameters.ElementOrDefault(BillTemplate.TransAccount);
            set => Parameters.TryAddValue(BillTemplate.TransAccount, value.DefaultOrEmpty());
        }
        public string CorAccount
        {
            get => Parameters.ElementOrDefault(BillTemplate.CorAccount);
            set => Parameters.TryAddValue(BillTemplate.CorAccount, value.DefaultOrEmpty());
        }

        public string Address
        {
            get => Parameters.ElementOrDefault(BillTemplate.Address);
            set => Parameters.TryAddValue(BillTemplate.Address, value.DefaultOrEmpty());
        }

        public string Telephone
        {
            get => Parameters.ElementOrDefault(BillTemplate.Telephone);
            set => Parameters.TryAddValue(BillTemplate.Telephone, value.DefaultOrEmpty());
        }
        public string INN
        {
            get => Parameters.ElementOrDefault(BillTemplate.INN);
            set => Parameters.TryAddValue(BillTemplate.INN, value.DefaultOrEmpty());
        }

        public string KPP
        {
            get => Parameters.ElementOrDefault(BillTemplate.KPP);
            set => Parameters.TryAddValue(BillTemplate.KPP, value.DefaultOrEmpty());
        }

        public string BIK
        {
            get => Parameters.ElementOrDefault(BillTemplate.BIK);
            set => Parameters.TryAddValue(BillTemplate.BIK, value.DefaultOrEmpty());
        }

        public string BankName
        {
            get => Parameters.ElementOrDefault(BillTemplate.BankName);
            set => Parameters.TryAddValue(BillTemplate.BankName, value.DefaultOrEmpty());
        }

        public string Director
        {
            get => Parameters.ElementOrDefault(BillTemplate.Director);
            set => Parameters.TryAddValue(BillTemplate.Director, value.DefaultOrEmpty());
        }

        public string PosDirector
        {
            get => Parameters.ElementOrDefault(BillTemplate.PosDirector);
            set => Parameters.TryAddValue(BillTemplate.PosDirector, value.DefaultOrEmpty());
        }

        public string Manager
        {
            get => Parameters.ElementOrDefault(BillTemplate.Manager);
            set => Parameters.TryAddValue(BillTemplate.Manager, value.DefaultOrEmpty());
        }

        public string PosManager
        {
            get => Parameters.ElementOrDefault(BillTemplate.PosManager);
            set => Parameters.TryAddValue(BillTemplate.PosManager, value.DefaultOrEmpty());
        }

        public string StampImageName
        {
            get => Parameters.ElementOrDefault(BillTemplate.StampImageName);
            set => Parameters.TryAddValue(BillTemplate.StampImageName, value.DefaultOrEmpty());
        }

        public bool ShowPaymentDetails
        {
            get => Parameters.ElementOrDefault(BillTemplate.ShowPaymentDetails).TryParseBool();
            set => Parameters.TryAddValue(BillTemplate.ShowPaymentDetails, value.ToString());
        }

        public bool RequiredPaymentDetails
        {
            get => Parameters.ElementOrDefault(BillTemplate.RequiredPaymentDetails).TryParseBool();
            set => Parameters.TryAddValue(BillTemplate.RequiredPaymentDetails, value.ToString());
        }

        public string CustomerCompanyName
        {
            get 
            {
                var value = Parameters.ElementOrDefault(BillTemplate.CustomerCompanyNameField);
                if (string.IsNullOrEmpty(value))
                {
                    var field = CustomerFieldService.GetCustomerFieldByFieldAssignment(CustomerFieldAssignment.CompanyName);
                    return field?.Id.ToString();
                }
                return value; 
            }
            set => Parameters.TryAddValue(BillTemplate.CustomerCompanyNameField, value.DefaultOrEmpty());
        }

        public string CustomerINN
        {
            get 
            {
                var value = Parameters.ElementOrDefault(BillTemplate.CustomerINNField);
                if (string.IsNullOrEmpty(value))
                {
                    var field = CustomerFieldService.GetCustomerFieldByFieldAssignment(CustomerFieldAssignment.INN);
                    return field?.Id.ToString();
                }
                return value; 
            }
            set => Parameters.TryAddValue(BillTemplate.CustomerINNField, value.DefaultOrEmpty());
        }

        public string CustomerKpp
        {
            get 
            {
                var value = Parameters.ElementOrDefault(BillTemplate.CustomerKppField);
                if (string.IsNullOrEmpty(value))
                {
                    var field = CustomerFieldService.GetCustomerFieldByFieldAssignment(CustomerFieldAssignment.KPP);
                    return field?.Id.ToString();
                }
                return value; 
            }
            set => Parameters.TryAddValue(BillTemplate.CustomerKppField, value.DefaultOrEmpty());
        }

        public List<SelectListItem> CustomerFields
        {
            get
            {
                var fields = CustomerFieldService.GetCustomerFields(true, CustomerType.LegalEntity);
                var customerFields = new List<SelectListItem>();
                foreach (var field in fields)
                {
                    customerFields.Add(new SelectListItem { Text = field.Name, Value = field.Id.ToString() });
                }

                return customerFields;
            }
        }

        public List<SelectListItem> GetCustomerDataMethods
        {
            get
            {
                return Enum.GetValues(typeof(EGetCustomerDataMethod)).Cast<EGetCustomerDataMethod>().Select(x => new SelectListItem
                {
                    Text = x.Localize(),
                    Value = x.ToString()
                }).ToList();
            }
        }

        public string GetCustomerDataMethod 
        {
            get => Parameters.ElementOrDefault(BillTemplate.GetCustomerDataMethod);
            set => Parameters.TryAddValue(BillTemplate.GetCustomerDataMethod, value.DefaultOrEmpty());
        }

        public bool IsEnabledLegalEntity => SettingsCustomers.IsRegistrationAsLegalEntity;

        public override Tuple<string, string> Instruction => new Tuple<string, string>("https://www.advantshop.net/help/pages/payment-legal", "Инструкция. Банковский перевод для юр.лиц");

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(CompanyName) ||
                string.IsNullOrWhiteSpace(Accountant) ||
                string.IsNullOrWhiteSpace(CorAccount) ||
                string.IsNullOrWhiteSpace(Address) ||
                string.IsNullOrWhiteSpace(Telephone) ||
                string.IsNullOrWhiteSpace(INN) ||
                string.IsNullOrWhiteSpace(BIK) ||
                string.IsNullOrWhiteSpace(BankName) ||
                string.IsNullOrWhiteSpace(Director) ||
                string.IsNullOrWhiteSpace(Manager))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
