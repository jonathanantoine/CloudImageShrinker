using System.IO;
using System.Threading.Tasks;

namespace CloudImageShrinker
{
    public interface IImageToProcess
    {
        public string Name { get; }

        int Size { get; }

        string Thumbnail { get; }

        public int Width { get; }
        public int Height { get; }
        string FullPath { get; }

        public Task<Stream> GetStreamAsync();
    }
}