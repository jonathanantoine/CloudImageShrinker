using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using ExifLibrary;
using MozJpegSharp;
using Newtonsoft.Json;

namespace CloudImageShrinkerUWP
{
    public class Shrinker
    {
        private readonly int _wantedQuality;
        private readonly HttpClient _httpClient;

        public Shrinker(string accessToken, int wantedQuality)
        {
            _wantedQuality = wantedQuality;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("Bearer", accessToken);
        }


        public async Task ProcessAsync()
        {
            try
            {
                var rootFolder =
                    await ApplicationData.Current.LocalFolder.CreateFolderAsync(Constants.WORKING_FOLDER, CreationCollisionOption.OpenIfExists);

                var response = await _httpClient.GetAsync(
                    new Uri("https://graph.microsoft.com/v1.0/me/drive/root:/Pellicule SkyDrive\\2022\\09:/children"));
                var content = await response.Content.ReadAsStringAsync();
                var folderContent = JsonConvert.DeserializeObject<RootObject>(content);
                var images = folderContent.value.Where(it => it.file.mimeType.Equals("image/jpeg")).OrderByDescending(i => i.size).ToArray();

                for (int i = 0; i < 50 && i < images.Length; i++)
                {
                    await ProcessImageAsync(images[i], rootFolder);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task ProcessImageAsync(Value image, StorageFolder storageFolder)
        {
            var notCompressedFile = await storageFolder.CreateFileAsync(image.name, CreationCollisionOption.ReplaceExisting);
            var newName = Path.ChangeExtension(image.name, Constants.COMPRESSED_EXTENSION + Path.GetExtension(image.name));
            var compressedFile = await storageFolder.CreateFileAsync(newName, CreationCollisionOption.ReplaceExisting);

            using (var response = await _httpClient.GetInputStreamAsync(new Uri(image.DownloadUrl)))
            using (var memStream = new MemoryStream())
            {
                // keep a reference
                await response.AsStreamForRead().CopyToAsync(memStream);
                memStream.Seek(0, SeekOrigin.Begin);
                // copy original
                using (var fileStream = await notCompressedFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await memStream.CopyToAsync(fileStream.AsStreamForWrite());
                }

                //var allProperties = await notCompressedFile.Properties.GetImagePropertiesAsync();
                var imageFile = ImageFile.FromFile(notCompressedFile.Path);
                // var originalProperties = imageFile.Properties.ToArray();

                var compressed = MozJpeg.Recompress(memStream.ToArray().AsSpan(), quality: _wantedQuality);
                using (var fileStream = await compressedFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await fileStream.AsStreamForWrite().WriteAsync(compressed, 0, compressed.Length);
                }

                var compressedExifFile = ImageFile.FromFile(compressedFile.Path);
                compressedExifFile.Properties.Clear();
                foreach (var originalProperty in imageFile.Properties)
                {
                    compressedExifFile.Properties.Set(originalProperty);
                }

                await compressedExifFile.SaveAsync(compressedFile.Path);
                // var originalProperties = ImageFile.FromFile(compressedFile.Path).Properties.se


                // compressedFile.Properties.SavePropertiesAsync(allProperties.);
            }
        }
    }
}