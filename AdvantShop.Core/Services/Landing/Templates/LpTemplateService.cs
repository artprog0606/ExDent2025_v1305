using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using AdvantShop.App.Landing.Domain;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Landing.Templates
{
    public class LpTemplateService
    {
        private const string CacheKey = "LpTemplates";
        private readonly LpTemplateHelperService _lpTemplateHelperService;
        private readonly LpService _lpService;

        public LpTemplateService()
        {
            _lpTemplateHelperService = new LpTemplateHelperService();
            _lpService = new LpService();
        }

        #region Prepare template config

        public string PrepareContent(string currentText, string text, LpConfiguration configuration)
        {
            var result = new StringBuilder(text);

            result = result
                .Replace("#BUTTON_ACTION#", ButtonAction(configuration.Type, configuration.Template))
                .Replace("#PHONE#", !string.IsNullOrEmpty(SettingsMain.Phone) ? Encode(SettingsMain.Phone) : "+7 (800) 333-68-03")
                .Replace("#EMAIL#", !string.IsNullOrEmpty(SettingsMail.EmailForFeedback) ? SettingsMail.EmailForFeedback : "info@mysite.ru")
                .Replace("#STORE_NAME#", Encode(SettingsMain.ShopName))
                .Replace("#STORE_URL#", SettingsMain.SiteUrlPlain)
                .Replace("#YEAR#", DateTime.Now.Year.ToString());
                       
            PreparePartWithProduct(ref result, text, configuration);

            if (text.Contains("#CATEGORY_FOR_PRODUCTSVIEW_NAME_") || text.Contains("#PRODUCT_IDS_FOR_PRODUCTSVIEW"))
            {
                var categories = _lpTemplateHelperService.GetCategoriesForProductView();
                if (categories.Count > 0)
                {
                    var ids = _lpTemplateHelperService.GetProductIdsForProductView(6, categories[0].CategoryId);

                    result =
                        result
                            .Replace("#CATEGORY_FOR_PRODUCTSVIEW_NAME_1#", categories[0].Name)
                            .Replace("\"#PRODUCT_IDS_FOR_PRODUCTSVIEW_1#\"", "[" + String.Join(", ", ids) + "]");

                    if (categories.Count > 1)
                    {
                        ids = _lpTemplateHelperService.GetProductIdsForProductView(6, categories[1].CategoryId);

                        result =
                            result
                                .Replace("#CATEGORY_FOR_PRODUCTSVIEW_NAME_2#", categories[1].Name)
                                .Replace("\"#PRODUCT_IDS_FOR_PRODUCTSVIEW_2#\"", "[" + String.Join(", ", ids) + "]");
                    }
                }
                else
                {
                    result =
                        result
                            .Replace("#CATEGORY_FOR_PRODUCTSVIEW_NAME_1#", "Новые коллекции")
                            .Replace("\"#PRODUCT_IDS_FOR_PRODUCTSVIEW_1#\"", "[]")
                            .Replace("#CATEGORY_FOR_PRODUCTSVIEW_NAME_2#", "Новые коллекции")
                            .Replace("\"#PRODUCT_IDS_FOR_PRODUCTSVIEW_2#\"", "[]");
                }
            }

            if (text.Contains("#PRODUCT_IDS_3#"))
            {
                var categories = _lpTemplateHelperService.GetCategoriesForProductView();
                if (categories.Count > 0)
                {
                    var ids = _lpTemplateHelperService.GetProductIdsForProductView(3, categories[0].CategoryId);

                    result = result.Replace("\"#PRODUCT_IDS_3#\"", "[" + String.Join(", ", ids) + "]");
                }
                else
                {
                    result = result.Replace("\"#PRODUCT_IDS_3#\"", "[]");
                }
            }

            if (text.Contains("#PRODUCT_IDS_ARRAY#"))
            {
                result = result.Replace("\"#PRODUCT_IDS_ARRAY#\"", "[" + String.Join(", ", configuration.ProductIds ?? new List<int>()) + "]");
            }

            if (text.Contains("#LINK_TO_PAGE#"))
            {
                result = result.Replace("#LINK_TO_PAGE#", configuration.PostActionUrl ?? "");
            }

            if (text.Contains("#CATEGORIES_IN_ProductsByCategories#"))
            {
                if (configuration.CategoryIds == null || configuration.CategoryIds.Count == 0)
                {
                    result = result.Replace("\"#CATEGORIES_IN_ProductsByCategories#\"", "[]");
                }
                else
                {
                    var categories = new List<Category>();
                    foreach (var categoryId in configuration.CategoryIds)
                    {
                        var c = CategoryService.GetCategory(categoryId);
                        if (c != null)
                            categories.Add(c);
                    }

                    result = result.Replace("\"#CATEGORIES_IN_ProductsByCategories#\"",
                        "[" +
                        string.Join(", ", categories.Select(x => string.Format("{{ \"CategoryId\": {0}, \"Name\": \"{1}\", \"Enabled\": {2} }}",
                                                                x.CategoryId, x.Name, x.Enabled.ToLowerString()))) +
                        "]");
                }
            }

            return result.ToString();
        }


        private void PreparePartWithProduct(ref StringBuilder result, string text, LpConfiguration configuration)
        {
            var product = configuration.Product;

            if (product == null)
                return;

            result =
                result
                    .Replace("#PRODUCT_NAME#", StringHelper.RemoveHTML(product.Name))
                    .Replace("#PRODUCT_NAME_RQ#", StringHelper.RemoveHTML(product.Name).Replace("\"", "\\\""))
                    .Replace("#PRODUCT_SHORT_DESCRIPTION#",
                        !string.IsNullOrEmpty(product.BriefDescription)
                            ? product.BriefDescription
                            : "<p>Напишите самые основные преимущества вашего товара.&nbsp;<br /><br /><em>&nbsp;- Почему у вас должны его купить?&nbsp;<br />&nbsp;-&nbsp;Что особенного в вашем товаре?&nbsp;<br />&nbsp;- Какую проблему он решает?</em></p>")
                    .Replace("#PRODUCT_SHORT_DESCRIPTION_2#",
                        !string.IsNullOrEmpty(product.BriefDescription)
                            ? product.BriefDescription
                            : "<p>Напишите самые основные преимущества вашего товара.</p>")
                    .Replace("#PRODUCT_SHORT_DESCRIPTION_3#",
                        !string.IsNullOrEmpty(product.BriefDescription)
                            ? product.BriefDescription
                            : "21-скоростная трансмиссия Shimano позволяет выбрать оптимальную нагрузку в зависимости от рельефа.&nbsp;Дисковая механическая тормозная система для безопасного торможения в любую погоду.")
                    .Replace("#PRODUCT_DESCRIPTION#",
                        !string.IsNullOrEmpty(product.Description)
                            ? product.Description
                            : "<p>Опишите ваш товар более подробно и не забудьте указать все преимущества для клиента</p><p><em>- габариты</em></p><p><em>- способы использования</em></p><p><em>- дополнительные критерии</em></p>");
                    
            if (text.Contains("#PRODUCT_PRICE#") || text.Contains("#PRODUCT_OLD_PRICE#") || 
                text.Contains("#PRODUCT_PRICE_PLAIN#") || text.Contains("#PRODUCT_OLD_PRICE_PLAIN#"))
            {
                var preparedPrice = "";
                var preparedOldPrice = "";
                var finalPrice = 0f;
                var oldPrice = 0f;

                if (configuration.ProductOffer != null)
                {
                    var price = configuration.ProductOffer.RoundedPrice.RoundPrice(CurrencyService.CurrentCurrency.Rate);
                    var finalDiscount =
                        PriceService.GetFinalDiscount(price, product.Discount, product.Currency.Rate,
                                                        CustomerContext.CurrentCustomer.CustomerGroup, product.ProductId,
                                                        doNotApplyOtherDiscounts: product.DoNotApplyOtherDiscounts,
                                                        productMainCategoryId: product.CategoryId);
                    finalPrice = PriceService.GetFinalPrice(price, finalDiscount);
                    oldPrice = price;

                    preparedPrice = PriceFormatService.FormatPrice(finalPrice);
                    if (price != finalPrice)
                        preparedOldPrice = PriceFormatService.FormatPrice(price);
                }

                result = !string.IsNullOrEmpty(preparedPrice)
                    ? result.Replace("#PRODUCT_PRICE#", "<div class=\"lp-h1 lp-h1--color lp-price\">" + preparedPrice + "</div> ")
                    : result.Replace("#PRODUCT_PRICE#", "");

                result = !string.IsNullOrEmpty(preparedOldPrice) && preparedPrice != preparedOldPrice
                    ? result.Replace("#PRODUCT_OLD_PRICE#", "<div class=\"lp-old-price\">" + preparedOldPrice + "</div> ")
                    : result.Replace("#PRODUCT_OLD_PRICE#", "");

                result = result.Replace("#PRODUCT_PRICE_PLAIN#", finalPrice.ToString("# ###"));
                result = result.Replace("#PRODUCT_OLD_PRICE_PLAIN#", oldPrice.ToString("# ###"));
            }

            if (text.Contains("#PRODUCT_DISCOUNT#") || text.Contains("#PRODUCT_DISCOUNT_OR_DEFAULT#"))
            {
                if (product.Discount.HasValue)
                {
                    var discountText = product.Discount.Type == DiscountType.Percent
                        ? "Скидка " + product.Discount.Percent + "%"
                        : "Скидка " + product.Discount.Amount + " Р";

                    result = result.Replace("#PRODUCT_DISCOUNT#", discountText).Replace("#PRODUCT_DISCOUNT_OR_DEFAULT#", discountText);
                }
                else
                {
                    result = result.Replace("#PRODUCT_DISCOUNT#", " ").Replace("#PRODUCT_DISCOUNT_OR_DEFAULT#", "Скидка 20%");
                }
            }

            if (text.Contains("#ACTION_OFFER_ID#"))
            {
                result = result.Replace("\"#ACTION_OFFER_ID#\"",
                    configuration.ProductOffer != null ? configuration.ProductOffer.OfferId.ToString() : "null");
            }

            if (text.Contains("#ACTION_OFFER_IDS#"))
            {
                result = result.Replace("\"#ACTION_OFFER_IDS#\"",
                    configuration.ProductOffer != null ? "[{ \"offerId\": \"" + configuration.ProductOffer.OfferId + "\" }]" : "null");
            }
            else if (text.Contains("#ACTION_OFFER_IDS_0#"))
            {
                result = result.Replace("\"#ACTION_OFFER_IDS_0#\"",
                    configuration.ProductOffer != null ? "[{ \"offerId\": \"" + configuration.ProductOffer.OfferId + "\", \"offerPrice\": \"0\" }]" : "null");
            }

            if (text.Contains("#PRODUCT_MAIN_PHOTO#"))
            {
                var photoPath = "";

                ProductPhoto photo = null;

                photo =
                    configuration.ProductOffer != null && configuration.ProductOffer.Photo != null
                        ? configuration.ProductOffer.Photo
                        : product.ProductPhotos.FirstOrDefault(x => x.Main);

                if (photo != null)
                    photoPath = photo.ImageSrcRelative(ProductImageType.Middle);

                result = result.Replace("#PRODUCT_MAIN_PHOTO#", photoPath);
            }

            if (text.Contains("#PRODUCT_MAIN_PHOTO_BIG#"))
            {
                var photoPath = "";

                ProductPhoto photo = null;

                photo =
                    configuration.ProductOffer != null && configuration.ProductOffer.Photo != null
                        ? configuration.ProductOffer.Photo
                        : product.ProductPhotos.FirstOrDefault(x => x.Main);

                if (photo != null)
                    photoPath = photo.ImageSrcRelative(ProductImageType.Big);

                result = result.Replace("#PRODUCT_MAIN_PHOTO_BIG#", photoPath);
            }

            if (text.Contains("#PRODUCT_PHOTO_"))
            {
                var photos = product.ProductPhotos.Where(x => !x.Main).ToList();

                result = result.Replace("#PRODUCT_PHOTO_1#", photos.Count > 0 ? photos[0].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/01-img.jpg");
                result = result.Replace("#PRODUCT_PHOTO_2#", photos.Count > 1 ? photos[1].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/02-img.jpg");
                result = result.Replace("#PRODUCT_PHOTO_3#", photos.Count > 2 ? photos[2].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/03-img.jpg");
                result = result.Replace("#PRODUCT_PHOTO_4#", photos.Count > 3 ? photos[3].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/04-img.jpg");
                result = result.Replace("#PRODUCT_PHOTO_5#", photos.Count > 4 ? photos[4].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/05-img.jpg");
                result = result.Replace("#PRODUCT_PHOTO_6#", photos.Count > 5 ? photos[5].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/06-img.jpg");
            }

            if (text.Contains("#PRODUCT_PHOTO_OR_NOPHOTO_"))
            {
                var photos = product.ProductPhotos.Where(x => !x.Main).ToList();

                result = result.Replace("#PRODUCT_PHOTO_OR_NOPHOTO_1#", photos.Count > 0 ? photos[0].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/nophoto.png");
                result = result.Replace("#PRODUCT_PHOTO_OR_NOPHOTO_2#", photos.Count > 1 ? photos[1].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/nophoto.png");
                result = result.Replace("#PRODUCT_PHOTO_OR_NOPHOTO_3#", photos.Count > 2 ? photos[2].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/nophoto.png");
                result = result.Replace("#PRODUCT_PHOTO_OR_NOPHOTO_4#", photos.Count > 3 ? photos[3].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/nophoto.png");
                result = result.Replace("#PRODUCT_PHOTO_OR_NOPHOTO_5#", photos.Count > 4 ? photos[4].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/nophoto.png");
                result = result.Replace("#PRODUCT_PHOTO_OR_NOPHOTO_6#", photos.Count > 5 ? photos[5].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/nophoto.png");
            }

            if (text.Contains("#PRODUCT_SCHEME_PHOTO_"))
            {
                var photos = product.ProductPhotos.ToList();

                result = result.Replace("#PRODUCT_SCHEME_PHOTO_1#", photos.Count > 0 ? photos[0].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/productsView/img.jpg");
                result = result.Replace("#PRODUCT_SCHEME_PHOTO_2#", photos.Count > 1 ? photos[1].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/productsView/img.jpg");
                result = result.Replace("#PRODUCT_SCHEME_PHOTO_3#", photos.Count > 2 ? photos[2].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/productsView/img.jpg");
                result = result.Replace("#PRODUCT_SCHEME_PHOTO_4#", photos.Count > 3 ? photos[3].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/productsView/img.jpg");
                result = result.Replace("#PRODUCT_SCHEME_PHOTO_5#", photos.Count > 4 ? photos[4].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/productsView/img.jpg");
                result = result.Replace("#PRODUCT_SCHEME_PHOTO_6#", photos.Count > 5 ? photos[5].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/productsView/img.jpg");
            }

            if (text.Contains("#PRODUCT_SCHEME_PHOTOS#"))
            {
                var photos = product.ProductPhotos.ToList();

                if (photos.Count == 0)
                {
                    var placeholder = new List<string>(3)
                    {
                        "areas/landing/images/productsView/img.jpg",
                        "areas/landing/images/productsView/img.jpg",
                        "areas/landing/images/productsView/img.jpg"
                    };

                    result = result.Replace("\"#PRODUCT_SCHEME_PHOTOS#\"",
                                                "[ " + string.Join(", ", placeholder.Select(x => string.Format("{{\"picture\": {{\"src\": \"{0}\", \"preview\": \"{0}\"}}}}", x))) + " ]");
                }
                else
                {
                    result = result.Replace("\"#PRODUCT_SCHEME_PHOTOS#\"",
                        "[ " + string.Join(", ", photos.Select(x => string.Format("{{\"picture\": {{\"src\": \"{1}\", \"preview\": \"{0}\"}}}}", x.ImageSrcRelative(ProductImageType.Middle), x.ImageSrcRelative(ProductImageType.Big)))) + " ]");
                }
            }

            if (text.Contains("#PRODUCT_SCHEME_GALLERY_PHOTO_"))
            {
                var photos = product.ProductPhotos.ToList();

                result = result.Replace("#PRODUCT_SCHEME_GALLERY_PHOTO_1#", photos.Count > 0 ? photos[0].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/gallery_scheme_img.jpg");
                result = result.Replace("#PRODUCT_SCHEME_GALLERY_PHOTO_2#", photos.Count > 1 ? photos[1].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/gallery_scheme_img.jpg");
                result = result.Replace("#PRODUCT_SCHEME_GALLERY_PHOTO_3#", photos.Count > 2 ? photos[2].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/gallery_scheme_img.jpg");
                result = result.Replace("#PRODUCT_SCHEME_GALLERY_PHOTO_4#", photos.Count > 3 ? photos[3].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/gallery_scheme_img.jpg");
                result = result.Replace("#PRODUCT_SCHEME_GALLERY_PHOTO_5#", photos.Count > 4 ? photos[4].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/gallery_scheme_img.jpg");
                result = result.Replace("#PRODUCT_SCHEME_GALLERY_PHOTO_6#", photos.Count > 5 ? photos[5].ImageSrcRelative(ProductImageType.Middle) : "areas/landing/images/gallery/gallery_scheme_img.jpg");
            }

            if (text.Contains("#PRODUCT_SCHEME_GALLERY_PHOTOS#"))
            {
                var photos = product.ProductPhotos.ToList();

                if (photos.Count == 0)
                {
                    result = result.Replace("\"#PRODUCT_SCHEME_GALLERY_PHOTOS#\"",
                        string.Format(
                            "[ {{\"picture\": {{\"src\": \"{0}\", \"preview\": \"{0}\"}}}}, {{\"picture\": {{\"src\": \"{0}\", \"preview\": \"{0}\"}}}}, {{\"picture\": {{\"src\": \"{0}\", \"preview\": \"{0}\"}}}} ]",
                            "areas/landing/images/gallery/gallery_scheme_img.jpg"));
                }
                else
                {
                    result = result.Replace("\"#PRODUCT_SCHEME_GALLERY_PHOTOS#\"", 
                        "[ " + string.Join(", ", photos.Select(x => string.Format("{{\"picture\": {{\"src\": \"{0}\", \"preview\": \"{1}\"}}}}", x.ImageSrcRelative(ProductImageType.Big), x.ImageSrcRelative(ProductImageType.Middle)))) + " ]");
                }
            }

            if (text.Contains("#PRODUCT_PROPERTIES_ITEMS#"))
            {
                var properties = PropertyService.GetPropertyValuesByProductId(product.ProductId)
                    .OrderBy(x => x.Property.UseInBrief).Take(4).ToList();

                if (properties.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.Append("[");

                    sb.Append(String.Join(", ",
                        properties.Select(x =>
                            "{\"title\": \"" + Encode(x.Property.NameDisplayed ?? x.Property.Name) + "\", \"text\": \"" + Encode(x.Value) + "\" }")));

                    sb.Append("]");

                    result = result.Replace("\"#PRODUCT_PROPERTIES_ITEMS#\"", sb.ToString());
                }
                else
                {
                    result = result.Replace("\"#PRODUCT_PROPERTIES_ITEMS#\"",
                        "[{\"title\":\"150\",\"text\":\"Выполненых проектов\"},{\"title\":\"11\",\"text\":\"Наград\"},{\"title\":\"65\",\"text\":\"Пользователей\"},{\"title\":\"80\",\"text\":\"Чашек кофе\"}]");
                }
            }

            if (text.Contains("#PRODUCT_VIDEO#"))
            {
                result = result.Replace("\"#PRODUCT_VIDEO#\"",
                    product.ProductVideos != null && product.ProductVideos.Count > 0
                        ? "\"" + product.ProductVideos[0].PlayerCode.Replace(@"""", @"\""") + "\""
                        : "\"https://youtu.be/U5n0WsStu34\"");
            }

            if (text.Contains("#PRODUCT_VIDEO_EMPTY#"))
            {
                var video = product.ProductVideos.FirstOrDefault();
                var videoSrc = video != null
                    ? video.PlayerCode.Split('"').FirstOrDefault(x => x.Contains("youtu"))
                    : null;
                
                result = result.Replace("#PRODUCT_VIDEO_EMPTY#", videoSrc ?? "https://www.youtube.com/watch?v=cti6lZsZq3s");
            }

            if (text.Contains("SHOW_PRODUCT_VIDEO"))
            {
                result = result.Replace("\"#SHOW_PRODUCT_VIDEO#\"", product.ProductVideos != null && product.ProductVideos.Count > 0 ? "true" : "false");
            }

            if (text.Contains("#PRODUCT_PROPERTIES#"))
            {
                var productPropertyValues = PropertyService.GetPropertyValuesByProductId(product.ProductId).Where(v => v.Property.UseInDetails).ToList();

                if (productPropertyValues.Count > 0)
                {
                    var propertyValues = new List<PropertyValue>();

                    foreach (var value in productPropertyValues.Where(x => propertyValues.All(pv => pv.PropertyId != x.PropertyId)))
                    {
                        propertyValues.Add(new PropertyValue()
                        {
                            Property = value.Property,
                            PropertyId = value.PropertyId,
                            PropertyValueId = value.PropertyValueId,
                            SortOrder = value.SortOrder,
                            Value = String.Join(", ", productPropertyValues.Where(x => x.PropertyId == value.PropertyId).Select(x => x.Value))
                        });
                    }

                    var sb = new StringBuilder();
                    sb.Append("[");

                    var groups = propertyValues.Select(x => x.Property.Group).Distinct().ToList();
                    for (var i = 0; i < groups.Count; i++)
                    {
                        if (i != 0)
                            sb.Append(", ");

                        sb.AppendFormat("{{ \"header\": \"{0}\", \"content_items\": [ ", groups[i] != null ? Encode(groups[i].NameDisplayed ?? groups[i].Name) : "");

                        var groupId = groups[i] != null ? groups[i].PropertyGroupId : default(int?);
                        sb.AppendLine(String.Join(", ",
                            propertyValues.Where(x => x.Property.GroupId == groupId)
                                .Select(x => "{ \"name\": \"" + Encode(x.Property.NameDisplayed ?? x.Property.Name) + "\", \"value\": \"" + Encode(x.Value) + "\" }")));

                        sb.AppendLine("]}");
                    }

                    sb.Append("]");

                    result = result.Replace("\"#PRODUCT_PROPERTIES#\"", sb.ToString());
                }
                else
                {
                    result = result.Replace("\"#PRODUCT_PROPERTIES#\"",
                        "[{\"header\":\"Описание\",\"content_items\":[{\"name\":\"Инструкция на цифровом носителе\",\"value\":\"Да\"},{\"name\":\"Гарантия\",\"value\":\"2 года\"},{\"name\":\"Страна\",\"value\":\"Тайвань\"}]},{\"header\":\"Дополнительно\",\"content_items\":[{\"name\":\"Количество раземов\",\"value\":\"5\"},{\"name\":\"Наличие подзарядки\",\"value\":\"есть\"}]}]");
                }
            }

            if (text.Contains("#PRODUCT_PROPERTIES_IN_BRIEF#"))
            {
                var productPropertyValues = PropertyService.GetPropertyValuesByProductId(product.ProductId).Where(x => x.Property.UseInBrief).ToList();

                if (productPropertyValues.Count > 0)
                {
                    var propertyValues = new List<PropertyValue>();

                    foreach (var value in productPropertyValues.Where(x => propertyValues.All(pv => pv.PropertyId != x.PropertyId)))
                    {
                        propertyValues.Add(new PropertyValue()
                        {
                            Property = value.Property,
                            PropertyId = value.PropertyId,
                            PropertyValueId = value.PropertyValueId,
                            SortOrder = value.SortOrder,
                            Value = String.Join(", ", productPropertyValues.Where(x => x.PropertyId == value.PropertyId).Select(x => x.Value))
                        });
                    }

                    var html = "";

                    var properties = propertyValues.Select(x => x.Property).Distinct().ToList();
                    foreach (var property in properties)
                    {
                        html +=
                            string.Format("{0}: {1} <br/> ",
                                property.NameDisplayed ?? property.Name,
                                String.Join(", ", propertyValues.Where(x => x.Property.PropertyId == property.PropertyId).Select(x => x.Value)));
                    }

                    result = result.Replace("#PRODUCT_PROPERTIES_IN_BRIEF#", html);
                }
                else
                {
                    result = result.Replace("#PRODUCT_PROPERTIES_IN_BRIEF#", "");
                }
            }

            if (text.Contains("#PRODUCT_REVIEWS#"))
            {
                var reviews =
                    ReviewService.GetReviews(product.ProductId, EntityType.Product)
                        .OrderByDescending(x => x.RatioByLikes)
                        .ToList();

                if (reviews.Count > 0)
                {
                    result = result.Replace("\"#PRODUCT_REVIEWS#\"",
                        "[" +
                        string.Join(", ",
                            reviews.Take(3).Select(x => string.Format("{{\"author\": \"{0}\", \"rating\": {1}, \"text\": \"{2}\"}}", x.Name, x.RatioByLikes != 0 ? x.RatioByLikes : 5, x.Text.Replace("\"", "")))) +
                        "]");
                }
                else
                {
                    result = result.Replace("\"#PRODUCT_REVIEWS#\"",
                        "[{\"author\": \"Юлия Золотова\", \"rating\": 4, \"text\": \"Если вы занимаетесь продвижением вашей компании, то должны знать, что Advantshop будет прекрасным помощником вашему бизнесу!\"}, {\"author\": \"Елена Прохорова\", \"rating\": 4, \"text\": \"Advantshop - прекрасное решение в области интернет-маркетинга. Долгое время мы не могли сделать сайт нашей редакции, Advantshop, быстро решил эту проблему.\"}, {\"author\": \"Кристина Крайнова\", \"rating\": 4, \"text\": \"Моя работа требует от меня скорости в исполнение задачи, в том числе и загрузка интернет-ресурсов. Advantshop - быстро, удобно, качественно!\"}]");
                }
            }

            if (configuration.Offers != null && configuration.Offers.Count > 0)
            {
                result = result.Replace("#PRODUCT_NAME_SET#", String.Join(" + ", configuration.Offers.Select(x => StringHelper.RemoveHTML(x.Product.Name))));

                if (text.Contains("#PRODUCT_PRICE_SET#") || text.Contains("#PRODUCT_OLD_PRICE_SET#") || 
                    text.Contains("#PRODUCT_PRICE_PLAIN_SET#") || text.Contains("#PRODUCT_OLD_PRICE_PLAIN_SET#"))
                {
                    var preparedPrice = 0f;
                    var preparedOldPrice = 0f;

                    foreach (var offer in configuration.Offers)
                    {
                        var price = offer.RoundedPrice.RoundPrice(CurrencyService.CurrentCurrency.Rate);
                        var finalDiscount =
                            PriceService.GetFinalDiscount(price, offer.Product.Discount, offer.Product.Currency.Rate,
                                CustomerContext.CurrentCustomer.CustomerGroup, offer.ProductId,
                                doNotApplyOtherDiscounts: offer.Product.DoNotApplyOtherDiscounts,
                                productMainCategoryId: product.CategoryId);
                        var finalPrice = PriceService.GetFinalPrice(price, finalDiscount);

                        preparedPrice += finalPrice;
                        preparedOldPrice += price;
                    }

                    var preparedPriceStr = PriceFormatService.FormatPrice(preparedPrice);
                    var preparedOldPriceStr = PriceFormatService.FormatPrice(preparedOldPrice);

                    result = 
                        result
                            .Replace("#PRODUCT_PRICE_SET#", "<div class=\"lp-h1 lp-h1--color lp-price\">" + preparedPriceStr + "</div> ")
                            .Replace("#PRODUCT_OLD_PRICE_SET#", 
                                        !string.IsNullOrEmpty(preparedOldPriceStr) && preparedPriceStr != preparedOldPriceStr
                                            ? "<div class=\"lp-old-price\">" + preparedOldPriceStr + "</div> "
                                            : "")
                            .Replace("#PRODUCT_PRICE_PLAIN_SET#", preparedPrice.ToString("# ###"))
                            .Replace("#PRODUCT_OLD_PRICE_PLAIN_SET#", preparedOldPrice.ToString("# ###"));
                }

                if (text.Contains("#ACTION_OFFER_IDS_SET#"))
                {
                    result = result.Replace("\"#ACTION_OFFER_IDS_SET#\"",
                        "[" + String.Join(",", configuration.Offers.Select(x => "{\"offerId\": \"" + x.OfferId + "\"}")) + "]");
                }

                if (text.Contains("#PRODUCT_SCHEME_GALLERY_PHOTOS_SET#"))
                {
                    var photos = configuration.Offers.SelectMany(x => x.Product.ProductPhotos).ToList();
                    if (photos.Count == 0)
                    {
                        result = result.Replace("\"#PRODUCT_SCHEME_GALLERY_PHOTOS_SET#\"",
                            string.Format(
                                "[ {{\"picture\": {{\"src\": \"{0}\", \"preview\": \"{0}\"}}}}, {{\"picture\": {{\"src\": \"{0}\", \"preview\": \"{0}\"}}}}, {{\"picture\": {{\"src\": \"{0}\", \"preview\": \"{0}\"}}}} ]",
                                "areas/landing/images/gallery/gallery_scheme_img.jpg"));
                    }
                    else
                    {
                        result = result.Replace("\"#PRODUCT_SCHEME_GALLERY_PHOTOS_SET#\"",
                            "[ " + string.Join(", ", photos.Select(x => string.Format("{{\"picture\": {{\"src\": \"{0}\", \"preview\": \"{0}\"}}}}", x.ImageSrcRelative(ProductImageType.Middle)))) + " ]");
                    }
                }

                if (text.Contains("#PRODUCT_SCHEME_PHOTOS_SET#"))
                {
                    var photos = configuration.Offers.SelectMany(x => x.Product.ProductPhotos).ToList();
                    if (photos.Count == 0)
                    {
                        var placeholder = new List<string>(3)
                        {
                            "areas/landing/images/productsView/img.jpg",
                            "areas/landing/images/productsView/img.jpg",
                            "areas/landing/images/productsView/img.jpg"
                        };

                        result = result.Replace("\"#PRODUCT_SCHEME_PHOTOS_SET#\"",
                            "[ " + string.Join(", ", placeholder.Select(x => string.Format("{{\"picture\": {{\"src\": \"{0}\", \"preview\": \"{0}\"}}}}", x))) + " ]");
                    }
                    else
                    {
                        result = result.Replace("\"#PRODUCT_SCHEME_PHOTOS_SET#\"",
                            "[ " + string.Join(", ", photos.Select(x => string.Format("{{\"picture\": {{\"src\": \"{1}\", \"preview\": \"{0}\"}}}}", x.ImageSrcRelative(ProductImageType.Middle), x.ImageSrcRelative(ProductImageType.Big)))) + " ]");
                    }
                }

                if (text.Contains("#PRODUCT_NAME_TABLESET#"))
                {
                    var names =
                        configuration.Offers.Select(
                            x =>
                                string.Format("<div><font color='#1779fa'>{0}</font></div>",
                                    StringHelper.RemoveHTML(x.Product.Name).Replace("\"", ""))).ToList();

                    result = result.Replace("#PRODUCT_NAME_TABLESET#", string.Join("<div>&nbsp;</div>", names));
                }

                if (text.Contains("#PRODUCT_PRICE_TABLESET#"))
                {
                    var priceStrs = new List<string>();

                    foreach (var offer in configuration.Offers)
                    {
                        var price = offer.RoundedPrice.RoundPrice(CurrencyService.CurrentCurrency.Rate);
                        var finalDiscount =
                            PriceService.GetFinalDiscount(price, offer.Product.Discount, offer.Product.Currency.Rate,
                                CustomerContext.CurrentCustomer.CustomerGroup, offer.ProductId,
                                doNotApplyOtherDiscounts: offer.Product.DoNotApplyOtherDiscounts,
                                productMainCategoryId: product.CategoryId);
                        var finalPrice = PriceService.GetFinalPrice(price, finalDiscount);

                        priceStrs.Add(
                            price != finalPrice
                                ? string.Format("<div><s>{0} руб</s>&nbsp; &nbsp;<strong>{1} руб</strong></div>", price, finalPrice)
                                : string.Format("<div><strong>{0} руб</strong></div>", finalPrice)
                            );
                    }

                    result = result.Replace("#PRODUCT_PRICE_TABLESET#", string.Join("<div>&nbsp;</div>", priceStrs));
                }
            }

            if (text.Contains("#PRODUCT_DESCRIPTION_(") && text.Contains(")_END#"))
            {
                var start = text.IndexOf("#PRODUCT_DESCRIPTION_(");
                var end = text.IndexOf(")_END#");

                if (start >= 0 && end >= 0)
                {
                    var defaultText = text.Substring(start + 22, end - start - 22);

                    result = result.Replace("#PRODUCT_DESCRIPTION_(" + defaultText + ")_END#",
                        !string.IsNullOrEmpty(product.Description) ? product.Description : defaultText);
                }
            }

            if (text.Contains("#PRODUCT_BRIEF_DESCRIPTION_(") && text.Contains(")_END#"))
            {
                var start = text.IndexOf("#PRODUCT_BRIEF_DESCRIPTION_(");
                var end = text.IndexOf(")_END#");

                if (start >= 0 && end >= 0)
                {
                    var defaultText = text.Substring(start + 28, end - start - 28);

                    result = result.Replace("#PRODUCT_BRIEF_DESCRIPTION_(" + defaultText + ")_END#",
                        !string.IsNullOrEmpty(product.Description) ? product.Description : defaultText);
                }
            }
        }

        public string PrepareContentAfter(string currentText, string text, LpConfiguration conf, LpConfigurationAfter confAfter)
        {
            var result = new StringBuilder(text);

            result = result
                .Replace("\"#ACTION_MAIN_LP_ID#\"", confAfter.MainLpId != null ? confAfter.MainLpId.ToString() : "null")
                .Replace("\"#ACTION_MAIN_2_LP_ID#\"", confAfter.MainSecondLpId != null ? confAfter.MainSecondLpId.ToString() : "null")
                .Replace("\"#ACTION_MAIN_3_LP_ID#\"", confAfter.MainThirdLpId != null ? confAfter.MainThirdLpId.ToString() : "null")
                .Replace("\"#ACTION_MAIN_4_LP_ID#\"", confAfter.MainFourLpId != null ? confAfter.MainFourLpId.ToString() : "null")
                .Replace("\"#ACTION_MAIN_5_LP_ID#\"", confAfter.MainFiveLpId != null ? confAfter.MainFiveLpId.ToString() : "null")
                .Replace("\"#ACTION_MAIN_6_LP_ID#\"", confAfter.MainSixLpId != null ? confAfter.MainSixLpId.ToString() : "null")

                .Replace("\"#ACTION_UPSELL_1_LP_ID#\"", 
                    confAfter.UpsellFirstLpId != null 
                        ? confAfter.UpsellFirstLpId.ToString() 
                        : (confAfter.ThankYouPageLpId != null ? confAfter.ThankYouPageLpId.ToString() : "null"))
                .Replace("\"#ACTION_UPSELL_2_LP_ID#\"", confAfter.UpsellSecondLpId != null ? confAfter.UpsellSecondLpId.ToString() : "null")
                .Replace("\"#ACTION_DOWNSELL_LP_ID#\"", 
                    confAfter.DownsellLpId != null 
                        ? confAfter.DownsellLpId.ToString() 
                        : (confAfter.ThankYouPageLpId != null 
                            ? confAfter.ThankYouPageLpId.ToString() 
                            : "null"))
                .Replace("\"#ACTION_THANKYOUPAGE_LP_ID#\"", confAfter.ThankYouPageLpId != null ? confAfter.ThankYouPageLpId.ToString() : "null");


            if (text.Contains("#ACTION_MAIN_1_LP_URL#"))
            {
                if (confAfter.MainSecondLpId != null)
                {
                    var url = _lpService.GetLpLinkRelative(confAfter.MainLpId.Value);
                    result = result.Replace("\"#ACTION_MAIN_1_LP_URL#\"", "\"" + url + "\"");
                }
                else
                    result = result.Replace("\"#ACTION_MAIN_1_LP_URL#\"", "null");
            }

            if (text.Contains("#ACTION_MAIN_2_LP_URL#"))
            {
                if (confAfter.MainSecondLpId != null)
                {
                    var url = _lpService.GetLpLinkRelative(confAfter.MainSecondLpId.Value);
                    result = result.Replace("\"#ACTION_MAIN_2_LP_URL#\"", "\"" + url + "\"");
                }
                else
                    result = result.Replace("\"#ACTION_MAIN_2_LP_URL#\"", "null");
            }

            if (text.Contains("#ACTION_MAIN_3_LP_URL#"))
            {
                result = result.Replace("\"#ACTION_MAIN_3_LP_URL#\"",
                    "\"" +
                    (confAfter.MainThirdLpId != null ? _lpService.GetLpLinkRelative(confAfter.MainThirdLpId.Value) : "") +
                    "\"");
            }

            if (text.Contains("#ACTION_MAIN_4_LP_URL#"))
            {
                result = result.Replace("\"#ACTION_MAIN_4_LP_URL#\"",
                    "\"" +
                    (confAfter.MainFourLpId != null ? _lpService.GetLpLinkRelative(confAfter.MainFourLpId.Value) : "") +
                    "\"");
            }

            if (text.Contains("#ACTION_MAIN_5_LP_URL#"))
            {
                result = result.Replace("\"#ACTION_MAIN_5_LP_URL#\"",
                    "\"" +
                    (confAfter.MainFiveLpId != null ? _lpService.GetLpLinkRelative(confAfter.MainFiveLpId.Value) : "") +
                    "\"");
            }

            if (text.Contains("#ACTION_MAIN_6_LP_URL#"))
            {
                result = result.Replace("\"#ACTION_MAIN_6_LP_URL#\"",
                    "\"" +
                    (confAfter.MainSixLpId != null ? _lpService.GetLpLinkRelative(confAfter.MainSixLpId.Value) : "") +
                    "\"");
            }

            if (text.Contains("#ACTION_UPSELL_1_LP_URL#"))
            {
                if (confAfter.UpsellFirstLpId != null)
                {
                    var url = _lpService.GetLpLinkRelative(confAfter.UpsellFirstLpId.Value);
                    result = result.Replace("\"#ACTION_UPSELL_1_LP_URL#\"", "\"" + url + "\"");
                }
                else
                    result = result.Replace("\"#ACTION_UPSELL_1_LP_URL#\"", "null");
            }

            if (text.Contains("#ACTION_UPSELL_2_LP_URL#"))
            {
                if (confAfter.UpsellSecondLpId != null)
                {
                    var url = _lpService.GetLpLinkRelative(confAfter.UpsellSecondLpId.Value);
                    result = result.Replace("\"#ACTION_UPSELL_2_LP_URL#\"", "\"" + url + "\"");
                }
                else
                    result = result.Replace("\"#ACTION_UPSELL_2_LP_URL#\"", "null");
            }

            if (text.Contains("#ACTION_DOWNSELL_LP_URL#"))
            {
                if (confAfter.DownsellLpId != null)
                {
                    var url = _lpService.GetLpLinkRelative(confAfter.DownsellLpId.Value);
                    result = result.Replace("\"#ACTION_DOWNSELL_LP_URL#\"", "\"" + url + "\"");
                }
                else
                {
                    if (confAfter.ThankYouPageLpId != null)
                    {
                        var url = _lpService.GetLpLinkRelative(confAfter.ThankYouPageLpId.Value);
                        result = result.Replace("\"#ACTION_DOWNSELL_LP_URL#\"", "\"" + url + "\"");
                    }
                    else
                        result = result.Replace("#ACTION_DOWNSELL_LP_URL#", "checkout/success");
                }
            }

            if (text.Contains("#ACTION_THANKYOUPAGE_LP_URL#"))
            {
                if (confAfter.ThankYouPageLpId != null)
                {
                    var url = _lpService.GetLpLinkRelative(confAfter.ThankYouPageLpId.Value);
                    result = result.Replace("\"#ACTION_THANKYOUPAGE_LP_URL#\"", "\"" + url + "\"");
                }
                else
                    result = result.Replace("#ACTION_THANKYOUPAGE_LP_URL#", "checkout/success");
            }

            if (confAfter.AddedBlocks.Count > 0 && text.Contains("#BLOCK_ID_"))
            {
                foreach (var block in confAfter.AddedBlocks.Where(x => x.LandingId == confAfter.LandingId))
                    result = result.Replace("#BLOCK_ID_" + block.Name + "#", "#block_" + block.Id);
            }

            if (text.Contains("#POST_MESSAGE_REDIRECT_URL#"))
            {
                result = result.Replace("#POST_MESSAGE_REDIRECT_URL#", confAfter.PostActionUrl ?? "");
            }

            return result.ToString();
        }


        private string Encode(string value)
        {
            return value.Replace("\"", "");
        }

        private string ButtonAction(LpFunnelType type, string template)
        {
            if (type == LpFunnelType.OneProductUpSellDownSell || type == LpFunnelType.Couponator)
                return "CheckoutUpsell";

            return "Url";
        }
        
        #endregion

        #region Templates

        private static bool IsTemplateAllowedBySaas(string templateKey)
        {
            if (SaasDataService.IsSaasEnabled is false
                || SaasDataService.CurrentSaasData.AvailableFunnelsTemplatesList is null
                || SaasDataService.CurrentSaasData.AvailableFunnelsTemplatesList.Count == 0)
                return true;

            return SaasDataService.CurrentSaasData.AvailableFunnelsTemplatesList
                .Any(x => x.Equals(templateKey, StringComparison.OrdinalIgnoreCase));
        }

        public List<LpTemplate> GetTemplates()
        {
            var lpTemplates = CacheManager.Get(CacheKey + "AllTemplates", 5, GetTemplatesInternal);

            return lpTemplates
                .Where(template => IsTemplateAllowedBySaas(template.Key))
                .OrderBy(x => x.SortOrder)
                .ToList();
        }

        private List<LpTemplate> GetTemplatesInternal()
        {
            const string key = CacheKey + "AllTemplates";
            var cached = CacheManager.TryGetValue(key, out List<LpTemplate> templates);

            if (cached)
            {
                return templates;
            }

            templates = new List<LpTemplate>();
            var path = HostingEnvironment.MapPath(LpFiles.TepmlateFolder);

            foreach (var directory in Directory.GetDirectories(path))
            {
                try
                {
                    var configPath = directory + "\\config.json";
                    if (File.Exists(configPath))
                    {
                        var configContent = "";

                        using (var sr = new StreamReader(configPath))
                            configContent = sr.ReadToEnd();

                        var templateConfig = JsonConvert.DeserializeObject<LpTemplateConfig>(configContent);
                        
                        if (templateConfig != null && !string.IsNullOrEmpty(templateConfig.Name))
                            templates.Add(new LpTemplate(templateConfig));
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }

            var resultObj = templates.ToList();

            CacheManager.Insert(key, resultObj, 5);

            return resultObj;
        }

        public LpTemplate GetTemplate(string name)
        {
            return CacheManager.Get(LpConstants.LpTemplatesCachePrefix + name, LpConstants.LpCacheTime,
                () =>
                {
                    try
                    {
                        var configPath = HostingEnvironment.MapPath(LpFiles.TepmlateFolder + "/" + name + "\\config.json");
                        if (File.Exists(configPath))
                        {
                            var configContent = "";

                            using (var sr = new StreamReader(configPath))
                                configContent = sr.ReadToEnd();

                            return new LpTemplate(JsonConvert.DeserializeObject<LpTemplateConfig>(configContent));
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                    return null;
                });
        }

        public List<LpBlockListItem> GetAllBlocks()
        {
            return CacheManager.Get(LpConstants.LpTemplatesCachePrefix + "allblocks", LpConstants.LpCacheTime,
                () =>
                {
                    try
                    {
                        var configPath = HostingEnvironment.MapPath(LpFiles.LpStaticPath + "/blocks.json");
                        if (File.Exists(configPath))
                        {
                            var configContent = "";

                            using (var sr = new StreamReader(configPath))
                                configContent = sr.ReadToEnd();

                            return JsonConvert.DeserializeObject<List<LpBlockListItem>>(configContent);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                    return null;
                });
        }

        #endregion
    }
}
