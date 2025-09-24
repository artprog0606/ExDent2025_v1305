using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using Debug=AdvantShop.Diagnostics.Debug;

namespace AdvantShop.Core.Services.Helpers
{
    public static class FilesStorageService
    {
        private static readonly object ObjSync = new object();
        private static bool _isRun;

        private static bool IsRun
        {
            get
            {
                lock (ObjSync)
                    return _isRun;
            }
            set
            {
                lock (ObjSync)
                    _isRun = value;
            }
        }

        public static void RecalcAttachmentsSizeInBackground()
        {
            if (!IsRun)
                Task.Run(RecalcAttachmentsSize);
        }

        public static bool RecalcAttachmentsSize()
        {
            if (IsRun)
                return false;

            IsRun = true;

            long length = 0;

            try
            {
                var folders = new List<string>
                {
                    FoldersHelper.GetPathAbsolut(FolderType.Pictures),
                    FoldersHelper.GetPathAbsolut(FolderType.UserFiles)
                };

                folders.AddRange(AttachmentService.FolderTypes
                    .Select(folderType => FoldersHelper.GetPathAbsolut(folderType.Value)));

                var sw = new Stopwatch();
                sw.Start();

                length += folders.Where(System.IO.Directory.Exists).Sum(FileHelpers.GetDirectorySize);

                length += FileHelpers.GetFilesInRootDirectory().Sum(x => x.Length);

                sw.Stop();

                SettingsMain.CurrentFilesStorageSize = length;
                SettingsMain.CurrentFilesStorageSwTime = sw.Elapsed.TotalMilliseconds;
                SettingsMain.CurrentFilesStorageLastUpdateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            IsRun = false;

            return true;
        }

        public static void IncrementAttachmentsSize(string fileName)
        {
            SettingsMain.CurrentFilesStorageSize += FileHelpers.GetFileSize(fileName);
        }

        public static void IncrementAttachmentsSize(long size)
        {
            SettingsMain.CurrentFilesStorageSize += size;
        }

        public static void DecrementAttachmentsSize(string fileName)
        {
            SettingsMain.CurrentFilesStorageSize -= FileHelpers.GetFileSize(fileName);
        }

        public static void DecrementAttachmentsSize(long size)
        {
            SettingsMain.CurrentFilesStorageSize -= size;
        }
    }
}
