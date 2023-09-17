using System;
using System.IO;
using System.Threading.Tasks;
using CloudImageShrinker;
using MozJpegSharp;

namespace CloudImageShrinkerUWP
{
    public class UwpImageProcessor : IImageProcessor
    {
        public Task<byte[]> CompressImageAsync(MemoryStream stream, int wantedQuality)
        {
            var compressed = MozJpeg.Recompress(stream.ToArray().AsSpan(), quality: wantedQuality);
            return Task.FromResult(compressed);
        }
    }
}