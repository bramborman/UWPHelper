using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWPHelper.Utilities
{
    public abstract class AppDataBase : NotifyPropertyChanged
    {
        public class LoadAsyncResult<T> where T : AppDataBase, new()
        {
            public T AppData { get; set; }
            public bool Success { get; set; }

            public LoadAsyncResult()
            {
                AppData = null;
                Success = true;
            }
        }

        protected async Task<bool> SaveAsync(string fileName)
        {
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                string json = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(this));
                await FileIO.WriteTextAsync(file, json);

                return true;
            }
            catch
            {
                return false;
            }
        }

        protected static async Task<LoadAsyncResult<T>> LoadAsync<T>(string fileName) where T : AppDataBase, new()
        {
            LoadAsyncResult<T> result = new LoadAsyncResult<T>();

            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                string json = await FileIO.ReadTextAsync(file);

                if (string.IsNullOrWhiteSpace(json))
                {
                    result.AppData = new T();
                }
                else
                {
                    result.AppData = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(json));
                }
            }
            catch (Exception ex)
            {
                result.AppData = new T();
                result.Success = ex is FileNotFoundException;
            }

            return result;
        }
    }
}