using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;


namespace Invaders.Uwp.View
{
    public sealed partial class StarControl : UserControl
    {
        public StarControl()
        {
            InitializeComponent();
        }

        public void SetFill(SolidColorBrush solidColorBrush)
        {
            StarPolygon.Fill = solidColorBrush;
        }
    }
}
