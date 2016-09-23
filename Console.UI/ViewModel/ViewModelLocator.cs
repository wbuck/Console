/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Console.UI"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;

namespace Console.UI.ViewModel
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
        public ViewModelLocator( )
        {
            ServiceLocator.SetLocatorProvider( ( ) => SimpleIoc.Default );

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
            SimpleIoc.Default.Register<IMessenger, Messenger>( );

            SimpleIoc.Default.Register<MainViewModel>( );
            SimpleIoc.Default.Register<AlignmentViewModel>( );
            SimpleIoc.Default.Register<JobQueueViewModel>( );
            SimpleIoc.Default.Register<SettingsViewModel>( );
            SimpleIoc.Default.Register<JobQueueItemViewModel>( );
            SimpleIoc.Default.Register<PrinterStatusViewModel>( );
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public AlignmentViewModel Alignment => ServiceLocator.Current.GetInstance<AlignmentViewModel>( );

        public JobQueueViewModel JobQueue => ServiceLocator.Current.GetInstance<JobQueueViewModel>( );

        public SettingsViewModel Settings => ServiceLocator.Current.GetInstance<SettingsViewModel>( );

        public JobQueueItemViewModel JobQueueItem => ServiceLocator.Current.GetInstance<JobQueueItemViewModel>( );

        public PrinterStatusViewModel PrinterStatus => ServiceLocator.Current.GetInstance<PrinterStatusViewModel>( );

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}