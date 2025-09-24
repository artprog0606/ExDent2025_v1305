using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Crm.BusinessProcesses;

namespace AdvantShop.Customers
{
    public enum CustomerFieldType
    {
        [Localize("Core.Customers.CustomerFieldType.Select"), FieldType(EFieldType.Select)]
        Select = 0,
        [Localize("Core.Customers.CustomerFieldType.Text"), FieldType(EFieldType.Text)]
        Text = 1,
        [Localize("Core.Customers.CustomerFieldType.Number"), FieldType(EFieldType.Number)]
        Number = 2,
        [Localize("Core.Customers.CustomerFieldType.TextArea"), FieldType(EFieldType.Text)]
        TextArea = 3,
        [Localize("Core.Customers.CustomerFieldType.Date"), FieldType(EFieldType.Date)]
        Date = 4,
        [Localize("Core.Customers.CustomerFieldType.Tel"), FieldType(EFieldType.Tel)]
        Tel = 5,
        [Localize("Core.Customers.CustomerFieldType.Checkbox"), FieldType(EFieldType.Checkbox)]
        Checkbox = 6,
        [Localize("Core.Customers.CustomerFieldType.Email"), FieldType(EFieldType.Email)]
        Email = 7
    }
    
    public enum CustomerType
    {
        [Localize("Core.Customers.CustomerFieldCustomerType.Physical")]
        PhysicalEntity = 0,
        [Localize("Core.Customers.CustomerFieldCustomerType.Legal")]
        LegalEntity = 1,
        [Localize("Core.Customers.CustomerFieldCustomerType.All")]
        All = 2
    }

    public enum CustomerFieldAssignment
    {
        [Localize("Core.Customers.CustomerLegalEntityField.None")]
        None = 0,
        [Localize("Core.Customers.CustomerLegalEntityField.CompanyName")]
        CompanyName = 1,
        [Localize("Core.Customers.CustomerLegalEntityField.LegalAddress")]
        LegalAddress = 2,
        [Localize("Core.Customers.CustomerLegalEntityField.INN")]
        INN = 3,
        [Localize("Core.Customers.CustomerLegalEntityField.KPP")]
        KPP = 4,
        [Localize("Core.Customers.CustomerLegalEntityField.OGRN")]
        OGRN = 5,
        [Localize("Core.Customers.CustomerLegalEntityField.OKPO")]
        OKPO = 6,
        [Localize("Core.Customers.CustomerLegalEntityField.BIK")]
        BIK = 7,
        [Localize("Core.Customers.CustomerLegalEntityField.BankName")]
        BankName = 8,
        [Localize("Core.Customers.CustomerLegalEntityField.CorrespondentAccount")]
        CorrespondentAccount = 9,
        [Localize("Core.Customers.CustomerLegalEntityField.PaymentAccount")]
        PaymentAccount = 10,
    }
}
