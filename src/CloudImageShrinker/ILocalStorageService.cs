using System;
using System.IO;
using System.Threading.Tasks;

namespace CloudImageShrinker
{
    public interface ILocalStorageService
    {
        Task<Stream> OpenFileAsync(string compressedlLocalPath);
        Task<string> StoreLocallyAsync(string fullPath, string fileName, Stream originalStream);
    }
}