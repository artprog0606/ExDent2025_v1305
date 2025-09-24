using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.ExportImport
{
    public enum EBrandFields
    {
        [StringName("none")]
        [Localize("Core.ExportImport.BrandFields.NotSelected")]
        [CsvFieldsStatus(CsvFieldStatus.None)]
        None,

        [StringName("название")]
        [Localize("Core.ExportImport.BrandFields.Name")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Name,

        [StringName("описание")]
        [Localize("Core.ExportImport.BrandFields.Description")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Description,

        [StringName("краткое описание")]
        [Localize("Core.ExportImport.BrandFields.BriefDescription")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        BriefDescription,

        [StringName("активность")]
        [Localize("Core.ExportImport.BrandFields.Enabled")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Enabled,

        [StringName("синоним для url запроса")]
        [Localize("Core.ExportImport.BrandFields.UrlPath")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        UrlPath,

        [StringName("сайт производителя")]
        [Localize("Core.ExportImport.BrandFields.BrandSiteUrl")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        BrandSiteUrl,

        [StringName("id страны")]
        [Localize("Core.ExportImport.BrandFields.CountryId")]
        [CsvFieldsStatus(CsvFieldStatus.Int)]
        CountryId,

        [StringName("название страны")]
        [Localize("Core.ExportImport.BrandFields.CountryName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CountryName,

        [StringName("id страны производства")]
        [Localize("Core.ExportImport.BrandFields.CountryOfManufactureId")]
        [CsvFieldsStatus(CsvFieldStatus.Int)]
        CountryOfManufactureId,

        [StringName("название страны производства")]
        [Localize("Core.ExportImport.BrandFields.CountryOfManufactureName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CountryOfManufactureName,

        [StringName("фото")]
        [Localize("Core.ExportImport.BrandFields.Photo")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Photo
    }
}
