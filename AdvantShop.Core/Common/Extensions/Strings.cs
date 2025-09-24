//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using AdvantShop.Helpers;
using DamienG.Security.Cryptography;

namespace AdvantShop.Core.Common.Extensions
{
    public enum StringHashType
    {
        MD5,
        SHA1,
    }
    /// <summary>
    /// Summary description for Strings
    /// </summary>
    public static class Strings
    {
        private const string RandomSymbols = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
        private const string RandomUrlSymbols = "qwertyuiopasdfghjklzxcvbnm0123456789_-";

        public static string GetRandomString(Random rnd, Int32 length, Int32 intSpaces)
        {
            return GetRandomString(rnd, length, intSpaces, RandomSymbols);
        }

        public static string GetRandomString(Random rnd, Int32 length, Int32 intSpaces, string strSymbols)
        {
            var sb = new StringBuilder();
            int intTempSpace = 0;
            for (int i = 0; i <= length - 1; i++)
            {
                if (intSpaces != -1 && intTempSpace >= intSpaces)
                {
                    sb.Append(" ");
                    intTempSpace = 0;
                }
                else
                {
                    int intTemp = rnd.Next(strSymbols.Length - 1);
                    sb.Append(strSymbols[intTemp]);
                    intTempSpace++;
                }
            }
            return sb.ToString();
        }

        public static string GetRandomString(Random rnd, Int32 length)
        {
            return GetRandomString(rnd, length, -1);
        }

        public static string GetRandomString(Int32 length)
        {
            var rnd = new Random();
            return GetRandomString(rnd, length, -1);
        }

        public static string GetRandomUrlString(Int32 length)
        {
            var rnd = new Random();
            return GetRandomString(rnd, length, -1, RandomUrlSymbols);
        }

        public static string Reduce(this String val, int length)
        {
            return val.Length > length ? val.Substring(0, length) : val;
        }

        public static bool IsLongerThan(this String val, int length)
        {
            return val != null && val.Length > length;
        }

        public static string AppendMany(this StringBuilder sb, params string[] values)
        {
            if (values != null)
                foreach (var value in values.Where(val => val != null))
                {
                    sb.Append(value);
                }
            return sb.ToString();
        }

        public static string Sha256(this string secret)
            => secret.Sha256(false);
        
        public static string Sha256(this string secret, bool upperCase)
            => secret.Sha256(upperCase, new ASCIIEncoding());
        
        public static string Sha256(this string secret, bool upperCase, Encoding encoding)
        {
            var keyByte = encoding.GetBytes(secret);
            using (var hashstring = new SHA256Managed())
            {
                byte[] hash = hashstring.ComputeHash(keyByte);
                return hash.Aggregate(new StringBuilder(), (curr, value) => curr.Append(value.ToString(upperCase ? "X2" : "x2"))).ToString();
            }
        }
        
        public static string Sha256AndToBase64UrlEncode(this string secret, Encoding encoding = null)
        {
            byte[] verifierBytes = (encoding ?? new ASCIIEncoding()).GetBytes(secret);
            byte[] hashBytes;
            
            using (var sha256 = SHA256.Create())
                hashBytes = sha256.ComputeHash(verifierBytes);
            
            var base64 = Convert.ToBase64String(hashBytes);
            
            return base64.Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }

        public static string HmacSha256(this string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new ASCIIEncoding();
            var keyByte = encoding.GetBytes(secret);
            var messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                var hashmessage = hmacsha256.ComputeHash(messageBytes);
                return hashmessage.Aggregate(new StringBuilder(), (curr, value) => curr.Append(value.ToString("x2"))).ToString();
            }
        }

        public static string Md5(this string val)
        {
            // This is one implementation of the abstract class MD5.
            return val.Md5(true);
        }

        public static string Md5(this string val, bool upperCase)
        {
            return val.Md5(upperCase, Encoding.Default);
        }

        public static string Md5(this string val, bool upperCase, Encoding encoding)
        {
            return
                new MD5CryptoServiceProvider()
                    .ComputeHash(encoding.GetBytes(val))
                    .Aggregate(new StringBuilder(), (curr, value) => curr.Append(value.ToString(upperCase ? "X2" : "x2"))).ToString();
        }

