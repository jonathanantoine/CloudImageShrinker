using System.IO;
using System.Threading.Tasks;

namespace CloudImageShrinker
{
    public interface IImageProcessor
    {
        Task<byte[]> CompressImageAsync(MemoryStream stream, int wantedQuality);
    }
}