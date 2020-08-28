using System;
using System.Configuration;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Vanara.Extensions.Reflection;

namespace GtaKeyboardHook.Model
{
   public static class AppConfigProperties
   {
      public static readonly string CallbackDuration = "callbackDuration";
      public static readonly string HookedKeyCode = "hookedKeyCode";
      public static readonly string HookedCoordinateX = "hookedCoordinateX";
      public static readonly string HookedCoordinateY = "hookedCoordinateY";
      public static readonly string HookedRgbColorCode = "hookedColorRgbCode";
   }
   
   public interface IConfigurationProvider
   {
      string GetValue(string property);
      void SetValue(string property, string value);
   }
   
   public class AppConfigProvider : IConfigurationProvider
   {
      private Configuration _configuration;

      public AppConfigProvider(Configuration configuration)
      {
         _configuration = configuration;
      }
      
      public string GetValue(string property)
      {
         return _configuration.AppSettings.Settings[property].Value;
      }

      public void SetValue(string property, string value)
      {
         _configuration.AppSettings.Settings[property].Value = value;
         _configuration.Save();
      }
   }
}