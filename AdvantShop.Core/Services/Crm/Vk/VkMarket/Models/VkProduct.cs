using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Export;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Models
{
    public class VkProduct
    {
        /// <summary>
        /// Идентификатор товара ВКонтакте.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// ProductId in store
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// OfferId in store
        /// </summary>
        public int OfferId { get; set; }

        public long AlbumId { get; set; }

        /// <summary>
        /// Идентификатор владельца товара. Обратите внимание, идентификатор сообщества в параметре owner_id необходимо указывать со знаком "-" — например, owner_id=-1 соответствует идентификатору сообщества ВКонтакте API (club1)  целое число, обязательный параметр (целое число, обязательный параметр).
        /// </summary>
        public long OwnerId { get; set; }
        
        /// <summary>
        /// Название товара. строка, минимальная длина 4, максимальная длина 100, обязательный параметр (строка, минимальная длина 4, максимальная длина 100, обязательный параметр).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание товара. строка, минимальная длина 10, обязательный параметр (строка, минимальная длина 10, обязательный параметр).
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Идентификатор категории товара. положительное число, обязательный параметр (положительное число, обязательный параметр).
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// Цена товара. дробное число, обязательный параметр, минимальное значение 0.01 (дробное число, обязательный параметр, минимальное значение 0.01).
        /// </summary>
        public decimal Price { get; set; }

        public decimal? OldPrice { get; set; }

        /// <summary>
        /// Статус товара (1 — товар удален, 0 — товар не удален). флаг, может принимать значения 1 или 0 (флаг, может принимать значения 1 или 0).
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Идентификатор фотографии обложки товара. положительное число, обязательный параметр (положительное число, обязательный параметр).
        /// </summary>
        public long MainPhotoId { get; set; }

        /// <summary>
        /// Идентификаторы дополнительных фотографий товара. список положительных чисел, разделенных запятыми, количество элементов должно составлять не более 4 (список положительных чисел, разделенных запятыми, количество элементов должно составлять не более 4).
        /// </summary>
        public IEnumerable<long> PhotoIdsList { get; set; }

        public string PhotoIds
        {
            get => PhotoIdsList != null ? String.Join(",", PhotoIdsList) : "";
            set
            {
                PhotoIdsList = value?.Split(',').Select(x => x.TryParseLong()).Where(x => x != 0);
            }
        }

        /// <summary>
        /// Json соответствия картинки в ВКонтакте и в магазине
        /// </summary>
        public string PhotosMapIds { get; set; }


        public List<VkPhotoMap> PhotosMap
        {
            get
            {
                if (string.IsNullOrEmpty(PhotosMapIds))
                    return null;

                try
                {
                    return JsonConvert.DeserializeObject<List<VkPhotoMap>>(PhotosMapIds);
                }
                catch {}
                return null;
            }
        }


        public float Widht { get; set; }
        public float Height { get; set; }
        public float Length { get; set; }

        public float Weight { get; set; }

        public string Sku { get; set; }


        public string VariantIds { get; set; }
        public bool? IsMainVariant { get; set; }
        public long? ItemGroupId { get; set; }
        //public string Url { get; set; }
        
        public float? Amount { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = obj as VkProduct;
            if (other == null)
                return false;

            return Equals(other);
        }
        protected bool Equals(VkProduct other)
        {
            return Id == other.Id && ProductId == other.ProductId && OfferId == other.OfferId &&
                   AlbumId == other.AlbumId && OwnerId == other.OwnerId && Name == other.Name &&
                   Description == other.Description && CategoryId == other.CategoryId && 
                   Price == other.Price && OldPrice == other.OldPrice && Deleted == other.Deleted && 
                   (PhotoIdsList == null && other.PhotoIdsList == null || 
                    (PhotoIdsList != null && other.PhotoIdsList != null && PhotoIdsList.SequenceEqual(other.PhotoIdsList))) && 
                   MainPhotoId == other.MainPhotoId && PhotosMapIds == other.PhotosMapIds &&
                   Widht.Equals(other.Widht) && Height.Equals(other.Height) && Length.Equals(other.Length) &&
                   Weight.Equals(other.Weight) && Sku == other.Sku && VariantIds == other.VariantIds &&
                   IsMainVariant == other.IsMainVariant && ItemGroupId == other.ItemGroupId && 
                   //Url == other.Url && 
                   Amount.Equals(other.Amount);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ ProductId;
                hashCode = (hashCode * 397) ^ OfferId;
                hashCode = (hashCode * 397) ^ AlbumId.GetHashCode();
                hashCode = (hashCode * 397) ^ OwnerId.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CategoryId.GetHashCode();
                hashCode = (hashCode * 397) ^ Price.GetHashCode();
                hashCode = (hashCode * 397) ^ OldPrice.GetHashCode();
                hashCode = (hashCode * 397) ^ Deleted.GetHashCode();
                hashCode = (hashCode * 397) ^ MainPhotoId.GetHashCode();
                hashCode = (hashCode * 397) ^ (PhotoIdsList != null ? PhotoIdsList.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PhotosMapIds != null ? PhotosMapIds.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Widht.GetHashCode();
                hashCode = (hashCode * 397) ^ Height.GetHashCode();
                hashCode = (hashCode * 397) ^ Length.GetHashCode();
                hashCode = (hashCode * 397) ^ Weight.GetHashCode();
                hashCode = (hashCode * 397) ^ (Sku != null ? Sku.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (VariantIds != null ? VariantIds.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsMainVariant.GetHashCode();
                hashCode = (hashCode * 397) ^ ItemGroupId.GetHashCode();
                //hashCode = (hashCode * 397) ^ (Url != null ? Url.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Amount != null ? Amount.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public class VkApiProduct
    {
        public long item_id { get; set; }
        public long owner_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public long category_id { get; set; }
        public decimal price { get; set; }
        public decimal? old_price { get; set; }
        public long main_photo_id { get; set; }
        public IEnumerable<long> photo_ids { get; set; }
        public float dimension_width { get; set; }
        public float dimension_height { get; set; }
        public float dimension_length { get; set; }
        public float weight { get; set; }
        public string sku { get; set; }
        public string variant_ids { get; set; }
        public bool? is_main_variant { get; set; }
        //public string url { get; set; }
        public float? amount { get; set; }

        public VkApiProduct(VkProduct product)
        {
            item_id = product.Id;
            owner_id = product.OwnerId;
            name = product.Name;
            description = product.Description;
            category_id = product.CategoryId;
            price = product.Price;
            old_price = product.OldPrice;
            main_photo_id = product.MainPhotoId;
            photo_ids = product.PhotoIdsList;
            dimension_width = product.Widht;
            dimension_height = product.Height;
            dimension_length = product.Length;
            weight = product.Weight * 1000; // г
            sku = product.Sku;
            variant_ids = product.VariantIds;
            is_main_variant = product.IsMainVariant;
            //url = product.Url;
            amount = product.Amount;
        }

        public override string ToString()
        {
            return string.Format(
                "owner_id={0}&name={1}&description={2}&category_id={3}&price={4}&main_photo_id={5}&photo_ids={6}&dimension_width={7}&dimension_height={8}&dimension_length={9}&weight={10}{11}&sku={12}{13}{14}{15}{16}",
                owner_id, HttpUtility.UrlEncode(name), HttpUtility.UrlEncode(description), category_id, price.ToString().Replace(",", "."),
                main_photo_id,
                photo_ids != null && photo_ids.Any() ? String.Join(",", photo_ids) : "",
                dimension_width.ToInvariantString(), dimension_height.ToInvariantString(), dimension_length.ToInvariantString(),
                weight.ToInvariantString(),
                item_id != 0 ? "&item_id=" + item_id : "",
                sku,
                old_price != null && old_price > 0 ? "&old_price=" + old_price.ToString().Replace(",", ".") : "",
                !string.IsNullOrEmpty(variant_ids) ? "&variant_ids=" + variant_ids : "",
                is_main_variant != null ? "&is_main_variant=" + is_main_variant.Value.ToLowerString() : "",
                amount != null ? "&stock_amount=" + amount.ToInvariantString() : null);
        }
    }

    public class VkApiProductEdit : VkApiProduct
    {
        public VkApiProductEdit(VkProduct product) : base(product)
        {
        }

        public override string ToString()
        {
            return string.Format(
                "owner_id={0}&name={1}&description={2}&category_id={3}&price={4}&dimension_width={5}&dimension_height={6}&dimension_length={7}&weight={8}{9}&sku={10}&photo_ids={11}&old_price={12}{13}{14}&main_photo_id={15}{16}",
                owner_id, HttpUtility.UrlEncode(name), HttpUtility.UrlEncode(description), category_id, price.ToString().Replace(",", "."),
                dimension_width.ToInvariantString(), dimension_height.ToInvariantString(), dimension_length.ToInvariantString(),
                weight.ToInvariantString(),
                item_id != 0 ? "&item_id=" + item_id : "",
                sku,
                photo_ids != null && photo_ids.Any() ? String.Join(",", photo_ids) : "",
                old_price != null && old_price > 0 ? old_price.ToString().Replace(",", ".") : price.ToString().Replace(",", "."),
                !string.IsNullOrEmpty(variant_ids) ? "&variant_ids=" + variant_ids : "",
                is_main_variant != null ? "&is_main_variant=" + is_main_variant.Value.ToLowerString() : "",
                main_photo_id,
                amount != null ? "&stock_amount=" + amount.ToInvariantString() : null);
        }
    }

    public class VkApiProductAddResult : IVkError
    {
        public VkApiProductAddResponse Response { get; set; }
        public VkApiError Error { get; set; }
    }

    public class VkApiProductAddResponse
    {
        public long market_item_id { get; set; }
    }

    public class VkApiProductUpdateResult : IVkError
    {
        public int Response { get; set; }
        public VkApiError Error { get; set; }
    }

    public class VkApiError
    {
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; }
    }

    public class VkProductSimple
    {
        public long Id { get; set; }
        public int ProductId { get; set; }
        public List<long> VariantIds { get; set; }
        public long? ItemGroupId { get; set; }

        public bool IsGrouped { get; set; }

        public VkProductSimple(VkProduct p)
        {
            Id = p.Id;
            ProductId = p.ProductId;
            VariantIds = !string.IsNullOrEmpty(p.VariantIds) 
                            ? p.VariantIds.Split(',').Select(x => x.TryParseLong()).ToList() 
                            : new List<long>();
            ItemGroupId = p.ItemGroupId;
        }
    }
}
