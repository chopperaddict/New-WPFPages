using System;
using System .Globalization;
using System .Windows .Data;

namespace WPFPages .Converts
{
	/// <summary>
	/// Default Multi Converter skeleton
	/// Author : %username$
	/// Date : $time$
	/// </summary>
	class MultiConverterTemplate : IMultiValueConverter
	{
		public object Convert ( object [ ] values , Type targetType , object parameter , CultureInfo culture )
		{
			throw new NotImplementedException ( );
		}

		public object [ ] ConvertBack ( object value , Type [ ] targetTypes , object parameter , CultureInfo culture )
		{
			throw new NotImplementedException ( );
		}
	}
}
