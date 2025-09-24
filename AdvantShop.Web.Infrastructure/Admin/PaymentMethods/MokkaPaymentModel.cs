using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("Mokka")]
    public class MokkaPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string StoreId
        {
            get { return Parameters.ElementOrDefault(MokkaTemplate.StoreId); }
            set { Parameters.TryAddValue(MokkaTemplate.StoreId, value.DefaultOrEmpty()); }
        }

        public string SecretKey
        {
            get { return Parameters.ElementOrDefault(MokkaTemplate.SecretKey); }
            set { Parameters.TryAddValue(MokkaTemplate.SecretKey, value.DefaultOrEmpty()); }
        }

        public string MinimumPrice
        {
            get { return Parameters.ElementOrDefault(MokkaTemplate.MinimumPrice, "3000"); }
            set { Parameters.TryAddValue(MokkaTemplate.MinimumPrice, value.TryParseFloat().ToInvariantString()); }
        }

        public string MaximumPrice
        {
            get { return Parameters.ElementOrDefault(MokkaTemplate.MaximumPrice); }
            set { Parameters.TryAddValue(MokkaTemplate.MaximumPrice, value.TryParseFloat(true)?.ToInvariantString() ?? string.Empty); }
        }

        public string NumberOfParts
        {
            get { return Parameters.ElementOrDefault(MokkaTemplate.NumberOfParts, "6"); }
            set { Parameters.TryAddValue(MokkaTemplate.NumberOfParts, value.TryParseFloat().ToInvariantString()); }
        }

        public bool SandboxServer
        {
            get { return Parameters.ElementOrDefault(MokkaTemplate.SandboxServer).TryParseBool(); }
            set { Parameters.TryAddValue(MokkaTemplate.SandboxServer, value.ToString()); }
        }

        public bool SendStatistic
        {
            get { return !Parameters.ElementOrDefault(MokkaTemplate.NotSendStatistic).TryParseBool(); }
            set { Parameters.TryAddValue(MokkaTemplate.NotSendStatistic, (!value).ToString()); }
        }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(StoreId) || string.IsNullOrWhiteSpace(SecretKey))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
