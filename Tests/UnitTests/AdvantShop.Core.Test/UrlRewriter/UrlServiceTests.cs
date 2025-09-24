using System;
using AdvantShop.Core.UrlRewriter;
using NUnit.Framework;

namespace AdvantShop.Core.Test.UrlRewriter
{
    public class UrlServiceTests
    {
        [Test]
        [TestCase("https://www.advantshop.net", 1)]
        [TestCase("http://admin.advantme.pro", 2)]
        [TestCase("http://test.admin.advantme.pro", 3)]
        [TestCase("http://test.com.ru", 1)]
        public void GetDomainLevel_ShouldReturnValidLevel(string url, int expectedLevel)
        {
            var uri = new Uri(url);
            var domainLevel = UrlService.GetDomainLevel(uri);
            Assert.AreEqual(expectedLevel, domainLevel);
        }

        [Test]
        [TestCase("https://www.advantshop.net", false)]
        [TestCase("http://admin.advantme.pro", true)]
        [TestCase("http://test.admin.advantme.pro", true)]
        [TestCase("http://test.com.ru", false)]
        public void IsOnSubdomain_ShouldReturnExpectedValue(string url, bool expected)
        {
            var uri = new Uri(url);
            var isOnSubdomain = UrlService.IsOnSubdomain(uri);
            Assert.AreEqual(expected, isOnSubdomain);
        }
        
        [Test]
        [TestCase("https://www.advantshop.net", "advantshop.net")]
        [TestCase("http://admin.advantme.pro", "advantme.pro")]
        [TestCase("http://test.admin.advantme.pro", "advantme.pro")]
        [TestCase("http://test.com.ru", "test.com.ru")]
        public void RemoveSubdomain_ShouldReturnExpectedValue(string url, string expected)
        {
            var uri = new Uri(url);
            var isOnSubdomain = UrlService.RemoveSubdomain(uri.DnsSafeHost);
            Assert.AreEqual(expected, isOnSubdomain);
        }

        [Test]
        [TestCase("https://www.advantshop.net", null)]
        [TestCase("http://admin.advantme.pro", "admin")]
        [TestCase("http://test.admin.advantme.pro", "test.admin")]
        [TestCase("http://test.com.ru", null)]
        public void GetSubDomain_ShouldReturnExpectedValue(string url, string expected)
        {
            var uri = new Uri(url);
            var subDomain = UrlService.GetSubDomain(uri);
            Assert.AreEqual(expected, subDomain);
        }
    }
}