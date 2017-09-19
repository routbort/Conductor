using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Conductor.GUI
{
    /// <summary>
    /// Helper class to provide an indexed palette of colors for syntax/word highlightin
    /// </summary>
    public static class ColorHelper
    {
        static string PALETTE = "#FFFFD700,#FF9400D3,#FF00FFFF,#FF7FFF00,#FFD2691E,#FF1E90FF,#FFFFB6C1,#FFFF69B4,#FF7FFFD4,#FFFFE4C4,#FF0000FF,#FF8A2BE2,#FFA52A2A,#FFDEB887,#FF5F9EA0,#FFFF7F50,#FF6495ED,#FFDC143C,#FF00FFFF,#FF008B8B,#FFB8860B,#FFA9A9A9,#FF006400,#FFBDB76B,#FF8B008B,#FF556B2F,#FFFF8C00,#FF9932CC,#FF8B0000,#FFE9967A,#FF8FBC8F,#FF483D8B,#FF2F4F4F,#FF00CED1,#FFFF1493,#FF00BFFF,#FF696969,#FFB22222,#FF228B22,#FFFF00FF,#FFDCDCDC,#FFF8F8FF,#FFDAA520,#FF808080,#FF008000,#FFADFF2F,#FFF0FFF0,#FFCD5C5C,#FF4B0082,#FFF0E68C,#FF7CFC00,#FFFFFACD,#FFADD8E6,#FFF08080,#FFE0FFFF,#FFFAFAD2,#FFD3D3D3,#FF90EE90,#FFFFA07A,#FF20B2AA,#FF87CEFA,#FF778899,#FFB0C4DE,#FF00FF00,#FF32CD32,#FFFAF0E6,#FFFF00FF,#FF800000,#FF66CDAA,#FF0000CD,#FFBA55D3,#FF9370DB,#FF3CB371,#FF7B68EE,#FF00FA9A,#FF48D1CC,#FFC71585,#FF191970,#FFF5FFFA,#FFFFE4E1,#FFFFE4B5,#FFFFDEAD,#FFFDF5E6,#FF808000,#FF6B8E23,#FFFFA500,#FFFF4500,#FFDA70D6,#FFEEE8AA,#FF98FB98,#FFAFEEEE,#FFDB7093,#FFFFEFD5,#FFFFDAB9,#FFCD853F,#FFFFC0CB,#FFDDA0DD,#FF800080,#FFFF0000,#FFBC8F8F,#FF4169E1,#FF8B4513,#FFFA8072,#FFF4A460,#FF2E8B57,#FFA0522D,#FFC0C0C0,#FF87CEEB,#FF6A5ACD,#FF708090,#FF00FF7F,#FF4682B4,#FFD2B48C,#FF008080,#FFD8BFD8,#FFFF6347,#FF40E0D0,#FFEE82EE,#FFF5DEB3,#FF9ACD32";
        static  ColorInfo[] _Colors;

        static ColorHelper()
        {
            string[] colors = PALETTE.Split(new char[] { ',' });
            _Colors = new ColorInfo[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                System.Windows.Media.Color mediacolor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colors[i]);
                ColorInfo colorInfo = new ColorInfo();
                colorInfo.Index = i;
                colorInfo.RGB = colors[i];
                colorInfo.Color = System.Drawing.Color.FromArgb(mediacolor.A, mediacolor.R, mediacolor.G, mediacolor.B);
                _Colors[i] = colorInfo;
            }
    }

        public static int GetColorSpaceLength()
        {
            return _Colors.Length;
        }

        public static System.Drawing.Color GetColor(int index)
        {
            return _Colors[index % _Colors.Length].Color;
        }

        public static ColorInfo GetColorInfo(int index)
        {
            return _Colors[index % _Colors.Length];
        }

        public class ColorInfo
        {
            public System.Drawing.Color Color { get; set; }
            public string RGB { get; set; }
            public int Index { get; set; }

        }



    }
}
