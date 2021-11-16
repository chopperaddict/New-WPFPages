using System;
using System . Collections . Generic;
using System . Globalization;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Data;
using System . Windows . Media;

namespace WPFPages . Converts
{
        class StringtoBrushConverter : IValueConverter
        {
                public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
                {
                        string str = targetType . ToString ( );
                        // check if we are requesting a Brush
                        if ( str . Contains ( "System.Windows.Media.Brush" ) )
                        {
                                if ( ( string ) parameter == "" )
                                        return ( Brush ) null;

                                Brush brush = ( Brush ) Application . Current . FindResource ( parameter as string );
                                return brush;
                        }
                        else
                                return ( Brush ) null;
                }

                public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
                {
                        throw new NotImplementedException ( );
                }
        }
}
