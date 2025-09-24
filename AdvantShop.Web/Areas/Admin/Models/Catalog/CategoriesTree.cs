using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Catalog
{
    public class CategoriesTree
    {
        public string Id { get; set; }

        public int? CategoryIdSelected { get; set; }

        public string ExcludeIds { get; set; }
        public string SelectedIds { get; set; }

        public bool ShowRoot { get; set; }

        public CategoriesTreeState State { get; set; }
        public CategoriesTreeState StateOriginal { get; set; }
        public CategoriesTreeCheckbox Checkbox { get; set; }
    }

    public class CategoriesTreeState
    {
        public bool Loaded { get; set; }
        public bool Opened { get; set; }
        public bool Selected { get; set; }
        public bool Disabled { get; set; }
        public bool Enabled { get; set; }
        public bool Failed { get; set; }
        public bool Loading { get; set; }
    }

    public class CategoriesTreeCheckbox
    {
        public eTreeCascade Cascade { get; set; }
        [JsonProperty("tie_selection")]
        public bool TieSelection { get; set; }
    }

    public enum eTreeCascade
    {
        Undetermined = 0,
        Up = 1,
        Down = 2
    }
}
