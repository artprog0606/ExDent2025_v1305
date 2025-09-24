using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Letters
{
    public enum CustomerRegistrationLetterTemplateKey
    {
        [LetterFormatKey("#REGDATE#", Description = "Дата регистрации. Пример: 11.07.2029 10:00")] 
        RegistrationDate,
        
        [LetterFormatKey("#PASSWORD#", Description = "Пароль")] 
        Password,
    }
}