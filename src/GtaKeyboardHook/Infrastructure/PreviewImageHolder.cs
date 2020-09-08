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
    public class PreviewImageHolder : IExecutable<Point>, IBitmapHolder, INotifyPropertyChanged
    {
        public void Execute(Point param, CancellationToken token)
        {
            //TODO: should be compatible with all screen resolutions 
            var desktopScreenshot = Win32ApiHelper.GetDesktopScreenshot(1920, 1080);
            var extendedScreenshot = ExtendBitmap(desktopScreenshot, 70, 70);
            
            //TODO: move width and height to a configuration
            var croppedBitmap =
                extendedScreenshot.Clone(new Rectangle(param.X, param.Y, 70, 70), extendedScreenshot.PixelFormat);

            var result = CreateAxisLines(croppedBitmap);
            var bitmapSource = Win32ApiHelper.ConvertToBitmapSource(result);
            
            bitmapSource.Freeze();
            Instance = bitmapSource;
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

        private Bitmap CreateAxisLines(Bitmap source)
        {
            using var graphics = Graphics.FromImage(source);
            
            //TODO: move width and height to a configuration
            graphics.DrawLine(Pens.Red, new Point(35, 0), new Point(35, 70));
            graphics.DrawLine(Pens.Red, new Point(0, 35), new Point(70, 35));

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