using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudImageShrinker
{
    public interface ICloudService
    {
        Task<List<IImageToProcess>> LoadImagesToProcessAsync(string targetFolder);
    }
}