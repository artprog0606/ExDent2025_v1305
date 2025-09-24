using System;
using System.Globalization;
using System.Xml.Serialization;

namespace AdvantShop.Core.Services.Shipping.Grastin.Api
{
    [Serializable]
    public class GrastinProduct
    {
        /// <summary>
        /// Артикул товара
        /// </summary>
        [XmlAttribute("article")]
        public string ArtNo { get; set; }

        /// <summary>
        /// Наименование товара
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Стоимость товара
        /// </summary>
        [XmlAttribute("cost")]
        public float Price { get; set; }

        /// <summary>
        /// Количество товара
        /// </summary>
        [XmlAttribute("amount")]
        public float Amount { get; set; }

        /// <summary>
        /// НДС на товар
        /// </summary>
        [XmlIgnore]
        public int? Vat { get; set; }
        
        [XmlAttribute("vat")]
        public string VatForXml
        {
            get { return Vat?.ToString("F2", CultureInfo.InvariantCulture); }
            set { }
        }
    }
}
