namespace AdvantShop.Shipping.NovaPoshta
{
    public class NovaResponseWarehouse
    {
        /// <summary>
        /// Идентификатор отделения
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// Код отделения
        /// </summary>
        public int SiteKey { get; set; }

        /// <summary>
        /// Название отделения на Украинском
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Название отделения на русском
        /// </summary>
        public string DescriptionRu { get; set; }

        public string ShortAddress { get; set; }
        public string ShortAddressRu { get; set; }
        public string Phone { get; set; }
        //public string TypeOfWarehouse { get; set; }
        public string Number { get; set; }
        //public string CityRef { get; set; }
        //public string CityDescription { get; set; }
        //public string CityDescriptionRu { get; set; }
        //public string SettlementRef { get; set; }
        //public string SettlementDescription { get; set; }
        //public string SettlementAreaDescription { get; set; }
        //public string SettlementRegionsDescription { get; set; }
        //public string SettlementTypeDescription { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        //public bool PostFinance { get; set; }
        //public string BicycleParking { get; set; }
        //public string PaymentAccess { get; set; }
        //public bool PosTerminal { get; set; }
        //public bool InternationalShipping { get; set; }
        //public string SelfServiceWorkplacesCount { get; set; }

        /// <summary>
        /// Максимальный вес отправления
        /// </summary>
        public float TotalMaxWeightAllowed { get; set; }
        public float PlaceMaxWeightAllowed { get; set; }

        ///// <summary>
        ///// График приема отправлений
        ///// </summary>
        //public Delivery Reception { get; set; }

        ///// <summary>
        ///// График отправки день в день
        ///// </summary>
        //public Delivery Delivery { get; set; }

        ///// <summary>
        ///// График работы
        ///// </summary>
        //public Delivery Schedule { get; set; }

        //public string DistrictCode { get; set; }
        //public string WarehouseStatus { get; set; }
        //public string WarehouseStatusDate { get; set; }
        //public string CategoryOfWarehouse { get; set; }
        //public string Direct { get; set; }
    }

    public class Delivery
    {
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
    }
}
