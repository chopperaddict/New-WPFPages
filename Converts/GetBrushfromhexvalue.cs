using System;
using System . Globalization;
using System . Windows . Data;
using System . Windows . Media;

namespace WPFPages . Converts
{
	public class GetBrushfromhexvalue : IValueConverter
	{


		public object Convert ( object value , Type targetType , object parameter , CultureInfo culture )
		{
			string input=value.ToString();
			if ( input =="")
					return value;
			if ( input [ 0 ] != '#' )
				input = "#" + input;
			if ( input . Length !=  9 )
				return value;
			if ( CheckValidColorCode ( input ) == false )
				return value;
			Brush brush = ( Brush ) new BrushConverter ( ) . ConvertFromString ( input );
			return brush;
		}

		public object ConvertBack ( object value , Type targetType , object parameter , CultureInfo culture )
		{
			throw new NotImplementedException ( );
		}

		private bool CheckValidColorCode ( string value )
		{
			string ValidColorCode= "#0123456789ABCDEFabcdef";
			bool isvalid = false;
			bool badhash=false;
			int validcount = 0;
			for ( int y = 0 ; y < value . Length ; y++ )
			{
				for ( int x = 0 ; x < ValidColorCode . Length ; x++ )
				{
					// Check for hash not in 1st position
					if ( value [ y ] == '#' && y > 0 )
					{
						badhash = true;
						break;            // broken, hash must be 1st char only
					}

					if ( value [ y ] == ValidColorCode [ x ] )
					{
						validcount ++;
						break;
					}
				}
				if ( badhash )
					break;
				// have we reached the end ?
				if ( y == value . Length - 1 )
				{
					//Yes, so all is well
					isvalid = true;
					break;
				}
			}
			if ( validcount < value . Length )
				return false;
			return isvalid;
		}

	}
}
