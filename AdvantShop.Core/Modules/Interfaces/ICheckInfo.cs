using System.Web;

namespace AdvantShop.Core.Modules.Interfaces
{
    public enum ECheckType
    {
        CheckoutUserInfo,
        Registration,
        Feedback,
        ProductReviews,
        Order,
        Other
    }

    public interface ICheckInfo
    {
        bool CheckInfo(HttpContext currentContext, ECheckType checkType, string senderEmail, string senderNickname, string message = "", string phone = "");
    }
}
