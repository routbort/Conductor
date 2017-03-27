using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class InstancePrintService
{
    ManualResetEvent connectDone;
    ManualResetEvent sendDone;

    bool ConnectionCallbackDone = false;

    public void PrintData(byte[] data, string IPaddress)
    {

        connectDone = new ManualResetEvent(false);
        sendDone = new ManualResetEvent(false);

        IPAddress ip = IPAddress.Parse(IPaddress);
        IPEndPoint remoteEP = new IPEndPoint(ip, 9100);



        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //  client.NoDelay = true;
        // client.LingerState.Enabled = false;
        //client.ExclusiveAddressUse = true;
        //   client.DontFragment = true;
        //    client.Blocking = true;


        //   try
        //   {



        IAsyncResult result = client.BeginConnect(IPaddress, 9100, new AsyncCallback(connectCallback), client);
        result.AsyncWaitHandle.WaitOne(500);
        bool connected = client.Connected;
        int waitCount = 0;
        while (waitCount < 5 && !ConnectionCallbackDone)
        {
            waitCount++;
            System.Diagnostics.Debug.WriteLine("Wait " + waitCount.ToString());
            System.Threading.Thread.Sleep(500);
        }

        if (ConnectionCallbackDone)
        {
            System.Diagnostics.Debug.WriteLine("Sending data...");
            client.Send(data);
            System.Diagnostics.Debug.WriteLine("Data sent");
            client.Shutdown(SocketShutdown.Both);
        }
        else
            System.Diagnostics.Debug.WriteLine("Unable to connect");


        client.Close();


    }

    public void PrintFile(string filename, string IPaddress)
    {
        byte[] source;
        using (FileStream fileStream = new FileStream(filename, FileMode.Open))
        using (BinaryReader binaryReader = new BinaryReader((Stream)fileStream))
            source = binaryReader.ReadBytes(Convert.ToInt32(fileStream.Length));
        PrintData(source, IPaddress);
    }


    public void PrintString(string data, string IPAddress)
    {
        string filename = System.IO.Path.GetTempFileName();
        File.WriteAllText(filename, data);
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = @"/C C:\windows\Sysnative\lpr.exe –S " + IPAddress + " –P " + "lp" + " –o l " + filename;
        process.StartInfo = startInfo;
        process.Start();

        return;




        //  System.Windows.Forms.MessageBox.Show("Trying to print to: " + IPAddress);
        try
        {
            PrintData(Encoding.ASCII.GetBytes(data), IPAddress);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show("Error encountered:" + ex.Message);

        }
    }

    private void connectCallback(IAsyncResult ar)
    {
        Socket client = (Socket)ar.AsyncState;

        if (client.Connected)
            client.EndConnect(ar);
        ConnectionCallbackDone = true;
    }

    private void sendCallback(IAsyncResult ar)
    {
        // Retrieve the socket from the state object.
        Socket client = (Socket)ar.AsyncState;

        // Complete sending the data to the remote device.
        int bytesSent = client.EndSend(ar);

        // Signal that all bytes have been sent.
        sendDone.Set();
    }

}