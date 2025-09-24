using System;
using System.Drawing;
using System.IO;
using System.Web;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Handlers.Shared.Common
{
    public class UploadAvatar
    {
        private readonly Customer _customer;
        private readonly HttpPostedFileBase _file;

        public UploadAvatar(Customer customer, HttpPostedFileBase file)
        {
            _customer = customer;
            _file = file;
        }

        public bool Validate(out string message)
        {
            message = string.Empty;
            if (_file == null || _file.ContentLength <= 0)
            {
                message = "Файл не найден";
                return false;
            }
            if (_file.ContentLength > 5242880)
            {
                message = "Размер файла превышает 5 МБ";
                return false;
            }

            if (!FileHelpers.CheckFileExtensionByType(_file.FileName, EFileType.Image))
            {
                message = "Недопустимое расширение файла";
                return false;
            }

            return true;
        }

        public string Execute()
        {
            string validateMessage;
            if (!Validate(out validateMessage))
                return null;
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Avatar, _customer.Avatar));

            var ext = Path.GetExtension(_file.FileName.Split('?')[0]);
            var newFileName = _customer.Id + "_" + DateTime.Now.ToString("yyMMddhhmmss") + ext;
            var newFilePath = FoldersHelper.GetPathAbsolut(FolderType.Avatar, newFileName);

            try
            {
                using (Image image = Image.FromStream(_file.InputStream, true))
                {
                    FileHelpers.SaveResizePhotoFile(newFilePath, 70, 70, image);
                }

                _customer.Avatar = newFileName;
                CustomerService.UpdateCustomer(_customer);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("UploadAvatar", ex);
                return null;
            }

            return FoldersHelper.GetPath(FolderType.Avatar, _customer.Avatar, false);
        }
    }
}
