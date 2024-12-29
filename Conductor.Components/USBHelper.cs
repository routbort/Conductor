using System.Management;

namespace Conductor.Components
{
    public static class USBHelper
    {
        #region Public Methods

        public static bool IsSpecificDeviceAvailable(string DeviceID)
        {
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE '%" + DeviceID + "%'"))
            {
                collection = searcher.Get();
                return (collection.Count > 0);
            }
        }

        #endregion Public Methods
    }
}