//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;

namespace AdvantShop.Repository
{
    public class MeasureUnits
    {
        public enum WeightUnit
        {
            Kilogramm = 0,
            Pound,
            Grams,
            Milligram,
            Micrograms,
            Tonne,
        }

        public enum LengthUnit
        {
            Centimeter,
            Millimeter,
            Metre
        }

        public enum VolumeUnit
        {
            Millimeter,
            Centimeter,
            Metre
        }

        private static Dictionary<WeightUnit, float> WeightRates = new Dictionary<WeightUnit, float>
                                                                   {
                                                                       // вес относительно килограмма
                                                                       {WeightUnit.Kilogramm, 1},
                                                                       {WeightUnit.Pound, 0.45359237F},
                                                                       {WeightUnit.Grams, 0.001F},
                                                                       {WeightUnit.Milligram, 0.000_001f},
                                                                       {WeightUnit.Micrograms, 0.000_000_001f},
                                                                       {WeightUnit.Tonne, 1000f},
                                                                   };

        private static Dictionary<LengthUnit, float> LengthRates = new Dictionary<LengthUnit, float>
                                                                   {
                                                                       // размеры относительно сантиметра
                                                                       {LengthUnit.Centimeter, 1F},
                                                                       {LengthUnit.Millimeter, 0.1F},
                                                                       {LengthUnit.Metre, 100F},
                                                                   };

        private static Dictionary<VolumeUnit, double> VolumeRates = new Dictionary<VolumeUnit, double>
                                                                   {
                                                                       // размеры в сантиметрах
                                                                       {VolumeUnit.Centimeter, 1d},
                                                                       {VolumeUnit.Millimeter, 0.001d},
                                                                       {VolumeUnit.Metre, 1_000_000d},
                                                                   };

        public static float ConvertWeight(float value, WeightUnit from, WeightUnit to)
        {
            return value * WeightRates[from] / WeightRates[to];
        }

        public static float ConvertLength(float value, LengthUnit from, LengthUnit to)
        {
            return value * LengthRates[from] / LengthRates[to];
        }

        public static double ConvertVolume(double value, VolumeUnit from, VolumeUnit to)
        {
            return value * VolumeRates[from] / VolumeRates[to];
        }
    }
}