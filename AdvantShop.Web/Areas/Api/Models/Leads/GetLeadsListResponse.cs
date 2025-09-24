using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Customers;
using AdvantShop.Localization;
using AdvantShop.Web.Infrastructure.Api;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Leads
{
    public sealed class GetLeadsListResponse: EntitiesFilterResult<LeadApi>, IApiResponse
    {
    }
    
    public sealed class LeadApi
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string DateFormatted => Culture.ConvertDate(Date);

        public float Sum { get; set; }

        public string SumFormatted => 
            PriceFormatService.FormatPrice(Sum, CurrencyValue, CurrencySymbol, CurrencyCode, IsCodeBefore);
        
        public string CustomerComment { get; set; }
        public string AdminComment { get; set; }
        
        public int DealStatusId { get; set; }
        public string DealStatusName { get; set; }
        public int SalesFunnelId { get; set; }
        public string SalesFunnelName { get; set; }
        
        public int SourceId { get; set; }

        public Guid? CustomerId { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        
        public CustomerType? CustomerType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<CustomerFieldWithValueApi> CustomerFields { get; set; }

        public int? ManagerId { get; set; }
        public string ManagerName { get; set; }
        
        
        public float ProductsSum { get; set; }
        public float ProductsCount { get; set; }


        [JsonIgnore]
        public string CurrencyCode { get; set; }
        
        [JsonIgnore]
        public float CurrencyValue { get; set; }
        
        [JsonIgnore]
        public string CurrencySymbol { get; set; }
        
        [JsonIgnore]
        public bool IsCodeBefore { get; set; }

        
        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<LeadItemApi> LeadItems { get; set; }
    }
    
    public sealed class LeadItemApi
    {
        public int LeadItemId { get; }
        
        public int? ProductId { get; }
        
        public string Name { get; }
        
        public string ArtNo { get; }

        public float Price { get; }

        public float Amount { get; }

        public string Color { get; }

        public string Size { get; }

        public string BarCode { get; }

        public float Length { get; }
        public float Width { get; }
        public float Height { get; }
        public float Weight { get; }

        public string CustomOptionsJson { get; set; }

        public LeadItemApi(LeadItem item)
        {
            LeadItemId = item.LeadItemId;
            ProductId = item.ProductId;
            Name = item.Name;
            ArtNo = item.ArtNo;
            Amount = item.Amount;
            Price = item.Price;
            Color = item.Color;
            Size = item.Size;
            Weight = item.Weight;
            Width = item.Width;
            Length = item.Length;
            Height = item.Height;
            BarCode = item.BarCode;
        }
    }

    public sealed class CustomerFieldWithValueApi
    {
        public int Id { get; }
        
        public string Name { get; }
        
        public string FieldType { get; }
        
        public string Value { get; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<KeyValuePair<string, string>> Values { get; }

        public CustomerFieldWithValueApi(CustomerFieldWithValue f)
        {
            Id = f.Id;
            Name = f.Name;
            FieldType = f.FieldType.ToString();
            Value = f.Value;

            var values = f.Values.Select(x => new KeyValuePair<string, string>(x.Text, x.Value)).ToList();
            Values = values.Count != 0 ? values : null;
        }
    }
}