﻿using BUtil.Core.Events;
using BUtil.Core.Logs;
using BUtil.Core.Options;
using BUtil.Core.State;
using BUtil.Core.Storages;
using BUtil.Core.TasksTree.Core;
using System.Threading;

namespace BUtil.Core.TasksTree
{
    internal class GetStateOfStorageTask : BuTask
    {
        private readonly BackupTask task;

        public IStorageSettings StorageSettings { get; }
        public IncrementalBackupState StorageState { get; private set; }

        public GetStateOfStorageTask(ILog log, BackupEvents events, IStorageSettings storageSettings, BackupTask task) : 
            base(log, events, string.Format(BUtil.Core.Localization.Resources.GetStateOfStorage, storageSettings.Name), TaskArea.Hdd)
        {
            StorageSettings = storageSettings;
            this.task = task;
        }

        public override void Execute(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            UpdateStatus(ProcessingStatus.InProgress);
            var service = new IncrementalBackupStateService(Log, StorageSettings, task);
            IsSuccess = service.TryRead(token, out var state);
            StorageState = state;
            UpdateStatus(IsSuccess ? ProcessingStatus.FinishedSuccesfully : ProcessingStatus.FinishedWithErrors);
        }
    }
}
