using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Crm.Vk.VkMarket;
using VkNet.Enums;
using VkNet.Model;
using VkNet.Model.Attachments;

namespace AdvantShop.Core.Services.Crm.Vk
{
    public class VkMessage
    {
        /// <summary>
        /// id в бд
        /// </summary>
        public int Id { get; set; }

        /// <summary>Идентификатор в vk</summary>
        public long? MessageId { get; set; }

        /// <summary>
        /// Идентификатор автора сообщения (для исходящего сообщения — идентификатор получателя).
        /// </summary>
        public long? UserId { get; set; }
        
        /// <summary>
        /// Дата отправки сообщения.
        /// </summary>
        public DateTime? Date { get; set; }
        
        /// <summary>Заголовок сообщения или беседы.</summary>
        public string Title { get; set; }

        /// <summary>
        /// Received, Sended
        /// </summary>
        public VkMessageType Type { get; set; }

        /// <summary>
        /// Unreaded, Readed
        /// </summary>
        public string ReadState { get; set; }

        /// <summary>Текст сообщения.</summary>
        public string Body { get; set; }
        
        /// <summary>
        /// Идентификатор автора сообщения.
        /// </summary>
        public long? FromId { get; set; }
       
        /// <summary>
        /// Идентификатор беседы.
        /// </summary>
        public long? ChatId { get; set; }


        public long? PostId { get; set; }

        public List<Offer> Offers { get; set; }

        public bool CreateLead { get; set; }

        public VkMessage()
        {
            Offers = new List<Offer>();
        }

        public VkMessage(Message message, bool createLead) : this()
        {
            MessageId = message.Id;
            UserId = message.UserId;
            Date = message.Date?.ToLocalTime();
            Body = !string.IsNullOrEmpty(message.Text)
                    ? HttpUtility.HtmlEncode(message.Text).Replace("\r\n", "<br>\r\n")
                    : "";
            ChatId = message.ChatId;

            FromId = 
                message.Type != null && message.Type == MessageType.Sended 
                        ? -SettingsVk.Group.Id 
                        : message.FromId;
            Type =
                message.Type != null
                    ? (message.Type == MessageType.Received ? VkMessageType.Received : VkMessageType.Sended)
                    : message.FromId == -SettingsVk.Group.Id ? VkMessageType.Sended : VkMessageType.Received;
            
            ReadState = message.ReadState != null ? message.ReadState.ToString() : "";

            CreateLead = createLead;
            
            if (message.Attachments != null && message.Attachments.Count > 0)
            {
                foreach (var attachment in message.Attachments.Where(x => x.Type == typeof(Market)))
                {
                    var market = (Market)attachment.Instance;
                    Body = String.Format("Товар: <a href=\"https://vk.com/{0}?w=product-{1}_{2}\" target=\"_blank\">{3}</a><br>\r\n{4}",
                                        SettingsVk.Group.ScreenName, SettingsVk.Group.Id, market.Id, market.Title, Body);

                    var offer = GetOffer(market);
                    if (offer != null)
                    {
                        if (market.Price != null && market.Price.Amount != null && market.Price.Amount > 0)
                        {
                            var price = ((float)market.Price.Amount.Value)/100;
                            offer.BasePrice = price;
                        }
                        Offers.Add(offer);
                    }
                }
            }
        }

        public VkMessage(Post post, Comment comment, bool createLead)
        {
            MessageId = comment.Id;
            UserId = comment.ReplyToUser ?? comment.FromId;
            Date = comment.Date?.ToLocalTime();

            var postUrl = string.Format("https://vk.com/wall-{0}_{1}", SettingsVk.Group.Id, post.Id);

            Body =
                string.Format(
                    "Комментарий к посту \"<a href=\"{0}\" target=\"_blank\">{1}</a>\": <br>\r\n {2}",
                    postUrl,
                    !string.IsNullOrWhiteSpace(post.Text) ? HttpUtility.HtmlEncode(post.Text.Reduce(30)) : postUrl,
                    HttpUtility.HtmlEncode(comment.Text));

            PostId = post.Id;

            FromId = comment.FromId;

            Type = comment.ReplyToUser != null && comment.ReplyToUser == SettingsVk.Group.Id
                ? VkMessageType.Sended
                : VkMessageType.Received;

            ReadState = "";

            CreateLead = createLead;
        }

        private Offer GetOffer(Market market)
        {
            if (market.Id != null)
            {
                var vkProduct = new VkProductService().Get(market.Id.Value);
                if (vkProduct != null)
                {
                    var o = OfferService.GetOffer(vkProduct.OfferId);
                    if (o != null)
                        return o;
                }
            }

            var title = market.Title.ToLower();
            var index = title.IndexOf("арт.", StringComparison.Ordinal);
            if (index >= 0)
            {
                var artno = market.Title.Substring(index + 4).Trim();

                var o = OfferService.GetOffer(artno);
                if (o != null)
                    return o;

                var p = ProductService.GetProduct(artno);
                if (p != null && p.Offers != null && p.Offers.Count > 0)
                    return p.Offers[0];
            }

            var productByName = ProductService.GetProductByName(market.Title);
            if (productByName != null && productByName.Offers != null && productByName.Offers.Count > 0)
                return productByName.Offers[0];

            return null;
        }
    }

    public enum VkMessageType
    {
        Received,
        Sended,
        Other
    }

    public class VkMessagePostCount
    {
        public long PostId { get; set; }
        public int Count { get; set; }
    }
}
