using System;
using System.Collections.Generic;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;

namespace AdvantShop.Core.Services.Shipping
{
    [Serializable]
    public class ShippingCalculationParameters
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Apartment { get; set; }
        public string Structure { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
        public string Zip { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public BaseShippingOption ShippingOption { get; set; }
        public List<PreOrderItem> PreOrderItems { get; set; }
        public Currency Currency { get; set; }
        
        /// <summary>
        /// Сумма всех товаров с учетом всех скидок
        /// </summary>
        public float ItemsTotalPriceWithDiscounts { get; set; }
        
        /// <summary>
        /// Сумма всех товаров с учетом всех скидок кроме бонусов
        /// </summary>
        public float ItemsTotalPriceWithDiscountsWithoutBonuses { get; set; }
        
        public float? TotalWeight { get; set; }
        public float? TotalLength { get; set; }
        public float? TotalWidth { get; set; }
        public float? TotalHeight { get; set; }

        public bool IsFromAdminArea { get; set; }
        public bool ShowOnlyInDetails { get; set; }
        public Guid? BonusCardId { get; set; }
        public float AppliedBonuses { get; set; }

        public override int GetHashCode()
        {
            var hashCode = 17
                           ^ (City ?? "").GetHashCode()
                           ^ (District ?? "").GetHashCode()
                           ^ (Country ?? "").GetHashCode()
                           ^ (Region ?? "").GetHashCode()
                           ^ (Street ?? "").GetHashCode()
                           ^ (House ?? "").GetHashCode()
                           ^ (Structure ?? "").GetHashCode()
                           ^ (Apartment ?? "").GetHashCode()
                           ^ (Entrance ?? "").GetHashCode()
                           ^ (Floor ?? "").GetHashCode()
                           ^ (Zip ?? "").GetHashCode()
                           ^ (Currency?.Iso3 ?? string.Empty).GetHashCode()
                           ^ (ShippingOption?.Id.GetHashCode() ?? 0)
                           // ^ PaymentOption.Id.GetHashCode()
                           ^ IsFromAdminArea.GetHashCode()
                           ^ (TotalWeight ?? 0).GetHashCode()
                           ^ (TotalLength ?? 0).GetHashCode()
                           ^ (TotalWidth ?? 0).GetHashCode()
                           ^ (TotalHeight ?? 0).GetHashCode();
            
            if (PreOrderItems != null)
                foreach (var preOrderItem in PreOrderItems)
                    hashCode = hashCode ^ preOrderItem.GetHashCode();

            return hashCode;
        }
    }
}