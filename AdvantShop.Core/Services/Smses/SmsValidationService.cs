using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdvantShop.Core.Services.Auth;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Loging.Smses;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;

namespace AdvantShop.Core.Services.Smses
{
    public class SmsValidationService
    {
        private readonly bool _isInternal;
        private static readonly Regex PhoneValid = new Regex("^([0-9]{10,15})$", RegexOptions.Compiled | RegexOptions.Singleline);

        public SmsValidationService(bool isInternal)
        {
            _isInternal = isInternal;
        }
        
        public bool Validate(long phone, string text, string ip, bool throwError)
        {
            if (!IsValidSmsText(text))
            {
                if (throwError)
                    throw new BlException("Укажите валидный текст sms");
                return false;
            }
            
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
                    LogError("Слишком много sms-запросов", throwError, phone, ip);
                    return false;
                }

                var hour = 1 * 60 * 60;
                var smsLogs = SmsLogger.GetLastSmsLogsByPhoneOrIp(hour, phone, ip);

                if (!CheckByTimeBetweenSms(phone, ip, smsLogs))
                {
                    LogError($"Следующее sms можно будет послать через {PhoneConfirmationConfig.SecondsPerPhoneBetweenMessage}с",
                        throwError, phone, ip);
                    return false;
                }

                if (!CheckByIp(ip, smsLogs))
                {
                    LogError("Слишком много sms-запросов. Попробуйте позже.", throwError, phone, ip);
                    return false;
                }

                if (!CheckByCountBetweenSms(phone, ip, smsLogs))
                {
                    LogError("Слишком много sms-запросов. Попробуйте позже.", throwError, phone, ip);
                    return false;
                }
            }

