using System;
using System . Globalization;
using System . Windows . Data;

namespace WPFPages . Converts
{
        public class Color2Hex : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string HexString = "";
			HexString = value . ToString ( );
			return HexString;
		}

		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotImplementedException ( );
		}
	}
}
