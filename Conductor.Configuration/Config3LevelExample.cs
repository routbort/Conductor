using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.Configuration
{
   public class Config3LevelExample: ConfigBase

    {
        /// <summary>
        /// This is an example of how to implement a ConfigBase inheriting class which can be used to stored configuration information in a 
        /// centralized SQL Server database at three levels:
        /// 1. Application (global)
        /// 2. Machine (can use Environment.MachineName as entityName)
        /// 3. User (can use application specific username login as entityName)
        /// 
        /// Custom enumerations and helper methods are used to provide IntelliSense support at design-time
        /// </summary>
        /// <param name="provider"></param>
        public Config3LevelExample(IConfigurationProvider provider) : base(provider)
        {
        }

        public override string DefaultEntityName { get { return "{DEFAULT}"; } }

        #region Custom Enums

        public new enum EntityType { Application, Machine, User }

        public enum MachineSetting
        {
           SampleMachineSetting1, SampleMachineSetting2, PreferredPrinterName, MachineAllowedToAccessStuff        }

        public enum ApplicationSetting
        {
            SampleApplicationSetting1, SampleApplicationSetting2
        }

        public enum UserSetting
        {
            SampleUserSetting1, SampleUserSetting2, UserIsAwesome, Nickname
        }

        #endregion

        #region Custom Settings

        public Dictionary<string, string> GetMachineSettings(string MachineName)
        {
            return _provider.GetSettings(EntityType.Machine.ToString(), MachineName);
        }

        public string GetMachineSetting(MachineSetting name, string MachineName)
        {
            return _provider.GetSetting(EntityType.Machine.ToString(), MachineName, name.ToString());
        }

        public void SetMachineSetting(MachineSetting name, string MachineName, string value)
        {
            _provider.SetSetting(EntityType.Machine.ToString(), MachineName, name.ToString(), value);
        }

        public Dictionary<string, string> GetUserSettings(string UserName)
        {
            return _provider.GetSettings(EntityType.User.ToString(), UserName);
        }

        public string GetUserSetting(UserSetting name, string UserName)
        {
            return _provider.GetSetting(EntityType.User.ToString(), UserName, name.ToString());
        }

        public void SetUserSetting(UserSetting name, string UserName, string value)
        {
            _provider.SetSetting(EntityType.User.ToString(), UserName, name.ToString(), value);
        }

        public Dictionary<string, string> GetApplicationSettings()
        {
            return _provider.GetSettings(EntityType.Application.ToString(), "");
        }

        public string GetApplicationSetting(ApplicationSetting name)
        {
            return _provider.GetSetting(EntityType.Application.ToString(), DefaultEntityName, name.ToString());
        }

        public void SetApplicationSetting(ApplicationSetting name, string value)
        {
            _provider.SetSetting(EntityType.Application.ToString(), DefaultEntityName, name.ToString(), value);
        }

        #endregion


    }
}
