using System;
using System . Collections . Generic;

using System . Globalization;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Data;
using System . Windows . Media;

namespace WPFPages . Views
{
	public class HexValueToBrush : IValueConverter
	{
		public object Convert ( object value , Type targetType , object parameter , CultureInfo culture )
		{
			string input=value.ToString();
			if ( input . Length < 9 )
				return value;

			Brush brush = ( Brush ) new BrushConverter ( ) . ConvertFromString ( input );
			return brush;
		}

		public object ConvertBack ( object value , Type targetType , object parameter , CultureInfo culture )
		{
			throw new NotImplementedException ( );
		}
	}
}
