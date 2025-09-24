namespace AdvantShop.ExportImport
{
    public class ExportProductModel
    {
        public int ProductId { get; set; }
        public int OfferId { get; set; }
        public string ArtNo { get; set; }
        public string Name { get; set; }
        public string UrlPath { get; set; }

        public string BriefDescription { get; set; }
        public string Description { get; set; }
        
        public string Title { get; set; }
        public string H1 { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }

        public string BrandName { get; set; }
        public string SizeChart { get; set; }
    }
}