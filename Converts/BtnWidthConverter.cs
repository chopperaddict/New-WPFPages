using System;
using System . Globalization;
using System . Windows . Data;

namespace WPFPages . Converts
{
        /// <summary>
        /// Reduces the actual width of the Rectangle button class Becuase otherwise Margin chages make it expand
        /// </summary>
        public partial class BtnWidthConverter : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			double d = 0;
			Console . WriteLine ( $"Orignal Width from Converter ={value}" );
			double result = ( double ) value;
			if ( parameter != null )
			{
				d = System . Convert . ToDouble ( parameter );
				result -= d;
			}
			Console . WriteLine ( $"Resultin Width from Converter ={result - 20}" );
			return result - 20;
		}

		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotImplementedException ( );
		}
	}
}
