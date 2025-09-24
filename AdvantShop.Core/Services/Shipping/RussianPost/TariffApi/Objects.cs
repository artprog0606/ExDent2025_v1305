using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Extensions;
using System.Collections.Generic;

namespace AdvantShop.Shipping.RussianPost.TariffApi
{
    #region Base

    public abstract class BaseResponse
    {
        /// <summary>
        /// Версия сервиса
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Массив сообщений об ошибке
        /// </summary>
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public string Msg { get; set; }
        public long Code { get; set; }
        public long Type { get; set; }
    }

    #endregion

    #region Calculate

    public class CalculateParams
    {
        public CalculateParams()
        {
            Services = new List<int>();
        }

        /// <summary>
        /// Код объекта расчета 
        /// </summary>
        public long ObjectId { get; set; }

        /// <summary>
        /// Почтовый индекс места отправления
        /// </summary>
        public string IndexFrom { get; set; }

        /// <summary>
        /// Почтовый индекс места назначения
        /// </summary>
        public string IndexTo { get; set; }

        /// <summary>
        /// Страна (не задокументированов, но на практике требуется)
        /// </summary>
        public int? Country { get; set; }

        /// <summary>
        /// Страна назначения для международных исходящих отправлений, код по РТМ-2 
        /// </summary>
        public int? CountryTo { get; set; }

        /// <summary>
        /// Страна приема для международных входящих отправлений, код по РТМ-2 
        /// </summary>
        public int? CountryFrom { get; set; }

        /// <summary>
        /// Вес отправления. Указывается в граммах 
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Сумма объявленной ценности в копейках
        /// </summary>
        public int? Sumoc { get; set; }

        /// <summary>
        /// Сумма наложенного платежа в копейках.
        /// Используется при тарификации переводов наложенного платежа.
        /// При указании сумма НП сравнивается c суммой ОЦ (если сумма НП больше, выводится сообщение об ошибке)
        /// </summary>
        public int? Sumnp { get; set; }

        /// <summary>
        /// Сумма платежа или вложения в копейках
        /// </summary>
        public int? Sumin { get; set; }

        /// <summary>
        /// Размер отправления в сантиметрах.
        /// <remarks>Указывается как три целых числа, разделенные знаком “x” </remarks>
        /// <example>100x50x25</example>
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Код типа упаковки
        /// </summary>
        public EnPackType Pack { get; set; }

        /// <summary>
        /// Договор между корпоративным клиентом и АО «Почта России». Строка состоит из кода
        /// региона по Конституции РФ – 2-значного с ведущими нулями; ИНН предприятия – 10-
        /// или 12-значного с ведущими нулями; номера договора.Коды разделены дефисом: «-».
        /// При наличии параметра dogovor в массив доп.услуг автоматически вносится услуга 70
        /// </summary>
        public string Dogovor { get; set; }

        /// <summary>
        /// Предпочтительный вариант доставки
        /// </summary>
        public EnIsAviaType IsAvia { get; set; }

        /// <summary>
        /// Указываются модификаторы расчета, т.е. коды дополнительных услуг или вариантов
        /// расчета(см.Приложение 2), которые должны быть разделены запятой.Список может быть
        /// обрамлен в квадратные скобки(не обязательно). Услуга 70 добавляется автоматически при
        /// наличии параметра dogovor. Если в массиве 2 взаимоисключающие услуги, которые не
        /// могут применяться одновременно, то используется та услуга, которая указана
        /// первой.Например, услуги 9 «Доставка документов» и 10 «Доставка товаров» –
        /// взаимоисключающие.Если в массиве указано «10, 9», то будет применяться услуга 10. 
        /// </summary>
        public List<int> Services { get; set; }

        /// <summary>
        /// Сумма платежа или вложения в копейках
        /// </summary>
        public long? Countinpack { get; set; }


