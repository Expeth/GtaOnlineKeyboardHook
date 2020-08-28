using System.Drawing;
using System.Windows.Forms;
using Vanara.PInvoke;

namespace GtaKeyboardHook.Infrastructure
{
    public static class Win32ApiHelper
    {
        public static Color GetPixelColor(int x, int y)
        {
            var window = User32.GetDesktopWindow();

            var hdc = User32.GetDC(window);
            uint pixel = Gdi32.GetPixel(hdc, x, y);

            User32.ReleaseDC(window, hdc);

            var color = Color.FromArgb((int)(pixel & 0x000000FF),
                (int)(pixel & 0x0000FF00) >> 8,
                (int)(pixel & 0x00FF0000) >> 16);

            return color;
        }

        public static void SendKeyPressedEvent(Keys key)
        {
            Sender.SendKeyPressedEvent(key, false);
            Sender.SendKeyPressedEvent(key, true);
        }
    }
}