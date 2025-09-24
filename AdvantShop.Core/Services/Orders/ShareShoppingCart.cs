//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Orders
{
    public class ShareShoppingCart : List<ShareShoppingCartItem>
    {
        private string _key;
        public string Key => _key ?? (_key = this.FirstOrDefault()?.Key ?? string.Empty);
        public override int GetHashCode()
        {
            return this.OrderBy(x => x).Aggregate(0, (curr, val) => val.GetHashCode() ^ curr);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is ShareShoppingCart other)) return false;

            return this.GetHashCode() == other.GetHashCode();
        }
    }
}