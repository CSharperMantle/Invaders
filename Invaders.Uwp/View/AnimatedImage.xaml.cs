using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace Invaders.Uwp.View
{
    public sealed partial class AnimatedImage : UserControl
    {
        public AnimatedImage()
        {
            this.InitializeComponent();
        }
        public AnimatedImage(IEnumerable<string> imageNames, TimeSpan interval) : this()
        {
            StartAnimation(imageNames, interval);
        }

        public void StartAnimation(IEnumerable<string> imageNames, TimeSpan interval)
        {
            var storyboard = new Storyboard();
            var animation = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(animation, Image);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Source").Path);

            var currentInterval = TimeSpan.FromMilliseconds(0);
            foreach (var imageName in imageNames)
            {
                ObjectKeyFrame keyFrame = new DiscreteObjectKeyFrame();
                keyFrame.Value = CreateImageFromAssets(imageName);
                keyFrame.KeyTime = currentInterval;
                animation.KeyFrames.Add(keyFrame);
                currentInterval = currentInterval.Add(interval);
            }

            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            storyboard.AutoReverse = true;
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        public void InvaderShot()
        {
            var obj = Resources["InvaderShotStoryboard"];
            var storyboard = obj as Storyboard;
            if (storyboard != null)
                storyboard.Begin();
            else
                throw new NullReferenceException(nameof(storyboard));
        }

        public void StartFlashing()
        {
            var obj = Resources["FlashStoryboard"];
            var storyboard = obj as Storyboard;
            if (storyboard != null)
                storyboard.Begin();
            else
                throw new NullReferenceException(nameof(storyboard));
        }

        public void StopFlashing()
        {
            var obj = Resources["FlashStoryboard"];
            var storyboard = obj as Storyboard;
            if (storyboard != null)
                storyboard.Stop();
            else
                throw new NullReferenceException(nameof(storyboard));
        }

        private static BitmapImage CreateImageFromAssets(string imageFileName)
        {
            try
            {
                var uri = new Uri("Assets/" + imageFileName, UriKind.Relative);
                return new BitmapImage(uri);
            }
            catch (Exception e)
            {
                //TODO: Add handler
                return new BitmapImage();
            }
        }
    }
}
