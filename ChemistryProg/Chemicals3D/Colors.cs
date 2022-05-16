using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace ChemistryProg._3D
{
    static class ColorExtension
    {
        public static Color GetTransparentColor(this Color color)
        {
            return color.GetColorWithAlpha(0);
        }

        public static Color GetSolidColor(this Color color, double v)
        {
            Color result = color;
            result.A = 255;
            result.B = (byte)(result.B * v);
            result.G = (byte)(result.G * v);
            result.R = (byte)(result.R * v);
            return result;
        }

        public static Color GetColorWithAlpha(this Color color, byte alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }
    }
}
