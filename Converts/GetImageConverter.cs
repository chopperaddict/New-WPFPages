using System;
using System . Globalization;
using System . Windows . Data;
using System . Windows . Media . Imaging;

namespace WPFPages . Converts
{
        public class GetImageConverter : IValueConverter
	{
		//public static HeaderToImageConverter Instance =
		//         new HeaderToImageConverter ( );

		public object Convert ( object value, Type targetType,
		    object parameter, CultureInfo culture )
		{
			//File alone
			Uri uri = new Uri ( $"pack://application:,,,{value as string}" );
			BitmapImage source = new BitmapImage ( uri );
			return source;
		}

		public object ConvertBack ( object value, Type targetType,
		    object parameter, CultureInfo culture )
		{
			throw new NotSupportedException ( "Cannot convert back" );
		}
	}
}
