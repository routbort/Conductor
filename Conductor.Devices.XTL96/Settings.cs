using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.Devices.XTL96
{
    public class Settings
    {

        public Settings()
        {
          
            this.Printer = @"CAB A4+/600";
            this.ComPort = "COM1";
        // this.FluidXPerceptionOutputPath = @"C:\fluidX Output";
          //  this.ConnectionString = @"Server=SPIDRSQL3\SPIDRSQL3;Database=MDL_Conductor;User Id=MDLSampleManager;Password=jfOiwekd82!jzz!";
        //    this.ConnectionString = @"Server=SPIDRSQL3\SPIDRSQL3;Database=MDL_Conductor;User Id=MDLSampleManager;Password=qwer!@#$asdf1234";
        //    this.OutputPath = @"\\plmfs1\plmlabs\MDL\Automation_Files_DO_NOT_MANIPULATE_THIS_FOLDER\TEST\FluidX_Labeler\OUTPUT";
          //  this.X_Offset_Helix = 0;
          //  this.Y_Offset_Helix = 0;
          //  this.X_Offset_Tissue = 0;
           // this.Y_Offset_Tissue = 0;
        }
        public string Printer { get; set; }
   //    public string FluidXPerceptionOutputPath { get; set; }
        public string ComPort { get; set; }
      //  public string LabelFilePath { get; set; }
       // public string LabelFileTemplate { get; set; }


        //public string ConnectionString { get; set; }
        //public string OutputPath { get; set; }
      //  public int X_Offset_Helix { get; set; }
       // public int Y_Offset_Helix { get; set; }
       // public int X_Offset_Tissue { get; set; }
       // public int Y_Offset_Tissue { get; set; }
    }


}
