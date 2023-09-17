using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CloudImageShrinker;

namespace CloudImageShrinkerUWP
{
    public class MainViewModel : BindableBase
    {
        private ObservableCollection<ImageToProcessViewModel> _items;

        public ObservableCollection<ImageToProcessViewModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
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

        public ICloudService CloudService { get; set; }

        private bool _isCompressedHidden;
        private int _wantedQuality = 90;

        public MainViewModel() => InitAsync();

        private async Task InitAsync()
        {
        }

        private string ExtractFileStart(string itemName)
            => itemName.Replace("." + Constants.COMPRESSED_EXTENSION, string.Empty);

        public async Task LoadItemsFromCloudAsync(string targetFolder)
        {
            var imagesToProcess = await CloudService.LoadImagesToProcessAsync(targetFolder);
            var items = imagesToProcess.Select(im => new ImageToProcessViewModel(im));
            Items = new ObservableCollection<ImageToProcessViewModel>(items);
        }
    }
}