using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


///<Summary>
/// HidWatcher2 Code.cs
///</Summary>

namespace Conductor.Devices.BarcodeScanner
{

  public  class Code
    {
      public string TextData { get; set; }
      public Symbology Symbology { get; set; }
      public bool PartialSegment { get; set; }
      private DateTime _ScanTime;
      public DateTime ScanTime { get {return _ScanTime;} }
      
      private Code() { }

      private Code(DateTime newScanTime) 
      {
          this._ScanTime = newScanTime;
      }

      public Code(string TextData, Symbology Symbology)
      {
          this.TextData = TextData;
          this.Symbology = Symbology;
          this.PartialSegment = false;
          this._ScanTime = DateTime.Now;
      }

      public Code(string TextData, Symbology Symbology, bool PartialSegment)
      {
          this.TextData = TextData;
          this.Symbology = Symbology;
          this.PartialSegment = PartialSegment;
          this._ScanTime = DateTime.Now;
      }
      
   
  }
    
}
