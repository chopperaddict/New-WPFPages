using System;
using System . Globalization;
using System . Windows;
using System . Windows . Data;
using System . Windows . Media;

namespace WPFPages . Converts
{
        /// <summary>
        /// Returns a SolidColorBrush from a received String value
        /// </summary>
        class StringtoColorConverter : IValueConverter
        {
                public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
                {
                        string str = targetType . ToString ( );
                        // check if we are requesting a Brush
                        if ( str . Contains ( "System.Windows.Media.SolidColorBrush" ) )
                        {
                                if ( ( string ) parameter == "" )
                                        return null;

                                SolidColorBrush color=  ( SolidColorBrush ) Application . Current . FindResource ( parameter as string );
                                return color;
                        }
                        else if ( str . Contains ( "System.Windows.Media.LinearGradientBrush" ) )
                        {
                                if ( ( string ) parameter == "" )
                                        return null;

                                LinearGradientBrush color = ( LinearGradientBrush ) Application . Current . FindResource ( parameter as string );
                                return color;
                        }
                        else if ( str . Contains ( "System.Windows.Media.RadialGradientBrush" ) )
                        {
                                if ( ( string ) parameter == "" )
                                        return null;

                                RadialGradientBrush color = ( RadialGradientBrush ) Application . Current . FindResource ( parameter as string );
                                return color;
                        }
                        else if ( str . Contains ( "System.Windows.Media.Brush" ) )
                        {
                                if ( ( string ) parameter == "" )
                                        return null;

                                Brush color = ( Brush ) Application . Current . FindResource ( parameter as string );
                                return color;
                        }
                        else if ( str . Contains ( "System.Windows.Media.Color" ) )
                        {
                                if ( ( string ) parameter == "" )
                                        return null;

                                Color color = ( Color) Application . Current . FindResource ( parameter as string );
                                return color;
                        }
                        else
                                return null;
                }

                public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
                {
                        throw new NotImplementedException ( );
                }
        }
}
