// minimalistic telnet implementation
// conceived by Tom Janssens on 2007/06/06  for codeproject
//
// http://www.corebvba.be



using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;

namespace MinimalisticTelnet
{

    enum Verbs
    {
        WILL = 251,
        WONT = 252,
        DO = 253,
        DONT = 254,
        IAC = 255
    }

    enum Options
    {
        SGA = 3
    }

    public class TelnetConnection
    {

        public delegate void DataReceivedEventHandler(string data);
        public event DataReceivedEventHandler DataReceived;


        TcpClient tcpSocket;

        int TimeOutMs = 100;

        public TelnetConnection(string Hostname, int Port)
        {
            tcpSocket = new TcpClient(Hostname, Port);

        }


        //public static string zGetIPAddress(string NetworkName)
        //{

        //    // Get a list of all network interfaces (usually one per network card, dialup, and VPN connection) 
        //    NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        //    foreach (NetworkInterface network in networkInterfaces)
        //    {
        //        if (network.OperationalStatus == OperationalStatus.Up)
        //        {
        //            // Read the IP configuration for each network 
        //            IPInterfaceProperties properties = network.GetIPProperties();


        //            // Each network interface may have multiple IP addresses 
        //            foreach (IPAddressInformation address in properties.UnicastAddresses)
        //            {
        //                // We're only interested in IPv4 addresses for now 
        //                if (address.Address.AddressFamily != AddressFamily.InterNetwork)
        //                    continue;

        //                // Ignore loopback addresses (e.g., 127.0.0.1) 
        //                if (IPAddress.IsLoopback(address.Address))
        //                    continue;

        //                if (NetworkName.Contains(network.Name))
        //                    return address.Address.ToString();


        //            }
        //        }
        //    }

        //    return null;
        //}

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

        public string Login(string Username, string Password, int LoginTimeOutMs)
        {
            int oldTimeOutMs = TimeOutMs;
            TimeOutMs = LoginTimeOutMs;
            string s = Read();
            if (!s.TrimEnd().EndsWith(":"))
                throw new Exception("Failed to connect : no login prompt");
            WriteLine(Username);

            s += Read();
            if (!s.TrimEnd().EndsWith(":"))
                throw new Exception("Failed to connect : no password prompt");
            WriteLine(Password);

            s += Read();
            TimeOutMs = oldTimeOutMs;
            return s;
        }

        public void WriteLine(string cmd)
        {
            Write(cmd + "\n");
        }

        public void Close()
        {
            tcpSocket.Close();

        }

        public void Write(string cmd)
        {
            if (!tcpSocket.Connected) return;
            byte[] buf = System.Text.ASCIIEncoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
            tcpSocket.GetStream().Write(buf, 0, buf.Length);
        }

        public string Read(int pause = 0, int timeout = 5000)
        {
            System.Threading.Thread.Sleep(pause);
            if (!tcpSocket.Connected) return null;
            StringBuilder sb = new StringBuilder();
            int startTime = Environment.TickCount;

            do
                ParseTelnet(sb);
            while (tcpSocket.Available > 0 || Environment.TickCount - startTime < timeout);


      //      if (DataReceived != null)
   //       DataReceived("TelnetRead:" + sb.ToString());

            return sb.ToString();
        }

        public string ReadWaitForStrings(string input, List<string> options, int Timeout)
        {
            this.WriteLine(input + System.Environment.NewLine);
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

        public bool IsConnected
        {
            get { return tcpSocket.Connected; }
        }

        void ParseTelnet(StringBuilder sb)
        {
            StringBuilder sbLog = new StringBuilder();

            while (tcpSocket.Available > 0)
            {
                int input = tcpSocket.GetStream().ReadByte();
                switch (input)
                {
                    case -1:
                        break;
                    case (int)Verbs.IAC:
                        // interpret as command
                        int inputverb = tcpSocket.GetStream().ReadByte();
                        if (inputverb == -1) break;
                        switch (inputverb)
                        {
                            case (int)Verbs.IAC:
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int)Verbs.DO:
                            case (int)Verbs.DONT:
                            case (int)Verbs.WILL:
                            case (int)Verbs.WONT:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = tcpSocket.GetStream().ReadByte();
                                if (inputoption == -1) break;
                                tcpSocket.GetStream().WriteByte((byte)Verbs.IAC);
                                if (inputoption == (int)Options.SGA)
                                    tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
                                else
                                    tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
                                tcpSocket.GetStream().WriteByte((byte)inputoption);
                                break;
                            default:
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
                        sb.Append((char)input);
                      if ((char) input == '\r')
                      {
                          
                      }
                      else
                          sbLog.Append((char)input);
                        break;
                }
            }
        }
    }
}
