
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace Conductor.Devices.XTL96
{

    public class PLCUtility
    {

        public delegate void MessageReceivedEventHandler(string message);
        public static event MessageReceivedEventHandler MessageReceived;

        private static SerialPort port;

        public static string PortName { get; set; }

        private static List<PLCResponse> pLCResponseBuffer;

        private static List<PLCResponse> PLCResponseBuffer
        {
            get
            {
                List<PLCResponse> pLCResponses = PLCUtility.pLCResponseBuffer;
                if (pLCResponses == null)
                {
                    pLCResponses = new List<PLCResponse>();
                    PLCUtility.pLCResponseBuffer = pLCResponses;
                }
                return pLCResponses;
            }
            set
            {
                PLCUtility.pLCResponseBuffer = value;
            }
        }

        public PLCUtility()
        {
        }

        public static void ClearBuffer()
        {
            PLCUtility.PLCResponseBuffer = new List<PLCResponse>();
        }

        public static bool GetPLCResponse(PLCResponseType expectedResponse, int maxNoOfBufferedMessages, out string errorMessage)
        {
            return PLCUtility.GetPLCResponse(expectedResponse, maxNoOfBufferedMessages, out errorMessage, 30);
        }

        public static bool GetPLCResponse(PLCResponseType expectedResponse, int maxNoOfBufferedMessages, out string errorMessage, int timeout)
        {
            PLCResponse item;
            bool flag;
            errorMessage = "";
            if (!PLCUtility.WaitForPLCResponse(timeout))
            {
                errorMessage = string.Concat("PLC has not responded within a timeout period of ", timeout, " seconds");
                flag = false;
            }
            else if (PLCUtility.PLCResponseBuffer.Count <= maxNoOfBufferedMessages)
            {
                lock (PLCUtility.PLCResponseBuffer)
                {
                    item = PLCUtility.PLCResponseBuffer[0];
                    PLCUtility.PLCResponseBuffer.RemoveAt(0);
                }
                if (item.PLCResponseType == expectedResponse)
                {
                    flag = true;
                }
                else
                {
                    errorMessage = string.Concat("Response from PLC was not as expected. Actual response = \"", item.Message, "\" expected type of response = ", expectedResponse.ToString("g"));
                    flag = false;
                }
            }
            else
            {
                object[] count = new object[] { "PLC has sent an unexpected no. of messages. Messages in buffer = ", PLCUtility.PLCResponseBuffer.Count, " expected no. of messages = ", maxNoOfBufferedMessages };
                errorMessage = string.Concat(count);
                flag = false;
            }
            return flag;
        }

        public static string GetPLCResponseNocheck(int timeout, bool RemoveMessages = true)
        {
            PLCResponse item;
            string msg = "";
            if (!PLCUtility.WaitForPLCResponse(timeout))
                msg = string.Concat("PLC has not responded within a timeout period of ", timeout, " seconds");
            else
                lock (PLCUtility.PLCResponseBuffer)
                    while (PLCUtility.PLCResponseBuffer.Count > 0)
                    {
                        item = PLCUtility.PLCResponseBuffer[0];

                        msg += item.Message + System.Environment.NewLine;

                        if (RemoveMessages)
                            PLCUtility.PLCResponseBuffer.RemoveAt(0);
                        else
                            break;
                    }


            return msg;
        }



        private static void InitPort()
        {
            PLCUtility.port = new SerialPort(PortName, 9600, Parity.None, 8, StopBits.One);
            PLCUtility.port.DataReceived += new SerialDataReceivedEventHandler(PLCUtility.PortDataReceived);
            PLCUtility.port.Open();
        }

        private static void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (PLCUtility.port.BytesToRead > 0)
            {
                string str = PLCUtility.port.ReadTo("\r");
                if (PLCUtility.PLCResponseBuffer == null)
                {
                    PLCUtility.PLCResponseBuffer = new List<PLCResponse>();
                }
                lock (PLCUtility.PLCResponseBuffer)
                {
                    PLCUtility.PLCResponseBuffer.Add(new PLCResponse(str));
                }

                if (MessageReceived != null)
                    MessageReceived(str);

            }




        }

        public static void SendPlcCommand(string cmd)
        {
                if ((PLCUtility.port == null ? true : PLCUtility.port.PortName != PortName))
                {
                    PLCUtility.InitPort();
                }
                if (!PLCUtility.port.IsOpen)
                {
                    PLCUtility.port.Open();
                }
                Debug.WriteLine(string.Concat("SendPLCCommand: |", cmd, "|"));
                PLCUtility.port.Write(cmd);  
        }

        public static void SendPlcStartCommand()
        {
             PLCUtility.SendPlcCommand( "A");
        }

        private static bool WaitForPLCResponse(int timeoutSeconds)
        {
            int num = timeoutSeconds * 1000;
            int num1 = 10;
            while (true)
            {
                if ((PLCUtility.PLCResponseBuffer.Count != 0 ? true : num <= 0))
                {
                    break;
                }
                Thread.Sleep(num1);
                num = num - num1;
            }
            return PLCUtility.PLCResponseBuffer.Count > 0;
        }
    }
}