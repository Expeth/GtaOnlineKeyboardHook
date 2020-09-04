using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using Vanara.Extensions.Reflection;

namespace GtaKeyboardHook.Model
{
   public class ProfileConfiguration
   {
      public int CallbackDuration { get; set; }
      public string HookedKeyCode { get; set; }
      public int HookedCoordinateX { get; set; }
      public int HookedCoordinateY { get; set; }
      public string HookedRgbColorCode { get; set; }

      public override bool Equals(object obj)
      {
         return CallbackDuration == (obj as ProfileConfiguration).CallbackDuration &&
                HookedKeyCode == (obj as ProfileConfiguration).HookedKeyCode &&
                HookedCoordinateX == (obj as ProfileConfiguration).HookedCoordinateX &&
                HookedCoordinateY == (obj as ProfileConfiguration).HookedCoordinateY &&
                HookedRgbColorCode == (obj as ProfileConfiguration).HookedRgbColorCode;
      }

      public override string ToString()
      {
         return JsonConvert.SerializeObject(this);
      }
   }

   public interface IProfileConfigurationManager
   {
      void LoadFromSource();
      Task LoadFromSourceAsync();
      ProfileConfiguration GetConfig();
      void Save();
      Task SaveAsync();
   }

   public class JsonConfigurationManager : IProfileConfigurationManager
   {
      private static readonly ILogger Logger = Log.ForContext<JsonConfigurationManager>();
      
      private readonly string _filePath;
      private ProfileConfiguration _configuration;

      public JsonConfigurationManager(string filePath)
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