        public Dictionary<string, string> ToDictionary()
        {
            var data = new Dictionary<string, string>();

            // Если указано значение 1, при ошибке расчета
            // возвращается HTTPS-код ответа в диапазоне
            // 400 -499. Иначе – возвращается код 200.
            data.Add("errorcode", "0");

            // Признак возможности 1 или невозможность 0
            // расчета во временно закрытый для доставки
            // период.При 0 и расчет ведется в закрытый
            // период, то выводится сообщение об ошибке,
            // иначе расчет производится по наземным
            // тарифам
            data.Add("closed", "1");

            data.Add("object", ObjectId.ToString());

            /*
             * Примечание 1:
             * При отсутствии значения параметра этот параметр не передается, передавать параметр с пустым
             * значением недопустимо, это приводит к ошибке анализа входных параметров. 
             */

            if (IndexFrom.IsNotEmpty())
                data.Add("from", IndexFrom);

            if (IndexTo.IsNotEmpty())
                data.Add("to", IndexTo);

            if (Country.HasValue)
                data.Add("country", Country.Value.ToString());

            if (CountryTo.HasValue)
                data.Add("country-to", CountryTo.Value.ToString());

            if (CountryFrom.HasValue)
                data.Add("country-from", CountryFrom.Value.ToString());

            data.Add("weight", Weight.ToString());

            if (Sumoc.HasValue)
                data.Add("sumoc", Sumoc.Value.ToString());

            if (Sumnp.HasValue)
                data.Add("sumnp", Sumnp.Value.ToString());

            if (Sumin.HasValue)
                data.Add("sumin", Sumin.Value.ToString());

            if (Size.IsNotEmpty())
                data.Add("size", Size);

            if (Pack != null)
                data.Add("pack", Pack.Value.ToString());

            if (Dogovor.IsNotEmpty())
                data.Add("dogovor", Dogovor);

            if (IsAvia != null)
                data.Add("isavia", IsAvia.Value.ToString());

            if (Countinpack.HasValue)
                data.Add("countinpack", Countinpack.Value.ToString());

            if (Services != null && Services.Count > 0)
                data.Add("service", string.Join(",", Services));

            return data;
        }
    }

    public class EnPackType : IntegerEnum<EnPackType>
    {
        public EnPackType(int value) : base(value) { }

        /// <summary>
        /// Коробка «S»
        /// </summary>
        public static EnPackType BoxS { get { return new EnPackType(10); } }

        /// <summary>
        /// Пакет полиэтиленовый «S»
        /// </summary>
        public static EnPackType PacketS { get { return new EnPackType(11); } }

        /// <summary>
        /// Конверт с воздушно-пузырчатой пленкой «S»
        /// </summary>
        public static EnPackType EnvelopeS { get { return new EnPackType(12); } }

        /// <summary>
        /// Коробка «М»
        /// </summary>
        public static EnPackType BoxM { get { return new EnPackType(20); } }

        /// <summary>
        /// Пакет полиэтиленовый «М»
        /// </summary>
        public static EnPackType PacketM { get { return new EnPackType(21); } }

        /// <summary>
        /// Конверт с воздушно-пузырчатой пленкой «М»
        /// </summary>
        public static EnPackType EnvelopeM { get { return new EnPackType(22); } }

        /// <summary>
        /// Коробка «L»
        /// </summary>
        public static EnPackType BoxL { get { return new EnPackType(30); } }

        /// <summary>
        /// Пакет полиэтиленовый «L»
        /// </summary>
        public static EnPackType PacketL { get { return new EnPackType(31); } }

        /// <summary>
        /// Коробка «ХL»
        /// </summary>
        public static EnPackType BoxXL { get { return new EnPackType(40); } }

        /// <summary>
        /// Пакет полиэтиленовый «ХL»
        /// </summary>
        public static EnPackType PacketXL { get { return new EnPackType(41); } }

        /// <summary>
        /// Нестандартная упаковка
        /// </summary>
        public static EnPackType Oversized { get { return new EnPackType(99); } }
    }

    public class EnIsAviaType: IntegerEnum<EnIsAviaType>
    {
        public EnIsAviaType(int value) : base(value) { }

        /// <summary>
        /// наземная доставка
        /// </summary>
        public static EnIsAviaType Surface { get { return new EnIsAviaType(0); } }

        /// <summary>
        /// предпочтительно воздушная доставка
        /// </summary>
        public static EnIsAviaType AviaOrSurface { get { return new EnIsAviaType(1); } }

        /// <summary>
        /// строго воздушная доставка
        /// </summary>
        public static EnIsAviaType Avia { get { return new EnIsAviaType(2); } }

    }

