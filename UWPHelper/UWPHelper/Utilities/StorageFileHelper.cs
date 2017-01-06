using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWPHelper.Utilities
{
    public static class StorageFileHelper
    {
        public static async Task<bool> SaveObjectAsync(object obj, string fileName, StorageFolder folder)
        {
            bool success = true;

            try
            {
                await FileIO.WriteTextAsync(await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists), await Task.Factory.StartNew(() => JsonConvert.SerializeObject(obj)));
            }
            catch
            {
                success = false;
            }

            DebugHelper.OperationInfo(fileName, "saving", success);
            return success;
        }

        public static async Task<LoadObjectAsyncResult<T>> LoadObjectAsync<T>(string fileName, StorageFolder folder) where T : class, new()
        {
            StorageFile file = await folder.TryGetItemAsync(fileName) as StorageFile;
            return file != null ? await LoadObjectAsync<T>(file) : new LoadObjectAsyncResult<T>() { Object = new T() };
        }

        public static async Task<LoadObjectAsyncResult<T>> LoadObjectAsync<T>(StorageFile file) where T : class, new()
        {
            LoadObjectAsyncResult<T> output = new LoadObjectAsyncResult<T>();

            try
            {
                // Reading from StorageFile could fail because of the file is used by another proccess
                string json = await FileIO.ReadTextAsync(file);
                output.Object = !string.IsNullOrWhiteSpace(json) ? await Task.Run(() => JsonConvert.DeserializeObject<T>(json)) : new T();
            }
            catch
            {
                output.Object  = new T();
                output.Success = false;
            }

            DebugHelper.OperationInfo(file.Name, "loading", output.Success);
            return output;
        }

        public sealed class LoadObjectAsyncResult<T> where T : class, new()
        {
            public T Object { get; set; }
            public bool Success { get; set; }

            public LoadObjectAsyncResult()
            {
                Object  = null;
                Success = true;
            }
        }
    }
}
