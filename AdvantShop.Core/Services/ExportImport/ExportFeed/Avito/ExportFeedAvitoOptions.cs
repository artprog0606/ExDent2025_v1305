using System;
using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;

namespace AdvantShop.ExportImport
{
    public enum EPaidPublicationOption
    {
        //размещение объявления осуществляется только при наличии подходящего пакета размещения;
        [StringName("Package")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidPublicationOption.Package")]
        Package,

        //при наличии подходящего пакета оплата размещения объявления произойдет с него; если нет подходящего пакета, но достаточно денег на кошельке Avito, то произойдет разовое размещение;
        [StringName("PackageSingle")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidPublicationOption.PackageSingle")]
        PackageSingle,

        //только разовое размещение, произойдет при наличии достаточной суммы на кошельке Avito; если есть подходящий пакет размещения, он будет проигнорирован.
        [StringName("Single")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidPublicationOption.Single")]
        Single
    }

    public enum EPaidServices
    {
        // обычное объявление;
        [StringName("Free")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.Free")]
        Free,

        //премиум-объявление;
        [StringName("Premium")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.Premium")]
        Premium,

        //VIP-объявление;
        [StringName("VIP")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.VIP")]
        VIP,

        //поднятие объявления в поиске;
        [StringName("PushUp")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.PushUp")]
        PushUp,

        //выделение объявления;
        [StringName("Highlight")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.Highlight")]
        Highlight,

        //применение пакета «Турбо-продажа»;
        [StringName("TurboSale")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.TurboSale")]
        TurboSale,

        //применение пакета «Быстрая продажа».
        [StringName("QuickSale")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.QuickSale")]
        QuickSale
    }

    public enum EAvitoExportMode
    {
        [Localize("Core.ExportImport.ExportFeedAvito.EAvitoExportMode.Product")]
        Product = 0,
        
        [Localize("Core.ExportImport.ExportFeedAvito.EAvitoExportMode.Offer")]
        Offer = 1
    }


    [Serializable()]
    public class ExportFeedAvitoOptions : IExportFeedCsvFilterOptions
    {
        public bool ExportNotAvailable { get; set; }
        [JsonIgnore]
        public bool CsvExportNoInCategory => false;
        [JsonIgnore]
        public bool ExportFromMainCategories => false;

        public string Currency { get; set; }
        public int PublicationDateOffset { get; set; }
        public int DurationOfPublicationInDays { get; set; }
        public EPaidPublicationOption PaidPublicationOption { get; set; }
        public EPaidServices PaidServices { get; set; }
        public bool EmailMessages { get; set; }
        public string ManagerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ProductDescriptionType { get; set; }
        public string DefaultAvitoCategory { get; set; }
        public bool UnloadProperties { get; set; }
        public bool IsActiveAboveAdditionalDescription { get; set; }
        public bool IsActiveBelowAdditionalDescription { get; set; }
        public string AboveAdditionalDescription { get; set; }
        public string BelowAdditionalDescription { get; set; }
        
        public bool NotExportColorSize { get; set; }
        public EAvitoExportMode ExportMode { get; set; }
        
        public int? PriceRuleId { get; set; }
    }
}
