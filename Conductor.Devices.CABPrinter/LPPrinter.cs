
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;

namespace CABPrinter
{
    class LPPrinter
    {
        #region variables

        private string phost;
        private string pqueue;
        private string puser;

        private string errormsg = "";
        private string logfile = "";

      //  private long filesSend;

        private const int pport = 515;			// hard coded LPR/LPD port number

        private string controlfile;				// not really a file but ok
        // Example: "HPC1\nProb\nfdfA040PC1\nUdfA040PC1\nNtimerH.ps\n"
        // H PC1   			=> responsible host (mandatory)
        // P rob 			=> responsible user (mandatory)
        // f dfA040PC1 		=> Print formatted file
        // U dfA040PC1      => Unlink data file (indicates that sourcefile is no longer needed!)
        // N timerH.ps      => Name of source file

        private Queue PrintQueue = new Queue(); // to keep the files to send

        #endregion variables

        #region constructor

        /// <summary>
        /// Constructor for an instance of an printer that can communicate 
        /// with LPR, LPQ and LPRM.
        /// </summary>
        /// <param name="printerName">idem</param>
        /// <param name="queueName">idem</param>
        /// <param name="userName">idem</param>
        internal LPPrinter(string printerName)
        {
            phost = printerName;
            pqueue = "lp";
            puser = "";
            string msg = string.Format("PRINTER: {0} {1} {2}", phost, pqueue, puser);
            WriteLog(msg);
        }

        #endregion constructor

        #region properties

        public string PHost
        {
            get
            {
                return phost;
            }
        }

        public string PQueue
        {
            get
            {
                return pqueue;
            }
            set
            {
                pqueue = value;
            }
        }

        public string PUser
        {
            get
            {
                return puser;
            }
        }

        public string LogFile
        {
            get
            {
                return logfile;
            }
            set
            {
                logfile = value;
                WriteLog("logfile -> " + logfile);
            }
        }

        public string ErrorMsg
        {
            get
            {
                return errormsg;
            }
        }

        public string Status
        {
            get
            {
                int cnt = InternalQueueSize;
                if (cnt > 0)
                {
                    return "Busy.";
                }
                return "Idle.";
            }
        }

        public int InternalQueueSize
        {
            get
            {
                if (PrintQueue != null)
                {
                    return PrintQueue.Count;
                }
                return 0;   // incorrect but ok
            }
        }

     //   public long FilesSend
     //   {
     //       get
     //       {
    //            return filesSend;
    //        }
    //    }


        #endregion properties

        #region Restart

        internal void Restart()
        {
            WriteLog("Restart");
            ProcessRestart();
        }

        /// <summary>
        /// This command starts the printing process if it not already running.
        /// </summary>
        private void ProcessRestart()
        {
            errormsg = "";

            ////////////////////////////////////////////////////////
            /// PREPARE TCPCLIENT STUF
            ///
            TcpClient tc = new TcpClient();
            tc.Connect(phost, pport);
            NetworkStream nws = tc.GetStream();
            if (!nws.CanWrite)
            {
                errormsg = "-1: cannot write to network stream";
                nws.Close();
                tc.Close();
                return;
            }

            ////////////////////////////////////////////////////////
            /// LOCAL VARIABLES
            ///
            const int BUFSIZE = 1024;				// 1KB buffer 
            byte[] buffer = new byte[BUFSIZE];
            byte[] ack = new byte[4];				// for acknowledge
            int cnt;								// for read acknowledge

            ////////////////////////////////////////////////////////
            /// COMMAND: RESTART
			///      +----+-------+----+
			///      | 01 | Queue | LF |
			///      +----+-------+----+
			///      Command code - 1
			///      Operand - Printer queue name
			/// 

            int pos = 0;
            buffer[pos++] = 1;
            for (int i = 0; i < pqueue.Length; i++)
            {
                buffer[pos++] = (byte)pqueue[i];
            }
            buffer[pos++] = (byte)'\n';

            nws.Write(buffer, 0, pos);
            nws.Flush();

            /////////////////////////////////////////////////////////
            /// READ ACK
            cnt = nws.Read(ack, 0, 4);
            if (ack[0] != 0)
            {
                errormsg = "-2: no ACK on RESTART";
                nws.Close();
                tc.Close();
                return;
            }
            nws.Close();
            tc.Close();
        }

