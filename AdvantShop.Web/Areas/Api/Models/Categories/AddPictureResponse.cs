using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Categories
{
    public class AddPictureResponse : ApiResponse
    {
        public string Url { get; private set; }

        public AddPictureResponse(string urlPath)
        {
            Url = urlPath;
        }
    }
}