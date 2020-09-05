using System;
using System.Drawing;
using System.Linq;

namespace GtaKeyboardHook.Infrastructure.Helpers
{
    public static class ColorHelper
    {
        public static Color FromRgb(string rgb)
        {
            var codes = rgb.Split(',').Select(x => Int32.Parse(x)).ToArray();
            
            return Color.FromArgb(codes[0], codes[1], codes[2]);
        }

        public static Color FromPixel(uint pixel)
        {
            return Color.FromArgb((int) (pixel & 0x000000FF), (int) (pixel & 0x0000FF00) >> 8,
                (int) (pixel & 0x00FF0000) >> 16);
        }
    }
}