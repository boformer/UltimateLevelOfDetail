using System;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using ColossalFramework.IO;
using UnityEngine;

namespace TrueLodToggler
{
    public abstract class Configuration<C> where C : class, new()
    {
        private static C instance;

        public static C Load()
        {
            if (instance == null)
            {
                var configPath = GetConfigPath();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(C));
                try
                {
                    if (File.Exists(configPath))
                    {
                        using (StreamReader streamReader = new StreamReader(configPath))
                        {
                            instance = xmlSerializer.Deserialize(streamReader) as C;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            return instance ?? (instance = new C());
        }

        public static void Save()
        {
            if (instance == null) return;

            var configPath = GetConfigPath();

            var xmlSerializer = new XmlSerializer(typeof(C));
            var noNamespaces = new XmlSerializerNamespaces();
            noNamespaces.Add("", "");
            try
            {
                using (var streamWriter = new StreamWriter(configPath))
                {
                    xmlSerializer.Serialize(streamWriter, instance, noNamespaces);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static string GetConfigPath()
        {
            var configPathAttribute = typeof(C).GetCustomAttributes(typeof(ConfigurationPathAttribute), true)
                .FirstOrDefault() as ConfigurationPathAttribute;

            if (configPathAttribute != null)
            {
                return Path.Combine(DataLocation.localApplicationData, configPathAttribute.Value);
            }
            else
            {
                Debug.LogError("ConfigurationPath attribute missing in " + typeof(C).Name);
                return Path.Combine(DataLocation.localApplicationData, typeof(C).Name + ".xml");
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationPathAttribute : Attribute
    {
        public ConfigurationPathAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}