using System;
using System . Globalization;
using System . Windows . Data;

namespace WPFPages . Converts
{
	public partial class TextWidthMinus10Percent : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			int val = System . Convert. ToInt32(value );
			if ( val == 0 )
				return value;
			if ( parameter != null )
			{
				if ( parameter . ToString ( ) != "" )
					return val - System . Convert . ToInt32 ( parameter );
				else
					return val - 10;
			}
			else
			{
				int converter = ( val * 90 ) / 100;
				return converter;
			}
		}

		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotImplementedException ( );
		}
	}
}
