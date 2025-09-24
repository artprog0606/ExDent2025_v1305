using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Customers;
using System;
using System.Configuration;
using System.Net.Http;
using System.Reflection;

namespace AdvantShop.Core.Test
{    
    public class BaseApiTest
    {
        protected HttpClient _client;
        protected string _apiKey;
        protected bool _apiKeyEmpty;
        protected AddUpdateCustomerModel _customer;

        protected void BaseInit()
        {
            Hack();

            _client = new HttpClient();
            _client.BaseAddress = new Uri(SettingsMain.SiteUrl);
            _client.Timeout = TimeSpan.FromSeconds(10);

            if (string.IsNullOrEmpty(SettingsApi.ApiKey))
            {
                _apiKey = SettingsApi.ApiKey = Guid.NewGuid().ToString();
                _apiKeyEmpty = true;
            }
            else
            {
                _apiKey = SettingsApi.ApiKey;
            }

            _customer = new AddUpdateCustomerModel()
            {
                Email = $"{Guid.NewGuid()}@test.testcustomerapi",
                FirstName = "Scarlett",
                LastName = "Johansson",
                Patronymic = "Ingrid",
                Organization = "Avengers",
                BirthDay = new DateTime(1984, 11, 28),
                AdminComment = "91-66-91 1.6 Owns a popcorn shop in Paris",
                GroupId = CustomerGroupService.DefaultCustomerGroup,
                Password = "ikissedagirl",
                Contact = new CustomerContactModel()
                {
                    Country = "USA",
                    Region = "NY",
                    City = "NY",
                    District = "Manhattan"
                }
            };
        }

        protected void BaseCleanup()
        {
            if (_apiKeyEmpty)
                SettingsApi.ApiKey = null;

            if (_customer.Id != Guid.Empty)
                CustomerService.DeleteCustomer(_customer.Id);
        }

        private void Hack()
        {
            if (ConfigurationManager.ConnectionStrings["AdvantConnectionString"] != null)
                return;

            typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ConfigurationManager.ConnectionStrings, false);
            ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings("AdvantConnectionString", GetConnectionString()));
        }

        private string GetConnectionString()
        {
            // todo: find file
            return "Data Source='.\\SQL2012EXPRESS'; Connect Timeout='3'; Initial Catalog='dev'; Persist Security Info='True'; User ID='sa'; Password='xxx';";
        }
    }
}