        public static string Crc32(this string val, bool toDecimal)
        {
            Crc32 crc32 = new Crc32();
            string hash = String.Empty;
            foreach (byte b in crc32.ComputeHash(StringHelper.StringToUTF8ByteArray(val)))
                hash += b.ToString("x2").ToLower();

            return toDecimal ? int.Parse(hash, NumberStyles.HexNumber).ToString() : hash;
        }


        public static string Sha1(this string val)
        {
            return val.Sha1(false);
        }
        
        public static string Sha1(this string val, bool upperCase)
        {
            return val.Sha1(upperCase, Encoding.Default);
        }

        public static string Sha1(this string val, bool upperCase, Encoding encoding)
        {
            return
                new SHA1CryptoServiceProvider()
                    .ComputeHash(encoding.GetBytes(val))
                    .Aggregate(new StringBuilder(), (curr, value) => curr.Append(value.ToString(upperCase ? "X2" : "x2"))).ToString();
        }

        public static string GetCryptoHash(this string val, StringHashType type)
        {
            switch (type)
            {
                case StringHashType.MD5:
                    return val.Md5();
                case StringHashType.SHA1:
                    return val.Sha1();
                default:
                    return val;
            }
        }

        /// <summary>
        /// string.IsNullOrEmpty
        /// </summary>
        public static bool IsNullOrEmpty(this string val)
        {
            return String.IsNullOrEmpty(val);
        }

        /// <summary>
        /// !string.IsNullOrWhiteSpace
        /// </summary>
        public static bool IsNotEmpty(this string val)
        {
            //return val != null && !String.IsNullOrEmpty(val.Trim());
            return !string.IsNullOrWhiteSpace(val);
        }

        public static bool EndsWith(this string val, IEnumerable<string> endStrings)
        {
            return endStrings.Any(val.EndsWith);
        }

        public static int TryParseInt(this string val)
        {
            var ret = val.TryParseInt(false);
            return ret.HasValue ? ret.Value : 0;
        }

        public static int? TryParseInt(this string val, bool isNullable)
        {
            int intval;
            return Int32.TryParse(val, out intval) || !isNullable ? intval : (int?)null;
        }

        public static int TryParseInt(this string val, int defaultValue)
        {
            int intval;
            return Int32.TryParse(val, out intval) ? intval : defaultValue;
        }

        public static float TryParseFloat(this string val)
        {
            val = val ?? "";
            var ret = val.TryParseFloat(false);
            return ret.HasValue ? ret.Value : 0;
        }

        public static float? TryParseFloat(this string val, bool isNullable)
        {
            val = val ?? "";
            float intval;
            return float.TryParse(val.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out intval) || !isNullable ? intval : (float?)null;
        }

        public static float TryParseFloat(this string val, float defaultValue)
        {
            val = val ?? "";
            float intval;
            return float.TryParse(val.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out intval) ? intval : defaultValue;
        }

        public static double TryParseDouble(this string val)
        {
            val = val ?? "";
            var ret = val.TryParseDouble(false);
            return ret.HasValue ? ret.Value : 0;
        }

        public static double? TryParseDouble(this string val, bool isNullable)
        {
            val = val ?? "";
            double intval;
            return double.TryParse(val.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out intval) || !isNullable ? intval : (double?)null;
        }

        public static double TryParseDouble(this string val, double defaultValue)
        {
            val = val ?? "";
            double intval;
            return double.TryParse(val.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out intval) ? intval : defaultValue;
        }

        public static long TryParseLong(this string val)
        {
            var ret = val.TryParseLong(false);
            return ret.HasValue ? ret.Value : 0;
        }

        public static long? TryParseLong(this string val, bool isNullable)
        {
            long intval;
            return long.TryParse(val, out intval) || !isNullable ? intval : (long?)null;
        }

        public static long TryParseLong(this string val, long defaultValue)
        {
            long intval;
            return long.TryParse(val, out intval) ? intval : defaultValue;
        }

        public static Guid TryParseGuid(this string val)
        {
            var ret = val.TryParseGuid(false);
            return ret.HasValue ? ret.Value : Guid.Empty;
        }

        public static Guid? TryParseGuid(this string val, bool isNullable)
        {
            Guid intval;
            return Guid.TryParse(val, out intval) || !isNullable ? intval : (Guid?)null;
        }

