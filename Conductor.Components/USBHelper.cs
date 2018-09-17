using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace Conductor.Components
{
    public static class USBHelper
    {
        public static bool IsSpecificDeviceAvailable(string DeviceID)
        {
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE '%" + DeviceID + "%'"))
            {
                collection = searcher.Get();
                return (collection.Count > 0);
            }
        }
    }
}
