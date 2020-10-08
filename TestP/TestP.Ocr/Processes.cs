using System;
using System.Diagnostics;

namespace TestP.Ocr
{
    public class Processes
    {
        public static void PrintActiveProcesses()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    Console.WriteLine("Process: {0} ID: {1} Window title: {2}", process.ProcessName, process.Id, process.MainWindowTitle);
                }
            }
        }


        public static Process GetCurrentGameProcess()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle) && process.ProcessName.Contains("DanskeSpil"))
                {
                    return process;
                }                
            }

            return null;
        }
    }
}
