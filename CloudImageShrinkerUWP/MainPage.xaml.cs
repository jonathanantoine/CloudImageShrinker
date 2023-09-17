using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web.Core;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

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
                if (int.TryParse(WantedQualityElement.Text, out int wantedQuality))
                {
                    await new Shrinker(token, wantedQuality).ProcessAsync();

                    await ViewModel.ProcessItemsAsync();
                }
            }
        }

        private void Start(object sender, RoutedEventArgs e) => AccountsSettingsPane.Show();


        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.AddedItems.FirstOrDefault() as ProcessedImageItemViewModel;
            ViewModel.SelectedItem = null;
            await Task.Yield();
            ViewModel.SelectedItem = selectedItem;
            await FitImageInScrollViewer(selectedItem);
        }

        private async Task FitImageInScrollViewer(ProcessedImageItemViewModel selectedItem)
        {
            if (selectedItem == null)
            {
                return;
            }

            var infos = await selectedItem.Original.Properties.GetImagePropertiesAsync();

            // Obtenez la taille de l'image
            double imageWidth = infos.Width;
            double imageHeight = infos.Height;

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