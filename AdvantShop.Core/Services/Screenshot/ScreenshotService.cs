using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.Core.Services.Screenshot
{
    public class ScreenshotService : IScreenshotService
    {
        private const string UrlService = "http://scr.advsrvone.pw:4343/";
        private static readonly HttpClient HttpClient = new HttpClient();

        private async Task<TOut> DoPost<TIn, TOut>(string url, TIn model)
        {
            try
            {
                return await RetryHelper.DoAsync(async () =>
                {
                    var response = await HttpClient.PostAsJsonAsync(url, model).ConfigureAwait(false);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    var result = JsonConvert.DeserializeObject<CommandResult<TOut>>(jsonString);
                    return result.Obj;
                }, returnException: ReturnException.LastException);
            }
            catch (Exception e)
            {
                Debug.Log.Error("ошибка отправки запроса",e);
            }
            return default(TOut);
        }

        public async Task<string> CreateScreenShotAsync(ScreenshotCreateDto screenShot)
        {
            var url = UrlService + "screenshot/create";

            var result = await DoPost<ScreenshotCreateDto, string>(url, screenShot).ConfigureAwait(false);
            return result;
        }

        public string CreateScreenShot(ScreenshotCreateDto screenShot)
        {
            if (screenShot == null || string.IsNullOrEmpty(screenShot.Url) || screenShot.Url.Contains("localhost"))
                return null;

            return RunSyncUtils.RunSync(() => CreateScreenShotAsync(screenShot));
        }


        public void UpdateStoreScreenShot()
        {
            var screenShotDto = new ScreenshotCreateDto()
            {
                Url = SettingsMain.SiteUrl,
                LicKey = SettingsLic.LicKey,
                Height = 200,
                Width = 240,
                CropWidth = 1400,
                CropHeight = 1400,
                ScreenShotName = "store" //DateTime.Now.ToString("yyyyMMddhhmmssfff")
            };

            var screenShot = CreateScreenShot(screenShotDto);

            if (!string.IsNullOrEmpty(screenShot))
                SettingsMain.StoreScreenShot = screenShot;

            screenShotDto.Height = 256;
            screenShotDto.Width = 414;
            screenShotDto.ScreenShotName = "store_middle"; //DateTime.Now.ToString("yyyyMMddhhmmssfff") + "_middle";

            var screenShotMiddle = CreateScreenShot(screenShotDto);

            if (!string.IsNullOrEmpty(screenShotMiddle))
                SettingsMain.StoreScreenShotMiddle = screenShotMiddle;
        }

        public void UpdateStoreScreenShotInBackground()
        {
            Task.Run(() => UpdateStoreScreenShot());
        }

        public void ClearStoreScreenShots()
        {
            SettingsMain.StoreScreenShot = null;
            SettingsMain.StoreScreenShotMiddle = null;
        }

        public void SetStoreScreenShots(string screen)
        {
            SettingsMain.StoreScreenShot = screen;
            SettingsMain.StoreScreenShotMiddle = screen;
        }

        public void UpdateFunnelScreenShot(LpSite site)
        {
            var url = LpService.GetTechUrl(site.Url, "", true);

            var screenShot = CreateScreenShot(new ScreenshotCreateDto()
            {
                Url = url,
                LicKey = SettingsLic.LicKey,
                Height = 200,
                Width = 240,
                CropWidth = 1400,
                CropHeight = 1400,
                ScreenShotName = $"lp-{site.Id}" //DateTime.Now.ToString("yyyyMMddhhmmssfff")
            });

            if (!string.IsNullOrEmpty(screenShot))
                LpSiteService.UpdateScreenShot(site.Id, screenShot);
        }

        public void UpdateFunnelScreenShotInBackground(LpSite site)
        {
            Task.Run(() => UpdateFunnelScreenShot(site));
        }
    }
}
