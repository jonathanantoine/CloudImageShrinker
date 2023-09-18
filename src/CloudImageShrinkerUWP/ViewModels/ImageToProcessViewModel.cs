using System.IO;
using System.Threading.Tasks;
using CloudImageShrinker;

namespace CloudImageShrinkerUWP
{
    public class ImageToProcessViewModel : BindableBase
    {
        private string _originalLocalPath;
        private string _statusText;
        private bool _isProcessing;
        private bool _isProcessed;
        private string _compressedlLocalPath;
        private string _compressedSize;
        private string _compressionDelta;
        public IImageToProcess Data { get; }

        public ImageToProcessViewModel(IImageToProcess data) => Data = data;

        public string Size => ((float) Data.Size / (1024 * 1024)).ToString("F2") + "Mo";

        public string CompressedSize
        {
            get => _compressedSize;
            private set => SetProperty(ref _compressedSize, value);
        }

        public string OriginalLocalPath
        {
            get => _originalLocalPath;
            private set => SetProperty(ref _originalLocalPath, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        public bool IsProcessed
        {
            get => _isProcessed;
            set => SetProperty(ref _isProcessed, value);
        }

        public string CompressedlLocalPath
        {
            get => _compressedlLocalPath;
            set => SetProperty(ref _compressedlLocalPath, value);
        }

        public string CompressionDelta
        {
            get => _compressionDelta;
            set => SetProperty(ref _compressionDelta, value);
        }

        public async Task ProcessAsync(int wantedQuality)
        {
            var localStorageService = ServiceLocator.Resolve<ILocalStorageService>();
            var imageProcessor = ServiceLocator.Resolve<IImageProcessor>();

            if (IsProcessed)
            {
                return;
            }

            try
            {
                IsProcessing = true;
                StatusText = "Downloading";
                using (var originalStream = await Data.GetStreamAsync())
                {

                    using (var memStream = new MemoryStream())
                    {
                        await originalStream.CopyToAsync(memStream);
                        memStream.Seek(0, SeekOrigin.Begin);
                        StatusText = "Creating local version";

                        OriginalLocalPath = await localStorageService.StoreLocallyAsync(Data.FullPath, Data.Name, memStream);

                        StatusText = "Compressing";
                        var compressedBytes = await imageProcessor.CompressImageAsync(memStream, wantedQuality);
                        CompressedSize = SizeToMoString(compressedBytes.Length);

                        CompressionDelta = "-" + SizeToMoString(Data.Size - compressedBytes.Length);
                        using (var compressedStream = new MemoryStream(compressedBytes))
                        {
                            var compressedName = Path.ChangeExtension(Data.Name, Constants.COMPRESSED_EXTENSION + Path.GetExtension(Data.Name));
                            CompressedlLocalPath = await localStorageService.StoreLocallyAsync(Data.FullPath, compressedName, compressedStream);
                        }
                    }
                }

                IsProcessed = true;
            }
            finally
            {
                IsProcessing = false;
            }
        }


        private static string SizeToMoString(int compressedBytesLength)
        {
            return ((float) compressedBytesLength / (1024 * 1024)).ToString("F2") + "Mo";
        }
    }
}