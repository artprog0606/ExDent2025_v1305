using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AdvantShop.Core.Services.Shipping.RussianPost.TrackingApi
{
    [Serializable]
    [XmlRoot(ElementName = "Envelope", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    public class GetOperationHistoryResponse
    {
        public GetOperationHistoryResponseBody Body { get; set; }
    }

    public class GetOperationHistoryResponseBody
    {
        [XmlElement(ElementName = "getOperationHistoryResponse", Namespace = "http://russianpost.org/operationhistory")]
        public GetOperationHistoryResponseInBody Response { get; set; }

        public RussianPostTrackingError Fault { get; set; }
    }

    public class RussianPostTrackingError
    {
        public ErrorCode Code { get; set; }
        public ErrorReason Reason { get; set; }
        public ErrorDetail Detail { get; set; }
    }

    public class ErrorCode
    {
        public string Value { get; set; }
    }

    public class ErrorReason
    {
        public string Text { get; set; }
    }

    public class ErrorDetail
    {
        [XmlElement(Namespace = "http://russianpost.org/operationhistory/data")]
        public string AuthorizationFaultReason { get; set; }

        [XmlElement(Namespace = "http://russianpost.org/operationhistory/data")]
        public string OperationHistoryFaultReason { get; set; }

        [XmlElement(Namespace = "http://russianpost.org/operationhistory/data")]
        public string LanguageFaultReason { get; set; }
    }

    public class GetOperationHistoryResponseInBody
    {
        [XmlElement(Namespace = "http://russianpost.org/operationhistory/data")]
        public OperationHistoryData OperationHistoryData { get; set; }
    }

    public class OperationHistoryData
    {
        [XmlElement("historyRecord")]
        public List<HistoryRecord> HistoryRecords { get; set; }
    }

    public class HistoryRecord
    {
        /// <summary>
        /// Содержит адресные данные с операцией над отправлением.
        /// </summary>
        public AddressParameters AddressParameters { get; set; }

        /// <summary>
        /// Содержит финансовые данные, связанные с операцией над почтовым отправлением.
        /// </summary>
        public FinanceParameters FinanceParameters { get; set; }

        /// <summary>
        /// Содержит данные о почтовом отправлении.
        /// </summary>
        public ItemParameters ItemParameters { get; set; }

        /// <summary>
        /// Cодержит параметры операции над отправлением
        /// </summary>
        public OperationParameters OperationParameters { get; set; }

        /// <summary>
        /// Содержит данные субъектов, связанных с операцией над почтовым отправлением.
        /// </summary>
        public UserParameters UserParameters { get; set; }
    }

    public class AddressParameters
    {
        /// <summary>
        /// Содержит данные о стране места назначения пересылки отправления.
        /// </summary>
        public Country MailDirect { get; set; }

        /// <summary>
        /// Содержит данные о стране приема почтового отправления.
        /// </summary>
        public Country CountryFrom { get; set; }

        /// <summary>
        /// Содержит данные о стране проведения операции над почтовым отправлением.
        /// </summary>
        public Country CountryOper { get; set; }

        /// <summary>
        /// Содержит адресные данные места назначения пересылки отправления.
        /// </summary>
        public Address DestinationAddress { get; set; }

        /// <summary>
        /// Содержит адресные данные места проведения операции над отправлением.
        /// </summary>
        public Address OperationAddress { get; set; }
    }

    public class Country
    {
        /// <summary>
        /// Код страны.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Двухбуквенный идентификатор страны.
        /// </summary>
        public string Code2A { get; set; }

        /// <summary>
        /// Трехбуквенный идентификатор страны
        /// </summary>
        public string Code3A { get; set; }

        /// <summary>
        /// Российское название страны
        /// </summary>
        public string NameRU { get; set; }

        /// <summary>
        /// Международное название страны
        /// </summary>
        public string NameEN { get; set; }
    }

    public class Address
    {
        /// <summary>
        /// Почтовый индекс места. Не возвращается для зарубежных операций.
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Адрес и/или название места.
        /// </summary>
        public string Description { get; set; }
    }

    public class FinanceParameters
    {
        /// <summary>
        /// Сумма наложенного платежа в копейках.
        /// </summary>
        public long? Payment { get; set; }

        /// <summary>
        /// Сумма объявленной ценности в копейках.
        /// </summary>
        public long? Value { get; set; }

        /// <summary>
        /// Общая сумма платы за пересылку наземным и воздушным транспортом в копейках.
        /// </summary>
        public long? MassRate { get; set; }

        /// <summary>
        /// Сумма платы за объявленную ценность в копейках.
        /// </summary>
        public long? InsrRate { get; set; }

        /// <summary>
        /// Выделенная сумма платы за пересылку воздушным транспортом из общей суммы платы за пересылку в копейках.
        /// </summary>
        public long? AirRate { get; set; }

        /// <summary>
        /// Сумма дополнительного тарифного сбора в копейках.
        /// </summary>
        public long? Rate { get; set; }

        /// <summary>
        /// Сумма таможенного платежа в копейках.
        /// </summary>
        public long? CustomDuty { get; set; }
    }

    public class ItemParameters
    {
        /// <summary>
        /// Идентификатор почтового отправления, текущий для данной операции.
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Служебная информация, идентифицирующая отправление, может иметь значение ДМ квитанции, связанной с отправлением или иметь значение <null>
        /// </summary>
        public string Internum { get; set; }

        /// <summary>
        /// Признак корректности вида и категории отправления для внутренней пересылки
        /// </summary>
        public bool ValidRuType { get; set; }

        /// <summary>
        /// Признак корректности вида и категории отправления для международной пересылки
        /// </summary>
        public bool ValidEnType { get; set; }

        /// <summary>
        /// Содержит текстовое описание вида и категории отправления.
        /// </summary>
        public string ComplexItemName { get; set; }

        /// <summary>
        /// Содержит информацию о разряде почтового отправления.
        /// </summary>
        public MailRank MailRank { get; set; }

        /// <summary>
        /// Содержит информацию об отметках почтовых отправлений.
        /// </summary>
        public PostMark PostMark { get; set; }

        /// <summary>
        /// Содержит данные о виде почтового отправления.
        /// </summary>
        public MailType MailType { get; set; }

        /// <summary>
        /// Содержит данные о категории почтового отправления.
        /// </summary>
        public MailCtg MailCtg { get; set; }

        /// <summary>
        /// Вес отправления в граммах.
        /// </summary>
        public int? Mass { get; set; }

        /// <summary>
        /// Значение максимально возможного веса для данного вида и категории отправления для внутренней пересылки.
        /// </summary>
        public int? MaxMassRu { get; set; }

        /// <summary>
        /// Значение максимально возможного веса для данного вида и категории отправления для международной пересылки.
        /// </summary>
        public int? MaxMassEn { get; set; }
    }

    public class MailRank
    {
        /// <summary>
        /// Код разряда почтового отправления.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название разряда почтового отправления.
        /// </summary>
        public string Name { get; set; }
    }

    public class PostMark
    {
        /// <summary>
        /// Код отметки почтового отправления.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование отметки почтового отправления.
        /// </summary>
        public string Name { get; set; }
    }

    public class MailType
    {
        /// <summary>
        /// Код вида почтового отправления.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название вида почтового отправления.
        /// </summary>
        public string Name { get; set; }
    }

    public class MailCtg
    {
        /// <summary>
        /// Код категории почтового отправления.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название категории почтового отправления.
        /// </summary>
        public string Name { get; set; }
    }

    public class OperationParameters
    {
        /// <summary>
        /// Содержит информацию об операции над отправлением.
        /// </summary>
        public OperType OperType { get; set; }

        /// <summary>
        /// Содержит информацию об атрибуте операции над отправлением.
        /// </summary>
        public OperAttr OperAttr { get; set; }

        /// <summary>
        /// Содержит данные о дате и времени проведения операции над отправлением.
        /// </summary>
        public DateTime OperDate { get; set; }
    }

    public class OperType
    {
        /// <summary>
        /// Код операции.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название операции.
        /// </summary>
        public string Name { get; set; }
    }

    public class OperAttr
    {
        /// <summary>
        /// Код атрибута.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название атрибута.
        /// </summary>
        public string Name { get; set; }
    }

    public class UserParameters
    {
        /// <summary>
        /// Содержит информацию о категории отправителя.
        /// </summary>
        public SendCtg SendCtg { get; set; }

        /// <summary>
        /// Содержит данные об отправителе.
        /// </summary>
        public string Sndr { get; set; }

        /// <summary>
        /// Содержит данные о получателе отправления.
        /// </summary>
        public string Rcpn { get; set; }
    }

    public class SendCtg
    {
        /// <summary>
        /// Идентификатор категории отправителя.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название категории отправителя.
        /// </summary>
        public string Name { get; set; }
    }
}
