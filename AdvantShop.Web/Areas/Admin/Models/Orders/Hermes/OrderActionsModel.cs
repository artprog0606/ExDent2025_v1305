namespace AdvantShop.Web.Admin.Models.Orders.Hermes
{
    public class OrderActionsModel
    {
        public int OrderId { get; set; }
        /// <summary>
        /// Создание предзаказа с возвратом сопроводительных документов с доставкой от склада клиента до пункта выдачи заказов для бизнес-юнитов с услугой DIRECT_DELIVERY, DIRECT_RETURN, ACCOMPANYING_DOCUMENTS_RETURN
        /// </summary>
        public bool ShowSendParcelVSD { get; set; }

        /// <summary>
        /// Создание предзаказа с доставкой силами клиента до пункта выдачи заказов для бизнес-юнитов с услугой DROP_OFF_TO_TARGET_PARCELSHOP
        /// </summary>
        public bool ShowSendParcelDrop { get; set; }

        /// <summary>
        /// Создание стандартного предзаказа с доставкой от склада клиента до пункта выдачи заказов для бизнес-юнитов с услугой DIRECT_DELIVERY, DIRECT_RETURN
        /// </summary>
        public bool ShowSendParcelStandart { get; set; }

        public bool ShowDeleteParcel { get; set; }
    }
}
