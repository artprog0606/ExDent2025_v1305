using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Letters
{
    public enum CustomerLetterTemplateKey
    {
        [LetterFormatKey("#FIRST_NAME#", Description = "Имя")]
        FirstName,

        [LetterFormatKey("#LAST_NAME#", Description = "Фамилия")]
        LastName,

        [LetterFormatKey("#PATRONYMIC#", Description = "Отчество")]
        Patronymic,

        [LetterFormatKey("#FULL_NAME#", Description = "ФИО")]
        FullName,

        [LetterFormatKey("#EMAIL#", Description = "Email")]
        Email,

        [LetterFormatKey("#ORGANIZATION#", Description = "Организация")]
        Organization,

        [LetterFormatKey("#PHONE#", Description = "Телефон")]
        Phone,

        [LetterFormatKey("#BIRTHDAY#", Description = "Дата рождения. Пример: 10.05.02")]
        BirthDay,

        [LetterFormatKey("#NEWS_SUBSCRIPTION#", Description = "Подписка на новости. Пример: \"Да\", \"Нет\"")]
        NewsSubscription,

        [LetterFormatKey("#BONUS_BALANCE#", Description = "Бонусный баланс")]
        BonusBalance,
    }
}