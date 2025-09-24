//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Repository
{
    public class Measure
    {
        public float[] XYZ { get; set; }
        public float Amount { get; set; }
    }

    public class MeasureHelper
    {
        public static float[] GetDimensions(ShippingCalculationParameters calculationParameters, 
                                        float defaultHeight = 0, float defaultWidth = 0, float defaultLength = 0, 
                                        float rate = 1)
        {
            if (calculationParameters.TotalLength != null &&
                calculationParameters.TotalWidth != null &&
                calculationParameters.TotalHeight != null)
            {
                return new float[3] {
                    (calculationParameters.TotalLength.Value == 0 ? defaultLength : calculationParameters.TotalLength.Value) / rate,
                    (calculationParameters.TotalWidth.Value == 0 ? defaultWidth : calculationParameters.TotalWidth.Value) / rate,
                    (calculationParameters.TotalHeight.Value == 0 ? defaultHeight : calculationParameters.TotalHeight.Value) / rate
                };
            }
            
            calculationParameters.PreOrderItems = calculationParameters.PreOrderItems ?? throw new ArgumentException($"The {nameof(calculationParameters.PreOrderItems)} property of the {nameof(calculationParameters)} parameter is null");

            var dimensions = calculationParameters.PreOrderItems.Select(item => new Measure
            {
                XYZ = new[]
                {
                    (item.Length == 0 ? defaultLength : item.Length) / rate,
                    (item.Width == 0 ? defaultWidth : item.Width) / rate,
                    (item.Height == 0 ? defaultHeight : item.Height) / rate,
                },
                Amount = item.Amount
            }).ToList();

            return calculationParameters.PreOrderItems.Count == 1 && calculationParameters.PreOrderItems[0].Amount == 1 
                ? dimensions[0].XYZ 
                : GetDimensions(dimensions);
        }

        private static float[] GetDimensions(List<Measure> dimensions)
        {
            var result = new float[3];

            foreach (var item in dimensions)
            {
                item.XYZ = item.XYZ.OrderByDescending(x => x).ToArray();
            }

            foreach (var dim in dimensions)
            {
                if (dim.XYZ[0] >= result[0])
                    result[0] = dim.XYZ[0];

                if (dim.XYZ[1] >= result[1])
                    result[1] = dim.XYZ[1];

                result[2] += dim.XYZ[2] * dim.Amount;
            }
            return result;
        }


        public static float[] GetDimensions(Order order, 
                                            float defaultHeight = 0, float defaultWidth = 0, float defaultLength = 0, 
                                            float rate = 1)
        {
            var calculationParameters =
                ShippingCalculationConfigurator.Configure()
                                               .WithTotalWeight(order.TotalWeight)
                                               .WithTotalLength(order.TotalLength)
                                               .WithTotalWidth(order.TotalWidth)
                                               .WithTotalHeight(order.TotalHeight)
                                               .WithPreOrderItems(
                                                    order.OrderItems
                                                         .Select(x => new PreOrderItem(x))
                                                         .ToList())
                                               .Build();

            return GetDimensions(calculationParameters, defaultHeight, defaultWidth, defaultLength, rate);
        }

        public static float[] GetDimensions(Order order, List<Measure> dimensions)
        {
            if (order.TotalLength != null &&
                order.TotalWidth != null &&
                order.TotalHeight != null)
            {
                return new float[3] { order.TotalLength.Value, order.TotalWidth.Value, order.TotalHeight.Value };
            }

            return GetDimensions(dimensions);
        }

        public static float GetTotalWeight(Order order, List<OrderItem> orderItems, float defaultWeight = 0, float rate = 1)
        {
            var weight = order.TotalWeight != null
                ? order.TotalWeight.Value * rate
                : orderItems.Sum(x => (x.Weight == 0 ? defaultWeight : x.Weight) * rate * x.Amount);

            return weight;
        }

        public static float ConvertUnitToNewAmount(float oldUnit, float oldAmount, float newAmount)
            => oldUnit * oldAmount / newAmount;
    }
}