        public static Guid TryParseGuid(this string val, Guid defaultValue)
        {
            Guid intval;
            return Guid.TryParse(val, out intval) ? intval : defaultValue;
        }

        public static DateTime TryParseDateTime(this string val)
        {
            var ret = val.TryParseDateTime(false);
            return ret.HasValue ? ret.Value : DateTime.MinValue;
        }
        public static DateTime? TryParseDateTime(this string val, bool isNullable)
        {
            DateTime intval;
            return DateTime.TryParse(val, out intval) || !isNullable ? intval : (DateTime?)null;
        }

        public static DateTime TryParseDateTime(this string val, DateTime defaultValue)
        {
            DateTime intval;
            return DateTime.TryParse(val, out intval) ? intval : defaultValue;
        }

        public static DateTime TryParseDateTime(this string val, DateTime defaultValue, CultureInfo value, DateTimeStyles style)
        {
            DateTime intval;
            return DateTime.TryParse(val, value, style, out intval) ? intval : defaultValue;
        }

        public static TimeSpan TryParseTimeSpan(this string val)
        {
            var ret = val.TryParseTimeSpan(false);
            return ret.HasValue ? ret.Value : TimeSpan.MinValue;
        }
        public static TimeSpan? TryParseTimeSpan(this string val, bool isNullable)
        {
            TimeSpan intval;
            return TimeSpan.TryParse(val, out intval) || !isNullable ? intval : (TimeSpan?)null;
        }

        public static TimeSpan TryParseTimeSpan(this string val, TimeSpan defaultValue)
        {
            TimeSpan intval;
            return TimeSpan.TryParse(val, out intval) ? intval : defaultValue;
        }

        public static string RemoveSymbols(this string val, string repStr)
        {
            return val.Trim().Replace("/", repStr)
                .Replace("\\", repStr)
                .Replace("\"", repStr)
                .Replace("-", repStr)
                .Replace("'", repStr)
                .Replace("&", repStr)
                .Replace(";", repStr)
                .Replace("?", repStr);
        }

        public static string RemoveSymvolsExt(this string val, string repStr)
        {
            var strSymbols = "~`!&*()-_=+\\/|[]{}';:\",.<>?".ToCharArray();
            var result = "";

            for (int i = 0; i < val.Length; i++)
            {
                if (strSymbols.Contains(val[i]))
                {
                    result += repStr;
                }
                else
                {
                    result += val[i];
                }
            }
            return result;
        }

        public static string RemoveSymbols(this string val)
        {
            return val.Trim().Replace("/", "")
                .Replace("\\", "")
                .Replace("\"", "")
                .Replace("-", "")
                .Replace("'", "")
                .Replace("&", "")
                .Replace(";", "")
                .Replace("?", "");
        }

        public static string HtmlEncode(this string val)
        {
            return HttpUtility.HtmlEncode(val);
        }

        public static string HtmlDecode(this string val)
        {
            return HttpUtility.HtmlDecode(val);
        }

        public static string XmlEncode(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            //var result = new StringBuilder(value);
            //return result.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;").ToString();
            return HttpUtility.HtmlEncode(value).Replace("&#39;", "&apos;");
        }

        public static string RemoveInvalidXmlChars(this string text)
        {
            if (text.IsNullOrEmpty())
                return text;

            var validXmlChars = text.Where(XmlConvert.IsXmlChar).ToArray();
            return new string(validXmlChars);
        }

        public static string RemoveEscapeXmlChars(this string text)
        {
            if (text.IsNullOrEmpty())
                return text;

            var validXmlChars = text.Where(c => c.ToString() == SecurityElement.Escape(c.ToString())).ToArray();
            return new string(validXmlChars);
        }

        public static string XmlDecode(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            //var result = new StringBuilder(value);
            //return result.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&apos;", "'").Replace("&amp;", "&").ToString();
            return HttpUtility.HtmlDecode(value.Replace("&apos;", "'"));
        }

        public static string RemoveHtmlEncodedCharacters(this string htmlEncoded) =>
            ReplaceHtmlEncodedCharacters(htmlEncoded, string.Empty);
        
