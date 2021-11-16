using System;
using System . Collections . Generic;
using System . Globalization;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Data;
using System . Windows;

namespace WPFPages . Converts
{
	public class MoveTextblockToTop : IValueConverter
	{
		/// <summary>
		/// Adds a dependency value received an XPath Converter parameter to move a textbolock downwrds to fit correctly
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			Thickness thick = new Thickness ( );
			thick . Left = 0;
			thick . Bottom = 0;
			thick . Right = 0;
			if ( value == null || parameter == null )
			{
				thick . Top = 0;
				return ( object ) thick;
			}
			if ( value != null && int . Parse ( parameter . ToString ( ) ) == 0 )
			{
				thick . Top = 0;
				return value;
			}
			int offset = System . Convert . ToInt32 ( parameter );
			thick . Top = offset;
			return ( object ) thick;
		}

		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			//if ( temp <= 255 )
			//	return ( string ) temp . ToString ( "X2" );
			//else if ( temp <= 65535 )
			//	return ( string ) temp . ToString ( "X4" );
			//else if ( temp <= 16777215 )
			//	return ( string ) temp . ToString ( "X6" );

			return value;
		}
	}

}
