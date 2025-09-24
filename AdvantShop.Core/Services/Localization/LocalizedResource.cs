using System.Collections.Concurrent;
using AdvantShop.Core.Common;

namespace AdvantShop.Core.Services.Localization
{
    public class LocalizedSetPair
    {
        private readonly LazyWithoutExceptionsCashing<ConcurrentDictionary<string, string>> _resources;

        public LocalizedSetPair()
        {
            _resources = new LazyWithoutExceptionsCashing<ConcurrentDictionary<string, string>>(() =>
                LocalizationService.GetResources(this.Culture));
        }
        public string Culture { get; set; }

        public ConcurrentDictionary<string, string> Resources => _resources.Value;
    }

    public class LocalizedResource
    {
        public int Id { get; set; }

        public string LanguageCode { get; set; }

        public string ResourceKey { get; set; }

        public string ResourceValue { get; set; }
    }
}