    public class CalculateResponse : BaseResponse
    {
        /// <summary>
        /// Наименование сервиса
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Код объекта расчета 
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Наименование объекта расчета 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Вид отправления, согласно РТМ-2. 
        /// </summary>
        public int Mailtype { get; set; }

        /// <summary>
        /// Категория отправления, согласно РТМ-2.
        /// </summary>
        public int Mailctg { get; set; }

        /// <summary>
        /// Направление доставки отправления:
        /// <para>1 – внутреннее(РТМ-2);</para>
        /// <para>2 – исходящее международное(РТМ-2);</para>
        /// <para>3 – входящее международное;</para>
        /// <para>4 – транзитное международное.</para>
        /// </summary>
        public int Directctg { get; set; }

        /// <summary>
        /// Почтовый индекс места отправления
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// Почтовый индекс места назначения
        /// </summary>
        public int To { get; set; }

        /// <summary>
        /// Способ доставки по РТМ-2
        /// </summary>
        public long Transtype { get; set; }

        public string Transname { get; set; }

        /// <summary>
        /// Трехзначное обозначение валюты расчета согласно ISO 4217.
        /// <para>Обязательно для тарифов, у которых валюта расчета отлична от рубля</para>
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Итоговая сумма при оплате почтовыми марками в копейках
        /// </summary>
        public long? Paymark { get; set; }

        /// <summary>
        /// Итоговая сумма платы без НДС в копейках (в валюте расчета)
        /// </summary>
        public long? Pay { get; set; }

        /// <summary>
        /// Итоговая сумма платы с НДС в копейках (в валюте расчета)
        /// </summary>
        public long? Paynds { get; set; }

        /// <summary>
        /// Ставка НДС в процентах 
        /// </summary>
        public int? Ndsrate { get; set; }

        /// <summary>
        /// Сумма НДС в копейках (в валюте расчета)
        /// </summary>
        public long? Nds { get; set; }

        /// <summary>
        /// Контрольные сроки доставки
        /// </summary>
        public Delivery Delivery { get; set; }
    }

    public class Delivery
    {
        /// <summary>
        /// Минимальное нормативное количество дней доставки отправления
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// Максимальное нормативное количество дней доставки отправления
        /// </summary>
        public int Max { get; set; }
    }

    #endregion

    #region DeliveryParams

    public class GetDeliveryParamsResponse : BaseResponse
    {
        /// <summary>
        /// Код категории объекта расчета
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Наименование объекта расчета 
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Список объектов расчета
        /// </summary>
        public List<DeliveryParamsObject> Object { get; set; }
    }
    
    public class DeliveryParamsObject
    {
        /// <summary>
        /// Код объекта расчета
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Вид отправления, согласно РТМ-2. 
        /// </summary>
        public int? Mailtype { get; set; }
        
        /// <summary>
        /// Категория отправления, согласно РТМ-2.
        /// </summary>
        public int? Mailctg { get; set; }
        
        /// <summary>
        /// направление доставки
        /// </summary>
        public byte? Directctg { get; set; }
        
        /// <summary>
        /// Наименование объекта расчета
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// При наличии указывается код группы ограничений страны:
        /// <para>correspondence - группа для письменной корреспонденции</para>
        /// <para>parcel - группа для посылок</para>
        /// <para>ems - группа для EMS отпралений</para>
        /// </summary>
        public string CountryGroup { get; set; }

        /// <summary>
        /// Признак наличия доставки отправления
        /// </summary>
        public bool? Transfer { get; set; }
        public long? Date { get; set; }
        // public List<ObjectParam> Params { get; set; }
        public List<ObjectService> Service { get; set; }
    }
    
    // public class ObjectParam
    // {
    //     public string Id { get; set; }
    //     public string Name { get; set; }
    //     public long? Datatype { get; set; }
    //     public string ParamParam { get; set; }
    //     public long? Min { get; set; }
    //     public long? Max { get; set; }
    //     public long? Seq { get; set; }
    //     public long? Direction { get; set; }
    //     public List<string> Unit { get; set; }
    //     public long? Def { get; set; }
    //     public List<> List { get; set; }
    // }

    public class ObjectService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> Soff { get; set; }
        public List<int> Serviceoff { get; set; }
    }

    #endregion
}
