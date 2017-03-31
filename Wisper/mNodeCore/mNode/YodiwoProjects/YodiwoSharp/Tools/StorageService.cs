using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace Yodiwo.Tools
{
    public class StorageService : IStorageService
    {
        Windows.Storage.StorageFolder settingsFolder;
        Windows.Storage.ApplicationDataContainer settingsContainer;

        public StorageService(bool Roaming)
        {
            if (Roaming)
            {
                settingsFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;
                settingsContainer = Windows.Storage.ApplicationData.Current.RoamingSettings;
            }
            else
            {
                settingsFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                settingsContainer = Windows.Storage.ApplicationData.Current.LocalSettings;
            }
        }

        public string Serialize(object obj)
        {
            if (obj == null)
                return string.Empty;
            using (var sw = new StringWriter())
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(sw, obj);
                return sw.ToString();
            }
        }

        public T Deserialize<T>(string xml)
        {
            if (xml == null)
                return default(T);
            using (var sw = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(sw);
            }
        }

        #region Settings

        public void SaveSetting(string key, object value)
        {
            settingsContainer.Values[key] = value;
        }

        public void SaveSetting(string key, string value)
        {
            settingsContainer.Values[key] = value;
        }

        public void DeleteSetting(string key)
        {
            settingsContainer.Values.Remove(key);
        }

        public string LoadSettingString(string key)
        {
            return LoadSetting(key) as string;
        }

        public object LoadSetting(string key)
        {
            var value = settingsContainer.Values[key];

            if (value == null)
                return null;

            return value;
        }

        #endregion

        #region Objects
        public async Task<bool> PersistObjectAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
                return false;

            string xml = Serialize(value);

            var file = await settingsFolder.CreateFileAsync(key, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, xml);

            return true;
        }

        public async Task<T> RetrieveObjectAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                return default(T);

            try
            {
                var file = await settingsFolder.GetFileAsync(key);
                string json = await FileIO.ReadTextAsync(file);
                return Deserialize<T>(json);
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return default(T);
            }
        }

        public async Task<bool> DeleteObjectAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            try
            {
                StorageFile file = await settingsFolder.GetFileAsync(key);
                await file.DeleteAsync();
                return true;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return false;
            }
        }

        public async Task<bool> SaveFile(string key, string value)
        {
            try
            {
                StorageFile file = await settingsFolder.CreateFileAsync(key, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, value);
                return true;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return false;
            }
        }

        public async Task<bool> FileExists(string key)
        {
            try
            {
                StorageFile file = await settingsFolder.GetFileAsync(key);
                return file != null;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return false;
            }
        }

        public async Task<string> LoadFile(string key)
        {
            try
            {
                StorageFile file = await settingsFolder.GetFileAsync(key);
                return await FileIO.ReadTextAsync(file);
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return null;
            }
        }
        #endregion
    }

    public interface IStorageService
    {
        void SaveSetting(string key, object value);
        void SaveSetting(string key, string value);

        void DeleteSetting(string key);

        string LoadSettingString(string key);
        object LoadSetting(string key);

        Task<bool> PersistObjectAsync<T>(string key, T value);

        Task<T> RetrieveObjectAsync<T>(string key);

        Task<bool> DeleteObjectAsync(string key);

        Task<bool> SaveFile(string key, string value);
        Task<string> LoadFile(string key);
    }
}
