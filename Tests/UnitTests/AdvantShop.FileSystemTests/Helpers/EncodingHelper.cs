using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UtfUnknown;

namespace AdvantShop.FileSystemTests.Helpers
{
    public static class EncodingHelper
    {
        private static readonly List<string> DirectoryBlackList = new()
        {
            ".git",
            ".vs",
            ".idea",
            "bin",
            "obj",
            "dist",
            "vendors",
            "node_modules",
            "Modules",
            "Templates"
        };

        public static Encoding? GetEncoding(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var detection = CharsetDetector.DetectFromFile(filePath);
            return detection?.Detected?.Encoding;
        }

        /// <summary>
        ///     берем сами первые байты, т.к. GetPreamble всегда выдавал BOM для UTF
        /// </summary>
        public static bool CheckBom(string filePath)
        {
            var bom = new byte[4];
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                _ = file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            return bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf;
        }

        public static (bool success, long numberOfCheckedFiles, List<string> errors) CheckFilesEncodingInDirectory(DirectoryInfo directoryInfo, List<Encoding> allowedEncodings,
            string fileExtension, List<string> fileWhiteList,
            bool withBom = false, bool recursive = true)
        {
            var result = (success: true, numberOfCheckedFiles: (long)0, errors: new List<string>());

            foreach (var file in directoryInfo.GetFiles()
                         .Where(f => !fileWhiteList.Contains(f.Name, StringComparer.OrdinalIgnoreCase)
                                     && Path.GetExtension(f.Name).Equals(fileExtension, StringComparison.OrdinalIgnoreCase)))
            {
                result.numberOfCheckedFiles++;

                var fileEncoding = GetEncoding(file.FullName);
                if (fileEncoding is null)
                {
                    result.success = false;
                    continue;
                }

                if (allowedEncodings.Contains(fileEncoding))
                    if (!withBom || !CheckBom(file.FullName))
                        continue;

                result.errors.Add($"{file.FullName} - wrong encoding - {fileEncoding.EncodingName}");

                result.success = false;
            }

            if (!recursive)
                return result;

            foreach (var subDirectory in directoryInfo.GetDirectories().Where(d => !DirectoryBlackList.Contains(d.Name, StringComparer.OrdinalIgnoreCase)))
            {
                var (success, numberOfCheckedFiles, errors) = CheckFilesEncodingInDirectory(subDirectory, allowedEncodings, fileExtension, fileWhiteList, withBom);
                result.numberOfCheckedFiles += numberOfCheckedFiles;

                if (success)
                    continue;

                result.success = false;
                result.errors.AddRange(errors);
            }

            return result;
        }
    }
}
