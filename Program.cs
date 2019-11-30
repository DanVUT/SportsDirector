using MFormat.Model;
using MFormat.ViewModel;
using System;
using System.Windows;

namespace MFormat
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                var app = new App();
                app.InitializeComponent();
                var mainView = new View.MainView();
                // to handle window event
                Settings settings = Settings.Instance;
                mainView.Closing += (obj, ars) => (App.Current.Resources["Locator"] as ViewModelLocator).Main.CloseWindow();
                app.Run(mainView);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
