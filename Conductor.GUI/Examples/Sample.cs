using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Conductor.GUI.Examples
{
    public class Sample : IGridCellInformation, INotifyPropertyChanged
    {

        string _CurrentCartesianAddress;
        public string CurrentCartesianAddress
        {
            get { return _CurrentCartesianAddress; }


            set
            {
                if (_CurrentCartesianAddress != value)
                {
                    _CurrentCartesianAddress = value;
                    OnPropertyChanged("CurrentCartesianAddress");
                }

            }
        }

        Color? _GridCellBackColor;
        public Color? GridCellBackColor
        {
            get { return _GridCellBackColor; }


            set
            {
                if (_GridCellBackColor != value)
                {
                    _GridCellBackColor = value;
                    OnPropertyChanged("GridCellBackColor");
                }
            }
        }

        string _GridCellLabel;
        public string GridCellLabel
        {
            get { return _GridCellLabel; }


            set
            {
                if (_GridCellLabel != value)
                {
                    _GridCellLabel = value;
                    OnPropertyChanged("GridCellLabel");
                }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            try
            {
                PropertyChanged?.Invoke(this, e);
            }
            catch
            {
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public bool? Confirmed { get; set; }

        public string tube_side_barcode { get; set; }

        public string tube_bottom_barcode { get; set; }

    }
}
