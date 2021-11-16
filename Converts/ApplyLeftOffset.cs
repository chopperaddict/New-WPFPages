using System;
using System . Globalization;
using System . Windows . Data;

namespace WPFPages . Converts
{
        public class ApplyLeftOffset : IValueConverter
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
			int offset = System . Convert . ToInt32 ( parameter );
			int currentvalue = System . Convert . ToInt32 ( value );
			currentvalue += offset;
			return currentvalue;
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
