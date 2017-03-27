///<Summary>
/// HidWatcher2 HidDeviceData
///</Summary>

namespace Conductor.Devices.BarcodeScanner
{
    public class HidDeviceData
    {
        public enum ReadStatus
        {
            Success = 0,
            WaitTimedOut = 1,
            WaitFail = 2,
            NoDataRead = 3,
            ReadError = 4,
            NotConnected = 5
        }

        public HidDeviceData(ReadStatus status)
	    {
		    Data = new byte[] {};
		    Status = status;
	    }

        public HidDeviceData(byte[] data, ReadStatus status)
        {
            Data = data;
            Status = status;
        }

        //public HidDeviceData(byte[] data, ReadStatus status, int MyThread)
        //{
        //    ThreadCount = MyThread;
        //    Data = data;
        //    Status = status;
        //}

        //public int ThreadCount;
        public byte[] Data { get; private set; }
        public ReadStatus Status { get; private set; }
    }
}
