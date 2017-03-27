
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Conductor.Devices.XTL96
{
    public static class Printer
    {
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int level, [MarshalAs(UnmanagedType.LPStruct), In] Printer.DOCINFOA di);

        [DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        [StructLayout(LayoutKind.Sequential)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        public static void SendStringToPrinter(string PrinterName, string Data)
        {
            int length = Data.Length;
            IntPtr PtrToData = Marshal.StringToCoTaskMemAnsi(Data);
            int dwWritten;
            IntPtr hPrinter;
            Printer.DOCINFOA di = new Printer.DOCINFOA();
            di.pDocName = "RAW";
            di.pDataType = "RAW";
            if (Printer.OpenPrinter(PrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                if (Printer.StartDocPrinter(hPrinter, 1, di))
                {
                    if (Printer.StartPagePrinter(hPrinter))
                    {
                        Printer.WritePrinter(hPrinter, PtrToData, length, out dwWritten);
                        Printer.EndPagePrinter(hPrinter);
                    }
                    Printer.EndDocPrinter(hPrinter);
                }
                Printer.ClosePrinter(hPrinter);
            }
            Marshal.FreeCoTaskMem(PtrToData);
        }

        public static void SendFileToPrinter(string PrinterName, string Path)
        {
            SendStringToPrinter(PrinterName, File.ReadAllText(Path));
        }
    }


}
