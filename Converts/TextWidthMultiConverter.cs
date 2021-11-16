using System;
using System . Diagnostics;
using System . Globalization;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Media;
using System . Windows . Threading;


using WPFPages . Views;

namespace WPFPages . Converts
{

	public partial class TextWidthMultiConverter : IMultiValueConverter
	{
		#region IValue Converter methods
		

		/// <summary>
		/// This gets  the control , plus any other values passed as arguments
		/// and hopefully calculates the correct width for a text string allowing 
		/// for font height and font style as best it can.
		/// </summary>
		/// <param name="values">An array of arguments</param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert ( object [ ] values , Type targetType , object parameter , CultureInfo culture )
		{
//			double multiplier = 0.00;
//			double  val = 0;
			double result = 0;
			double TextWidth = 0;//			Type t = values[0].GetType();
			double TextHeight = 0;
			string fontstyle = "";
//			double divisor = 2.0;
			object caller = values[0];
			Type  t = caller . GetType ( );
			ThreeDeeBtnControl tb = caller as ThreeDeeBtnControl;
			string Content  = tb.BtnText;
			double len = (double)tb . TextWidth;
			fontstyle = tb . FontDecoration;
			TextHeight = ( double ) tb . TextHeight;
			object obj = tb . FindName ( "ThreeDBtn" );
			System . Windows . Controls .  Button b = obj as System . Windows . Controls . Button;
			TextWidth = b . ActualWidth;
			object o2 = b.FindName ( "Btn6Content");
			//if(len == 0)
			//	TextWidth = ( double ) Content . Length;
			//else
			//	TextWidth = ( double ) Content .Length;

//			multiplier = 2;
			Size size = MeasureString ( b, Content );

			//			Console . W/riteLine ( $"Width going back is  {TextWidth} * {multiplier} = {TextWidth * multiplier} for [{Content}]\n" );

			//			return TextWidth * multiplier;
			len = Content . Length;

			result = ( int ) ( size . Width / 2);
			result = len * 11;
			result = len;
			Console . WriteLine ( $"Value going back = {result} from text length of ={len} is {size . Width} DPI \n" );
			return result*5; 


//			return size . Width * len;




//			if ( TextWidth > 40 )
//				multiplier += 2.4;
//			else if ( TextWidth > 35 )
//				multiplier += 2.1;
//			else if ( TextWidth > 30 )
//				multiplier += 1.8;
//			else if ( TextWidth > 25 )
//				multiplier += 1.5;
//			else if ( TextWidth > 20 )
//				multiplier += 1.2;      // OK
//			else if ( TextWidth > 15 )
//				multiplier += 1.0;
//			else if ( TextWidth > 10 )
//				multiplier += 0.8;
//			else if ( TextWidth > 5 )
//				multiplier += 0.5;	// OK
//			else
//				multiplier += 0;
			
	
//			//Console . WriteLine ( $"Result calcuated as  {result}\nCalculated {result * (TextHeight / divisor)} \nfor text length of {stringlen}\n\n");
//			//divisor = 3;
//			//			Console . WriteLine ($"Returning {stringlen * (TextHeight / 10)} * {multiplier} = {stringlen * multiplier}\n");
//			//result = ((stringlen * 1.5) * ( TextHeight / 10 )) * multiplier;
////			result = TextWidth * multiplier;
//			result = ( (TextHeight / 2.5) * ((TextWidth /5) * 5.5 )) ;
//			Console . WriteLine ( $"Returning [{Content}] from Converter =(({TextHeight} / 2)  / {TextWidth} = {result}" );
////			result = result * multiplier;
//			Console . WriteLine ($"Value going back is {result}\n"); 
		
//			return result + 5;
		}
		private double MeasureWidth ( )
		{

//			textBlock . Measure ( new Size ( Double . PositiveInfinity, Double . PositiveInfinity ) );
			//textBlock . Arrange ( new Rect ( textBlock . DesiredSize ) );

//			Debug . WriteLine ( textBlock . ActualWidth ); // prints 80.323333333333
//			Debug . WriteLine ( textBlock . ActualHeight );// prints 15.96

			//// constrain the width to 16
//			textBlock . Measure ( new Size ( 16, Double . PositiveInfinity ) );
			//textBlock . Arrange ( new Rect ( textBlock . DesiredSize ) );

//			Debug . WriteLine ( textBlock . ActualWidth ); // prints 14.58
//			Debug . WriteLine ( textBlock . ActualHeight );// prints 111.72			
			return 4;
		}
		private Size MeasureString ( System . Windows . Controls . Button control, string candidate )
		{
			var formattedText = new FormattedText (
			    candidate,
			    CultureInfo . CurrentCulture,
			    FlowDirection . LeftToRight,
			    new Typeface ( control. FontFamily, control . FontStyle, control . FontWeight, control . FontStretch ),
			    control . FontSize,
			    Brushes . Black,
			    new NumberSubstitution ( ),
			    1 );

			return new Size ( formattedText . Width, formattedText . Height );
		}
		private double CheckLength ( int stringlen,double width)
		{
			double extender = 3;
			if ( stringlen < 5 )
				return width;
			else if ( stringlen < 10 )
				return width + ( extender * 2 );
			else if ( stringlen < 15 )
				return width + ( extender * 3 );
			else if ( stringlen < 20 )
				return width + ( extender * 4 );
			else if ( stringlen < 25 )
				return width + ( extender * 5 );
			else if ( stringlen < 30 )
				return width + ( extender * 6 );
			else
				return width;
		}
			public object [ ] ConvertBack ( object value , Type [ ] targetTypes , object parameter , CultureInfo culture )
		{
			throw new NotImplementedException ( );
		}
		#endregion IValue Converter methods
	}
}
