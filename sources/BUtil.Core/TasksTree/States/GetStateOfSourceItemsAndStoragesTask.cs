﻿using BUtil.Core.Events;
using BUtil.Core.Logs;
using BUtil.Core.Options;
using BUtil.Core.TasksTree.Core;
using BUtil.Core.TasksTree.IncrementalModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BUtil.Core.TasksTree.States
{
    internal class GetStateOfSourceItemsAndStoragesTask : ParallelBuTask
    {
        public IEnumerable<GetStateOfSourceItemTask> GetSourceItemStateTasks { get; }
        public GetStateOfStorageTask StorageStateTask { get; }
        public DeleteUnversionedFilesStorageTask DeleteUnversionedFilesStorageTask { get; }

        public GetStateOfSourceItemsAndStoragesTask(
            ILog log,
            BackupEvents events, 
            IEnumerable<SourceItem> sourceItems,
            CommonServicesIoc commonServicesIoc,
            StorageSpecificServicesIoc servicesIoc,
            IEnumerable<string> fileExcludePatterns,
            string password)
            : base(log, events, Localization.Resources.GetStateOfSourceItemsAndStorages, TaskArea.File, null)
        {
            var childTasks = new List<BuTask>();
            var setSourceItemStateTasks = new List<GetStateOfSourceItemTask>();

            foreach (var item in sourceItems)
            {
                var getSourceItemStateTask = new GetStateOfSourceItemTask(log, Events, item, fileExcludePatterns, commonServicesIoc);
                setSourceItemStateTasks.Add(getSourceItemStateTask);
                childTasks.Add(getSourceItemStateTask);
            }
            GetSourceItemStateTasks = setSourceItemStateTasks;

            var getStorageStateTask = new GetStateOfStorageTask(servicesIoc, Events, password);
            StorageStateTask = getStorageStateTask;
            childTasks.Add(getStorageStateTask);

            var deleteUnversionedFilesStorageTask = new DeleteUnversionedFilesStorageTask(servicesIoc, Events, password, getStorageStateTask);
            DeleteUnversionedFilesStorageTask = deleteUnversionedFilesStorageTask;
            childTasks.Add(deleteUnversionedFilesStorageTask);

            Children = childTasks;
        }

        public override void Execute()
        {
            UpdateStatus(ProcessingStatus.InProgress);

            var storageTasksExecuter = new ParallelExecuter(new[] { StorageStateTask }, 1);
            var sourceItemGroupTasks = GetSourceItemStateTasks
                .GroupBy(x => Directory.GetDirectoryRoot(x.SourceItem.Target))
                .Select(x => new SequentialBuTask(new StubLog(), new BackupEvents(), string.Empty, TaskArea.File, x.ToList()))
                .ToList();
            var sourceItemGroupTasksExecuter = new ParallelExecuter(sourceItemGroupTasks, 10);
            
            sourceItemGroupTasksExecuter.Wait();
            storageTasksExecuter.Wait();

            var deleteUnversionedFilesStorageTasksExecuter = new ParallelExecuter(new[] { DeleteUnversionedFilesStorageTask }, 1);
            deleteUnversionedFilesStorageTasksExecuter.Wait();

            IsSuccess = Children.All(x => x.IsSuccess);
            UpdateStatus(IsSuccess ? ProcessingStatus.FinishedSuccesfully : ProcessingStatus.FinishedWithErrors);
        }
    }
}
