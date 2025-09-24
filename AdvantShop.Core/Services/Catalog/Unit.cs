using System;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Catalog
{
    public class Unit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public MeasureType? MeasureType { get; set; }
        public int SortOrder { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
    }

    public enum MeasureType : byte
    {
        /// <summary>
        /// шт. или ед.
        /// </summary>
        [Localize("Штука или единица товара")]
        Piece = 0,
        
        /// <summary>
        /// Грамм
        /// </summary>
        [Localize("Грамм")]
        Gram = 10,
        
        /// <summary>
        /// Килограмм
        /// </summary>
        [Localize("Килограмм")]
        Kilogram = 11,
        
        /// <summary>
        /// Тонна
        /// </summary>
        [Localize("Тонна")]
        Ton = 12,
        
        /// <summary>
        /// Сантиметр
        /// </summary>
        [Localize("Сантиметр")]
        Centimetre = 20,
        
        /// <summary>
        /// Дециметр
        /// </summary>
        [Localize("Дециметр")]
        Decimeter = 21,
        
        /// <summary>
        /// Метр
        /// </summary>
        [Localize("Метр")]
        Metre = 22,
        
        /// <summary>
        /// Квадратный сантиметр
        /// </summary>
        [Localize("Квадратный сантиметр")]
        SquareCentimeter = 30,

        /// <summary>
        /// Квадратный дециметр
        /// </summary>
        [Localize("Квадратный дециметр")]
        SquareDecimeter = 31,

        /// <summary>
        /// Квадратный метр
        /// </summary>
        [Localize("Квадратный метр")]
        SquareMeter = 32,

        /// <summary>
        /// Миллилитр
        /// </summary>
        [Localize("Миллилитр")]
        Milliliter = 40,

        /// <summary>
        /// Литр
        /// </summary>
        [Localize("Литр")]
        Liter = 41,

        /// <summary>
        /// Кубический метр
        /// </summary>
        [Localize("Кубический метр")]
        CubicMeter = 42,

        /// <summary>
        /// Киловатт час
        /// </summary>
        [Localize("Киловатт час")]
        KilowattHour = 50,

        /// <summary>
        /// Гигакалория
        /// </summary>
        [Localize("Гигакалория")]
        Gigacaloria = 51,

        /// <summary>
        /// Сутки (день)
        /// </summary>
        [Localize("Сутки (день)")]
        Day = 70,

        /// <summary>
        /// Час
        /// </summary>
        [Localize("Час")]
        Hour = 71,

        /// <summary>
        /// Минута
        /// </summary>
        [Localize("Минута")]
        Minute = 72,

        /// <summary>
        /// Секунда
        /// </summary>
        [Localize("Секунда")]
        Second = 73,

        /// <summary>
        /// Килобайт
        /// </summary>
        [Localize("Килобайт")]
        Kilobyte = 80,

        /// <summary>
        /// Мегабайт
        /// </summary>
        [Localize("Мегабайт")]
        Megabyte = 81,

        /// <summary>
        /// Гигабайт
        /// </summary>
        [Localize("Гигабайт")]
        Gigabyte = 82,

        /// <summary>
        /// Терабайт
        /// </summary>
        [Localize("Терабайт")]
        Terabyte = 83,
        
        [Localize("Другое")]
        Other = 255
    }
}