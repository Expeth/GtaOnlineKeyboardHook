using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using GtaKeyboardHook.Infrastructure;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers.TaskFactories;
using GtaKeyboardHook.Infrastructure.Configuration;
using GtaKeyboardHook.Model.Parameters;
using GtaKeyboardHook.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TinyMessenger;

namespace GtaKeyboardHook
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceCollection _serviceCollection;

        public App()
        {
            _serviceCollection = new ServiceCollection();
            ConfigureServices(_serviceCollection);

            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            SetupConfigurationProvider(services);
            SetupLogger(services);
            SetupInfrastructure(services);
            SetupMainWindow(services);
        }

        private void SetupInfrastructure(IServiceCollection services)
        {
            services.AddSingleton<ITinyMessengerHub, TinyMessengerHub>();

            services.AddScoped<BaseBackgoundWorker<SendKeyEventParameter>>(provider =>
                    new SendKeyEventBackgroundWorker(new MultipleTaskFactory()))
                .AddScoped<BaseBackgoundWorker<IProfileConfigurationProvider>>(provider =>
                    new ConfigSaverBackgroundWorker(new MultipleTaskFactory()))
                .AddScoped<BaseBackgoundWorker<CheckPixelDifferenceParameter>>(provider =>
                    new PixelTrackerBackgroundWorker(new SingleTaskFactory(),
                        provider.GetRequiredService<ITinyMessengerHub>()));
        }

        private void SetupConfigurationProvider(IServiceCollection services)
        {
            var mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(Directory.GetCurrentDirectory() + "\\Resources\\intro.mp3"));
            mediaPlayer.Volume = 100;

            //TODO: to use Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            var documentsFolder = AppDomain.CurrentDomain.BaseDirectory;
            var configManager = new JsonConfigurationProvider(documentsFolder + "configuration.json");
            configManager.LoadFromSource();

            services.AddSingleton<IProfileConfigurationProvider>(configManager)
                .AddSingleton(typeof(KeyboardHook))
                .AddSingleton(mediaPlayer);
        }

        private void SetupMainWindow(IServiceCollection services)
        {
            services.AddTransient(typeof(MainWindowViewModel));
            services.AddTransient(typeof(MainWindow));
        }

        private void SetupLogger(IServiceCollection services)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.RollingFile("logs.txt")
                .CreateLogger();

            Log.Logger = logger;
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}