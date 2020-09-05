using System;
using System.IO;
using System.Threading.Tasks;
using GtaKeyboardHook.Model.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace GtaKeyboardHook.Infrastructure.Configuration
{
    public class JsonConfigurationProvider : IProfileConfigurationProvider
    {
        private static readonly ILogger Logger = Log.ForContext<JsonConfigurationProvider>();

        private readonly string _filePath;
        private ProfileConfiguration _configuration;

        public JsonConfigurationProvider(string filePath)
        {
            _filePath = filePath;
        }

        public async Task LoadFromSourceAsync()
        {
            try
            {
                using var configFile = new StreamReader(new FileStream(_filePath, FileMode.OpenOrCreate));

                var jsonString = await configFile.ReadToEndAsync();

                _configuration = JsonConvert.DeserializeObject<ProfileConfiguration>(jsonString);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Unable to open file {path}", _filePath);
                throw;
            }
        }

        public void LoadFromSource()
        {
            LoadFromSourceAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public ProfileConfiguration GetConfig()
        {
            return _configuration;
        }

        public void Save()
        {
            SaveAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task SaveAsync()
        {
            try
            {
                using var configFile = new StreamWriter(new FileStream(_filePath, FileMode.Truncate));

                var jsonString = JsonConvert.SerializeObject(_configuration);

                return configFile.WriteLineAsync(jsonString);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Unable to open file {path}", _filePath);
                throw;
            }
        }
    }
}