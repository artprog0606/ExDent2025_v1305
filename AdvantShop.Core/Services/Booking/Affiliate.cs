//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.Booking
{
    public class Affiliate
    {
        public int Id { get; set; }
        public int? CityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int BookingIntervalMinutes { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public bool AccessForAll { get; set; }

        private List<int> _managerIds;
        public List<int> ManagerIds
        {
            get { return _managerIds ?? (_managerIds = AffiliateService.GetManagerIds(Id)); }
            set
            {
                _managerIds = value;
                _managers = null;
            }
        }

        private List<Manager> _managers;
        public List<Manager> Managers
        {
            get { return _managers ?? (_managers = ManagerService.GetManagers(ManagerIds)); }
        }

        public bool AnalyticsAccessForAll { get; set; }

        private List<int> _managerAnalyticsIds;
        public List<int> AnalyticManagerIds
        {
            get { return _managerAnalyticsIds ?? (_managerAnalyticsIds = AffiliateService.GetAnalyticManagerIds(Id)); }
            set
            {
                _managerAnalyticsIds = value;
                _managersAnalytics = null;
            }
        }

        private List<Manager> _managersAnalytics;
        public List<Manager> AnalyticManagers
        {
            get { return _managersAnalytics ?? (_managersAnalytics = ManagerService.GetManagers(AnalyticManagerIds)); }
        }

        /// <summary>
        /// Просмотр данных брони менджером ресурса (не имеющего прав в филиале), т.е. открытие самой брони.
        /// Не подразумевает ограничение на отображение в гриде и других списках, для этих менеджеров.
        /// </summary>
        public bool AccessToViewBookingForResourceManagers { get; set; }

        public bool IsActiveSmsNotification { get; set; }
        public int? ForHowManyMinutesToSendSms { get; set; }
        public string SmsTemplateBeforeStartBooiking { get; set; }
        public int? CancelUnpaidViaMinutes { get; set; }
    }
}
