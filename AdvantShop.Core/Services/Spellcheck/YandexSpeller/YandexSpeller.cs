using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AdvantShop.Core.Services.Spellcheck
{
    // Docs: https://tech.yandex.ru/speller/doc/dg/reference/checkText-docpage/

    public class YandexSpeller : ISpellProvaider
    {
        private const string UrlHost = "https://speller.yandex.net/services/spellservice.json";

        //https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        //we can improve https://www.thomaslevesque.com/2018/02/25/better-timeout-handling-with-httpclient/
        private static HttpClient _httpClient = new HttpClient();

        public string CheckText(string misspell)
        {
            var temp = Process(misspell).Result;
            if (temp != null && temp.Any())
            {
                var result = "";
                foreach (var item in temp)
                {
                    result += " " + item.S.First();
                }
                return result.Trim();
            }

            return misspell;
        }

        public async Task<List<YandexSpellerResponse>> Process(string misspell)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(3));

            try
            {
                var response = await _httpClient.GetAsync(UrlHost + "/checkText?text=" + misspell, cts.Token).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var temp = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<YandexSpellerResponse>>(temp);
                    return result;
                }

                Debug.Log.Warn("YandexSpeller. Resource server returned an error. StatusCode " + response.StatusCode);
            }
            catch (TaskCanceledException ex)
            {
                if (!cts.Token.IsCancellationRequested)
                {
                    // Timed Out
                    Debug.Log.Warn(ex);
                }
                else
                {
                    // Cancelled for some other reason
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.Log.Warn(ex);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }
    }
}
