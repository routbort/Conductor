using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///<Summary>
/// HidWatcher2 SpecificDevice
///</Summary>

namespace Conductor.Devices.BarcodeScanner
{

    public static class SpecificDevice
    {

        public static bool IsDeviceInstalled(int VID, int PID, int version = 0, int size = 0)
        {

            foreach (var device in HidDevices.Enumerate())
            {
                if (device.Attributes.ProductId == PID &&
                    device.Attributes.VendorId == VID &&
                    (version == 0 || device.Attributes.Version == version))
                    return true;
            }
            return false;

        }

        public static string IsSpeechMikeInstalled()
        {

            if (SpecificDevice.IsDeviceInstalled(2321, 5274)) return "SpeechMikeII";
            if (SpecificDevice.IsDeviceInstalled(2321, 3100)) return "SpeechMike III";
            return null;

        }


    }
}
