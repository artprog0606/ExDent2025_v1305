using AdvantShop.Core.Common.Extensions;
using AdvantShop.Shipping.FivePost.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Services.Shipping.FivePost.Helpers
{
    public class FivePostHelper
    {
        public const float MaxWeightWithoutOverweightKg = 3;
        public const float InsureCoef = 0.005f; // 0,5%

        public static float Calculate(float value, float extraWeightValue, float weight)
        {
            var extraWeight = weight > MaxWeightWithoutOverweightKg
                ? (float)Math.Ceiling(weight - MaxWeightWithoutOverweightKg)
                : 0f;

            var result = extraWeight * extraWeightValue + value;
            return result;
        }

        public static string TimeWorkToString(List<FivePostPickPointWorkHour> timeWorks)
        {
            var timeWorkParts = timeWorks.GroupBy(x => new { x.OpensAt, x.CloseAt });

            return string.Join(", ",
                timeWorkParts
                .Select(x =>
                {
                    var days = string.Join(", ", x.Select(item => item.Day));
                    var opensAt = x.Key.OpensAt.ToString("hh':'mm");
                    var closeAt = x.Key.CloseAt.ToString("hh':'mm");
                    return $"{days}: {opensAt} - {closeAt}";
                }));
        }

        [Obsolete("Тарифы и сроки сопоставляются клиентом вручную")]
        /// <summary>
        /// Связь тарифов и сроков доставки
        /// Сроки доставки до точки приоритетней, чем до города
        /// </summary>
        public static readonly Dictionary<EFivePostRateType, List<EFivePostDeliveryType>> DefaultRateDeliveryReference = new Dictionary<EFivePostRateType, List<EFivePostDeliveryType>>
        {
            {
                EFivePostRateType.HubMoscow,
                new List<EFivePostDeliveryType> { EFivePostDeliveryType.HubMoscowPoint, EFivePostDeliveryType.HubMoscowCity,
                    EFivePostDeliveryType.HubMoscowCityPlus1, EFivePostDeliveryType.HubMoscowCityPlus2, EFivePostDeliveryType.HubMoscowCityPlus3,
                    EFivePostDeliveryType.HubMoscowCityPlus4, EFivePostDeliveryType.HubMoscowPointPlus1, EFivePostDeliveryType.HubMoscowPointPlus2,
                    EFivePostDeliveryType.HubMoscowPointPlus3, EFivePostDeliveryType.HubMoscowPointPlus4, EFivePostDeliveryType.HubMoscowPointPlus5 }
            },
            {
                EFivePostRateType.HubSpb,
                new List<EFivePostDeliveryType> {EFivePostDeliveryType.HubSpbPoint, EFivePostDeliveryType.HubSpbCity,
                    EFivePostDeliveryType.HubSpbPointPlus3, EFivePostDeliveryType.HubSpbCityPlus1, EFivePostDeliveryType.HubSpbPointPlus2}
            },
            {
                EFivePostRateType.HubEkb,
                new List<EFivePostDeliveryType> {EFivePostDeliveryType.HubEkbPoint, EFivePostDeliveryType.HubEkbCity,
                    EFivePostDeliveryType.HubEkbCityPlus1, EFivePostDeliveryType.HubEkbPointPlus2, EFivePostDeliveryType.HubEkbPointPlus3}
            },
            {
                EFivePostRateType.HubKrasnodar,
                new List<EFivePostDeliveryType> {EFivePostDeliveryType.HubKrasnodarPoint, EFivePostDeliveryType.HubKrasnodarCity }
            },
            {
                EFivePostRateType.HubKazan,
                new List<EFivePostDeliveryType> {EFivePostDeliveryType.HubKazanPoint, EFivePostDeliveryType.HubKazanCity,
                    EFivePostDeliveryType.HubKazanCityPlus1, EFivePostDeliveryType.HubKazanCityPlus2 }
            },
            {
                EFivePostRateType.HubRostovNaDonu,
                new List<EFivePostDeliveryType> {EFivePostDeliveryType.HubRostovOnDonPoint, EFivePostDeliveryType.HubRostovOnDonCity,
                    EFivePostDeliveryType.HubRostovOnDonPointMinus3 }
            },
            {
                EFivePostRateType.HubVoronej,
                new List<EFivePostDeliveryType> {EFivePostDeliveryType.HubVoronezhPoint, EFivePostDeliveryType.HubVoronezhCity }
            },
            {
                EFivePostRateType.HubChelyabinsk,
                new List<EFivePostDeliveryType> {EFivePostDeliveryType.HubChelyabinskPoint, EFivePostDeliveryType.HubChelyabinskCity }
            },
            {
                EFivePostRateType.HubSamara,
                new List<EFivePostDeliveryType> {EFivePostDeliveryType.HubSamaraCity, EFivePostDeliveryType.HubSamaraPoint }
            },
            {
                EFivePostRateType.HubElabuga,
                new List<EFivePostDeliveryType> {EFivePostDeliveryType.HubKazanPoint, EFivePostDeliveryType.HubKazanCity,
                    EFivePostDeliveryType.HubKazanCityPlus1, EFivePostDeliveryType.HubKazanCityPlus2 }
            },
        };
    }
}
