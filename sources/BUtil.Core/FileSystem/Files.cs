using System;
using System.IO;

namespace BUtil.Core.FileSystem
{
    public static class Files
    {
		#region Private fields
		
		static readonly string _ConsoleBackupTool = 
			Path.Combine(Directories.BinariesDir, "butilc.exe");
		
        static readonly string _BugReportFile = 
        	Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "BUtil BUG report.txt");

        #endregion

		public const string LogFilesExtension = ".BUtilLog.html";
		public const string SuccesfullBackupMarkInHtmlLog = "<!-- SUCCESFULL BACKUP -->";
		public const string ErroneousBackupMarkInHtmlLog = "<!-- ERRONEOUS BACKUP -->";

        public static string BugReportFile => _BugReportFile;

        public static string ConsoleBackupTool => _ConsoleBackupTool;
    }
}
