using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Crm.SalesFunnels
{
    public enum ELeadAutoCompleteActionType
    {
        [Localize("Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.None")]
        None = 0,
        [Localize("Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.OrderCreated")]
        [DescriptionKey("Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.Description.OrderCreated")]
        OrderCreated = 1,
        [Localize("Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.OrderPaid")]
        [DescriptionKey("Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.Description.OrderPaid")]
        OrderPaid = 2,
        [Localize("Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.BookingCreated")]
        [DescriptionKey("Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.Description.BookingCreated")]
        BookingCreated = 3
    }

    /// <summary>
    /// Действие при успешном завершении лида
    /// </summary>
    public enum SalesFunnelFinalSuccessAction
    {
        /// <summary>
        /// Создать заказ
        /// </summary>
        [Localize("Core.Crm.SalesFunnels.SalesFunnelFinalSuccessAction.CreateOrder")]
        CreateOrder = 0,

        /// <summary>
        /// Ничего не делать
        /// </summary>
        [Localize("Core.Crm.SalesFunnels.SalesFunnelFinalSuccessAction.None")]
        None = 1,
    }

    /// <summary>
    /// Воронка продаж
    /// </summary>
    public class SalesFunnel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }

        public bool Enable { get; set; }

        /// <summary>
        /// Не отправлять уведомления при создании нового лида
        /// </summary>
        public bool NotSendNotificationsOnLeadCreation { get; set; }

        /// <summary>
        /// Не отправлять уведомления при изменении лида
        /// </summary>
        public bool NotSendNotificationsOnLeadChanged { get; set; }

        public SalesFunnelFinalSuccessAction FinalSuccessAction { get; set; }

        public ELeadAutoCompleteActionType LeadAutoCompleteActionType { get; set; }
    }
}
