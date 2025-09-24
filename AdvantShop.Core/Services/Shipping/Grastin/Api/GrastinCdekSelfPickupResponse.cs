using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AdvantShop.Core.Services.Shipping.Grastin.Api
{
    [Serializable]
    [XmlRoot(ElementName = "SelfpickupCdekList")]
    public class GrastinCdekSelfPickupResponse
    {
        [XmlElement("SelfpickupCdek")]
        public List<SelfpickupCdek> SelfpickupsCdek { get; set; }
    }

    [Serializable]
    public class SelfpickupCdek
    {
        [XmlElement("externalid")]
        public string Id { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("city")]
        public string City { get; set; }

        [XmlElement("schedule")]
        public string Schedule { get; set; }

        [XmlElement("drivingdescription")]
        public string DrivingDescription { get; set; }

        [XmlElement("DeliveryPeriod")]
        public string DeliveryPeriod { get; set; }

        [XmlElement("TypeOfSelfpickup")]
        public string TypeOfSelfpickup { get; set; }

        [XmlElement("latitude")]
        public float Latitude { get; set; }

        [XmlElement("longitude")]
        public float Longitude { get; set; }
    }
}