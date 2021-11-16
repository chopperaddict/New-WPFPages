using System;
using System . Globalization;
using System . Windows;
using System . Windows . Data;

using WPFPages . UserControls;

namespace WPFPages . Converts
{
        public class Addoffset : IValueConverter
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
			DependencyObject dpo = new DependencyObject ( );
			object dobj = dpo . GetValue ( ImgButton . ImageLeftOffsetProperty );
			Console . WriteLine ( $"offset object = {( double ) dobj}" );

			double offset = System . Convert . ToDouble ( dobj );
			Console . WriteLine ( $"offset double = {offset}" );
			dobj = dpo . GetValue ( ImgButton . ImgWidthProperty );

			Console . WriteLine ( $"imgwidth object= {( double ) dobj}" );
			double val = System . Convert . ToDouble ( value );
			Console . WriteLine ( $"imgwidth double = {( double ) dobj}" );
			Console . WriteLine ( $"Image width returned = {val + offset}" );
			return ( val + offset ) as object;
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
