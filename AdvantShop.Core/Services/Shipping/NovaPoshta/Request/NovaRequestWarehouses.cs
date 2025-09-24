namespace AdvantShop.Shipping.NovaPoshta
{
    public class NovaRequestWarehouses
    {
        /// <summary>
        /// Дополнительный фильтр по имени города
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Дополнительный фильтр по идентификатору города
        /// </summary>
        public string CityRef { get; set; }

        /// <summary>
        /// Страница, максимум 500 записей на странице. Работает в связке с параметром Limit
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// Количество записей на странице. Работает в связке с параметром Page
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Вывод описания на Украинском или русском языках - ru. По умолчанию всегда выводиться на Украинском языке.
        /// </summary>
        public string Language { get; set; }
    }
}
