﻿using BUtil.Core.BackupModels;
using BUtil.Core.FileSystem;
using BUtil.Core.Logs;
using BUtil.Core.Misc;
using BUtil.Core.Options;
using BUtil.Core.Storages;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;

namespace BUtil.Core.State
{
    public class IncrementalBackupFileService
    {
        private readonly ILog log;
        private readonly IStorageSettings storageSettings;
        private readonly ProgramOptions programOptions;

        public IncrementalBackupFileService(ILog log, IStorageSettings storageSettings, ProgramOptions programOptions)
        {
            this.log = log;
            this.storageSettings = storageSettings;
            this.programOptions = programOptions;
        }

        public bool Download(CancellationToken cancellationToken, SourceItem sourceItem, StorageFile storageFile, string destinationFolder)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            log.WriteLine(LoggingEvent.Debug, $"Storage \"{storageSettings.Name}\": downloading \"{storageFile.FileState.FileName}\"");
            var storage = StorageFactory.Create(log, storageSettings);

            var sourceItemDiectory = SourceItemHelper.GetSourceItemDirectory(sourceItem);
            var sourceItemRelativeFileName = SourceItemHelper.GetSourceItemRelativeFileName(sourceItemDiectory, storageFile.FileState);
            var destinationFileName = Path.Combine(destinationFolder, sourceItemRelativeFileName);
            var destinationDir = Path.GetDirectoryName(destinationFileName);
            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            if (storageFile.StorageMethod == StorageMethodNames.Plain)
            {
                storage.Download(storageFile, destinationFileName);
            }
            else
            {
                using var tempFolder = new TempFolder();
                var tempArchive = Path.Combine(tempFolder.Folder, "archive.7z");
                storage.Download(storageFile, tempArchive);
                if (!SevenZipProcessHelper.Extract(log, tempArchive, storageFile.StoragePassword, destinationDir, programOptions.ProcessPriority, cancellationToken))
                {
                    log.WriteLine(LoggingEvent.Error, $"Storage \"{storageSettings.Name}\": extracting \"{storageFile.FileState.FileName}\" failed");
                    return false;
                }
            }
            return true;
        }

        public bool Upload(CancellationToken cancellationToken, StorageFile storageFile)
        {
            if (cancellationToken.IsCancellationRequested)
                return false;

            log.WriteLine(LoggingEvent.Debug, $"Upload \"{storageSettings.Name}\": Upload \"{storageFile.FileState.FileName}\"");
            var storage = StorageFactory.Create(log, storageSettings);

            if (storageFile.StorageMethod == StorageMethodNames.Plain)
            {
                storage.Upload(storageFile.FileState.FileName, storageFile.StorageRelativeFileName);
                return true;
            }
            else
            {
                using var tempFolder = new TempFolder();
                var archiveFile = Path.Combine(tempFolder.Folder, "archive.7z");

                var encryptionEnabled = storageFile.StorageMethod == StorageMethodNames.SevenZipEncrypted;
                if (encryptionEnabled)
                {
                    storageFile.StoragePassword = RandomString();
                }

                if (!SevenZipProcessHelper.CompressFile(log,
                    storageFile.FileState.FileName,
                    storageFile.StoragePassword,
                    archiveFile,
                    programOptions.ProcessPriority,
                    cancellationToken))
                {
                    log.WriteLine(LoggingEvent.Error, $"Upload \"{storageSettings.Name}\": Error compressing \"{storageFile.FileState.FileName}\"");
                    return false;
                }
                storage.Upload(archiveFile, storageFile.StorageRelativeFileName);
                return true;
            }
        }

        private static string RandomString()
        {
            using var generator = RandomNumberGenerator.Create();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789&?%$@";
            var bytes = new byte[255];
            generator.GetBytes(bytes);

            var randomChars = bytes
                .Select(x => chars[x % chars.Length])
                .ToArray();
            return new string(randomChars);
        }
    }
}
