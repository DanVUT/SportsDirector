/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:MFormat"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace MFormat.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ImageListViewModel>();
            SimpleIoc.Default.Register<SettingsDialogViewModel>();
            SimpleIoc.Default.Register<RecordingSettingsDialogViewModel>();
            SimpleIoc.Default.Register<HighlightsListViewModel>();
            SimpleIoc.Default.Register<VideoListViewModel>();
            SimpleIoc.Default.Register<LoadHighligtsDialogViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        
        public ImageListViewModel imageListViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ImageListViewModel>();
            }
        }

        public SettingsDialogViewModel settingsDialogViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsDialogViewModel>(System.Guid.NewGuid().ToString());
            }
        }

        public RecordingSettingsDialogViewModel recordingSettingsDialogViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RecordingSettingsDialogViewModel>(System.Guid.NewGuid().ToString());
            }
        }

        public HighlightsListViewModel highlightsListViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HighlightsListViewModel>();
            }
        }

        public LoadHighligtsDialogViewModel loadHighligtsDialogViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoadHighligtsDialogViewModel>(System.Guid.NewGuid().ToString());
            }
        }

        public VideoListViewModel videoListViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VideoListViewModel>();
            }
        }



        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}