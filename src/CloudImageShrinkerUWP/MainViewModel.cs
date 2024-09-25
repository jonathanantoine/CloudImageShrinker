using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CloudImageShrinker;

namespace CloudImageShrinkerUWP
{
    public class MainViewModel : BindableBase
    {
        private const int MegaOctet = 1024 * 1024;
        private ObservableCollection<ImageToProcessViewModel> _items;

        public ObservableCollection<ImageToProcessViewModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private string _targetFolder = "Pellicule SkyDrive\\2022\\09";

        public string TargetFolder
        {
            get => _targetFolder;
            set => SetProperty(ref _targetFolder, value);
        }

        private ImageToProcessViewModel _selectedItem;

        public ImageToProcessViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                value?.ProcessAsync(WantedQuality);
            }
        }

        public int WantedQuality
        {
            get => _wantedQuality;
            set => SetProperty(ref _wantedQuality, value);
        }

        public bool IsCompressedHidden
        {
            get => _isCompressedHidden;
            set => SetProperty(ref _isCompressedHidden, value);
        }

        private bool _canCompressAllFolder;

        public bool CanCompressAllFolder
        {
            get => _canCompressAllFolder;
            set => SetProperty(ref _canCompressAllFolder, value);
        }



        private bool _isProcessing;

        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                SetProperty(ref _isProcessing, value);
                RaisePropertyChanged(nameof(IsNotProcessing));
            }
        }

        public bool IsNotProcessing => !IsProcessing;

        private string _processingState;

        public string ProcessingState
        {
            get => _processingState;
            set => SetProperty(ref _processingState, value);
        }


        public ICloudService CloudService { get; set; }
        public bool IsConnected => CloudService != null;

        private bool _isCompressedHidden;
        private int _wantedQuality = 90;


        public MainViewModel() => InitAsync();

        private async Task InitAsync()
        {
        }

        private string ExtractFileStart(string itemName)
            => itemName.Replace("." + Constants.COMPRESSED_EXTENSION, string.Empty);

        public async Task LoadItemsFromCloudAsync()
        {
            CanCompressAllFolder = false;
            var imagesToProcess = await CloudService.LoadImagesToProcessAsync(TargetFolder);
            var items = imagesToProcess.Select(im => new ImageToProcessViewModel(im));
            Items = new ObservableCollection<ImageToProcessViewModel>(items);
            CanCompressAllFolder = Items?.Any() == true;
        }

        internal async Task CompressAllFolderAsync()
        {
            CanCompressAllFolder = false;

            IsProcessing = true;
            var target = Items.Where(i => i.CanBeCompressed).ToArray();
            int processed = 0;
            int ignored = 0;
            try
            {
                for (int i = 0; i < target.Length; i++)
                {
                    ProcessingState = $"Processing item {i} of {target.Length}.{Environment.NewLine}Ignored : {ignored} of {processed}";
                    ImageToProcessViewModel item = target[i];
                    await item.ProcessAsync(WantedQuality);
                    bool enoughDifference = item.CompressedSizeBytes * 1.15f < item.Data.Size;
                    bool enoughDelta = item.DeltaInBytes > 2 * MegaOctet && item.DeltaInBytes > 0;
                    if (enoughDifference && enoughDelta)
                    {
                        await item.ReplaceOriginalByCompressedAsync();
                    }
                    else
                    {
                        ignored++;
                    }
                    processed++;
                }

            }
            catch (Exception e)
            {
                ProcessingState = $"{processed} processed but error happened : {e}";
            }
            CanCompressAllFolder = true;
            IsProcessing = false;
            LoadItemsFromCloudAsync();
        }
    }
}