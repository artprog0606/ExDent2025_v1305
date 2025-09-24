using System.Collections.Generic;

namespace AdvantShop.Catalog
{
    public class ProductLabel
    {
        public string LabelCode { get; set; }
        public List<int> ProductIds { get; set; } 
    }
    
    public class ProductMarker
    {
        public string Title { get; set; }
        public string ColorBackground { get; set; }
        public string ColorText { get; set; }
        public string Url { get; set; }
        public bool OpenInNewTab { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
    }

    public class ProductMarkerMap
    {
        public int ProductId { get; set; }
        public List<ProductMarker> Markers { get; set; }
    }
}