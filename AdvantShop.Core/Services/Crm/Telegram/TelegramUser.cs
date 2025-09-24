using System;
using Telegram.Bot.Types;

namespace AdvantShop.Core.Services.Crm.Telegram
{
    public class TelegramUser
    {
        public long Id { get; set; }
        public bool IsBot { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }

        public string PhotoUrl { get; set; }

        public Guid CustomerId { get; set; }


        public TelegramUser()
        {
        }

        public TelegramUser(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            IsBot = user.IsBot;
        }

    }
}
