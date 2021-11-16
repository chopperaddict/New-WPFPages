using System;
using System . Collections . Generic;
using System . Globalization;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Data;

namespace WPFPages . Converts
{
	/// <summary>
	/// Base class for all single converters
	/// Created : 10/17/2021 2:57:58 PM
	/// </summary>
	//[ValueConversion ( typeof ( string ), typeof ( double) )]
	public class PadHeight : IValueConverter
	{
		public object Convert ( object value , Type targetType , object parameter , CultureInfo culture )
		{
			//return value;
			//return (double)value + ( double ) 5;
			double str = System.Convert.ToDouble(value);
			double d = str  -25;
			return d;
		}

		public object ConvertBack ( object value , Type targetType , object parameter , CultureInfo culture )
		{
			throw new NotImplementedException ( );
		}
	}
}
