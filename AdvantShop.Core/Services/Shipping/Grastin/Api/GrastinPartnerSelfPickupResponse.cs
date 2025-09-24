using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AdvantShop.Shipping.Grastin;

namespace AdvantShop.Core.Services.Shipping.Grastin.Api
{
    [Serializable]
    [XmlRoot(ElementName = "SelfpickupPartnerList")]
    public class GrastinPartnerSelfPickupResponse
    {
        [XmlElement("SelfpickupPartner")]
        public List<SelfpickupPartner> SelfpickupsPartner { get; set; }
    }

    [Serializable]
    public class SelfpickupPartner : ISelfpickupGrastin
    {
        [XmlElement("Id")]
        public string Id { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        public string TimeTable { get; set; }

        [XmlElement("city")]
        public string City { get; set; }

        [XmlElement("drivingdescription")]
        public string DrivingDescription { get; set; }

        [XmlElement("linkdrivingdescription")]
        public string LinkDrivingDescription { get; set; }

        [XmlElement("paymentcard")]
        public bool AcceptsPaymentCards { get; set; }

        [XmlElement("regional")]
        public bool RegionalPoint { get; set; }

        [XmlElement("phone")]
        public string Phone { get; set; }

        [XmlElement("addres")]
        public string Address { get; set; }

        [XmlElement("metrostation")]
        public string Metrostation { get; set; }

        [XmlElement("latitude")]
        public float Latitude { get; set; }

        [XmlElement("longitude")]
        public float Longitude { get; set; }
        
        [XmlElement("weightMax")]
        public float MaxWeight { get; set; }
        
        [XmlElement("sumSidesMax")]
        public float MaxDimensions { get; set; }

        [XmlIgnore]
        public EnPartner TypePoint
        {
            get { return EnPartner.Partner; }
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Address) ? Name : Address;
        }
    }
}
