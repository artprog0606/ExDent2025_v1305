using AdvantShop.Areas.Api.Models.Customers;
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
    public class CustomersApiTest : BaseApiTest
    {
        //[OneTimeSetUp]
        public void Init() => base.BaseInit();

        //[OneTimeTearDown]
        public void Cleanup() => base.BaseCleanup();

        /* 
            Plan:

            1) Create customer
            2) Get this customer
            3) Change this customer
            4) Get list of customers (with this customer)            
        */

        // 1) Create customer
        //[Test]
        public async Task AddCustomerAsync()
        {
            var json = JsonConvert.SerializeObject(_customer);
            var data = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await _client.PostAsync($"/api/customers/add?apikey={_apiKey}", data);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));

            var result = JsonConvert.DeserializeObject<AddUpdateCustomerResponse>(content);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(ApiStatus.Ok, result.Status);

            _customer.Id = result.Id;
        }

        // 2) Get this customer
        //[Test]
        public async Task GetCustomerAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/customers/{_customer.Id}?apikey={_apiKey}");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));

            var result = JsonConvert.DeserializeObject<GetCustomerResponse>(content);

            Assert.AreEqual(true, result != null);
            Assert.AreEqual(_customer.Email, result.Email);
        }

        // 3) Change this customer
        //[Test]
        public async Task UpdateCustomerAsync()
        {
            _customer.Organization = "The Avengers";

            var json = JsonConvert.SerializeObject(_customer);
            var data = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await _client.PostAsync($"/api/customers/{_customer.Id}?apikey={_apiKey}", data);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));

            var result = JsonConvert.DeserializeObject<AddUpdateCustomerResponse>(content);

            Assert.IsNotNull(result);
            Assert.AreEqual(ApiStatus.Ok, result.Status);
        }

        // 4) Get list of customers (with this customer)
        //[Test]
        public async Task GetCustomersListAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/customers?apikey={_apiKey}");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
                       
            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));

            var result = JsonConvert.DeserializeObject<GetCustomersResponse>(content);

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.Customers != null && result.Customers.Count > 0);
            Assert.AreEqual(true, result.Customers.Find(x => x.Email == _customer.Email) != null);
        }
    }
}
