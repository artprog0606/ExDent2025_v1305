using AdvantShop.Core.Common.Extensions;
using AdvantShop.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Services.Crm.Ok.OkMarket
{
    public class OkMarketGroup
    {
        [JsonProperty("groupId")]
        public string GroupId { get; set; }
        [JsonProperty("uid")]
        public string UID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("business")]
        public bool? Business { get; set; }
        [JsonProperty("attrs")]
        public OkMarketGroupAttributes Attributes { get; set; }
    }

    public class OkMarketGroupAttributes
    {
        [JsonProperty("flags")]
        public string Flags { get; set; }
    }

    public class OkMarketCatalog
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("Id")]
        public long OkCatalogId { get; set; }
        [JsonIgnore]
        public List<int> CategoryIds { get; set; }
    }

    #region Product

    public class OkMarketProduct
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
        [JsonProperty("media", NullValueHandling = NullValueHandling.Ignore)]
        public List<OkMarketProductMedia> Media { get; set; }

        public OkMarketProduct() { }
        //создание модели продукта на экспорт:
        public OkMarketProduct(OkMarketProductModel model, List<string> newPhotos = null, bool businessGroup = false)
        {
            var productPhotos = new List<OkMarketProductPhoto>();
            if (model.OkPhotoIdsList != null)
            {
                foreach (var token in model.OkPhotoIdsList)
                {
                    productPhotos.Add(new OkMarketProductPhoto()
                    {
                        Group = true,
                        ExistingPhotoId = token
                    });
                }
            }
            if (newPhotos != null)
            {
                foreach (var token in newPhotos)
                {
                    productPhotos.Add(new OkMarketProductPhoto()
                    {
                        Id = token
                    });
                }
            }

            Media = new List<OkMarketProductMedia>()
            {
                new OkMarketProductMedia()
                {
                    Type = OkMarketProductMediaType.text.ToString(),
                    Text = model.Name
                }
            };
            if (!string.IsNullOrEmpty(model.Description))
            {
                Media.Add(new OkMarketProductMedia()
                {
                    Type = OkMarketProductMediaType.text.ToString(),
                    Text = StringHelper.RemoveHTML(model.Description)
                });
            }
            if (productPhotos.Count > 0)
            {
                Media.Add(new OkMarketProductMedia()
                {
                    Type = OkMarketProductMediaType.photo.ToString(),
                    List = productPhotos
                });
            }
            Media.Add(new OkMarketProductMedia()
            {
                Type = OkMarketProductMediaType.product.ToString(),
                Price = model.Price,
                Currency = model.CurrencyName,
                Lifetime = businessGroup ? 0 : 30,
            });
        }
    }

    public class OkMarketProductModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string UrlPath { get; set; }
        public string Description { get; set; }
        public string BriefDescription { get; set; }
        public float Discount { get; set; }
        public float DiscountAmount { get; set; }
        public bool Enabled { get; set; }
        public float? MinAmount { get; set; }
        public string Unit { get; set; }

        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public float Price { get; set; }
        public bool Main { get; set; }
        public float Amount { get; set; }

        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public float CurrencyValue { get; set; }
        public string CurrencyName { get; set; }

        public long OkProductId { get; set; }
        public IEnumerable<string> OkPhotoIdsList { get; set; }
        public string OkPhotoIds
        {
            get { return OkPhotoIdsList != null ? string.Join(",", OkPhotoIdsList) : ""; }
            set { OkPhotoIdsList = string.IsNullOrEmpty(value) ? null : value.Split(','); }
        }
        public long OkCatalogId { get; set; }

        public OkMarketProductModel() { }

        public OkMarketProductModel(OkMarketProduct product)
        {
            this.OkProductId = product.Id.TryParseLong();
            foreach (var media in product.Media)
            {
                switch (media.Type)
                {
                    case "product":
                        this.Name = media.ProductTitle;
                        this.Price = media.Price ?? 0;
                        this.CurrencyName = media.Currency;
                        this.Enabled = media.Status.TryParseEnum<OkMarketProductStatus>() == OkMarketProductStatus.ACTIVE;
                        break;
                    case "text":
                        this.Description = media.Text;
                        break;
                    case "photo":
                        this.OkPhotoIdsList = media.PhotoIds.Select(x => x.Replace("group_photo:", ""));
                        break;
                    case "catalog":
                        var catalogs = media.Catalog.Select(x => x.Replace("catalog:", ""));
                        if (catalogs != null)
                            this.OkCatalogId = catalogs.FirstOrDefault().TryParseLong();
                        else
                            this.OkCatalogId = 0;
                        break;
                }
            }
        }
    }

    public class OkMarketProductMedia
    {
        [JsonProperty("photo_refs", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> PhotoIds { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("list", NullValueHandling = NullValueHandling.Ignore)]
        public List<OkMarketProductPhoto> List { get; set; }

        [JsonProperty("price", NullValueHandling = NullValueHandling.Ignore)]
        public float? Price { get; set; }

        [JsonProperty("currency", NullValueHandling = NullValueHandling.Ignore)]
        public string Currency { get; set; }

        [JsonProperty("lifetime", NullValueHandling = NullValueHandling.Ignore)]
        public int? Lifetime { get; set; }

        [JsonProperty("product_title", NullValueHandling = NullValueHandling.Ignore)]
        public string ProductTitle { get; set; }

        [JsonProperty("product_status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("product_ccy", NullValueHandling = NullValueHandling.Ignore)]
        public string ProductCurrency
        {
            set { this.Currency = value; }
        }

        [JsonProperty("product_price_number", NullValueHandling = NullValueHandling.Ignore)]
        public float? ProductPrice
        {
            set { this.Price = value; }
        }

        [JsonProperty("catalog_refs", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Catalog { get; set; }
    }

    public class OkMarketProductPhoto
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
        [JsonProperty("existing_photo_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ExistingPhotoId { get; set; }
        [JsonProperty("group", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Group { get; set; }
    }

    #endregion

    #region User
    public class OkMarketUser
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("pic128x128")]
        public string Photo { get; set; }
    }
    #endregion

    public class OkMarketPhoto
    {
        [JsonProperty("pic640x480")]
        public string PhotoUrl { get; set; }
    }

    public class OkMarketPhotoToken
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }


    public enum OkMarketProductStatus
    {
        ACTIVE,
        AUTO_CLOSED,
        CLOSED,
        OUT_OF_STOCK,
        SOLD,
    }

    public enum OkMarketProductMediaType
    {
        [JsonProperty(propertyName: "text")]
        text,
        [JsonProperty(propertyName: "photo")]
        photo,
        [JsonProperty(propertyName: "product")]
        product
    }
}