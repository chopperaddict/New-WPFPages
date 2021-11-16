using System;
using System . Globalization;
using System . Windows . Data;
using System . Windows;
using WPFPages . UserControls;

namespace WPFPages . Converts
{
        public class ReduceTextWidth : IValueConverter
        {
                /// <summary>
                /// Adds a dependency value received an XPath Converter parameter to move a textbolock downwrds to fit correctly
                /// </summary>
                /// <param name="value"></param>
                /// <param name="targetType"></param>
                /// <param name="parameter"></param>
                /// <param name="culture"></param>
                /// <returns></returns>
                public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
                {
                       // return value;
                        double currentvalue = (double)value;
                        currentvalue -=10;
                        return (object)currentvalue;
                }

                public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
                {
                        //if ( temp <= 255 )
                        //	return ( string ) temp . ToString ( "X2" );
                        //else if ( temp <= 65535 )
                        //	return ( string ) temp . ToString ( "X4" );
                        //else if ( temp <= 16777215 )
                        //	return ( string ) temp . ToString ( "X6" );

                        return value;
                }
        }
}
