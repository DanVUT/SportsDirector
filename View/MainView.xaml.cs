using System.Windows;

namespace MFormat.View
{
    using System;
    using ViewModel;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            (Application.Current.Resources["Locator"] as ViewModelLocator).Main.view = this;
        }
    }
}
