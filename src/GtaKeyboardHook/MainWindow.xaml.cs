using System.Windows;
using System.Windows.Media.Imaging;
using GtaKeyboardHook.Infrastructure.Helpers;
using GtaKeyboardHook.ViewModel;

namespace GtaKeyboardHook
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel dataContext)
        {
            DataContext = dataContext;
            InitializeComponent();
        }
    }
}