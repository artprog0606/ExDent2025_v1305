using AdvantShop.Core.Services.Catalog;
using AdvantShop.Repository.Currencies;
using NUnit.Framework;

namespace AdvantShop.Core.Test.Services.Catalog
{
    public class PriceServiceTest
    {
        [Test]
        [TestCase(11232.76f, 1, 11232.76f, 1f, false, 0.01f, 1f)]
        [TestCase(11232.76f, 6, 67396.56f, 1f, false, 0.01f, 1f)]
        public void RoundPrice_ShouldReturnRoundedPrice(float price, float amount, float expectedPrice,
                                                        float currencyRate, bool enablePriceRounding,
                                                        float currencyRoundNumbers, 
                                                        float baseCurrencyValue)
        {
            var renderingCurrency = new Currency()
            {
                Rate = currencyRate,
                EnablePriceRounding = enablePriceRounding,
                RoundNumbers = currencyRoundNumbers,
            };

            var roundedPrice = PriceService.RoundPrice(price*amount, renderingCurrency, baseCurrencyValue);

            Assert.AreEqual(expectedPrice, roundedPrice);
        }
        
        [Test]
        [TestCase(11232.76f, 1, 11232.76f, 1f, false, 0.01f)]
        [TestCase(11232.76f, 6, 67396.56f, 1f, false, 0.01f)]
        public void SimpleRoundPrice_ShouldReturnRoundedPrice(float price, float amount, float expectedPrice,
                                                                float currencyRate, bool enablePriceRounding,
                                                                float currencyRoundNumbers)
        {
            var renderingCurrency = new Currency()
            {
                Rate = currencyRate,
                EnablePriceRounding = enablePriceRounding,
                RoundNumbers = currencyRoundNumbers,
            };

            var roundedPrice = PriceService.SimpleRoundPrice(price*amount, renderingCurrency);

            Assert.AreEqual(expectedPrice, roundedPrice);
        }
    }
}