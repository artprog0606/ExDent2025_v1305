namespace AdvantShop.Core.Services.Auth
{
    public class PhoneConfirmationConfig
    {
        /// <summary>
        /// Сколько секунд между сообщением для одного телефона
        /// </summary>
        public const int SecondsPerPhoneBetweenMessage = 30;

        /// <summary>
        /// Сколько запросов в секунду с одного ip
        /// </summary>
        public const int RequestsCountPerSecondByIp = 1;
    }
}