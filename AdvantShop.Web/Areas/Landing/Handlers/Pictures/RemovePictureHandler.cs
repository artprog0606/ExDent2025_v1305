using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using AdvantShop.App.Landing.Models.Inplace;
using AdvantShop.Diagnostics;

namespace AdvantShop.App.Landing.Handlers.Pictures
{
    public class RemovePictureHandler
    {
        private string _picture;
        private List<PictureParameters> _parameters;

        public RemovePictureHandler(string picture, List<PictureParameters> parameters)
        {
            _picture = picture;
            _parameters = parameters;
        }

        public bool Execute()
        {
            if (string.IsNullOrEmpty(_picture) || 
                _picture.Contains("areas/landing/frontend/images/") || 
                _picture.Contains("areas/landing/templates/") || 
                _picture.Contains("areas/landing/images/") ||
                _picture.StartsWith("pictures/product/") ||
                _picture.StartsWith("http"))
                return true;

            try
            {
                var path = HostingEnvironment.MapPath("~/" + _picture.TrimStart('/'));
                
                if (path != null && File.Exists(path))
                    File.Delete(path);

                if (_parameters != null)
                    foreach (var item in _parameters)
                    {
                        var parameterFileName = _picture.Split('/').Last();
                        var parameterExt = parameterFileName.Split('.').Last();
                        var parameterName = parameterFileName.Replace("." + parameterExt, "");
                        var parameterDirectory = _picture.Replace(parameterFileName, "").TrimStart('/');
                        
                        var parameterPath = HostingEnvironment.MapPath("~/" + parameterDirectory +  parameterName + "_" + item.Postfix + "." + parameterExt);

                        if (parameterPath != null && File.Exists(parameterPath))
                            File.Delete(parameterPath);
                    }
            }
            catch (Exception ex)
            {
                Debug.Log.Error("LandingPage module, RemovePictureHandler", ex);
                return false;
            }

            return true;
        }
    }
}
