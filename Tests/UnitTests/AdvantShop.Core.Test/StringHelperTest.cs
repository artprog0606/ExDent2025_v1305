using AdvantShop.Helpers;
using NUnit.Framework;

namespace AdvantShop.Core.Test
{
    [TestFixture]
    public class StringHelperTest
    {
        [Test]
        public void ToPuny()
        {
            // relative
            Assert.AreEqual(StringHelper.ToPuny("pages/contacts"), "pages/contacts");
            Assert.AreEqual(StringHelper.ToPuny("/pages/contacts"), "/pages/contacts");

            Assert.AreEqual(StringHelper.ToPuny("test.html"), "test.html");
            Assert.AreEqual(StringHelper.ToPuny("/test.html"), "/test.html");
            //Assert.AreEqual(StringHelper.ToPuny("тест.html"), "тест.html"); // wrong actual
            Assert.AreEqual(StringHelper.ToPuny("/тест.html"), "/тест.html");

            // without http
            Assert.AreEqual(StringHelper.ToPuny("гпмрм.рф"), "xn--c1asakg.xn--p1ai");
            Assert.AreEqual(StringHelper.ToPuny("гпмрм.рф/test"), "xn--c1asakg.xn--p1ai/test");
            Assert.AreEqual(StringHelper.ToPuny("www.гпмрм.рф/тест"), "www.xn--c1asakg.xn--p1ai/тест");

            // with http
            Assert.AreEqual(StringHelper.ToPuny("https://гпмрм.рф/test"), "https://xn--c1asakg.xn--p1ai/test");
            Assert.AreEqual(StringHelper.ToPuny("https://гпмрм.рф/тест"), "https://xn--c1asakg.xn--p1ai/тест");
            Assert.AreEqual(StringHelper.ToPuny("https://www.гпмрм.рф/тест"), "https://www.xn--c1asakg.xn--p1ai/тест");
            Assert.AreEqual(StringHelper.ToPuny("http://гпмрм.рф/test"), "http://xn--c1asakg.xn--p1ai/test");

            // others
            Assert.AreEqual(StringHelper.ToPuny("google.com"), "google.com");
            Assert.AreEqual(StringHelper.ToPuny("www.google.com/search?q=test"), "www.google.com/search?q=test");
            Assert.AreEqual(StringHelper.ToPuny("https://www.google.com/search?q=test"), "https://www.google.com/search?q=test");            
        }
    }
}
