﻿using BUtil.Core.BackupModels;
using BUtil.Core.FileSystem;
using BUtil.Core.Logs;
using BUtil.Core.Misc;
using BUtil.Core.Storages;
using BUtil.Core.TasksTree.IncrementalModel;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace BUtil.Core.State
{
    public class IncrementalBackupStateService
    {
        private readonly ILog log;
        private readonly IStorageSettings storageSettings;
        private readonly StorageSpecificServicesIoc _services;

        public IncrementalBackupStateService(StorageSpecificServicesIoc services)
        {
            this.log = services.Log;
            this.storageSettings = services.StorageSettings;
            _services = services;
        }

        public bool TryRead(string password, out IncrementalBackupState state)
        {
            log.WriteLine(LoggingEvent.Debug, $"Storage \"{storageSettings.Name}\": Reading state");
            using var tempFolder = new TempFolder();

            if (_services.Storage.Exists(IncrementalBackupModelConstants.StorageIncrementedNonEncryptedNonCompressedStateFile))
            {
                log.WriteLine(LoggingEvent.Debug, $"Storage \"{storageSettings.Name}\": Reading non-encrypted non-compressed state");
                var destFile = Path.Combine(tempFolder.Folder, IncrementalBackupModelConstants.StorageIncrementedNonEncryptedNonCompressedStateFile);
                _services.Storage.Download(IncrementalBackupModelConstants.StorageIncrementedNonEncryptedNonCompressedStateFile, destFile);
                using var destFileStream = File.OpenRead(destFile);
                state = JsonSerializer.Deserialize<IncrementalBackupState>(destFileStream);
                return true;
            }

            if (_services.Storage.Exists(IncrementalBackupModelConstants.StorageIncrementalNonEncryptedCompressedStateFile))
            {
                log.WriteLine(LoggingEvent.Debug, $"Storage \"{storageSettings.Name}\": Reading non-encrypted compressed state");
                var destFile = Path.Combine(tempFolder.Folder, IncrementalBackupModelConstants.StorageIncrementalNonEncryptedCompressedStateFile);
                _services.Storage.Download(IncrementalBackupModelConstants.StorageIncrementalNonEncryptedCompressedStateFile, destFile);
                if (!SevenZipProcessHelper.Extract(log, destFile, null, tempFolder.Folder))
                {
                    log.WriteLine(LoggingEvent.Error, $"Storage \"{storageSettings.Name}\": Failed to read state");
                    state = null;
                    return false;
                }

                var uncompressedFile = Path.Combine(tempFolder.Folder, IncrementalBackupModelConstants.StorageIncrementedNonEncryptedNonCompressedStateFile);
                using var uncompressedFileStream = File.OpenRead(destFile);
                state = JsonSerializer.Deserialize<IncrementalBackupState>(uncompressedFileStream);
                return true;
            }

            if (_services.Storage.Exists(IncrementalBackupModelConstants.StorageIncrementalEncryptedCompressedStateFile))
            {
                log.WriteLine(LoggingEvent.Debug, $"Storage \"{storageSettings.Name}\": Reading encrypted compressed state");
                var destFile = Path.Combine(tempFolder.Folder, IncrementalBackupModelConstants.StorageIncrementalEncryptedCompressedStateFile);
                _services.Storage.Download(IncrementalBackupModelConstants.StorageIncrementalEncryptedCompressedStateFile, destFile);
                if (!SevenZipProcessHelper.Extract(log, destFile, password, tempFolder.Folder))
                {
                    log.WriteLine(LoggingEvent.Error, $"Storage \"{storageSettings.Name}\": Failed to read state");
                    state = null;
                    return false;
                }

                var uncompressedFile = Path.Combine(tempFolder.Folder, IncrementalBackupModelConstants.StorageIncrementedNonEncryptedNonCompressedStateFile);
                using var uncompressedFileStream = File.OpenRead(destFile);
                state = JsonSerializer.Deserialize<IncrementalBackupState>(uncompressedFileStream);
                return true;
            }

            state = new IncrementalBackupState();
            return true;
        }

        public StorageFile Write(IncrementalBackupModelOptions incrementalBackupModelOptions, string password, IncrementalBackupState state)
        {
            log.WriteLine(LoggingEvent.Debug, $"Storage \"{storageSettings.Name}\": Writing state");
            _services.Storage.Delete(IncrementalBackupModelConstants.StorageIncrementedNonEncryptedNonCompressedStateFile);
            _services.Storage.Delete(IncrementalBackupModelConstants.StorageIncrementalNonEncryptedCompressedStateFile);
            _services.Storage.Delete(IncrementalBackupModelConstants.StorageIncrementalEncryptedCompressedStateFile);

            using var tempFolder = new TempFolder();
            var jsonFile = Path.Combine(tempFolder.Folder, IncrementalBackupModelConstants.StorageIncrementedNonEncryptedNonCompressedStateFile);
            using (var jsonFileWriter = File.OpenWrite(jsonFile))
                JsonSerializer.Serialize(jsonFileWriter, state, new JsonSerializerOptions { WriteIndented = true });

            var storageFile = new StorageFile
            {
                StorageMethod = GetStorageMethod(incrementalBackupModelOptions, password),
                StorageIntegrityMethod = StorageIntegrityMethod.Sha512
            };

            string fileToUpload;
            if (incrementalBackupModelOptions.DisableCompressionAndEncryption)
            {
                storageFile.StorageRelativeFileName = IncrementalBackupModelConstants.StorageIncrementedNonEncryptedNonCompressedStateFile;
                fileToUpload = jsonFile;
            }
            else
            {
                var encryptionEnabled = !string.IsNullOrWhiteSpace(password);
                storageFile.StorageRelativeFileName = encryptionEnabled ? IncrementalBackupModelConstants.StorageIncrementalEncryptedCompressedStateFile : IncrementalBackupModelConstants.StorageIncrementalNonEncryptedCompressedStateFile;
                fileToUpload = Path.Combine(tempFolder.Folder, storageFile.StorageRelativeFileName);

                if (!SevenZipProcessHelper.CompressFile(log, jsonFile, password, fileToUpload))
                {
                    log.WriteLine(LoggingEvent.Error, $"Storage \"{storageSettings.Name}\": Failed state");
                    return null;
                }
            }

            var uploadResult = _services.Storage.Upload(fileToUpload, storageFile.StorageRelativeFileName);
            storageFile.StorageFileName = uploadResult.StorageFileName;
            storageFile.StorageFileNameSize = uploadResult.StorageFileNameSize;
            storageFile.StorageIntegrityMethodInfo = HashHelper.GetSha512(fileToUpload);

            return storageFile;
        }

        private string GetStorageMethod(IncrementalBackupModelOptions incrementalBackupModelOptions, string password)
        {
            if (incrementalBackupModelOptions.DisableCompressionAndEncryption)
                return StorageMethodNames.Plain;

            if (string.IsNullOrEmpty(password))
                return StorageMethodNames.SevenZip;

            return StorageMethodNames.SevenZipEncrypted;

        }
    }
}
