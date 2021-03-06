﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///<Summary>
/// HidWatcher2 IHidDevice
///</Summary>

namespace Conductor.Devices.BarcodeScanner
{
    public delegate void InsertedEventHandler();
    public delegate void RemovedEventHandler();

    public enum DeviceMode
    {
        NonOverlapped = 0,
        Overlapped = 1
    }

    public delegate void ReadCallback(HidDeviceData data);
    public delegate void ReadReportCallback(HidReport report);
    public delegate void WriteCallback(bool success);

    public interface IHidDevice : IDisposable
    {
        event InsertedEventHandler Inserted;
        event RemovedEventHandler Removed;

        IntPtr ReadHandle { get; }
        IntPtr WriteHandle { get; }
        bool IsOpen { get; }
        bool IsConnected { get; }
        string Description { get; }
        HidDeviceCapabilities Capabilities { get; }
        HidDeviceAttributes Attributes { get;  }
        string DevicePath { get; }

        bool MonitorDeviceEvents { get; set; }

        void OpenDevice();

        void OpenDevice(DeviceMode readMode, DeviceMode writeMode);
        
        void CloseDevice();

        HidDeviceData Read();

        void Read(ReadCallback callback);

        HidDeviceData Read(int timeout);

        void ReadReport(ReadReportCallback callback);

        HidReport ReadReport(int timeout);
        HidReport ReadReport();

        bool ReadFeatureData(out byte[] data, byte reportId = 0);

        bool ReadProduct(out byte[] data);

        bool ReadManufacturer(out byte[] data);

        bool ReadSerialNumber(out byte[] data);

        void Write(byte[] data, WriteCallback callback);

        bool Write(byte[] data);

        bool Write(byte[] data, int timeout);

        void WriteReport(HidReport report, WriteCallback callback);

        bool WriteReport(HidReport report);

        bool WriteReport(HidReport report, int timeout);

        HidReport CreateReport();

        bool WriteFeatureData(byte[] data);
    }
}
