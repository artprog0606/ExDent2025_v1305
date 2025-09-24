using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Letters
{
    public enum CommonLetterTemplateKey
    {
        [LetterFormatKey("#LOGO#", Description = "Логотип")]
        Logo,
        
        [LetterFormatKey("#MAIN_PHONE#", Description = "Телефон магазина")]
        StorePhone,
        
        [LetterFormatKey("#STORE_NAME#", Description = "Название магазина")]
        StoreName,
        
        [LetterFormatKey("#STORE_URL#", Description = "URL-адрес магазина")]
        StoreUrl,
    } 
}