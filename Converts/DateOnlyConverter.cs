using System;
using System . Globalization;
using System . Windows . Data;

namespace WPFPages . Converts
{
        /// <summary>
        /// This is the MAIN Date converter I use (but ee also Date2UTCConverter )
        /// Returns a date in DD/MM/YYY format
        /// </summary>
        public class DateOnlyConverter : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string date = value . ToString ( );
			char [ ] ch = { ' ' };
			string [ ] dateonly = date . Split ( ch [ 0 ] );
			return dateonly [ 0 ];
		}

		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			//if (value.ToString() != "")
			//	return (DateTime)value;
			//else
			return null as object;
		}
	}
}