        #endregion Restart

        #region LPR


        public void PrintString(string data)

        {
            string filename = Path.GetTempFileName();
            File.WriteAllText(filename, data);
            SendFile(filename, true);
        }


        private void SendFile(string fname, bool del)
        {



            errormsg = "";

            ////////////////////////////////////////////////////////
            /// PREPARE TCPCLIENT
            ///
            TcpClient tc = new TcpClient();

            tc.Connect(phost, pport);

            NetworkStream nws = tc.GetStream();
            if (!nws.CanWrite)
            {
                errormsg = "-20: cannot write to network stream";
                nws.Close();
                tc.Close();
                return;
            }

            ////////////////////////////////////////////////////////
            /// SOME LOCAL VARIABLES
            ///
        	string localhost = Dns.GetHostName();
            int jobID = GetJobId();
            string dname = String.Format("dfA{0}{1}", jobID, localhost);
            string cname = String.Format("cfA{0}{1}", jobID, localhost);
            controlfile = String.Format("H{0}\nP{1}\nf{2}\nU{3}\nN{4}\n",
                                        localhost, puser, dname, dname, Path.GetFileName(fname));

            const int BUFSIZE = 4 * 1024;			// 4KB buffer
            byte[] buffer = new byte[BUFSIZE];		// 
            byte[] ack = new byte[4];				// for the acknowledges
            int cnt;								// for read acknowledge

            ////////////////////////////////////////////////////////
            /// COMMAND: RECEIVE A PRINTJOB
			///      +----+-------+----+
			///      | 02 | Queue | LF |
			///      +----+-------+----+
			///
            int pos = 0;
            buffer[pos++] = 2;
            for (int i = 0; i < pqueue.Length; i++)
            {
                buffer[pos++] = (byte)pqueue[i];
            }
            buffer[pos++] = (byte)'\n';

            nws.Write(buffer, 0, pos);
            nws.Flush();

            /////////////////////////////////////////////////////////
            /// READ ACK
            cnt = nws.Read(ack, 0, 4);
            if (ack[0] != 0)
            {
                errormsg = "-21: no ACK on COMMAND 02.";
                nws.Close();
                tc.Close();
                return;
            }

            /////////////////////////////////////////////////////////
            /// SUBCMD: RECEIVE CONTROL FILE
            ///
            ///      +----+-------+----+------+----+
			///      | 02 | Count | SP | Name | LF |
			///      +----+-------+----+------+----+
			///      Command code - 2
			///      Operand 1 - Number of bytes in control file
			///      Operand 2 - Name of control file
			///
            pos = 0;
            buffer[pos++] = 2;
            string len = controlfile.Length.ToString();
            for (int i = 0; i < len.Length; i++)
            {
                buffer[pos++] = (byte)len[i];
            }
            buffer[pos++] = (byte)' ';
            for (int i = 0; i < cname.Length; i++)
            {
                buffer[pos++] = (byte)cname[i];
            }
            buffer[pos++] = (byte)'\n';

            nws.Write(buffer, 0, pos);
            nws.Flush();

            /////////////////////////////////////////////////////////
            /// READ ACK
            cnt = nws.Read(ack, 0, 4);
            if (ack[0] != 0)
            {
                errormsg = "-22: no ACK on SUBCMD 2";
                nws.Close();
                tc.Close();
                return;
            }

            /////////////////////////////////////////////////////////
            /// ADD CONTENT OF CONTROLFILE
            pos = 0;
            for (int i = 0; i < controlfile.Length; i++)
            {
                buffer[pos++] = (byte)controlfile[i];
            }
            buffer[pos++] = 0;

            nws.Write(buffer, 0, pos);
            nws.Flush();

            /////////////////////////////////////////////////////////
            /// READ ACK
            cnt = nws.Read(ack, 0, 4);
            if (ack[0] != 0)
            {
                errormsg = "-23: no ACK on CONTROLFILE";
                nws.Close();
                tc.Close();
                return;
            }

            /////////////////////////////////////////////////////////
            /// SUBCMD: RECEIVE DATA FILE
            ///
			///      +----+-------+----+------+----+
			///      | 03 | Count | SP | Name | LF |
			///      +----+-------+----+------+----+
			///      Command code - 3
			///      Operand 1 - Number of bytes in data file
			///      Operand 2 - Name of data file
			///
            pos = 0;
            buffer[pos++] = 3;

            FileInfo DataFileInfo = new FileInfo(fname);
            len = DataFileInfo.Length.ToString();

            for (int i = 0; i < len.Length; i++)
            {
                buffer[pos++] = (byte)len[i];
            }
            buffer[pos++] = (byte)' ';
            for (int i = 0; i < dname.Length; i++)
            {
                buffer[pos++] = (byte)dname[i];
            }
            buffer[pos++] = (byte)'\n';

            nws.Write(buffer, 0, pos);
            nws.Flush();

            /////////////////////////////////////////////////////////
            /// READ ACK
            cnt = nws.Read(ack, 0, 4);
            if (ack[0] != 0)
            {
                errormsg = "-24: no ACK on SUBCMD 3";
                nws.Close();
                tc.Close();
                return;
            }

            /////////////////////////////////////////////////////////
            /// ADD CONTENT OF DATAFILE

            // use BinaryReader as print files may contain non ASCII characters.
            //			FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read);
            //        	BinaryReader br = new BinaryReader(fs);
            //        	long totalbytes = 0;
            //            while (br.PeekChar() > -1)
            //            {
            //				int n = br.Read(buffer, 0, BUFSIZE);
            //				totalbytes += n;
            //	            nws.Write(buffer, 0, n);
            //            	nws.Flush();
            //            }
            //			br.Close();
            //			fs.Close();

            // Code Patched
            // thanx to Karl Fleishmann
            long totalbytes = 0;
            int bytesRead = 0;
            FileStream fstream = new FileStream(fname, FileMode.Open);
            while ((bytesRead = fstream.Read(buffer, 0, BUFSIZE)) > 0)
            {
                totalbytes += bytesRead;
                nws.Write(buffer, 0, bytesRead);
                nws.Flush();
            }
            fstream.Close();


            if (DataFileInfo.Length != totalbytes)
            {
                string msg = fname + ": file length error";
                WriteLog(msg);
                // just proceed for now
            }

            // close data file with a 0 ..
            pos = 0;
            buffer[pos++] = 0;
            nws.Write(buffer, 0, pos);
            nws.Flush();

            /////////////////////////////////////////////////////////
            /// READ ACK
            cnt = nws.Read(ack, 0, 4);
            if (ack[0] != 0)
            {
                errormsg = "-25: no ACK on DATAFILE";
                nws.Close();
                tc.Close();
                return;
            }

            nws.Close();
            tc.Close();

            // all printed well
            // should we delete the file?
            if (del) File.Delete(fname);
        }

