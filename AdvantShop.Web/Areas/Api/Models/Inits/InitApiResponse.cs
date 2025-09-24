using System.Collections.Generic;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Areas.Api.Models.Settings;
using AdvantShop.Areas.Api.Models.Shared;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Api;
using AdvantShop.Repository;

namespace AdvantShop.Areas.Api.Models.Inits
{
    public class InitApiModel
    {
        public string Settings { get; set; }
        
        public string Device { get; set; }
        public string Os { get; set; }
        public string AppVersion { get; set; }
    }
    
    public class BaseInitApiResponse : IApiResponse
    {
        public List<IApiSettingItem> OptionalSettings { get; set; }
        public GetCustomerResponse Customer { get; set; }
        public CurrencyApi Currency { get; set; }
        public IpZone Location { get; set; }
        public GetDadataSettingsResponse Dadata { get; set; }
        public ContactInformation Contact { get; set; }
        public List<CarouselItem> Carousel { get; set; }
        public MainPageProductsInit Products { get; set; }
        public GetStoriesRespone Stories { get; set; }
    }
    
    public class MainPageProductsInit
    {
        public GetYouOrderedProductsResponse YouOrderedProducts { get; set; } 
    }

    
    public class InitApiResponse : BaseInitApiResponse
    {
        public SettingsResponse Settings { get; set; }
    }
    
    public class InitApiResponseV2 : BaseInitApiResponse
    {
        public SettingsResponseV2 Settings { get; set; }

        public InitApiResponseV2(InitApiResponse response)
        {
            Customer = response.Customer;
            Currency = response.Currency;
            Location = response.Location;
            Dadata = response.Dadata;
            Contact = response.Contact;
            Carousel = response.Carousel;
            Products = response.Products;
            Stories = response.Stories;
            Settings = new SettingsResponseV2(response.Settings);
            OptionalSettings = response.OptionalSettings;
        }
    }
}