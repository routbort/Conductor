using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using System.ComponentModel;

namespace Conductor.Configuration
{
    public abstract class ConfigBase
    {
        public enum EntityType { OVERRIDE_THIS_IN_AN_INHERITED_CLASS }


        public abstract string DefaultEntityName { get; }

        public IConfigurationProvider Provider { get { return _provider; } }

        protected IConfigurationProvider _provider = null;

        private ConfigBase() { }

        public ConfigBase(IConfigurationProvider provider)
        {
            _provider = provider;
        }

        public string[] GetEntityTypes()

        {
            return this.GetType().GetNestedType("EntityType").GetEnumNames();
        }

        public string[] GetSettingNames(EntityType entityType)

        {
            return GetSettingNames(entityType.ToString());
        }

        public string[] GetSettingNames(string entityType)

        {
            string enumName = entityType + "Setting";
            Type enumType = this.GetType().GetNestedType(enumName);
            if (enumType != null)
                return enumType.GetEnumNames();
            return null;
        }

        /// <summary>
        /// This returns a merged group of settings - both those that are defined at Enum level, even if not returning values as the data provider level -
        /// plus any data provider level settings, even if they are not defind at the Enum level
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public DataTable GetMergedSettings(string entityType, string entityName)
        {
            DataTable results = new DataTable();
            results.Columns.Add("settingName", typeof(string));
            results.Columns["settingName"].ReadOnly = true;
            results.Columns.Add("value", typeof(string));
            Dictionary<string, string> initialResults = _provider.GetSettings(entityType, entityName);
            SortedDictionary<string, string> mergedResults = new SortedDictionary<string, string>();
            var definedKeys = GetSettingNames(entityType);
            var allKeys = definedKeys.Union(initialResults.Keys);
            List<string> sortedAllKeys = allKeys.ToList<string>();
            sortedAllKeys.Sort();
            foreach (string settingName in sortedAllKeys)
            {
                mergedResults[settingName] = (initialResults.ContainsKey(settingName)) ? initialResults[settingName] : null;
                DataRow row = results.NewRow();
                row["settingName"] = settingName;
                row["value"] = (initialResults.ContainsKey(settingName)) ? initialResults[settingName] : null;
                results.Rows.Add(row);
            }

            return results;
        }

        public Dictionary<string, string[]> GetAllSettingNames()
        {
            Dictionary<string, string[]> results = new Dictionary<string, string[]>();
            foreach (string et in GetEntityTypes())
                results[et] = GetSettingNames((EntityType)Enum.Parse(typeof(EntityType), et));
            return results;

        }

        public BindingList<string> GetEntityNames(EntityType entityType)
        {
            return _provider.GetEntityNames(entityType.ToString());
        }

        public BindingList<string> GetEntityNames(string entityType)
        {
            return _provider.GetEntityNames(entityType);
        }

    }
}
