using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conductor.Configuration
{
    public class SQLConfigurationProvider : IConfigurationProvider
    {
        bool _Initialized = false;
        string _ConnectionString = null;
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> _AllSettings = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        public event EventHandler SettingsUpdated;

        public bool IsInitialized
        {
            get
            {
                return _Initialized;
            }
        }

        public void Initialize(string Initializer)
        {
            _ConnectionString = Initializer;
        }

        public BindingList<string> GetEntityNames(string entityType)
        {
            BindingList<string> results = new BindingList<string>();
            Dictionary<string, string> settings = new Dictionary<string, string>();
            using (SqlConnection connection = new SqlConnection(this._ConnectionString))
            using (SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandText = "GetEntities",
                CommandType = CommandType.StoredProcedure
            })
            {
                command.Parameters.AddWithValue("@entityType", entityType);
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                connection.Close();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                    results.Add(row["entityName"].ToString());
                return results;
            }
        }

        public string GetSetting(string entityType, string entityName, string settingName)
        {
            if (!_AllSettings.ContainsKey(entityType) || !_AllSettings[entityType].ContainsKey(entityName))
            {
                if (!_AllSettings.ContainsKey(entityType))
                    _AllSettings[entityType] = new Dictionary<string, Dictionary<string, string>>();
                _AllSettings[entityType][entityName] = GetSettings(entityType, entityName);
            }

            if (!_AllSettings[entityType][entityName].ContainsKey(settingName))
                return null;

            return _AllSettings[entityType][entityName][settingName];
        }

        public Dictionary<string, string> GetSettings(string entityType, string entityName)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            using (SqlConnection connection = new SqlConnection(this._ConnectionString))
            using (SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandText = "GetSettings",
                CommandType = CommandType.StoredProcedure
            })
            {
                command.Parameters.AddWithValue("@entityType", entityType);
                command.Parameters.AddWithValue("@entityName", entityName);
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                connection.Close();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                    settings[row["settingName"].ToString()] = row["value"].ToString();
                return settings;
            }




        }

        public void SetSetting(string entityType, string entityName, string settingName, string value)
        {
            SetSetting(entityType, entityName, settingName, value, true);
        }

        public void SetSetting(string entityType, string entityName, string settingName, string value, bool TriggerUpdateEvent = true)
        {
            
            using (SqlConnection connection = new SqlConnection(this._ConnectionString))
            using (SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandText = "SetSetting",
                CommandType = CommandType.StoredProcedure
            })
            {
                command.Parameters.AddWithValue("@entityType", entityType);
                command.Parameters.AddWithValue("@entityName", entityName);
                command.Parameters.AddWithValue("@settingName", settingName);
                command.Parameters.AddWithValue("@value", value);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                //update local settings cache if needed
                if (_AllSettings.ContainsKey(entityType) && _AllSettings[entityType].ContainsKey(entityName) && _AllSettings[entityType][entityName].ContainsKey(settingName))
                    _AllSettings[entityType][entityName][settingName] = value;
            }


            if (TriggerUpdateEvent)
                    SettingsUpdated?.Invoke(this, null);
         
        }

        public void SetSettings(string entityType, string entityName, Dictionary<string, string> settings)
        {
            foreach (var settingName in settings.Keys)
                SetSetting(entityType, entityName, settingName, settings[settingName],false);
            SettingsUpdated?.Invoke(this, null);
        }
    }
}
