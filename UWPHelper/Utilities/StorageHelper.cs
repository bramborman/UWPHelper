using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWPHelper.Utilities
{
    public static class StorageHelper
    {
        public static Task<StorageHelperResult> SaveObjectAsync(object obj, string fileName, StorageFolder folder)
        {
            return SaveObjectAsync(obj, fileName, folder, CreationCollisionOption.OpenIfExists);
        }
        
        public static async Task<StorageHelperResult> SaveObjectAsync(object obj, string fileName, StorageFolder folder, CreationCollisionOption creationCollisionOption)
        {
            ExceptionHelper.ValidateStringNotNullOrWhiteSpace(fileName, nameof(fileName));
            ExceptionHelper.ValidateObjectNotNull(folder, nameof(folder));
            ExceptionHelper.ValidateEnumValueDefined(creationCollisionOption, nameof(creationCollisionOption));

            StorageHelperResult result = new StorageHelperResult();

            try
            {
                StorageFile file    = await folder.CreateFileAsync(fileName, creationCollisionOption);
                string json         = await Task.Run(() => JsonConvert.SerializeObject(obj));

                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception exception)
            {
                result.Status       = StorageHelperStatus.Failure;
                result.Exception    = exception;
            }

            Debug.WriteLine(fileName + " saving " + (result.Status != StorageHelperStatus.Failure ? "succeeded" : "failed"));
            return result;
        }
        
        public static async Task<StorageHelperLoadResult<T>> LoadObjectAsync<T>(string fileName, StorageFolder folder) where T : class, new()
        {
            ExceptionHelper.ValidateStringNotNullOrWhiteSpace(fileName, nameof(fileName));
            ExceptionHelper.ValidateObjectNotNull(folder, nameof(folder));
            
            if (await folder.TryGetItemAsync(fileName) is StorageFile file)
            {
                return await LoadObjectAsync<T>(file);
            }
            else
            {
                return new StorageHelperLoadResult<T>(new T(), StorageHelperStatus.SuccessFileNotFound, new FileNotFoundException("Unable to find the specified file.", fileName));
            }
        }
        
        public static async Task<StorageHelperLoadResult<T>> LoadObjectAsync<T>(StorageFile file) where T : class, new()
        {
            ExceptionHelper.ValidateObjectNotNull(file, nameof(file));

            StorageHelperLoadResult<T> loadResult = new StorageHelperLoadResult<T>();

            // Reading from the file could fail while the file is used by another proccess
            try
            {
                string json = await FileIO.ReadTextAsync(file);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    loadResult.LoadedObject = await Task.Run(() => JsonConvert.DeserializeObject<T>(json));
                }
            }
            catch (Exception exception)
            {
                loadResult.Status       = StorageHelperStatus.Failure;
                loadResult.Exception    = exception;
            }

            Debug.WriteLine(file.Name + " loading " + (loadResult.Status != StorageHelperStatus.Failure ? "succeeded" : "failed"));
            return loadResult;
        }

        public static async Task<bool> FileExistsAsync(this StorageFolder folder, string fileName)
        {
            return await folder.TryGetItemAsync(fileName) != null;
        }
    }

    public enum StorageHelperStatus
    {
        Success,
        Failure,
        SuccessFileNotFound
    }

    public class StorageHelperResult
    {
        public StorageHelperStatus Status { get; internal set; }
        public Exception Exception { get; internal set; }

        internal StorageHelperResult()
        {
            Status = StorageHelperStatus.Success;
        }

        public StorageHelperResult(StorageHelperStatus status, Exception exception)
        {
            ExceptionHelper.ValidateEnumValueDefined(status, nameof(status));

            Status      = status;
            Exception   = exception;
        }
    }

    public sealed class StorageHelperLoadResult<T> : StorageHelperResult
    {
        public T LoadedObject { get; internal set; }

        internal StorageHelperLoadResult()
        {

        }

        public StorageHelperLoadResult(T loadedObject, StorageHelperStatus status, Exception exception) : base(status, exception)
        {
            LoadedObject = loadedObject;
        }
    }
}
