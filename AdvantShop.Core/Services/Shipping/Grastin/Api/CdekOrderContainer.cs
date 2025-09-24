using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace AdvantShop.Core.Services.Shipping.Grastin.Api
{
    [Serializable]
    [XmlRoot(ElementName = "File")]
    public class CdekOrderContainer : GrastinBaseObject
    {
        [XmlElement("Method")]
        public override EnApiMethod Method
        {
            get { return EnApiMethod.NewOrderCdek; }
            set { throw new NotSupportedException("Setting the Method property is not supported"); }
        }

        [XmlArray("Orders")]
        [XmlArrayItem("Order")]
        public List<CdekOrder> Orders { get; set; }
    }

    public class CdekOrder
    {
        /// <summary>
        /// Номер заказа в Вашей системе с префиксом
        /// </summary>
        [XmlAttribute("number")]
        public string Number { get; set; }

        /// <summary>
        /// ФИО покупателя
        /// </summary>
        [XmlAttribute("buyer")]
        public string Buyer { get; set; }

        /// <summary>
        /// Номер мобильного телефона
        /// </summary>
        [XmlAttribute("phone1")]
        public string Phone { get; set; }

        /// <summary>
        /// Дополнительный номер телефона получателя
        /// </summary>
        [XmlAttribute("phone2")]
        public string Phone2 { get; set; }

        /// <summary>
        /// Адрес электронной почты
        /// </summary>
        [XmlAttribute("email")]
        public string Email { get; set; }

        /// <summary>
        /// Адрес доставки
        /// <para>Если курьерская доставка</para>
        /// </summary>
        [XmlAttribute("address")]
        public string Address { get; set; }

        /// <summary>
        /// Сумма заказа
        /// </summary>
        [XmlIgnore]
        public float OrderSum { get; set; }

        [XmlAttribute("summa")]
        public string OrderSumForXml
        {
            get { return OrderSum.ToString("F2", CultureInfo.InvariantCulture); }
            set { }
        }

        /// <summary>
        /// Оценочная стоимость заказа
        /// </summary>
        [XmlIgnore]
        public float AssessedCost { get; set; }

        [XmlAttribute("assessedsumma")]
        public string AssessedCostForXml
        {
            get { return AssessedCost.ToString("F2", CultureInfo.InvariantCulture); }
            set { }
        }

        /// <summary>
        /// Стоимость доставки для покупателя
        /// </summary>
        [XmlAttribute("costdelivery")]
        public float CostDelivery { get; set; }

        /// <summary>
        /// Количество мест
        /// </summary>
        [XmlAttribute("seats")]
        public int Seats { get; set; }

        /// <summary>
        /// Склад приёма заказа
        /// </summary>
        [XmlAttribute("takewarehouse")]
        public string TakeWarehouse { get; set; }

        /// <summary>
        /// Вид груза
        /// </summary>
        [XmlAttribute("cargotype")]
        public string CargoType { get; set; }

        /// <summary>
        /// Наименование сайта для вывода в маршрутный лист курьера
        /// </summary>
        [XmlAttribute("sitename")]
        public string SiteName { get; set; }

        /// <summary>
        /// ID пункта самовывоза
        /// </summary>
        [XmlAttribute("pickup")]
        public string PickupId { get; set; }

        /// <summary>
        /// ID города доставки
        /// </summary>
        [XmlAttribute("city")]
        public string CityId { get; set; }

        [XmlElement("good")]
        public List<GrastinProduct> Products { get; set; }
    }
}