using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.Devices.XTL96
{
   public static class Commands
    {

     //  public static string SendRackToScanner { get { return "m"; } }

       public static string SendRackToScanner { get { return "u"; } }
      
    //   public static string ReturnRackFromScanner { get { return "o"; } }

       public static string CheckInitializeStatus { get { return "p"; } }

       public static string ResetInitialize { get { return "D";}}

       public static string SendToLoadPosition { get { return "C"; } }

       public static string SendToPrintPosition { get { return "V"; } }


       public static string MoveToRow(int RowIndex)
       
       {
           if (RowIndex < 0 || RowIndex > 7) throw new ApplicationException("Row index out of bounds");
           return ((char)((int)'J' + RowIndex)).ToString() + "";
       }

       public static string MoveToColumn(int ColIndex)
       {
           if (ColIndex < 0 || ColIndex > 11) throw new ApplicationException("Column index out of bounds");
           return ((char)((int)'a' + ColIndex)).ToString() + "";
       }

       public static string MoveToPosition(int ColIndex, int RowIndex)
       {
           return MoveToRow(RowIndex) + MoveToColumn(ColIndex);
       }

       public static string MoveToPosition(string Address)
       {
           char rowChar = Address[0];
           int rowIndex = (int)rowChar - (int)'A';
           string colString = Address.Substring(1);
           int colIndex = Convert.ToInt32(colString) - 1;
           return MoveToPosition(colIndex, rowIndex);
       }


    }
}
