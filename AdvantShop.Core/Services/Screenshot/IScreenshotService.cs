using System.Threading.Tasks;

namespace AdvantShop.Core.Services.Screenshot
{
    public interface IScreenshotService
    {
        Task<string> CreateScreenShotAsync(ScreenshotCreateDto screenShot);
        string CreateScreenShot(ScreenshotCreateDto screenShot);
    }
}
