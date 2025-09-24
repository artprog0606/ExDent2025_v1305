using System;
using System.IO;
using System.Text;
using Ude.Core;

namespace AdvantShop.Core.Services.Helpers
{
    public sealed class EncodingDetector
    {
        /// <summary>
        /// Try to detect encoding for UTF-7, UTF-8/16/32 (bom, no bom, little and big endian), windows-1251
        /// </summary>
        /// <param name="b">Bytes array</param>
        /// <param name="encoding">Detected encoding or Encoding.Default</param>
        /// <param name="taster">Number of bytes to check.
        /// Higher value is slower, but more reliable (especially UTF-8 with special characters later on may appear to be ASCII initially).
        /// If taster = 0, then taster becomes full length.</param>
        /// <returns>true and encoding, if detect, false and Encoding.Default, if not detect</returns>
        public bool TryDetect(byte[] b, out Encoding encoding, int taster = 1000)
        {
            // https://stackoverflow.com/questions/1025332/determine-a-strings-encoding-in-c-sharp

            //////////////// First check the low hanging fruit by checking if a
            //////////////// BOM/signature exists (sourced from http://www.unicode.org/faq/utf_bom.html#bom4)
            if (b.Length >= 4 && b[0] == 0x00 && b[1] == 0x00 && b[2] == 0xFE && b[3] == 0xFF) { encoding = Encoding.GetEncoding("utf-32BE"); return true; }  // UTF-32, big-endian 
            else if (b.Length >= 4 && b[0] == 0xFF && b[1] == 0xFE && b[2] == 0x00 && b[3] == 0x00) { encoding = Encoding.UTF32; return true; }    // UTF-32, little-endian
            else if (b.Length >= 2 && b[0] == 0xFE && b[1] == 0xFF) { encoding = Encoding.BigEndianUnicode; return true; }     // UTF-16, big-endian
            else if (b.Length >= 2 && b[0] == 0xFF && b[1] == 0xFE) { encoding = Encoding.Unicode; return true; }              // UTF-16, little-endian
            else if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF) { encoding = Encoding.UTF8; return true; } // UTF-8
            else if (b.Length >= 3 && b[0] == 0x2b && b[1] == 0x2f && b[2] == 0x76) { encoding = Encoding.UTF7; return true; } // UTF-7


            //////////// If the code reaches here, no BOM/signature was found, so now
            //////////// we need to 'taste' the file to see if can manually discover
            //////////// the encoding. A high taster value is desired for UTF-8
            if (taster == 0 || taster > b.Length) taster = b.Length;    // Taster size can't be bigger than the filesize obviously.


            // Some text files are encoded in UTF8, but have no BOM/signature. Hence
            // the below manually checks for a UTF8 pattern. This code is based off
            // the top answer at: https://stackoverflow.com/questions/6555015/check-for-invalid-utf8
            // For our purposes, an unnecessarily strict (and terser/slower)
            // implementation is shown at: https://stackoverflow.com/questions/1031645/how-to-detect-utf-8-in-plain-c
            // For the below, false positives should be exceedingly rare (and would
            // be either slightly malformed UTF-8 (which would suit our purposes
            // anyway) or 8-bit extended ASCII/UTF-16/32 at a vanishingly long shot).
            int i = 0;
            bool utf8 = false;
            while (i < taster - 4)
            {
                if (b[i] <= 0x7F) { i += 1; continue; }     // If all characters are below 0x80, then it is valid UTF8, but UTF8 is not 'required' (and therefore the text is more desirable to be treated as the default codepage of the computer). Hence, there's no "utf8 = true;" code unlike the next three checks.
                if (b[i] >= 0xC2 && b[i] <= 0xDF && b[i + 1] >= 0x80 && b[i + 1] < 0xC0) { i += 2; utf8 = true; continue; }
                if (b[i] >= 0xE0 && b[i] <= 0xF0 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0) { i += 3; utf8 = true; continue; }
                if (b[i] >= 0xF0 && b[i] <= 0xF4 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0 && b[i + 3] >= 0x80 && b[i + 3] < 0xC0) { i += 4; utf8 = true; continue; }
                utf8 = false; break;
            }
            
            if (utf8)
            {
                encoding = Encoding.UTF8; 
                return true;
            }


