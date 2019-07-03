using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Invaders.Wpf.Model;

namespace Invaders.Wpf.View
{
    public static class InvadersHelper
    {
        private static readonly Random _random = new Random();

        public static IEnumerable<string> CreateImageList(InvaderType invaderType)
        {
            string filename;
            switch (invaderType)
            {
                case InvaderType.Bug:
                    filename = "bug";
                    break;
                case InvaderType.Satellite:
                    filename = "satellite";
                    break;
                case InvaderType.Saucer:
                    filename = "flyingsaucer";
                    break;
                case InvaderType.Spaceship:
                    filename = "spaceship";
                    break;
                case InvaderType.Star:
                    filename = "star";
                    break;
                default:
                    throw new ArgumentException(nameof(invaderType));
            }

            var nameList = new List<string>();
            for (var index = 1; index <= 4; index++) nameList.Add(filename + index + ".png");
            nameList.Add(filename + "1.png");

            return nameList;
        }

        public static string GetMediaFileName(MediaType mediaType)
        {
            var filename = "";
            switch (mediaType)
            {
                case MediaType.NextWave:
                    filename = "charge.wav";
                    break;
                case MediaType.EndGame:
                    filename = "fogblast.wav";
                    break;
                case MediaType.PlayerShot:
                    filename = "factwhistle.wav";
                    break;
                default:
                    throw new ArgumentException(nameof(mediaType));
            }

            return "./Assets/" + filename;
        }

        internal static FrameworkElement InvaderControlFactory(Invader invader, double scale)
        {
            var imageNames = CreateImageList(invader.InvaderType);
            var invaderControl = new AnimatedImage(imageNames, TimeSpan.FromSeconds(.75));
            invaderControl.Width = invader.Size.Width * scale;
            invaderControl.Height = invader.Size.Height * scale;
            SetCanvasLocation(invaderControl, invader.Location.X * scale, invader.Location.Y * scale);
            Panel.SetZIndex(invaderControl, 1);
            return invaderControl;
        }

        internal static FrameworkElement ShotControlFactory(Shot shot, double scale)
        {
            var rectangle = new Rectangle();
            if (shot is BasicShot)
                rectangle.Fill = new SolidColorBrush(Colors.White);
            else if (shot is LazerShot)
                rectangle.Fill = new SolidColorBrush(Color.FromRgb(0x33, 0xFF, 0x00));
            else if (shot is HomingShot) rectangle.Fill = new SolidColorBrush(Colors.Yellow);
            rectangle.Width = shot.ShotSize.Width * scale;
            rectangle.Height = shot.ShotSize.Height * scale;
            SetCanvasLocation(rectangle, shot.Location.X * scale, shot.Location.Y * scale);
            Panel.SetZIndex(rectangle, 0);
            return rectangle;
        }

        internal static FrameworkElement StarControlFactory(Point point, double scale)
        {
            FrameworkElement star;
            switch (_random.Next(3))
            {
                case 0:
                    star = new Rectangle();
                    ((Rectangle) star).Fill = new SolidColorBrush(RandomStarColor());
                    star.Width = 2;
                    star.Height = 2;
                    break;
                case 1:
                    star = new Ellipse();
                    ((Ellipse) star).Fill = new SolidColorBrush(RandomStarColor());
                    star.Width = 2;
                    star.Height = 2;
                    break;
                default:
                    star = new StarControl();
                    ((StarControl) star).SetFill(new SolidColorBrush(RandomStarColor()));
                    break;
            }

            SetCanvasLocation(star, point.X * scale, point.Y * scale);
            Panel.SetZIndex(star, -1000);
            return star;
        }

        public static FrameworkElement ScanLineFactory(int y, int width, double scale)
        {
            var rectangle = new Rectangle();
            rectangle.Width = width * scale;
            rectangle.Height = 2;
            rectangle.Opacity = .1;
            rectangle.Fill = new SolidColorBrush(Colors.White);
            SetCanvasLocation(rectangle, 0, y * scale);
            return rectangle;
        }

        private static Color RandomStarColor()
        {
            switch (_random.Next(6))
            {
                case 0:
                    return Colors.White;
                case 1:
                    return Colors.LightBlue;
                case 2:
                    return Colors.MediumPurple;
                case 3:
                    return Colors.PaleVioletRed;
                case 4:
                    return Colors.Yellow;
                default:
                    return Colors.LightSlateGray;
            }
        }

        internal static FrameworkElement PlayerControlFactory(Player player, double scale)
        {
            var playerControl =
                new AnimatedImage(new List<string> {"player.png", "player.png"}, TimeSpan.FromSeconds(1));
            playerControl.Width = player.Size.Width * scale;
            playerControl.Height = player.Size.Height * scale;
            SetCanvasLocation(playerControl, player.Location.X * scale, player.Location.Y * scale);
            Panel.SetZIndex(playerControl, 1);
            return playerControl;
        }

        public static void SetCanvasLocation(FrameworkElement control, double x, double y)
        {
            Canvas.SetLeft(control, x);
            Canvas.SetTop(control, y);
        }

        public static void MoveElementOnCanvas(FrameworkElement frameworkElement, double toX, double toY)
        {
            var fromX = Canvas.GetLeft(frameworkElement);
            var fromY = Canvas.GetTop(frameworkElement);
            var storyboard = new Storyboard();
            var animationX = CreateDoubleAnimation(frameworkElement, fromX, toX, "(Canvas.Left)");
            var animationY = CreateDoubleAnimation(frameworkElement, fromY, toY, "(Canvas.Top)");
            storyboard.Children.Add(animationX);
            storyboard.Children.Add(animationY);
            storyboard.Begin();
        }

        public static DoubleAnimation CreateDoubleAnimation(FrameworkElement frameworkElement, double from, double to,
            string propertyToAnimate)
        {
            return CreateDoubleAnimation(frameworkElement, from, to, propertyToAnimate, TimeSpan.FromMilliseconds(25));
        }

        public static DoubleAnimation CreateDoubleAnimation(FrameworkElement frameworkElement, double from, double to,
            string propertyToAnimate, TimeSpan timeSpan)
        {
            var animation = new DoubleAnimation();
            Storyboard.SetTarget(animation, frameworkElement);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyToAnimate));
            animation.From = from;
            animation.To = to;
            animation.Duration = timeSpan;
            return animation;
        }

        public static void ResizeElement(FrameworkElement control, double width, double height)
        {
            if (control.Width != width) control.Width = width;
            if (control.Height != height) control.Height = height;
        }
    }
}