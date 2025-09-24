using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdvantShop.Core.Services.Loging.CallsAuth;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;

namespace AdvantShop.Core.Services.Auth.Calls
{
    public class CallValidationService
    {
        private readonly bool _isInternal;
        private static readonly Regex PhoneValid = new Regex("^([0-9]{10,15})$", RegexOptions.Compiled | RegexOptions.Singleline);
        
        public CallValidationService(bool isInternal)
        {
            _isInternal = isInternal;
        }

        public bool Validate(long phone, string ip, bool throwError)
        {
            if (!IsValidPhone(phone))
            {
                if (throwError)
                    throw new BlException("Укажите валидный телефон");
                return false;
            }

            if (!_isInternal)
            {
                if (IsBanned(phone, ip))
                {
                    LogError("Слишком много запросов", throwError, phone, ip);
                    return false;
                }
                
                var hour = 1 * 60 * 60;
                var callLogs = CallLogger.GetLastCallLogsByPhoneOrIp(hour, phone, ip);
                
                if (!CheckByTimeBetweenCall(phone, ip, callLogs))
                {
                    LogError($"Следующий звонок можно будет послать через {PhoneConfirmationConfig.SecondsPerPhoneBetweenMessage}с",
                        throwError, phone, ip);
                    return false;
                }
                
                if (!CheckByIp(ip, callLogs))
                {
                    LogError("Слишком много запросов. Попробуйте позже.", throwError, phone, ip);
                    return false;
                }
                
                if (!CheckByCountBetweenCall(phone, ip, callLogs))
                {
                    LogError("Слишком много запросов. Попробуйте позже.", throwError, phone, ip);
                    return false;
                }
            }

            return true;
        }
        
        private void LogError(string error, bool throwError, long phone, string ip)
        {
            Debug.Log.Warn($"call abuse {phone} {ip} {error}");
                
            if (throwError)
                throw new BlException(error);
        }
        
        private static bool IsValidPhone(long phone)
        {
            return PhoneValid.IsMatch(phone.ToString());
        }
        
        /// <summary>
        /// Проверка что прошло разрешенное время с последнего звонка для телефона 
        /// </summary>
        public bool CheckByTimeBetweenCall(long phone, string ip, List<CallLogData> callLogs)
        {
            var lastLogData = callLogs.FindLast(x => x.Phone == phone && x.Ip == ip && x.Status == CallAuthStatus.Sent);

            var check = 
                lastLogData == null ||
                lastLogData.CreatedOn.AddSeconds(PhoneConfirmationConfig.SecondsPerPhoneBetweenMessage) < DateTime.Now;

            if (!check)
            {
                CallLogger.Log(
                    new CallLogData(phone, ip,
                        CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : default(Guid?),
                        CallAuthStatus.Fault));
            }

            return check;
        }
        
        /// <summary>
        /// Проверка что с одного ip не идет много запросов
        /// </summary>
        public bool CheckByIp(string ip, List<CallLogData> callLogs)
        {
            if (string.IsNullOrEmpty(ip) || ip == "::1" || ip == "127.0.0.1")
                return true;

            var seconds = 5;
            var dateFrom = DateTime.Now.AddSeconds(-seconds);
            
            var countPerSeconds = callLogs.Count(x => x.Ip == ip && x.CreatedOn >= dateFrom);

            // если с одного ip за 5с было 5 запросов, то баним ip
            if (countPerSeconds > PhoneConfirmationConfig.RequestsCountPerSecondByIp * seconds)
            {
                CallBanService.Ban(null, ip, DateTime.Now.AddMinutes(10));
                return false;
            }

            dateFrom = DateTime.Now.AddSeconds(-3 * 60);
            countPerSeconds = callLogs.Count(x => x.Ip == ip && x.CreatedOn >= dateFrom);

            // если с одного ip за 3 минуты было 30 запросов, то баним ip
            if (countPerSeconds > 30)
            {
                CallBanService.Ban(null, ip, DateTime.Now.AddHours(6));
                return false;
            }

            if (!CheckByIpNSec(ip, callLogs))
                return false;
            
            return true;
        }
        
        /// <summary>
        /// Проверка если будут посылать с одного ip каждые n-секунд на разные телефоны
        /// </summary>
        private bool CheckByIpNSec(string ip, List<CallLogData> callLogs)
        {
            var logs = callLogs.Where(x => x.Ip == ip).OrderByDescending(x => x.CreatedOn).ToList();
            
            if (logs.Count < 7) 
                return true;
            
            var i = 0;
            var matchCount = 0;
            var previousDate = DateTime.MinValue;
            var previousDiff = TimeSpan.Zero;

            try
            {
                var deltaTicks = TimeSpan.FromSeconds(1).Ticks;
                
                foreach (var log in logs)
                {
                    if (i != 0)
                    {
                        var diff = previousDate - log.CreatedOn;
                        
                        // если интервалы примерно развны (погрешность 1с), то увеличиваем счетчик
                        if (Math.Abs((diff - previousDiff).Ticks) < deltaTicks)
                            matchCount++;
                        
                        previousDiff = diff;
                    }
                    
                    previousDate = log.CreatedOn;
                    i++;
                    
                    // если 70% звонков посылались с одинаковым интревалом, то баним ip
                    if (matchCount == (int)(logs.Count*0.7))
                    {
                        CallBanService.Ban(null, ip, DateTime.Now.AddMinutes(30));
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return true;
        }
        
        /// <summary>
        /// Проверка что не было слишком много попыток.
        /// Запрет абьюзить "посылать запросы каждые {AuthConfig.SecondsPerPhoneBetweenMessage} секунд"  
        /// </summary>
        public bool CheckByCountBetweenCall(long phone, string ip, List<CallLogData> callLogs)
        {
            var minutes = 3;
            var allowedCount = 5; // 5 попыток = 2,5 минуты (SecondsPerPhoneBetweenMessage = 30s)
            var dateFrom = DateTime.Now.AddSeconds(-minutes * 60);

            var count = callLogs.Count(x => x.Phone == phone && x.Ip == ip && x.CreatedOn >= dateFrom);
            
            // если за 3 мин было > 5 запросов
            if (count > allowedCount)
            {
                CallBanService.Ban(phone, ip, DateTime.Now.AddHours(3*24));
                return false;
            }
            
            // если за час было > 5 запросов
            if (callLogs.Count(x => x.Phone == phone && x.Ip == ip && x.Status == CallAuthStatus.Sent) > 5)
            {
                CallBanService.Ban(phone, ip, DateTime.Now.AddHours(24));
                return false;
            }

            return true;
        }
        
        public bool IsBanned(long phone, string ip) => CallBanService.IsBannedByPhoneOrIp(phone, ip);
    }
}