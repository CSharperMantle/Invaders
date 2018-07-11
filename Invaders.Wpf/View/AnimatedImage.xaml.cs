using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

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
            Storyboard storyboard = new Storyboard();
            ObjectAnimationUsingKeyFrames animation = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(animation, Image);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Source"));

            TimeSpan currentInterval = TimeSpan.FromMilliseconds(0);
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
            object obj = Resources["InvaderShotStoryboard"];
            var storyboard = obj as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
            else
            {
                throw new NullReferenceException(nameof(storyboard));
            }
            
        }

        public void StartFlashing()
        {
            object obj = Resources["FlashStoryboard"];
            var storyboard = obj as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
            else
            {
                throw new NullReferenceException(nameof(storyboard));
            }
        }
        
        public void StopFlashing()
        {
            object obj = Resources["FlashStoryboard"];
            var storyboard = obj as Storyboard;
            if (storyboard != null)
            {
                storyboard.Stop();
            }
            else
            {
                throw new NullReferenceException(nameof(storyboard));
            }
        }
        
        private static BitmapImage CreateImageFromAssets(string imageFileName)
        {
            try
            {
                Uri uri = new Uri("Assets/" + imageFileName, UriKind.Relative);
                return new BitmapImage(uri);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception: " + e.GetType());
                Console.Error.WriteLine(e.StackTrace);
                Console.Error.WriteLine("Message: " + e.Message);
                return new BitmapImage();
            }
            
        }
    }
}
