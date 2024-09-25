using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CloudImageShrinker
{
    public interface ICloudService
    {
        Task InitAsync(string token);
        Task<List<IImageToProcess>> LoadImagesToProcessAsync(string targetFolder);

        Task ReplaceFileAsync(string filePath,string compressedFullPath, Stream newContent);
    }
}