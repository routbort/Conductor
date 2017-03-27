using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Conductor.GUI
{
    public interface IGridCellInformation
    {
        string GridCellLabel { get; }
        Color? GridCellBackColor { get; }
        string CurrentCartesianAddress { get; set; }
    }

    //public interface IGridCellInformation2
    //{
    //    string GridCellLabel { get; }
    //    Color? GridCellBackColor { get; }
    //    List<string> CurrentCartesianAddresses { get; set; }
    //}

}
