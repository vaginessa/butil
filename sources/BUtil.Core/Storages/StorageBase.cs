﻿using BUtil.Core.Logs;

namespace BUtil.Core.Storages
{
    public abstract class StorageBase<TStorageSettings>: IStorage
	{
        protected StorageBase(ILog log, TStorageSettings settings)
        {
            Log = log;
            Settings = settings;
        }

        protected readonly ILog Log;
        protected readonly TStorageSettings Settings;

        public abstract IStorageUploadResult Upload(string sourceFile, string relativeFileName);
        public abstract string Test();

        public abstract void Delete(string relativeFileName);
        public abstract void DeleteFolder(string relativeFolderName);
        public abstract string[] GetFolders(string relativeFolderName, string mask = null);
        public abstract void Download(string relativeFileName, string targetFileName);
        public abstract bool Exists(string relativeFileName);
        public abstract void Dispose();
    }
}
