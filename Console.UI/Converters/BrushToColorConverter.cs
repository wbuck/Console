using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace Console.UI.Converters
{
    [ValueConversion( typeof( SolidColorBrush ), typeof( Color ) )]
    public class BrushToColorConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if( value == null )
                return null;

            var brush = value as SolidColorBrush;

            if( brush != null )
                return brush.Color;

            throw new InvalidOperationException(
                $"Unsupported type [{value.GetType( ).Name}], BrushToColorConverter.Convert()" );
        }


        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException( );
        }
    }
}
