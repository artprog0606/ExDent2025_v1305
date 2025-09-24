using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Letters
{
    public enum LeadLetterTemplateKey
    {
        [LetterFormatKey("#LEAD_ID#", Description = "Номер лида")]
        LeadId,
        
        [LetterFormatKey("#LEAD_TITLE#", Description = "Заголовок лида")]
        Title,
        
        [LetterFormatKey("#DESCRIPTION#", Description = "Описание. HTML")]
        DescriptionHtml,
        
        [LetterFormatKey("#DESCRIPTION_TEXT#", Description = "Описание")]
        Description,

        [LetterFormatKey("#COMMENTS#", Description = "Комментарий пользователя")]
        Comment,

        [LetterFormatKey("#DEAL_STATUS#", Description = "Этап сделки")]
        DealStatus,
        
        [LetterFormatKey("#LEADS_LIST#", "#SALES_FUNNEL#", Description = "Список лидов")]
        SalesFunnel,

        [LetterFormatKey("#DATE#", Description = "Дата создания")]
        DateCreated,

        [LetterFormatKey("#MANAGER_NAME#", Description = "Менеджер")]
        ManagerName,
        
        [LetterFormatKey("#SOURCE#", Description = "Источник")]
        LeadSource,
        
        [LetterFormatKey("#LEAD_ATTACHMENTS#", Description = "Прикрепленные ссылки")]
        AttachmentHtmlLinks,
        
        [LetterFormatKey("#LEAD_URL#", Description = "URL ссылка на лид")]
        LeadUrl,


        [LetterFormatKey("#NAME#", "#FULL_NAME#", Description = "ФИО")]
        FullName,
        
        [LetterFormatKey("#FIRST_NAME#", Description = "Имя")]
        FirstName,
        
        [LetterFormatKey("#LAST_NAME#", Description = "Фамилия")]
        LastName,

        [LetterFormatKey("#PHONE#", Description = "Телефон")]
        Phone,
        
        [LetterFormatKey("#EMAIL#", Description = "Email")] 
        Email,
        
        [LetterFormatKey("#ORGANIZATION#", Description = "Организация")]
        Organization,
        
        [LetterFormatKey( "#ADDITIONALCUSTOMERFIELDS#", "#ADDITIONAL_CUSTOMER_FIELDS#", Description = "Дополнительные поля пользователя. HTML")]
        AdditionalCustomerFields,
        
        
        [LetterFormatKey("#LEAD_ITEMS_HTML#", "#ORDERTABLE#", Description = "Список товаров. HTML")]
        LeadItemsHtml,

        [LetterFormatKey("#SUM#", Description = "Бюджет (сумма). Пример: 1 100 руб.")]
        Sum,
        
        [LetterFormatKey("#SUM_WITHOUT_CURRENCY#", Description = "Бюджет (сумма без валюты). Пример: 1 100")]
        SumWithoutCurrency,
        
        
        [LetterFormatKey("#SHIPPING_NAME#", "#SHIPPINGMETHOD#", Description = "Название доставки. HTML")]
        ShippingName,
        
        [LetterFormatKey("#SHIPPING_NAME_WITHOUT_HTML#", Description = "Название доставки")]
        ShippingNameWithoutHtml,

        [LetterFormatKey("#COUNTRY#", Description = "Страна")]
        Country,

        [LetterFormatKey("#REGION#", Description = "Область")]
        Region,
        
        [LetterFormatKey("#DISTRICT#", Description = "Район")]
        District,
        
        [LetterFormatKey("#CITY#", Description = "Город")]
        City,
    }
}