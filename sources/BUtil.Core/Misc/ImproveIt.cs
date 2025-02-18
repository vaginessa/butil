using System;
using System.IO;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using BUtil.Core.FileSystem;
using BUtil.Core.Localization;

namespace BUtil.Core.Misc
{
    public static class ImproveIt
    {
        public static Action<string> HandleUiError;

        static ImproveIt()
        {
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(
                    unhandledException);
        }

        public static void ProcessUnhandledException(Exception exception)
        { 
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine();
                builder.AppendLine();
                builder.AppendLine("BUtil " + CopyrightInfo.Version + " - Bug report (" + DateTime.Now.ToString("g", CultureInfo.InvariantCulture) + ")");
                builder.AppendLine("Please report about it here: ");
                builder.AppendLine(SupportManager.GetLink(SupportRequest.Homepage));
                builder.AppendLine(exception.Message);
                builder.AppendLine(exception.StackTrace);
                builder.AppendLine(exception.Source);
                
                Exception inner = exception.InnerException;
                if (inner != null)
                {
                    builder.AppendLine(inner.Message);
                    builder.AppendLine(inner.StackTrace);
                    builder.AppendLine(inner.Source);
                }
                
                File.AppendAllText(Files.BugReportFile, builder.ToString());
                
                if (HandleUiError != null)
                    HandleUiError(string.Format(Resources.AnUnexpectedErrorOccuredNNpleaseContactTheDevelopersWithInformationSavedOnYourDesktopInN0NNapplicationWillNowClose, Files.BugReportFile));
            }
            finally
            {
                Application.Exit();
            }
        }

        private static void unhandledException(object sender, UnhandledExceptionEventArgs exception)
        {
            ProcessUnhandledException(exception.ExceptionObject as Exception);
        }
    }
}
