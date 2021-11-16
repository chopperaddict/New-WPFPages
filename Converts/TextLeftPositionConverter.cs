using System;
using System . Collections . Generic;
using System . Globalization;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Data;

using WPFPages . Views;

namespace WPFPages . Converts
{
	public partial class TextLeftPositionConverter : IValueConverter
	{
		public object Convert ( object value , Type targetType , object parameter , CultureInfo culture )
		{
			double  values=125;
			Type t = value.GetType();
//			doesn't give me the text in the button !!
			ThreeDeeBtnControl td = new ThreeDeeBtnControl();
//			string s = td . BtnText;
			Console . WriteLine ( $"Incoming ActualWidth in Converter ={values}" );
			if ( value != null && System . Convert . ToDouble ( value ) > 0 )
			{
				values = System . Convert . ToDouble ( value );
				Console . WriteLine ( $"ActualWidth in Converter ={values}" );
				if ( values < 25 )
					values = 11;                    //OK
				else if ( values < 45 )
					values = 28;
				else if ( values < 65 )
					values = 45;
				else if ( values < 100 )
					values = 92;
				else if ( values < 150 )
					values = 72;                              //OK
				else if ( values < 200 )
					values =145;
				else if ( values < 300 )
					values = 210;
				else if ( values < 400 )
					values =350;                                 //OK
				else if ( values < 500 )
					values = 165;
				else if ( values < 600 )
					values = 594;
			}
			Console . WriteLine ( $"Returning Width from Converter ={values}" );
			return values;

		}

		public object ConvertBack ( object value , Type targetType , object parameter , CultureInfo culture )
		{
			throw new NotImplementedException ( );
		}
	}
	
}
