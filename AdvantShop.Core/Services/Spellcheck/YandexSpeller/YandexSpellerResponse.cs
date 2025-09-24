using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Spellcheck
{
    public class YandexSpellerResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("pos")]
        public int Pos { get; set; }

        [JsonProperty("row")]
        public int Row { get; set; }

        [JsonProperty("col")]
        public int Col { get; set; }

        [JsonProperty("len")]
        public int Len { get; set; }

        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("s")]
        public List<string> S { get; set; }

    }
}
