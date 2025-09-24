using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.ExportImport
{
    public enum EAvitoCommonTags
    {
        [StringName("AvitoId")]
        AvitoId,
        [StringName("DateBegin")]
        DateBegin,
        [StringName("DateEnd")]
        DateEnd,
        [StringName("ListingFee")]
        ListingFee,
        [StringName("AdStatus")]
        AdStatus,
        [StringName("AllowEmail")]
        AllowEmail,
        [StringName("ManagerName")]
        ManagerName,
        [StringName("ContactPhone")]
        ContactPhone,
        [StringName("Address")]
        Address
    }

    public class ExportFeedAvitoProductProperty
    {
        public int ProductId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
    }
}
