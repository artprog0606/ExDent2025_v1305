using AdvantShop.Core.Common.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Services.TrafficStatistics
{
    public class TrafficStatisticsService
    {
        private static readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> RequestStatistics = new ConcurrentDictionary<string, ConcurrentQueue<DateTime>>();

        public static void AddIpToRequestStatistics(string ip)
        {
            if (RequestStatistics.TryGetValue(ip, out ConcurrentQueue<DateTime> data))
                data.Enqueue(DateTime.Now);
            else
                RequestStatistics.TryAdd(ip, new ConcurrentQueue<DateTime>(new DateTime[] { DateTime.Now }));
        }

        public static Dictionary<string, int> GetTrafficStatistics(DateTime? startDateTime = null)
        {
            return RequestStatistics.ToDictionary(key => key.Key, val => startDateTime.HasValue ? val.Value.Count(x => x >= startDateTime) : val.Value.Count);
        }

        public static void DeleteExpiredDateTime()
        {
            if (RequestStatistics == null)
                return;
            var minDateTime = DateTime.Now.AddHours(-1);
            var deletedIps = new List<string>();
            foreach (var requestStatistic in RequestStatistics)
            {
                while (requestStatistic.Value.TryDequeue(out DateTime dateTime))
                    if (dateTime > minDateTime)
                        break;
                if (requestStatistic.Value.Count == 0)
                    deletedIps.Add(requestStatistic.Key);
            }
            foreach (var deletedIp in deletedIps)
                RequestStatistics.TryRemove(deletedIp, out _);
        }
    }
}