        public static string ReplaceHtmlEncodedCharacters(this string htmlEncoded, string replacement)
        {
            if (string.IsNullOrEmpty(htmlEncoded))
                return string.Empty;
            return HttpUtility.HtmlDecode(Regex.Replace(htmlEncoded, "&[#a-z0-9]+;", replacement, RegexOptions.IgnoreCase));
        }

        public static bool IsInt(this string val)
        {
            int intval;
            return Int32.TryParse(val, out intval);
        }

        public static decimal TryParseDecimal(this string val)
        {
            return val.TryParseDecimal(0);
        }

        public static decimal? TryParseDecimal(this string val, bool isNullable)
        {
            return 
                Decimal.TryParse(val?.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out var decval) || !isNullable 
                    ? decval 
                    : (decimal?)null;
        }

        public static decimal TryParseDecimal(this string val, decimal defaultValue)
        {
            return
                Decimal.TryParse(val?.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out var decval)
                    ? decval
                    : defaultValue;
        }

        public static bool IsDecimal(this string val)
        {
            return Decimal.TryParse(val?.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out _);
        }

        public static bool TryParseBool(this string val)
        {
            var ret = val.TryParseBool(false);
            return ret.HasValue && ret.Value;
        }

        public static bool? TryParseBool(this string val, bool isNullable)
        {
            bool boolval;
            return Boolean.TryParse(val, out boolval) || !isNullable ? boolval : (bool?)null;
        }

        public static string Default(this string val, string defaultValue)
        {
            return val.IsNotEmpty() ? val : defaultValue;
        }

        public static string DefaultOrEmpty(this string val)
        {
            return val.IsNotEmpty() ? val.Trim() : "";
        }

        public static string EncodeOrEmpty(this string val)
        {
            return HttpUtility.HtmlEncode(val.DefaultOrEmpty());
        }
        
        public static string EncodeOrEmpty(this string val, bool allowQuotes)
        {
            var result = val.EncodeOrEmpty();
            return allowQuotes 
                ? result.Replace("&quot;", "\"").Replace("&#39;", "'") 
                : result;
        }

        public static string ToString(object val)
        {
            return val.ToString();
        }

        public static string Numerals(float value, string zeroText, string oneText, string twoText, string fiveText)
        {
            if (value <= 0) return zeroText;
            value = value % 100;
            var val = value % 10;
            if (value > 10 && value < 20) return fiveText;
            if (val > 1 && val < 5) return twoText;
            return val == 1 ? oneText : fiveText;
        }

        public static IHtmlString Numerals(float value, IHtmlString zeroText, IHtmlString oneText, IHtmlString twoText, IHtmlString fiveText)
        {
            if (value == 0) return zeroText;
            value = value % 100;
            var val = value % 10;
            if (value > 10 && value < 20) return fiveText;
            if (val > 1 && val < 5) return twoText;
            return val == 1 ? oneText : fiveText;
        }

        public static string FileNamePlusDate(this string fileName, DateTime date, string newFileName = "")
        {
            var temp = fileName.Split('.');
            if (temp.Length == 2)
            {
                return (string.IsNullOrEmpty(newFileName) ? temp[0] : newFileName) + "_" + date.ToString("yyyyMMddHHmmss") + "." + temp[1];
            }
            else if (temp.Length == 1)
            {
                return fileName + "_" + date.ToString("yyyyMMddHHmmss");
            }

            return fileName;
        }

        public static string FileNamePlusDate(this string fileName, string newFileName = "")
        {
            return fileName.FileNamePlusDate(DateTime.Now, newFileName);
        }

