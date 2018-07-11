using System;
using System.Windows;
using System.Windows.Input;
using Invaders.Wpf.ViewModel;

namespace Invaders.Wpf.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly InvadersViewModel _viewModel;
        
        public MainWindow()
        {
            InitializeComponent();

            object vm = Resources["ViewModel"];
            var model = vm as InvadersViewModel;
            if (model != null)
            {
                _viewModel = model;
            }
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePlayAreaSize(new Size(e.NewSize.Width, e.NewSize.Height));
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
                double leftRightMargin = (newSize.Width - targetWidth) / 2;
                PlayArea.Margin = new Thickness(leftRightMargin, 0, leftRightMargin, 0);
            }
            else
            {
                targetHeight = newSize.Width * 4 / 3;
                targetWidth = newSize.Width;
                double topBottoMarginMargin = (newSize.Height - targetHeight) / 2;
                PlayArea.Margin = new Thickness(0, topBottoMarginMargin, 0, topBottoMarginMargin);
            }

            PlayArea.Width = targetWidth;
            PlayArea.Height = targetHeight;
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