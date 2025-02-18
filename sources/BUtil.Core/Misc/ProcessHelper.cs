﻿using System.Diagnostics;
using System.Text;

namespace BUtil.Core.Misc
{
    public static class ProcessHelper
    {
        public static void Execute(
            string executable,
            string args,
            string workingDirectory,

            bool sendNewLine,
            ProcessPriorityClass processPriority,

            out string stdOutput,
            out string stdError,
            out int returnCode)
        {
            var stdOutputBuilder = new StringBuilder();
            var stdErrorBuilder = new StringBuilder();

            var process = new Process();
            
            process.StartInfo.FileName = executable;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.OutputDataReceived += (s, a) => stdOutputBuilder.AppendLine(a.Data);
            process.ErrorDataReceived += (s, a) => stdErrorBuilder.AppendLine(a.Data);
            if (sendNewLine)
            {
                process.StartInfo.RedirectStandardInput = true;
            }
            process.Start();
            process.PriorityClass = processPriority;
            if (sendNewLine)
            {
                try
                {
                    process.StandardInput.WriteLine();
                }
                catch // process might end here.
                { }
            }
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.WaitForExit();
            if (process.HasExited)
            {
                stdOutput = stdOutputBuilder.ToString();
                stdError = stdErrorBuilder.ToString();
                returnCode = process.ExitCode;
            }
            else
            {
                process.Kill();
                stdOutput = null;
                stdError = "Processed was killed due to cancellation.";
                returnCode = -1;
            }
        }

        public static void ShellExecute(string argument)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = argument,
                    UseShellExecute = true,
                }
            };
            process.Start();
        }
    }
}
