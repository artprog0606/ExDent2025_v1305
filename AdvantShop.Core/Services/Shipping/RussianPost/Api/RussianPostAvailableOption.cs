using System.Collections.Generic;

namespace AdvantShop.Shipping.RussianPost.Api
{
    public class RussianPostAvailableOption
    {
        #region AvailableMailCategory

        // не используется, а потому не актуализируется
        public static readonly Dictionary<EnMailType, List<EnMailCategory>> AvailableMailCategory = new Dictionary
            <EnMailType, List<EnMailCategory>>()
        {
            {
                EnMailType.Letter, new List<EnMailCategory>()
                {
                    EnMailCategory.Simple,
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.LetterClass1, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.Banderol, new List<EnMailCategory>()
                {
                    EnMailCategory.Simple,
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.BanderolClass1, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.PostalParcel, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.ParcelClass1, new List<EnMailCategory>
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.OnlineParcel, new List<EnMailCategory>
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.OnlineCourier, new List<EnMailCategory>
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.BusinessCourier, new List<EnMailCategory>
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue
                }
            },
            {
                EnMailType.BusinessCourierExpress, new List<EnMailCategory>
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue
                }
            },
            {
                EnMailType.Ems, new List<EnMailCategory>
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.EmsOptimal, new List<EnMailCategory>
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.EmsRT, new List<EnMailCategory>
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue
                }
            }
        };

        #endregion

        /// <summary>
        /// Максимальный вес отправлений в граммах
        /// </summary>
        public static readonly Dictionary<EnMailType, int> MaxWeightMailType = new Dictionary<EnMailType, int>()
        {
            { EnMailType.Letter, 100},
            { EnMailType.LetterClass1, 500},
            { EnMailType.Banderol, 5000},
            { EnMailType.BanderolClass1, 2500 },
            { EnMailType.PostalParcel, 50000 },
            { EnMailType.ParcelClass1, 2500 },
            { EnMailType.OnlineParcel, 20000 },
            { EnMailType.OnlineCourier, 31500 },
            { EnMailType.BusinessCourier, 31500 },
            { EnMailType.BusinessCourierExpress, 31500 },
            { EnMailType.Ems, 31500},
            { EnMailType.EmsOptimal, 20000 },
            { EnMailType.EmsRT, 31500 },
            { EnMailType.ECOM, 20000 },
            { EnMailType.SmallPacket, 2000 },
        };

        /// <summary>
        /// Максимальный вес отправлений в граммах
        /// <para>Для международных отправлений</para>
        /// </summary>
        public static readonly Dictionary<EnMailType, int> MaxWeightForInternationalMailType = new Dictionary<EnMailType, int>()
        {
            { EnMailType.PostalParcel, 20000 },
        };

        /// <summary>
        /// Максимальный размер одной из сторон отправлений в миллиметрах
        /// </summary>
        public static readonly Dictionary<EnMailType, int> MaxDimensionMailType = new Dictionary<EnMailType, int>()
        {
            { EnMailType.OnlineParcel, 1500},
            { EnMailType.OnlineCourier, 1500},
            { EnMailType.ECOM, 600},
            { EnMailType.ParcelClass1, 360},
            { EnMailType.BusinessCourier, 1500 },
            { EnMailType.BusinessCourierExpress, 1500 },
            { EnMailType.EmsRT, 1500 },
            { EnMailType.Ems, 1500},
            { EnMailType.EmsOptimal, 600},
            { EnMailType.PostalParcel, 3500 },
        };

        /// <summary>
        /// Максимальный размер всех сторон отправлений в миллиметрах
        /// </summary>
        public static readonly Dictionary<EnMailType, int> MaxSumDimensionsMailType = new Dictionary<EnMailType, int>()
        {
            { EnMailType.OnlineParcel, 2200},
            { EnMailType.OnlineCourier, 2200},
            { EnMailType.ECOM, 1400},
            { EnMailType.ParcelClass1, 700},
            { EnMailType.SmallPacket, 900 },
            { EnMailType.BusinessCourier, 3000 },
            { EnMailType.BusinessCourierExpress, 3000 },
            { EnMailType.EmsOptimal, 1400 },
            { EnMailType.EmsRT, 2200 },
            { EnMailType.Banderol, 900},
            { EnMailType.BanderolClass1, 700 },
            { EnMailType.Ems, 3000},
            { EnMailType.PostalParcel, 6700 },
        };

        /// <summary>
        /// Максимальные габариты (д*ш*в) в миллиметрах
        /// </summary>
        public static readonly Dictionary<EnMailType, int[]> MaxDimensionsMailType = new Dictionary<EnMailType, int[]>()
        {
            { EnMailType.PostalParcel, new[]{ 3500, 1900, 1300 } },
        };

