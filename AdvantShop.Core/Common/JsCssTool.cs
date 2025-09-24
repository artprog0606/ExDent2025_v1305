using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using SquishIt.Framework;

namespace AdvantShop.Core
{
    public class JsCssTool
    {
        private const string CacheKeyPrefix = "squishit_";

        static JsCssTool()
        {
            SquishIt.Framework.Configuration.Apply(SquishIt.AspNet.ConfigurationLoader.RegisterPlatform);
        }

        public static string MiniCss(List<string> paths, string filename, string outputFolder = null, bool inline = false)
        {
            if (inline)
            {
                var cssContent = CacheManager.Get<string>("squishit_rawcontent_css_raw_" + filename);
                if (!string.IsNullOrEmpty(cssContent))
                    return cssContent;
            }

            var outputfile = string.IsNullOrEmpty(outputFolder)
                                ? "~/" + FoldersHelper.PhotoFoldersPath[FolderType.Combine] + filename
                                : outputFolder + filename;

            var bundle = Bundle.Css();
            
            foreach (var item in paths)
                bundle.Add(item);

            var result = bundle.WithMinifier(new SquishIt.Framework.Minifiers.CSS.MsMinifier())
                               .ForceDebugIf(() => DebugMode.IsDebugMode(eDebugMode.Css));

            return inline ? result.RenderRawContent(filename) : result.Render(outputfile);
        }

        public static string MiniJs(List<string> paths, string filename, string outputFolder = null)
        {
            var outputfile = string.IsNullOrEmpty(outputFolder)
                ? "~/" + FoldersHelper.PhotoFoldersPath[FolderType.Combine] + filename
                : outputFolder + filename;

            var bundle = Bundle.JavaScript();

            foreach (var item in paths)
                bundle.Add(item);

            return bundle.WithMinifier(new SquishIt.Framework.Minifiers.JavaScript.MsMinifier())
                         .ForceDebugIf(() => DebugMode.IsDebugMode(eDebugMode.Js))
                         .Render(outputfile);
        }

        public static void ReCreateIfNotExist()
        {
            if (!FileHelpers.IsDirectoryHaveFiles(SettingsGeneral.AbsolutePath + "/" + FoldersHelper.PhotoFoldersPath[FolderType.Combine]))
            {
                CacheManager.RemoveByPattern(CacheKeyPrefix);
            }
        }

        public static void Clear()
        {
            FileHelpers.DeleteFilesFromPath(SettingsGeneral.AbsolutePath + "/" + FoldersHelper.PhotoFoldersPath[FolderType.Combine]);
        }
    }
}