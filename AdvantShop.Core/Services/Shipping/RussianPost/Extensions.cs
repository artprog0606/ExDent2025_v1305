using AdvantShop.Core.Common.Extensions;
using AdvantShop.Shipping.RussianPost.Api;

namespace AdvantShop.Shipping.RussianPost
{
    public static class Extensions
    {
        public static string AddressFromStreet(this Address address)
        {
            return string.Format("{0}{1}{2}{3}", 
                address.Street, 
                address.House.IsNotEmpty() ? ", " + address.House : null,
                address.Corpus.IsNotEmpty() ? " к " + address.Corpus : null,
                address.Building.IsNotEmpty() ? " стр " + address.Building : null);
        }

        public static string AddressFromLocation(this Address address)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}", 
                address.Location, 
                address.Location.IsNotEmpty() ? ", " : null,
                address.Street, 
                address.House.IsNotEmpty() ? ", " + address.House : null,
                address.Corpus.IsNotEmpty() ? " к " + address.Corpus : null,
                address.Building.IsNotEmpty() ? " стр " + address.Building : null);
        }

        //public static string Localize<T>(this T enumValue)
        //    where T: StringEnum<T>
        //{
        //    return AttributeHelper.GetAttributeValueProperty<LocalizeAttribute, string>(enumValue) ?? enumValue.Value;
        //}
    }
}
