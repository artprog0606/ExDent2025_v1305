
namespace AdvantShop.Web.Admin.Handlers.Design
{
    public class CssEditorHandler
    {
        public string GetFileContent()
        {
            return FilePath.FoldersHelper.ReadCss(FilePath.CssType.extra);
        }

        public bool SaveFileContent(string cssContent)
        {
            return FilePath.FoldersHelper.SaveCSS(cssContent, FilePath.CssType.extra);
        }
    }
}
