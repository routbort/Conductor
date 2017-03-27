using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Conductor.GUI
{


    public class CartesianMapper : ICartesianMapper
    {


        const int CHARACTER_COUNT = 26;
        int _ColumnAddressCharacters;
        string _ColumnFormat = null;
        string _AddressRegex = "^" + Mapping.REGEX_COL + Mapping.REGEX_ROW + "$";


        private string GetRowAddressFromIndex(int RowIndex)
        {
            char letter = (char)((int)('A') + RowIndex % CHARACTER_COUNT);
            int repeats = 1 + RowIndex / CHARACTER_COUNT;
            return new String(letter, repeats);
        }

        private string GetColAddressFromIndex(int ColIndex)
        {
            return (ColIndex + 1).ToString(_ColumnFormat);
        }

        void UpdateDimensions()
        {
            _ColumnAddressCharacters = (int)Math.Floor(Math.Log10(ColCount) + 1);
            _ColumnFormat = "D" + _ColumnAddressCharacters.ToString();
        }

        public int RowCount { get; set; }

        int _ColCount;
        public int ColCount
        {
            get { return _ColCount; }
            set
            {
                _ColCount = value;
                UpdateDimensions();
            }

        }

        public int Capacity { get { return RowCount * ColCount; } }

        public Mapping.FillOrder Order { get; set; } = Mapping.FillOrder.TopBottomLeftRight;

        public System.Drawing.Point GetCoordinatesFromOrdinal(int Ordinal)
        {
            if (Ordinal > Capacity - 1) throw new ApplicationException("Capacity of " + RowCount.ToString() + " x " + ColCount.ToString() + " Rack exceeded.");
            int RowIndex, ColIndex;
            switch (Order)
            {
                case Mapping.FillOrder.TopBottomLeftRight: RowIndex = Ordinal % RowCount; ColIndex = Ordinal / RowCount; break;
                case Mapping.FillOrder.LeftRightTopBottom: RowIndex = Ordinal / ColCount; ColIndex = Ordinal % ColCount; break;
                case Mapping.FillOrder.BottomTopRightLeft: RowIndex = (Capacity - Ordinal - 1) % RowCount; ColIndex = (Capacity - Ordinal - 1) / RowCount; break;
                case Mapping.FillOrder.RightLeftBottomTop: RowIndex = (Capacity - Ordinal - 1) / ColCount; ColIndex = (Capacity - Ordinal - 1) % ColCount; break;
                default: throw new ApplicationException("Unhandled mapping enumeration");
            }
            return new System.Drawing.Point(ColIndex, RowIndex);
        }

        public Point GetCoordinatesFromAddress(string p)
        {
          
            string columnString = Mapping.GetColAddressFromAddress(p);
            int col = Convert.ToInt32(columnString) - 1;
            string rowString = Mapping.GetRowAddressFromAddress(p);
            char rowChar = rowString[0];
            int rowStringCharCount = rowString.Length;
            int row = (rowStringCharCount - 1) * CHARACTER_COUNT + (int)rowChar - (int)'A';
            return new Point(col, row);
        }

        public string GetAddressFromOrdinal(int Ordinal)
        {

            string Address = "";
            Point coordinates = GetCoordinatesFromOrdinal(Ordinal);
            string RowAddress = GetRowAddressFromIndex(coordinates.Y);
            string ColAddress = GetColAddressFromIndex(coordinates.X);
            Address = RowAddress + ColAddress;
            return Address;

        }

        public string GetAddressFromCoordinates(int ColIndex, int RowIndex)
        {
            return GetRowAddressFromIndex(RowIndex) + GetColAddressFromIndex(ColIndex);
        }

        public int GetOrdinalFromAddress(string Address)
        {
            Point coordinates = GetCoordinatesFromAddress(Address);
            return GetOrdinalFromCoordinates(coordinates.X, coordinates.Y);
        }

        public int GetOrdinalFromCoordinates(int ColIndex, int RowIndex)

        {
            if (ColIndex < 0 || RowIndex < 0 || ColIndex > ColCount - 1 || RowIndex > RowCount - 1)
                throw new Exception("Attempt to map invalid coordinates. Column:" + ColIndex.ToString() + ", Row:" + RowIndex.ToString());

            switch (Order)
            {
                case Mapping.FillOrder.TopBottomLeftRight:
                    return ColIndex * RowCount + RowIndex;
                case Mapping.FillOrder.LeftRightTopBottom:
                    return RowIndex  * ColCount + ColIndex;
                case Mapping.FillOrder.BottomTopRightLeft:
                    return (ColCount - ColIndex - 1) * RowCount + RowCount - RowIndex - 1;
                case Mapping.FillOrder.RightLeftBottomTop:
                    return (RowCount - RowIndex - 1) * ColCount + ColCount - ColIndex - 1;
                default:
                    throw new Exception("Unhandled mapping mode");
            }

        }

        public bool IsValidAddess(string Address)
        {
            return (Regex.IsMatch(Address, _AddressRegex));
        }
    }
}
