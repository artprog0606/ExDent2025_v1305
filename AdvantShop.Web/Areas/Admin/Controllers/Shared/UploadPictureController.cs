using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Web.Admin.Handlers.Shared.UploadPictures;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Shared
{    
    public partial class PictureUploaderController : BaseAdminController
    {
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadPicture(HttpPostedFileBase file, PhotoType type, int? objId)
        {
            var result = new UploadPicture(file, type, objId).Execute();
            if (result.Result)
            {
                return JsonOk(new { picture = result.Picture, pictureId = result.PictureId });
            }
            else
            {
                return JsonError(T("Admin.Shared.ErrorLoadingImage"));
            }
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadPictureByLink(PhotoType type, int? objId, string fileLink)
        {
            var result = new UploadPictureByLink(type, objId, fileLink).Execute();
            if (result.Result)
            {
                return JsonOk(new { picture = result.Picture, pictureId = result.PictureId });
            }
            else
            {
                return JsonError(T("Admin.Shared.ErrorLoadingImage"));
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeletePicture(int pictureId)
        {
            var result = new DeletePictureHandler(pictureId).Execute();
            if (result.Result)
            {
                return JsonOk(new { picture = result.Picture });
            }
            else
            {
                return JsonError(T("Admin.Shared.ErrorWhileDeletingImage"));
            }
        }
    }
}