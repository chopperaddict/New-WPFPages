using System;
using System . Globalization;
using System . Linq;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;

namespace WPFPages . Converts
{
	public class PadImgConverter: IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value == null || parameter == null )
				return value;
			int val = ( int ) value;
			Image i = parameter as Image;
//			int par = ( int ) parameter;
			int result = val - (int)i.Width;
			return (object) result;
		}

		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return ( object ) null;
		}

	}
}

