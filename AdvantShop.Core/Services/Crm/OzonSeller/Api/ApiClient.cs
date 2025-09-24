using System.Collections.Generic;

namespace AdvantShop.Core.Services.Crm.OzonSeller.Api
{
    public class ApiClient : BaseApiClient
    {
        public const string DefaultLanguage = "RU";

        public ApiClient(string clientId, string apiKey) : base(clientId, apiKey)
        {
        }

        public BaseResultList<Category> GetTree(int? categoryId = null, string language = "RU")
        {
            return MakeRequest<BaseResultList<Category>, GetTreeParams>(
                "/v1/category/tree",
                new GetTreeParams
                {
                    CategoryId = categoryId,
                    Language = language
                });
        }

        public BaseResultList<CategoryAttribute> GetCategoryAttribute(int categoryId, ParamAttributeType? attributeType = null, string language = DefaultLanguage)
        {
            return MakeRequest<BaseResultList<CategoryAttribute>, GetCategoryAttributeParams>(
                "/v2/category/attribute",
                new GetCategoryAttributeParams
                {
                    CategoryId = categoryId,
                    AttributeType = attributeType,
                    Language = language
                });
        }

        public BaseResultList<CategoryAttributeValueByOption> GetCategoryAttributeValuesByOption(List<AttributeOptionOld> options, string language = DefaultLanguage)
        {
            return MakeRequest<BaseResultList<CategoryAttributeValueByOption>, GetCategoryAttributeValuesByOptionParams>(
                "/v2/category/attribute/value/by-option",
                new GetCategoryAttributeValuesByOptionParams
                {
                    Options = options,
                    Language = language
                });
        }

        public BaseResultList<CategoryAttributeValue> GetCategoryAttributeValues(int categoryId, int attributeId, int lastValueId = 0, int limit = 50, string language = DefaultLanguage)
        {
            return MakeRequest<BaseResultList<CategoryAttributeValue>, GetCategoryAttributeValuesParams>(
                "/v2/category/attribute/values",
                new GetCategoryAttributeValuesParams
                {
                    CategoryId = categoryId,
                    AttributeId = attributeId,
                    Language = language,
                    LastValueId = lastValueId,
                    Limit = limit
                });
        }

        /// <param name="optimalUseOfMemory">Хранение в памяти только последней полученной коллекции. Оптималь подходит для единоразового прочтения коллекции.</param>
        public IEnumerable<CategoryAttributeValue> GetCategoryAttributeValuesEnumerable(int categoryId, int attributeId, string language = DefaultLanguage, bool optimalUseOfMemory = true)
        {
            return EntityList<CategoryAttributeValue, GetCategoryAttributeValuesParams>.FactoryOffsetValueParam(
                clientId, 
                apiKey,
                "/v2/category/attribute/values",
                new GetCategoryAttributeValuesParams
                {
                    CategoryId = categoryId,
                    AttributeId = attributeId,
                    Language = language,
                    LastValueId = 0,
                    Limit = 50
                },
                expressionPropertyOffset: x => x.LastValueId,
                expressionPropertyLimit: x => x.Limit,
                expressionPropertyOffsetValue: x => x.Id,
                keepActualCollection: optimalUseOfMemory);
        }

        /// <summary>
        /// Метод для загрузки товаров
        /// <para>В одном запросе можно передать до 1000 товаров</para>
        /// </summary>
        /// <param name="products">Список товаров</param>
        /// <returns>Код задачи на импорт товаров</returns>
        public BaseResult<ImportProductsResult> ImportProducts(List<ProductImport> products)
        {
            return MakeRequest<BaseResult<ImportProductsResult>, ProductsImport>(
                "/v2/product/import",
                new ProductsImport
                {
                    Items = products
                });
        }

        /// <summary>
        /// Узнать статус добавления товара
        /// </summary>
        /// <param name="importProductsResult">Код задачи на импорт товаров</param>
        /// <returns></returns>
        public BaseResult<StatusImportProducts> GetStatusImportProducts(int taskId)
        {
            return MakeRequest<BaseResult<StatusImportProducts>, ImportProductsResult>(
                "/v1/product/import/info",
                new ImportProductsResult { TaskId = taskId });
        }

        public BaseResult<ProductSetPrepaymentResult> ProductSetPrepayment(ProductSetPrepaymentParams @params)
        {
            return MakeRequest<BaseResult<ProductSetPrepaymentResult>, ProductSetPrepaymentParams>(
                "/v1/product/prepayment/set",
                @params);
        }

        public BaseResult<ProductInfo> GetProductInfo(GetProductInfoParams @params)
        {
            return MakeRequest<BaseResult<ProductInfo>, GetProductInfoParams>(
                "/v2/product/info",
                @params);
        }

        /// <summary>
        /// Характеристики товара
        /// </summary>
        /// <returns></returns>
        public BaseResultList<ProductsInfoAttribute> GetProductsInfoAttributes(GetProductsInfoAttributesFilter filter, int page, int pageSize = 100)
        {
            return MakeRequest<BaseResultList<ProductsInfoAttribute>, GetProductsInfoAttributesParam>(
                "/v2/product/info/attributes",
                new GetProductsInfoAttributesParam { Filter = filter, Page = page, PageSize = pageSize });
        }

        /// <summary>
        /// Характеристики товара
        /// </summary>
        /// <param name="optimalUseOfMemory">Хранение в памяти только последней полученной коллекции. Оптималь подходит для единоразового прочтения коллекции.</param>
        /// <returns></returns>
        public IEnumerable<ProductsInfoAttribute> GetProductsInfoAttributesEnumerable(GetProductsInfoAttributesFilter filter, int pageSize = 100, bool optimalUseOfMemory = true)
        {
            return EntityList<ProductsInfoAttribute, GetProductsInfoAttributesParam>.FactoryPageParam(
                clientId,
                apiKey,
                "/v2/product/info/attributes",
                new GetProductsInfoAttributesParam { Filter = filter, PageSize = pageSize },
                expressionPropertyPage: x => x.Page,
                keepActualCollection: optimalUseOfMemory);
        }

        /// <summary>
        /// Информация о стоках товаров
        /// </summary>
        /// <returns></returns>
        public BaseResultItemsList<ProductStocks> GetProductsStocks(int page, int pageSize = 100)
        {
            return MakeRequest<BaseResultItemsList<ProductStocks>, GetProductsStocksParam>(
                "/v1/product/info/stocks",
                new GetProductsStocksParam { Page = page, PageSize = pageSize });
        }

        /// <summary>
        /// Информация о стоках товаров
        /// </summary>
        /// <param name="optimalUseOfMemory">Хранение в памяти только последней полученной коллекции. Оптималь подходит для единоразового прочтения коллекции.</param>
        /// <returns></returns>
        public IEnumerable<ProductStocks> GetProductsStocksEnumerable(int pageSize = 100, bool optimalUseOfMemory = true)
        {
            return EntityList<ProductStocks, GetProductsStocksParam>.FactoryPageParam(
                clientId,
                apiKey,
                "/v1/product/info/stocks",
                new GetProductsStocksParam { PageSize = pageSize },
                expressionPropertyPage: x => x.Page,
                typeContainer: TypeResult.BaseResultItemsList,
                keepActualCollection: optimalUseOfMemory);
        }
    }
}
