using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Media.Imaging;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers;
using GtaKeyboardHook.Infrastructure.Helpers;
using GtaKeyboardHook.Infrastructure.Interfaces;
using Point = System.Drawing.Point;

namespace GtaKeyboardHook.Infrastructure
{
    public class PreviewImageHolder : IExecutable<(Point pixel, Color axisColor)>, IBitmapHolder, INotifyPropertyChanged
    {
        public void Execute((Point pixel, Color axisColor) param, CancellationToken token, Action callback = null)
        {   
            var screenResolution = Win32ApiHelper.GetScreenResolution();
            var desktopScreenshot = Win32ApiHelper.GetDesktopScreenshot(screenResolution.width, screenResolution.height);
            var extendedScreenshot = ExtendBitmap(desktopScreenshot, 70, 70);
            
            //TODO: move width and height to a configuration
            var croppedBitmap =
                extendedScreenshot.Clone(new Rectangle(param.pixel.X, param.pixel.Y, 70, 70), extendedScreenshot.PixelFormat);

            var result = CreateAxisLines(croppedBitmap, param.axisColor);
            var bitmapSource = Win32ApiHelper.ConvertToBitmapSource(result);
            
            bitmapSource.Freeze();
            Instance = bitmapSource;

            callback?.Invoke();
        }

        //TODO: consider to move these methods to a helper
        private Bitmap ExtendBitmap(Bitmap source, int width, int height)
        {
            var extendedBitmap = new Bitmap(source.Width + width, source.Height + height, source.PixelFormat);
            using var graphics = Graphics.FromImage(extendedBitmap);
            
            graphics.FillRegion(Brushes.Black,
                new Region(new Rectangle(0, 0, extendedBitmap.Width, extendedBitmap.Height)));
            graphics.DrawImage(source, new Point(width / 2, height / 2));

            return extendedBitmap;
        }

        private Bitmap CreateAxisLines(Bitmap source, Color color)
        {
            using var graphics = Graphics.FromImage(source);
            
            var pen = new Pen(color);
            graphics.DrawLine(pen, new Point(35, 0), new Point(35, 70));
            graphics.DrawLine(pen, new Point(0, 35), new Point(70, 35));

            return source;
        }

        private BitmapSource _previewImage;
        
        public BitmapSource Instance
        {
            get => _previewImage;
            set
            {
                _previewImage = value;
                Notify();
            }
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion
    }
}