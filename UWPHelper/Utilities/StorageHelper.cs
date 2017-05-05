using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWPHelper.Utilities
{
    public static class StorageHelper
    {
        public static Task<StorageFileHelperResult> SaveObjectAsync(object obj, string fileName, StorageFolder folder)
        {
            return SaveObjectAsync(obj, fileName, folder, CreationCollisionOption.OpenIfExists);
        }
        
        public static async Task<StorageFileHelperResult> SaveObjectAsync(object obj, string fileName, StorageFolder folder, CreationCollisionOption creationCollisionOption)
        {
            ExceptionHelper.ValidateStringNotNullOrWhiteSpace(fileName, nameof(fileName));
            ExceptionHelper.ValidateObjectNotNull(folder, nameof(folder));
            ExceptionHelper.ValidateEnumValueDefined(creationCollisionOption, nameof(creationCollisionOption));

            StorageFileHelperResult result = new StorageFileHelperResult();

            try
            {
                StorageFile file    = await folder.CreateFileAsync(fileName, creationCollisionOption);
                string json         = await Task.Run(() => JsonConvert.SerializeObject(obj));

                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception exception)
            {
                result.Status       = StorageFileHelperStatus.Failure;
                result.Exception    = exception;
            }

            DebugHelper.OperationInfo(fileName, "saving", result.Status != StorageFileHelperStatus.Failure);
            return result;
        }
        
        public static async Task<StorageFileHelperLoadResult<T>> LoadObjectAsync<T>(string fileName, StorageFolder folder) where T : class, new()
        {
            ExceptionHelper.ValidateStringNotNullOrWhiteSpace(fileName, nameof(fileName));
            ExceptionHelper.ValidateObjectNotNull(folder, nameof(folder));
            
            if (await folder.TryGetItemAsync(fileName) is StorageFile file)
            {
                return await LoadObjectAsync<T>(file);
            }
            else
            {
                return new StorageFileHelperLoadResult<T>(new T(), StorageFileHelperStatus.SuccessFileNotFound, new FileNotFoundException("Unable to find the specified file.", fileName));
            }
        }
        
        public static async Task<StorageFileHelperLoadResult<T>> LoadObjectAsync<T>(StorageFile file) where T : class, new()
        {
            ExceptionHelper.ValidateObjectNotNull(file, nameof(file));

            StorageFileHelperLoadResult<T> loadResult = new StorageFileHelperLoadResult<T>();

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
                loadResult.Status       = StorageFileHelperStatus.Failure;
                loadResult.Exception    = exception;
            }

            DebugHelper.OperationInfo(file.Name, "loading", loadResult.Status != StorageFileHelperStatus.Failure);
            return loadResult;
        }
    }

    public enum StorageFileHelperStatus
    {
        Success,
        Failure,
        SuccessFileNotFound
    }

    public class StorageFileHelperResult
    {
        public StorageFileHelperStatus Status { get; internal set; }
        public Exception Exception { get; internal set; }

        internal StorageFileHelperResult()
        {
            Status = StorageFileHelperStatus.Success;
        }

        public StorageFileHelperResult(StorageFileHelperStatus status, Exception exception)
        {
            ExceptionHelper.ValidateEnumValueDefined(status, nameof(status));

            Status      = status;
            Exception   = exception;
        }
    }

    public sealed class StorageFileHelperLoadResult<T> : StorageFileHelperResult
    {
        public T LoadedObject { get; internal set; }

        internal StorageFileHelperLoadResult()
        {

        }

        public StorageFileHelperLoadResult(T loadedObject, StorageFileHelperStatus status, Exception exception) : base(status, exception)
        {
            LoadedObject = loadedObject;
        }
    }
}
