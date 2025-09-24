using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AdvantShop.Core.Services.Shipping.Grastin.Api
{
    [Serializable]
    [XmlRoot(ElementName = "CityCdekList")]
    public class GrastinCdekCitiesResponse
    {
        [XmlElement("CityCdek")]
        public List<CityCdek> CitiesCdek { get; set; }
    }

    public class CityCdek
    {
        [XmlElement("externalid")]
        public string Id { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }
    }
}