using System.Windows.Media.Imaging;

namespace GtaKeyboardHook.Infrastructure.Interfaces
{
    public interface IBitmapHolder
    {
        BitmapSource Instance { get; set; }
    }
}