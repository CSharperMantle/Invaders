using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Invaders.Wpf.Commons;

namespace Invaders.Wpf.View
{
    public partial class AnimatedImage : UserControl
    {
        public AnimatedImage()
        {
            InitializeComponent();
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
            Storyboard.SetTargetProperty(animation, new PropertyPath("Source"));

            var currentInterval = TimeSpan.FromMilliseconds(0);
            foreach (var imageName in imageNames)
            {
                
                ObjectKeyFrame keyFrame = new DiscreteObjectKeyFrame();
                if (File.Exists(imageName))
                {
                    keyFrame.Value = CreateImageFromAssets(imageName);
                }
                else
                {
                    Log.C($"{imageName} does not exist");
                    throw new FileNotFoundException($"{imageName} does not exist");
                }
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
            if (obj is Storyboard storyboard)
                storyboard.Begin();
            else
                throw new ArgumentException(nameof(storyboard));
        }

        public void StartFlashing()
        {
            var obj = Resources["FlashStoryboard"];
            if (obj is Storyboard storyboard)
                storyboard.Begin();
            else
                throw new ArgumentException(nameof(storyboard));
        }

        public void StopFlashing()
        {
            var obj = Resources["FlashStoryboard"];
            if (obj is Storyboard storyboard)
                storyboard.Stop();
            else
                throw new ArgumentException(nameof(storyboard));
        }

        private static BitmapImage CreateImageFromAssets(string imageFileName)
        {
            try
            {
                var uri = new Uri($"Assets/{imageFileName}", UriKind.Relative);
                return new BitmapImage(uri);
            }
            catch (Exception e)
            {
                Log.E("Exception: " + e.GetType());
                Log.I(e.StackTrace);
                return new BitmapImage();
            }
        }
    }
}