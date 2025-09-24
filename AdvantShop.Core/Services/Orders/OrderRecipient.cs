using AdvantShop.Core.Common.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Services.Orders
{
    public class OrderRecipient
    {
        public int OrderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        public long? StandardPhone { get; set; }

        private string _fullName;
        public string FullName => _fullName ?? (_fullName = string.Join(" ", new List<string> { LastName, FirstName, Patronymic }.Where(x => x.IsNotEmpty())));
        public bool Exists => FirstName.IsNotEmpty() || LastName.IsNotEmpty() || Patronymic.IsNotEmpty() || Phone.IsNotEmpty() || StandardPhone.HasValue;
    }
}
