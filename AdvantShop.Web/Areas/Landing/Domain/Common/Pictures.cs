using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AdvantShop.App.Landing.Domain.Common
{
    public class PicturesHelper
    {
        private static List<string> defaultPicturesPaths = new List<string>()
        {
            "frontend/images",
            "images"
        };

        public static bool IsDefaultPicture(string picture)
        {
            return defaultPicturesPaths.Any(x => picture.Contains(picture));
        }
    }
}
