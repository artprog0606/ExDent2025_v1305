using System;
using System.Xml.Serialization;

namespace AdvantShop.Core.Services.Shipping.RussianPost.TrackingApi
{
    [Serializable]
    [XmlRoot(ElementName = "Envelope", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    public class RequestContentBase<T, TB>
        where T : BodyBase<TB> where TB : AuthorizationHeader
    {
        public RequestContentBase()
        {
            Header = new Header();
        }

        [XmlElement(Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public Header Header { get; set; }

        [XmlElement(Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public T Body { get; set; }
    }

    public class Header { }

    public abstract class BodyBase<T> where T : AuthorizationHeader
    {
        [XmlElement(Namespace = "http://russianpost.org/operationhistory")]
        public abstract T BodyData { get; set; }
    }

    public class AuthorizationHeader
    {
        [XmlElement(ElementName = "AuthorizationHeader", Namespace = "http://russianpost.org/operationhistory/data")]
        public AuthorizationHeaderData AuthorizationHeaderData { get; set; }
    }

    public class AuthorizationHeaderData
    {
        [XmlAttribute(AttributeName = "mustUnderstand", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public int MustUnderstand
        {
            get { return 1; }
        }

        [XmlElement(ElementName = "login", Namespace = "http://russianpost.org/operationhistory/data")]
        public string Login { get; set; }

        [XmlElement(ElementName = "password", Namespace = "http://russianpost.org/operationhistory/data")]
        public string Password { get; set; }
    }

    public class GetOperationHistoryBody : BodyBase<GetOperationHistory>
    {
        //[XmlElement(ElementName = "getOperationHistory", Namespace = "http://russianpost.org/operationhistory")]
        [XmlElement(Namespace = "http://russianpost.org/operationhistory")]
        public override GetOperationHistory BodyData { get; set; }
    }

    public class GetOperationHistory : AuthorizationHeader
    {
        [XmlElement(ElementName = "OperationHistoryRequest", Namespace = "http://russianpost.org/operationhistory/data")]
        public OperationHistoryRequest OperationHistoryRequest { get; set; }
    }

    public class OperationHistoryRequest
    {
        [XmlElement(Namespace = "http://russianpost.org/operationhistory/data")]
        public string Barcode { get; set; }

        [XmlElement(Namespace = "http://russianpost.org/operationhistory/data")]
        public string MessageType { get; set; }

        [XmlElement(Namespace = "http://russianpost.org/operationhistory/data", IsNullable = true)]
        public string Language { get; set; }
    }
}
