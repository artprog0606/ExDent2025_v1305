using AdvantShop.Configuration;
using AdvantShop.Shipping;
using System.Collections.Generic;
using System.Linq;
using System;
using AdvantShop.Core;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Customers;

namespace AdvantShop.Payment
{
    public class PaymentManager
    {
        private readonly PaymentCalculationParameters _calculationParameters;
        protected bool PaymentOptionFromParameters;

        public PaymentManager(Func<IConfiguratorPaymentCalculation, PaymentCalculationParameters> paymentCalculationConfiguration) 
            : this(paymentCalculationConfiguration(PaymentCalculationConfigurator.Configure()))
        {
        }

        public PaymentManager(PaymentCalculationParameters calculationParameters)
        {
            _calculationParameters = calculationParameters;
        }

        public PaymentManager PreferPaymentOptionFromParameters()
        {
            PaymentOptionFromParameters = true;
            return this;
        }

        public PaymentManager WithoutPreferPaymentOptionFromParameters()
        {
            PaymentOptionFromParameters = false;
            return this;
        }

        public List<BasePaymentOption> GetOptions()
        {
            var options = new List<BasePaymentOption>();

            if (_calculationParameters.ShippingOption == null)
                return options;

            var listMethods = PaymentService.GetAllPaymentMethods(true);
            if (listMethods.Any(x => x.ModuleStringId.IsNotEmpty()))
            {
                var activePaymentModules =
                    AttachedModules.GetModules<Core.Modules.Interfaces.IPaymentMethod>()
                                   .Where(module => module != null)
                                   .Select(module => (Core.Modules.Interfaces.IPaymentMethod) Activator.CreateInstance(module))
                                   .Select(module => module.ModuleStringId)
                                   .ToList();

                listMethods =
                    listMethods
                       .Where(x => x.ModuleStringId.IsNullOrEmpty() || activePaymentModules.Contains(x.ModuleStringId))
                       .ToList();
            }

            listMethods = GetAvailableSaasMethods(listMethods);
            
            var preCoast = _calculationParameters.ItemsTotalPriceWithDiscounts + (_calculationParameters.ShippingOption?.FinalRate ?? 0f);

            var displayCertificateMethod =
                SettingsCheckout.EnableGiftCertificateService
                && _calculationParameters.Certificate != null
                && (SettingsCertificates.ShowCertificatePaymentMetodOnlyCoversSum == false
                    || _calculationParameters.ItemsTotalPriceWithDiscounts + (_calculationParameters.ShippingOption?.FinalRate ?? 0f) <= 0);
            if (displayCertificateMethod)
            {
                var certificateMethod = listMethods.FirstOrDefault(method => method is IPaymentGiftCertificate);
                if (certificateMethod == null)
                {
                    var paymentGiftCertificate = AdvantshopConfigService
                                        .GetDropdownPayments()
                                        .Select(model => model.Value)
                                        .Select(PaymentMethod.Create)
                                        .FirstOrDefault(method => method is IPaymentGiftCertificate) as IPaymentGiftCertificate;
                    if (paymentGiftCertificate is null)
                        paymentGiftCertificate =
                            AttachedModules.GetModules<Core.Modules.Interfaces.IPaymentMethod>()
                                           .Where(module => module != null)
                                           .Select(module => (Core.Modules.Interfaces.IPaymentMethod) Activator.CreateInstance(module))
                                           .Select(module => module.PaymentKey)
                                           .Select(PaymentMethod.Create)
                                           .FirstOrDefault(method => method is IPaymentGiftCertificate) as IPaymentGiftCertificate;

                    if (paymentGiftCertificate != null)
                    {
                        certificateMethod = paymentGiftCertificate.GetDefaultPaymentCertificate();
                        PaymentService.AddPaymentMethod(certificateMethod);
                    }
                }

                if (certificateMethod != null)
                {
                    var certificatePaymentOption = certificateMethod.GetOption(_calculationParameters.ShippingOption, preCoast, _calculationParameters.CustomerType);
                    if (certificatePaymentOption != null)
                    {
                        options.Add(certificatePaymentOption);
                        return options;
                    }
                }
            }
            
            var availableMethod= GetAvailableMethods(listMethods);

            foreach (var paymentMethod in availableMethod)
            {
                if (paymentMethod is IPaymentGiftCertificate) continue;

                var preCoastInPaymentCurrency =
                    preCoast.ConvertCurrency(CurrencyService.CurrentCurrency,
                        paymentMethod.PaymentCurrency ?? CurrencyService.CurrentCurrency);

                if (paymentMethod is ICreditPaymentMethod creditPaymentMethod 
                    && creditPaymentMethod.ActiveCreditPayment 
                    && (creditPaymentMethod.MinimumPrice > preCoastInPaymentCurrency
                        || creditPaymentMethod.MaximumPrice < preCoastInPaymentCurrency)) continue;

                if (ShippingMethodService.IsPaymentNotUsed(_calculationParameters.ShippingOption.MethodId, paymentMethod.PaymentMethodId))
                    continue;

                options.Add(paymentMethod.GetOption(_calculationParameters.ShippingOption, preCoast, _calculationParameters.CustomerType));
            }
            options = options
                     .Where(x => x != null)
                     .Where(x => _calculationParameters.ShippingOption.AvailablePayment(x))
                     .ToList();

            return options;
        }

        private List<PaymentMethod> GetAvailableSaasMethods(List<PaymentMethod> listMethods)
        {
            if (Saas.SaasDataService.IsSaasEnabled
                && Saas.SaasDataService.CurrentSaasData.AvailablePaymentTypesList?.Count > 0)
            {
                listMethods = listMethods
                             .Where(method =>
                                  method.ModuleStringId.IsNullOrEmpty()
                                  && Saas.SaasDataService.CurrentSaasData.AvailablePaymentTypesList
                                      .Contains(method.PaymentKey, StringComparer.OrdinalIgnoreCase))
                             .ToList();
            }

            return listMethods;
        }

        protected virtual List<PaymentMethod> GetAvailableMethods(List<PaymentMethod> listMethods)
        {
            var customerType = _calculationParameters.CustomerType;
            if (customerType == null) 
                customerType = SettingsCustomers.IsRegistrationAsLegalEntity
                                ? SettingsCustomers.IsRegistrationAsPhysicalEntity
                                    ? CustomerType.All
                                    : CustomerType.LegalEntity
                                : CustomerType.PhysicalEntity;

            listMethods = listMethods
                         .Where(x => customerType == CustomerType.All || x.CustomerType == (int)customerType || x.CustomerType == (int)CustomerType.All)
                         .ToList();

            var notAvailableByShipping = ShippingMethodService.NotAvailablePayments(_calculationParameters.ShippingOption.MethodId);
            
            return
                PaymentService.UseGeoMapping(listMethods, _calculationParameters.Country, _calculationParameters.City)
                              .Where(x => !notAvailableByShipping.Contains(x.PaymentMethodId))
                              .Where(x => PaymentOptionFromParameters is false || _calculationParameters.PaymentOption == null
                                   || x.PaymentMethodId == _calculationParameters.PaymentOption.Id)
                              .ToList();
        }
    }
}
