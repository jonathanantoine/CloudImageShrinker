using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using CloudImageShrinker;

namespace CloudImageShrinkerUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = DataContext as MainViewModel;
        }

        public MainViewModel ViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += BuildPaneAsync;

            base.OnNavigatedTo(e);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested -= BuildPaneAsync;

            base.OnNavigatedFrom(e);
        }

        private async void BuildPaneAsync(AccountsSettingsPane sender, AccountsSettingsPaneCommandsRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();
            var msaProvider = await WebAuthenticationCoreManager.FindAccountProviderAsync("https://login.microsoft.com", "consumers");

            var command = new WebAccountProviderCommand(msaProvider, GetMsaTokenAsync);

            args.WebAccountProviderCommands.Add(command);
            deferral.Complete();
        }

        private async void GetMsaTokenAsync(WebAccountProviderCommand command)
        {
            WebTokenRequest request = new WebTokenRequest(command.WebAccountProvider, "files.read");
            WebTokenRequestResult result = await WebAuthenticationCoreManager.RequestTokenAsync(request);
            if (result.ResponseStatus == WebTokenRequestStatus.Success)
            {
                string token = result.ResponseData[0].Token;

                // await new Shrinker(token, wantedQuality).ProcessAsync();

                var cloudService = new OnedriveService();
                await cloudService.InitAsync(token);
                ViewModel.CloudService = cloudService;

                await ViewModel.LoadItemsFromCloudAsync(targetFolder: "Pellicule SkyDrive\\2022\\09");
            }
        }

        private void Start(object sender, RoutedEventArgs e) => AccountsSettingsPane.Show();


        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.AddedItems.FirstOrDefault() as ImageToProcessViewModel;
            ViewModel.SelectedItem = null;
            await Task.Yield();
            ViewModel.SelectedItem = selectedItem;
            FitImageInScrollViewer(selectedItem);
        }

        private void FitImageInScrollViewer(ImageToProcessViewModel selectedItem)
        {
            if (selectedItem == null)
            {
                return;
            }

            // Obtenez la taille de l'image
            double imageWidth = selectedItem.Data.Width;
            double imageHeight = selectedItem.Data.Height;

            // Obtenez la taille du ScrollViewer
            double scrollViewerWidth = MainScrollViewer.ActualWidth;
            double scrollViewerHeight = MainScrollViewer.ActualHeight;

            // Calculez le ZoomFactor nécessaire pour afficher l'image en entier
            double horizontalZoom = scrollViewerWidth / imageWidth;
            double verticalZoom = scrollViewerHeight / imageHeight;
            double zoomFactor = Math.Min(horizontalZoom, verticalZoom);

            // Définissez le ZoomFactor du ScrollViewer
            MainScrollViewer.ChangeView(null, null, (float) zoomFactor);

            // Calculez la position X et Y pour centrer l'image
            double centerX = (imageWidth * zoomFactor - scrollViewerWidth) / 2;
            double centerY = (imageHeight * zoomFactor - scrollViewerHeight) / 2;

            // Définissez la position du ScrollViewer pour centrer l'image
            MainScrollViewer.ChangeView(centerX, centerY, null);
        }

        private void UIElement_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var element = sender as UIElement;
            element.CapturePointer(e.Pointer);
            ViewModel.IsCompressedHidden = true;
            element.Opacity = 0;
            Indicator.Opacity = 100;
        }

        private void UIElement_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var element = sender as UIElement;
            element.Opacity = 100;
            ViewModel.IsCompressedHidden = false;
            Indicator.Opacity = 0;
        }
    }
}