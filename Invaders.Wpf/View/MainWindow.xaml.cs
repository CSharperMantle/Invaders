using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Invaders.Wpf.Commons;
using Invaders.Wpf.ViewModel;

namespace Invaders.Wpf.View
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly InvadersViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            var viewModel = Resources["ViewModel"] as InvadersViewModel;
            if (viewModel != null) _viewModel = viewModel;

            _viewModel.NextWaveGenerated += ViewModelNextWaveGeneratedEventHandler;
            _viewModel.GameLost += ViewModelGameLostEventHandler;
            _viewModel.PlayerShot += ViewModelPlayerShotEventHandler;
        }

        private void ViewModelPlayerShotEventHandler(object sender, EventArgs e)
        {
            var t = new Thread(() =>
            {
                using (var music = File.OpenRead(InvadersHelper.GetMediaFileName(MediaType.PlayerShot)))
                using (var player = new SoundPlayer(music))
                {
                    player.PlaySync();
                }
            });
            Log.Debug(t.ToString());
            t.Start();
            Log.Debug("Thread started!");
        }

        private void ViewModelGameLostEventHandler(object sender, EventArgs e)
        {
            var t = new Thread(() =>
            {
                using (var music = File.OpenRead(InvadersHelper.GetMediaFileName(MediaType.EndGame)))
                using (var player = new SoundPlayer(music))
                {
                    player.PlaySync();
                }
            });
            Log.Debug(t.ToString());
            t.Start();
            Log.Debug("Thread started!");
        }

        private void ViewModelNextWaveGeneratedEventHandler(object sender, EventArgs e)
        {
            var t = new Thread(() =>
            {
                using (var music = File.OpenRead(InvadersHelper.GetMediaFileName(MediaType.NextWave)))
                using (var player = new SoundPlayer(music))
                {
                    player.PlaySync();
                }
            });
            Log.Debug(t.ToString());
            t.Start();
            Log.Debug("Thread started!");
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePlayAreaSize(new Size(e.NewSize.Width - 10, e.NewSize.Height - 160));
        }

        private void PlayArea_OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdatePlayAreaSize(PlayArea.RenderSize);
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

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            _viewModel.KeyDown(e.Key);
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            _viewModel.KeyUp(e.Key);
        }

        private void BeginButton_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.StartGame();
        }
    }
}