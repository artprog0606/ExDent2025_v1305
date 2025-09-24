// using AdvantShop.Areas.Api.Models.Bonuses;
// using AdvantShop.Areas.Api.Models.Customers;
// using AdvantShop.Core.Services.Api;
// using AdvantShop.Core.Services.Bonuses.Model;
// using AdvantShop.Core.Services.Bonuses.Service;
// using AdvantShop.Customers;
// using Newtonsoft.Json;
// using NUnit.Framework;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
// using System.Net.Http;
// using System.Text;
// using System.Threading.Tasks;
//
// namespace AdvantShop.Core.Test
// {
//     // change connection string in base class
//     //[TestFixture]
//     public class ObsoleteBonusesApiTest : BaseApiTest
//     {
//         #region init & cleanup
//
//         private Customer _customerBonuses;
//         private long cardId;
//         private int _additionalBonusId;
//         private BonusSystemSettings _settings;
//
//         private const string AddMainBonusReason = "Add main bonuses";
//         private const string SubstractMainBonusReason = "Substract main bonuses";
//         private const string AddAddditionalBonusReason = "Add addditional bonuses";
//         private const string SubstractAddditionalBonusReason = "Substract addditional bonuses";
//
//         //[OneTimeSetUp]
//         public void Init()
//         {
//             base.BaseInit();
//
//             _customerBonuses = new Customer()
//             {
//                 EMail = _customer.Email,
//                 FirstName = _customer.FirstName,
//                 LastName = _customer.LastName,
//                 Patronymic = _customer.Patronymic,
//                 BirthDay = _customer.BirthDay,
//                 StandardPhone = _customer.Phone,
//                 Phone = _customer.Phone.ToString(),
//                 AdminComment = _customer.AdminComment
//             };
//             CustomerService.InsertNewCustomer(_customerBonuses);
//         }
//
//         //[OneTimeTearDown]
//         public void Cleanup()
//         {
//             base.BaseCleanup();
//
//             if (_customerBonuses != null && _customerBonuses.Id != Guid.Empty)
//                 CustomerService.DeleteCustomer(_customerBonuses.Id);
//
//             if (cardId != 0)
//                 CardService.Delete(_customerBonuses.Id);
//         }
//
//         #endregion
//
//         /* 
//             Plan:
//
//             1) Create card
//             2) Get card
//             3) Add main bonuses
//             4) Substract main bonuses
//             5) Add addditional bonuses
//             6) Get addditional bonuses
//             7) Substract addditional bonuses
//             8) Get transactions
//             
//             9) Get settings
//             10) Save settings
//
//             11) Get grades
//         */
//
//         // 1) Create card
//         //[Test, Order(0)]
//         public async Task CreateCardAsync()
//         {
//             var json = JsonConvert.SerializeObject(new { customerId = _customerBonuses.Id });
//             var data = new StringContent(json, Encoding.UTF8, "application/json");
//
//
//             var response = await _client.PostAsync($"/api/bonus-cards/add?apikey={_apiKey}", data);
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//
//             Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
//             Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));
//
//             var result = JsonConvert.DeserializeObject<BonusCardResponse>(content);
//
//             Assert.IsNotNull(result);
//
//             cardId = result.CardId;
//         }
//
//         // 2) Get card
//         //[Test, Order(1)]
//         public async Task GetCardAsync()
//         {
//             var request = new HttpRequestMessage(HttpMethod.Get, $"/api/bonus-cards/{cardId}?apikey={_apiKey}");
//
//             var response = await _client.SendAsync(request);
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//
//             Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
//             Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));
//
//             var result = JsonConvert.DeserializeObject<BonusCardResponse>(content);
//
//             Assert.AreEqual(true, result != null);
//             Assert.AreEqual(cardId, result.CardId);
//         }
//
//
//         // 3) Add main bonuses
//         //[Test, Order(2)]
//         public async Task AddMainBonusesAsync()
//         {
//             var json = JsonConvert.SerializeObject(new MainBonusModel() { Amount = 2000, Reason = AddMainBonusReason, SendSms = false });
//             var data = new StringContent(json, Encoding.UTF8, "application/json");
//
//
//             var response = await _client.PostAsync($"/api/bonus-cards/{cardId}/main-bonuses/accept?apikey={_apiKey}", data);
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//
//             Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
//             Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));
//
//             var result = JsonConvert.DeserializeObject<ApiResponse>(content);
//
//             Assert.IsNotNull(result);
//             Assert.AreEqual(ApiStatus.Ok, result.Status);
//         }
//
//         //  4) Substract main bonuses
//         //[Test, Order(3)]
//         public async Task SubstractMainBonusesAsync()
//         {
//             var json = JsonConvert.SerializeObject(new MainBonusModel() { Amount = 1000, Reason = SubstractMainBonusReason, SendSms = false });
//             var data = new StringContent(json, Encoding.UTF8, "application/json");
//
//
//             var response = await _client.PostAsync($"/api/bonus-cards/{cardId}/main-bonuses/substract?apikey={_apiKey}", data);
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//
//             Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
//             Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));
//
//             var result = JsonConvert.DeserializeObject<ApiResponse>(content);
//
//             Assert.IsNotNull(result);
//             Assert.AreEqual(ApiStatus.Ok, result.Status);
//         }
//
//
//
//         // 5) Add addditional bonuses
//         //[Test, Order(5)]
//         public async Task AddAddditionalBonusesAsync()
//         {
//             var json = JsonConvert.SerializeObject(new AddAdditionalBonusModel() { 
//                 Amount = 73, 
//                 Reason = AddAddditionalBonusReason, 
//                 Name = "Add addditional bonuses", 
//                 SendSms = false,
//                 StartDate = DateTime.Now.AddDays(-10),
//                 EndDate = DateTime.Now.AddDays(10)
//             });
//             var data = new StringContent(json, Encoding.UTF8, "application/json");
//
//
//             var response = await _client.PostAsync($"/api/bonus-cards/{cardId}/additional-bonuses/accept?apikey={_apiKey}", data);
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//
//             Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
//             Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));
//
//             var result = JsonConvert.DeserializeObject<ApiResponse>(content);
//
//             Assert.IsNotNull(result);
//             Assert.AreEqual(ApiStatus.Ok, result.Status);
//         }
//
//         // 6) Get addditional bonuses
//         //[Test, Order(6)]
//         public async Task GetAddditionalBonusesAsync()
//         {
//             var request = new HttpRequestMessage(HttpMethod.Get, $"/api/bonus-cards/{cardId}/additional-bonuses?apikey={_apiKey}");
//
//             var response = await _client.SendAsync(request);
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//
//             Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
//             Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));
//
//             var result = JsonConvert.DeserializeObject<List<Bonus>>(content);
//
//             Assert.AreEqual(true, result != null && result.Count > 1);
//             
//             _additionalBonusId = result[0].Id;
//             
//             Assert.AreEqual(true, result.Count == 1);
//         }
//
//         // 7) Substract main bonuses
//         //[Test, Order(7)]
//         public async Task SubstractAddditionalBonusesAsync()
//         {
//             var json = JsonConvert.SerializeObject(new SubctractAdditionalBonusModel() 
//             { 
//                 AdditionalBonusId = _additionalBonusId,
//                 Amount = 23, 
//                 Reason = SubstractAddditionalBonusReason, 
//                 SendSms = false 
//             });
//             var data = new StringContent(json, Encoding.UTF8, "application/json");
//
//
//             var response = await _client.PostAsync($"/api/bonus-cards/{cardId}/additional-bonuses/substract?apikey={_apiKey}", data);
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//
//             Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
//             Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));
//
//             var result = JsonConvert.DeserializeObject<ApiResponse>(content);
//
//             Assert.IsNotNull(result);
//             Assert.AreEqual(ApiStatus.Ok, result.Status);
//         }
//
//
//
//         // 8) Get transactions
//         //[Test, Order(8)]
//         public async Task GetTransactionsAsync()
//         {
//             var request = new HttpRequestMessage(HttpMethod.Get, $"/api/bonus-cards/{cardId}/transactions?apikey={_apiKey}");
//
//             var response = await _client.SendAsync(request);
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//
//             Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
//             Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));
//
//             var result = JsonConvert.DeserializeObject<GetTransactionsResponse>(content);
//
//             Assert.AreEqual(true, result != null && result.Transactions != null);
//             Assert.AreEqual(4, result.Transactions.Count);
//
//             Assert.AreEqual(true, result.Transactions.Any(x => x.Basis == AddMainBonusReason));
//             Assert.AreEqual(true, result.Transactions.Any(x => x.Basis == SubstractMainBonusReason));
//             Assert.AreEqual(true, result.Transactions.Any(x => x.Basis == AddAddditionalBonusReason));
//             Assert.AreEqual(true, result.Transactions.Any(x => x.Basis == SubstractAddditionalBonusReason));
//         }
//
//
//
//         // 9) Get settings
//         //[Test, Order(9)]
//         public async Task GetSettingsAsync()
//         {
//             var request = new HttpRequestMessage(HttpMethod.Get, $"/api/bonus-cards/settings?apikey={_apiKey}");
//
//             var response = await _client.SendAsync(request);
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//
//             Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
//             Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));
//
//             var result = JsonConvert.DeserializeObject<BonusSystemSettings>(content);
//
//             Assert.AreEqual(true, result != null);
//
//             _settings = result;
//         }
//
//         // 10) Save settings
//         //[Test, Order(10)]
//         public async Task SaveSettingsCardAsync()
//         {
//             _settings.BonusType = Services.Bonuses.EBonusType.ByProductsCost;
//
//             var json = JsonConvert.SerializeObject(_settings);
//             var data = new StringContent(json, Encoding.UTF8, "application/json");
//
//
//             var response = await _client.PostAsync($"/api/bonus-cards/settings?apikey={_apiKey}", data);
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//
//             Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
//             Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));
//         }
//
//
//
//         // 11) Get grades
//         //[Test, Order(11)]
//         public async Task GetGradesAsync()
//         {
//             var request = new HttpRequestMessage(HttpMethod.Get, $"/api/bonus-grades?apikey={_apiKey}");
//
//             var response = await _client.SendAsync(request);
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//
//             Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
//             Assert.AreEqual(true, !string.IsNullOrEmpty(content) && !content.Contains("error"));
//         }
//     }
// }
