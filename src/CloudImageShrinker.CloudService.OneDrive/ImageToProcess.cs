using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CloudImageShrinkerUWP;

namespace CloudImageShrinker
{
    public class ImageToProcess : IImageToProcess
    {
        private readonly Value _onedriveImage;
        private readonly HttpClient _httpClient;

        internal ImageToProcess(Value onedriveImage, HttpClient httpClient)
        {
            _onedriveImage = onedriveImage;
            _httpClient = httpClient;
        }

        public string Name => _onedriveImage.name;
        public int Size => _onedriveImage.size;

        public string Thumbnail => _onedriveImage.thumbnails.FirstOrDefault()?.medium.url;
        public int Width => _onedriveImage.image.width;
        public int Height => _onedriveImage.image.height;
        public string FullPath => _onedriveImage.parentReference.path?.Replace("/drive/root:/", string.Empty) + "/" + _onedriveImage.name;

        public Task<Stream> GetStreamAsync() => _httpClient.GetStreamAsync(new Uri(_onedriveImage.DownloadUrl));
    }
}