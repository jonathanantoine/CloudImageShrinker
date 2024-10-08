﻿using System;
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
        private bool _canBeCompressed;

        private string _compressedlLocalPath;
        private string _compressedSize;
        private string _compressionDelta;
        public IImageToProcess Data { get; }

        public ImageToProcessViewModel(IImageToProcess data)
        {
            Data = data;
            CanBeCompressed = data.Name.IndexOf(Constants.COMPRESSED_EXTENSION) < 0;
        }

        public string Size => ((float)Data.Size / (1024 * 1024)).ToString("F2") + "Mo";

        public string CompressedSize
        {
            get => _compressedSize;
            private set => SetProperty(ref _compressedSize, value);
        }

        public int CompressedSizeBytes { get; private set; } = 0;

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
            set
            {
                if (SetProperty(ref _isProcessing, value))
                {
                    RaisePropertyChanged(nameof(CanBeCompressed));
                    RaisePropertyChanged(nameof(CanBeUploaded));
                }
            }
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
        public int DeltaInBytes { get; private set; }

        public bool CanBeCompressed
        {
            get => _canBeCompressed;
            set
            {
                if (SetProperty(ref _canBeCompressed, value))
                {
                    RaisePropertyChanged(nameof(CanBeUploaded));
                }

            }
        }
        public bool CanBeUploaded
        {
            get => _canBeCompressed && !IsProcessing;
            set => SetProperty(ref _canBeCompressed, value);
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

                        if (!CanBeCompressed)
                        {
                            IsProcessed = true;
                            return;
                        }

                        StatusText = "Compressing";
                        var compressedBytes = await imageProcessor.CompressImageAsync(memStream, wantedQuality);
                        CompressedSizeBytes = compressedBytes.Length;
                        CompressedSize = SizeToMoString(compressedBytes.Length);

                        CompressionDelta = "-" + SizeToMoString(Data.Size - compressedBytes.Length);
                        DeltaInBytes = Data.Size - compressedBytes.Length;
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
            return ((float)compressedBytesLength / (1024 * 1024)).ToString("F2") + "Mo";
        }

        internal async Task ReplaceOriginalByCompressedAsync()
        {
            if (string.IsNullOrEmpty(CompressedlLocalPath))
            {
                return;
            }
            try
            {
                IsProcessing = true;
                StatusText = "Uploading";

                var localStorageService = ServiceLocator.Resolve<ILocalStorageService>();
                var cloudService = ServiceLocator.Resolve<ICloudService>();

                using (var compressedStream = await localStorageService.OpenFileAsync(CompressedlLocalPath))
                {
                    var compressedFullPath = Path.ChangeExtension(Data.FullPath, Constants.COMPRESSED_EXTENSION + Path.GetExtension(Data.FullPath));

                    await cloudService.ReplaceFileAsync(Data.FullPath, compressedFullPath, compressedStream);
                    CanBeCompressed = false;
                }
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}