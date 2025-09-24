//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Repository;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace AdvantShop.Repository
{
    public class CountryService
    {
        private const string CountryCacheKey = "Country_";

        public static List<Country> GetAllCountries()
        {
            const string cacheKey = CountryCacheKey + "All";

            return CacheManager.Get(cacheKey,
                () =>
                    SQLDataAccess.ExecuteReadList("SELECT * FROM [Customers].[Country] ORDER BY [CountryName] ASC",
                        CommandType.Text, GetCountryFromReader));
        }

        public static List<Country> GetAllCountryIdAndName()
        {
            return SQLDataAccess.ExecuteReadList("SELECT CountryID,CountryName FROM [Customers].[Country]",
                CommandType.Text,
                reader => new Country
                {
                    CountryId = SQLDataHelper.GetInt(reader, "CountryID"),
                    Name = SQLDataHelper.GetString(reader, "CountryName")
                });
        }

        public static List<Country> GetCountriesByDisplayInPopup()
        {
            return CacheManager.Get(CountryCacheKey + "DisplayInPopup",
                () =>
                    SQLDataAccess.ExecuteReadList(
                        "Select top 12 * From Customers.Country Where DisplayInPopup=1 Order By SortOrder desc, CountryName asc",
                        CommandType.Text, GetCountryFromReader));
        }

        #region Update / Add / Delete Country

        public static void Delete(int countryId)
        {
            if (countryId != SettingsMain.SellerCountryId)
            {
                SQLDataAccess.ExecuteNonQuery("DELETE FROM [Customers].[Country] where CountryID = @CountryId",
                    CommandType.Text, new SqlParameter("@CountryId", countryId));

                AdditionalOptionsService.Delete(countryId, EnAdditionalOptionObjectType.Country);
                CacheManager.RemoveByPattern(CountryCacheKey);
            }
        }

        public static void Add(Country country)
        {
            country.CountryId =
                SQLDataAccess.ExecuteScalar<int>(
                    "INSERT INTO [Customers].[Country] (CountryName, CountryISO2, CountryISO3, DisplayInPopup,SortOrder,DialCode) VALUES (@Name, @ISO2, @ISO3, @DisplayInPopup,@SortOrder,@DialCode); SELECT scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@Name", country.Name),
                    new SqlParameter("@ISO2", country.Iso2),
                    new SqlParameter("@ISO3", country.Iso3),
                    new SqlParameter("@DisplayInPopup", country.DisplayInPopup),
                    new SqlParameter("@SortOrder", country.SortOrder),
                    new SqlParameter("@DialCode", country.DialCode ?? (object)DBNull.Value)
                    );
            CacheManager.RemoveByPattern(CountryCacheKey);
        }

        public static void Update(Country country)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[Country] set CountryName=@name, CountryISO2=@ISO2, CountryISO3=@ISO3, DisplayInPopup=@DisplayInPopup, SortOrder=@SortOrder, DialCode=@DialCode Where CountryID = @id",
                CommandType.Text,
                new SqlParameter("@id", country.CountryId),
                new SqlParameter("@name", country.Name),
                new SqlParameter("@ISO2", country.Iso2),
                new SqlParameter("@ISO3", country.Iso3),
                new SqlParameter("@DisplayInPopup", country.DisplayInPopup),
                new SqlParameter("@SortOrder", country.SortOrder),
                new SqlParameter("@DialCode", country.DialCode ?? (object)DBNull.Value));

            CacheManager.RemoveByPattern(CountryCacheKey);
        }

        #endregion


        public static string GetIso2(string name)
        {
            return
                SQLDataAccess.ExecuteScalar<string>(
                    "SELECT [CountryISO2] FROM [Customers].[Country] Where CountryName = @CountryName",
                    CommandType.Text, new SqlParameter("@CountryName", name));
        }

        public static string GetIso3(string name)
        {
            return
                SQLDataAccess.ExecuteScalar<string>(
                    "SELECT [CountryISO3] FROM [Customers].[Country] Where CountryName = @CountryName",
                    CommandType.Text, new SqlParameter("@CountryName", name));
        }

        public static Country GetCountry(int id)
        {
            var cacheKey = CountryCacheKey + id;

            var country =
                CacheManager.Get<Country>(cacheKey,
                    () => SQLDataAccess.ExecuteReadOne("SELECT * FROM [Customers].[Country] Where CountryID = @id",
                        CommandType.Text, GetCountryFromReader, new SqlParameter("@id", id)));

            return country;
        }

        public static Country GetCountryByName(string countryName)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM [Customers].[Country] Where CountryName = @CountryName",
                CommandType.Text, GetCountryFromReader, new SqlParameter("@CountryName", countryName));
        }

        public static Country GetCountryByIso2(string iso2)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM [Customers].[Country] Where CountryISO2 = @iso2",
                CommandType.Text, GetCountryFromReader, new SqlParameter("@iso2", iso2));
        }


        public static Country GetCountryFromReader(SqlDataReader reader)
        {
            return new Country
            {
                CountryId = SQLDataHelper.GetInt(reader, "CountryID"),
                Iso2 = SQLDataHelper.GetString(reader, "CountryISO2"),
                Iso3 = SQLDataHelper.GetString(reader, "CountryISO3"),
                Name = SQLDataHelper.GetString(reader, "CountryName"),
                DisplayInPopup = SQLDataHelper.GetBoolean(reader, "DisplayInPopup"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                DialCode = SQLDataHelper.GetNullableInt(reader, "DialCode")
            };
        }


        //public static string GetCountryNameById(int countryId)
        //{
        //    return SQLDataAccess.ExecuteScalar<string>(
        //        "SELECT CountryName FROM Customers.Country Where CountryID = @id",
        //        CommandType.Text, new SqlParameter("@id", countryId));
        //}

        //public static string GetCountryIso2ById(int countryId)
        //{
        //    return SQLDataAccess.ExecuteScalar<string>(
        //        "SELECT CountryISO2 FROM Customers.Country Where CountryID = @id",
        //        CommandType.Text, new SqlParameter("@id", countryId));
        //}

        //public static List<int> GetCountryIdByIp(string Ip)
        //{
        //    long ipDec;
        //    try
        //    {
        //        if (Ip == "::1")
        //            ipDec = 127 * 16777216 + 1;
        //        else
        //        {
        //            string[] ip = Ip.Split('.');
        //            ipDec = (Int32.Parse(ip[0])) * 16777216 + (Int32.Parse(ip[1])) * 65536 + (Int32.Parse(ip[2])) * 256 + Int32.Parse(ip[3]);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        ipDec = 127 * 16777216 + 1;
        //    }
        //    List<int> ids = SQLDataAccess.ExecuteReadList<int>("SELECT CountryID FROM Customers.Country Where CountryISO2 = (SELECT country_code FROM Customers.GeoIP Where begin_num <= @IP AND end_num >= @IP)",
        //                                                 CommandType.Text,
        //                                                 reader => SQLDataHelper.GetInt(reader, "CountryID"), new SqlParameter("@IP", ipDec));
        //    return ids;
        //}

        //public static List<string> GetCountryNameByIp(string Ip)
        //{
        //    long ipDec;
        //    try
        //    {
        //        if (Ip == "::1")
        //            ipDec = 127 * 16777216 + 1;
        //        else
        //        {
        //            string[] ip = Ip.Split('.');
        //            ipDec = (Int32.Parse(ip[0])) * 16777216 + (Int32.Parse(ip[1])) * 65536 + (Int32.Parse(ip[2])) * 256 + Int32.Parse(ip[3]);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        ipDec = 127 * 16777216 + 1;
        //    }

        //    List<string> listNames = SQLDataAccess.ExecuteReadList<string>("SELECT * FROM Customers.Country WHERE CountryISO2 = (SELECT country_code FROM Customers.GeoIP WHERE begin_num <= @IP AND end_num >= @IP)",
        //                                                                   CommandType.Text, reader => SQLDataHelper.GetString(reader, "CountryName"),
        //                                                                   new SqlParameter("@IP", ipDec)) ?? new List<string> { { "local" } };

        //    if (listNames.Count == 0)
        //        listNames.Add("local");

        //    return listNames;
        //}

        public static int GetCountryIdByName(string countryName)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT CountryID FROM Customers.Country Where CountryName = @name",
                CommandType.Text, new SqlParameter("@name", countryName));
        }

        public static List<string> GetCountriesByName(string name)
        {
            var translit = StringHelper.TranslitToRusKeyboard(name);

            return
                SQLDataAccess.ExecuteReadList(
                    "Select CountryName From Customers.Country Where CountryName like @name + '%' or CountryName like @trname + '%'",
                    CommandType.Text,
                    reader => SQLDataHelper.GetString(reader, "CountryName"), 
                    new SqlParameter("@name", name),
                    new SqlParameter("@trname", translit));
        }

        #region AdditionalSettings

        public static void UpdateAdditionalSettings(CountryAdditionalSettings additionalSettings, bool cleanCache = false)
        {
            AdditionalOptionsService.AddOrUpdate(new AdditionalOption()
            {
                ObjId = additionalSettings.CountryId,
                ObjType = EnAdditionalOptionObjectType.Country,
                Name = CountryAdditionalOptionNames.AllowSiteBrowsing,
                Value = additionalSettings.AllowSiteBrowsing.ToString()
            });
            if (cleanCache)
                CacheManager.RemoveByPattern(CountryCacheKey);
        }

        public static void DeleteAdditionalSetting(int countryId, string settingName, bool cleanCache = false)
        {
            AdditionalOptionsService.Delete(countryId, EnAdditionalOptionObjectType.Country, settingName);
            if (cleanCache)
                CacheManager.RemoveByPattern(CountryCacheKey);
        }

        #endregion

        #region Helps Methods

        public static bool IsAllowSiteBrowsingByIp(string ip)
        {
            if (BrowsersHelper.IsBot() && SettingsGeneral.AllowSearchBotsFromOtherCountries)
                return true;
            var saasData = SaasDataService.CurrentSaasData;
            return CacheManager.Get($"{CountryCacheKey}AccessByIp_{ip}_{saasData.IpWhiteListValues?.GetHashCode()}", 60*29,
                    () =>
                    {
                        if (saasData.IpWhiteListValues?.Any(x => UrlService.IsInSubnet(ip, x)) is true)
                            return true;
                        var allowedSiteBrowsingCountries = new List<int>();
                        foreach (var option in AdditionalOptionsService.Get(EnAdditionalOptionObjectType.Country, CountryAdditionalOptionNames.AllowSiteBrowsing))
                            if (option.ObjId != 0 && option.Value.TryParseBool())
                                allowedSiteBrowsingCountries.Add(option.ObjId);
                        
                        if (allowedSiteBrowsingCountries.Count == 0)
                            return true;
                        int? countryId = GetCountryIdByIp(ip);
                        if (!countryId.HasValue || countryId == 0)
                            return true;

                        return allowedSiteBrowsingCountries.Contains(countryId.Value);
                    });
        }

        public static int? GetCountryIdByIp(string ip)
        {
            if (ip.IsLocalIP())
                return null;

            var modules = AttachedModules.GetModuleInstances<IGeoIp>();
            if (modules != null && modules.Count != 0)
            {
                foreach (var module in modules)
                {
                    var moduleZone = module.GetIpZone(ip);
                    if (moduleZone != null && moduleZone.CountryId != 0)
                        return moduleZone.CountryId;
                }
            }

            var geoIpData = GeoIpService.GetGeoIpData(ip);
            if (geoIpData != null && geoIpData.Country.IsNotEmpty())
            {
                var country = GetCountryByIso2(geoIpData.Country);
                return country?.CountryId;
            }
            return null;
        }

        public static string Iso2ToIso3(string iso2)
        {
            if (iso2.IsNullOrEmpty())
                return null;

            switch (iso2.ToLower())
            {
                case "af": return "AFG";
                case "al": return "ALB";
                case "dz": return "DZA";
                case "as": return "ASM";
                case "ad": return "AND";
                case "ao": return "AGO";
                case "ai": return "AIA";
                case "aq": return "ATA";
                case "ag": return "ATG";
                case "ar": return "ARG";
                case "am": return "ARM";
                case "aw": return "ABW";
                case "au": return "AUS";
                case "at": return "AUT";
                case "az": return "AZE";
                case "bs": return "BHS";
                case "bh": return "BHR";
                case "bd": return "BGD";
                case "bb": return "BRB";
                case "by": return "BLR";
                case "be": return "BEL";
                case "bz": return "BLZ";
                case "bj": return "BEN";
                case "bm": return "BMU";
                case "bt": return "BTN";
                case "bo": return "BOL";
                case "bq": return "BES";
                case "ba": return "BIH";
                case "bw": return "BWA";
                case "bv": return "BVT";
                case "br": return "BRA";
                case "io": return "IOT";
                case "bn": return "BRN";
                case "bg": return "BGR";
                case "bf": return "BFA";
                case "bi": return "BDI";
                case "cv": return "CPV";
                case "kh": return "KHM";
                case "cm": return "CMR";
                case "ca": return "CAN";
                case "ky": return "CYM";
                case "cf": return "CAF";
                case "td": return "TCD";
                case "cl": return "CHL";
                case "cn": return "CHN";
                case "cx": return "CXR";
                case "cc": return "CCK";
                case "co": return "COL";
                case "km": return "COM";
                case "cd": return "COD";
                case "cg": return "COG";
                case "ck": return "COK";
                case "cr": return "CRI";
                case "hr": return "HRV";
                case "cu": return "CUB";
                case "cw": return "CUW";
                case "cy": return "CYP";
                case "cz": return "CZE";
                case "ci": return "CIV";
                case "dk": return "DNK";
                case "dj": return "DJI";
                case "dm": return "DMA";
                case "do": return "DOM";
                case "ec": return "ECU";
                case "eg": return "EGY";
                case "sv": return "SLV";
                case "gq": return "GNQ";
                case "er": return "ERI";
                case "ee": return "EST";
                case "sz": return "SWZ";
                case "et": return "ETH";
                case "fk": return "FLK";
                case "fo": return "FRO";
                case "fj": return "FJI";
                case "fi": return "FIN";
                case "fr": return "FRA";
                case "gf": return "GUF";
                case "pf": return "PYF";
                case "tf": return "ATF";
                case "ga": return "GAB";
                case "gm": return "GMB";
                case "ge": return "GEO";
                case "de": return "DEU";
                case "gh": return "GHA";
                case "gi": return "GIB";
                case "gr": return "GRC";
                case "gl": return "GRL";
                case "gd": return "GRD";
                case "gp": return "GLP";
                case "gu": return "GUM";
                case "gt": return "GTM";
                case "gg": return "GGY";
                case "gn": return "GIN";
                case "gw": return "GNB";
                case "gy": return "GUY";
                case "ht": return "HTI";
                case "hm": return "HMD";
                case "va": return "VAT";
                case "hn": return "HND";
                case "hk": return "HKG";
                case "hu": return "HUN";
                case "is": return "ISL";
                case "in": return "IND";
                case "id": return "IDN";
                case "ir": return "IRN";
                case "iq": return "IRQ";
                case "ie": return "IRL";
                case "im": return "IMN";
                case "il": return "ISR";
                case "it": return "ITA";
                case "jm": return "JAM";
                case "jp": return "JPN";
                case "je": return "JEY";
                case "jo": return "JOR";
                case "kz": return "KAZ";
                case "ke": return "KEN";
                case "ki": return "KIR";
                case "kp": return "PRK";
                case "kr": return "KOR";
                case "kw": return "KWT";
                case "kg": return "KGZ";
                case "la": return "LAO";
                case "lv": return "LVA";
                case "lb": return "LBN";
                case "ls": return "LSO";
                case "lr": return "LBR";
                case "ly": return "LBY";
                case "li": return "LIE";
                case "lt": return "LTU";
                case "lu": return "LUX";
                case "mo": return "MAC";
                case "mg": return "MDG";
                case "mw": return "MWI";
                case "my": return "MYS";
                case "mv": return "MDV";
                case "ml": return "MLI";
                case "mt": return "MLT";
                case "mh": return "MHL";
                case "mq": return "MTQ";
                case "mr": return "MRT";
                case "mu": return "MUS";
                case "yt": return "MYT";
                case "mx": return "MEX";
                case "fm": return "FSM";
                case "md": return "MDA";
                case "mc": return "MCO";
                case "mn": return "MNG";
                case "me": return "MNE";
                case "ms": return "MSR";
                case "ma": return "MAR";
                case "mz": return "MOZ";
                case "mm": return "MMR";
                case "na": return "NAM";
                case "nr": return "NRU";
                case "np": return "NPL";
                case "nl": return "NLD";
                case "nc": return "NCL";
                case "nz": return "NZL";
                case "ni": return "NIC";
                case "ne": return "NER";
                case "ng": return "NGA";
                case "nu": return "NIU";
                case "nf": return "NFK";
                case "mk": return "MKD";
                case "mp": return "MNP";
                case "no": return "NOR";
                case "om": return "OMN";
                case "pk": return "PAK";
                case "pw": return "PLW";
                case "ps": return "PSE";
                case "pa": return "PAN";
                case "pg": return "PNG";
                case "py": return "PRY";
                case "pe": return "PER";
                case "ph": return "PHL";
                case "pn": return "PCN";
                case "pl": return "POL";
                case "pt": return "PRT";
                case "pr": return "PRI";
                case "qa": return "QAT";
                case "ro": return "ROU";
                case "ru": return "RUS";
                case "rw": return "RWA";
                case "re": return "REU";
                case "bl": return "BLM";
                case "sh": return "SHN";
                case "kn": return "KNA";
                case "lc": return "LCA";
                case "mf": return "MAF";
                case "pm": return "SPM";
                case "vc": return "VCT";
                case "ws": return "WSM";
                case "sm": return "SMR";
                case "st": return "STP";
                case "sa": return "SAU";
                case "sn": return "SEN";
                case "rs": return "SRB";
                case "sc": return "SYC";
                case "sl": return "SLE";
                case "sg": return "SGP";
                case "sx": return "SXM";
                case "sk": return "SVK";
                case "si": return "SVN";
                case "sb": return "SLB";
                case "so": return "SOM";
                case "za": return "ZAF";
                case "gs": return "SGS";
                case "ss": return "SSD";
                case "es": return "ESP";
                case "lk": return "LKA";
                case "sd": return "SDN";
                case "sr": return "SUR";
                case "sj": return "SJM";
                case "se": return "SWE";
                case "ch": return "CHE";
                case "sy": return "SYR";
                case "tw": return "TWN";
                case "tj": return "TJK";
                case "tz": return "TZA";
                case "th": return "THA";
                case "tl": return "TLS";
                case "tg": return "TGO";
                case "tk": return "TKL";
                case "to": return "TON";
                case "tt": return "TTO";
                case "tn": return "TUN";
                case "tr": return "TUR";
                case "tm": return "TKM";
                case "tc": return "TCA";
                case "tv": return "TUV";
                case "ug": return "UGA";
                case "ua": return "UKR";
                case "ae": return "ARE";
                case "gb": return "GBR";
                case "um": return "UMI";
                case "us": return "USA";
                case "uy": return "URY";
                case "uz": return "UZB";
                case "vu": return "VUT";
                case "ve": return "VEN";
                case "vn": return "VNM";
                case "vg": return "VGB";
                case "vi": return "VIR";
                case "wf": return "WLF";
                case "eh": return "ESH";
                case "ye": return "YEM";
                case "zm": return "ZMB";
                case "zw": return "ZWE";
                case "ax": return "ALA";
            }

            return null;
        }

        public static string Iso3ToIso2(string iso3)
        {
            if (iso3.IsNullOrEmpty())
                return null;

            switch (iso3.ToLower())
            {
                case "afg": return "AF";
                case "alb": return "AL";
                case "dza": return "DZ";
                case "asm": return "AS";
                case "and": return "AD";
                case "ago": return "AO";
                case "aia": return "AI";
                case "ata": return "AQ";
                case "atg": return "AG";
                case "arg": return "AR";
                case "arm": return "AM";
                case "abw": return "AW";
                case "aus": return "AU";
                case "aut": return "AT";
                case "aze": return "AZ";
                case "bhs": return "BS";
                case "bhr": return "BH";
                case "bgd": return "BD";
                case "brb": return "BB";
                case "blr": return "BY";
                case "bel": return "BE";
                case "blz": return "BZ";
                case "ben": return "BJ";
                case "bmu": return "BM";
                case "btn": return "BT";
                case "bol": return "BO";
                case "bes": return "BQ";
                case "bih": return "BA";
                case "bwa": return "BW";
                case "bvt": return "BV";
                case "bra": return "BR";
                case "iot": return "IO";
                case "brn": return "BN";
                case "bgr": return "BG";
                case "bfa": return "BF";
                case "bdi": return "BI";
                case "cpv": return "CV";
                case "khm": return "KH";
                case "cmr": return "CM";
                case "can": return "CA";
                case "cym": return "KY";
                case "caf": return "CF";
                case "tcd": return "TD";
                case "chl": return "CL";
                case "chn": return "CN";
                case "cxr": return "CX";
                case "cck": return "CC";
                case "col": return "CO";
                case "com": return "KM";
                case "cod": return "CD";
                case "cog": return "CG";
                case "cok": return "CK";
                case "cri": return "CR";
                case "hrv": return "HR";
                case "cub": return "CU";
                case "cuw": return "CW";
                case "cyp": return "CY";
                case "cze": return "CZ";
                case "civ": return "CI";
                case "dnk": return "DK";
                case "dji": return "DJ";
                case "dma": return "DM";
                case "dom": return "DO";
                case "ecu": return "EC";
                case "egy": return "EG";
                case "slv": return "SV";
                case "gnq": return "GQ";
                case "eri": return "ER";
                case "est": return "EE";
                case "swz": return "SZ";
                case "eth": return "ET";
                case "flk": return "FK";
                case "fro": return "FO";
                case "fji": return "FJ";
                case "fin": return "FI";
                case "fra": return "FR";
                case "guf": return "GF";
                case "pyf": return "PF";
                case "atf": return "TF";
                case "gab": return "GA";
                case "gmb": return "GM";
                case "geo": return "GE";
                case "deu": return "DE";
                case "gha": return "GH";
                case "gib": return "GI";
                case "grc": return "GR";
                case "grl": return "GL";
                case "grd": return "GD";
                case "glp": return "GP";
                case "gum": return "GU";
                case "gtm": return "GT";
                case "ggy": return "GG";
                case "gin": return "GN";
                case "gnb": return "GW";
                case "guy": return "GY";
                case "hti": return "HT";
                case "hmd": return "HM";
                case "vat": return "VA";
                case "hnd": return "HN";
                case "hkg": return "HK";
                case "hun": return "HU";
                case "isl": return "IS";
                case "ind": return "IN";
                case "idn": return "ID";
                case "irn": return "IR";
                case "irq": return "IQ";
                case "irl": return "IE";
                case "imn": return "IM";
                case "isr": return "IL";
                case "ita": return "IT";
                case "jam": return "JM";
                case "jpn": return "JP";
                case "jey": return "JE";
                case "jor": return "JO";
                case "kaz": return "KZ";
                case "ken": return "KE";
                case "kir": return "KI";
                case "prk": return "KP";
                case "kor": return "KR";
                case "kwt": return "KW";
                case "kgz": return "KG";
                case "lao": return "LA";
                case "lva": return "LV";
                case "lbn": return "LB";
                case "lso": return "LS";
                case "lbr": return "LR";
                case "lby": return "LY";
                case "lie": return "LI";
                case "ltu": return "LT";
                case "lux": return "LU";
                case "mac": return "MO";
                case "mdg": return "MG";
                case "mwi": return "MW";
                case "mys": return "MY";
                case "mdv": return "MV";
                case "mli": return "ML";
                case "mlt": return "MT";
                case "mhl": return "MH";
                case "mtq": return "MQ";
                case "mrt": return "MR";
                case "mus": return "MU";
                case "myt": return "YT";
                case "mex": return "MX";
                case "fsm": return "FM";
                case "mda": return "MD";
                case "mco": return "MC";
                case "mng": return "MN";
                case "mne": return "ME";
                case "msr": return "MS";
                case "mar": return "MA";
                case "moz": return "MZ";
                case "mmr": return "MM";
                case "nam": return "NA";
                case "nru": return "NR";
                case "npl": return "NP";
                case "nld": return "NL";
                case "ncl": return "NC";
                case "nzl": return "NZ";
                case "nic": return "NI";
                case "ner": return "NE";
                case "nga": return "NG";
                case "niu": return "NU";
                case "nfk": return "NF";
                case "mkd": return "MK";
                case "mnp": return "MP";
                case "nor": return "NO";
                case "omn": return "OM";
                case "pak": return "PK";
                case "plw": return "PW";
                case "pse": return "PS";
                case "pan": return "PA";
                case "png": return "PG";
                case "pry": return "PY";
                case "per": return "PE";
                case "phl": return "PH";
                case "pcn": return "PN";
                case "pol": return "PL";
                case "prt": return "PT";
                case "pri": return "PR";
                case "qat": return "QA";
                case "rou": return "RO";
                case "rus": return "RU";
                case "rwa": return "RW";
                case "reu": return "RE";
                case "blm": return "BL";
                case "shn": return "SH";
                case "kna": return "KN";
                case "lca": return "LC";
                case "maf": return "MF";
                case "spm": return "PM";
                case "vct": return "VC";
                case "wsm": return "WS";
                case "smr": return "SM";
                case "stp": return "ST";
                case "sau": return "SA";
                case "sen": return "SN";
                case "srb": return "RS";
                case "syc": return "SC";
                case "sle": return "SL";
                case "sgp": return "SG";
                case "sxm": return "SX";
                case "svk": return "SK";
                case "svn": return "SI";
                case "slb": return "SB";
                case "som": return "SO";
                case "zaf": return "ZA";
                case "sgs": return "GS";
                case "ssd": return "SS";
                case "esp": return "ES";
                case "lka": return "LK";
                case "sdn": return "SD";
                case "sur": return "SR";
                case "sjm": return "SJ";
                case "swe": return "SE";
                case "che": return "CH";
                case "syr": return "SY";
                case "twn": return "TW";
                case "tjk": return "TJ";
                case "tza": return "TZ";
                case "tha": return "TH";
                case "tls": return "TL";
                case "tgo": return "TG";
                case "tkl": return "TK";
                case "ton": return "TO";
                case "tto": return "TT";
                case "tun": return "TN";
                case "tur": return "TR";
                case "tkm": return "TM";
                case "tca": return "TC";
                case "tuv": return "TV";
                case "uga": return "UG";
                case "ukr": return "UA";
                case "are": return "AE";
                case "gbr": return "GB";
                case "umi": return "UM";
                case "usa": return "US";
                case "ury": return "UY";
                case "uzb": return "UZ";
                case "vut": return "VU";
                case "ven": return "VE";
                case "vnm": return "VN";
                case "vgb": return "VG";
                case "vir": return "VI";
                case "wlf": return "WF";
                case "esh": return "EH";
                case "yem": return "YE";
                case "zmb": return "ZM";
                case "zwe": return "ZW";
                case "ala": return "AX";
            }

            return null;
        }

        public static string Iso2ToIso3Number(string iso2)
        {
            if (iso2.IsNullOrEmpty())
                return null;

            switch (iso2.ToLower())
            {
                case "af": return "004";
                case "al": return "008";
                case "dz": return "012";
                case "as": return "016";
                case "ad": return "020";
                case "ao": return "024";
                case "ai": return "660";
                case "aq": return "010";
                case "ag": return "028";
                case "ar": return "032";
                case "am": return "051";
                case "aw": return "533";
                case "au": return "036";
                case "at": return "040";
                case "az": return "031";
                case "bs": return "044";
                case "bh": return "048";
                case "bd": return "050";
                case "bb": return "052";
                case "by": return "112";
                case "be": return "056";
                case "bz": return "084";
                case "bj": return "204";
                case "bm": return "060";
                case "bt": return "064";
                case "bo": return "068";
                case "bq": return "535";
                case "ba": return "070";
                case "bw": return "072";
                case "bv": return "074";
                case "br": return "076";
                case "io": return "086";
                case "bn": return "096";
                case "bg": return "100";
                case "bf": return "854";
                case "bi": return "108";
                case "cv": return "132";
                case "kh": return "116";
                case "cm": return "120";
                case "ca": return "124";
                case "ky": return "136";
                case "cf": return "140";
                case "td": return "148";
                case "cl": return "152";
                case "cn": return "156";
                case "cx": return "162";
                case "cc": return "166";
                case "co": return "170";
                case "km": return "174";
                case "cd": return "180";
                case "cg": return "178";
                case "ck": return "184";
                case "cr": return "188";
                case "hr": return "191";
                case "cu": return "192";
                case "cw": return "531";
                case "cy": return "196";
                case "cz": return "203";
                case "ci": return "384";
                case "dk": return "208";
                case "dj": return "262";
                case "dm": return "212";
                case "do": return "214";
                case "ec": return "218";
                case "eg": return "818";
                case "sv": return "222";
                case "gq": return "226";
                case "er": return "232";
                case "ee": return "233";
                case "sz": return "748";
                case "et": return "231";
                case "fk": return "238";
                case "fo": return "234";
                case "fj": return "242";
                case "fi": return "246";
                case "fr": return "250";
                case "gf": return "254";
                case "pf": return "258";
                case "tf": return "260";
                case "ga": return "266";
                case "gm": return "270";
                case "ge": return "268";
                case "de": return "276";
                case "gh": return "288";
                case "gi": return "292";
                case "gr": return "300";
                case "gl": return "304";
                case "gd": return "308";
                case "gp": return "312";
                case "gu": return "316";
                case "gt": return "320";
                case "gg": return "831";
                case "gn": return "324";
                case "gw": return "624";
                case "gy": return "328";
                case "ht": return "332";
                case "hm": return "334";
                case "va": return "336";
                case "hn": return "340";
                case "hk": return "344";
                case "hu": return "348";
                case "is": return "352";
                case "in": return "356";
                case "id": return "360";
                case "ir": return "364";
                case "iq": return "368";
                case "ie": return "372";
                case "im": return "833";
                case "il": return "376";
                case "it": return "380";
                case "jm": return "388";
                case "jp": return "392";
                case "je": return "832";
                case "jo": return "400";
                case "kz": return "398";
                case "ke": return "404";
                case "ki": return "296";
                case "kp": return "408";
                case "kr": return "410";
                case "kw": return "414";
                case "kg": return "417";
                case "la": return "418";
                case "lv": return "428";
                case "lb": return "422";
                case "ls": return "426";
                case "lr": return "430";
                case "ly": return "434";
                case "li": return "438";
                case "lt": return "440";
                case "lu": return "442";
                case "mo": return "446";
                case "mg": return "450";
                case "mw": return "454";
                case "my": return "458";
                case "mv": return "462";
                case "ml": return "466";
                case "mt": return "470";
                case "mh": return "584";
                case "mq": return "474";
                case "mr": return "478";
                case "mu": return "480";
                case "yt": return "175";
                case "mx": return "484";
                case "fm": return "583";
                case "md": return "498";
                case "mc": return "492";
                case "mn": return "496";
                case "me": return "499";
                case "ms": return "500";
                case "ma": return "504";
                case "mz": return "508";
                case "mm": return "104";
                case "na": return "516";
                case "nr": return "520";
                case "np": return "524";
                case "nl": return "528";
                case "nc": return "540";
                case "nz": return "554";
                case "ni": return "558";
                case "ne": return "562";
                case "ng": return "566";
                case "nu": return "570";
                case "nf": return "574";
                case "mk": return "807";
                case "mp": return "580";
                case "no": return "578";
                case "om": return "512";
                case "pk": return "586";
                case "pw": return "585";
                case "ps": return "275";
                case "pa": return "591";
                case "pg": return "598";
                case "py": return "600";
                case "pe": return "604";
                case "ph": return "608";
                case "pn": return "612";
                case "pl": return "616";
                case "pt": return "620";
                case "pr": return "630";
                case "qa": return "634";
                case "ro": return "642";
                case "ru": return "643";
                case "rw": return "646";
                case "re": return "638";
                case "bl": return "652";
                case "sh": return "654";
                case "kn": return "659";
                case "lc": return "662";
                case "mf": return "663";
                case "pm": return "666";
                case "vc": return "670";
                case "ws": return "882";
                case "sm": return "674";
                case "st": return "678";
                case "sa": return "682";
                case "sn": return "686";
                case "rs": return "688";
                case "sc": return "690";
                case "sl": return "694";
                case "sg": return "702";
                case "sx": return "534";
                case "sk": return "703";
                case "si": return "705";
                case "sb": return "090";
                case "so": return "706";
                case "za": return "710";
                case "gs": return "239";
                case "ss": return "728";
                case "es": return "724";
                case "lk": return "144";
                case "sd": return "729";
                case "sr": return "740";
                case "sj": return "744";
                case "se": return "752";
                case "ch": return "756";
                case "sy": return "760";
                case "tw": return "158";
                case "tj": return "762";
                case "tz": return "834";
                case "th": return "764";
                case "tl": return "626";
                case "tg": return "768";
                case "tk": return "772";
                case "to": return "776";
                case "tt": return "780";
                case "tn": return "788";
                case "tr": return "792";
                case "tm": return "795";
                case "tc": return "796";
                case "tv": return "798";
                case "ug": return "800";
                case "ua": return "804";
                case "ae": return "784";
                case "gb": return "826";
                case "um": return "581";
                case "us": return "840";
                case "uy": return "858";
                case "uz": return "860";
                case "vu": return "548";
                case "ve": return "862";
                case "vn": return "704";
                case "vg": return "092";
                case "vi": return "850";
                case "wf": return "876";
                case "eh": return "732";
                case "ye": return "887";
                case "zm": return "894";
                case "zw": return "716";
                case "ax": return "248";
            }

            return null;
        }

        public static string Iso3ToIso3Number(string iso3)
        {
            if (iso3.IsNullOrEmpty())
                return null;

            switch (iso3.ToLower())
            {
                case "afg": return "004";
                case "alb": return "008";
                case "dza": return "012";
                case "asm": return "016";
                case "and": return "020";
                case "ago": return "024";
                case "aia": return "660";
                case "ata": return "010";
                case "atg": return "028";
                case "arg": return "032";
                case "arm": return "051";
                case "abw": return "533";
                case "aus": return "036";
                case "aut": return "040";
                case "aze": return "031";
                case "bhs": return "044";
                case "bhr": return "048";
                case "bgd": return "050";
                case "brb": return "052";
                case "blr": return "112";
                case "bel": return "056";
                case "blz": return "084";
                case "ben": return "204";
                case "bmu": return "060";
                case "btn": return "064";
                case "bol": return "068";
                case "bes": return "535";
                case "bih": return "070";
                case "bwa": return "072";
                case "bvt": return "074";
                case "bra": return "076";
                case "iot": return "086";
                case "brn": return "096";
                case "bgr": return "100";
                case "bfa": return "854";
                case "bdi": return "108";
                case "cpv": return "132";
                case "khm": return "116";
                case "cmr": return "120";
                case "can": return "124";
                case "cym": return "136";
                case "caf": return "140";
                case "tcd": return "148";
                case "chl": return "152";
                case "chn": return "156";
                case "cxr": return "162";
                case "cck": return "166";
                case "col": return "170";
                case "com": return "174";
                case "cod": return "180";
                case "cog": return "178";
                case "cok": return "184";
                case "cri": return "188";
                case "hrv": return "191";
                case "cub": return "192";
                case "cuw": return "531";
                case "cyp": return "196";
                case "cze": return "203";
                case "civ": return "384";
                case "dnk": return "208";
                case "dji": return "262";
                case "dma": return "212";
                case "dom": return "214";
                case "ecu": return "218";
                case "egy": return "818";
                case "slv": return "222";
                case "gnq": return "226";
                case "eri": return "232";
                case "est": return "233";
                case "swz": return "748";
                case "eth": return "231";
                case "flk": return "238";
                case "fro": return "234";
                case "fji": return "242";
                case "fin": return "246";
                case "fra": return "250";
                case "guf": return "254";
                case "pyf": return "258";
                case "atf": return "260";
                case "gab": return "266";
                case "gmb": return "270";
                case "geo": return "268";
                case "deu": return "276";
                case "gha": return "288";
                case "gib": return "292";
                case "grc": return "300";
                case "grl": return "304";
                case "grd": return "308";
                case "glp": return "312";
                case "gum": return "316";
                case "gtm": return "320";
                case "ggy": return "831";
                case "gin": return "324";
                case "gnb": return "624";
                case "guy": return "328";
                case "hti": return "332";
                case "hmd": return "334";
                case "vat": return "336";
                case "hnd": return "340";
                case "hkg": return "344";
                case "hun": return "348";
                case "isl": return "352";
                case "ind": return "356";
                case "idn": return "360";
                case "irn": return "364";
                case "irq": return "368";
                case "irl": return "372";
                case "imn": return "833";
                case "isr": return "376";
                case "ita": return "380";
                case "jam": return "388";
                case "jpn": return "392";
                case "jey": return "832";
                case "jor": return "400";
                case "kaz": return "398";
                case "ken": return "404";
                case "kir": return "296";
                case "prk": return "408";
                case "kor": return "410";
                case "kwt": return "414";
                case "kgz": return "417";
                case "lao": return "418";
                case "lva": return "428";
                case "lbn": return "422";
                case "lso": return "426";
                case "lbr": return "430";
                case "lby": return "434";
                case "lie": return "438";
                case "ltu": return "440";
                case "lux": return "442";
                case "mac": return "446";
                case "mdg": return "450";
                case "mwi": return "454";
                case "mys": return "458";
                case "mdv": return "462";
                case "mli": return "466";
                case "mlt": return "470";
                case "mhl": return "584";
                case "mtq": return "474";
                case "mrt": return "478";
                case "mus": return "480";
                case "myt": return "175";
                case "mex": return "484";
                case "fsm": return "583";
                case "mda": return "498";
                case "mco": return "492";
                case "mng": return "496";
                case "mne": return "499";
                case "msr": return "500";
                case "mar": return "504";
                case "moz": return "508";
                case "mmr": return "104";
                case "nam": return "516";
                case "nru": return "520";
                case "npl": return "524";
                case "nld": return "528";
                case "ncl": return "540";
                case "nzl": return "554";
                case "nic": return "558";
                case "ner": return "562";
                case "nga": return "566";
                case "niu": return "570";
                case "nfk": return "574";
                case "mkd": return "807";
                case "mnp": return "580";
                case "nor": return "578";
                case "omn": return "512";
                case "pak": return "586";
                case "plw": return "585";
                case "pse": return "275";
                case "pan": return "591";
                case "png": return "598";
                case "pry": return "600";
                case "per": return "604";
                case "phl": return "608";
                case "pcn": return "612";
                case "pol": return "616";
                case "prt": return "620";
                case "pri": return "630";
                case "qat": return "634";
                case "rou": return "642";
                case "rus": return "643";
                case "rwa": return "646";
                case "reu": return "638";
                case "blm": return "652";
                case "shn": return "654";
                case "kna": return "659";
                case "lca": return "662";
                case "maf": return "663";
                case "spm": return "666";
                case "vct": return "670";
                case "wsm": return "882";
                case "smr": return "674";
                case "stp": return "678";
                case "sau": return "682";
                case "sen": return "686";
                case "srb": return "688";
                case "syc": return "690";
                case "sle": return "694";
                case "sgp": return "702";
                case "sxm": return "534";
                case "svk": return "703";
                case "svn": return "705";
                case "slb": return "090";
                case "som": return "706";
                case "zaf": return "710";
                case "sgs": return "239";
                case "ssd": return "728";
                case "esp": return "724";
                case "lka": return "144";
                case "sdn": return "729";
                case "sur": return "740";
                case "sjm": return "744";
                case "swe": return "752";
                case "che": return "756";
                case "syr": return "760";
                case "twn": return "158";
                case "tjk": return "762";
                case "tza": return "834";
                case "tha": return "764";
                case "tls": return "626";
                case "tgo": return "768";
                case "tkl": return "772";
                case "ton": return "776";
                case "tto": return "780";
                case "tun": return "788";
                case "tur": return "792";
                case "tkm": return "795";
                case "tca": return "796";
                case "tuv": return "798";
                case "uga": return "800";
                case "ukr": return "804";
                case "are": return "784";
                case "gbr": return "826";
                case "umi": return "581";
                case "usa": return "840";
                case "ury": return "858";
                case "uzb": return "860";
                case "vut": return "548";
                case "ven": return "862";
                case "vnm": return "704";
                case "vgb": return "092";
                case "vir": return "850";
                case "wlf": return "876";
                case "esh": return "732";
                case "yem": return "887";
                case "zmb": return "894";
                case "zwe": return "716";
                case "ala": return "248";
            }

            return null;
        }

        #endregion
    }
}