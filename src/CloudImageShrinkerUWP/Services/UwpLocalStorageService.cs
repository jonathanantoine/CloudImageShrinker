using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using CloudImageShrinker;

namespace CloudImageShrinkerUWP.Services
{
    public class UwpLocalStorageService : ILocalStorageService
    {
        public Task<Stream> OpenFileAsync(string compressedlLocalPath)
        {
            return Task.FromResult<Stream>( new FileStream(compressedlLocalPath,FileMode.Open));
        }

        public async Task<string> StoreLocallyAsync(string fullPath, string fileName, Stream originalStream)
        {
            var folderPath = Uri.EscapeDataString(Path.GetDirectoryName(fullPath));
            var storageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(folderPath, CreationCollisionOption.OpenIfExists);

            try
            {
                var existingFile = await storageFolder.GetFileAsync(fileName);
                var basicProperties = await existingFile.GetBasicPropertiesAsync();
                
                // if the file is already here with the same size : no need to download it again.
                if ((ulong) originalStream.Length == basicProperties.Size)
                {
                    return existingFile.Path;
                }
            }
            catch (FileNotFoundException e)
            {
                // expected behavior
            }

            var fileStorage = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (var fileStream = await fileStorage.OpenAsync(FileAccessMode.ReadWrite))
            {
                await originalStream.CopyToAsync(fileStream.AsStreamForWrite());
            }

            return fileStorage.Path;
        }
    }
}