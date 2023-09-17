using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using CloudImageShrinker;

namespace CloudImageShrinkerUWP.Services
{
    public class UwpLocalStorageService : ILocalStorageService
    {
        public async Task<string> StoreLocallyAsync(string fullPath, string fileName, Stream originalStream)
        {
            var folderPath = Path.GetDirectoryName(fullPath);
            var storageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(folderPath,CreationCollisionOption.OpenIfExists);

            var fileStorage = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (var fileStream = await fileStorage.OpenAsync(FileAccessMode.ReadWrite))
            {
                await originalStream.CopyToAsync(fileStream.AsStreamForWrite());
            }

            return fileStorage.Path;
        }
    }
}