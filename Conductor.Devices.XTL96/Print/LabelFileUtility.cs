
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace FluidXTubeLabeler
{
    public class LabelFileUtility
    {
        public LabelFileUtility()
        {
        }

        public static string GetMappedLabelFile(string LabelFilePath, Dictionary<string, string> jobfileReplacements)
        {
            string DataFilePath = Path.GetTempFileName();
            List<string> strs = new List<string>();
            int num = 0;
            byte[] numArray = new byte[] { 13, 10, 100 };
            byte[] numArray1 = new byte[] { 27, 46, 13, 10 };
            byte[] numArray2 = File.ReadAllBytes(LabelFilePath);
            while (true)
            {
                int length = (int)numArray2.Length - 1 - num;
                int num1 = LabelFileUtility.IndexOfBytes(numArray2, numArray, num, length);
                if (num1 == -1)
                {
                    break;
                }
                num1 = num1 + 2;
                int num2 = LabelFileUtility.IndexOfBytes(numArray2, numArray1, num, length) + 4;
                byte[] numArray3 = new byte[num2 - num1];
                Array.Copy(numArray2, num1, numArray3, 0, num2 - num1);
                strs.Add(Convert.ToBase64String(numArray3));
                num = num2 - 2;
            }
            StreamReader streamReader = new StreamReader(LabelFilePath, Encoding.GetEncoding("iso-8859-1"));
            try
            {
                int num3 = 0;
                if (File.Exists(DataFilePath))
                {
                    FileStream fileStream = new FileStream(DataFilePath, FileMode.Truncate);
                    try
                    {
                    }
                    finally
                    {
                        if (fileStream != null)
                        {
                            ((IDisposable)fileStream).Dispose();
                        }
                    }
                }
                while (streamReader.Peek() != -1)
                {
                    string str = streamReader.ReadLine();
                    if (!str.StartsWith("d"))
                    {
                        if (str.Contains("VAR"))
                            foreach (string key in jobfileReplacements.Keys)
                                str = str.Replace(key, jobfileReplacements[key]);
                        LabelFileUtility.WriteText(str, DataFilePath);
                    }
                    else
                    {
                        LabelFileUtility.WriteBinary(strs[num3], DataFilePath);
                        num3++;
                        while (!streamReader.ReadLine().TrimEnd(new char[] { ' ', '\r', '\n' }).EndsWith("\u001b."))
                        {
                        }
                    }
                }
            }
            finally
            {
                if (streamReader != null)
                {
                    ((IDisposable)streamReader).Dispose();
                }
            }

            return DataFilePath;

        }

        private static int IndexOfBytes(byte[] array, byte[] pattern, int startIndex, int count)
        {
            int num = 0;
            int num1 = Array.FindIndex<byte>(array, startIndex, count, (byte b) =>
            {
                num = (b == pattern[num] ? num + 1 : 0);
                return num == (int)pattern.Length;
            });
            return (num1 < 0 ? -1 : num1 - num + 1);
        }

        private static void WriteBinary(string binString, string file)
        {
            FileStream fileStream = new FileStream(file, FileMode.Append);
            try
            {
                BinaryWriter binaryWriter = new BinaryWriter(fileStream);
                try
                {
                    binaryWriter.Write(Convert.FromBase64String(binString));
                }
                finally
                {
                    if (binaryWriter != null)
                    {
                        ((IDisposable)binaryWriter).Dispose();
                    }
                }
            }
            finally
            {
                if (fileStream != null)
                {
                    ((IDisposable)fileStream).Dispose();
                }
            }
        }

        private static void WriteText(string textString, string file)
        {
            FileStream fileStream = new FileStream(file, FileMode.Append);
            try
            {
                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.GetEncoding("iso-8859-1"));
                try
                {
                    streamWriter.WriteLine(textString);
                }
                finally
                {
                    if (streamWriter != null)
                    {
                        ((IDisposable)streamWriter).Dispose();
                    }
                }
            }
            finally
            {
                if (fileStream != null)
                {
                    ((IDisposable)fileStream).Dispose();
                }
            }
        }
    }
}