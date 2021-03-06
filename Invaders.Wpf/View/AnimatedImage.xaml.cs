﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Invaders.Wpf.View
{
    public partial class AnimatedImage
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
            var uri = new Uri($"Assets/{imageFileName}", UriKind.Relative);
            if (!File.Exists(uri.OriginalString))
                throw new FileNotFoundException("asset file not found", uri.OriginalString);
            return new BitmapImage(uri);
        }
    }
}