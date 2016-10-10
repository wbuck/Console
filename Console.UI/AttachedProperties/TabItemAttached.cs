using System;
using System.Windows;
using System.Windows.Media;

namespace Console.UI.AttachedProperties
{
    public class TabItemAttached
    {
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.RegisterAttached( "Data",
                typeof( string ),
                typeof( TabItemAttached ),
                new FrameworkPropertyMetadata( string.Empty, FrameworkPropertyMetadataOptions.AffectsRender ), ValidateData );

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.RegisterAttached( "Text",
               typeof( string ),
               typeof( TabItemAttached ),
               new PropertyMetadata( string.Empty ) );

        private static bool ValidateData( object value )
        {
            try
            {
                Geometry.Parse( ( string )value );
                return true;
            }
            catch( FormatException )
            {
                return false;
            }
        }

        public static string GetText( DependencyObject obj )
        {
            return ( string )obj.GetValue( TextProperty );
        }

        public static void SetText( DependencyObject obj, string text )
        {
            obj.SetValue( TextProperty, text );
        }

        public static string GetData( DependencyObject obj )
        {
            return ( string )obj.GetValue( DataProperty );
        }

        public static void SetData( DependencyObject obj, string data )
        {
            obj.SetValue( DataProperty, data );
        }
    }
}
