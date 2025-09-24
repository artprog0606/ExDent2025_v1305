//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.ExportImport
{
    public enum ECustomerFields
    {
        [StringName("none")]
        [Localize("Core.ExportImport.CustomerFields.NotSelected")]
        [CsvFieldsStatus(CsvFieldStatus.None)]
        None,
        
        [StringName("customerid")]
        [Localize("Core.ExportImport.CustomerFields.CustomerId")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CustomerId,

        [StringName("firstname")]
        [Localize("Core.ExportImport.CustomerFields.FirstName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        FirstName,

        [StringName("lastname")]
        [Localize("Core.ExportImport.CustomerFields.LastName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        LastName,

        [StringName("patronymic")]
        [Localize("Core.ExportImport.CustomerFields.Patronymic")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Patronymic,

        [StringName("phone")]
        [Localize("Core.ExportImport.CustomerFields.Phone")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Phone,

        [StringName("email")]
        [Localize("Core.ExportImport.CustomerFields.Email")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Email,
        
        [StringName("birthday")]
        [Localize("Core.ExportImport.CustomerFields.BirthDay")]
        [CsvFieldsStatus(CsvFieldStatus.NullableDateTime)]
        BirthDay,

        [StringName("organization")]
        [Localize("Core.ExportImport.CustomerFields.Organization")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Organization,

        [StringName("customergroup")]
        [Localize("Core.ExportImport.CustomerFields.CustomerGroup")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CustomerGroup,
        
        [StringName("customertype")]
        [Localize("Core.ExportImport.CustomerFields.CustomerType")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CustomerType,

        [StringName("enabled")]
        [Localize("Core.ExportImport.CustomerFields.Enabled")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Enabled,
        
        [StringName("registrationdatetime")]
        [Localize("Core.ExportImport.CustomerFields.RegistrationDateTime")]
        [CsvFieldsStatus(CsvFieldStatus.NullableDateTime)]
        RegistrationDateTime,
        
        [StringName("admincomment")]
        [Localize("Core.ExportImport.CustomerFields.AdminComment")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        AdminComment,
        
        [StringName("managername")]
        [Localize("Core.ExportImport.CustomerFields.ManagerName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        ManagerName,

        [StringName("managerid")]
        [Localize("Core.ExportImport.CustomerFields.ManagerId")]
        [CsvFieldsStatus(CsvFieldStatus.Int)]
        ManagerId,
        
        [StringName("isagreeforpromotionalnewsletter")]
        [Localize("Core.ExportImport.CustomerFields.IsAgreeForPromotionalNewsletter")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        IsAgreeForPromotionalNewsletter,

        [StringName("zip")]
        [Localize("Core.ExportImport.CustomerFields.Zip")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Zip,
        
        [StringName("country")]
        [Localize("Core.ExportImport.CustomerFields.Country")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Country,

        [StringName("region")]
        [Localize("Core.ExportImport.CustomerFields.Region")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Region,
        
        [StringName("city")]
        [Localize("Core.ExportImport.CustomerFields.City")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        City,
        
        [StringName("district")]
        [Localize("Core.ExportImport.CustomerFields.District")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        District,

        [StringName("street")]
        [Localize("Core.ExportImport.CustomerFields.Street")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Street,

        [StringName("house")]
        [Localize("Core.ExportImport.CustomerFields.House")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        House,

        [StringName("apartment")]
        [Localize("Core.ExportImport.CustomerFields.Apartment")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Apartment,
        
        [StringName("structure")]
        [Localize("Core.ExportImport.CustomerFields.Structure")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Structure,
        
        [StringName("entrance")]
        [Localize("Core.ExportImport.CustomerFields.Entrance")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Entrance,
        
        [StringName("floor")]
        [Localize("Core.ExportImport.CustomerFields.Floor")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Floor,
        
        [StringName("lastorder")]
        [Localize("Core.ExportImport.CustomerFields.LastOrder")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        LastOrder,
        
        [StringName("paidorderscount")]
        [Localize("Core.ExportImport.CustomerFields.PaidOrdersCount")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        PaidOrdersCount,
        
        [StringName("paidorderssum")]
        [Localize("Core.ExportImport.CustomerFields.PaidOrdersSum")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        PaidOrdersSum,
        
        [StringName("bonuscard")]
        [Localize("Core.ExportImport.CustomerFields.BonusCard")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        BonusCard,
        
        [StringName("tags")]
        [Localize("Core.ExportImport.CustomerFields.Tags")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Tags,
        
        [StringName("registeredfrom")]
        [Localize("Core.ExportImport.CustomerFields.RegisteredFrom")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        RegisteredFrom,
        
        [StringName("registeredfromip")]
        [Localize("Core.ExportImport.CustomerFields.RegisteredFromIp")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        RegisteredFromIp,
        
        [StringName("agreeforpromotionalnewsletterdatetime")]
        [Localize("Core.ExportImport.CustomerFields.AgreeForPromotionalNewsletterDateTime")]
        [CsvFieldsStatus(CsvFieldStatus.NullableDateTime)]
        AgreeForPromotionalNewsletterDateTime,
        
        [StringName("agreeforpromotionalnewsletterfrom")]
        [Localize("Core.ExportImport.CustomerFields.AgreeForPromotionalNewsletterFrom")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        AgreeForPromotionalNewsletterFrom,
        
        [StringName("agreeforpromotionalnewsletterfromip")]
        [Localize("Core.ExportImport.CustomerFields.AgreeForPromotionalNewsletterFromIp")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        AgreeForPromotionalNewsletterFromIp,
    }
}