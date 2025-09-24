//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using Newtonsoft.Json;

namespace AdvantShop.Catalog
{

    public class CustomOptionsService
    {
        private const string CustomOptionsCachePrefix = "CustomOptions_";
        private const string CustomOptionsByProductIdCachePrefix = CustomOptionsCachePrefix + "_ByProductId_";
        
        #region  Public CustomOption Methods

        public static int AddCustomOption(CustomOption copt)
        {
            copt.CustomOptionsId = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Catalog].[CustomOptions] " +
                "([Title],[IsRequired],[InputType],[SortOrder],[ProductID],[MinQuantity],[MaxQuantity],[Description]) " +
                "VALUES " +
                "(@Title,@IsRequired,@InputType,@SortOrder,@ProductID,@MinQuantity,@MaxQuantity, @Description);" +
                "SELECT SCOPE_IDENTITY()",
                CommandType.Text,
                new SqlParameter("@Title", copt.Title),
                new SqlParameter("@IsRequired", copt.IsRequired),
                new SqlParameter("@InputType", copt.InputType),
                new SqlParameter("@SortOrder", copt.SortOrder),
                new SqlParameter("@ProductID", copt.ProductId),
                new SqlParameter("@MinQuantity", copt.MinQuantity ?? (object)DBNull.Value),
                new SqlParameter("@MaxQuantity", copt.MaxQuantity ?? (object)DBNull.Value),
                new SqlParameter("@Description", copt.Description ?? string.Empty)
                );

            if (copt.CustomOptionsId != 0)
            {
                foreach (var optionItem in copt.Options)
                {
                    if (optionItem.Title != null)
                    {
                        AddOption(optionItem, copt.CustomOptionsId);
                    }
                }
            }
            
            CacheManager.RemoveByPattern(CustomOptionsByProductIdCachePrefix + copt.ProductId);

