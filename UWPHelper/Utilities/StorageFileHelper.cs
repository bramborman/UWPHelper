using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWPHelper.Utilities
{
    public static class StorageFileHelper
    {
        public static Task<bool> SaveObjectAsync(object obj, string fileName, StorageFolder folder)
        {
            return SaveObjectAsync(obj, fileName, folder, CreationCollisionOption.OpenIfExists);
        }

        public static async Task<bool> SaveObjectAsync(object obj, string fileName, StorageFolder folder, CreationCollisionOption creationCollisionOption)
        {
            ExceptionHelper.ValidateStringNotNullOrWhiteSpace(fileName, nameof(fileName));
            ExceptionHelper.ValidateObjectNotNull(folder, nameof(folder));
            ExceptionHelper.ValidateEnumValueDefined(creationCollisionOption, nameof(creationCollisionOption));

            bool success = true;

            try
            {
                StorageFile file    = await folder.CreateFileAsync(fileName, creationCollisionOption);
                string json         = await Task.Run(() => JsonConvert.SerializeObject(obj));

                await FileIO.WriteTextAsync(file, json);
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
            ExceptionHelper.ValidateStringNotNullOrWhiteSpace(fileName, nameof(fileName));
            ExceptionHelper.ValidateObjectNotNull(folder, nameof(folder));

            StorageFile file = await folder.TryGetItemAsync(fileName) as StorageFile;
            return file != null ? await LoadObjectAsync<T>(file) : new LoadObjectAsyncResult<T>(new T(), true);
        }

        public static async Task<LoadObjectAsyncResult<T>> LoadObjectAsync<T>(StorageFile file) where T : class, new()
        {
            ExceptionHelper.ValidateObjectNotNull(file, nameof(file));

            bool success    = true;
            T obj           = null;

            // Reading from the file could fail while the file is used by another proccess
            try
            {
                string json = await FileIO.ReadTextAsync(file);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    obj = await Task.Run(() => JsonConvert.DeserializeObject<T>(json));
                }
            }
            catch
            {
                success = false;
            }

            DebugHelper.OperationInfo(file.Name, "loading", success);
            return new LoadObjectAsyncResult<T>(obj ?? new T(), success);
        }
        
        public sealed class LoadObjectAsyncResult<T> where T : class, new()
        {
            public T Object { get; }
            public bool Success { get; }

            public LoadObjectAsyncResult(T @object, bool success)
            {
                Object  = @object;
                Success = success;
            }
        }
    }
}
