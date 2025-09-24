using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Api;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdvantShop.Core.Test
{
    // change web.config in base class
    //[TestFixture]
    public class CategoriesApiTest : BaseApiTest
    {
        private CategoryModel _category;

        //[OneTimeSetUp]
        public void Init()
        {
            _category = new CategoryModel()
            {
                Name = "test category",
                BriefDescription = "test brief",
                Description = "test description",
                Url = "test-category-url",
                Enabled = true,
                SortOrder = 10
            };
            base.BaseInit();
        }

        //[OneTimeTearDown]
        public void Cleanup()
        {
            if (_category != null && _category.Id != 0)
                CategoryService.DeleteCategoryAndPhotos(_category.Id);
            
            base.BaseCleanup();
        }

        /* 
            Plan:

            1) Create category
            2) Get this category
            3) Change this category            
            4) Get list of categories
            5) Delete this category
        */

        // 1) Create category
        //[Test, Order(0)]
        public async Task AddCategoryAsync()
        {
            var json = JsonConvert.SerializeObject(_category);
            var data = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await _client.PostAsync($"/api/categories/add?apikey={_apiKey}", data);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));

            var result = JsonConvert.DeserializeObject<AddUpdateCategoryResponse>(content);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(ApiStatus.Ok, result.Status);

            _category.Id = result.Id;
        }

        // 2) Get this category
        //[Test, Order(1)]
        public async Task GetCategoryAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/categories/{_category.Id}?apikey={_apiKey}");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));

            var result = JsonConvert.DeserializeObject<GetCategoryResponse>(content);

            Assert.AreEqual(true, result != null);
            Assert.AreEqual(_category.Name, result.Name);

            _category = result;
        }

        // 3) Change this category
        //[Test, Order(2)]
        public async Task UpdateCategoryAsync()
        {
            _category.Description = "new description";

            var json = JsonConvert.SerializeObject(_category);
            var data = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await _client.PostAsync($"/api/categories/{_category.Id}?apikey={_apiKey}", data);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));

            var result = JsonConvert.DeserializeObject<AddUpdateCategoryResponse>(content);

            Assert.IsNotNull(result);
            Assert.AreEqual(ApiStatus.Ok, result.Status);            
        }

        // 4) Get list of categories
        //[Test, Order(3)]
        public async Task GetCategoriesListAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/categories?apikey={_apiKey}");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
                       
            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));

            // var result = JsonConvert.DeserializeObject<GetCategoriesResponse>(content); // cannot deserialize ICategoryApi

            Assert.IsNotNull(content);
        }

        // 5) Delete this category
        //[Test, Order(4)]
        public async Task DeleteCategoryAsync()
        {
            var data = new StringContent("", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"/api/categories/{_category.Id}/delete?apikey={_apiKey}", data);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));

            var result = JsonConvert.DeserializeObject<DeleteCategoryResponse>(content);

            Assert.IsNotNull(result);
            Assert.AreEqual(ApiStatus.Ok, result.Status);
        }
    }
}
