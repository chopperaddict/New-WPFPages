using System;
using System . Globalization;
using System . Windows . Data;

using WPFPages . UserControls;

namespace WPFPages . Converts
{
        public class CalculateLeftOffset : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			//			double val = ( double ) value;
			//			double par = ( double ) parameter;
			//				targetType.GetValue(
			ImgButton ic = new ImgButton ( );
			//			double dp = System . Convert . ToDouble ( ic. ImgName . ActualWidth );
			//			double result = val;
			return ( object ) value;
		}

		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return ( object ) null;
		}
	}
}
