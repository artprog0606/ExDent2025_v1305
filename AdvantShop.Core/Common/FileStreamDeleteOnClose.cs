using System;
using System.IO;
using System.Security.AccessControl;
using AdvantShop.Helpers;
using Microsoft.Win32.SafeHandles;

namespace AdvantShop.Core.Common
{
    public class FileStreamDeleteOnClose : FileStream
    {
        private readonly Action _actionOnDeleted;

        public FileStreamDeleteOnClose(String path, FileMode mode) : base(path, mode)
        {
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, Action actionOnDeleted) : base(path, mode)
        {
            _actionOnDeleted = actionOnDeleted;
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileAccess access) : base(path, mode, access)
        {
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileAccess access, Action actionOnDeleted) : base(path, mode, access)
        {
            _actionOnDeleted = actionOnDeleted;
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share)
        {
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileAccess access, FileShare share, Action actionOnDeleted) : base(path, mode, access, share)
        {
            _actionOnDeleted = actionOnDeleted;
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileAccess access, FileShare share, int bufferSize) : base(path, mode, access, share, bufferSize)
        {
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileAccess access, FileShare share, int bufferSize, Action actionOnDeleted) : base(path, mode, access, share, bufferSize)
        {
            _actionOnDeleted = actionOnDeleted;
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options) : base(path, mode, access, share, bufferSize, options)
        {
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options, Action actionOnDeleted) : base(path, mode, access, share, bufferSize, options)
        {
            _actionOnDeleted = actionOnDeleted;
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) : base(path, mode, access, share, bufferSize, useAsync)
        {
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync, Action actionOnDeleted) : base(path, mode, access, share, bufferSize, useAsync)
        {
            _actionOnDeleted = actionOnDeleted;
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity) : base(path, mode, rights, share, bufferSize, options, fileSecurity)
        {
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity, Action actionOnDeleted) : base(path, mode, rights, share, bufferSize, options, fileSecurity)
        {
            _actionOnDeleted = actionOnDeleted;
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options) : base(path, mode, rights, share, bufferSize, options)
        {
        }

        public FileStreamDeleteOnClose(String path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, Action actionOnDeleted) : base(path, mode, rights, share, bufferSize, options)
        {
            _actionOnDeleted = actionOnDeleted;
        }

        public FileStreamDeleteOnClose(SafeFileHandle handle, FileAccess access) : base(handle, access)
        {
        }

        public FileStreamDeleteOnClose(SafeFileHandle handle, FileAccess access, Action actionOnDeleted) : base(handle, access)
        {
            _actionOnDeleted = actionOnDeleted;
        }

        public FileStreamDeleteOnClose(SafeFileHandle handle, FileAccess access, int bufferSize) : base(handle, access, bufferSize)
        {
        }

        public FileStreamDeleteOnClose(SafeFileHandle handle, FileAccess access, int bufferSize, Action actionOnDeleted) : base(handle, access, bufferSize)
        {
            _actionOnDeleted = actionOnDeleted;
        }

        public FileStreamDeleteOnClose(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) : base(handle, access, bufferSize, isAsync)
        {
        }

        public FileStreamDeleteOnClose(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync, Action actionOnDeleted) : base(handle, access, bufferSize, isAsync)
        {
            _actionOnDeleted = actionOnDeleted;
        }

        public override void Close()
        {
            base.Close();
            FileHelpers.DeleteFile(Name);
            if (_actionOnDeleted != null)
                _actionOnDeleted();
        }
    }
}
