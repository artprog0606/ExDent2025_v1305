using AdvantShop.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdvantShop.Core.Services.Spellcheck
{
    public class SpellService : ISpellService
    {
        public static double SpellCheckCacheTime = HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled ? 0 : 30;
        public const string SpellCheckCachePrefix = "SpellCheck_";

        private ISpellProvaider _spellProvaider;

        public SpellService()
        {
            //for some else spell provaider
            _spellProvaider = new YandexSpeller();
        }

        public string Check(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            return CacheManager.Get(SpellCheckCachePrefix + text, SpellCheckCacheTime,
                () => _spellProvaider.CheckText(text));           
        }
    }
}
