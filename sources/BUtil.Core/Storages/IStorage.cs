﻿using System;

namespace BUtil.Core.Storages
{
    public interface IStorage: IDisposable
    {
        IStorageUploadResult Upload(string sourceFile, string relativeFileName);
        void Delete(string relativeFileName);
        void Download(string relativeFileName, string targetFileName);
        bool Exists(string relativeFileName);
        void DeleteFolder(string relativeFolderName);
        string[] GetFolders(string relativeFolderName, string mask = null);
        string Test();
    }
}
