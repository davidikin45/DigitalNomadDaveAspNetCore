using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace DND.Common.Testing.Processes
{
    public static class ProcessExtensions
    {
        public static void KillProcessAndChildren(this Process process)
        {
            var children = process.GetChildProcesses();
            foreach (var child in children)
            {
                KillProcessAndChildren(child);
            }

            if (!process.HasExited)
            {
                process.Kill();
            }
        }

        public static IEnumerable<Process> GetChildProcesses(this Process process)
        {
            List<Process> children = new List<Process>();
            ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));

            foreach (ManagementObject mo in mos.Get())
            {
                children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
            }

            return children;
        }
    }
}
