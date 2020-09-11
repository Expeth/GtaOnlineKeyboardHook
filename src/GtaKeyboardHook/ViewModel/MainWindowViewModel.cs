using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using GtaKeyboardHook.Infrastructure;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers;
using GtaKeyboardHook.Infrastructure.Configuration;
using GtaKeyboardHook.Infrastructure.Helpers;
using GtaKeyboardHook.Infrastructure.Interfaces;
using GtaKeyboardHook.Model;
using GtaKeyboardHook.Model.Messages;
using GtaKeyboardHook.Model.Parameters;
using Serilog;
using TinyMessenger;
using Color = System.Drawing.Color;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using KeyEventHandler = System.Windows.Forms.KeyEventHandler;

namespace GtaKeyboardHook.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private static readonly ILogger Logger = Log.ForContext<MainWindowViewModel>();

        public MainWindowViewModel(
            BaseBackgoundWorker<CheckPixelDifferenceParameter> checkPixelForDifferenceTask,
            BaseBackgoundWorker<IProfileConfigurationProvider> saveConfigurationTask,
            BaseBackgoundWorker<SendKeyEventParameter> sendKeyEventTask,
            IProfileConfigurationProvider appConfigProvider,
            PreviewUpdateWorker previewUpdateTask,
            ITinyMessengerHub messageBus,
            KeyboardHook keyboardHook,
            MediaPlayer mediaPlayer)
        {
            _checkPixelForDifferenceTask = checkPixelForDifferenceTask;
            _saveConfigurationTask = saveConfigurationTask;
            _appConfigProvider = appConfigProvider;
            _sendKeyEventTask = sendKeyEventTask;
            _previewUpdateTask = previewUpdateTask;
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
            _keyDownHandler = KeyDownHandler;
            _keyUpHandler = KeyUpHandler;

            _keyboardHook.KeyDownEvent += _keyDownHandler;
            _keyboardHook.KeyUpEvent += _keyUpHandler;
        }

        private void InitializeMessageBusHandlers()
        {
            _messageBus.Subscribe<PixelColorChangedMessage>(msg =>
            {
                _sendKeyEventTask.Execute(
                    new SendKeyEventParameter
                        {DelayDuration = _appConfigProvider.GetConfig().CallbackDuration, HookedKey = Keys.S},
                    new CancellationToken());
            });
        }

        private void ReadConfiguration()
        {
            AvailableKeys = Enum.GetNames(typeof(Keys)).ToList();

            // to setup a key for the keyboard hook
            HookedKey = _appConfigProvider.GetConfig().HookedKeyCode;
            
            // initial setup of preview window
            _previewUpdateTask.Execute((() => new Point(CoordinateX, CoordinateY), DisableHookCommand.CanExecute),
                CancellationToken.None);

            _screenResolution = Win32ApiHelper.GetScreenResolution();
            
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
            DisableHookCommand = new RelayCommand(o =>
            {
                _pixelHookCancellationTokenSource.Cancel();
            }, o =>
            {
                return !(_pixelHookCancellationTokenSource == null ||
                       _pixelHookCancellationTokenSource.IsCancellationRequested);
            });
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (DisableHookCommand.CanExecute(sender)) return;

            _pixelHookCancellationTokenSource = new CancellationTokenSource();
            _checkPixelForDifferenceTask.Execute(
                new CheckPixelDifferenceParameter
                    {Pixel = new Point(CoordinateX, CoordinateY), HookedColor = _hookedColor},
                _pixelHookCancellationTokenSource.Token,
                () => { _pixelHookCancellationTokenSource = null; });
        }

        private void KeyUpHandler(object sender, KeyEventArgs e)
        {
            // nothing is needed for key up event
        }

        #region PrivateFileds

        private readonly BaseBackgoundWorker<CheckPixelDifferenceParameter> _checkPixelForDifferenceTask;
        private readonly BaseBackgoundWorker<IProfileConfigurationProvider> _saveConfigurationTask;
        private readonly BaseBackgoundWorker<SendKeyEventParameter> _sendKeyEventTask;
        private readonly IProfileConfigurationProvider _appConfigProvider;
        private readonly PreviewUpdateWorker _previewUpdateTask;
        private KeyEventHandler _keyDownHandler;
        private KeyEventHandler _keyUpHandler;
        private readonly ITinyMessengerHub _messageBus;
        private readonly KeyboardHook _keyboardHook;
        private readonly MediaPlayer _mediaPlayer;
        private CancellationTokenSource _pixelHookCancellationTokenSource;
        private (int width, int height) _screenResolution;
        private IEnumerable<string> _keys;
        private Color _hookedColor;

        #endregion

        #region Commands

        public ICommand SaveConfigurationCommand { get; set; }
        public ICommand PlayerIntroSoundCommand { get; set; }
        public ICommand DisableHookCommand { get; set; }

        #endregion

        #region PublicViewProperties

        public IBitmapHolder PreviewWindow
        {
            get => _previewUpdateTask.PreviewWindow;
        }

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
            set
            {
                if (value > _screenResolution.width || value < 0) return;
                _appConfigProvider.GetConfig().HookedCoordinateX = value;
            }
        }

        public int CoordinateY
        {
            get => _appConfigProvider.GetConfig().HookedCoordinateY;
            set
            {
                if (value > _screenResolution.height || value < 0) return;
                _appConfigProvider.GetConfig().HookedCoordinateY = value;
            }
        }

        public string HookedKey
        {
            get => _appConfigProvider.GetConfig().HookedKeyCode;
            set
            {
                if (_keyboardHook != null) _keyboardHook.HookedKey = (Keys) Enum.Parse(typeof(Keys), value);
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

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion
    }
}