using System;
using System . Globalization;
using System . Windows;
using System . Windows . Data;

namespace WPFPages . Converts
{
        /// <summary>
        /// A date converter 2 (see also DateOnlyConverter)
        /// that returns the date in std UNC YYYY/MM/DD format
        /// </summary>
        public class Date2UTCConverter : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value == DependencyProperty . UnsetValue )
				return DependencyProperty . UnsetValue;
			// Assumes receiving a date as "10/04/1932" or similar
			// Returns "1932/04/10"
			string date = value . ToString ( );
			string Output = "";
			Output += date [ 6 ];
			Output += date [ 7 ];
			Output += date [ 8 ];
			Output += date [ 9 ];
			Output += "/";
			Output += date [ 3 ];
			Output += date [ 4 ];
			Output += "/";
			Output += date [ 0 ];
			Output += date [ 1 ];
			return Output;
		}

		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return null as object;
		}
	}
}
