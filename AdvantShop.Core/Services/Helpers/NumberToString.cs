//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Globalization;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Helpers
{
    public class NumberToString
    {
        /// <summary>
        /// Convert sum to words (ex: "Сто двадцать пять рублей ноль копеек")
        /// </summary>
        public static string ConvertToString(decimal number)
        {
            return ConvertToString(number, true);
        }

        /// <summary>
        /// Convert sum to words (ex: "Сто двадцать пять рублей ноль копеек", "Сто двадцать пять рублей 00 копеек")
        /// </summary>
        public static string ConvertToString(decimal number, bool kopPropis)
        {
            if (kopPropis)
                return Num2Text(number);
            else
            {
                var total = Num2Text(Math.Truncate(number), false);
                var floatPart = number != 0
                    ? (int)(Math.Round(number - Math.Truncate(number), 2) * 100) //((double)number).ToString("F2", CultureInfo.InvariantCulture).Split(new []{CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator}, StringSplitOptions.None)[1].TryParseInt()
                    : 0;

                switch (floatPart % 10)
                {
                    case 1:
                        return string.Format("{0} {1:0#} копейка", total, floatPart);
                    case 2:
                    case 3:
                    case 4:
                        return string.Format("{0} {1:0#} копейки", total, floatPart);
                    default:
                        return string.Format("{0} {1:0#} копеек", total, floatPart);
                }
            }
        }

        #region ConvertToString private

        // Алгоритм на основе JS Bill.cshtml
        private static string Num2Text(decimal dNum, bool showKop = true)
        {
            var M = new string[,]
            {
                {"", "", "", "", "тысяч", "миллионов", "миллиардов"},
                {"один", "одиннадцать", "десять", "сто", "тысяча", "миллион", "миллиард"},
                {"два", "двенадцать", "двадцать", "двести", "тысячи", "миллиона", "миллиарда"},
                {"три", "тринадцать", "тридцать", "триста", "тысячи", "миллиона", "миллиарда"},
                {"четыре", "четырнадцать", "сорок", "четыреста", "тысячи", "миллиона", "миллиарда"},
                {"пять", "пятнадцать", "пятьдесят", "пятьсот", "тысяч", "миллионов", "миллиардов"},
                {"шесть", "шестнадцать", "шестьдесят", "шестьсот", "тысяч", "миллионов", "миллиардов"},
                {"семь", "семнадцать", "семьдесят", "семьсот", "тысяч", "миллионов", "миллиардов"},
                {"восемь", "восемнадцать", "восемьдесят", "восемьсот", "тысяч", "миллионов", "миллиардов"},
                {"девять", "девятнадцать", "девяносто", "девятьсот", "тысяч", "миллионов", "миллиардов"}
            };
            var R = new string[] { "рублей", "рубль", "рубля", "рубля", "рубля", "рублей", "рублей", "рублей", "рублей", "рублей" };
            var K = new string[] { "копеек", "копейка", "копейки", "копейки", "копейки", "копеек", "копеек", "копеек", "копеек", "копеек" };

            var rub = ""; var kop = ""; var minus = "";
            var money = dNum.ToString(CultureInfo.InvariantCulture);

            if (JSSubstring(money, 0, 1) == "-") { money = JSSubstring(money, 1); minus = "минус "; }
            else minus = "";
            money = (Math.Round(dNum * 100) / 100).ToString(CultureInfo.InvariantCulture);

            if (money.IndexOf(".") != -1)
            {
                rub = JSSubstring(money, 0, money.IndexOf("."));
                kop = JSSubstring(money, money.IndexOf(".") + 1);
                if (kop.Length == 1) kop += "0";
            }
            else rub = money;

            if (rub.Length > 12)
                return null;

            string ru, ko, res;
            ru = Propis(rub, R, M, K, R);
            ko = showKop ? Propis(kop, K, M, K, R) : "";
            if (ko != "")
                res = ru + " " + ko;
            else
                res = ru;
            if (ru == "Ноль " + R[0] && ko != "")
                res = ko;
            if (showKop && kop.TryParseInt() == 0)
                res += " ноль " + K[0];

            res = minus + res;

            return JSSubstring(res, 0, 1).ToUpper() + JSSubstring(res, 1);
        }

        private static string Propis(string price, string[] D, string[,] M, string[] K, string[] R)
        {
            var litera = "";
            for (var i = 0; i < price.Length; i += 3)
            {
                var sotny = ""; var desatky = ""; var edinicy = "";
                if (n(price, i + 2, 2) > 10 && n(price, i + 2, 2) < 20)
                {
                    edinicy = " " + M[n(price, i + 1, 1), 1] + " " + M[0, i / 3 + 3];
                    if (i == 0) edinicy += D[0];
                }
                else
                {
                    edinicy = M[n(price, i + 1, 1), 0];
                    if (edinicy == "один" && (i == 3 || D == K)) edinicy = "одна"; // Одна коейка, одна тысяча
                    if (edinicy == "два" && (i == 3 || D == K)) edinicy = "две"; // Две коейки, две тысячи
                    if (i != 0 || edinicy == "") edinicy += " " + M[n(price, i + 1, 1), i / 3 + 3];
                    if (edinicy == " ")
                        edinicy = "";
                    else if (edinicy != " " + M[n(price, i + 1, 1), i / 3 + 3])
                        edinicy = " " + edinicy;
                    if (i == 0) edinicy += " " + D[n(price, i + 1, 1)];
                    if ((desatky = M[n(price, i + 2, 1), 2]) != "") desatky = " " + desatky;
                }
                if ((sotny = M[n(price, i + 3, 1), 3]) != "") sotny = " " + sotny;
                if (JSSubstring(price, price.Length - i - 3, 3) == "000" && edinicy == " " + M[0, i / 3 + 3]) edinicy = "";
                litera = sotny + desatky + edinicy + litera;
            }
            if (litera == " " + R[0])
                return "ноль" + litera;
            return JSSubstring(litera, 1);
        }

        private static int n(string price, int start, int len)
        {
            if (start > price.Length) return 0;
            return JSSubstring(price, price.Length - start, len).TryParseInt();
        }

        private static string JSSubstring(string str, int start)
        {
            return JSSubstring(str, start, str.Length - start);
        }

        private static string JSSubstring(string str, int start, int len)
        {
            if (start < 0)
                start = str.Length - start;
            if (start < 0)
                start = 0;
            if (start + len > str.Length)
                len = str.Length - start;
            if (len <= 0)
                return "";
            if (start == str.Length)
                return "";

            return str.Substring(start, len);
        }

        #endregion

        /// <summary>
        /// Convert number to words (ex: "Сто двадцать пять", "Двадцать два", "Двадцать две")
        /// </summary>
        public static string NumberToWords(ulong value, bool isMale = true)
        {
            if (value == 0UL) return "Ноль";
            string[] dek1 = { "", " од", " дв", " три", " четыре", " пять", " шесть", " семь", " восемь", " девять", " десять", " одиннадцать", " двенадцать", " тринадцать", " четырнадцать", " пятнадцать", " шестнадцать", " семнадцать", " восемнадцать", " девятнадцать" };
            string[] dek2 = { "", "", " двадцать", " тридцать", " сорок", " пятьдесят", " шестьдесят", " семьдесят", " восемьдесят", " девяносто" };
            string[] dek3 = { "", " сто", " двести", " триста", " четыреста", " пятьсот", " шестьсот", " семьсот", " восемьсот", " девятьсот" };
            string[] Th = { "", "", " тысяч", " миллион", " миллиард", " триллион", " квадрилион", " квинтилион" };
            string str = "";
            for (byte th = 1; value > 0; th++)
            {
                ushort gr = (ushort)(value % 1000);
                value = (value - gr) / 1000;
                if (gr > 0)
                {
                    byte d3 = (byte)((gr - gr % 100) / 100);
                    byte d1 = (byte)(gr % 10);
                    byte d2 = (byte)((gr - d3 * 100 - d1) / 10);
                    if (d2 == 1) d1 += (byte)10;
                    bool ismale = (th > 2) || ((th == 1) && isMale);
                    str = dek3[d3] + dek2[d2] + dek1[d1] + EndDek1(d1, ismale) + Th[th] + EndTh(th, d1) + str;
                };
            };
            str = str.Substring(1, 1).ToUpper() + str.Substring(2);
            return str;
        }

        private static string EndTh(byte thNum, byte dek)
        {
            bool in234 = ((dek >= 2) && (dek <= 4));
            bool more4 = ((dek > 4) || (dek == 0));
            if (((thNum > 2) && in234) || ((thNum == 2) && (dek == 1))) return "а";
            if ((thNum > 2) && more4) return "ов";
            if ((thNum == 2) && in234) return "и";
            else return "";
        }
        private static string EndDek1(byte dek, bool isMale)
        {
            if ((dek > 2) || (dek == 0)) return "";
            return dek == 1 ? (isMale ? "ин" : "на") : (isMale ? "а" : "е");
        }
    }
}