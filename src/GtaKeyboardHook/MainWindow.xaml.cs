using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using GtaKeyboardHook.ViewModel;
using Vanara.PInvoke;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using KeyEventHandler = System.Windows.Forms.KeyEventHandler;
using TextBox = System.Windows.Controls.TextBox;

namespace GtaKeyboardHook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel dataContext)
        {
            this.DataContext = dataContext;
            InitializeComponent();
        }
    }
}
