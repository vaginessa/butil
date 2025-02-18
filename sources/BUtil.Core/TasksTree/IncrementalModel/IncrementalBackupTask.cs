﻿using BUtil.Core.BackupModels;
using BUtil.Core.Events;
using BUtil.Core.Localization;
using BUtil.Core.Logs;
using BUtil.Core.Options;
using BUtil.Core.TasksTree.Core;
using BUtil.Core.TasksTree.States;
using System.Collections.Generic;
using System.Linq;

namespace BUtil.Core.TasksTree.IncrementalModel
{
    class IncrementalBackupTask : SequentialBuTask
    {
        private readonly CommonServicesIoc _commonServicesIoc;
        private readonly StorageSpecificServicesIoc _storageService;

        public IncrementalBackupTask(ILog log, BackupEvents backupEvents, BackupTask backupTask)
            : base(log, backupEvents, Resources.IncrementalBackup, TaskArea.ProgramInRunBeforeAfterBackupChain, null)
        {
            var tasks = new List<BuTask>();

            _commonServicesIoc = new CommonServicesIoc();

            var storage = backupTask
                .Storages
                .First();

            _storageService = new StorageSpecificServicesIoc(Log, storage, _commonServicesIoc.HashService);

            var readSatesTask = new GetStateOfSourceItemsAndStoragesTask(Log, Events, backupTask.Items, _commonServicesIoc, _storageService, backupTask.FileExcludePatterns, backupTask.Password);
            tasks.Add(readSatesTask);

            tasks.Add(new WriteIncrementedVersionTask(_storageService, Events, readSatesTask.StorageStateTask, readSatesTask.GetSourceItemStateTasks, backupTask.Model as IncrementalBackupModelOptions, backupTask.Password));

            Children = tasks;
        }

        public override void Execute()
        {
            Events.OnMessage += OnAddLastMinuteLogMessage;
            UpdateStatus(ProcessingStatus.InProgress);
            base.Execute();

            _storageService.Dispose();
            _commonServicesIoc.Dispose();

            UpdateStatus(IsSuccess ? ProcessingStatus.FinishedSuccesfully : ProcessingStatus.FinishedWithErrors);
            Events.OnMessage -= OnAddLastMinuteLogMessage;
            PutLastMinuteLogMessages();
        }

        private void PutLastMinuteLogMessages()
        {
            foreach (var lastMinuteLogMessage in _lastMinuteLogMessages)
                Log.WriteLine(LoggingEvent.Debug, lastMinuteLogMessage);
        }

        private List<string> _lastMinuteLogMessages = new List<string>();
        private void OnAddLastMinuteLogMessage(object sender, MessageEventArgs e)
        {
            _lastMinuteLogMessages.Add(e.Message);
        }
    }
}
