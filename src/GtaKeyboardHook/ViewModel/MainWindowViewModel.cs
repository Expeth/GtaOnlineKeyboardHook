using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Serilog;
using GtaKeyboardHook.Infrastructure;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers;
using GtaKeyboardHook.Infrastructure.Configuration;
using GtaKeyboardHook.Infrastructure.Helpers;
using GtaKeyboardHook.Model;
using GtaKeyboardHook.Model.Messages;
using GtaKeyboardHook.Model.Parameters;
using TinyMessenger;
using Color = System.Drawing.Color;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using KeyEventHandler = System.Windows.Forms.KeyEventHandler;

namespace GtaKeyboardHook.ViewModel
{
    public class MainWindowViewModel
    {
        private static readonly ILogger Logger = Log.ForContext<MainWindowViewModel>();

        #region PrivateFileds
        private BaseBackgoundWorker<CheckPixelDifferenceParameter> _checkPixelForDifferenceTask;
        private BaseBackgoundWorker<IProfileConfigurationProvider> _saveConfigurationTask;
        private BaseBackgoundWorker<SendKeyEventParameter> _sendKeyEventTask;
        private IProfileConfigurationProvider _appConfigProvider;
        private KeyEventHandler _keyDownHandler;
        private KeyEventHandler _keyUpHandler;
        private ITinyMessengerHub _messageBus;
        private KeyboardHook _keyboardHook;
        private MediaPlayer _mediaPlayer;
        private IEnumerable<string> _keys;
        private Color _hookedColor;
        #endregion
        
        #region Commands
        public ICommand SaveConfigurationCommand { get; set; }
        public ICommand PlayerIntroSoundCommand { get; set; }
        #endregion
        
        #region PublicViewProperties
        public IEnumerable<string> AvailableKeys
        {
            get => _keys;
            set
            {
                _keys = value;
                Notify();
            }
        }
        public int CoordinateX
        {
            get => _appConfigProvider.GetConfig().HookedCoordinateX;
            set => _appConfigProvider.GetConfig().HookedCoordinateX = value;
        }
        public int CoordinateY
        {
            get => _appConfigProvider.GetConfig().HookedCoordinateY;
            set => _appConfigProvider.GetConfig().HookedCoordinateY = value;
        }
        public string HookedKey
        {
            get => _appConfigProvider.GetConfig().HookedKeyCode;
            set
            {
                if (_keyboardHook != null) _keyboardHook.HookedKey = (Keys)Enum.Parse(typeof(Keys), value);
                _appConfigProvider.GetConfig().HookedKeyCode = value;
                Notify();
            }
        }
        public int CallbackDuration
        {
            get => _appConfigProvider.GetConfig().CallbackDuration;
            set => _appConfigProvider.GetConfig().CallbackDuration = value;
        }
        #endregion

        public MainWindowViewModel(
            BaseBackgoundWorker<CheckPixelDifferenceParameter> checkPixelForDifferenceTask,
            BaseBackgoundWorker<IProfileConfigurationProvider> saveConfigurationTask,
            BaseBackgoundWorker<SendKeyEventParameter> sendKeyEventTask,
            IProfileConfigurationProvider appConfigProvider,
            ITinyMessengerHub messageBus,
            KeyboardHook keyboardHook,
            MediaPlayer mediaPlayer)
        {
            _checkPixelForDifferenceTask = checkPixelForDifferenceTask;
            _saveConfigurationTask = saveConfigurationTask;
            _appConfigProvider = appConfigProvider;
            _sendKeyEventTask = sendKeyEventTask;
            _keyboardHook = keyboardHook;
            _mediaPlayer = mediaPlayer;
            _messageBus = messageBus;
            
            InitializeMessageBusHandlers();
            InitializeKeyEventHandlers();
            InitializeCommands();
            ReadConfiguration();
        }

        private void InitializeKeyEventHandlers()
        {
            _keyDownHandler = new KeyEventHandler(KeyDownHandler);
            _keyUpHandler = new KeyEventHandler(KeyUpHandler);

            _keyboardHook.KeyDownEvent += _keyDownHandler;
            _keyboardHook.KeyUpEvent += _keyUpHandler;
        }

        private void InitializeMessageBusHandlers()
        {
            _messageBus.Subscribe<PixelColorChangedMessage>(msg =>
            {
               _sendKeyEventTask.Execute(
                   new SendKeyEventParameter {DelayDuration = _appConfigProvider.GetConfig().CallbackDuration, HookedKey = Keys.S},
                   new CancellationToken()); 
            });
        }

        private void ReadConfiguration()
        {
            AvailableKeys = Enum.GetNames(typeof(Keys)).ToList();

            try
            {
                _hookedColor = ColorHelper.FromRgb(_appConfigProvider.GetConfig().HookedRgbColorCode);
            }
            catch (Exception e)
            {
                _hookedColor = Constants.GtaButtonColor;
                Logger.Error(e, "Couldn't parse rgb code");
            }
        }

        private void InitializeCommands()
        {
            SaveConfigurationCommand = new RelayCommand(o =>
            {
                _saveConfigurationTask.Execute(_appConfigProvider, new CancellationToken());
            });
            PlayerIntroSoundCommand = new RelayCommand(o =>
            {
                _mediaPlayer.Position = TimeSpan.Zero;
                _mediaPlayer.Play();
            });
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            _checkPixelForDifferenceTask.Execute(
                new CheckPixelDifferenceParameter {Pixel = new Point(CoordinateX, CoordinateY), HookedColor = _hookedColor},
                new CancellationToken());
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