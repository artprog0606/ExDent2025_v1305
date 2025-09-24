//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Letters;
using AdvantShop.Orders;

namespace AdvantShop.Mails
{
    public class EmptyMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.None;

        public EmptyMailTemplate()
        {
            IsBuilt = true;
        }
    }

    public class CustomerMailTemplate : MailTemplate
    {
        private readonly LetterBuilder _customerLetterBuilder;
        public override MailType Type => MailType.None;

        public CustomerMailTemplate() { }

        public CustomerMailTemplate(Customer customer)
        {
            _customerLetterBuilder =
                new LetterBuilder()
                    .Add(new CustomerLetterBuilder(customer))
                    .Add(new CustomerFieldsLetterBuilder(customer));
        }

        protected override string FormatString(string text)
        {
            return _customerLetterBuilder.FormatText(text);
        }

        protected override Dictionary<string, string> GetFormattedParams(string text)
        {
            return _customerLetterBuilder.GetFormattedParams(text);
        }

        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return new LetterBuilder()
                .Add(new CustomerLetterBuilder(null))
                .Add(new CustomerFieldsLetterBuilder(null))
                .GetKeyDescriptions();
        }
    }

    [MailTemplateType(MailType.OnRegistration)]
    public sealed class RegistrationMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnRegistration;
        
        private readonly LetterBuilder _letterBuilder;

        private RegistrationMailTemplate() { }

        public RegistrationMailTemplate(Customer customer)
        {
            _letterBuilder =
                new LetterBuilder()
                    .Add(new CustomerLetterBuilder(customer))
                    .Add(new CustomerRegistrationLetterBuilder(customer))
                    .Add(new CustomerFieldsLetterBuilder(customer));
        }

        protected override string FormatString(string text)
        {
            return _letterBuilder.FormatText(text);
        }

        protected override Dictionary<string, string> GetFormattedParams(string text)
        {
            return _letterBuilder.GetFormattedParams(text);
        }

        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return new LetterBuilder()
                .Add(new CustomerLetterBuilder(null))
                .Add(new CustomerRegistrationLetterBuilder(null))
                .Add(new CustomerFieldsLetterBuilder(null))
                .GetKeyDescriptions();
        }
    }

    public class PwdRepairMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnPwdRepair;

        private readonly string _recoveryCode;
        private readonly string _email;
        private readonly string _link;

        public PwdRepairMailTemplate(string recoveryCode, string email, string link)
        {
            _recoveryCode = recoveryCode;
            _email = email;
            _link = link;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#EMAIL#", _email);
            sb.Replace("#RECOVERYCODE#", _recoveryCode);
            sb.Replace("#LINK#", _link);
            
            return sb.ToString();
        }
    }

    [MailTemplateType(MailType.OnNewOrder)]
    public sealed class NewOrderMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnNewOrder;
        
        private readonly OrderLetterBuilder _orderLetterBuilder;
        
        private NewOrderMailTemplate() { }

        private NewOrderMailTemplate(OrderLetterBuilder orderLetterBuilder)
        {
            _orderLetterBuilder = orderLetterBuilder;
        }
        
        public static NewOrderMailTemplate Create(Order order, float? bonusPlus = null)
        {
            return Create(order, order.TotalDiscount, bonusPlus, order.ArchivedShippingName);
        }
        
        public static NewOrderMailTemplate Create(Order order, float totalDiscount, float? bonusPlus, string shippingName)
        {
            return new NewOrderMailTemplate(new OrderLetterBuilder(order, totalDiscount, bonusPlus, shippingName, null));
        }

        public static NewOrderMailTemplate CreateForCertificate(Order order)
        {
            return new NewOrderMailTemplate(new OrderLetterBuilder(order, true, null));
        }

        protected override string FormatString(string text)
        {
            return _orderLetterBuilder.FormatText(text);
        }

        protected override Dictionary<string, string> GetFormattedParams(string text)
        {
            return _orderLetterBuilder.GetFormattedParams(text);
        }

        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return new OrderLetterBuilder(null).GetKeyDescriptions();
        }
    }

    [MailTemplateType(MailType.OnChangeOrderStatus)]
    public sealed class OrderStatusMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnChangeOrderStatus;
        
        private readonly OrderLetterBuilder _orderLetterBuilder;

        private OrderStatusMailTemplate() { }
        
        public OrderStatusMailTemplate(Order order)
        {
            _orderLetterBuilder = new OrderLetterBuilder(order);
        }

        protected override string FormatString(string text)
        {
            return _orderLetterBuilder.FormatText(text);
        }
        
        protected override Dictionary<string, string> GetFormattedParams(string text)
        {
            return _orderLetterBuilder.GetFormattedParams(text);
        }
        
        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return new OrderLetterBuilder(null).GetKeyDescriptions();
        }
    }

    public sealed class FeedbackMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnFeedback;

        private readonly string _shopUrl;
        private readonly string _shopName;
        private readonly string _userName;
        private readonly string _userEmail;
        private readonly string _userPhone;
        private readonly string _subjectMessage;
        private readonly string _textMessage;
        private readonly string _orderNumber;

        public FeedbackMailTemplate(string shopUrl, string shopName, string userName, string userEmail,
                                    string userPhone, string subjectMessage, string textMessage, string orderNumber)
        {
            _shopUrl = shopUrl;
            _shopName = shopName;
            _userName = userName;
            _userEmail = userEmail;
            _userPhone = userPhone;
            _subjectMessage = subjectMessage;
            _textMessage = textMessage;
            _orderNumber = orderNumber;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#SHOPURL#", _shopUrl);
            sb.Replace("#STORE_NAME#", _shopName);
            sb.Replace("#USERNAME#", _userName);
            sb.Replace("#USEREMAIL#", _userEmail);
            sb.Replace("#USERPHONE#", _userPhone);
            sb.Replace("#SUBJECTMESSAGE#", _subjectMessage);
            sb.Replace("#TEXTMESSAGE#", _textMessage);
            sb.Replace("#ORDERNUMBER#", _orderNumber);
            
            return sb.ToString();
        }
    }

    public sealed class ProductDiscussMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnProductDiscuss;

        private readonly string _sku;
        private readonly string _productName;
        private readonly string _productLink;
        private readonly string _author;
        private readonly string _date;
        private readonly string _text;
        private readonly string _deleteLink;
        private readonly string _email;

        public ProductDiscussMailTemplate(string sku, string productName, string productLink, string author, string date,
                                          string text, string deleteLink, string email)
        {
            _sku = sku;
            _productName = productName;
            _productLink = productLink;
            _author = author;
            _date = date;
            _text = text;
            _deleteLink = deleteLink;
            _email = email;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#PRODUCTNAME#", _productName);
            sb.Replace("#PRODUCTLINK#", _productLink);
            sb.Replace("#USERMAIL#", _email);
            sb.Replace("#SKU#", _sku);
            sb.Replace("#AUTHOR#", _author);
            sb.Replace("#DATE#", _date);
            sb.Replace("#DELETELINK#", _deleteLink);
            sb.Replace("#TEXT#", _text);
            
            return sb.ToString();
        }
    }

    public sealed class ProductDiscussAnswerMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnProductDiscussAnswer;

        private readonly string _sku;
        private readonly string _productName;
        private readonly string _productLink;
        private readonly string _author;
        private readonly string _date;
        private readonly string _previousMsgText;
        private readonly string _answerText;

        public ProductDiscussAnswerMailTemplate(string sku, string productName, string productLink, string author, string date,
                                          string previousMsgText, string answerText)
        {
            _sku = sku;
            _productName = productName;
            _productLink = productLink;
            _author = author;
            _date = date;
            _previousMsgText = previousMsgText;
            _answerText = answerText;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#PRODUCTNAME#", _productName);
            sb.Replace("#PRODUCTLINK#", _productLink);
            sb.Replace("#SKU#", _sku);
            sb.Replace("#AUTHOR#", _author);
            sb.Replace("#DATE#", _date);
            sb.Replace("#ANSWER_TEXT#", _answerText);
            sb.Replace("#PREVIOUS_MSG_TEXT#", _previousMsgText);
            
            return sb.ToString();
        }
    }

    public sealed class OrderByRequestMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnOrderByRequest;

        private readonly string _orderByRequestId;
        private readonly string _artNo;
        private readonly string _productName;
        private readonly string _quantity;
        private readonly string _userName;
        private readonly string _email;
        private readonly string _phone;
        private readonly string _comment;

        private readonly string _color;
        private readonly string _size;
        private readonly string _options;

        public OrderByRequestMailTemplate(string orderByRequestId, string artNo, string productName, string quantity,
                                          string userName, string email, string phone, string comment, string color,
                                          string size, string options)
        {
            _orderByRequestId = orderByRequestId;
            _artNo = artNo;
            _productName = productName;
            _quantity = quantity;
            _userName = userName;
            _email = email;
            _phone = phone;
            _comment = comment;
            _color = color;
            _size = size;
            _options = options;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#ORDERID#", _orderByRequestId);
            sb.Replace("#ARTNO#", _artNo);
            sb.Replace("#PRODUCTNAME#", _productName);
            sb.Replace("#QUANTITY#", _quantity);
            sb.Replace("#USERNAME#", _userName);
            sb.Replace("#EMAIL#", _email);
            sb.Replace("#PHONE#", _phone);
            sb.Replace("#COMMENT#", _comment);

            sb.Replace("#COLOR#", _color);
            sb.Replace("#SIZE#", _size);
            sb.Replace("#OPTIONS#", _options);
            
            return sb.ToString();
        }
    }

    public sealed class LinkByRequestMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnSendLinkByRequest;

        private readonly string _orderByRequestId;
        private readonly string _userName;
        private readonly string _artNo;
        private readonly string _productName;
        private readonly string _quantity;
        private readonly string _code;
        private readonly string _color;
        private readonly string _size;
        private readonly string _comment;

        public LinkByRequestMailTemplate(string orderByRequestId, string artNo, string productName, string quantity,
                                             string code, string userName, string comment, string color, string size)
        {
            _orderByRequestId = orderByRequestId;
            _artNo = artNo;
            _productName = productName;
            _quantity = quantity;
            _userName = userName;
            _comment = comment;
            _color = color;
            _size = size;
            _code = code;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#NUMBER#", _orderByRequestId);
            sb.Replace("#USERNAME#", _userName);
            sb.Replace("#ARTNO#", _artNo);
            sb.Replace("#PRODUCTNAME#", _productName);
            sb.Replace("#QUANTITY#", _quantity);
            sb.Replace("#LINK#", SettingsMain.SiteUrl + "/preorder/linkbycode?code=" + _code);

            sb.Replace("#COLOR#", _color);
            sb.Replace("#SIZE#", _size);

            sb.Replace("#COMMENT#", _comment);

            return sb.ToString();
        }
    }

    public sealed class FailureByRequestMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnSendFailureByRequest;

        private readonly string _orderByRequestId;
        private readonly string _userName;
        private readonly string _artNo;
        private readonly string _productName;
        private readonly string _quantity;
        private readonly string _color;
        private readonly string _size;
        private readonly string _comment;

        public FailureByRequestMailTemplate(string orderByRequestId, string artNo, string productName,
                                                string quantity, string userName, string comment, string color, string size)
        {
            _orderByRequestId = orderByRequestId;
            _artNo = artNo;
            _productName = productName;
            _quantity = quantity;
            _userName = userName;
            _comment = comment;
            _color = color;
            _size = size;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#NUMBER#", _orderByRequestId);
            sb.Replace("#USERNAME#", _userName);
            sb.Replace("#ARTNO#", _artNo);
            sb.Replace("#PRODUCTNAME#", _productName);
            sb.Replace("#QUANTITY#", _quantity);

            sb.Replace("#COLOR#", _color);
            sb.Replace("#SIZE#", _size);
            sb.Replace("#COMMENT#", _comment);

            return sb.ToString();
        }
    }

    public sealed class CertificateMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnSendGiftCertificate;

        private readonly string _certificateCode;
        private readonly string _fromName;
        private readonly string _toName;
        private readonly string _sum;
        private readonly string _message;

        public CertificateMailTemplate(string certificateCode, string fromName, string toName, string sum,
                                       string message)
        {
            _certificateCode = certificateCode;
            _fromName = fromName;
            _toName = toName;
            _sum = sum;
            _message = message;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#CODE#", _certificateCode);
            sb.Replace("#FROMNAME#", _fromName);
            sb.Replace("#TONAME#", _toName);
            sb.Replace("#LINK#", StringHelper.ToPuny(SettingsMain.SiteUrl));
            sb.Replace("#SUM#", _sum);
            sb.Replace("#MESSAGE#", _message);

            return sb.ToString();
        }
    }

    [MailTemplateType(MailType.OnBuyInOneClick)]
    public class BuyInOneClickMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnBuyInOneClick;
        
        private readonly OrderLetterBuilder _orderLetterBuilder;

        protected BuyInOneClickMailTemplate() { }
        
        public BuyInOneClickMailTemplate(Order order)
        {
            _orderLetterBuilder = new OrderLetterBuilder(order);
        }

        protected override string FormatString(string text)
        {
            return _orderLetterBuilder.FormatText(text);
        }

        protected override Dictionary<string, string> GetFormattedParams(string text)
        {
            return _orderLetterBuilder.GetFormattedParams(text);
        }

        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return new OrderLetterBuilder(null).GetKeyDescriptions();
        }
    }

    [MailTemplateType(MailType.OnPreOrder)]
    public sealed class PreorderMailTemplate : BuyInOneClickMailTemplate
    {
        public override MailType Type => MailType.OnPreOrder;
        
        private PreorderMailTemplate() { }

        public PreorderMailTemplate(Order order) : base(order)
        {
        }
    }

    [MailTemplateType(MailType.OnBillingLink)]
    public sealed class BillingLinkMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnBillingLink;
        
        private readonly OrderLetterBuilder _orderLetterBuilder;

        public BillingLinkMailTemplate() { }

        public BillingLinkMailTemplate(Order order)
        {
            _orderLetterBuilder = order.OrderCertificates.Count > 0 
                ? new OrderLetterBuilder(order, true, null) 
                : new OrderLetterBuilder(order);
        }

        protected override string FormatString(string text)
        {
            return _orderLetterBuilder.FormatText(text);
        }

        protected override Dictionary<string, string> GetFormattedParams(string text)
        {
            return _orderLetterBuilder.GetFormattedParams(text);
        }

        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return new OrderLetterBuilder(null).GetKeyDescriptions();
        }
    }

    public sealed class SetOrderManagerMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnSetOrderManager;

        private readonly string _managerName;
        private readonly int _orderId;
        private readonly string _number;

        public SetOrderManagerMailTemplate(string managerName, int orderId, string number)
        {
            _managerName = managerName;
            _orderId = orderId;
            _number = number;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#SHOPURL#", SettingsMain.SiteUrl);
            sb.Replace("#STORE_NAME#", SettingsMain.ShopName);
            sb.Replace("#MANAGER_NAME#", _managerName);
            sb.Replace("#ORDER_ID#", _orderId.ToString());
            sb.Replace("#NUMBER#", _number);
            sb.Replace("#ORDER_URL#", UrlService.GetAdminUrl("orders/edit/" + _orderId));
            
            return sb.ToString();
        }
    }

    public sealed class ChangeUserCommentTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnChangeUserComment;

        private readonly string _orderId;
        private readonly string _orderUserComment;
        private readonly string _number;
        private readonly string _managerName;


        public ChangeUserCommentTemplate(string orderId, string orderUserComment, string number, string managerName)
        {
            _orderId = orderId;
            _orderUserComment = orderUserComment;
            _number = number;
            _managerName = managerName;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#ORDER_ID#", _orderId);
            sb.Replace("#ORDER_USER_COMMENT#", _orderUserComment);
            sb.Replace("#NUMBER#", _number);
            sb.Replace("#STORE_NAME#", SettingsMain.ShopName);
            sb.Replace("#MANAGER_NAME#", _managerName);

            return sb.ToString();
        }
    }

    [MailTemplateType(MailType.OnPayOrder)]
    public sealed class PayOrderTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnPayOrder;
        
        private readonly OrderLetterBuilder _orderLetterBuilder;
        
        private PayOrderTemplate() { }

        public PayOrderTemplate(Order order)
        {
            _orderLetterBuilder = new OrderLetterBuilder(order);
        }

        protected override string FormatString(string text)
        {
            return _orderLetterBuilder.FormatText(text);
        }

        protected override Dictionary<string, string> GetFormattedParams(string text)
        {
            return _orderLetterBuilder.GetFormattedParams(text);
        }

        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return new OrderLetterBuilder(null).GetKeyDescriptions();
        }
    }

    public sealed class SendToCustomerTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnSendToCustomer;

        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _patronymic;
        private readonly string _text;
        private readonly string _trackNumber;
        private readonly string _managerName;
        private readonly int? _orderId;
        private readonly int? _leadId;

        public SendToCustomerTemplate(string firstName, string lastName, string patronymic, string text, string trackNumber, 
                                        string managerName, int? orderId, int? leadId)
        {
            _firstName = firstName;
            _lastName = lastName;
            _patronymic = patronymic;
            _text = text;
            _trackNumber = trackNumber;
            _managerName = managerName;
            _orderId = orderId;
            _leadId = leadId;
        }

        protected override string FormatString(string text)
        {
            var format = new StringBuilder(text);
            
            format.Replace("#TEXT#", _text)
                .Replace("#FIRSTNAME#", _firstName)
                .Replace("#LASTNAME#", _lastName)
                .Replace("#PATRONYMIC#", _patronymic)
                .Replace("#STORE_NAME#", SettingsMain.ShopName)
                .Replace("#MANAGER_NAME#", _managerName);

            if (!string.IsNullOrEmpty(_trackNumber))
                format.Replace("#TRACKNUMBER#", _trackNumber)
                      .Replace("#ORDER_TRACK_NUMBER#", _trackNumber);
            
            var order = _orderId != null && _orderId != 0 
                ? OrderService.GetOrder(_orderId.Value) 
                : null;
            
            OrderMailService.FormatOrderLetter(order, format, text);
            
            var lead = _leadId != null && _leadId != 0 
                ? LeadService.GetLead(_leadId.Value) 
                : null;
            
            MailFormatService.FormatLead(lead, format, text);
            
            return format.ToString();
        }

        protected override Dictionary<string, string> GetFormattedParams(string text)
        {
            var dict = new Dictionary<string, string>()
            {
                {"#TEXT#", _text},
                {"#FIRSTNAME#", _firstName},
                {"#LASTNAME#", _lastName},
                {"#PATRONYMIC#", _patronymic},
                {"#STORE_NAME#", SettingsMain.ShopName},
                {"#MANAGER_NAME#", _managerName},
            };
            
            var order = _orderId != null && _orderId != 0 
                ? OrderService.GetOrder(_orderId.Value) 
                : null;

            if (order != null)
            {
                var orderParams = new OrderLetterBuilder(order).GetFormattedParams(text);
                
                foreach (var orderParam in orderParams)
                    if (!dict.ContainsKey(orderParam.Key))
                        dict.Add(orderParam.Key, orderParam.Value);
            }
            
            var lead = _leadId != null && _leadId != 0 
                ? LeadService.GetLead(_leadId.Value) 
                : null;
                
            if (lead != null)
            {
                var leadParams = new LeadLetterBuilder(lead).GetFormattedParams(text);
                
                foreach (var leadParam in leadParams)
                    if (!dict.ContainsKey(leadParam.Key))
                        dict.Add(leadParam.Key, leadParam.Value);
            }

            return dict;
        }
    }

    public sealed class UserRegisteredMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnUserRegistered;

        private readonly string _email;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _regDate;
        private readonly string _hash;

        public UserRegisteredMailTemplate(string email, string firstName, string lastName, string regDate, string hash)
        {
            _email = email;
            _firstName = firstName;
            _lastName = lastName;
            _regDate = regDate;
            _hash = hash;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#EMAIL#", _email);
            sb.Replace("#FIRSTNAME#", _firstName);
            sb.Replace("#LASTNAME#", _lastName);
            sb.Replace("#REGDATE#", _regDate);
            sb.Replace("#STORE_NAME#", SettingsMain.ShopName);
            sb.Replace("#STORE_URL#", SettingsMain.SiteUrl);
            sb.Replace("#LINK#", UrlService.GetAdminUrl("account/setpassword?email=" + _email + "&hash=" + _hash));
            
            return sb.ToString();
        }
    }

    public sealed class UserPasswordRepairMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnUserPasswordRepair;

        private readonly string _email;
        private readonly string _hash;

        public UserPasswordRepairMailTemplate(string email, string hash)
        {
            _email = email;
            _hash = hash;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#EMAIL#", _email);
            sb.Replace("#STORE_NAME#", SettingsMain.ShopName);
            sb.Replace("#STORE_URL#", SettingsMain.SiteUrl);
            sb.Replace("#LINK#", UrlService.GetAdminUrl("account/forgotpassword?email=" + _email + "&hash=" + _hash));
            
            return sb.ToString();
        }
    }

    public sealed class OrderCommentAddedMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnOrderCommentAdded;

        private readonly string _author;
        private readonly string _comment;
        private readonly string _orderLink;
        private readonly string _orderNumber;

        public OrderCommentAddedMailTemplate(string author, string comment, string orderId, string orderNumber)
        {
            _author = author;
            _comment = comment;
            _orderLink = orderId.IsNotEmpty() ? UrlService.GetAdminUrl("orders/edit/" + orderId) : string.Empty;
            _orderNumber = orderNumber;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#AUTHOR#", _author);
            sb.Replace("#COMMENT#", _comment);
            sb.Replace("#ORDER_LINK#", _orderLink);
            sb.Replace("#ORDER_NUMBER#", _orderNumber);
            
            return sb.ToString();
        }
    }

    public sealed class CustomerCommentAddedMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnCustomerCommentAdded;

        private readonly string _author;
        private readonly string _comment;
        private readonly string _customerLink;
        private readonly string _customerName;

        public CustomerCommentAddedMailTemplate(string author, string comment, string customerId, string customerName)
        {
            _author = author;
            _comment = comment;
            _customerLink = customerId.IsNotEmpty() ? UrlService.GetAdminUrl("customers/view/" + customerId) : string.Empty;
            _customerName = customerName;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#AUTHOR#", _author);
            sb.Replace("#COMMENT#", _comment);
            sb.Replace("#CUSTOMER_LINK#", _customerLink);
            sb.Replace("#CUSTOMER#", _customerName);
            
            return sb.ToString();
        }
    }

    public sealed class MissedCallMailTemplate : MailTemplate
    {
        private readonly string _phone;

        public override MailType Type => MailType.OnMissedCall;

        public MissedCallMailTemplate(string phone)
        {
            _phone = phone;
        }

        protected override string FormatString(string text)
        {
            var textFormatted = text.Replace("#PHONE#", _phone);
            return textFormatted;
        }
    }

    public sealed class NewOrderReviewMailTemplate : MailTemplate
    {
        public override MailType Type => MailType.OnNewOrderReview;
        private readonly OrderReview _orderReview;
        private readonly string _orderLink;
        private readonly string _orderNumber;

        public NewOrderReviewMailTemplate(OrderReview orderReview, string orderNumber)
        {
            _orderReview = orderReview;
            _orderLink = orderReview.OrderId != 0 ? UrlService.GetAdminUrl("orders/edit/" + orderReview.OrderId) : string.Empty;
            _orderNumber = orderNumber;
        }

        protected override string FormatString(string text)
        {
            var sb = new StringBuilder(text);
            
            sb.Replace("#ORDER_NUMBER#", _orderNumber);
            sb.Replace("#RATIO#", _orderReview.Ratio.ToString());
            sb.Replace("#COMMENT#", _orderReview.Text);
            sb.Replace("#ORDER_LINK#", _orderLink);

            return sb.ToString();
        }
    }

}