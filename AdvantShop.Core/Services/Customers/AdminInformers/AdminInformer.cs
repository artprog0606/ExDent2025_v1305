using System;

namespace AdvantShop.Core.Services.Customers.AdminInformers
{
    public enum AdminInformerType
    {
        Other = 0,
        Vk = 1,
        Facebook = 2,
        Instagram = 3,
        Error = 4,

        Comment = 5,
        Email = 6,
        Telegram = 7,
        Review = 8,
        Ok = 9,
        TaskReminder = 10
    }

    public class AdminInformer
    {
        public int Id { get; set; }
        public AdminInformerType Type { get; set; }

        /// <summary>
        /// Id объекта (сообщения, комментария и тд.)
        /// </summary>
        public int ObjId { get; set; }
        public Guid? CustomerId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        /// <summary>
        /// Ссылка на задачу/заказа/тд
        /// </summary>
        public string Link { get; set; }
        public int Count { get; set; }
        public bool Seen { get; set; }
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Id пользователя, он единственный сможет увидеть это сообщение
        /// </summary>
        public Guid? PrivateCustomerId { get; set; }

        /// <summary>
        /// Id сущности, к которой относится объект (т.е. id лида, задачи, заказа и т.д.)
        /// </summary>
        public int? EntityId { get; set; }

        public AdminInformer() { }

        public AdminInformer(AdminInformerType type, int id, Guid? customerId)
        {
            Type = type;
            ObjId = id;
            CustomerId = customerId;
        }

    }
}
