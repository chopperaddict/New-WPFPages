using System;
using System . Collections . Generic;
using System . Globalization;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Data;

namespace WPFPages . Converts
{
	public class PadImageBase : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			double d = ( double ) value;
			return ( object ) ( d + 15 );
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
