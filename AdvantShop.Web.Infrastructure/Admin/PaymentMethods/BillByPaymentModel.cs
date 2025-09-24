using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("BillBy")]
    public class BillByPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string CompanyName
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.CompanyName); }
            set { Parameters.TryAddValue(BillByTemplate.CompanyName, value.DefaultOrEmpty()); }
        }

        public string Accountant
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.Accountant); }
            set { Parameters.TryAddValue(BillByTemplate.Accountant, value.DefaultOrEmpty()); }
        }

        public string PosAccountant
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.PosAccountant); }
            set { Parameters.TryAddValue(BillByTemplate.PosAccountant, value.DefaultOrEmpty()); }
        }

        public string TransAccount
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.TransAccount); }
            set { Parameters.TryAddValue(BillByTemplate.TransAccount, value.DefaultOrEmpty()); }
        }
        public string CorAccount
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.CorAccount); }
            set { Parameters.TryAddValue(BillByTemplate.CorAccount, value.DefaultOrEmpty()); }
        }

        public string Address
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.Address); }
            set { Parameters.TryAddValue(BillByTemplate.Address, value.DefaultOrEmpty()); }
        }

        public string Telephone
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.Telephone); }
            set { Parameters.TryAddValue(BillByTemplate.Telephone, value.DefaultOrEmpty()); }
        }
        public string UNP
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.UNP); }
            set { Parameters.TryAddValue(BillByTemplate.UNP, value.DefaultOrEmpty()); }
        }

        public string OKPO
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.OKPO); }
            set { Parameters.TryAddValue(BillByTemplate.OKPO, value.DefaultOrEmpty()); }
        }

        public string BIK
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.BIK); }
            set { Parameters.TryAddValue(BillByTemplate.BIK, value.DefaultOrEmpty()); }
        }

        public string BankName
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.BankName); }
            set { Parameters.TryAddValue(BillByTemplate.BankName, value.DefaultOrEmpty()); }
        }

        public string Director
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.Director); }
            set { Parameters.TryAddValue(BillByTemplate.Director, value.DefaultOrEmpty()); }
        }

        public string PosDirector
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.PosDirector); }
            set { Parameters.TryAddValue(BillByTemplate.PosDirector, value.DefaultOrEmpty()); }
        }

        public string Manager
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.Manager); }
            set { Parameters.TryAddValue(BillByTemplate.Manager, value.DefaultOrEmpty()); }
        }

        public string PosManager
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.PosManager); }
            set { Parameters.TryAddValue(BillByTemplate.PosManager, value.DefaultOrEmpty()); }
        }

        public string StampImageName
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.StampImageName); }
            set { Parameters.TryAddValue(BillByTemplate.StampImageName, value.DefaultOrEmpty()); }
        }

        public bool ShowPaymentDetails
        {
            get { return Parameters.ElementOrDefault(BillByTemplate.ShowPaymentDetails).TryParseBool(); }
            set { Parameters.TryAddValue(BillByTemplate.ShowPaymentDetails, value.ToString()); }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(CompanyName) ||
                string.IsNullOrWhiteSpace(Accountant) ||
                string.IsNullOrWhiteSpace(CorAccount) ||
                string.IsNullOrWhiteSpace(Address) ||
                string.IsNullOrWhiteSpace(Telephone) ||
                string.IsNullOrWhiteSpace(UNP) ||
                //string.IsNullOrWhiteSpace(OKPO) ||
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
