//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Orders
{
    public class ShareShoppingCartItem : IComparable<ShareShoppingCartItem>
    {
        public string Key { get; set; }
        public Guid CustomerId { get; set; }
        public string AttributesXml { get; set; }
        public float Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public int OfferId { get; set; }

        public int CompareTo(ShareShoppingCartItem other)
        {
            if (other == null)
                return 1;

            var compareOfferId = OfferId.CompareTo(other.OfferId);
            if (compareOfferId != 0)
                return compareOfferId;
            
            var compareAmount = Amount.CompareTo(other.Amount);
            if (compareAmount != 0)
                return compareAmount;

            return string.Compare(AttributesXml, other.AttributesXml, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return CustomerId.GetHashCode() ^ OfferId ^ Amount.GetHashCode() ^ (AttributesXml ?? "").GetHashCode();
        }
    }
}