using System.Collections.Generic;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Orders
{
    public static class GiftCertificatesExtensions
    {
        public static IEnumerable<GiftCertificate> ConvertCurrency(this IEnumerable<GiftCertificate> items, Currency sourceCurrency, Currency newCurrency)
        {
            foreach (var item in items)
                item.Sum = item.Sum.ConvertCurrency(sourceCurrency, newCurrency);

            return items;
        }

    }
}