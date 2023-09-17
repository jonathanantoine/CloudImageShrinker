using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace CloudImageShrinkerUWP
{
    public class MainViewModel : BindableBase
    {
        public ObservableCollection<ProcessedImageItemViewModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public ProcessedImageItemViewModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public bool IsCompressedHidden
        {
            get => _isCompressedHidden;
            set => SetProperty(ref _isCompressedHidden, value);
        }

        private StorageFolder _imagesFolder;
        private ObservableCollection<ProcessedImageItemViewModel> _items;
        private ProcessedImageItemViewModel _selectedItem;
        private bool _isCompressedHidden;

        public MainViewModel() => InitAsync();

        private async Task InitAsync()
        {
            _imagesFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(Constants.WORKING_FOLDER);
            ProcessItemsAsync();
        }

        public async Task ProcessItemsAsync()
        {
            var files = await _imagesFolder.GetFilesAsync();

            var items = files
                .GroupBy(item => ExtractFileStart(item.Name))
                .Where(g => g.FirstOrDefault() != null && g.Skip(1).FirstOrDefault() != null)
                .Select(gro => new ProcessedImageItemViewModel(
                    gro.First(i => i.Name.LastIndexOf("." + Constants.COMPRESSED_EXTENSION, StringComparison.CurrentCultureIgnoreCase) < 0),
                    gro.First(i => i.Name.LastIndexOf("." + Constants.COMPRESSED_EXTENSION, StringComparison.OrdinalIgnoreCase) >= 0)))
                .ToArray();


            foreach (var itemViewModel in items)
            {
                await itemViewModel.InitAsync();
            }

            Items = new ObservableCollection<ProcessedImageItemViewModel>(items);
        }

        private string ExtractFileStart(string itemName)
            => itemName.Replace("." + Constants.COMPRESSED_EXTENSION, string.Empty);
    }
}