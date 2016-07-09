using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWPHelper.Utilities
{
    public static class Storage
    {
        public static async Task SaveObjectAsync(object obj, string fileName, StorageFolder folder)
        {

#if DEBUG
            bool success = true;
#endif

            try
            {
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                string json = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(obj));
                await FileIO.WriteTextAsync(file, json);
            }
            catch
            {

#if DEBUG
                success = false;
#endif

            }

#if DEBUG
            finally
            {
                DebugMessages.OperationInfo(fileName, Operation.Saving, success);
            }
#endif

        }

        public static async Task<T> LoadObjectAsync<T>(string fileName, StorageFolder folder) where T : class, new()
        {

#if DEBUG
            bool success = true;
#endif

            T output = null;

            try
            {
                StorageFile file = await folder.GetFileAsync(fileName);
                string json = await FileIO.ReadTextAsync(file);

                if (string.IsNullOrWhiteSpace(json))
                {
                    output = new T();
                }
                else
                {
                    output = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(json));
                }
            }
            catch (Exception ex)
            {
                output = new T();

#if DEBUG
                success = ex is FileNotFoundException;
#endif

            }

#if DEBUG
            finally
            {
                DebugMessages.OperationInfo(fileName, Operation.Loading, success);
            }
#endif

            return output;
        }
    }
}