            return true;
        }

        private void LogError(string error, bool throwError, long phone, string ip)
        {
            Debug.Log.Warn($"sms abuse {phone} {ip} {error}");
                
            if (throwError)
                throw new BlException(error);
        }
        
        public bool IsValidSmsText(string text)
        {
            return !string.IsNullOrWhiteSpace(text);
        }
        
        private static bool IsValidPhone(long phone)
        {
            return PhoneValid.IsMatch(phone.ToString());
        }
        
        /// <summary>
        /// Проверка что прошло разрешенное время с последнего смс для телефона 
        /// </summary>
        public bool CheckByTimeBetweenSms(long phone, string ip, List<SmsLogData> smsLogs)
        {
            var lastLogData = smsLogs.FindLast(x => x.Phone == phone && x.Ip == ip && x.Status == SmsStatus.Sent);
            var now = DateTime.Now;

            var check = 
                lastLogData == null ||
                lastLogData.CreatedOn.AddSeconds(PhoneConfirmationConfig.SecondsPerPhoneBetweenMessage) < now;

            if (!check)
            {
                SmsLogger.Log(
                    new SmsLogData(phone, "", ip,
                        CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.Id : default(Guid?),
                        SmsStatus.Fault));

                if (SettingsSms.SmsBanLevel == SmsBanLevel.High)
                {
                    // если предыдущий запрос был < 26c
                    if (lastLogData.CreatedOn.AddSeconds(PhoneConfirmationConfig.SecondsPerPhoneBetweenMessage - 4) < now)
                    {
                        Debug.Log.Warn($"sms abuse ban по номеру+ip: предыдущий запрос был < 26c {phone} {ip} ");
                        SmsBanService.Ban(phone, ip, DateTime.Now.AddHours(6));
                        return false;
                    }
                    
                    // если с ip было много запросов
                    if (smsLogs.Count(x => x.Ip == ip) >= 10)
                    {
                        Debug.Log.Warn($"sms abuse ban ip: с ip было много запросов >= 10 {phone} {ip} ");
                        SmsBanService.Ban(null, ip, DateTime.Now.AddHours(6));
                        return false;
                    }
                }
            }

            return check;
        }
        
        /// <summary>
        /// Проверка что не было слишком много попыток.
        /// Запрет абьюзить "посылать запросы каждые {SmsNotifierConfig.SecondsPerPhoneBetweenSms} секунд"  
        /// </summary>
        public bool CheckByCountBetweenSms(long phone, string ip, List<SmsLogData> smsLogs)
        {
            var minutes = 3;
            var allowedCount = 5; // 5 попыток = 2,5 минуты (SecondsPerPhoneBetweenSms = 30s)
            var dateFrom = DateTime.Now.AddSeconds(-minutes * 60);

            var count = smsLogs.Count(x => x.Phone == phone && x.Ip == ip && x.CreatedOn >= dateFrom);
            
            // если за 3 мин было > 5 запросов
            if (count > allowedCount)
            {
                SmsBanService.Ban(phone, ip, DateTime.Now.AddHours(3*24));
                return false;
            }
            
            // если за час было > 5 запросов
            if (smsLogs.Count(x => x.Phone == phone && x.Ip == ip && x.Status == SmsStatus.Sent) > 5)
            {
                SmsBanService.Ban(phone, ip, DateTime.Now.AddHours(24));
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Проверка что с одного ip не идет много запросов
        /// </summary>
        public bool CheckByIp(string ip, List<SmsLogData> smsLogs)
        {
            if (string.IsNullOrEmpty(ip) || ip == "::1" || ip == "127.0.0.1")
                return true;

            var seconds = 5;
            var dateFrom = DateTime.Now.AddSeconds(-seconds);
            
            var countPerSeconds = smsLogs.Count(x => x.Ip == ip && x.CreatedOn >= dateFrom);

            // если с одного ip за 5с было 5 запросов, то баним ip
            if (countPerSeconds > PhoneConfirmationConfig.RequestsCountPerSecondByIp * seconds)
            {
                SmsBanService.Ban(null, ip, DateTime.Now.AddMinutes(10));
                return false;
            }

            dateFrom = DateTime.Now.AddSeconds(-3 * 60);
            countPerSeconds = smsLogs.Count(x => x.Ip == ip && x.CreatedOn >= dateFrom);

            // если с одного ip за 3 минуты было 30 запросов, то баним ip
            if (countPerSeconds >= 30)
            {
                SmsBanService.Ban(null, ip, DateTime.Now.AddHours(6));
                return false;
            }
            
            if (SettingsSms.SmsBanLevel == SmsBanLevel.High)
            {
                countPerSeconds = SmsLogger.GetLogsByIpAndSeconds(ip, 30 * 60).Count;
                
                // (много запросов) если с ip было >= 8 запросов за 30 минут
                if (countPerSeconds >= 8)
                {
                    Debug.Log.Warn($"sms abuse ban: с ip ({ip}) было >= 8 ({countPerSeconds}) запросов за 30 минут");
                    SmsBanService.Ban(null, ip, DateTime.Now.AddHours(3*24));
                    return false;
                }
            }

            if (!CheckByIpNSec(ip, smsLogs))
                return false;
            
            return true;
        }

        /// <summary>
        /// Проверка если будут посылать с одного ip каждые n-секунд на разные телефоны
        /// </summary>
        private bool CheckByIpNSec(string ip, List<SmsLogData> smsLogs)
        {
            var logs = smsLogs.Where(x => x.Ip == ip).OrderByDescending(x => x.CreatedOn).ToList();
            
            if (logs.Count < 7) 
                return true;
            
            var i = 0;
            var matchCount = 0;
            var previousDate = DateTime.MinValue;
            var previousDiff = TimeSpan.Zero;

            try
            {
                var deltaTicks = TimeSpan.FromSeconds(1).Ticks;
                
                foreach (var sms in logs)
                {
                    if (i != 0)
                    {
                        var diff = previousDate - sms.CreatedOn;
                        
                        // если интервалы примерно развны (погрешность 1с), то увеличиваем счетчик
                        if (Math.Abs((diff - previousDiff).Ticks) < deltaTicks)
                            matchCount++;
                        
                        previousDiff = diff;
                    }
                    
                    previousDate = sms.CreatedOn;
                    i++;
                    
                    // если 70% смс посылались с одинаковым интревалом, то баним ip
                    if (matchCount == (int)(logs.Count*0.7))
                    {
                        SmsBanService.Ban(null, ip, DateTime.Now.AddMinutes(30));
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

        public bool IsBanned(long phone, string ip) => SmsBanService.IsBannedByPhoneOrIp(phone, ip);
    }
}