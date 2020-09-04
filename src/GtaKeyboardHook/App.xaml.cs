using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using Application = System.Windows.Application;
using Serilog;
using GtaKeyboardHook.Infrastructure;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers;
using GtaKeyboardHook.Infrastructure.BackgroundWorkers.TaskFactories;
using GtaKeyboardHook.Model;
using GtaKeyboardHook.Model.Parameters;
using GtaKeyboardHook.ViewModel;
using TinyMessenger;
using IConfigurationProvider = GtaKeyboardHook.Model.IConfigurationProvider;

namespace GtaKeyboardHook
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        private IServiceCollection _serviceCollection;
        
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
                .AddScoped<BaseBackgoundWorker<CheckPixelDifferenceParameter>>(provider =>
                    new PixelTrackerBackgroundWorker(new SingleTaskFactory(),
                        provider.GetRequiredService<ITinyMessengerHub>()));
        }

        private void SetupConfigurationProvider(IServiceCollection services)
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var configuration = ConfigurationManager.OpenExeConfiguration(assemblyLocation);

            var mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(Directory.GetCurrentDirectory() + "\\Resources\\intro.mp3"));
            mediaPlayer.Volume = 100;
            
            services.AddSingleton(configuration)
                .AddSingleton<IConfigurationProvider, AppConfigProvider>()
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
