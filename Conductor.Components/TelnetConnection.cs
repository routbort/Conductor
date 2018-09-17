using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;


namespace Conductor.Components
{
    public class TelnetConnection : IDisposable
    {
        private TcpClient tcpSocket;
        private int TimeoutMs = 100;
        public delegate void DataReceivedEventHandler(string data);
        public event DataReceivedEventHandler DataReceived;


        public bool IsConnected
        {
            get
            {
                return tcpSocket.Connected;
            }
        }

        public TelnetConnection(string hostname, int port)
        {
            tcpSocket = new TcpClient(hostname, port);
        }

        ~TelnetConnection()
        {
            Dispose(false);
        }

        public static TelnetConnection GetLocalConnectionOnPort(int port)
        {

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface network in networkInterfaces)
                if (network.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties properties = network.GetIPProperties();
                    foreach (IPAddressInformation address in properties.UnicastAddresses)
                    {
                        if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;
                        try
                        {
                            TelnetConnection tc = new TelnetConnection(address.Address.ToString(), port);
                            return tc;
                        }

                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Could not bind on " + address.Address + ":" + port.ToString() + " - " + ex.Message);
                        }
                    }
                }

            return null;


        }

        public void Close()
        {

            //  this.tcpSocket.GetStream().Close();
            //   this.tcpSocket.Close();


        }
        public string Login(string username, string password, int loginTimeoutMs)
        {
            int oldTimeoutMs = TimeoutMs;
            TimeoutMs = loginTimeoutMs;

            string s = Read();
            if (!s.TrimEnd().EndsWith(":"))
            {
                throw new Exception("Failed to connect : no login prompt");
            }

            WriteLine(username);

            s += Read();
            if (!s.TrimEnd().EndsWith(":"))
            {
                throw new Exception("Failed to connect : no password prompt");
            }

            WriteLine(password);

            s += Read();

            TimeoutMs = oldTimeoutMs;

            return s;
        }

        public void WriteLine(string cmd)
        {
            Write(cmd + "\n");
        }

        public void Write(string cmd)
        {
            if (!tcpSocket.Connected)
            {
                return;
            }

            byte[] buf = ASCIIEncoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
            tcpSocket.GetStream().Write(buf, 0, buf.Length);
        }

        public string Read()
        {
            if (!tcpSocket.Connected)
            {
                return null;
            }

            var sb = new StringBuilder();
            do
            {
                ParseTelnet(sb);
                Thread.Sleep(TimeoutMs);

            } while (tcpSocket.Available > 0);

            return sb.ToString();
        }

        public string ReadLine(int Timeout)
        {
            string current = "";
            if (!tcpSocket.Connected)
            {
                return null;
            }
            DateTime start = DateTime.Now;
            int startTime = Environment.TickCount;
            while (true)
            {
                current += Read();
                if (current != null)

                    if (current.Contains("\r\n")) return current;
                if (Environment.TickCount - startTime > Timeout) return null;
            }

        }

        public string ReadWaitForStrings(string input, List<string> options, int Timeout)
        {
            if (input != null && input != "")
                this.Write(input + System.Environment.NewLine);
            int startTime = Environment.TickCount; string current = "";
            while (true)
            {
                current += Read();
                if (current != null)
                    foreach (string option in options)
                        if (current.Contains(option)) return current;
                if (Environment.TickCount - startTime > Timeout) return null;
            }
        }

        private void ParseTelnet(StringBuilder sb)
        {
            StringBuilder sbLog = new StringBuilder();


            while (tcpSocket.Available > 0)
            {
                int input = tcpSocket.GetStream().ReadByte();
                switch (input)
                {
                    case -1:
                        break;

                    case (int)Verbs.Iac:
                        // interpret as command
                        int inputVerb = tcpSocket.GetStream().ReadByte();
                        if (inputVerb == -1)
                        {
                            break;
                        }

                        switch (inputVerb)
                        {
                            case (int)Verbs.Iac:
                                // literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputVerb);
                                break;

                            case (int)Verbs.Do:
                            case (int)Verbs.Dont:
                            case (int)Verbs.Will:
                            case (int)Verbs.Wont:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = tcpSocket.GetStream().ReadByte();
                                if (inputoption == -1)
                                {
                                    break;
                                }

                                tcpSocket.GetStream().WriteByte((byte)Verbs.Iac);

                                if (inputoption == (int)Options.Sga)
                                {
                                    tcpSocket.GetStream().WriteByte(inputVerb == (int)Verbs.Do ? (byte)Verbs.Will : (byte)Verbs.Do);
                                }
                                else
                                {
                                    tcpSocket.GetStream().WriteByte(inputVerb == (int)Verbs.Do ? (byte)Verbs.Wont : (byte)Verbs.Dont);
                                }

                                tcpSocket.GetStream().WriteByte((byte)inputoption);
                                break;
                        }

                        break;

                    default:

                        if ((char)input == '\n')
                        {
                            if (DataReceived != null)
                                DataReceived(sbLog.ToString());
                            sbLog.Clear();
                        }

                        sbLog.Append((char)input);
                        sb.Append((char)input);
                        break;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {

            if (disposing)
            {
                if (tcpSocket != null)
                {

                    ((IDisposable)tcpSocket).Dispose();
                    tcpSocket = null;
                }
            }
        }

        #region Private Enums

        enum Verbs
        {
            Will = 251,
            Wont = 252,
            Do = 253,
            Dont = 254,
            Iac = 255
        }

        enum Options
        {
            Sga = 3
        }

        #endregion
    }
}