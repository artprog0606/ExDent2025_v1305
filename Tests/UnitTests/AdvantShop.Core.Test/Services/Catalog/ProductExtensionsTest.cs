using AdvantShop.Catalog;
using AdvantShop.Core.Services.Catalog;
using NUnit.Framework;

namespace AdvantShop.Core.Test.Services.Catalog
{
    public class ProductExtensionsTest
    {
        [Test]
        [TestCase(1f, 1f, 1f)]
        [TestCase(1f, null, 1f)]
        [TestCase(0f, null, 1f)]
        [TestCase(2f, 3f, 4f)]
        [TestCase(0.5f, null, 0.5f)]
        [TestCase(0.5f, 1.3f, 1.5f)]
        [TestCase(0.2f, 1.2f, 1.2f)] // этот вариант глючит при доставании значения из бд (0.20000002 и 1.200000005)
        public void GetMinAmount(float multiplicity, float? minAmount, float expectedMinAmount)
        {
            var product = new Product()
            {
                Multiplicity = multiplicity,
                MinAmount = minAmount
            };

            var actualMinAmount = product.GetMinAmount();

            Assert.AreEqual(expectedMinAmount, actualMinAmount);
        }
    }
}