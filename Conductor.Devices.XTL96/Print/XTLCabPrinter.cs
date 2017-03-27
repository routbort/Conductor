using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.Devices.XTL96
{
   public static class XTLCabPrinter
    {

       public static string PrinterName { get; set; }

       private static void CheckInitialized()
       {
           if (PrinterName == null) throw new ApplicationException("Cedrex Printer Name not set");
       }

       public static void Reset()
       {
           CheckInitialized();
           Printer.SendStringToPrinter(PrinterName, "\u001bt");
       }



    }
}
