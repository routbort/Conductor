using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Conductor.GUI

{
    public static class Mapping
    {

        public const string REGEX_ROW = "[A-Z]+";
        public const string REGEX_COL = @"\d+";

        //    public enum FillMode { UseCartesianMapper, SpecifyCellAddresses, Hybrid }


        public enum AutoFillMode { Repack, Preserve }

        public enum FillOrder
        {
            TopBottomLeftRight,
            LeftRightTopBottom,
            //BottomTopLeftRight,
            BottomTopRightLeft,
            //  TopBottomRightLeft,
            //  LeftRightBottomTop,
            //  RightLeftTopBottom,
            RightLeftBottomTop
            //   Manual
        }
        public static string GetRowAddressFromAddress(string RackAddress)
        {
            return Regex.Match(RackAddress, REGEX_ROW).ToString();
        }

        public static string GetColAddressFromAddress(string RackAddress)
        {
            return Regex.Match(RackAddress, REGEX_COL).ToString();
        }
    }
}
