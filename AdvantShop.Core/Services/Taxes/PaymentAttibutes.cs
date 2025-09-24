using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Core.Services.Taxes
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ePaymentSubjectType
    {
        /// <summary>
        /// товар
        /// </summary>
        [Localize("Товар")]
        commodity = 1,

        /// <summary>
        ///  подакцизный товар
        /// </summary>
        [Localize("Подакцизный товар")]
        excise = 2,

        /// <summary>
        /// работа
        /// </summary>
        [Localize("Работа")]
        job = 3,

        /// <summary>
        /// услуга
        /// </summary>
        [Localize("Услуга")]
        service = 4,

        /// <summary>
        /// ставка в азартной игре
        /// </summary>
        [Localize("Ставка в азартной игре")]
        gambling_bet = 5,

        /// <summary>
        /// выигрыш в азартной игре
        /// </summary>
        [Localize("Выигрыш в азартной игре")]
        gambling_prize = 6,

        /// <summary>
        ///  лотерейный билет
        /// </summary>
        [Localize("Лотерейный билет")]
        lottery = 7,

        /// <summary>
        /// выигрыш в лотерею
        /// </summary>
        [Localize("Выигрыш в лотерею")]
        lottery_prize = 8,

        /// <summary>
        /// результаты интеллектуальной деятельности
        /// </summary>
        [Localize("Результаты интеллектуальной деятельности")]
        intellectual_activity = 9,

        /// <summary>
        /// платеж
        /// </summary>
        [Localize("Платеж")]
        payment = 10,

        /// <summary>
        ///  агентское вознаграждение
        /// </summary>
        [Localize("Агентское вознаграждение")]
        agent_commission = 11,

        /// <summary>
        ///  несколько вариантов
        /// </summary>
        [Localize("Несколько вариантов")]
        composite = 12,

        /// <summary>
        /// другое
        /// </summary>
        [Localize("Другое")]
        another = 13
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ePaymentMethodType
    {
        /// <summary>
        /// полная предоплата
        /// </summary>
        [Localize("Полная предоплата")]
        full_prepayment = 1,

        /// <summary>
        /// частичная предоплата
        /// </summary>
        [Localize("Частичная предоплата")]
        partial_prepayment = 2,

        /// <summary>
        /// аванс
        /// </summary>
        [Localize("Аванс")]
        advance = 3,

        /// <summary>
        /// полный расчет
        /// </summary>
        [Localize("Полный расчет")]
        full_payment = 4,

        /// <summary>
        /// частичный расчет и кредит
        /// </summary>
        [Localize("Частичный расчет и кредит")]
        partial_payment = 5,

        /// <summary>
        /// кредит
        /// </summary>
        [Localize("Кредит")]
        credit = 6,

        /// <summary>
        /// выплата по кредиту
        /// </summary>
        [Localize("Выплата по кредиту")]
        credit_payment = 7
    }
}
