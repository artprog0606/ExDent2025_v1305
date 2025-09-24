using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.InplaceEditor;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.App.Landing.Handlers.Inplace
{
    public class InplaceLogo
    {
        public List<object> Execute(List<HttpPostedFileBase> files, ImageInplaceField field,
                                    ImageInplaceCommands command, int? colorId, string additionalData, int id, int objId)
        {
            var result = new List<object>();
            var customer = CustomerContext.CurrentCustomer;

            switch (field)
            {
                case ImageInplaceField.Logo:
                    switch (command)
                    {
                        case ImageInplaceCommands.Add:
                        case ImageInplaceCommands.Update:
                            result.Add(UpdateLogo(files[0]));
                            break;

                        case ImageInplaceCommands.Delete:
                            result.Add(DeleteLogo());
                            break;
                    }
                    break;
            }

            return result;
        }

        private object UpdateLogo(HttpPostedFileBase file)
        {
            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.LogoImageName));
            var newFile = file.FileName.FileNamePlusDate("logo");
            file.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.Pictures, newFile));
            SettingsMain.LogoImageName = newFile;

            try
            {
                var img = System.Drawing.Image.FromStream(file.InputStream, true, true);
                SettingsMain.LogoImageWidth = img.Width;
                SettingsMain.LogoImageHeight = img.Height;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return ReturnItem(FoldersHelper.GetPath(FolderType.Pictures, newFile, false), null);
        }

        private object DeleteLogo()
        {
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.LogoImageName));
            SettingsMain.LogoImageName = string.Empty;
            SettingsMain.LogoImageWidth = 0;
            SettingsMain.LogoImageHeight = 0;

            return ReturnItem("images/nophoto-logo.png", null);
        }

        private object ReturnItem(string filename, int? id)
        {
            return new {id = id, filename = filename};
        }
    }
}
