using Invaders.Uwp.ViewModel;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Invaders.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        private readonly InvadersViewModel _viewModel;

        public GamePage()
        {
            this.InitializeComponent();

            var viewModel = Resources["ViewModel"] as InvadersViewModel;
            if (viewModel != null) _viewModel = viewModel;

            _viewModel.NextWaveGenerated += ViewModelNextWaveGeneratedEventHandler;
            _viewModel.GameLost += ViewModelGameLostEventHandler;
            _viewModel.PlayerShot += ViewModelPlayerShotEventHandler;
        }

        private async Task PlayFromDisk(CoreDispatcher dispatcher, string filename)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                MediaElement playbackMediaElement = new MediaElement();
                StorageFolder storageFolder = Package.Current.InstalledLocation;
                StorageFile storageFile = await storageFolder.GetFileAsync(filename);
                IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read);
                playbackMediaElement.SetSource(stream, storageFile.FileType);
                playbackMediaElement.Play();
            });
        }

        private async void ViewModelPlayerShotEventHandler(object sender, EventArgs e)
        {
            //await PlayFromDisk(Dispatcher, InvadersHelper.GetMediaFileName(MediaType.PlayerShot));
        }

        private async void ViewModelGameLostEventHandler(object sender, EventArgs e)
        {
            /*var t = new Task(() =>
            {
                using (Stream music = File.OpenRead(InvadersHelper.GetMediaFileName(MediaType.EndGame)))
                using (var player = new SoundPlayer(music))
                {
                    player.PlaySync();
                }
            });
            t.Start();*/
            //await PlayFromDisk(Dispatcher, InvadersHelper.GetMediaFileName(MediaType.EndGame));
        }

        private async void ViewModelNextWaveGeneratedEventHandler(object sender, EventArgs e)
        {
            //await PlayFromDisk(Dispatcher, InvadersHelper.GetMediaFileName(MediaType.NextWave));
        }

        private void UpdatePlayAreaSize(Size newSize)
        {
            double targetWidth;
            double targetHeight;
            if (newSize.Width > newSize.Height)
            {
                targetWidth = newSize.Height * 4 / 3;
                targetHeight = newSize.Height;
                var leftRightMargin = (newSize.Width - targetWidth) / 2;
                PlayArea.Margin = new Thickness(leftRightMargin, 0, leftRightMargin, 0);
            }
            else
            {
                targetHeight = newSize.Width * 4 / 3;
                targetWidth = newSize.Width;
                var topBottoMarginMargin = (newSize.Height - targetHeight) / 2;
                PlayArea.Margin = new Thickness(0, topBottoMarginMargin, 0, topBottoMarginMargin);
            }

            PlayArea.Width = targetWidth;
            PlayArea.Height = targetHeight;
            _viewModel.PlayAreaSize = new Size(targetWidth, targetHeight);
        }

        private void BeginButton_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.StartGame();
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _viewModel.KeyDown(e.Key);
        }

        private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            _viewModel.KeyUp(e.Key);
        }

        private void PlayArea_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePlayAreaSize(PlayArea.RenderSize);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePlayAreaSize(new Size(e.NewSize.Width - 5, e.NewSize.Height - 160));
        }
    }
}
