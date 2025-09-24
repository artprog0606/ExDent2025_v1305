//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Mails
{
    public enum MailType
    {
        None,
        OnRegistration = 1,
        OnPwdRepair = 2,
        OnNewOrder = 3,
        OnChangeOrderStatus = 4,
        OnFeedback = 8,
        OnProductDiscuss = 11,
        OnOrderByRequest = 12,
        OnSendLinkByRequest = 13,
        OnSendFailureByRequest = 14,
        OnSendGiftCertificate = 15,
        OnBuyInOneClick = 16,
        OnBillingLink = 17,
        OnSetOrderManager = 18,
        OnLead = 21,
        OnLeadChanged = 37,
        OnLeadAssigned = 36,
        OnProductDiscussAnswer = 22,
        OnChangeUserComment = 23,
        OnPayOrder = 24,
        OnTaskChanged = 25,
        OnTaskCreated = 26,
        OnTaskAssigned = 34,
        OnTaskDeleted = 27,
        OnTaskCommentAdded = 28,
        OnSendToCustomer = 29,
        OnUserRegistered = 30,
        OnUserPasswordRepair = 31,
        OnOrderCommentAdded = 32,
        OnCustomerCommentAdded = 33,
        OnBookingCreated = 35,
        OnBookingCommentAdded = 38,
        OnLeadCommentAdded = 39,
        OnPartnerRegistration = 40,
        OnPartnerCustomerBinded = 41,
        OnPartnerMoneyAdded = 42,
        OnPartnerMonthReport = 43,
        OnPartnerLegalEntityActReport = 44,
        OnPartnerNaturalPersonActReport = 45,
        OnTaskReminder = 46,
        OnMissedCall = 47,
        OnPreOrder = 48,
        OnTaskObserverAdded = 49,
        OnNewOrderReview = 50
    }
}