            // The next check is a heuristic attempt to detect UTF-16 without a BOM.
            // We simply look for zeroes in odd or even byte places, and if a certain
            // threshold is reached, the code is 'probably' UF-16.          
            double threshold = 0.1; // proportion of chars step 2 which must be zeroed to be diagnosed as utf-16. 0.1 = 10%
            int count = 0;
            for (int n = 0; n < taster; n += 2) if (b[n] == 0) count++;
            if (((double)count) / taster > threshold) { encoding = Encoding.BigEndianUnicode; return true; }
            count = 0;
            for (int n = 1; n < taster; n += 2) if (b[n] == 0) count++;
            if (((double)count) / taster > threshold) { encoding = Encoding.Unicode; return true; } // (little-endian)

            
            for (int n = 0; n < taster - 9; n++)
            {
                if (
                    ((b[n + 0] == 'c' || b[n + 0] == 'C') && (b[n + 1] == 'h' || b[n + 1] == 'H') && (b[n + 2] == 'a' || b[n + 2] == 'A') && (b[n + 3] == 'r' || b[n + 3] == 'R') && (b[n + 4] == 's' || b[n + 4] == 'S') && (b[n + 5] == 'e' || b[n + 5] == 'E') && (b[n + 6] == 't' || b[n + 6] == 'T') && (b[n + 7] == '=')) ||
                    ((b[n + 0] == 'e' || b[n + 0] == 'E') && (b[n + 1] == 'n' || b[n + 1] == 'N') && (b[n + 2] == 'c' || b[n + 2] == 'C') && (b[n + 3] == 'o' || b[n + 3] == 'O') && (b[n + 4] == 'd' || b[n + 4] == 'D') && (b[n + 5] == 'i' || b[n + 5] == 'I') && (b[n + 6] == 'n' || b[n + 6] == 'N') && (b[n + 7] == 'g' || b[n + 7] == 'G') && (b[n + 8] == '='))
                    )
                {
                    if (b[n + 0] == 'c' || b[n + 0] == 'C') n += 8; else n += 9;
                    if (b[n] == '"' || b[n] == '\'') n++;
                    int oldn = n;
                    while (n < taster && (b[n] == '_' || b[n] == '-' || (b[n] >= '0' && b[n] <= '9') || (b[n] >= 'a' && b[n] <= 'z') || (b[n] >= 'A' && b[n] <= 'Z')))
                    { n++; }
                    byte[] nb = new byte[n - oldn];
                    Array.Copy(b, oldn, nb, 0, n - oldn);
                    try
                    {
                        string internalEnc = Encoding.ASCII.GetString(nb);
                        encoding = Encoding.GetEncoding(internalEnc); 
                        return true;
                    }
                    catch { break; }    // If C# doesn't recognize the name of the encoding, break.
                }
            }

            if (IsWindows1251Encoding(b))
            {
                encoding = Encoding.GetEncoding(1251);
                return true;
            }

            encoding = Encoding.Default;
            return false;
        }
        
        public bool IsWindows1251Encoding(byte[] bytes)
        {
            var buff = new byte[1024];
            var length = 0;

            if (bytes.Length > 1024)
            {
                Array.Copy(bytes, 0, buff, 0, 1024);
                length = 1024;
            }
            else
            {
                buff = bytes;
                length = bytes.Length;
            }
            
            var prober = new SingleByteCharSetProber(new Win1251Model());

            var newBuf = FilterWithoutEnglishLetters(buff, 0, length);
            if (newBuf.Length == 0)
                return false;

            var state = prober.HandleData(newBuf, 0, newBuf.Length);
            if (state == ProbingState.FoundIt)
                return true;

            var confidence = prober.GetConfidence();

            return confidence >= 0.9;
        }

        private const byte SPACE = 0x20;
        private const byte CAPITAL_A = 0x41;
        private const byte CAPITAL_Z = 0x5A;
        private const byte SMALL_A = 0x61;
        private const byte SMALL_Z = 0x7A;

        private static byte[] FilterWithoutEnglishLetters(byte[] buf, int offset, int len)
        {
            byte[] result = null;

            using (MemoryStream ms = new MemoryStream(buf.Length))
            {
                bool meetMSB = false;
                int max = offset + len;
                int prev = offset;
                int cur = offset;

                while (cur < max)
                {
                    byte b = buf[cur];

                    if ((b & 0x80) != 0)
                    {
                        meetMSB = true;
                    }
                    else if (b < CAPITAL_A || (b > CAPITAL_Z && b < SMALL_A)
                                           || b > SMALL_Z)
                    {
                        if (meetMSB && cur > prev)
                        {
                            ms.Write(buf, prev, cur - prev);
                            ms.WriteByte(SPACE);
                            meetMSB = false;
                        }

                        prev = cur + 1;
                    }

                    cur++;
                }

                if (meetMSB && cur > prev)
                    ms.Write(buf, prev, cur - prev);
                ms.SetLength(ms.Position);
                result = ms.ToArray();
            }

            return result;
        }
    }
}