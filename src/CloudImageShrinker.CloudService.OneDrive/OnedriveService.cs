using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CloudImageShrinkerUWP;
using Newtonsoft.Json;

namespace CloudImageShrinker
{
    public class OnedriveService : ICloudService
    {
        private HttpClient _httpClient;

        public Task InitAsync(string accessToken)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return Task.CompletedTask;
        }

        public async Task<List<IImageToProcess>> LoadImagesToProcessAsync(string targetFolder)
        {
            // var targetFolder = "Pellicule SkyDrive\\2022\\09";
            var response = await _httpClient.GetAsync(
                new Uri($"https://graph.microsoft.com/v1.0/me/drive/root:/{targetFolder}:/children?$expand=thumbnails&orderby=size desc&filter=contains(file/mimeType, 'image/jpeg')"));
            var content = await response.Content.ReadAsStringAsync();
            var folderContent = JsonConvert.DeserializeObject<RootObject>(content);
            if (folderContent.value == null)
            {
                return new List<IImageToProcess>();
            }
            List<IImageToProcess> images = folderContent.value
                .Where(it => it.file.mimeType.Equals("image/jpeg"))
                .OrderByDescending(i => i.size)
                .Select(image => new ImageToProcess(image, _httpClient))
                .Cast<IImageToProcess>()
                .ToList();

            return images;
        }
    }
}