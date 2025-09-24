using AdvantShop.Core;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Models.Common
{
    public class SvgSprite
    {
        //private AssetsTool.PathData _pathData = new AssetsTool.PathData();

        public SvgSprite(string name, string cssClass, string svgAttributes, string spriteFileName, string areaName)
        {
            Name = name;
            CssClass = cssClass;
            SvgAttributes = svgAttributes;
            SpriteFileName = spriteFileName;
            AreaName = areaName;
        }

        public string Name { get; set; }
        public string CssClass { get; set; }
        public string SvgAttributes { get; set; }
        public string SpriteFileName { get; set; }
        public string AreaName { get; set; }
        public string SpriteFullPath
        {
            get
            {
                return new AssetsTool.PathData(this.AreaName).GetPathByOriginalFileName(SpriteFileName + ".svg");
            }
        }
    }
}