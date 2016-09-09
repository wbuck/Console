using System.Windows.Controls;
using System.Windows.Input;

namespace Console.UI.Views
{
    /// <summary>
    /// Interaction logic for PrinterStatusView.xaml
    /// </summary>
    public partial class PrinterStatusView : UserControl
    {
        public PrinterStatusView( )
        {
            InitializeComponent( );
        }

        // Prevents the Windows boundry feedback.
        // When a scroll viewer or list box (or anything that scrolls)  
        // are touch manipulated at their boundry the
        // entire windows will move to signal to
        // the user that they have reached the 
        // boundry. Setting this to true prevents
        // that from happening.
        private void UIElement_OnManipulationBoundaryFeedback( object sender, ManipulationBoundaryFeedbackEventArgs e )
        {
            e.Handled = true;
        }
    }
}
