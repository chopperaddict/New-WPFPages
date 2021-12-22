using Microsoft . SqlServer . Management . Smo;
using Microsoft . SqlServer . Management . XEvent;

using System;
using System . Collections . Generic;
using System . Globalization;
using System . IO;
using System . Linq;
using System . Runtime . ConstrainedExecution;
using System . Text;
using System . Text . RegularExpressions;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Navigation;
using System . Windows . Shapes;

using WPFPages . AttachedProperties;
using WPFPages . Converts;
using WPFPages . UserControls;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for MsgboxSetup.xaml
	/// </summary>
	public partial class MsgboxSetup : Window
	{
		private int ActiveField = 1;
		private string[] bytes = {"","","","" };
		public MsgboxSetup ( )
		{
			InitializeComponent ( );
			Utils . SetupWindowDrag ( this );
			ReadDlgInput ( );
			
		}

		private void ReadDlgInput ( )
		{
			SolidColorBrush sb;
			string input = File . ReadAllText ( @"Messageboxes.dat" );
			string[] fields = input.Split('\n');
			int indx= 0;
			foreach ( var item in fields )
			{
				switch ( indx++ )
				{
					case 0:
						DlgInput . dlgbackground = Utils . GetNewBrush ( item );
						SetCurrentValue ( Msgbox.DlgBackGroundProperty, DlgInput . dlgbackground );
						textBox . Text = DlgInput . dlgbackground . BrushtoText ( );
						break;
					case 1:
						DlgInput . dlgforeground = Utils . GetNewBrush ( item );
						SetCurrentValue ( Msgbox .DlgForeGroundProperty, DlgInput . dlgforeground );
						textBox1 . Text = DlgInput . dlgforeground . BrushtoText ( );
						break;
					case 2:
						DlgInput . btnbackground = Utils . GetNewBrush ( item );
						SetCurrentValue ( Msgbox .BtnBackGroundProperty, DlgInput . btnbackground );
						textBox2 . Text = DlgInput . btnbackground . BrushtoText ( );
						break;
					case 3:
						DlgInput . btnforeground = Utils . GetNewBrush ( item );
						SetCurrentValue ( Msgbox.BtnForeGroundProperty, DlgInput . btnforeground );
						textBox3 . Text = DlgInput . btnforeground . BrushtoText ( );
						break;
					case 4:
						DlgInput . Btnborder = Utils . GetNewBrush ( item );
						SetCurrentValue ( Msgbox.BorderColorProperty, DlgInput . Btnborder );
						textBox4 . Text = DlgInput . Btnborder . BrushtoText ( );
						break;
					case 5:
						DlgInput . mousebackground = Utils . GetNewBrush ( item );
						SetCurrentValue ( Msgbox .MouseoverBackGroundProperty, DlgInput . mousebackground );
						textBox5 . Text = DlgInput . mousebackground . BrushtoText ( );
						break;
					case 6:
						DlgInput . mouseforeground = Utils . GetNewBrush ( item );
						SetCurrentValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . mouseforeground );
						textBox6 . Text = DlgInput . mouseforeground . BrushtoText ( );
						break;
					case 7:
						DlgInput . defbtnbackground = Utils . GetNewBrush ( item );
						//SetCurrentValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . mouseforeground );
						textBox7 . Text = DlgInput . defbtnbackground . BrushtoText ( );
						break;
					case 8:
						DlgInput . defbtnforeground = Utils . GetNewBrush ( item );
						//SetCurrentValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . mouseforeground );
						textBox8 . Text = DlgInput . defbtnforeground . BrushtoText ( );
						break;
				}
			}


			Console . WriteLine ( $"MsgBox  configuraton : Data read in from disk ....\n"
				+ "DlgInput Data\n"
				+ $"Dlg background :			[{DlgInput . dlgbackground}]\n"
				+ $"Dlg foreground :			[{DlgInput . dlgforeground}]\n"
				+ $"Btn Background :			[{DlgInput . btnbackground}]\n"
				+ $"Btn Foreground :			[{DlgInput . btnforeground}]\n"
				+ $"Btn Border :				[{DlgInput . Btnborder}]\n"
				+ $"Btn Mouseover Background :	[{DlgInput . mousebackground}]\n"
				+ $"Btn Mouseover Foreground :	[{DlgInput . mouseforeground}]\n"
				+ $"Btn DefBackground :			[{DlgInput . defbtnbackground}]\n"
				+ $"Btn DefForeground :			[{DlgInput . defbtnforeground}]\n"
				+ $"Btn Border Size :			[{DlgInput . BorderSize . Top}, {DlgInput . BorderSize . Left},{DlgInput . BorderSize . Right},{DlgInput . BorderSize . Bottom}],\n\n"

				+ "DP Data\n"
				+ $"Dlg background :			[{GetValue ( Msgbox . DlgBackGroundProperty )}]\n"
				+ $"Dlg foreground :			[{GetValue ( Msgbox . DlgForeGroundProperty )}]\n"
				+ $"Btn Background :			[{GetValue ( Msgbox . BtnBackGroundProperty )}]\n"
				+ $"Btn Foreground :			[{GetValue ( Msgbox . BtnForeGroundProperty )}]\n"
				+ $"Btn Border :				[{GetValue ( Msgbox . BorderColorProperty )}]\n\n"
				+ $"Btn Mouseover Background :	[{GetValue ( Msgbox . MouseoverBackGroundProperty )}]\n"
				+ $"Btn Mouseover Foreground :	[{GetValue ( Msgbox . MouseoverForeGroundProperty )}]\n" );
			textBox . Focus ( );
			ListDps ( this, null);
		}

		private void SaveButtonColors ( )
		{
			// Save colors to both types of MessageBoxes
			Brush brush;
			bool b = false;			  
			string output="";
			DlgInput . dlgbackground = color1 . Background as Brush;
			output += color1 . Background . ToString ( ) + "\n";
			SetValue ( Msgbox . DlgBackGroundProperty , DlgInput . dlgbackground );	  			

			DlgInput . dlgforeground = color2 . Background as Brush;
			output += color2 . Background . ToString ( ) + "\n";
			SetValue ( Msgbox . DlgForeGroundProperty , DlgInput . dlgforeground );
			
			DlgInput . btnbackground = color3 . Background as Brush;
			output += color3 . Background . ToString ( ) + "\n";
			SetCurrentValue ( Msgbox . BtnBackGroundProperty , DlgInput . btnbackground );

			DlgInput . btnforeground = color4 . Background as Brush;
			output += color4 . Background . ToString ( ) + "\n";
			SetValue ( Msgbox . BtnForeGroundProperty , DlgInput . btnforeground );

			DlgInput . Btnborder = color5 . Background as Brush;
			output += color5 . Background . ToString ( ) + "\n";
			SetValue ( Msgbox . BorderColorProperty , DlgInput . Btnborder );

			DlgInput . mousebackground = color6 . Background as Brush;
			output += color6 . Background . ToString ( ) + "\n";
			SetValue ( Msgbox . MouseoverBackGroundProperty , DlgInput . mousebackground );

			DlgInput . mouseforeground = color7 . Background as Brush;
			output += color7 . Background . ToString ( ) + "\n";
			SetValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . mouseforeground );

			DlgInput . defbtnbackground = color8 . Background as Brush;
			output += color8. Background . ToString ( ) + "\n";
			//SetValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . mouseforeground );

			DlgInput . defbtnforeground = color9 . Background as Brush;
			output += color9 . Background . ToString ( ) + "\n";
			//SetValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . mouseforeground );

			Thickness d =  DlgInput.BorderSize;
			output += d . ToString ( ) + "\n";
			SetValue ( Msgbox . BorderSizeProperty , d );

			b = DlgInput . UseIcon;
			output += ( b == true ? "T" : "F" ) + "\n";
			b = DlgInput . UseDarkMode;
			output += ( b == true ? "T" : "F" ) + "\n";
			b = DlgInput . isClean;
			output += ( b == true ? "T" : "F" ) + "\n";

			File . WriteAllText ( @"Messageboxes.dat" , output );

			Console . WriteLine ( $"Data written out to disk ....\n"
				+ $"Dlg background :			[{DlgInput . dlgbackground}]\n"
				+ $"Dlg foreground :			[{DlgInput . dlgforeground}]\n"
				+ $"Btn Background :			[{DlgInput . btnbackground}]\n"
				+ $"Btn Foreground :			[{DlgInput . btnforeground}]\n"
				+ $"Btn Border :				[{DlgInput . Btnborder}]\n"
				+ $"Btn Mouseover Background :	[{DlgInput . mousebackground}]\n"
				+ $"Btn Mouseover Foreground :	[{DlgInput . mouseforeground}]\n"
				+ $"Btn DefBackground :			[{DlgInput . defbtnbackground}]\n"
				+ $"Btn DefForeground :			[{DlgInput . defbtnforeground}]\n"
				+ $"Btn Border Size :			[{d . Top}, {d . Left},{d . Right},{d . Bottom}],\n\n"
				);
		}

		#region Set display panels to new color
		private void Button_Click ( object sender , RoutedEventArgs e )
		{
			SaveButtonColors ( );
			this . Close ( );
		}
		private void textBox_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
			{
				if ( textBox . Text . Contains ( "#" ) == false )
					textBox . Text = "#" + textBox . Text;
				color1 . Background = textBox . Text . ToSolidBrush ( );
				ActiveField = 1;
			}
		}
		private void textBox1_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
			{
				if ( textBox . Text . Contains ( "#" ) == false )
					textBox . Text = "#" + textBox . Text;
				color2 . Background = textBox1 . Text . ToSolidBrush ( );
				ActiveField = 2;
			}
		}
		private void textBox2_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
			{
				{
					if ( textBox . Text . Contains ( "#" ) == false )
						textBox . Text = "#" + textBox . Text;
					color3 . Background = textBox2 . Text . ToSolidBrush ( );
					DlgInput . btnbackground = color3 . Background;
					ActiveField = 3;
				}
			}
		}
		private void textBox3_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
			{
				{
					if ( textBox . Text . Contains ( "#" ) == false )
						textBox . Text = "#" + textBox . Text;
					color4 . Background = textBox3 . Text . ToSolidBrush ( );
					DlgInput . btnbackground = color4 . Background;
					ActiveField = 4;
				}
			}
		}
		private void textBox4_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
			{
				{
					if ( textBox . Text . Contains ( "#" ) == false )
						textBox . Text = "#" + textBox . Text;
					color5 . Background = textBox4 . Text . ToSolidBrush ( );
					DlgInput . btnbackground = color5 . Background;
					ActiveField = 5;
				}
			}
		}

		private void textBox5_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
			{
				{
					if ( textBox . Text . Contains ( "#" ) == false )
						textBox . Text = "#" + textBox . Text;
					color6 . Background = textBox5 . Text . ToSolidBrush ( );
					DlgInput . btnbackground = color6 . Background;
					ActiveField = 6;
				}
			}
		}
		private void textBox6_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
			{
				{
					if ( textBox . Text . Contains ( "#" ) == false )
						textBox . Text = "#" + textBox . Text;
					color7 . Background = textBox6 . Text . ToSolidBrush ( );
					DlgInput . btnbackground = color7 . Background;
					ActiveField = 7;
				}
			}
		}
		private void textBox7_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
			{
				{
					if ( textBox . Text . Contains ( "#" ) == false )
						textBox . Text = "#" + textBox . Text;
					color8 . Background = textBox7 . Text . ToSolidBrush ( );
					DlgInput . btnbackground = color8 . Background;
					ActiveField = 7;
				}
			}
		}
		private void textBox8_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
			{
				{
					if ( textBox . Text . Contains ( "#" ) == false )
						textBox . Text = "#" + textBox . Text;
					color9 . Background = textBox8 . Text . ToSolidBrush ( );
					DlgInput . btnbackground = color9 . Background;
					ActiveField = 7;
				}
			}
		}
		#endregion Set display panel to new color

		private void Window_Closing ( object sender , System . ComponentModel . CancelEventArgs e )
		{
			// already done
			//			MainWindow . SaveMsgboxData ( this );
		}
		private void textBox_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			ActiveField = 1;
		}
		private void textBox1_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			ActiveField = 2;
		}
		private void textBox2_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			ActiveField = 3;
		}
		private void textBox3_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			ActiveField = 4;
		}
		private void textBox4_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			ActiveField = 5;
		}
		private void textBox5_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			ActiveField = 6;
		}
		private void textBox6_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			ActiveField = 7;
		}
		private void textBox7_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			ActiveField = 8;
		}
		private void textBox8_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			ActiveField = 9;
		}

		private void trans_ValueChanged ( object sender , RoutedPropertyChangedEventArgs<double> e )
		{
			ResSetActiveColorDisplay ( e . NewValue , "TRANS" );
			//			textBox3 . Text = CalculateNewColor ( e . NewValue , "TRANS" );
		}

		private void SRed_ValueChanged ( object sender , RoutedPropertyChangedEventArgs<double> e )
		{
			ResSetActiveColorDisplay ( e . NewValue , "RED" );
			//			textBox3 . Text= CalculateNewColor ( e . NewValue , "RED" );
		}

		private void SGreen_ValueChanged ( object sender , RoutedPropertyChangedEventArgs<double> e )
		{
			ResSetActiveColorDisplay ( e . NewValue , "GREEN" );
			//textBox3 . Text = CalculateNewColor ( e . NewValue , "GREEN" );
		}

		private void SBlue_ValueChanged ( object sender , RoutedPropertyChangedEventArgs<double> e )
		{
			ResSetActiveColorDisplay ( e . NewValue , "BLUE" );
			//textBox3 . Text = CalculateNewColor ( e . NewValue , "BLUE" );
		}

		// Reset text display and therefore color panel
		private void ResSetActiveColorDisplay ( double value , string caller )
		{
			switch ( ActiveField )
			{
				case 1:
					textBox . Text = CalculateNewColor ( value , caller );
					color1 . UpdateLayout ( );
					break;
				case 2:
					textBox1 . Text = CalculateNewColor ( value , caller );
					textBox1 . Refresh ( );
					color2 . UpdateLayout ( );
					break;
				case 3:
					textBox2 . Text = CalculateNewColor ( value , caller );
					color3 . UpdateLayout ( );
					break;
				case 4:
					textBox3 . Text = CalculateNewColor ( value , caller );
					color4 . UpdateLayout ( );
					break;
				case 5:
					textBox4 . Text = CalculateNewColor ( value , caller );
					color5 . UpdateLayout ( );
					break;
				case 6:
					textBox5 . Text = CalculateNewColor ( value , caller );
					color6 . UpdateLayout ( );
					break;
				case 7:
					textBox6 . Text = CalculateNewColor ( value , caller );
					color7 . UpdateLayout ( );
					break;
				case 8:
					textBox7 . Text = CalculateNewColor ( value , caller );
					color8 . UpdateLayout ( );
					break;
				case 9:
					textBox8 . Text = CalculateNewColor ( value , caller );
					color9 . UpdateLayout ( );
					break;
			}
		}

		// Returns a new Hex color string (based on value from slider mostly)
		private string CalculateNewColor ( double value , string caller )
		{
			string output="", temp="", clr="";
			byte t=0, r=0,g=0,b=0;

			double val=0.00;
			// Find out the active field we are working with
			temp = GetActiveValue ( );
			// Get that fields vars as an array of individual bytes in string[]
			bytes = GetColorInBytes ( temp );

			switch ( caller )
			{
				case "TRANS":
					bytes [ 0 ] = ResetByteToSliderValue ( bytes [ 0 ] , value );
					output = GetStringfromColor ( bytes );
					break;
				case "RED":
					bytes [ 1 ] = ResetByteToSliderValue ( bytes [ 1 ] , value );
					output = GetStringfromColor ( bytes );
					break;
				case "GREEN":
					bytes [ 2 ] = ResetByteToSliderValue ( bytes [ 2 ] , value );
					output = GetStringfromColor ( bytes );
					break;
				case "BLUE":
					bytes [ 3 ] = ResetByteToSliderValue ( bytes [ 3 ] , value );
					output = GetStringfromColor ( bytes );
					break;
			}
			return output;
		}

		private string GetStringfromColor ( string [ ] bytes )
		{
			double a=0.0, r=0.0, g=0.0, b=0.0;
			int w=0,x=0,y=0,z=0;
			if ( bytes [ 0 ] == "" || bytes [ 1 ] == "" || bytes [ 2 ] == "" || bytes [ 3 ] == "" )
				return "";
			w = Convert . ToInt32 ( bytes [ 0 ] , 16 );
			x = Convert . ToInt32 ( bytes [ 1 ] , 16 );
			y = Convert . ToInt32 ( bytes [ 2 ] , 16 );
			z = Convert . ToInt32 ( bytes [ 3 ] , 16 );
			Color c = Color . FromArgb (
					Convert . ToByte ( (float)Convert.ToDouble(w)) ,
					Convert . ToByte ( (float)Convert.ToDouble(x)) ,
					Convert . ToByte ( (float)Convert.ToDouble( y) ),
					Convert . ToByte ( (float)Convert.ToDouble (z)));
			return c . ToString ( );
		}


		// reset  single "double value" as a string[] array with 4 elements
		private string ResetByteToSliderValue ( string bytestring , double value )
		{
			if ( bytestring . Length < 2 )
				return bytestring;
			int r = Convert . ToInt16( bytestring, 16);
			r = ( int ) value;
			// Convert to a single byte Hex Value
			HexConvert hc = new HexConvert();
			var v = hc.Convert(r,  null,null, null);

			return v . ToString ( );
		}

		// convert string ("#FF112233") into string[] as byte colors
		private string [ ] GetColorInBytes ( string inputcolor )
		{
			string clr="";
			string[] output = {"","","",""};
			if ( inputcolor . Length < 9 )
				return output;
			clr = inputcolor . Substring ( 1 , 2 );
			output [ 0 ] = clr;
			clr = inputcolor . Substring ( 3 , 2 );
			output [ 1 ] = clr;
			clr = inputcolor . Substring ( 5 , 2 );
			output [ 2 ] = clr;
			clr = inputcolor . Substring ( 7 , 2 );
			output [ 3 ] = clr;
			return output;
		}

		// return current selected item as a color string (#FF112233)
		private string GetActiveValue ( )
		{
			string output="";
			if ( ActiveField == 1 )
				output = textBox . Text;
			else if ( ActiveField == 2 )
				output = textBox1 . Text;
			else if ( ActiveField == 3 )
				output = textBox2 . Text;
			else if ( ActiveField == 4 )
				output = textBox3 . Text;
			else if ( ActiveField == 5 )
				output = textBox4 . Text;
			else if ( ActiveField == 6 )
				output = textBox5 . Text;
			else if ( ActiveField == 7 )
				output = textBox6 . Text;
			else if ( ActiveField == 8 )
				output = textBox7 . Text;
			else if ( ActiveField == 9 )
				output = textBox8 . Text;
			return output;
		}

		private void ResetFieldColors ( int activefield )
		{
			textBox . Background = Utils . GetNewBrush ( "#47FFDDF5" );
			textBox . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox1 . Background = Utils . GetNewBrush ( "#47FFDDF5" );
			textBox1 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox2 . Background = Utils . GetNewBrush ( "#47FFDDF5" );
			textBox2 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox3 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox3 . Background = Utils . GetNewBrush ( "#47FFDDF5" );
			textBox4 . Background = Utils . GetNewBrush ( "#47FFDDF5" );
			textBox4 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox5 . Background = Utils . GetNewBrush ( "#47FFDDF5" );
			textBox5 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox6 . Background = Utils . GetNewBrush ( "#47FFDDF5" );
			textBox6 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox7 . Background = Utils . GetNewBrush ( "#47FFDDF5" );
			textBox7 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox8 . Background = Utils . GetNewBrush ( "#47FFDDF5" );
			textBox8 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			switch ( activefield )
			{
				case 1:
					textBox . Background = ( Brush ) FindResource ( "White0" ) as Brush;
					textBox . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
					break;
				case 2:
					textBox1 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
					textBox1 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
					break;
				case 3:
					textBox2 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
					textBox2 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
					break;
				case 4:
					textBox3 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
					textBox3 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
					break;
				case 5:
					textBox4 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
					textBox4 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
					break;
				case 6:
					textBox5 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
					textBox5 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
					break;
				case 7:
					textBox6 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
					textBox6 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
					break;
				case 8:
					textBox7 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
					textBox7 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
					break;
				case 9:
					textBox8 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
					textBox8 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
					break;
			}
		}
		private void SetSliders ( string color )
		{
			string[] bytes = {"","","",""};
			bytes [ 0 ] = color . Substring ( 1 , 2 );
			bytes [ 1 ] = color . Substring ( 3 , 2 );
			bytes [ 2 ] = color . Substring ( 5 , 2 );
			bytes [ 3 ] = color . Substring ( 7 , 2 );
			trans . Value = Convert . ToInt16 ( bytes [ 0 ] , 16 );
			SRed . Value = Convert . ToInt16 ( bytes [ 1 ] , 16 );
			SGreen . Value = Convert . ToInt16 ( bytes [ 2 ] , 16 );
			SBlue . Value = Convert . ToInt16 ( bytes [ 3 ] , 16 );

		}
		private void textBox_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 1;
			ResetFieldColors ( 1 );
			SetSliders ( textBox . Text );
		}
		private void textBox1_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 2;
			ResetFieldColors ( 2 );
			SetSliders ( textBox1 . Text );
		}
		private void textBox2_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 3;
			ResetFieldColors ( 3 );
			SetSliders ( textBox2 . Text );
		}
		private void textBox3_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 4;
			ResetFieldColors ( 4 );
			SetSliders ( textBox3 . Text );
		}
		private void textBox4_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 5;
			ResetFieldColors ( 5 );
			SetSliders ( textBox4 . Text );
		}
		private void textBox5_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 6;
			ResetFieldColors ( 6 );
			SetSliders ( textBox5 . Text );
		}
		private void textBox6_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 7;
			ResetFieldColors ( 7 );
			SetSliders ( textBox6 . Text );
		}
		private void textBox7_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 8;
			ResetFieldColors ( 8 );
			SetSliders ( textBox7 . Text );
		}
		private void textBox8_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 9;
			ResetFieldColors ( 9 );
			SetSliders ( textBox8 . Text );
		}

		private void ShowMsgbox ( object sender , RoutedEventArgs e )
		{
			Utils . Mssg ( 
				caption: "*** SQL Query Error ***" ,
				string1: $"This is the Middle, and main row of data used to \ncreate the information provided.\nThis is  the duplicate to make it longer than the window should be able to use, and  this is  This is  the duplicate to make it longer than the window should be able to use" ,
				string2: "string  2 goes here" ,
				string3: "" ,
				title: "" ,
				iconstring: "\\icons\\error.png" ,
				defButton: 1 ,
				Btn1: 1 ,
				Btn2: 2 ,
				Btn3: 3 ,
				Btn4: 4 ,
				btn1Text: "" ,
				btn2Text: "Get on with it" ,
				btn3Text: "Bale out" ,
				btn4Text: ""
	     );
		}

		private void SaveMsgbox ( object sender , RoutedEventArgs e )
		{
			Window win;
			SaveButtonColors ( );
			if ( DlgInput . MsgboxWin != null )
			{
				win = DlgInput . MsgboxWin;
				Msgbox . updateVars ( );
				win. UpdateLayout ( );
				win . Refresh ( );
			}
		}

		private void ListDps ( object sender , RoutedEventArgs e )
		{
			string output="";
			output = GetValue ( Msgbox . DlgBackGroundProperty ) . ToString ( ) + "\n";
			output += GetValue ( Msgbox . DlgForeGroundProperty ) . ToString ( ) + "\n";
			output += GetValue ( Msgbox . BtnBackGroundProperty ) . ToString ( ) + "\n";
			output += GetValue ( Msgbox . BtnForeGroundProperty ) . ToString ( ) + "\n";
			output += GetValue ( Msgbox . MouseoverBackGroundProperty ) . ToString ( ) + "\n";
			output += GetValue ( Msgbox . MouseoverForeGroundProperty ) . ToString ( ) + "\n";
			output += GetValue ( Msgbox . BorderColorProperty ) . ToString ( ) + "\n";
			info . Text = output;
			output = DlgInput.dlgbackground . ToString ( ) + "\n";
			output += DlgInput .dlgforeground .ToString ( ) + "\n";
			output += DlgInput . btnbackground . ToString ( ) + "\n";
			output += DlgInput . btnforeground . ToString ( ) + "\n";
			output += DlgInput .mousebackground. ToString ( ) + "\n";
			output += DlgInput .mouseforeground. ToString ( ) + "\n";
			output += DlgInput.Btnborder.ToString() + "\n";
			output += DlgInput . defbtnbackground . ToString ( ) + "\n";
			output += DlgInput . defbtnforeground . ToString ( ) + "\n";
			Memvars . Text = output;
		}

		#region focusing
		private void color1_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox . Focus ( );
		}
		private void color2_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox1 . Focus ( );
		}
		private void color3_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox2 . Focus ( );
		}
		private void color4_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox3. Focus ( );
		}
		private void color5_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox4 . Focus ( );
		}
		private void color6_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox5 . Focus ( );
		}
		private void color7_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox6 . Focus ( );
		}
		private void color8_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox7 . Focus ( );
		}
		private void color9_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox8 . Focus ( );
		}
		#endregion focusing
	}
}
