using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.ExportImport
{
    public enum ELeadFields
    {
        [StringName("none")]
        [Localize("Core.ExportImport.LeadFields.NotSelected")]
        [CsvFieldsStatus(CsvFieldStatus.None)]
        None,
        
        [StringName("id")]
        [Localize("Core.ExportImport.LeadFields.LeadId")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Id,

        [StringName("список лидов")]
        [Localize("Core.ExportImport.LeadFields.SalesFunnel")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        SalesFunnel,

        [StringName("этап сделки")]
        [Localize("Core.ExportImport.LeadFields.DealStatus")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        DealStatus,
        
        [StringName("имя менеджера")]
        [Localize("Core.ExportImport.LeadFields.ManagerName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        ManagerName,
        
        [StringName("заголовок")]
        [Localize("Core.ExportImport.LeadFields.Title")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Title,

        [StringName("описание")]
        [Localize("Core.ExportImport.LeadFields.Description")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Description,
        
        [StringName("id пользователя")]
        [Localize("Core.ExportImport.LeadFields.CustomerId")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CustomerId,

        [StringName("имя")]
        [Localize("Core.ExportImport.LeadFields.FirstName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        FirstName,

        [StringName("фамилия")]
        [Localize("Core.ExportImport.LeadFields.LastName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        LastName,

        [StringName("отчество")]
        [Localize("Core.ExportImport.LeadFields.Patronymic")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Patronymic,

        [StringName("организация")]
        [Localize("Core.ExportImport.LeadFields.Organization")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Organization,

        [StringName("email")]
        [Localize("Core.ExportImport.LeadFields.Email")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Email,

        [StringName("телефон")]
        [Localize("Core.ExportImport.LeadFields.Phone")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Phone,

        [StringName("страна")]
        [Localize("Core.ExportImport.LeadFields.Country")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Country,

        [StringName("регион")]
        [Localize("Core.ExportImport.LeadFields.Region")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Region,
        
        [StringName("район региона")]
        [Localize("Core.ExportImport.LeadFields.District")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        District,

        [StringName("город")]
        [Localize("Core.ExportImport.LeadFields.City")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        City,

        [StringName("день рождения")]
        [Localize("Core.ExportImport.LeadFields.BirthDay")]
        [CsvFieldsStatus(CsvFieldStatus.NullableDateTime)]
        BirthDay,

        [StringName("артикул:цена:количество")]
        [Localize("Core.ExportImport.LeadFields.MultiOffer")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        MultiOffer,
        
        [StringName("валюта")]
        [Localize("Core.ExportImport.LeadFields.Currency")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Currency,
        
        [StringName("скидка процент")]
        [Localize("Core.ExportImport.LeadFields.Discount")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Discount,
        
        [StringName("скидка число")]
        [Localize("Core.ExportImport.LeadFields.DiscountValue")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        DiscountValue,
        
        [StringName("доставка")]
        [Localize("Core.ExportImport.LeadFields.ShippingName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        ShippingName,
        
        [StringName("стоимость доставки")]
        [Localize("Core.ExportImport.LeadFields.ShippingCost")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        ShippingCost,
        
        [StringName("итоговая стоимость")]
        [Localize("Core.ExportImport.LeadFields.TotalSum")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        TotalSum,
        
        [StringName("комментарий покупателя")]
        [Localize("Core.ExportImport.LeadFields.Comment")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Comment,
        
        [StringName("дата создания")]
        [Localize("Core.ExportImport.LeadFields.CreatedDate")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CreatedDate,
    }
}
