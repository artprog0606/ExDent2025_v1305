using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AdvantShop.Shipping.Grastin;

namespace AdvantShop.Core.Services.Shipping.Grastin.Api
{
    [Serializable]
    [XmlRoot(ElementName = "SelfpickupList")]
    public class GrastinSelfPickupResponse
    {
        [XmlElement("Selfpickup")]
        public List<Selfpickup> Selfpickups { get; set; }
    }

    [Serializable]
    public class Selfpickup : ISelfpickupGrastin
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("timetable")]
        public string TimeTable { get; set; }

        [XmlElement("linkdrivingdescription")]
        public string LinkDrivingDescription { get; set; }

        [XmlElement("drivingdescription")]
        public string DrivingDescription { get; set; }

        [XmlElement("phone")]
        public string Phone { get; set; }

        [XmlElement("city")]
        public string City { get; set; }

        [XmlElement("metrostation")]
        public string Metrostation { get; set; }

        [XmlElement("address")]
        public string Address { get; set; }

        [XmlElement("deliveryregion")]
        public string DeliveryContract { get; set; }

        [XmlElement("paymentcard")]
        public bool AcceptsPaymentCards { get; set; }

        [XmlElement("regional")]
        public bool RegionalPoint { get; set; }

        [XmlElement("largesize")]
        public bool IssuesLargeSize { get; set; }

        [XmlElement("onlylargesize")]
        public bool IssuesOnlyLargeSize { get; set; }

        [XmlElement("dressingroom")]
        public bool DressingRoom { get; set; }

        [XmlElement("latitude")]
        public float Latitude { get; set; }

        [XmlElement("longitude")]
        public float Longitude { get; set; }

        [XmlIgnore]
        public EnPartner TypePoint
        {
            get { return EnPartner.Grastin; }
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Name) ? Id : Name;
        }
    }
}
