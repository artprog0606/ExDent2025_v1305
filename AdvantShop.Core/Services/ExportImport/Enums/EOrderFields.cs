using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.ExportImport
{
    public enum EOrderFields
    {
        [StringName("none")]
        [Localize("Core.ExportImport.OrderFields.NotSelected")]
        [CsvFieldsStatus(CsvFieldStatus.None)]
        None,
        
        [StringName("номер заказа")]
        [Localize("Core.ExportImport.OrderFields.Number")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Number,

        [StringName("статус")]
        [Localize("Core.ExportImport.OrderFields.Status")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        OrderStatus,

        [StringName("источник заказа")]
        [Localize("Core.ExportImport.OrderFields.OrderSource")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        OrderSource,
        
        [StringName("дата заказа")]
        [Localize("Core.ExportImport.OrderFields.OrderDateTime")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        OrderDateTime,
        
        [StringName("фио")]
        [Localize("Core.ExportImport.OrderFields.FullName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        FullName,

        [StringName("email")]
        [Localize("Core.ExportImport.OrderFields.Email")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Email,
        
        [StringName("телефон")]
        [Localize("Core.ExportImport.OrderFields.Phone")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Phone,

        [StringName("группа покупателя")]
        [Localize("Core.ExportImport.OrderFields.CustomerGroup")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CustomerGroup,

        [StringName("фио получателя")]
        [Localize("Core.ExportImport.OrderFields.RecipientFullName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        RecipientFullName,

        [StringName("телефон получателя")]
        [Localize("Core.ExportImport.OrderFields.RecipientPhone")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        RecipientPhone,

        [StringName("общий вес")]
        [Localize("Core.ExportImport.OrderFields.Weight")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Weight,
        
        [StringName("общие габариты")]
        [Localize("Core.ExportImport.OrderFields.Dimensions")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Dimensions,

        [StringName("оплачен")]
        [Localize("Core.ExportImport.OrderFields.Payed")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Payed,

        [StringName("скидка")]
        [Localize("Core.ExportImport.OrderFields.Discount")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        DiscountValue,

        [StringName("стоимость доставки")]
        [Localize("Core.ExportImport.OrderFields.ShippingCost")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        ShippingCost,

        [StringName("наценка оплаты")]
        [Localize("Core.ExportImport.OrderFields.PaymentCost")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        PaymentCost,

        [StringName("купон")]
        [Localize("Core.ExportImport.OrderFields.Coupon")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Coupon,
        
        [StringName("оплачено бонусами")]
        [Localize("Core.ExportImport.OrderFields.BonusCost")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        BonusCost,

        [StringName("итоговая стоимость")]
        [Localize("Core.ExportImport.OrderFields.Sum")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Sum,

        [StringName("валюта")]
        [Localize("Core.ExportImport.OrderFields.Currency")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Currency,

        [StringName("метод доставки")]
        [Localize("Core.ExportImport.OrderFields.ShippingName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        ShippingName,

        [StringName("метод оплаты")]
        [Localize("Core.ExportImport.OrderFields.PaymentName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        PaymentName,

        [StringName("страна")]
        [Localize("Core.ExportImport.OrderFields.Country")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Country,

        [StringName("регион")]
        [Localize("Core.ExportImport.OrderFields.Region")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Region,

        [StringName("район региона")]
        [Localize("Core.ExportImport.OrderFields.District")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        District,

        [StringName("город")]
        [Localize("Core.ExportImport.OrderFields.City")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        City,

        [StringName("улица")]
        [Localize("Core.ExportImport.OrderFields.Street")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Street,

        [StringName("индекс")]
        [Localize("Core.ExportImport.OrderFields.Zip")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Zip,

        [StringName("дом")]
        [Localize("Core.ExportImport.OrderFields.House")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        House,

        [StringName("строение")]
        [Localize("Core.ExportImport.OrderFields.Structure")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Structure,

        [StringName("квартира")]
        [Localize("Core.ExportImport.OrderFields.Apartment")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Apartment,

        [StringName("подъезд")]
        [Localize("Core.ExportImport.OrderFields.Entrance")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Entrance,

        [StringName("этаж")]
        [Localize("Core.ExportImport.OrderFields.Floor")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Floor,

        [StringName("дата доставки")]
        [Localize("Core.ExportImport.OrderFields.DeliveryDate")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        DeliveryDate,

        [StringName("время доставки")]
        [Localize("Core.ExportImport.OrderFields.DeliveryTime")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        DeliveryTime,

        [StringName("комментарий пользователя")]
        [Localize("Core.ExportImport.OrderFields.CustomerComment")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CustomerComment,

        [StringName("комментарий администратора")]
        [Localize("Core.ExportImport.OrderFields.AdminComment")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        AdminComment,

        [StringName("комментарий к статусу")]
        [Localize("Core.ExportImport.OrderFields.StatusComment")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        StatusComment,

        [StringName("менеджер")]
        [Localize("Core.ExportImport.OrderFields.Manager")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Manager,

        [StringName("код купона")]
        [Localize("Core.ExportImport.OrderFields.CouponCode")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CouponCode,

        [StringName("артикул")]
        [Localize("Core.ExportImport.OrderFields.OrderItemArtNo")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        OrderItemArtNo,

        [StringName("название")]
        [Localize("Core.ExportImport.OrderFields.OrderItemName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        OrderItemName,

        [StringName("дополнительные опции")]
        [Localize("Core.ExportImport.OrderFields.OrderItemCustomOptions")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        OrderItemCustomOptions,

        [StringName("размер")]
        [Localize("Core.ExportImport.OrderFields.OrderItemSize")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        OrderItemSize,

        [StringName("цвет")]
        [Localize("Core.ExportImport.OrderFields.OrderItemColor")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        OrderItemColor,

        [StringName("цена")]
        [Localize("Core.ExportImport.OrderFields.OrderItemPrice")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        OrderItemPrice,

        [StringName("количество")]
        [Localize("Core.ExportImport.OrderFields.OrderItemAmount")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        OrderItemAmount,

        [StringName("Google client id")]
        [Localize("Core.ExportImport.OrderFields.GoogleClientId")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        GoogleClientId,

        [StringName("Yandex client id")]
        [Localize("Core.ExportImport.OrderFields.YandexClientId")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexClientId,

        [StringName("referral")]
        [Localize("Core.ExportImport.OrderFields.Referral")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Referral,

        [StringName("страница входа")]
        [Localize("Core.ExportImport.OrderFields.LoginPage")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        LoginPage,

        [StringName("utm source")]
        [Localize("Core.ExportImport.OrderFields.UtmSource")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        UtmSource,

        [StringName("utm medium")]
        [Localize("Core.ExportImport.OrderFields.UtmMedium")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        UtmMedium,

        [StringName("utm campaign")]
        [Localize("Core.ExportImport.OrderFields.UtmCampaign")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        UtmCampaign,

        [StringName("utm content")]
        [Localize("Core.ExportImport.OrderFields.UtmContent")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        UtmContent,

        [StringName("utm term")]
        [Localize("Core.ExportImport.OrderFields.UtmTerm")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        UtmTerm,
    }
}
