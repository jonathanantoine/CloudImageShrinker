using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace CloudImageShrinkerUWP
{
    public class LocalProcessedImageItemViewModel
    {
        public StorageFile Original { get; }
        public StorageFile Compressed { get; }

        public LocalProcessedImageItemViewModel(StorageFile original, StorageFile compressed)
        {
            Original = original;
            Compressed = compressed;
        }

        public async Task InitAsync()
        {
            OriginalProperties = await Original.GetBasicPropertiesAsync();
            CompressedProperties = await Compressed.GetBasicPropertiesAsync();
        }

        private BasicProperties CompressedProperties { get; set; }

        private BasicProperties OriginalProperties { get; set; }

        public string Delta
        {
            get
            {
                if (CompressedProperties != null && OriginalProperties != null)
                {
                    return  ((CompressedProperties.Size / (float)OriginalProperties.Size) * 100).ToString("F2") + "%";
                }

                return "-";
            }
        }

        public string OriginalSize
        {
            get
            {
                if (OriginalProperties != null)
                {
                    return ((float)OriginalProperties.Size / (1024 * 1024)).ToString("F2") + "Mo";
                }

                return "-";
            }
        }

        public string CompressedSize
        {
            get
            {
                if (CompressedProperties != null)
                {
                    return ((float)CompressedProperties.Size / (1024 * 1024)).ToString("F2") + "Mo";
                }


                return "-";
            }
        }
    }
}