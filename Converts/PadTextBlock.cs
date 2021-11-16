
using System;
using System . Collections . Generic;
using System . Globalization;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Data;

namespace WPFPages . Converts
{
        public class PadTextBlock : IValueConverter
        {
                public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
                {
                        double diff = 0;
                        double result = 23;
                        double actual = 0;
                        //			return value;
                        //			if ( value == null || parameter == null )
                        //				return value;
                        string s = parameter as string;
                        //			diff = double . Parse ( s );
                        s = value as string;
                        //			actual = double . Parse ( s );
                        //			result = ( actual - diff );
                        return result;
                }

                public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
                {
                        return ( object ) null;
                }

        }
}
