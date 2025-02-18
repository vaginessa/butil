﻿using BUtil.Core.Events;
using BUtil.Core.Logs;
using BUtil.Core.Options;
using BUtil.Core.State;
using BUtil.Core.TasksTree.Core;
using BUtil.Core.TasksTree.IncrementalModel;
using BUtil.Core.TasksTree.States;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BUtil.Core.TasksTree
{
    internal class GetStateOfSourceItemTask : SequentialBuTask
    {
        private List<GetStateOfFileTask> _getFileStateTasks;
        private readonly IEnumerable<string> _fileExcludePatterns;
        private readonly CommonServicesIoc _commonServicesIoc;

        public SourceItemState SourceItemState { get; private set; } 

        public SourceItem SourceItem { get; }

        public GetStateOfSourceItemTask(ILog log, BackupEvents events, SourceItem sourceItem, IEnumerable<string> fileExcludePatterns, CommonServicesIoc commonServicesIoc) : 
            base(log, events, string.Format(BUtil.Core.Localization.Resources.GetStateOfSourceItem, sourceItem.Target), sourceItem.IsFolder ? TaskArea.Folder : TaskArea.File, null)
        {
            SourceItem = sourceItem;
            this._fileExcludePatterns = fileExcludePatterns;
            _commonServicesIoc = commonServicesIoc;
        }

        public override void Execute()
        {
            UpdateStatus(ProcessingStatus.InProgress);
            var files = new List<string>();
            if (SourceItem.IsFolder)
            {
                
                files = GetDirectoryFiles(SourceItem.Target);

            }
            else
            {
                files.Add(SourceItem.Target);
            }

            _getFileStateTasks = files
                .Select(file => new GetStateOfFileTask(Log, Events, _commonServicesIoc, file))
                .ToList();
            Children = _getFileStateTasks;
            Events.DuringExecutionTasksAdded(Id, Children);

            base.Execute();

            SourceItemState = new SourceItemState(SourceItem,
                _getFileStateTasks
                .Select(x => x.State)
                .ToList());

            UpdateStatus(IsSuccess ? ProcessingStatus.FinishedSuccesfully : ProcessingStatus.FinishedWithErrors);
        }

        private List<string> GetDirectoryFiles(string folder)
        {
            Matcher matcher = new();
            
            foreach (var excludeFilePattern in _fileExcludePatterns)
            {
                var actualPattern = excludeFilePattern;
                if (excludeFilePattern.StartsWith(folder) && (folder.Length + 1) < excludeFilePattern.Length)
                    actualPattern = actualPattern.Substring(folder.Length + 1);
                matcher.AddExclude(actualPattern);
            }
            
            matcher.AddIncludePatterns(new[] { "**/*" });

            return matcher
                .Execute(new DirectoryInfoWrapperEx(new DirectoryInfo(folder)))
                .Files
                .Select(x => new FileInfo(Path.Combine(folder, x.Path)).FullName)
                .ToList();
        }
    }
}
