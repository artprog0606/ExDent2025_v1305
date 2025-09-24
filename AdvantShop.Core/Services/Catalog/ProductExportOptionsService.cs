
using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Catalog
{
    public static class ProductExportOptionsService
    {
        public const string YandexSizeUnitValidate = "AU.DE.EU.FR.Japan.INT.IT.RU.RU_COM.UK.US.INCH.CM.HEIG.Height.Months.Round.Years";
        
        public static ProductExportOptions Get(int productId)
        {
            if (productId == 0)
                return new ProductExportOptions();
            
            return SQLDataAccess.ExecuteReadOne(
                "Select * from [Catalog].[ProductExportOptions] Where ProductId=@ProductId", CommandType.Text,
                GetProductExportOptionsFromReader,
                new SqlParameter("@ProductId", productId)) ??  new ProductExportOptions();
        }

        public static ProductExportOptions GetProductExportOptionsFromReader(SqlDataReader reader)
        {
            var exportParams = new ProductExportOptions()
            {
                Adult = SQLDataHelper.GetBoolean(reader, "Adult"),
                Gtin = SQLDataHelper.GetString(reader, "Gtin"),
                Mpn = SQLDataHelper.GetString(reader, "Mpn"),
                GoogleProductCategory = SQLDataHelper.GetString(reader, "GoogleProductCategory"),
                YandexSalesNote = SQLDataHelper.GetString(reader, "YandexSalesNote"),
                YandexTypePrefix = SQLDataHelper.GetString(reader, "YandexTypePrefix"),
                YandexModel = SQLDataHelper.GetString(reader, "YandexModel"),
                YandexName = SQLDataHelper.GetString(reader, "YandexName"),
                YandexDeliveryDays = SQLDataHelper.GetString(reader, "YandexDeliveryDays"),
                YandexSizeUnit = SQLDataHelper.GetString(reader, "YandexSizeUnit"),
                YandexProductDiscounted = SQLDataHelper.GetBoolean(reader, "YandexProductDiscounted"),
                YandexProductDiscountCondition = SQLDataHelper.GetString(reader, "YandexProductDiscountCondition")
                    .TryParseEnum<EYandexDiscountCondition>(),
                YandexProductDiscountReason = SQLDataHelper.GetString(reader, "YandexProductDiscountReason"),
                YandexProductQuality = (EYandexProductQuality)SQLDataHelper.GetInt(reader, "YandexProductQuality"),
                YandexMarketCategory = SQLDataHelper.GetString(reader, "YandexMarketCategory"),
                ManufacturerWarranty = SQLDataHelper.GetBoolean(reader, "ManufacturerWarranty"),
                Bid = SQLDataHelper.GetFloat(reader, "Bid"),
                YandexMarketExpiry = SQLDataHelper.GetString(reader, "YandexMarketExpiry"),
                YandexMarketWarrantyDays = SQLDataHelper.GetString(reader, "YandexMarketWarrantyDays"),
                YandexMarketCommentWarranty = SQLDataHelper.GetString(reader, "YandexMarketCommentWarranty"),
                YandexMarketPeriodOfValidityDays = SQLDataHelper.GetString(reader, "YandexMarketPeriodOfValidityDays"),
                YandexMarketServiceLifeDays = SQLDataHelper.GetString(reader, "YandexMarketServiceLifeDays"),
                YandexMarketTnVedCode = SQLDataHelper.GetString(reader, "YandexMarketTnVedCode"),
                YandexMarketStepQuantity = SQLDataHelper.GetNullableInt(reader, "YandexMarketStepQuantity"),
                YandexMarketMinQuantity = SQLDataHelper.GetNullableInt(reader, "YandexMarketMinQuantity"),
                YandexMarketCategoryId = SQLDataHelper.GetNullableLong(reader, "YandexMarketCategoryId"),
                GoogleAvailabilityDate = SQLDataHelper.GetNullableDateTime(reader, "GoogleAvailabilityDate"),
            };
            
            exportParams.IsChanged = false;

            return exportParams;
        }

        public static void AddOrUpdate(int productId, ProductExportOptions exportOptions, bool trackChanges = true, ChangedBy changedBy = null)
        {
            if (trackChanges)
                ProductHistoryService.TrackExportOptionsChanges(productId, exportOptions, changedBy);
            
            SQLDataAccess.ExecuteNonQuery(
                "if exists (Select 1 From [Catalog].[ProductExportOptions] Where ProductId=@ProductId) " +
                    "Update [Catalog].[ProductExportOptions] " +
                    "Set Adult=@Adult, Gtin=@Gtin, GoogleProductCategory=@GoogleProductCategory, YandexSalesNote=@YandexSalesNote, " +
                    "YandexTypePrefix=@YandexTypePrefix, YandexModel=@YandexModel, YandexName=@YandexName, YandexDeliveryDays=@YandexDeliveryDays, " +
                    "YandexSizeUnit=@YandexSizeUnit, YandexProductDiscounted=@YandexProductDiscounted, YandexProductDiscountCondition=@YandexProductDiscountCondition, " +
                    "YandexProductDiscountReason=@YandexProductDiscountReason, YandexMarketCategory=@YandexMarketCategory, ManufacturerWarranty=@ManufacturerWarranty, Bid=@Bid," +
                    "YandexMarketExpiry=@YandexMarketExpiry, YandexMarketWarrantyDays=@YandexMarketWarrantyDays, YandexMarketCommentWarranty=@YandexMarketCommentWarranty," +
                    "YandexMarketPeriodOfValidityDays=@YandexMarketPeriodOfValidityDays, YandexMarketServiceLifeDays=@YandexMarketServiceLifeDays, YandexMarketTnVedCode=@YandexMarketTnVedCode, " +
                    "YandexMarketStepQuantity=@YandexMarketStepQuantity, YandexMarketMinQuantity=@YandexMarketMinQuantity, YandexProductQuality=@YandexProductQuality, Mpn=@Mpn," +
                    "YandexMarketCategoryId=@YandexMarketCategoryId, GoogleAvailabilityDate=@GoogleAvailabilityDate " +
                    "Where ProductId=@ProductId " +
                "else " +
                    "Insert Into [Catalog].[ProductExportOptions] (ProductId, Adult, Gtin, GoogleProductCategory, YandexSalesNote, YandexTypePrefix, " +
                    "YandexModel, YandexName, YandexDeliveryDays, YandexSizeUnit, YandexProductDiscounted, YandexProductDiscountCondition, YandexProductDiscountReason, " +
                    "YandexMarketCategory, ManufacturerWarranty, Bid, YandexMarketExpiry, YandexMarketWarrantyDays, YandexMarketCommentWarranty, " +
                    "YandexMarketPeriodOfValidityDays, YandexMarketServiceLifeDays, YandexMarketTnVedCode, YandexMarketStepQuantity, YandexMarketMinQuantity, YandexProductQuality, Mpn, " +
                    "YandexMarketCategoryId, GoogleAvailabilityDate)" +
                    "Values (@ProductId, @Adult, @Gtin, @GoogleProductCategory, @YandexSalesNote, @YandexTypePrefix, " +
                    "@YandexModel, @YandexName, @YandexDeliveryDays, @YandexSizeUnit, @YandexProductDiscounted, @YandexProductDiscountCondition, @YandexProductDiscountReason, " +
                    "@YandexMarketCategory, @ManufacturerWarranty, @Bid, @YandexMarketExpiry, @YandexMarketWarrantyDays, @YandexMarketCommentWarranty, " +
                    "@YandexMarketPeriodOfValidityDays, @YandexMarketServiceLifeDays, @YandexMarketTnVedCode, @YandexMarketStepQuantity, @YandexMarketMinQuantity, @YandexProductQuality, @Mpn, " +
                    "@YandexMarketCategoryId, @GoogleAvailabilityDate) ",
                CommandType.Text,
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@Adult", exportOptions.Adult),
                new SqlParameter("@Gtin", exportOptions.Gtin ?? (object)DBNull.Value),
                new SqlParameter("@GoogleProductCategory", exportOptions.GoogleProductCategory ?? (object)DBNull.Value),
                new SqlParameter("@YandexSalesNote", exportOptions.YandexSalesNote ?? (object)DBNull.Value),
                new SqlParameter("@YandexTypePrefix", exportOptions.YandexTypePrefix ?? (object)DBNull.Value),
                new SqlParameter("@YandexModel", exportOptions.YandexModel ?? (object)DBNull.Value),
                new SqlParameter("@YandexName", string.IsNullOrWhiteSpace(exportOptions.YandexName) ? string.Empty : exportOptions.YandexName),
                new SqlParameter("@YandexDeliveryDays", exportOptions.YandexDeliveryDays ?? (object)DBNull.Value),
                new SqlParameter("@YandexSizeUnit", string.IsNullOrEmpty(exportOptions.YandexSizeUnit) ? (object)DBNull.Value : exportOptions.YandexSizeUnit),
                new SqlParameter("@YandexProductDiscounted", exportOptions.YandexProductDiscounted),
                new SqlParameter("@YandexProductDiscountCondition", exportOptions.YandexProductDiscountCondition == EYandexDiscountCondition.None ? (object)DBNull.Value : exportOptions.YandexProductDiscountCondition.ToString()),
                new SqlParameter("@YandexProductDiscountReason", exportOptions.YandexProductDiscountReason ?? (object)DBNull.Value),
                new SqlParameter("@YandexProductQuality", exportOptions.YandexProductQuality == EYandexProductQuality.None ? (object)DBNull.Value : (int)exportOptions.YandexProductQuality),
                new SqlParameter("@YandexMarketCategory", exportOptions.YandexMarketCategory ?? (object)DBNull.Value),
                new SqlParameter("@ManufacturerWarranty", exportOptions.ManufacturerWarranty),
                new SqlParameter("@Bid", exportOptions.Bid),
                new SqlParameter("@YandexMarketExpiry", exportOptions.YandexMarketExpiry ?? (object)DBNull.Value),
                new SqlParameter("@YandexMarketWarrantyDays", exportOptions.YandexMarketWarrantyDays ?? (object)DBNull.Value),
                new SqlParameter("@YandexMarketCommentWarranty", exportOptions.YandexMarketCommentWarranty ?? (object)DBNull.Value),
                new SqlParameter("@YandexMarketPeriodOfValidityDays", exportOptions.YandexMarketPeriodOfValidityDays ?? (object)DBNull.Value),
                new SqlParameter("@YandexMarketServiceLifeDays", exportOptions.YandexMarketServiceLifeDays ?? (object)DBNull.Value),
                new SqlParameter("@YandexMarketTnVedCode", exportOptions.YandexMarketTnVedCode ?? (object)DBNull.Value),
                new SqlParameter("@YandexMarketStepQuantity", exportOptions.YandexMarketStepQuantity ?? (object)DBNull.Value),
                new SqlParameter("@YandexMarketMinQuantity", exportOptions.YandexMarketMinQuantity ?? (object)DBNull.Value),
                new SqlParameter("@Mpn", exportOptions.Mpn ?? (object)DBNull.Value),
                new SqlParameter("@YandexMarketCategoryId", exportOptions.YandexMarketCategoryId ?? (object)DBNull.Value),
                new SqlParameter("@GoogleAvailabilityDate", exportOptions.GoogleAvailabilityDate ?? (object)DBNull.Value)
            );
        }
    }
}