            return copt.CustomOptionsId;
        }

        public static void UpdateCustomOption(CustomOption copt)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[CustomOptions] " +
                "SET " +
                "[Title] = @Title " +
                ",[IsRequired] = @IsRequired " +
                ",[InputType] = @InputType " +
                ",[SortOrder] = @SortOrder " +
                ",[ProductID] = @ProductID " +
                ",[MinQuantity] = @MinQuantity " +
                ",[MaxQuantity] = @MaxQuantity " +
                ",[Description] = @Description " +
                "WHERE [CustomOptionsID] = @CustomOptionsId",
                CommandType.Text,
                new SqlParameter("@CustomOptionsId", copt.CustomOptionsId),
                new SqlParameter("@Title", copt.Title),
                new SqlParameter("@IsRequired", copt.IsRequired),
                new SqlParameter("@InputType", copt.InputType),
                new SqlParameter("@SortOrder", copt.SortOrder),
                new SqlParameter("@ProductID", copt.ProductId),
                new SqlParameter("@MinQuantity", copt.MinQuantity ?? (object)DBNull.Value),
                new SqlParameter("@MaxQuantity", copt.MaxQuantity ?? (object)DBNull.Value),
                new SqlParameter("@Description", copt.Description ?? string.Empty)
                );

            foreach (var option in copt.Options.Where(x => x.OptionId > 0))
                UpdateOption(option, copt.CustomOptionsId);
            foreach (var option in copt.Options.Where(x => x.OptionId < 1))
                AddOption(option, copt.CustomOptionsId);
            
            CacheManager.RemoveByPattern(CustomOptionsByProductIdCachePrefix + copt.ProductId);
        }

        public static void DeleteCustomOption(int customOptionId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeleteCustomOption]", CommandType.StoredProcedure,
                new SqlParameter("@CustomOptionsId", customOptionId));
            
            CacheManager.RemoveByPattern(CustomOptionsByProductIdCachePrefix);
        }

        public static void DeleteCustomOptionByProduct(int productId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Catalog].[CustomOptions] where [ProductID] = @productId", CommandType.Text, new SqlParameter("@productId", productId));
            
            CacheManager.RemoveByPattern(CustomOptionsByProductIdCachePrefix + productId);
        }

        public static List<CustomOption> GetCustomOptionsByProductId(int productId)
        {
            var customOptions = SQLDataAccess.ExecuteReadList(
                    "SELECT [CustomOptions].*, [CurrencyValue] " +
                    "FROM [Catalog].[CustomOptions] " +
                    "INNER JOIN [Catalog].[Product] ON [Product].[ProductID] = [CustomOptions].[ProductID] " +
                    "INNER JOIN [Catalog].[Currency] ON [Product].[CurrencyID] = [Currency].[CurrencyID] " +
                    "WHERE [CustomOptions].[ProductID] = @ProductId " +
                    "ORDER BY [SortOrder]",
                    CommandType.Text,
                    GetCustomOptionFromReader, 
                    new SqlParameter("@ProductId", productId));

            foreach (var customOption in customOptions
                         .Where(customOption => customOption.Options.Count > 0 && (customOption.IsRequired
                             || customOption.Options.Any(optionItem => optionItem.DefaultQuantity > 0))))
            {
                if (customOption.InputType == CustomOptionInputType.ChoiceOfProduct
                    || customOption.InputType == CustomOptionInputType.MultiCheckBox)
                {
                    var selectedItems = customOption.Options.Where(optionItem => optionItem.DefaultQuantity > 0)
                        .ToList();

                    customOption.SelectedOptions = selectedItems.Count > 0
                        ? selectedItems
                        : new List<OptionItem> { customOption.Options.ElementAt(0) };
                }
                else
                {
                    var selectedItem =
                        customOption.Options.FirstOrDefault(optionItem => optionItem.DefaultQuantity > 0) ??
                        customOption.Options.ElementAt(0);

                    customOption.SelectedOptions = new List<OptionItem> { selectedItem };
                }
            }

            return customOptions;
        }

        public static ImmutableList<CustomOption> GetCustomOptionsByProductIdCached(int productId)
        {
            return CacheManager.Get(
                CustomOptionsByProductIdCachePrefix + productId, 
                () => GetCustomOptionsByProductId(productId).ToImmutableList());
        }

        public static bool DoesProductHaveCustomOptions(int productId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "if exists (SELECT 1 FROM [Catalog].[CustomOptions] WHERE ProductID = @ProductId) Select 1 Else Select 0", CommandType.Text,
                new SqlParameter("@ProductId", productId));
        }

        public static bool DoesProductHaveRequiredCustomOptions(int productId)
        {
            return 
                SQLDataAccess.ExecuteScalar<int>(
                    "[Catalog].[sp_GetCustomOptionsIsRequiredByProductId]",
                    CommandType.StoredProcedure, 
                    new SqlParameter("@ProductId", productId)) > 0;
        }


        // Получаем сумму кастом опций по цене продукта, и сериализованным опциям
        public static float GetCustomOptionPrice(float price, string attributeXml, float baseCurrencyValue)
        {
            if (string.IsNullOrEmpty(attributeXml))
                return 0;

            return GetCustomOptionPrice(price, DeserializeFromXml(attributeXml, baseCurrencyValue));
        }

        // Получаем сумму каcтом опций по цене продукта, и списку десериализованных кастом опций
        public static float GetCustomOptionPrice(float price, IEnumerable<EvaluatedCustomOptions> customOptions)
        {
            float fixedPrice = 0;
            float percentPrice = 0;

            if (customOptions != null)
            {
                foreach (var item in customOptions)
                {
                    switch (item.OptionPriceType)
                    {
                        case OptionPriceType.Fixed:
                            fixedPrice += item.OptionPriceBc * (item.OptionAmount ?? 1);
                            break;

                        case OptionPriceType.Percent:
                            percentPrice += price * item.OptionPriceBc * 0.01F * (item.OptionAmount ?? 1);
                            break;
                    }
                }
            }

            return (fixedPrice + percentPrice); // CurrencyService.CurrentCurrency.Rate 
        }

        #endregion

        #region  Public OptionItem Methods
        public static int AddOption(OptionItem opt, int customOptionsId)
        {
            opt.OptionId = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Catalog].[Options] ([CustomOptionsId],[Title],[PriceBC],[PriceType],[SortOrder]," +
                "[ProductId],[OfferId],[MinQuantity],[MaxQuantity],[DefaultQuantity], [Description] ) " +
                "VALUES (@CustomOptionsId,@Title,@PriceBC,@PriceType,@SortOrder," +
                "@ProductId,@OfferId,@MinQuantity,@MaxQuantity,@DefaultQuantity,@Description);" +
                "SELECT SCOPE_IDENTITY()", 
                CommandType.Text,
                new SqlParameter("@CustomOptionsId", customOptionsId),
                new SqlParameter("@Title", opt.Title),
                new SqlParameter("@PriceBC", opt.BasePrice),
                new SqlParameter("@PriceType", opt.PriceType),
                new SqlParameter("@SortOrder", opt.SortOrder),
                new SqlParameter("@ProductId", opt.ProductId ?? (object)DBNull.Value),
                new SqlParameter("@OfferId", opt.OfferId ?? (object)DBNull.Value),
                new SqlParameter("@MinQuantity", opt.MinQuantity ?? (object)DBNull.Value),
                new SqlParameter("@MaxQuantity", opt.MaxQuantity ?? (object)DBNull.Value),
                new SqlParameter("@DefaultQuantity", opt.DefaultQuantity ?? (object)DBNull.Value),
                new SqlParameter("@Description", opt.Description ?? string.Empty)
                );
            
            CacheManager.RemoveByPattern(CustomOptionsByProductIdCachePrefix);

            return opt.OptionId;
        }

        public static void DeleteOption(int optionId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeleteOption]", CommandType.StoredProcedure, new SqlParameter("@OptionID", optionId));
            
            CacheManager.RemoveByPattern(CustomOptionsByProductIdCachePrefix);
        }

        public static void UpdateOption(OptionItem opt, int customOptionsId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[Options] " +
                "SET [CustomOptionsId] = @CustomOptionsId" +
                ",[Title] = @Title" +
                ",[PriceBC] = @PriceBC" +
                ",[PriceType] = @PriceType" +
                ",[SortOrder] = @SortOrder" +
                ",[ProductId] = @ProductId" +
                ",[OfferId] = @OfferId " +
                ",[MinQuantity] = @MinQuantity " +
                ",[MaxQuantity] = @MaxQuantity " +
                ",[DefaultQuantity] = @DefaultQuantity " +
                ",[Description] = @Description " +
                "WHERE [OptionID] = @OptionID", 
                CommandType.Text,
                new SqlParameter("@OptionID", opt.OptionId),
                new SqlParameter("@CustomOptionsId", customOptionsId),
                new SqlParameter("@Title", opt.Title),
                new SqlParameter("@PriceBC", opt.BasePrice),
                new SqlParameter("@PriceType", opt.PriceType),
                new SqlParameter("@SortOrder", opt.SortOrder),
                new SqlParameter("@ProductId", opt.ProductId ?? (object)DBNull.Value),
                new SqlParameter("@OfferId", opt.OfferId ?? (object)DBNull.Value),
                new SqlParameter("@MinQuantity", opt.MinQuantity ?? (object)DBNull.Value),
                new SqlParameter("@MaxQuantity", opt.MaxQuantity ?? (object)DBNull.Value),
                new SqlParameter("@DefaultQuantity", opt.DefaultQuantity ?? (object)DBNull.Value),
                new SqlParameter("@Description", opt.Description ?? string.Empty)
                );
            
            CacheManager.RemoveByPattern(CustomOptionsByProductIdCachePrefix);
        }

        public static List<OptionItem> GetCustomOptionItems(int customOptionId)
        {
            return SQLDataAccess.ExecuteReadList<OptionItem>(
                "SELECT [Options].*,[CurrencyValue],[Photo].[PhotoName] " +
                "FROM [Catalog].[Options] " +
                "INNER JOIN [Catalog].[CustomOptions] ON [CustomOptions].[CustomOptionsId] = [Options].[CustomOptionsId] " +
                "INNER JOIN [Catalog].[Product] ON [Product].ProductId = [CustomOptions].ProductId " +
                "INNER JOIN [Catalog].[Currency] ON [Product].CurrencyID = [Currency].CurrencyID " +
                "LEFT JOIN [Catalog].[Photo] ON [Photo].[ObjId] = [Options].[OptionID] AND [Type]=@Type " +
                "WHERE [CustomOptions].CustomOptionsId = @CustomOptionId " +
                "ORDER BY [SortOrder]",
                CommandType.Text, 
                GetOptionItemFromReader,
                new SqlParameter("@CustomOptionId", customOptionId),
                new SqlParameter("@Type", PhotoType.CustomOptions.ToString()));
        }

        public static List<OptionItem> GetOptionsWithProductPhotoByOfferIds(List<int> offerIds)
        {
            return SQLDataAccess.ExecuteReadList<OptionItem>(
                "SELECT Offer.ProductID, Offer.OfferID, Photo.PhotoName, Product.Name as ProductName " +
                "FROM Catalog.Offer " +
                "LEFT JOIN Catalog.Photo on Photo.ObjId = Offer.ProductID AND Photo.Type = @type AND Photo.Main = 1 " +
                "INNER JOIN Catalog.Product ON Product.ProductID = Offer.ProductID " +
                $"WHERE Offer.OfferID in ({string.Join(",", offerIds)})",
                CommandType.Text,
                reader =>
                {
                    return new OptionItem
                    {
                        ProductPhoto = new ProductPhoto
                        {
                            PhotoName = SQLDataHelper.GetString(reader, "PhotoName"),
                        },
                        ProductId = SQLDataHelper.GetNullableInt(reader, "ProductID"),
                        OfferId = SQLDataHelper.GetNullableInt(reader, "OfferId"),
                        Title = SQLDataHelper.GetString(reader, "ProductName")
                    };
                },
                new SqlParameter("@type", PhotoType.Product.ToString()));
        }

        public static void SubmitCustomOptionsWithSameProductId(int productId, List<CustomOption> list)
        {
            var oldCustomOptions = GetCustomOptionsByProductId(productId);
            
            foreach (var customOption in oldCustomOptions.Where(x => list.WithId(x.CustomOptionsId) == null))
                DeleteCustomOption(customOption.CustomOptionsId);
            
            foreach (var customOption in list.Where(x => oldCustomOptions.WithId(x.CustomOptionsId) != null))
                UpdateCustomOption(customOption);

            foreach (var customOption in list.Where(x => x.CustomOptionsId <= 0))
                AddCustomOption(customOption);
        }

        public static List<EvaluatedCustomOptions> GetEvaluatedCustomOptions(List<CustomOption> customOptions, List<OptionItem> options)
        {
            var result = new List<EvaluatedCustomOptions>();

            for (var i = 0; i <= customOptions.Count - 1; i++)
            {
                var optItems = options.Where(x => x.CustomOptionsId == customOptions[i].CustomOptionsId).ToList();
                var evaluatedOptions = optItems.Select(x => new EvaluatedCustomOptions
                {
                    CustomOptionId = customOptions[i].CustomOptionsId,
                    CustomOptionTitle = customOptions[i].Title,
                    OptionId = x.OptionId,
                    OptionTitle = x.Title,
                    OptionPriceBc = x.BasePrice,
                    OptionPriceType = x.PriceType,
                    OptionAmount = x.DefaultQuantity
                }).ToList();

                result.AddRange(evaluatedOptions);
            }
            return result;
        }

        public static string SerializeToXml(List<EvaluatedCustomOptions> evcoList)
        {
            if (evcoList == null || evcoList.Count == 0)
                return string.Empty;


            var doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Options");
            foreach (var evco in evcoList)
            {
                XmlElement xopt = doc.CreateElement("Option");
                XmlElement xel = doc.CreateElement("CustomOptionId");
                xel.InnerText = evco.CustomOptionId.ToString(CultureInfo.InvariantCulture);
                xopt.AppendChild(xel);
                xel = doc.CreateElement("CustomOptionTitle");
                xel.InnerText = evco.CustomOptionTitle;
                xopt.AppendChild(xel);

                xel = doc.CreateElement("OptionId");
                xel.InnerText = evco.OptionId.ToString(CultureInfo.InvariantCulture);
                xopt.AppendChild(xel);

                xel = doc.CreateElement("OptionTitle");
                xel.InnerText = evco.OptionTitle;
                xopt.AppendChild(xel);

                xel = doc.CreateElement("OptionPriceBC");
                xel.InnerText = evco.OptionPriceBc.ToString(CultureInfo.InvariantCulture);
                xopt.AppendChild(xel);

                xel = doc.CreateElement("OptionPriceType");
                xel.InnerText = evco.OptionPriceType.ToString();
                xopt.AppendChild(xel);

                xel = doc.CreateElement("OptionAmount");
                xel.InnerText = evco.OptionAmount.ToString();
                xopt.AppendChild(xel);
                root.AppendChild(xopt);
            }
            doc.AppendChild(root);

            string str;
            using (var memstream = new MemoryStream())
            {

                var wrtr = new XmlTextWriter(memstream, null);
                doc.WriteTo(wrtr);
                wrtr.Close();
                byte[] buff = memstream.GetBuffer();
                int eidx = buff.Length - 1;
                for (int i = buff.Length - 1; i >= 0; i--)
                {
                    if (buff[i] == 0)
                        continue;
                    eidx = i;
                    break;
                }

                str = Encoding.UTF8.GetString(buff, 0, eidx + 1);
                memstream.Close();
            }

            return str;
        }

        /// <summary>
        /// Get custom options from string
        /// </summary>
        /// <param name="xml">Custom option xml</param>
        /// <param name="baseCurrencyValue">Product currency rate</param>
        /// <returns></returns>
        public static List<EvaluatedCustomOptions> DeserializeFromXml(string xml, float baseCurrencyValue, Currency renderCurrency = null)
        {
            if (String.IsNullOrWhiteSpace(xml))
                return null;

            try
            {
                var res = new List<EvaluatedCustomOptions>();

                var doc = new XmlDocument();
                doc.LoadXml(xml);

                foreach (XmlElement xel in doc.GetElementsByTagName("Option"))
                {
                    var xelm = xel.GetElementsByTagName("OptionId")[0];
                    if (int.Parse(xelm.InnerText) < 0)
                    {
                        continue;
                    }
                    var evco = new EvaluatedCustomOptions();
                    xelm = xel.GetElementsByTagName("CustomOptionId")[0];
                    evco.CustomOptionId = int.Parse(xelm.InnerText);

                    xelm = xel.GetElementsByTagName("OptionId")[0];
                    evco.OptionId = int.Parse(xelm.InnerText);

                    xelm = xel.GetElementsByTagName("OptionTitle")[0];
                    evco.OptionTitle = xelm.InnerText;

                    xelm = xel.GetElementsByTagName("CustomOptionTitle")[0];
                    evco.CustomOptionTitle = xelm.InnerText;

                    xelm = xel.GetElementsByTagName("OptionPriceType")[0];
                    evco.OptionPriceType = (OptionPriceType)Enum.Parse(typeof(OptionPriceType), xelm.InnerText);

                    xelm = xel.GetElementsByTagName("OptionPriceBC")[0];
                    var price = float.Parse(xelm.InnerText, CultureInfo.InvariantCulture);
                    evco.OptionPriceBc = evco.OptionPriceType == OptionPriceType.Percent
                        ? price
                        : price.RoundPrice(baseCurrencyValue, renderCurrency);

                    xelm = xel.GetElementsByTagName("OptionAmount")[0];
                    evco.OptionAmount = xelm.InnerText.TryParseFloat(true);

                    res.Add(evco);
                }

                return res;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }

        public static string GetJsonHash(List<EvaluatedCustomOptions> options)
        {
            return options != null && options.Count > 0
                ? Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(
                    options.Select(x => new
                    {
                        x.CustomOptionId,
                        x.CustomOptionTitle,
                        x.OptionId,
                        x.OptionTitle,
                        x.OptionPriceBc,
                        x.OptionPriceType,
                        x.OptionAmount
                    }).ToList())))
                : string.Empty;
        }

        public static List<EvaluatedCustomOptions> GetFromJsonHash(string hash, float baseCurrencyValue)
        {
            if (hash.IsNullOrEmpty())
                return null;
            try
            {
                var json = Encoding.UTF8.GetString(Convert.FromBase64String(hash));
                return DeserializeFromJson(json, baseCurrencyValue);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return null;
            }
        }

        public static string SerializeToJson(List<EvaluatedCustomOptions> evcoList)
        {
            if (evcoList == null || !evcoList.Any())
                return string.Empty;
            return JsonConvert.SerializeObject(evcoList);
        }

        public static List<EvaluatedCustomOptions> DeserializeFromJson(string json, float baseCurrencyValue)
        {
            try
            {
                var result = json.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject<List<EvaluatedCustomOptions>>(json);
                if (result == null || !result.Any())
                    return null;

                foreach (var evco in result)
                {
                    if (evco.OptionPriceType == OptionPriceType.Fixed)
                        evco.OptionPriceBc = evco.OptionPriceBc.RoundPrice(baseCurrencyValue);
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return null;
            }
        }

        public static bool IsValidCustomOptions(ShoppingCartItem item)
        {
            if (string.IsNullOrWhiteSpace(item.AttributesXml))
                return true;

            var selectedOptions =
                DeserializeFromXml(item.AttributesXml, item.Offer.Product.Currency.Rate);

            if (selectedOptions == null)
                return true;

            var productOptions = GetCustomOptionsByProductId(item.Offer.ProductId);

            foreach (var productOption in productOptions)
            {
                var selectedOption = selectedOptions
                    .FirstOrDefault(options => options.CustomOptionId == productOption.CustomOptionsId);

                if (selectedOption == null && productOption.IsRequired)
                    return false;

                if (selectedOption == null)
                    continue;

                if (productOption.Options == null ||
                    productOption.Options.All(optionItem => optionItem.OptionId != selectedOption.OptionId))
                    return false;

                foreach (var productOptionItem in productOption.Options)
                {
                    var selectedOptionItem = selectedOptions
                        .FirstOrDefault(options => options.CustomOptionId == productOption.CustomOptionsId
                                                   && options.OptionId == productOptionItem.OptionId);

                    if (selectedOptionItem == null)
                        continue;

                    if (selectedOptionItem.OptionAmount < productOptionItem.MinQuantity
                        || selectedOptionItem.OptionAmount > productOptionItem.MaxQuantity)
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region  Private Methods
        private static CustomOption GetCustomOptionFromReader(SqlDataReader reader)
        {
            return new CustomOption
            {
                CustomOptionsId = SQLDataHelper.GetInt(reader, "CustomOptionsID"),
                Title = SQLDataHelper.GetString(reader, "Title"),
                InputType = (CustomOptionInputType)SQLDataHelper.GetInt(reader, "InputType"),
                IsRequired = SQLDataHelper.GetBoolean(reader, "IsRequired"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                ProductId = SQLDataHelper.GetInt(reader, "ProductID"),
                MinQuantity = SQLDataHelper.GetNullableFloat(reader, "MinQuantity"),
                MaxQuantity = SQLDataHelper.GetNullableFloat(reader, "MaxQuantity"),
                Description = SQLDataHelper.GetString(reader, "Description")
            };
        }

        private static OptionItem GetOptionItemFromReader(SqlDataReader reader)
        {
            var pictureData = new OptionItemPhoto
            {
                PhotoName = SQLDataHelper.GetString(reader, "PhotoName"),
            };
            pictureData.PictureUrl = pictureData.ImageSrc();

            return new OptionItem()
            {
                OptionId = SQLDataHelper.GetInt(reader, "OptionID"),
                CustomOptionsId = SQLDataHelper.GetInt(reader, "CustomOptionsId"),
                Title = SQLDataHelper.GetString(reader, "Title"),
                BasePrice = SQLDataHelper.GetFloat(reader, "PriceBC"),
                CurrencyRate = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                PriceType = (OptionPriceType)SQLDataHelper.GetInt(reader, "PriceType"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                ProductId = SQLDataHelper.GetNullableInt(reader, "ProductId"),
                OfferId = SQLDataHelper.GetNullableInt(reader, "OfferId"),
                MinQuantity = SQLDataHelper.GetNullableFloat(reader, "MinQuantity"),
                MaxQuantity = SQLDataHelper.GetNullableFloat(reader, "MaxQuantity"),
                DefaultQuantity = SQLDataHelper.GetNullableFloat(reader, "DefaultQuantity"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                PictureData = pictureData
             };
        }
        #endregion

        public static string OptionsToString(List<OptionItem> options)
        {
            var res = new StringBuilder();
            for (var i = 0; i < options.Count; i++)
            {
                res.Append("{");
                res.Append(options[i].Title);
                res.Append("|");
                res.Append(options[i].BasePrice.ToString("F2"));
                res.Append("|");
                res.Append(options[i].PriceType);
                res.Append("|");
                res.Append(options[i].SortOrder);
                res.Append("|");
                res.Append(options[i].ProductId);
                res.Append("|");
                res.Append(options[i].OfferId);
                res.Append("|");
                res.Append(options[i].MinQuantity);
                res.Append("|");
                res.Append(options[i].MaxQuantity);
                res.Append("|");
                res.Append(options[i].DefaultQuantity);
                res.Append("}");
            }
            return res.ToString();
        }

        public static string CustomOptionsToString(List<CustomOption> customOptions)
        {   //[title|type|IsRequired|sortOrder|MinQuantity|MaxQuantity:{value|price|typePrice|sort|ProductId|OfferId|MinQuantity|MaxQuantity|DefaultQuantity}...] 
            //[Цвет1|drop|1|0:{Синий|100|f|10|0|2}{Красный|10|p|20|1234|15234|0|1|0}],....
            var res = new StringBuilder();
            for (var i = 0; i < customOptions.Count; i++)
            {
                res.Append("[");
                res.Append(customOptions[i].Title);
                res.Append("|");
                res.Append(customOptions[i].InputType);
                res.Append("|");
                res.Append(customOptions[i].IsRequired ? 1 : 0);
                res.Append("|");
                res.Append(customOptions[i].SortOrder);
                res.Append("|");
                res.Append(customOptions[i].MinQuantity);
                res.Append("|");
                res.Append(customOptions[i].MaxQuantity);
                res.Append(":");
                res.Append(OptionsToString(customOptions[i].Options));
                res.Append("]");
            }
            return res.ToString();
        }

        public static void CustomOptionsFromString(int productId, string customOptionString, bool isNewProduct)
        {
            try
            {
                if (!isNewProduct)
                    DeleteCustomOptionByProduct(productId);
                
                if (string.IsNullOrEmpty(customOptionString))
                    return;
                
                var items = customOptionString.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in items)
                {
                    var ind = item.LastIndexOf(":{"); // со значениями
                    var customOptionStr = ind != -1 ? item.Substring(0, ind) : item.TrimEnd(':');
                    var customOption = ParseCustomOption(customOptionStr, productId);
                    if (customOption == null)
                        continue;

                    var optionsStr = ind != -1 ? item.Substring(ind + 1) : null;
                    if (optionsStr.IsNotEmpty())
                        customOption.Options = ParseOption(optionsStr);
                    
                    AddCustomOption(customOption);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private static CustomOption ParseCustomOption(string source, int productId)
        {
            var parts = source.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4) return null;
            var co = new CustomOption
            {
                Title = parts[0],
                InputType = parts[1].TryParseEnum<CustomOptionInputType>(),
                IsRequired = parts[2] == "1",
                SortOrder = parts[3].TryParseInt(),
                ProductId = productId
            };

            if (parts.Length == 6)
            {
                co.MinQuantity = parts[4].TryParseFloat(true);
                co.MaxQuantity = parts[5].TryParseFloat(true);
            }

            return co;
        }
        private static List<OptionItem> ParseOption(string source)
        {
            var res = new List<OptionItem>();
            var items = source.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var parts in items.Select(item => item.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)))
            {
                if (parts.Length == 9)
                    res.Add(new OptionItem
                    {
                        Title = parts[0],
                        BasePrice = parts[1].TryParseFloat(),
                        PriceType = parts[2].TryParseEnum<OptionPriceType>(),
                        SortOrder = parts[3].TryParseInt(),
                        ProductId = parts[4].TryParseInt(true),
                        OfferId = parts[5].TryParseInt(true),
                        MinQuantity = parts[6].TryParseFloat(true),
                        MaxQuantity = parts[7].TryParseFloat(true),
                        DefaultQuantity = parts[8].TryParseFloat(true)
                    });
                if (parts.Length == 4)
                res.Add(new OptionItem
                    {
                        Title = parts[0],
                        BasePrice = parts[1].TryParseFloat(),
                        PriceType = parts[2].TryParseEnum<OptionPriceType>(),
                        SortOrder = parts[3].TryParseInt()
                    });
                if (parts.Length == 3)
                    res.Add(new OptionItem
                    {
                        Title = string.Empty,
                        BasePrice = parts[0].TryParseFloat(),
                        PriceType = parts[1].TryParseEnum<OptionPriceType>(),
                        SortOrder = parts[2].TryParseInt()
                    });
            }
            return res;
        }
        
        public static string RenderSelectedOptions(string xml, float baseCurrencyRate)
        {
            if (String.IsNullOrEmpty(xml))
                return String.Empty;

            var result = new StringBuilder("<div class=\"customoptions\">");

            foreach (var item in DeserializeFromXml(xml, baseCurrencyRate))
            {

                result.AppendFormat("<div class=\"customoption-item\">{0}: {1}", item.CustomOptionTitle, item.OptionTitle);
                if (item.OptionPriceBc != 0)
                {
                    result.Append(" ");
                    if (item.OptionPriceBc > 0)
                        result.Append("+" +
                                      (item.OptionPriceType == OptionPriceType.Fixed
                                          ? PriceFormatService.FormatPrice(item.OptionPriceBc)
                                          : item.OptionPriceBc + " %"));
                }
                result.Append("</div>");
            }

            result.Append("</div>");

            return result.ToString();
        }

        public static string RenderSelectedOptions(List<EvaluatedCustomOptions> customOptions)
        {
            if (customOptions == null)
                return String.Empty;

            var result = new StringBuilder("<div class=\"customoptions\">");

            foreach (var option in customOptions)
            {
                result.AppendFormat("<div class=\"customoption-item\">{0}: {1}", option.CustomOptionTitle, option.OptionTitle);
                if (option.OptionPriceBc != 0)
                {
                    result.Append(" ");
                    if (option.OptionPriceBc > 0)
                        result.Append("+" +
                                      (option.OptionPriceType == OptionPriceType.Fixed
                                          ? PriceFormatService.FormatPrice(option.OptionPriceBc)
                                          : option.OptionPriceBc + " %"));
                }
                result.Append("</div>");
            }

            result.Append("</div>");

            return result.ToString();
        }

    }
}