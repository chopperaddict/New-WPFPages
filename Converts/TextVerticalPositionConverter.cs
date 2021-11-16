using System;
using System . Globalization;
using System . Windows . Data;

using WPFPages . Views;

namespace WPFPages . Converts
{
	public partial class TextVerticalPositionConverter : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			double values = 0;
			Type t = value . GetType ( );
			if ( value != null )
			{

				values = System . Convert . ToDouble ( value );
				Console . WriteLine ( $"ActualHeight in Converter ={values}" );
				if ( values < 25 )
					values = 10;                  // OK
				else if ( values < 50 )
					values = 30;                      //OK
				else if ( values < 100 )
					values = 32;                        //ok
				else if ( values < 150 )
					values = 45;                                      //OK
				else if ( values < 200 )
					values = 52;                                //OK
				else if ( values < 250 )
					values = 50;                                   //OK
				else if ( values < 300 )
					values = 60;
				else
					values = 70;
			}
			Console . WriteLine ( $"Returning Height from Converter ={values}" );
			return values;
		}
		public object ConvertBack ( object value, Type targetTypes, object parameter, CultureInfo culture )
		{
			return ( object [ ] ) value;
		}
	}

	public partial class TextHeightMultiConverter : IMultiValueConverter
	{
		public object Convert ( object [ ] values, Type targetType, object parameter, CultureInfo culture )
		{
//			double multiplier = 0.00;
//			double val = 0;
			double result = 0;
			double TextHeight = 0;
			string fontstyle = "";
//			double divisor = 2.0;
			object caller = values [ 0 ];
			ThreeDeeBtnControl tb = caller as ThreeDeeBtnControl;
			string Content = tb . BtnText;
			fontstyle = tb . FontDecoration;
			TextHeight = ( double ) tb . TextHeight;

			return TextHeight * 2;
			//if ( TextHeight > 40 )
			//	multiplier += 2.4;
			//else if ( TextHeight > 35 )
			//	multiplier += 2.1;
			//else if ( TextHeight > 30 )
			//	multiplier += 1.8;
			//else if ( TextHeight > 25 )
			//	multiplier += 1.5;
			//else if ( TextHeight > 20 )
			//	multiplier += 1.2;      // OK
			//else if ( TextHeight > 15 )
			//	multiplier += 1.0;
			//else if ( TextHeight > 10 )
			//	multiplier += 0.8;
			//else if ( TextHeight > 5 )
			//	multiplier += 0.5;      // OK
			//else
			//	multiplier += 0;


			////Console . WriteLine ( $"Result calcuated as  {result}\nCalculated {result * (TextHeight / divisor)} \nfor text length of {stringlen}\n\n");
			////divisor = 3;
			////			Console . WriteLine ($"Returning {stringlen * (TextHeight / 10)} * {multiplier} = {stringlen * multiplier}\n");
			////result = ((stringlen * 1.5) * ( TextHeight / 10 )) * multiplier;
			////			result = TextWidth * multiplier;
			//result = (TextHeight / 5 ) * 5.5 ;
			//Console . WriteLine ( $"Returning Height for [{Content}] from Converter =(({TextHeight} / 5)  * 5.5 = {result}" );
			////			result = result * multiplier;
			//Console . WriteLine ( $"Height being returned from Converter is {result}\n" );

			//return result + 5;
		}

		public object [ ] ConvertBack ( object value, Type [ ] targetTypes, object parameter, CultureInfo culture )
		{
			throw new NotImplementedException ( );
		}
	}
}
