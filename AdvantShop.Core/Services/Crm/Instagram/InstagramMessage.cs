using System;
using AdvantShop.Localization;

namespace AdvantShop.Core.Services.Crm.Instagram
{
    public enum InstagramMessageType
    {
        /// <summary>
        /// Комментарий к посту
        /// </summary>
        Media = 0,

        /// <summary>
        /// Сообщение в директ (текст)
        /// </summary>
        DirectText = 1,

        /// <summary>
        /// Сообщение в директ (медиа)
        /// </summary>
        DirectMedia = 2,
    }

    public class InstagramMessage
    {
        /// <summary>
        /// db id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// thread Id, если не пустой, то из директа
        /// </summary>
        public string ThreadId { get; set; }

        /// <summary>
        /// media Id, если не пустой, то из сообщение из посту
        /// </summary>
        public string MediaPk { get; set; }

        /// <summary>
        /// instagram id
        /// </summary>
        public string InstagramId { get; set; }

        public string Text { get; set; }

        /// <summary>
        /// Название к чему комментарий
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// От кого сообщение
        /// </summary>
        public string FromUserPk { get; set; }

        /// <summary>
        /// Кому сообщение
        /// </summary>
        public string ToUserPk { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public string CreatedDateFormatted
        {
            get { return Culture.ConvertDate(CreatedDate); }
        }

        public InstagramMessageType ItemType { get; set; }

        //public InstaMedia MediaShare { get; set; }
        
    }

    public class InstagramUserMessage : InstagramMessage
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string ProfilePicture { get; set; }
        public Guid CustomerId { get; set; }
    }
}
