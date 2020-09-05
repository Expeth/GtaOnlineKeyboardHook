using System.Windows;
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