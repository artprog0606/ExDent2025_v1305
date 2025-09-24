using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.TemplatesDocx
{
    public class TemplateDocxItem
    {
        private TemplateDocxItem() { }

        public TemplateDocxItem(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key", "key is null");
            if (key.Contains(":"))
                throw new ArgumentException("the key contains a char ':'", "key");

            Key = key;
        }
        public string Key { get; private set; }

        public string Description { get; set; }
        public TypeItem Type { get; set; }
        public object Value { get; set; }
        public Type TypeValue { get; set; }
        public List<TemplateDocxItem[]> ChildItems { get; set; }
        public bool Hidden { get; set; }
    }

    public enum TypeItem
    {
        Field,
        InheritedFields,
        Image,
        Table,
        Repeat,
        List,
    }
}
