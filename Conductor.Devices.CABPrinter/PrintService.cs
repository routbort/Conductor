using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Net.NetworkInformation;

public static class PrintService
{
    // static ManualResetEvent connectDone;
    // static ManualResetEvent sendDone;

    // public static void PrintData(byte[] data, string IPaddress)
    // {


    //     connectDone = new ManualResetEvent(false);
    //     sendDone = new ManualResetEvent(false);




    //     IPAddress ip = IPAddress.Parse(IPaddress);
    //     IPEndPoint remoteEP = new IPEndPoint(ip, 9100);

    //     TcpClient myClient = new TcpClient();
    //     IAsyncResult result =    myClient.BeginConnect(IPaddress, 9100, new AsyncCallback(connectCallback), myClient);
    //     result.AsyncWaitHandle.WaitOne(1000);
    //     bool connected = myClient.Connected;


    //     return;

    //     Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    //     client.NoDelay = true;
    //     client.LingerState.Enabled = false;
    //     client.ExclusiveAddressUse = true;
    //     client.DontFragment = true;
    //     client.Blocking = true;


    //  //   try
    //  //   {
    //        // client.BeginConnect(remoteEP, new AsyncCallback(connectCallback), client);
    //      client.Connect(remoteEP);

    //       //  connectDone.WaitOne();
    //       //  client.BeginSend(data, 0, data.Length, 0, new AsyncCallback(sendCallback), client);
    //     client.Send(data);
    //       //  sendDone.WaitOne();
    // //    }
    //     //  catch (Exception ex)

    //  //   finally
    // //    {
    //         // Shutdown the client
    //         shutDown(client);
    ////     }
    // }

    //public static void PrintFile(string filename, string IPaddress)
    //{
    //    byte[] source;
    //    using (FileStream fileStream = new FileStream(filename, FileMode.Open))
    //    using (BinaryReader binaryReader = new BinaryReader((Stream)fileStream))
    //        source = binaryReader.ReadBytes(Convert.ToInt32(fileStream.Length));
    //    PrintData(source, IPaddress);
    //}

  //  static List<string> GoodPrinters = new List<string>();


    public static void PrintString(string data, string IPAddress)
    {

        System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
        PingReply pr = ping.Send(IPAddress, 1000);
        if (pr.Status != IPStatus.Success)
            throw new ApplicationException("Unable to connect to printer at " + IPAddress + System.Environment.NewLine + "Confirm it is turned on and connected to the network." +  System.Environment.NewLine +System.Environment.NewLine + "Ping status:" + pr.Status.ToString());



        //if (!GoodPrinters.Contains(IPAddress))
        //{
        //    System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
        //    PingReply pr = ping.Send(IPAddress, 1000);
        //    if (pr.Status == IPStatus.Success)
        //        GoodPrinters.Add(IPAddress);
        //    else
        //        throw new ApplicationException("Unable to connect to printer at " + IPAddress + System.Environment.NewLine + "Ping status:" + pr.Status.ToString());
        //}

        CABPrinter.LPPrinter p = new CABPrinter.LPPrinter(IPAddress);

        try
        {
            p.PrintString(data);
        }
        catch (Exception ex)


        {
            string message = "Unable to connect to printer at " + IPAddress + System.Environment.NewLine + System.Environment.NewLine + "Is this a CAB Printer?  Check setup." + System.Environment.NewLine + System.Environment.NewLine + ex.Message;
            ApplicationException newEx = new ApplicationException(message, ex);
            throw newEx;
        }

        return;



        //try
        //  {
        //      PrintData(Encoding.ASCII.GetBytes(data), IPAddress);
        //  }
        //  catch (Exception ex)
        //  {
        //      System.Windows.Forms.MessageBox.Show("Error encountered:" + ex.Message);

        //  }
    }

    //private static void connectCallback(IAsyncResult ar)
    //{
    //    return;
    //    // Retrieve the socket from the state object.
    //    Socket client = (Socket)ar.AsyncState;

    //    // Complete the connection.
    ////    if (!client.Connected)
    //     //   throw new ApplicationException ("Could not connect to printer at ": )
    //    client.EndConnect(ar);


    //    // Signal that the connection has been made.
    //    connectDone.Set();
    //}

    //private static void sendCallback(IAsyncResult ar)
    //{
    //    // Retrieve the socket from the state object.
    //    Socket client = (Socket)ar.AsyncState;

    //    // Complete sending the data to the remote device.
    //    int bytesSent = client.EndSend(ar);

    //    // Signal that all bytes have been sent.
    //    sendDone.Set();
    //}

    //private static void shutDown(Socket client)
    //{
    //    client.Shutdown(SocketShutdown.Both);
    //    client.Close();
    //}
}