namespace AdvantShop.Web.Admin.Models.Orders.Grastin
{
    public class OrderActionsModel
    {
        public int OrderId { get; set; }
        public bool ShowSendOrderForGrasting { get; set; }
        public bool ShowSendOrderForRussianPost { get; set; }
        public bool ShowSendOrderForBoxberry { get; set; }
        public bool ShowSendOrderForHermes { get; set; }
        public bool ShowSendOrderForPartner { get; set; }
        public bool ShowSendRequestForIntake { get; set; }
        public bool ShowSendRequestForAct { get; set; }
        public bool ShowSendRequestForMark { get; set; }
        public bool ShowSendOrderForCdek { get; set; }
    }
}
