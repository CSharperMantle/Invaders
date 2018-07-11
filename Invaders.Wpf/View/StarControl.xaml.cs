using System.Windows.Controls;
using System.Windows.Media;

namespace Invaders.Wpf.View
{
    public partial class StarControl : UserControl
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
