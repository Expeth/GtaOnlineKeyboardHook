using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Application = System.Windows.Application;
using Serilog;
using GtaKeyboardHook.Infrastructure;
using GtaKeyboardHook.Model;
using GtaKeyboardHook.ViewModel;
using Vanara.PInvoke;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;
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
            SetupMainWindow(services);
        }

        private void SetupConfigurationProvider(IServiceCollection services)
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var configuration = ConfigurationManager.OpenExeConfiguration(assemblyLocation);

            services.AddSingleton(configuration)
                .AddSingleton<IConfigurationProvider, AppConfigProvider>();
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
