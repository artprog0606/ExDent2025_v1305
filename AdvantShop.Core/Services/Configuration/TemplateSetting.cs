//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Configuration
{
    public class TemplateOptionSetting
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string Image { get; set; }
    }

    public class TemplateSetting
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ETemplateSettingType Type { get; set; }
        public string TypeStr
        {
            get { return Type.ToString(); }
        }

        public string Value { get; set; }

        public object ValueObj
        {
            get
            {
                if (Type == ETemplateSettingType.Checkbox || Type == ETemplateSettingType.StaticBlockCheckbox)
                    return Value.TryParseBool();

                return Value;
            }
        }

        public bool Hidden { get; set; }

        public string DataType { get; set; }

        public string SectionName { get; set; }

        public List<TemplateOptionSetting> Options { get; set; }

        public bool IsAdditional { get; set; }
    }


    public class TemplateSettingBox
    {
        public string Message { get; set; }
        public List<TemplateSetting> Settings { get; set; }
    }

    public class TemplateSettingSection
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public List<TemplateSetting> Settings { get; set; }

        public bool IsOther
        {
            get
            {
                return !Enum.GetNames(typeof(ETemplateSettingSection)).Contains(Key);
            }
        }
    }

    public enum ETemplateSettingType
    {
        TextBox,
        Checkbox,
        DropDownList,

        StaticBlockCheckbox
    }

    public enum ETemplateSettingSection
    {
        [Localize("AdvantShop.Configuration.ETemplateSettingSection.MainPage")]
        MainPage,

        [Localize("AdvantShop.Configuration.ETemplateSettingSection.Category")]
        Category,

        [Localize("AdvantShop.Configuration.ETemplateSettingSection.Checkout")]
        Checkout,

        [Localize("AdvantShop.Configuration.ETemplateSettingSection.Design")]
        Design,

        [Localize("AdvantShop.Configuration.ETemplateSettingSection.Brands")]
        Brands,

        [Localize("AdvantShop.Configuration.ETemplateSettingSection.News")]
        News,

        [Localize("AdvantShop.Configuration.ETemplateSettingSection.Product")]
        Product
    }
}