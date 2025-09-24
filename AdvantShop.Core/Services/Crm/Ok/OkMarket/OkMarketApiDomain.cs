using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace AdvantShop.Core.Services.Crm.Ok.OkMarket
{
    #region Responses
    public class OkMarketApiBaseResponse
    {
        public bool Success
        {
            get
            {
                return ErrorCode == 0 && ErrorMsg == null;
            }
        }

        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; }
        [DefaultValue(0)]
        [JsonProperty("error_code", DefaultValueHandling = DefaultValueHandling.Populate)]
        public int ErrorCode { get; set; }
        [JsonProperty("wait_millis")]
        public int? Millis { get; set; }

        private string fullErrorMsg;
        public string FullErrorMsg
        {
            get { return string.IsNullOrEmpty(fullErrorMsg) ? (fullErrorMsg = $"{GetErrorMessage()} {ErrorMsg}") : fullErrorMsg; }
        }

        private string GetErrorMessage()
        {
            if (ErrorCode == 1) { return "Неизвестная ошибка"; }
            if (ErrorCode == 10) { return "Отказ в разрешении. Возможная причина - пользователь не авторизовал приложение на выполнение операции"; }
            if (ErrorCode == 100) { return "Отсутствующий или неверный параметр"; }
            if (ErrorCode == 1001) { return "Ошибка возвращена сервером приложений для уведомления о неверной информации транзакции"; }
            if (ErrorCode == 1003) { return "Неверная платежная транзакция"; }
            if (ErrorCode == 1004) { return "Слишком частые запросы платежа"; }
            if (ErrorCode == 1005) { return "У пользователя недостаточно денег на аккаунте"; }
            if (ErrorCode == 101) { return "Параметр application_key не указан или указан неверно"; }
            if (ErrorCode == 102) { return "Истек срок действия ключа сессии"; }
            if (ErrorCode == 103) { return "Неверный ключ сессии"; }
            if (ErrorCode == 104) { return "Неверная подпись"; }
            if (ErrorCode == 105) { return "Неверная повторная подпись"; }
            if (ErrorCode == 106) { return "Неверный идентификатор дискуссии"; }
            if (ErrorCode == 11) { return "Достигнут предел вызовов метода"; }
            if (ErrorCode == 110) { return "Неверный идентификатор пользователя"; }
            if (ErrorCode == 1101) { return "Видео-чат отключен"; }
            if (ErrorCode == 1102) { return "Пользователь недоступен для видео-чата или видео-сообщения"; }
            if (ErrorCode == 1103) { return "Указанный пользователь должен быть другом"; }
            if (ErrorCode == 12) { return "Операция прервана пользователем"; }
            if (ErrorCode == 120) { return "Неверный идентификатор альбома"; }
            if (ErrorCode == 1200) { return "Ошибка вызова батчинга"; }
            if (ErrorCode == 121) { return "Неверный идентификатор фотографии"; }
            if (ErrorCode == 130) { return "Неверный идентификатор виджета"; }
            if (ErrorCode == 1300) { return "Платформы для приложения не установлены"; }
            if (ErrorCode == 1301) { return "Указанное устройство не доступно"; }
            if (ErrorCode == 1302) { return "Устройство не указано"; }
            if (ErrorCode == 140) { return "Неверный идентификатор сообщения"; }
            if (ErrorCode == 1400) { return "Ошибка поиска мест"; }
            if (ErrorCode == 1401) { return "Ошибка поиска мест"; }
            if (ErrorCode == 141) { return "Неверный идентификатор комментария"; }
            if (ErrorCode == 150) { return "Неверный идентификатор события"; }
            if (ErrorCode == 151) { return "Неверный идентификатор фотографии события"; }
            if (ErrorCode == 160) { return "Неверный идентификатор группы"; }
            if (ErrorCode == 2) { return "Сервис временно недоступен"; }
            if (ErrorCode == 200) { return "Приложение не может выполнить операцию. В большинстве случаев причиной является попытка получения доступа к операции без авторизации от пользователя."; }
            if (ErrorCode == 2001) { return "Неверный тип POST-контента (Graph API). Необходимо добавить заголовок Content-Type: application/json;charset=utf-8"; }
            if (ErrorCode == 21) { return "Не multi-part запрос при добавлении фотографий"; }
            if (ErrorCode == 210) { return "Приложение отключено"; }
            if (ErrorCode == 211) { return "Неверный идентификатор выбора"; }
            if (ErrorCode == 212) { return "Неверный идентификатор значка"; }
            if (ErrorCode == 213) { return "Неверный идентификатор подарка"; }
            if (ErrorCode == 214) { return "Неверный идентификатор типа связи"; }
            if (ErrorCode == 22) { return "Пользователь должен активировать свой аккаунт"; }
            if (ErrorCode == 220) { return "Неверный формат поля fields"; }
            if (ErrorCode == 23) { return "Пользователь не вовлечён в приложение"; }
            if (ErrorCode == 24) { return "Пользователь не является владельцем объекта"; }
            if (ErrorCode == 25) { return "Ошибка рассылки нотификаций. Пользователь неактивен в приложении"; }
            if (ErrorCode == 26) { return "Ошибка рассылки нотификаций. Достигнут лимит нотификаций для приложения"; }
            if (ErrorCode == 3) { return "Метод не существует"; }
            if (ErrorCode == 30) { return "Слишком большое тело запроса или проблема в обработке заголовков"; }
            if (ErrorCode == 300) { return "Информация о запросе не найдена"; }
            if (ErrorCode == 31) { return "Клиент слишком долго передавал тело запроса"; }
            if (ErrorCode == 324) { return "Ошибка обработки multi-part запроса"; }
            if (ErrorCode == 4) { return "Не удалось обработать запрос, так как он неверный"; }
            if (ErrorCode == 401) { return "Сбой аутентификации. Неверное имя пользователя/пароль или маркер аутентификации или пользователь удален/заблокирован"; }
            if (ErrorCode == 402) { return "Сбой аутентификации. Требуется ввести капчу для проверки пользователя"; }
            if (ErrorCode == 403) { return "Сбой аутентификации"; }
            if (ErrorCode == 451) { return "Указан ключ сессии, но метод должен быть вызван вне сессии"; }
            if (ErrorCode == 453) { return "Ключ сессии не указан для метода, требующего сессии"; }
            if (ErrorCode == 454) { return "Текст отклонен цензором"; }
            if (ErrorCode == 455) { return "Невозможно выполнить операцию, так как друг установил на нее ограничение(поместил в «черный список» или сделал свой аккаунт приватным)"; }
            if (ErrorCode == 456) { return "Невозможно выполнить операцию, так как группа установила на нее ограничение"; }
            if (ErrorCode == 457) { return "Неавторизованный доступ"; }
            if (ErrorCode == 458) { return "Невозможно выполнить операцию, так как друг установил на нее ограничение (поместил в «черный список» или сделал свой аккаунт приватным)"; }
            if (ErrorCode == 50) { return "У пользователя нет административных прав для выполнения данного метода"; }
            if (ErrorCode == 500) { return "Размер двоичного содержимого изображения в байтах превышает предел"; }
            if (ErrorCode == 5000) { return "Недопустимый ответ (например, указан несуществующий формат)"; }
            if (ErrorCode == 501) { return "Слишком маленький размер изображения в пикселях"; }
            if (ErrorCode == 502) { return "Слишком большой размер изображения в пикселях"; }
            if (ErrorCode == 503) { return "Невозможно распознать формат изображения"; }
            if (ErrorCode == 504) { return "Формат изображения распознан, но содержимое повреждено"; }
            if (ErrorCode == 505) { return "В запросе не найдено изображение"; }
            if (ErrorCode == 508) { return "Слишком много отметок на фотографии"; }
            if (ErrorCode == 511) { return "Ошибка проверки антиспама"; }
            if (ErrorCode == 512) { return "Попытка использовать альбом или фотографию не принадлежащую указанному пользователю"; }
            if (ErrorCode == 513) { return "Попытка использовать альбом или фотографию не принадлежащую указанной группе"; }
            if (ErrorCode == 514) { return "Пользователю необходимо пройти верификацию"; }
            if (ErrorCode == 600) { return "Слишком много параметров \"медиа\""; }
            if (ErrorCode == 601) { return "Достигнут лимит длины текста"; }
            if (ErrorCode == 602) { return "Достигнут лимит длины текста вопроса к голосованию"; }
            if (ErrorCode == 603) { return "Слишком много ответов к голосованию"; }
            if (ErrorCode == 604) { return "Достигнут лимит длины текста ответа к голосованию"; }
            if (ErrorCode == 605) { return "Достигнут лимит количества отмечаемых друзей"; }
            if (ErrorCode == 606) { return "Достигнут лимит количества отмечаемых друзей (юзер-специфик)"; }
            if (ErrorCode == 607) { return "Неверный формат ссылки в медиатопике"; }
            if (ErrorCode == 610) { return "Запрос на вступление в группу уже зарегистрирован"; }
            if (ErrorCode == 7) { return "Запрошенное действие временно заблокировано для текущего пользователя"; }
            if (ErrorCode == 700) { return "Комментарий не найден"; }
            if (ErrorCode == 701) { return "Попытка отредактировать комментарий, не принадлежащий пользователю"; }
            if (ErrorCode == 702) { return "Попытка отредактировать удалённый комментарий"; }
            if (ErrorCode == 704) { return "Время редактирования истекло"; }
            if (ErrorCode == 705) { return "Чат не найден"; }
            if (ErrorCode == 706) { return "Попытка отредактировать удалённое сообщение"; }
            if (ErrorCode == 707) { return "Пользователю недоступны стикеры"; }
            if (ErrorCode == 708) { return "Неверный формат стикеров"; }
            if (ErrorCode == 709) { return "Пользователю недоступен сервис GIF-анимаций"; }
            if (ErrorCode == 8) { return "Выполнение метода заблокировано вследствие флуда"; }
            if (ErrorCode == 800) { return "Достигнуто максимальное количество участников чата"; }
            if (ErrorCode == 801) { return "Указанный участник заблокировал текущего пользователя"; }
            if (ErrorCode == 802) { return "Передан несуществующий пользователь"; }
            if (ErrorCode == 9) { return "Выполнение метода заблокировано по IP-адресу вследствие подозрительных действий текущего пользователя или вследствие прочих ограничений, распространяющихся на конкретный метод"; }
            if (ErrorCode == 900) { return "Возвращается при попытке получить открытую информацию для несуществующего приложения"; }
            if (ErrorCode == 9999) { return "Критическая системная ошибка. Оповестите об этом службу поддержки"; }
            return string.Empty;
        }
    }

    public class OkMarketApiGetGroupResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("groups")]
        public List<OkMarketGroup> Groups { get; set; }
    }

    public class OkMarketApiAddCatalogResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("catalog_id")]
        public long CatalogId { get; set; }
    }

    public class OkMarketApiGetCatalogResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("catalogs")]
        public List<OkMarketCatalog> Catalogs { get; set; }
    }

    public class OkMarketApiAddProductResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("product_id")]
        public long ProductId { get; set; }
    }

    public class OkMarketApiGetProductsResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("anchor")]
        public string Anchor { get; set; }
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }
        [JsonProperty("products")]
        public List<OkMarketProduct> Products { get; set; }
        [JsonProperty("short_products")]
        public List<OkMarketProduct> ShortProducts { get; set; }
    }

    public class OkMarketApiPhotoUploadResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("photos")]
        public Dictionary<string, OkMarketPhotoToken> Photos { get; set; }

        [JsonProperty("photo_ids")]
        public List<string> PhotoIds { get; set; }
        [JsonProperty("upload_url")]
        public string UploadUrl { get; set; }
    }

    public class OkMarketApiGetPhotoResponse : OkMarketApiBaseResponse
    {
        [JsonProperty("photo")]
        public OkMarketPhoto Photo { get; set; }
    }

    public class OKMarketApiGetIdFromUrlResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("objectIdStr")]
        public string IdString { get; set; }
        [JsonProperty("objectId")]
        public long Id { get; set; }
    }
    #endregion
    #region Request
    public class OkMarketApiRequest
    {
        public string access_token { get; set; }
        public string application_key { get; set; }
        public string attachment { get; set; }
        public string anchor { get; set; }
        public string catalog_ids { get; set; }
        public string catalog_id { get; set; }
        public int? count { get; set; }
        public bool? delete_products { get; set; }
        public string fields { get; set; }
        public string format { get; set; }
        public string gid { get; set; }
        public string method { get; set; }
        public string name { get; set; }
        public string photos { get; set; }
        public bool? placeholders { get; set; }
        public string product_id { get; set; }
        public string product_ids { get; set; }
        public string sig { get; set; }
        public string tab { get; set; }
        public string type { get; set; }
        public string uids { get; set; }
        public string photo_id { get; set; }
        public string product_status { get; set; }
        public string url { get; set; }

        public void UpdateSinature(string secretKey)
        {
            var sortedParams = this.ToString("", forSignature: true);
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(sortedParams + secretKey));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            sig = hash.ToString().ToLower();
        }

        public string ToString(string separator, bool codeActtachmet = false, bool forSignature = false)
        {
            return string.Join(separator, ToDictionary().Where(x => x.Value != null && (forSignature ? (x.Key != "access_token" && x.Key != "sig") : true)).
                OrderBy(x => x.Key).Select(x => x.Key + "=" + (codeActtachmet ? x.Value.ToString().Replace("%", "%25").Replace("+", "%2B").Replace("&", "%26") : x.Value.ToString())));
        }

        private IDictionary<string, object> ToDictionary (BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return this.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(this, null)
            );
        }
    }
    #endregion
}