        /// <summary>
        /// Максимальная сумма длины и периметра наибольшего поперечного сечения в миллиметрах (Length + 2Height + 2Width)
        /// <para>Специально для EMS</para>
        /// </summary>
        public static readonly Dictionary<EnMailType, int> MaxLength2Height2WidthDimensionsMailType = new Dictionary<EnMailType, int>()
        {
            { EnMailType.Ems, 3000},
        };

        public static readonly Dictionary<EnMailType, int> MinWeightMailType = new Dictionary<EnMailType, int>()
        {
            { EnMailType.ECOM, 50 },
        };

        public static readonly List<EnMailType> DimensionsSendWithOrder = new List<EnMailType>()
        {
            EnMailType.PostalParcel,
            EnMailType.OnlineParcel,
            EnMailType.OnlineCourier,
            EnMailType.ParcelClass1,
            EnMailType.Ems,
            EnMailType.EmsRT,
            EnMailType.EmsTender,
        };

        public static readonly List<EnMailType> DimensionsNotRequired = new List<EnMailType>()
        {
            EnMailType.Letter,
            EnMailType.LetterClass1,
        };

        public static readonly List<EnMailType> CourierOptionAvailable = new List<EnMailType>()
        {
            EnMailType.EmsOptimal,
            EnMailType.EmsRT,
            EnMailType.EmsTender,
        };

        public static readonly List<EnMailType> FragileOptionAvailable = new List<EnMailType>()
        {
            EnMailType.PostalParcel
        };

        public static readonly List<EnMailType> DeliveryWithCodAvailable = new List<EnMailType>()
        {
            EnMailType.EmsOptimal,
            EnMailType.OnlineCourier,
            EnMailType.Ems,
            EnMailType.PostalParcel,
            EnMailType.ParcelClass1,
            EnMailType.OnlineParcel,
        };

        public static readonly List<EnMailType> DeliveryToOpsAvailable = new List<EnMailType>()
        {
            EnMailType.PostalParcel,
            EnMailType.ParcelClass1,
            EnMailType.OnlineParcel,
        };

        public static readonly Dictionary<EnMailType, List<EnMailCategory>> OrderOfNoticeOptionAvailable = 
            new Dictionary<EnMailType, List<EnMailCategory>>()
        {
            {
                EnMailType.Letter, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.LetterClass1, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.Banderol, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.BanderolClass1, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
        };

        public static readonly Dictionary<EnMailType, List<EnMailCategory>> SimpleNoticeOptionAvailable = 
            new Dictionary<EnMailType, List<EnMailCategory>>()
        {
            {
                EnMailType.Letter, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.LetterClass1, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.Banderol, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.BanderolClass1, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
        };

        public static readonly Dictionary<EnMailType, List<EnMailCategory>> ElectronicNoticeOptionAvailable =
            new Dictionary<EnMailType, List<EnMailCategory>>()
        {
            {
                EnMailType.Letter, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.LetterClass1, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.Banderol, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.BanderolClass1, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
        };

        public static readonly Dictionary<EnMailType, List<EnMailCategory>> SmsNoticeOptionAvailable =
            new Dictionary<EnMailType, List<EnMailCategory>>()
        {
            {
                EnMailType.Letter, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.LetterClass1, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.Banderol, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.BanderolClass1, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordered,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.PostalParcel, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.ParcelClass1, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.OnlineParcel, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.OnlineCourier, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.BusinessCourierExpress, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue,
                }
            },
            {
                EnMailType.Ems, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.EmsOptimal, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithDeclaredValue,
                    EnMailCategory.WithDeclaredValueAndCashOnDelivery
                }
            },
            {
                EnMailType.ECOM, new List<EnMailCategory>()
                {
                    EnMailCategory.Ordinary,
                    EnMailCategory.WithCompulsoryPayment,
                    EnMailCategory.WithDeclaredValueAndCompulsoryPayment
                }
            },
        };

        public static readonly Dictionary<EnMailType, List<EnTransportType>> InternationalTransportTypeAvailable =
            new Dictionary<EnMailType, List<EnTransportType>>()
        {
            {
                EnMailType.Letter, new List<EnTransportType>()
                {
                    EnTransportType.Surface,
                    EnTransportType.Avia
                }
            },
            {
                EnMailType.PostalParcel, new List<EnTransportType>()
                {
                    EnTransportType.Surface,
                    EnTransportType.Avia
                }
            },
        };

        public static readonly Dictionary<EnMailType, List<EnTransportType>> LocalTransportTypeAvailable =
            new Dictionary<EnMailType, List<EnTransportType>>()
        {
            {
                EnMailType.EmsTender, new List<EnTransportType>()
                {
                    EnTransportType.Standard,
                    EnTransportType.Express
                }
            },
        };

        /// <summary>
        /// ISO3 номера стран ЕАЭС
        /// </summary>
        public static readonly List<int> CountryIso3NumberEurasianEconomicUnion = new List<int>()
        {
            // Армения
            51,
            
            // Беларусь
            // 112,
            
            // Казахстан
            398,
            
            // Киргизия
            417
        };
    }
}
