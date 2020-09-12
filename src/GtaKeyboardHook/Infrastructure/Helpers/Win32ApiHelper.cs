using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Vanara.PInvoke;

namespace GtaKeyboardHook.Infrastructure.Helpers
{
    public static class Win32ApiHelper
    {
        public static Color GetPixelColorFromHdc(HDC windowHdc, int x, int y)
        {
            // UINT representation of a pixel information
            uint pixel = Gdi32.GetPixel(windowHdc, x, y);

            // get color
            try
            {
                return ColorHelper.FromPixel(pixel);
            }
            catch (Exception e)
            {
                return Color.White;
            }
        }
        
        public static Color GetPixelColorFromDesktop(int x, int y)
        {
            // get HWND of the Desktop
            var window = User32.GetDesktopWindow();
            
            // get HDC from HWND
            var hdc = User32.GetDC(window);

            var pixelColor = GetPixelColorFromHdc(hdc, x, y);
            
            // release HDC
            User32.ReleaseDC(window, hdc);

            return pixelColor;
        }

        public static BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource source;
            
            try 
            {
                source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally 
            {
                Gdi32.DeleteObject(hBitmap);
            }
            return source;
        }

        public static (int width, int height) GetScreenResolution()
        {
            int width = User32.GetSystemMetrics(User32.SystemMetric.SM_CXSCREEN);
            int height = User32.GetSystemMetrics(User32.SystemMetric.SM_CYSCREEN);

            return (width, height);
        }

        public static void SendKeyPressedEvent(Keys code, bool isKeyUp)
        {
            var input = new User32.INPUT();

            //TODO: consider moving this code into a factory
            input.type = User32.INPUTTYPE.INPUT_KEYBOARD;
            input.ki = new User32.KEYBDINPUT();
            input.ki.time = 0;
            input.ki.dwFlags = isKeyUp
                ? User32.KEYEVENTF.KEYEVENTF_KEYUP | User32.KEYEVENTF.KEYEVENTF_SCANCODE
                : 0 | User32.KEYEVENTF.KEYEVENTF_SCANCODE;
            input.ki.dwExtraInfo = UIntPtr.Zero;
            input.ki.wScan = (ushort) KeysHelper.GetScanCode(code);
            input.ki.wVk = 0;

            User32.SendInput(1, new[] {input}, Marshal.SizeOf(typeof(User32.INPUT)));
        }

        public static void SendKeyPressedEvent(Keys code)
        {
            SendKeyPressedEvent(code, false);
            SendKeyPressedEvent(code, true);
        }
    }
}