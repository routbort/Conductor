using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Conductor.Devices.CABPrinter
{
    public static class CABLabelHelper
    {
        /// <summary>
        /// Temporary stub to help testin, before settings model is finalized.  Remove before production.
        /// </summary>
        public static string TestPrinterIPMark { get { return "10.127.68.220"; } }

        /// <summary>
        /// Temporary stub to help testin, before settings model is finalized.  Remove before production.
        /// </summary>
        public static string TestPrinterIPRoom { get { return "10.127.107.118"; } }
       
        public static string GetAdjustedLabel(string input, float x_offset, float y_offset)
        {

            int splitLoc = input.IndexOf("T:");
            string start = input.Substring(0, splitLoc);
            string ending = input.Substring(splitLoc);
            string full = start + ending;

            // System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\;(\d+\.?\d*)");
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\;(\d+\.?\d*),(\d+\.?\d*)");

            string result = r.Replace(ending, delegate(Match m)
            {
                float currentx = Convert.ToSingle(m.Groups[1].Value.ToString());
                currentx += x_offset;
                float currenty = Convert.ToSingle(m.Groups[2].Value.ToString());
                currenty += y_offset;
                return ';' + currentx.ToString("0.00") + "," + currenty.ToString("0.00");
            });

            return start + result;
        }

        public static string RackLabelTemplate
        {

            get
            {
                throw new ApplicationException("Deprecated");
              
            }
        }

        public static string RackLabelTemplateExample

        {
            get { return @"e IMG;*
; LABEL ->  = C:\Labeler\TEST\Rack Label 3.stc
; CREATED: 10/4/2016 8:09:18 AM, Driver version: 1.0.0.288
m m
zO
J
j 3162131046333028
S l1;6,0,37,42,13
H 75,0,T,R0,B0
O P
T:Text2;9.15,3.99,270,5,1.7,b,k,q125;VAR003
T:Text3;7.06,3.99,270,5,1.7,b,k,q125;VAR004
T:Text4;5.05,3.99,270,5,1.7,b,k,q125;VAR005
T:Text5;3.13,3.99,270,5,1.7,b,k,q125;VAR006
T:Text6;1.27,4.09,270,5,1.7,b,k,q125;VAR001
B:Barcode1;3.03,22.01,0,datamatrix,0.5;VAR007
T:Text7;2.86,2.88,0,3,1.83,k,q215;TEST
T:Text1;12.06,3.99,270,5,1.7,b,q125;VAR002
A 1
"; }

        }

        public static string BlankTemplate { get { return "f" + System.Environment.NewLine; } }

        public static void Print(string input, string IPAddress, int PrintCount = 1, bool FormFeed = false)
        {
            for (int i = 0; i < PrintCount; i++)
                PrintService.PrintString(input, IPAddress);
            if (FormFeed)
                CABLabelHelper.FormFeed(IPAddress);
        }

        public static void FormFeed(string IPAddress)
        {
            Print(BlankTemplate, IPAddress);
        }

        public static string GetLabelData(string template, Dictionary<string, string> dataElements)
        {
            foreach (string find in dataElements.Keys)
                template = template.Replace(find, dataElements[find]);
            return template;
        }

    }

}