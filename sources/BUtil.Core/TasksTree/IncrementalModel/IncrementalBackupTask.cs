﻿using BUtil.Core.BackupModels;
using BUtil.Core.Events;
using BUtil.Core.Localization;
using BUtil.Core.Logs;
using BUtil.Core.Options;
using BUtil.Core.TasksTree.Apps;
using BUtil.Core.TasksTree.Core;
using BUtil.Core.TasksTree.States;
using System.Collections.Generic;
using System.Threading;

namespace BUtil.Core.TasksTree.IncrementalModel
{
    class IncrementalBackupTask : SequentialBuTask
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly IncrementalBackupModelOptions _incrementalBackupModelOptions;
#pragma warning restore IDE0052 // Remove unread private members

        public IncrementalBackupTask(ILog log, BackupEvents backupEvents, BackupTask backupTask,
            IncrementalBackupModelOptions incrementalBackupModelOptions)
            : base(log, backupEvents, Resources.IncrementalBackup, TaskArea.ProgramInRunBeforeAfterBackupChain, null)
        {
            var tasks = new List<BuTask>();

            foreach (var executeBeforeBackup in backupTask.ExecuteBeforeBackup)
            {
                tasks.Add(new ExecuteProgramTask(log, backupEvents, executeBeforeBackup));
            }

            var readSatesTask = new GetStateOfSourceItemsAndStoragesTask(Log, Events, backupTask.Items, backupTask.Storages, backupTask.FileExcludePatterns, backupTask.Password);
            tasks.Add(readSatesTask);

            foreach (var storageStateTask in readSatesTask.StorageStateTasks)
            {
                tasks.Add(new WriteIncrementedVersionTask(Log, Events, storageStateTask, readSatesTask.GetSourceItemStateTasks, backupTask.Model as IncrementalBackupModelOptions, backupTask.Password));
            }

            foreach (var executeAfterBackup in backupTask.ExecuteAfterBackup)
            {
                tasks.Add(new ExecuteProgramTask(log, backupEvents, executeAfterBackup));
            }
            Children = tasks;
            _incrementalBackupModelOptions = incrementalBackupModelOptions;
        }

        public override void Execute(CancellationToken token)
        {
            UpdateStatus(ProcessingStatus.InProgress);

            base.Execute(token);
            UpdateStatus(IsSuccess ? ProcessingStatus.FinishedSuccesfully : ProcessingStatus.FinishedWithErrors);
        }
    }
}
