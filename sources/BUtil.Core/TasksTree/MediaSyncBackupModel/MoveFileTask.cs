﻿using BUtil.Core.Events;
using BUtil.Core.Localization;
using BUtil.Core.Logs;
using BUtil.Core.TasksTree.Core;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace BUtil.Core.TasksTree.MediaSyncBackupModel
{
    class MoveFileTask : BuTask
    {
        private readonly string _from;
        private readonly string _toFolder;
        private readonly string _transformFileName;
        private readonly string _destinationFileName;

        public MoveFileTask(ILog log, BackupEvents backupEvents, string from, string toFolder, string transformFileName)
            : base(log, backupEvents, string.Empty, TaskArea.File)
        {
            _from = from;
            _toFolder = toFolder;
            _transformFileName = transformFileName;

            _destinationFileName = GetDestinationFileName();

            Title = string.Format(Resources.MoveFileToDestFolder, Path.GetFileNameWithoutExtension(from), _destinationFileName.Substring(toFolder.Length + 1));
        }

        public override void Execute()
        {
            UpdateStatus(ProcessingStatus.InProgress);

            try
            {
                var actualFileName = GetActualDestinationFileName(_destinationFileName);
                var destFolder = Path.GetDirectoryName(actualFileName);
                Directory.CreateDirectory(destFolder);
                File.Move(_from, actualFileName);

                IsSuccess = true;
                UpdateStatus(ProcessingStatus.FinishedSuccesfully);
            }
            catch (Exception ex)
            {
                IsSuccess = false;

                this.LogError(ex.Message);
                UpdateStatus(ProcessingStatus.FinishedWithErrors);
            }
        }

        private static string GetActualDestinationFileName(string destFile)
        {
            var actualDestFile = destFile;
            var id = 0;
            while (File.Exists(actualDestFile))
            {
                id++;
                actualDestFile = Path.Combine(Path.GetDirectoryName(destFile), $"{Path.GetFileNameWithoutExtension(destFile)}_{id}{Path.GetExtension(destFile)}");
            }

            return actualDestFile;
        }

        private string GetDestinationFileName()
        {
            var lastWriteTime = new FileInfo(_from).LastWriteTime;

            var relativePath = ParseString(_transformFileName, lastWriteTime);
            var relativeDir = Path.GetDirectoryName(relativePath);
            foreach (var ch in Path.GetInvalidPathChars())
            {
                relativeDir = relativeDir.Replace(ch, '_');
            }

            var relativeFileName = Path.GetFileName(relativePath);
            foreach (var ch in Path.GetInvalidFileNameChars())
            {
                relativeFileName = relativeFileName.Replace(ch, '_');
            }

            var actualFolder = Path.Combine(_toFolder, relativeDir);
            var destFile = Path.Combine(actualFolder, $"{relativeFileName}{Path.GetExtension(_from)}");

            return destFile;
        }

        private const string RegexIncludeBrackets = @"{(?<Param>.*?)}";

        public static string ParseString(string input, DateTime date)
        {
            return Regex.Replace(input, RegexIncludeBrackets, match =>
            {
                string cleanedString = match.Groups["Param"].Value;
                if (cleanedString.StartsWith("DATE:"))
                {
                    var format = cleanedString.Replace("DATE:", string.Empty);
                    return date.ToString(format, CultureInfo.CurrentUICulture);
                }
                return string.Empty;
            });
        }
    }
}
