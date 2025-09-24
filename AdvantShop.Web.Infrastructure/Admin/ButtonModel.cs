using AdvantShop.Core.Common.Attributes;
using AdvantShop.Web.Infrastructure.Localization;
using System.Collections.Generic;

namespace AdvantShop.Web.Infrastructure.Admin.Buttons
{
    public enum eButtonType
    {
        Simple,
        Add,
        Save,
        Link
    }

    public enum eButtonSize
    {
        Default,
        Large,
        XMiddle,
        Middle,
        Small,
        XSmall,
        None
    }

    public enum eColorType
    {
        Default,
        Primary,
        Success,
        Info,
        InfoOutline,
        Warning,
        Danger,
        Link,
        Secondary,
        Tertiary
    }

    public enum eButtonModificators {
        [StringName("btn--only-icon")]
        OnlyIcon,
        [StringName("btn--full-width")]
        FullWidth,
        [StringName("btn--horizontal-padding-zero")]
        HorizontalPaddingZero
    }

    public class ButtonIcon
    {
        public string Source { get; set; }
        public string FaName { get; set; }
        public string SvgName { get; set; }
        public object Attributes { get; set; }
        public string CssClass { get; set; }
    }

    public class ButtonModel: IButton
    {

        public ButtonModel()
        {
            if (Type == eButtonType.Add)
            {
                Icon = new ButtonIcon()
                {
                    FaName = "plus"
                };
            }else if (Type == eButtonType.Save)
            {

                Icon = new ButtonIcon()
                {
                    FaName = "check"
                };
            }
        }

        public string Name { get; set; }
        public eButtonType Type { get; set; }
        public eColorType ColorType { get; set; }
        public eButtonSize Size { get; set; }
        public LocalizedString Text { get; set; }
        public string Link { get; set; }
        public string CssClass { get; set; }
        public string[] Attributes { get; set; }
        public bool Validation { get; set; }
        public bool IsOutline { get; set; }
        public ButtonIcon Icon { get; set; }    
        public string SizeCssClass
        {
            get {
                switch (Size)
                {
                    case eButtonSize.Large: return "lg";
                    case eButtonSize.XMiddle: return "xmd";
                    case eButtonSize.Middle: return "md";
                    case eButtonSize.Small: return "sm";
                    case eButtonSize.XSmall: return "xs";
                    case eButtonSize.Default:
                    default:
                        return string.Empty;
                }
            }
        }
        public string AttibutesString
        {
            get
            {
                if (Attributes == null)
                    return string.Empty;

                string res = "";
                foreach (var str in Attributes)
                {
                    res += str + " ";
                }
                return res.Trim();
            }
        }
        public List<eButtonModificators> Modificators { get; set; }
        public string WrapStart { get; set; }
        public string WrapEnd { get; set; }
        public string HtmlElement { get; set; }
    }

    public interface IButton
    {
        string Name { get; set; }
        eButtonType Type { get; set; }
        eColorType ColorType { get; set; }
        eButtonSize Size { get; set; }
        LocalizedString Text { get; set; }
        string Link { get; set; }
        string CssClass { get; set; }
        string[] Attributes { get; set; }
        bool Validation { get; set; }
        bool IsOutline { get; set; }
        ButtonIcon Icon { get; set; }
        string SizeCssClass { get; }
        string AttibutesString { get; }
        List<eButtonModificators> Modificators { get; set; }
        string WrapStart { get; set; }
        string WrapEnd { get; set; }
    }
}
