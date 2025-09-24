using System;
using AdvantShop.Core.Services.TemplatesDocx;

namespace AdvantShop.Core.Common.Attributes.TemplateDocx
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TemplateDocxPropertyAttribute : Attribute
    {
        public TemplateDocxPropertyAttribute(string key)
        {
            Key = key;
            Type = TypeItem.Field;
        }

        public string Key { get; set; }
        public TypeItem Type { get; set; }
        public string LocalizeDescription { get; set; }
        /// <summary>
        /// Скрывает из описания, но продолжает работать
        /// </summary>
        public bool Hide { get; set; }
    }
}
