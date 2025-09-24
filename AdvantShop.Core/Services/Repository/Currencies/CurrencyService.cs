//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Repository.Currencies
{
    public class CurrencyService
    {
        private const string CurrencyCookieName = "Currency";

        private static Currency GetCurrencyFromReader(IDataReader reader)
        {
            return new Currency()
            {
                CurrencyId = SQLDataHelper.GetInt(reader, "CurrencyID"),
                Rate = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                Iso3 = SQLDataHelper.GetString(reader, "CurrencyIso3"),
                Symbol = SQLDataHelper.GetString(reader, "Code"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                IsCodeBefore = SQLDataHelper.GetBoolean(reader, "IsCodeBefore"),
                NumIso3 = SQLDataHelper.GetInt(reader, "CurrencyNumIso3"),
                RoundNumbers = SQLDataHelper.GetFloat(reader, "RoundNumbers"),
                EnablePriceRounding = SQLDataHelper.GetBoolean(reader, "EnablePriceRounding")
            };
        }

        public static Currency CurrentCurrency
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var contextCurrency = HttpContext.Current.Items["Currency"] as Currency;
                    if (contextCurrency != null)
                        return contextCurrency;

                    var cookieCurrency = CommonHelper.GetCookie(CurrencyCookieName);
                    var isCookieCurrencyExist = cookieCurrency != null && !string.IsNullOrEmpty(cookieCurrency.Value);

                    var iso3 = isCookieCurrencyExist
                                ? cookieCurrency.Value
                                : SettingsCatalog.DefaultCurrencyIso3;

                    contextCurrency = Currency(iso3) ?? Currency(SettingsCatalog.DefaultCurrencyIso3) ??
                        GetAllCurrencies().FirstOrDefault();

                    if (contextCurrency != null && (!isCookieCurrencyExist || (contextCurrency.Iso3 != cookieCurrency.Value)))
                        CommonHelper.SetCookie(CurrencyCookieName, contextCurrency.Iso3.ToUpper(), crossSubDomains:false);

                    HttpContext.Current.Items["Currency"] = contextCurrency;

                    return contextCurrency;
                }

                var currency = Currency(SettingsCatalog.DefaultCurrencyIso3) ?? GetAllCurrencies().FirstOrDefault();
                return currency;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items["Currency"] = value;
                    CommonHelper.SetCookie(CurrencyCookieName, value.Iso3.ToUpper(), crossSubDomains: false);
                }
            }
        }

        public static Currency BaseCurrency
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var baseCurrency = HttpContext.Current.Items["BaseCurrency"] as Currency;
                    if (baseCurrency != null)
                        return baseCurrency;

                    baseCurrency = GetAllCurrencies().FirstOrDefault(x => x.Rate == 1f);

                    HttpContext.Current.Items["BaseCurrency"] = baseCurrency;

                    return baseCurrency;
                }

                var currency = GetAllCurrencies().FirstOrDefault(x => x.Rate == 1f);
                return currency;
            }
        }

        public static Currency Currency(string iso3)
        {
            return GetAllCurrencies(true).FirstOrDefault(x => x.Iso3.Equals(iso3, StringComparison.OrdinalIgnoreCase));
        }

        public static List<Currency> GetAllCurrencies(bool fromCache)
        {
            if (!fromCache) return GetAllCurrencies();

            var cacheName = CacheNames.GetCurrenciesCacheObjectName();
            List<Currency> res;

            if (!CacheManager.TryGetValue(cacheName, out res))
            {
                res = GetAllCurrencies();

                if (res != null)
                    CacheManager.Insert(cacheName, res, 360);
            }

            return res;
        }

        public static List<Currency> GetAllCurrencies()
        {
            return SQLDataAccess.ExecuteReadList<Currency>("SELECT Catalog.Currency.* FROM Catalog.Currency", CommandType.Text, GetCurrencyFromReader);
        }


        public static Currency GetCurrency(int idCurrency, bool fromCache)
        {
            if (!fromCache)
                return GetCurrency(idCurrency);

            return GetAllCurrencies(true).FirstOrDefault(cur => cur.CurrencyId == idCurrency);
        }

        public static Currency GetCurrency(int idCurrency)
        {
            return SQLDataAccess.ExecuteReadOne<Currency>("SELECT Catalog.Currency.* FROM Catalog.Currency where CurrencyID = @id", CommandType.Text, GetCurrencyFromReader, new SqlParameter("@id", idCurrency));
        }

        public static Currency GetCurrencyByIso3(string iso3)
        {
            return SQLDataAccess.ExecuteReadOne<Currency>("SELECT Catalog.Currency.* FROM Catalog.Currency where CurrencyIso3 = @iso3", CommandType.Text, GetCurrencyFromReader, new SqlParameter("@iso3", iso3));
        }

        public static bool DeleteCurrency(int idCurrency)
        {
            try
            {
                SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[Currency] where CurrencyID = @id", CommandType.Text, new SqlParameter("@id", idCurrency));

                CacheManager.RemoveByPattern(CacheNames.GetCurrenciesCacheObjectName());
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return false;
        }

        public static void UpdateCurrency(Currency currency)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Catalog].[Currency] " +
                "Set Name=@name, Code=@code, CurrencyValue=@value, CurrencyIso3=@ISO3, CurrencyNumIso3=@CurrencyNumIso3, IsCodeBefore=@isCodeBefore, RoundNumbers=@RoundNumbers, EnablePriceRounding=@EnablePriceRounding " +
                "Where CurrencyID = @id",
                CommandType.Text,
                new SqlParameter("@id", currency.CurrencyId),
                new SqlParameter("@name", currency.Name),
                new SqlParameter("@code", currency.Symbol),
                new SqlParameter("@value", currency.Rate),
                new SqlParameter("@ISO3", currency.Iso3),
                new SqlParameter("@CurrencyNumIso3", currency.NumIso3 != 0 ? currency.NumIso3 : new Random().Next(1, 999)),
                new SqlParameter("@isCodeBefore", currency.IsCodeBefore),
                new SqlParameter("@RoundNumbers", currency.RoundNumbers),
                new SqlParameter("@EnablePriceRounding", currency.EnablePriceRounding));

            CacheManager.RemoveByPattern(CacheNames.GetCurrenciesCacheObjectName());
            CacheManager.RemoveByPattern(CacheNames.SQLPagingItems);
            CacheManager.RemoveByPattern(CacheNames.SQLPagingCount);
        }

        public static void InsertCurrency(Currency currency)
        {
            currency.CurrencyId = SQLDataAccess.ExecuteScalar<int>(
                 "INSERT INTO [Catalog].[Currency] (Name, Code, CurrencyValue, CurrencyIso3, CurrencyNumIso3, IsCodeBefore, RoundNumbers, EnablePriceRounding) " +
                 "VALUES (@Name, @Code, @CurrencyValue, @CurrencyIso3, @CurrencyNumIso3, @IsCodeBefore, @RoundNumbers, @EnablePriceRounding);" +
                 "SELECT scope_identity();",
                 CommandType.Text,
                 new SqlParameter("@Name", currency.Name),
                 new SqlParameter("@Code", currency.Symbol),
                 new SqlParameter("@CurrencyValue", currency.Rate),
                 new SqlParameter("@CurrencyIso3", currency.Iso3),
                 new SqlParameter("@CurrencyNumIso3", currency.NumIso3 != 0 ? (object)currency.NumIso3 : DBNull.Value),
                 new SqlParameter("@isCodeBefore", currency.IsCodeBefore),
                 new SqlParameter("@RoundNumbers", currency.RoundNumbers),
                 new SqlParameter("@EnablePriceRounding", currency.EnablePriceRounding));

            CacheManager.RemoveByPattern(CacheNames.GetCurrenciesCacheObjectName());
        }

        public static bool UpdateCurrenciesFromCentralBank()
        {
            try
            {
                var request = WebRequest.Create("http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + DateTime.Now.ToString("dd/MM/yyyy")) as HttpWebRequest;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";

                var responseContent = "";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                responseContent = reader.ReadToEnd();
                            }
                    }
                }

                if (responseContent.IsNullOrEmpty())
                    return false;

                var doc = XDocument.Parse(responseContent);
                var currenciesCentralBank = new List<CurrencyCentralBank>();
                foreach (var el in doc.Root.Elements("Valute"))
                    currenciesCentralBank.Add(new CurrencyCentralBank
                    {
                        Iso3 = el.Element("CharCode").Value,
                        NumIso3 = el.Element("NumCode").Value.TryParseInt(),
                        Rate = el.Element("Value").Value.TryParseFloat(),
                        Nominal = el.Element("Nominal").Value.TryParseFloat()
                    });

                var rubScale = 1f; // ЦБ возвращает курсы относительно рубля


                var baseCurrency = GetAllCurrencies()
                    .OrderBy(x => "RUB".Equals(x.Iso3, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                    .FirstOrDefault(x => x.Rate == 1f);

                var baseCurrencyIso3 = baseCurrency != null ? baseCurrency.Iso3 : string.Empty;

                var rubIsNotBase = !"RUB".Equals(baseCurrencyIso3, StringComparison.OrdinalIgnoreCase);
                if (rubIsNotBase)
                {
                    var centralBankBaseCurrency = currenciesCentralBank.FirstOrDefault(x =>
                        x.Iso3.Equals(baseCurrencyIso3, StringComparison.OrdinalIgnoreCase));

                    if (centralBankBaseCurrency != null)
                    {
                        rubScale = 1 / centralBankBaseCurrency.Rate * centralBankBaseCurrency.Nominal; //получаем курс рубля относительно базовой валюты

                        SQLDataAccess.ExecuteNonQuery(
                            "UPDATE Catalog.Currency SET CurrencyValue=@CurrencyValue WHERE CurrencyIso3=@CurrencyIso3",
                            CommandType.Text,
                            new SqlParameter("@CurrencyValue", rubScale),
                            new SqlParameter("@CurrencyIso3", "RUB"));
                    }
                    else
                    {
                        // базовая валюта не нашлся среди валют ЦБ
                        // скорее всего это какие-то условные единицы
                        // поэтому оталкивается от курса рубля
                        var rub = GetCurrencyByIso3("RUB");
                        if (rub != null)
                            rubScale = rub.Rate;
                    }
                }

                var currencies = SQLDataAccess.ExecuteTable("SELECT CurrencyIso3,CurrencyValue FROM Catalog.Currency", CommandType.Text);

                for (int i = 0; i < currencies.Rows.Count; i++)
                {
                    foreach (var currencyCentralBank in currenciesCentralBank)
                    {
                        if (currencies.Rows[i]["CurrencyIso3"].ToString().ToLower() != currencyCentralBank.Iso3.ToLower()) continue;

                        if (currencies.Rows[i]["CurrencyIso3"].ToString().ToLower() != baseCurrencyIso3.ToLower())
                        {
                            SQLDataAccess.ExecuteNonQuery(
                                "UPDATE Catalog.Currency SET CurrencyValue=@CurrencyValue, CurrencyNumIso3=@CurrencyNumIso3 WHERE CurrencyIso3=@CurrencyIso3",
                                CommandType.Text,
                                new SqlParameter("@CurrencyValue",
                                    currencyCentralBank.Rate / currencyCentralBank.Nominal * rubScale),
                                new SqlParameter("@CurrencyIso3", currencyCentralBank.Iso3),
                                new SqlParameter("@CurrencyNumIso3", currencyCentralBank.NumIso3));
                        }
                        else
                        {
                            // базовую валюту обновляем всегда еденицей,
                            // т.к. при расчете может быть погрешность
                            // и получается близкое значение к еденице, но не 1
                            SQLDataAccess.ExecuteNonQuery(
                                "UPDATE Catalog.Currency SET CurrencyValue=@CurrencyValue, CurrencyNumIso3=@CurrencyNumIso3 WHERE CurrencyIso3=@CurrencyIso3",
                                CommandType.Text,
                                new SqlParameter("@CurrencyValue", 1f),
                                new SqlParameter("@CurrencyIso3", currencyCentralBank.Iso3),
                                new SqlParameter("@CurrencyNumIso3", currencyCentralBank.NumIso3));
                        }
                    }
                }

                CacheManager.RemoveByPattern(CacheNames.GetCurrenciesCacheObjectName());

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return false;
            }
        }

        public static float ConvertCurrency(float sum, float newCurrencyValue, float oldCurrencyValue)
        {
            return oldCurrencyValue == newCurrencyValue
                ? sum
                : sum * oldCurrencyValue / newCurrencyValue;
        }
    }
}

