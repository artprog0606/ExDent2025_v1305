using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("BillKz")]
    public class BillKzPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string CompanyName
        {
            get { return Parameters.ElementOrDefault(BillKzTemplate.CompanyName); }
            set { Parameters.TryAddValue(BillKzTemplate.CompanyName, value.DefaultOrEmpty()); }
        }

        public string Contractor
        {
            get { return Parameters.ElementOrDefault(BillKzTemplate.Contractor); }
            set { Parameters.TryAddValue(BillKzTemplate.Contractor, value.DefaultOrEmpty()); }
        }

        public string PosContractor
        {
            get { return Parameters.ElementOrDefault(BillKzTemplate.PosContractor); }
            set { Parameters.TryAddValue(BillKzTemplate.PosContractor, value.DefaultOrEmpty()); }
        }

        public string IIK
        {
            get { return Parameters.ElementOrDefault(BillKzTemplate.IIK); }
            set { Parameters.TryAddValue(BillKzTemplate.IIK, value.DefaultOrEmpty()); }
        }

        public string Address
        {
            get { return Parameters.ElementOrDefault(BillKzTemplate.Address); }
            set { Parameters.TryAddValue(BillKzTemplate.Address, value.DefaultOrEmpty()); }
        }

        public string Telephone
        {
            get { return Parameters.ElementOrDefault(BillKzTemplate.Telephone); }
            set { Parameters.TryAddValue(BillKzTemplate.Telephone, value.DefaultOrEmpty()); }
        }
        public string BINIIN
        {
            get { return Parameters.ElementOrDefault(BillKzTemplate.BINIIN); }
            set { Parameters.TryAddValue(BillKzTemplate.BINIIN, value.DefaultOrEmpty()); }
        }

        public string KNP
        {
            get { return Parameters.ElementOrDefault(BillKzTemplate.KNP); }
            set { Parameters.TryAddValue(BillKzTemplate.KNP, value.DefaultOrEmpty()); }
        }

        public string KBE
        {
            get { return Parameters.ElementOrDefault(BillKzTemplate.KBE); }
            set { Parameters.TryAddValue(BillKzTemplate.KBE, value.DefaultOrEmpty()); }
        }

        public string BIK
        {
            get { return Parameters.ElementOrDefault(BillKzTemplate.BIK); }
            set { Parameters.TryAddValue(BillKzTemplate.BIK, value.DefaultOrEmpty()); }
        }

        public string BankName
        {
            get { return Parameters.ElementOrDefault(BillKzTemplate.BankName); }
            set { Parameters.TryAddValue(BillKzTemplate.BankName, value.DefaultOrEmpty()); }
        }

        public string StampImageName
        {
            get { return Parameters.ElementOrDefault(BillKzTemplate.StampImageName); }
            set { Parameters.TryAddValue(BillKzTemplate.StampImageName, value.DefaultOrEmpty()); }
        }

        public bool ShowPaymentDetails
        {
            get { return Parameters.ElementOrDefault(BillKzTemplate.ShowPaymentDetails).TryParseBool(); }
            set { Parameters.TryAddValue(BillKzTemplate.ShowPaymentDetails, value.ToString()); }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(CompanyName) ||
                string.IsNullOrWhiteSpace(Contractor) ||
                //string.IsNullOrWhiteSpace(PosContractor) ||
                string.IsNullOrWhiteSpace(IIK) ||
                string.IsNullOrWhiteSpace(Address) ||
                string.IsNullOrWhiteSpace(Telephone) ||
                string.IsNullOrWhiteSpace(BINIIN) ||
                string.IsNullOrWhiteSpace(KNP) ||
                string.IsNullOrWhiteSpace(KBE) ||
                string.IsNullOrWhiteSpace(BIK) ||
                string.IsNullOrWhiteSpace(BankName))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
