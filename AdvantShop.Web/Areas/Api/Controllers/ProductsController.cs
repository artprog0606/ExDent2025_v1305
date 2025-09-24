using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Domain;
using AdvantShop.Areas.Api.Handlers.Products;
using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class ProductsController : BaseApiController
    {
        // GET api/products/{id}?colorId={colorId}&sizeId={sizeId}
        [HttpGet]
        public JsonResult Get(int id, int? colorId, int? sizeId) => JsonApi(new GetProductApi(id, colorId, sizeId));
        
        // POST api/products/{id}/price
        [HttpPost]
        public JsonResult Price(int id, GetPriceModel model) => JsonApi(new GetProductPriceApi(id, model));
        
        // GET api/products/{id}/properties
        [HttpGet]
        public JsonResult Properties(int id, PropertyTypeApi? type) => 
            type == null || type == PropertyTypeApi.InDetails 
                ? JsonApi(new GetProductPropertiesApi(id))
                : JsonApi(new GetProductPropertiesInBriefApi(id));
        
        // GET api/products/{id}/reviews
        [HttpGet]
        public JsonResult Reviews(int id) => JsonApi(new GetProductReviewsApi(id));
        
        // POST api/products/{id}/reviews/add
        [HttpPost]
        public JsonResult AddReview(int id, AddReviewModel review) => JsonApi(new AddProductReviewApi(id, review));
        
        // GET api/products/{id}/related-products
        [HttpGet]
        public JsonResult RelatedProducts(int id, ERelatedProductsApiType type) => JsonApi(new GetRelatedProductsApi(id, type));
        
        // GET api/products/{id}/gifts
        [HttpGet]
        public JsonResult Gifts(int id, int? offerId) => JsonApi(new GetGiftsApi(id, offerId));
        
        // GET api/products/{id}/stocks/?offerId={offerId}
        [HttpGet]
        public JsonResult Stocks(int id, int? offerId) => JsonApi(new GetStocksApi(id, offerId));
        
        // GET api/products/{id}/price-rule-amount-list?offerId={offerId}
        [HttpGet]
        public JsonResult PriceRuleAmountList(int id, int? offerId) => JsonApi(new GetPriceRuleAmountListApi(id, offerId));
    }
}