using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;

namespace Conductor.Components
{
    public static class ProcessHelper
    {

       public static Process GetParent(Process process)
        {
            try
            {
                using (var query = new ManagementObjectSearcher(
                  "SELECT * " +
                  "FROM Win32_Process " +
                  "WHERE ProcessId=" + process.Id))
                {
                    return query
                      .Get()
                      .OfType<ManagementObject>()
                      .Select(p => Process.GetProcessById((int)(uint)p["ParentProcessId"]))
                      .FirstOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
