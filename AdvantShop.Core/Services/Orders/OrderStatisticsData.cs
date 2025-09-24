using System;
using System.Collections.Generic;
using AdvantShop.Localization;

namespace AdvantShop.Core.Services.Orders
{
    public class AbcXyzAnalysisData
    {
        public string ArtNo { get; set; }

        public List<DateTime> Dates { get; set; }

        public float PriceSum { get; set; }
    }

    public class AbcXyzAnalysis
    {
        public string ArtNo { get; set; }

        public List<AbcXyzAnalysisDateTime> PriceSumList { get; set; } 

        public float PriceSummary { get; set; }

        public float Percent { get; set; }

        public double StdDev { get; set; }


        public string Abc { get; set; }

        public string Xyz { get; set; }
    }

    public class AbcXyzAnalysisDateTime
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public float Sum { get; set; }
    }


    public class RfmAnalysisDataItem
    {
        public Guid CustomerId { get; set; }
        public string LastOrderNumber { get; set; }
        public DateTime LastOrderDate { get; set; }
        public int OrdersCount { get; set; }
        public float OrdersSum { get; set; }

        public int DaysCount { get; set; }
        public int R { get; set; }
        public int F { get; set; }
        public int M { get; set; }
        public int Rfm { get; set; }
    }

    public class StatisticsDataItem
    {
        public string Name { get; set; }
        public float Sum { get; set; }
        public float Count { get; set; }
    }

    public class OrdersStatisticsDto
    {
        public int OrdersCount { get; set; }
        public float OrdersSum { get; set; }
        public float AvgCheck { get; set; }
        public float ProductsCount { get; set; }
        public float Profit { get; set; }
    }
}