        #endregion LPR

        #region misc
        /// <summary>
        /// GetCounter returns the next jobid for the LPR protocol
        /// which must be between 0 and 999.
        /// The jobid is incremented every call but will be wrapped to 0 when 
        /// larger than 999. 
        /// </summary>
        /// <returns>next number</returns>
        private int GetJobId()
        {
            // TODO: GetJobID: keep counter in the registry, or use random generator.
            string temp = Environment.GetEnvironmentVariable("TEMP");
            string cntpath = temp + "LprJobId.txt";

            int cnt = 0;
            try
            {
                StreamReader sr = new StreamReader(cntpath);
                cnt = Int32.Parse(sr.ReadLine());
                sr.Close();
            }
            catch	// file doesn't exist
            {
                cnt = 0;
            }
            cnt++;			// next number but 
            cnt %= 1000;	// keep cnt between 0 and 999
            try
            {
                StreamWriter sw = new StreamWriter(cntpath);
                sw.WriteLine("{0}\n", cnt);
                sw.Close();
            }
            catch
            {
            }
            return cnt;
        }

        /// <summary>
        /// WriteLog writes a message with a date to the logfile.
        /// </summary>
        /// <param name="message">string to write in the logfile.</param>
        private void WriteLog(string message)
        {
            if ((logfile != null) && (logfile != ""))
            {
                string msg = DateTime.Now.ToString() + "; " + message;
                StreamWriter sw = new StreamWriter(logfile, true);
                sw.WriteLine(msg);
                sw.Close();
            }
        }
        #endregion misc
    }
}
