using System;

namespace AdvantShop.Core.Services.Auth.Calls
{
    public class CallCodeConfirmation
    {
        public int Id { get; set; }
        public long Phone { get; set; }
        public string Code { get; set; }
        public bool IsUsed { get; set; }
        public DateTime DateAdded { get; set; }
    }
}