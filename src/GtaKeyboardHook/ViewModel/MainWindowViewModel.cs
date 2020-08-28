using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Serilog;
using GtaKeyboardHook.Infrastructure;
using GtaKeyboardHook.Model;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using KeyEventHandler = System.Windows.Forms.KeyEventHandler;

namespace GtaKeyboardHook.ViewModel
{
    public class MainWindowViewModel
    {
        private static readonly ILogger Logger = Log.ForContext<MainWindowViewModel>();

        private IConfigurationProvider _appConfigProvider;
        private KeyEventHandler _keyDownHandler;
        private KeyEventHandler _keyUpHandler;
        private KeyboardHook _keyboardHook;
        private Task _backgroundTask;
        private Color _hookedColor;
        
        public ICommand SaveConfigurationCommand { get; set; }
        
        private IEnumerable<string> _keys; 
        public IEnumerable<string> AvailableKeys
        {
            get => _keys;
            set
            {
                _keys = value;
                Notify();
            }
        }

        private int _coordinateX;
        public int CoordinateX
        {
            get => _coordinateX;
            set => _coordinateX = value;
        }

        private int _coordinateY;
        public int CoordinateY
        {
            get => _coordinateY;
            set => _coordinateY = value;
        }

        private string _hookedKey;
        public string HookedKey
        {
            get => _hookedKey;
            set
            {
                if (_keyboardHook != null) _keyboardHook.HookedKey = (Keys)Enum.Parse(typeof(Keys), value);
                _hookedKey = value;
                Notify();
            }
        }

        private int _callbackDuration;
        public int CallbackDuration
        {
            get => _callbackDuration;
            set => _callbackDuration = value;
        }

        public MainWindowViewModel(IConfigurationProvider appConfigProvider)
        {
            _appConfigProvider = appConfigProvider;

            InitializeCommands();
            ReadConfiguration();
            
            _backgroundTask = new Task(CheckPixelColorForDifference);
            _keyboardHook = new KeyboardHook();

            _keyUpHandler = new KeyEventHandler(KeyUpHandler);
            _keyDownHandler = new KeyEventHandler(KeyDownHandler);
            
            _keyboardHook.KeyUpEvent += _keyUpHandler;
            _keyboardHook.KeyDownEvent += _keyDownHandler;
        }

        private void ReadConfiguration()
        {
            CoordinateX = Int32.Parse(_appConfigProvider.GetValue(AppConfigProperties.HookedCoordinateX));
            CoordinateY = Int32.Parse(_appConfigProvider.GetValue(AppConfigProperties.HookedCoordinateY));
            CallbackDuration = Int32.Parse(_appConfigProvider.GetValue(AppConfigProperties.CallbackDuration));
            AvailableKeys = Enum.GetNames(typeof(Keys)).ToList();
            HookedKey = _appConfigProvider.GetValue(AppConfigProperties.HookedKeyCode);

            try
            {
                var rgbCode = _appConfigProvider.GetValue(AppConfigProperties.HookedRgbColorCode);
                var codes = rgbCode.Split(',').Select(x => Int32.Parse(x)).ToArray();

                _hookedColor = Color.FromArgb(codes[0], codes[1], codes[2]);
            }
            catch (Exception e)
            {
                _hookedColor = Const.GtaButtonColor;
                Logger.Error(e, "Couldn't parse rgb code");
            }
        }

        private void InitializeCommands()
        {
            SaveConfigurationCommand = new RelayCommand(o => Task.Run(() =>
            {
                SaveConfiguration(o);
            }));
        }

        private void SaveConfiguration(object obj)
        {
            _appConfigProvider.SetValue(AppConfigProperties.CallbackDuration, _callbackDuration.ToString());
            _appConfigProvider.SetValue(AppConfigProperties.HookedCoordinateX, _coordinateX.ToString());
            _appConfigProvider.SetValue(AppConfigProperties.HookedCoordinateY, _coordinateY.ToString());
            _appConfigProvider.SetValue(AppConfigProperties.HookedKeyCode, _hookedKey);
        }

        private void CheckPixelColorForDifference()
        {
            while (true)
            {
                var color = Win32ApiHelper.GetPixelColor(_coordinateX, _coordinateY);

                if (!(color.R == _hookedColor.R && color.G == _hookedColor.G && color.B == _hookedColor.B))
                    continue;
                
                Task.Run(() =>
                {
                    Logger.Information("Sending KeyPressedEvent for key {key} after {callback} ms", "S", _callbackDuration);
                    Thread.Sleep(_callbackDuration);
                    
                    Win32ApiHelper.SendKeyPressedEvent(Keys.S);
                });
                
                break;
            }
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (_backgroundTask.Status == TaskStatus.Running) return;
            
            _backgroundTask = _backgroundTask.Status == TaskStatus.RanToCompletion
                ? new Task(CheckPixelColorForDifference)
                : _backgroundTask;
            _backgroundTask.Start();
        }

        private void KeyUpHandler(object sender, KeyEventArgs e)
        {
            // nothing is needed for key up event
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