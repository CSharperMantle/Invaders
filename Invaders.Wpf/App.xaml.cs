using System;
using System.Windows;
using Invaders.Wpf.Commons;

namespace Invaders.Wpf
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        AppLifeCycleManager lifeCycleManager = new AppLifeCycleManager();

        public App() : base()
        {
            this.Activated += lifeCycleManager.OnAppActivated;
            this.Deactivated += lifeCycleManager.OnAppDeactivated;
        }
    }
}