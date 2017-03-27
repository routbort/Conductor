using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conductor.Configuration
{
  public  interface IConfigurationProvider
    {
        void Initialize(string Initializer);
        bool IsInitialized { get; }
        BindingList<string> GetEntityNames(string entityType);
        string GetSetting(string entityType, string entityName, string settingName);
        Dictionary<string, string> GetSettings(string entityType, string entityName);
        void SetSetting(string entityType, string entityName, string settingName, string value);
        void SetSettings(string entityType, string entityName, Dictionary<string, string> settings);
        event EventHandler SettingsUpdated;

    }
}
