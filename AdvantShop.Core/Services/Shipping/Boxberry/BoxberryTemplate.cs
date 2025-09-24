//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Shipping.Boxberry
{
    public class BoxberryTemplate : DefaultCargoParams
    {
        public const string ApiUrl = "ApiUrl";
        public const string Token = "token";
        public const string IntegrationToken = "integrationToken";
        public const string ReceptionPointCode = "receptionPointCode";
        public const string ReceptionPointCity = "receptionPointCity";
        public const string CalculateCourier = "calculatecourier";
        public const string TypeOption = "TypeOption";
        public const string YaMapsApiKey = "YaMapsApiKey";
        public const string DeliveryTypes = "DeliveryTypes";

        public const string StatusesSync = "StatusesSync";
        public const string StatusesReference = "StatusesReference";
        public const string Status_Created = "Created";
        public const string Status_AcceptedForDelivery = "AcceptedForDelivery";
        public const string Status_SentToSorting = "SentToSorting";
        public const string Status_TransferredToSorting = "TransferredToSorting";
        public const string Status_SentToDestinationCity = "SentToDestinationCity";
        public const string Status_Courier = "Courier";
        public const string Status_PickupPoint = "PickupPoint";
        public const string Status_Delivered = "Delivered";
        public const string Status_ReturnPreparing = "ReturnPreparing";
        public const string Status_ReturnSentToReceivingPoint = "ReturnSentToReceivingPoint";
        public const string Status_ReturnReturnedToReceivingPoint = "ReturnReturnedToReceivingPoint";
        public const string Status_ReturnByCourier = "ReturnByCourier";
        public const string Status_ReturnReturned = "ReturnReturned";
        public const string WithInsure = "WithInsure";
    }
}