        public static T TryParseEnum<T>(this string value) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value)) return default(T);
            T res;
            return Enum.TryParse(value, true, out res) ? res : default(T);
        }

        public static T TryParseEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;
            T res;
            return Enum.TryParse(value, true, out res) ? res : defaultValue;
        }

        public static string[] Split(this string val, string separator)
        {
            return val.Split(new[] { separator }, StringSplitOptions.None);
        }

        public static float RemoveChars(this string val)
        {
            var result = "";

            for (int i = 0; i < val.Length; i++)
            {
                if (Char.IsDigit(val[i]) || (val[i] == ',' || val[i] == '.') || (i == 0 && val[i] == '-'))
                {
                    result += val[i];
                }
                else
                {
                    result += " ";
                }
            }

            return result.Trim().TryParseFloat();
        }

        public static bool Contains(this string val, string substring, StringComparison comp)
        {
            if (substring == null)
                throw new ArgumentNullException("substring", "substring cannot be null.");
            else if (!Enum.IsDefined(typeof(StringComparison), comp))
                throw new ArgumentException("comp is not a member of StringComparison", "comp");

            return val.IndexOf(substring, comp) >= 0;
        }

        public static string SupperTrim(this string inputString)
        {
            return inputString.Trim(new[] { ' ', '\t', '\r', '\n' });
        }

        public static bool IsLocalIP(this string ip)
        {
            return ip == "::1" || ip == "127.0.0.1";
        }

        public static string AnonimizePhone(this string phone)
        {
            if (phone.IsNullOrEmpty())
                return null;
            var count = phone.Count(c => Char.IsDigit(c));
            if (count == 0)
                return phone;

            int from = count > 9
                ? count - 7
                : count > 4 ? 2 : 0;
            int to = count > 5 ? count - 3 : count;

            var sb = new StringBuilder(phone);
            int ind = 0;
            for (int i = 0; i < sb.Length; i++)
            {
                if (!Char.IsDigit(sb[i]))
                    continue;
                if (ind >= from && ind <= to)
                    sb[i] = '*';
                ind++;
            }
            return sb.ToString();
        }

        public static string AnonimizeEmail(this string email)
        {
            if (email.IsNullOrEmpty())
                return null;
            if (!email.Contains("@"))
                return AnonimizeEmailPart(email);
            var parts = email.Split("@");
            var domain = parts.LastOrDefault();

            return string.Format("{0}@{1}{2}",
                AnonimizeEmailPart(parts.FirstOrDefault()),
                AnonimizeEmailPart(domain.Contains('.') ? domain.Substring(0, domain.LastIndexOf('.')) : domain),
                domain.Contains('.') ? domain.Substring(domain.LastIndexOf('.'), domain.Length - domain.LastIndexOf('.')) : string.Empty);
        }

        private static string AnonimizeEmailPart(string str)
        {
            if (str.IsNullOrEmpty())
                return string.Empty;

            int from = str.Length > 3 ? 2 : 0;
            int to = str.Length > 5 ? str.Length - 1 : str.Length;

            var sb = new StringBuilder(str);
            for (int i = from; i < to; i++)
                sb[i] = '*';
            return sb.ToString();
        }

        public static string Replace(this string str,
            string oldValue, string @newValue,
            StringComparison comparisonType)
        {

            // Check inputs.
            if (str == null)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentNullException("str");
            }
            if (str.Length == 0)
            {
                // Same as original .NET C# string.Replace behavior.
                return str;
            }
            if (oldValue == null)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentNullException("oldValue");
            }
            if (oldValue.Length == 0)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentException("String cannot be of zero length.");
            }


            //if (oldValue.Equals(newValue, comparisonType))
            //{
            //This condition has no sense
            //It will prevent method from replacesing: "Example", "ExAmPlE", "EXAMPLE" to "example"
            //return str;
            //}



            // Prepare string builder for storing the processed string.
            // Note: StringBuilder has a better performance than String by 30-40%.
            StringBuilder resultStringBuilder = new StringBuilder(str.Length);



            // Analyze the replacement: replace or remove.
            bool isReplacementNullOrEmpty = string.IsNullOrEmpty(@newValue);



            // Replace all values.
            const int valueNotFound = -1;
            int foundAt;
            int startSearchFromIndex = 0;
            while ((foundAt = str.IndexOf(oldValue, startSearchFromIndex, comparisonType)) != valueNotFound)
            {

                // Append all characters until the found replacement.
                int @charsUntilReplacment = foundAt - startSearchFromIndex;
                bool isNothingToAppend = @charsUntilReplacment == 0;
                if (!isNothingToAppend)
                {
                    resultStringBuilder.Append(str, startSearchFromIndex, @charsUntilReplacment);
                }



                // Process the replacement.
                if (!isReplacementNullOrEmpty)
                {
                    resultStringBuilder.Append(@newValue);
                }


                // Prepare start index for the next search.
                // This needed to prevent infinite loop, otherwise method always start search 
                // from the start of the string. For example: if an oldValue == "EXAMPLE", newValue == "example"
                // and comparisonType == "any ignore case" will conquer to replacing:
                // "EXAMPLE" to "example" to "example" to "example" … infinite loop.
                startSearchFromIndex = foundAt + oldValue.Length;
                if (startSearchFromIndex == str.Length)
                {
                    // It is end of the input string: no more space for the next search.
                    // The input string ends with a value that has already been replaced. 
                    // Therefore, the string builder with the result is complete and no further action is required.
                    return resultStringBuilder.ToString();
                }
            }


            // Append the last part to the result.
            int @charsUntilStringEnd = str.Length - startSearchFromIndex;
            resultStringBuilder.Append(str, startSearchFromIndex, @charsUntilStringEnd);


            return resultStringBuilder.ToString();

        }

        public static string RemoveTypeFromRegion(this string region)
        {
            return RemoveTypeFromRegion(region, 
                regionTypes: new string[] { "АО" },
                regionTypesIgnoreCase: /* последовательность важна */ new string[]
                {
                    "автономная область",
                    "авт. область",
                    "авт область",
                    "автономная обл.",
                    "автономная обл",
                    "область",
                    "обл.",
                    "обл",
                    "республика",
                    "респ.",
                    "респ",
                    "автономный округ",
                    "авт. округ",
                    "авт округ",
                    "край"
                });
        }

        public static string RemoveTypeFromRegion(this string region, string[] regionTypes, string[] regionTypesIgnoreCase)
        {
            if (!string.IsNullOrEmpty(region))
            {
                if (regionTypes != null && regionTypes.Length > 0)
                    foreach (var regionType in regionTypes)
                        region = region.Replace(regionType, string.Empty);

                if (regionTypesIgnoreCase != null && regionTypesIgnoreCase.Length > 0)
                    foreach (var regionType in regionTypesIgnoreCase)
                        region = region.Replace(regionType, null, StringComparison.OrdinalIgnoreCase);

                while (region.IndexOf("  ") != -1)
                    region = region.Replace("  ", " ");

                region = region.Trim();
            }

            return region;
        }


        /// <summary>
        /// Html Encode (softly allows " and & in string)
        /// </summary>
        public static string HtmlEncodeSoftly(this string s, bool softly = true)
        {
            switch (s)
            {
                case "":
                    return string.Empty;
                case null:
                    return null;
                default:
                    bool flag = false;
                    for (int index = 0; index < s.Length; ++index)
                    {
                        char ch = s[index];
                        switch (ch)
                        {
                            case '"':
                            case '&':
                            case '<':
                            case '>':
                                flag = true;
                                goto label_1;
                            default:
                                if (ch <= '\u009F')
                                    continue;
                                goto case '"';
                        }
                    }
                label_1:
                    if (!flag)
                        return s;

                    var stringBuilder = new StringBuilder();
                    var length = s.Length;
                    for (int index = 0; index < length; ++index)
                    {
                        switch (s[index])
                        {
                            case '"':
                                stringBuilder.Append(softly ? "\"" : "&quot;");
                                break;
                            case '&':
                                stringBuilder.Append(softly ? "&" : "&amp;");
                                break;
                            case '<':
                                stringBuilder.Append("&lt;");
                                break;
                            case '>':
                                stringBuilder.Append("&gt;");
                                break;
                            default:
                                char ch = s[index];
                                if (ch > '\u009F' && ch < 'Ā')
                                {
                                    stringBuilder.Append("&#");
                                    stringBuilder.Append(((int)ch).ToString(CultureInfo.InvariantCulture));
                                    stringBuilder.Append(";");
                                    break;
                                }
                                stringBuilder.Append(ch);
                                break;
                        }
                    }
                    return stringBuilder.ToString();
            }
        }

        public static string TrimEnd(this string input, string suffixToRemove, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            if (suffixToRemove != null && input.EndsWith(suffixToRemove, comparisonType)) 
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }

            return input;
        }

        public static string ToCamelCase(this string word)
        {
            return !string.IsNullOrEmpty(word)
                ? string.Concat(word.Substring(0, 1).ToLower(), word.Substring(1))
                : word;
        }
    }
}