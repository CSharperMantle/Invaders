using System;
using System.Windows;

namespace Invaders.Wpf
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            this.Activated += this.ApplicationActivedEventHandler;
            this.Deactivated += this.ApplicationDeactivedEventHandler;
        }

        private void ApplicationActivedEventHandler(object sender, EventArgs e)
        {

        }

        private void ApplicationDeactivedEventHandler(object sender, EventArgs e)
        {

        }
    }
}