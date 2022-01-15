using Microsoft . SqlServer . Management . Smo;
using Microsoft . SqlServer . Management . XEvent;

using Newtonsoft . Json . Linq;

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
using System . Xml;

using WPFPages . AttachedProperties;
using WPFPages . Converts;
using WPFPages . UserControls;

using static System . Net . Mime . MediaTypeNames;
using static System . Windows . Forms . VisualStyles . VisualStyleElement . Window;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for MsgboxSetup.xaml
	/// </summary>
	public partial class MsgboxSetup : Window
	{
		private int ActiveField = 1;
		private string[] bytes = {"","","","" };
		private static Window DlgHandle;
		public static bool IsLoaded = false;
		public Brush BtnBackground { get; set; }
		public Brush BtnForeground { get; set; }
		public Brush BorderColor { get; set; }
		//public Thickness Bordersize { get; set; }

		public Thickness MouseoverBordersize = new Thickness { Left = 2 , Top = 5 , Right = 2 , Bottom = 0 };
		public Thickness NormalBordersize = new Thickness { Left = 0 , Top = 0 , Right = 0 , Bottom = 5 };
		public string ValidBorderSizes = "0123456789";
		public string ValidColorCode= "#0123456789ABCDEFabcdef";
		#region Startup / Closedown

		public MsgboxSetup ( )
		{
			bool MsgBoxDataFound = true;
			IsLoaded = false;
			InitializeComponent ( );
			IsLoaded = true;
			Console . WriteLine ( " IsLoaded set to TRUE..." );
			Utils . SetupWindowDrag ( this );
			this . Show ( );

			if ( IsLoaded == false )
				return;
			Console . WriteLine ( "In MsgboxSetup ..." );
			DataContext = this;

			if ( File . Exists ( @"Messageboxes.dat" ) == false )
				MsgBoxDataFound = false;
			CreateDefaultDlgInputSettings ( );

			// Read all data to populate DlgInput
			ReadDlgInput ( );

			// Save all settings in case defaults had any effect
			SaveButtonColors ( );

			// Set default border sizes if  neccessary
			if ( DlgInput . BorderSizeNormal == null )
				DlgInput . BorderSizeNormal = NormalBordersize;
			if ( DlgInput . BorderSizeDefault == null )
				DlgInput . BorderSizeDefault = MouseoverBordersize;

			// Setup handlers
			//	MouseMove += Grab_MouseMove;
			KeyDown += Window_PreviewKeyDown;

			// NB  BUTTON 1/ BUTTON 3 ARE THE DEFAULT BUTTONS
			if ( DlgInput . UseDarkMode )
			{
				//----------------------------------------------------------------------------------------------//
				// MessageBox itself  Dark back/foreground
				Image1Border . Background = FindResource ( "Black1" ) as SolidColorBrush;
				Image2Border . Background = FindResource ( "Black1" ) as SolidColorBrush;
				img1MainText1 . Background = FindResource ( "Black1" ) as SolidColorBrush;
				img1MainText2 . Background = FindResource ( "Black1" ) as SolidColorBrush;
				img2MainText1 . Background = FindResource ( "Black1" ) as SolidColorBrush;
				img2MainText2 . Background = FindResource ( "Black1" ) as SolidColorBrush;
				img2MainText3 . Background = FindResource ( "Black1" ) as SolidColorBrush;

				img1MainText1 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				img1MainText2 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				img2MainText1 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				img2MainText2 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				img2MainText3 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				//----------------------------------------------------------------------------------------------//

				// default Dlg Btn - small
				Button1 . Background = DlgInput . defbtnbackgroundDark;
				Button1Text . Foreground = DlgInput . defbtnforegroundDark;
				Button1 . BorderBrush = DlgInput . BtnborderDark;
				//----------------------------------------------------------------------------------------------//

				//default Dlg Btn - large
				Button3 . Background = DlgInput . defbtnbackgroundDark;
				Button3 . Foreground = DlgInput . defbtnforegroundDark;
				Button3 . BorderBrush = DlgInput . BtnborderDark;
				//----------------------------------------------------------------------------------------------//

				//std Dlg Buttons -All								    
				Button2 . Background = DlgInput . btnbackgroundDark;
				Button2Text . Foreground = DlgInput . btnforegroundDark;
				Button2 . BorderBrush = DlgInput . BtnborderDark;

				Button4 . Foreground = DlgInput . btnbackgroundDark;
				Button4 . Background = DlgInput . btnforegroundDark;
				Button4 . BorderBrush = DlgInput . BtnborderDark;

				Button22 . Background = DlgInput . btnbackgroundDark;
				Button22Text . Foreground = DlgInput . btnforegroundDark;
				Button22 . BorderBrush = DlgInput . BtnborderDark;

				Button23 . Background = DlgInput . btnbackgroundDark;
				Button23Text . Foreground = DlgInput . btnforegroundDark;
				Button23 . BorderBrush = DlgInput . BtnborderDark;
				ToggleMode ( );
				textBox9 . Focus ( );
			}
			else
			{
				//----------------------------------------------------------------------------------------------//
				// MessageBox itself back/foreground
				Image1Border . Background = DlgInput . dlgbackground;
				Image2Border . Background = DlgInput . dlgbackground;
				img1MainText1 . Background = DlgInput . dlgbackground;
				img1MainText2 . Background = DlgInput . dlgbackground;
				img2MainText1 . Background = DlgInput . dlgbackground;
				img2MainText2 . Background = DlgInput . dlgbackground;
				img2MainText3 . Background = DlgInput . dlgbackground;

				img1MainText1 . Foreground = DlgInput . dlgforeground;
				img1MainText2 . Foreground = DlgInput . dlgforeground;
				img2MainText1 . Foreground = DlgInput . dlgforeground;
				img2MainText2 . Foreground = DlgInput . dlgforeground;
				img2MainText3 . Foreground = DlgInput . dlgforeground;
				//----------------------------------------------------------------------------------------------//
				// default Dlg Btn - small
				Button1 . Background = DlgInput . defbtnbackground;
				Button1Text . Foreground = DlgInput . defbtnforeground;
				Button1 . BorderBrush = DlgInput . Btnborder;
				//----------------------------------------------------------------------------------------------//
				//default Dlg Btn - large
				Button3 . Background = DlgInput . defbtnbackground;
				Button3 . Foreground = DlgInput . defbtnforeground;
				Button3 . BorderBrush = DlgInput . Btnborder;
				//----------------------------------------------------------------------------------------------//
				// std buttons
				Button2 . Background = DlgInput . btnbackground;
				Button2Text . Foreground = DlgInput . btnforeground;
				Button2 . BorderBrush = DlgInput . Btnborder;

				Button4 . Background = DlgInput . btnbackground;
				Button4 . Foreground = DlgInput . btnforeground;
				Button4 . BorderBrush = DlgInput . Btnborder;

				Button22 . Background = DlgInput . btnbackground;
				Button22Text . Foreground = DlgInput . btnforeground;
				Button22 . BorderBrush = DlgInput . Btnborder;

				Button23 . Background = DlgInput . btnbackground;
				Button23Text . Foreground = DlgInput . btnforeground;
				Button23 . BorderBrush = DlgInput . Btnborder;
				//----------------------------------------------------------------------------------------------//
				textBox . Focus ( );
			}
			//----------------------------------------------------------------------------------------------//
			// No changes needed - border shadow size is global
			Button1 . BorderThickness = DlgInput . BorderSizeNormal;
			Button3 . BorderThickness = DlgInput . BorderSizeNormal;

			Button2 . BorderThickness = DlgInput . BorderSizeNormal;
			Button4 . BorderThickness = DlgInput . BorderSizeNormal;
			Button23 . BorderThickness = DlgInput . BorderSizeNormal;
			Button22 . BorderThickness = DlgInput . BorderSizeNormal;
			//----------------------------------------------------------------------------------------------//
			// NB All buttons use same shadow color, only size varies between default and standard
			// Large Dlg
			//Button23 . Background = color3 . Background;
			//Button23Text . Foreground = DlgInput . btnforeground;
			//Button2 . Background = color3 . Background;
			//Button2Text . Foreground = DlgInput . btnforeground;
			//Button22 . Background = color3 . Background;
			//Button22Text . Foreground = DlgInput . btnforeground;
			////----------------------------------------------------------------------------------------------//
			////Small Dlg  
			//Button4 . Background = color3 . Background;
			//----------------------------------------------------------------------------------------------//
			//default btn			
			Button1 . UpdateLayout ( );
			Button3 . UpdateLayout ( );

			Button23 . UpdateLayout ( );
			Button22 . UpdateLayout ( );
			Button2 . UpdateLayout ( );
			Button4 . UpdateLayout ( );

			Button3 . BringIntoView ( );
			Button1 . BringIntoView ( );

			Button23 . BringIntoView ( );
			Button22 . BringIntoView ( );
			Button2 . BringIntoView ( );
			Button4 . BringIntoView ( );
			//----------------------------------------------------------------------------------------------//
			// Force startup in normal mode 
			//DlgInput . UseDarkMode = false;
			//ToggleMode ( );

			if ( MsgBoxDataFound == false )
				Utils . Mbox ( this , string1: "The Configuration data for the MessageBox system was not found ! " , string2: "Therefore default settings have been generated for you.  You can modify these using the Messagebox configuration option.." , caption: "" , iconstring: "/icons/green-tick.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK , minsize: true );

			// Ensure text fields have white background
			// background
			ResetDefaultFieldColors ( 0 );

			//			RefreshDlgTextFields ( color2 . Background );
			// force dialog samples to repaint
			//			darkmode_Click ( null , null );
			//Image1Bkgrnd . Width = image1 .ActualWidth +95;
			//Image1Bkgrnd . Height = image1 . ActualHeight;
			//Image2Bkgrnd . Width = image2 . ActualWidth;
			//Image2Bkgrnd . Height = image2 . ActualHeight;
		}
		private void Window_Closing ( object sender , System . ComponentModel . CancelEventArgs e )
		{
			SaveButtonColors ( );
			Mouse . OverrideCursor = Cursors . Arrow;
		}

		#endregion Startup / Closedown

		// Toggle between Std and Dark mode

		#region KEY handlers
		private void Window_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . F11 )
			{
				var pos = Mouse . GetPosition ( this);
				Utils . Grab_Object ( sender , pos );
				if ( Utils . ControlsHitList . Count == 0 )
					return;
				Utils . Grabscreen ( this , Utils . ControlsHitList [ 0 ] . VisualHit , null , sender as Control );
			}
			if ( e . Key == Key . F1 )
			{
				ListDlgInput ( );
				e . Handled = true;
			}
		}

		private void Button_Click ( object sender , RoutedEventArgs e )
		{
			//SaveButtonColors ( );
			Mouse . OverrideCursor = Cursors . Arrow;
			DlgInput . MsgboxWin = null;
			this . Close ( );
		}
		private void textBox_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
			{
				if ( textBox . Text . Contains ( "#" ) == false && CheckValid ( textBox . Text ) )
				{
					textBox . Text = "#" + textBox . Text;
					color1 . Background = textBox . Text . ToSolidBrush ( );
					ActiveField = 1;
				}
			}
		}
		private void textBox1_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
			{
				if ( textBox1 . Text . Contains ( "#" ) == false && CheckValid ( textBox1 . Text ) )
					textBox1 . Text = "#" + textBox1 . Text;
				color2 . Background = textBox1 . Text . ToSolidBrush ( );
				ActiveField = 2;
				SetSliders ( textBox1 . Text );
			}
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
						validcount++;
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

		//*******************************************************************************************************//
		// START OF NORMAL COLORS
		private void textBox2_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox2.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			ActiveField = 3;
			SetSliders ( input );
			DlgInput . btnbackground = input . ToSolidBrush ( );
		}
		private void textBox3_KeyUp ( object sender , KeyEventArgs e )
		{
			ActiveField = 4;
			string input = textBox3.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			SetSliders ( input );
			DlgInput . btnforeground = color4 . Background;
		}
		private void textBox4_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox4.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			SetSliders ( input );
			DlgInput . Btnborder = color5 . Background;
			ActiveField = 5;
			SetSliders ( textBox4 . Text );
		}

		private void textBox5_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox5.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			DlgInput . Btnmousebackground = color6 . Background;
			ActiveField = 6;
			SetSliders ( textBox5 . Text );
		}
		private void textBox6_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox6.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			DlgInput . Btnmouseforeground = color7 . Background;
			ActiveField = 7;
			SetSliders ( textBox6 . Text );
		}
		private void textBox7_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox7.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			DlgInput . defbtnbackground = color8 . Background;
			ActiveField = 8;
			SetSliders ( textBox7 . Text );
		}
		private void textBox8_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox8.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			DlgInput . defbtnforeground = color9 . Background;
			ActiveField = 9;
			SetSliders ( textBox8 . Text );
		}

		// END OF NORMAL COLORS

		//*******************************************************************************************************//

		// START OF DARK MODE COLORS

		private void textBox9_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox9.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			DlgInput . BtnborderDark = color10 . Background;
			ActiveField = 10;
			SetSliders ( textBox9 . Text );
		}
		private void textBox10_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox10.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			DlgInput . btnbackgroundDark = color11 . Background;
			ActiveField = 11;
			SetSliders ( textBox10 . Text );
		}
		private void textBox11_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox11.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			DlgInput . btnforegroundDark = color12 . Background;
			ActiveField = 12;
			SetSliders ( textBox11 . Text );
		}

		private void textBox12_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox12.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			DlgInput . mouseborderDark = color13 . Background;
			ActiveField = 13;
			SetSliders ( textBox12 . Text );
		}

		private void textBox13_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox13.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			DlgInput . mousebackgroundDark = color14 . Background;
			ActiveField = 14;
			SetSliders ( textBox13 . Text );
		}

		private void textBox14_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox14.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			DlgInput . mouseforegroundDark = color15 . Background;
			ActiveField = 15;
			SetSliders ( textBox14 . Text );
		}

		private void textBox15_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox15.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			DlgInput . defbtnbackgroundDark = color16 . Background;
			ActiveField = 16;
			SetSliders ( textBox15 . Text );
		}

		private void textBox16_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = textBox16.Text;
			if ( input . Length < 9 )
				return;
			if ( CheckValidColorCode ( input ) == false )
				return;

			if ( input . Contains ( "#" ) == false )
				return;
			DlgInput . defbtnforegroundDark = color17 . Background;
			ActiveField = 17;
			SetSliders ( textBox16 . Text );
		}

		//Shadow size fields
		private void textBox17_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( textBox17 . Text != "" && CheckValid ( textBox17 . Text ) )
			{
				DlgInput . BorderSizeDefault . Left = Convert . ToDouble ( textBox17 . Text );
				ActiveField = 18;
				ResSetColorSelectionDisplay ( 0 , "" );
			}
			else
				textBox17 . Text = "0";
		}
		//*******************************************************************************************************//
		// END OF DARK MODE COLORS


		// Boder size fields
		private void textBox18_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( textBox18 . Text != "" && CheckValid ( textBox18 . Text ) )
			{
				DlgInput . BorderSizeNormal . Top = Convert . ToDouble ( textBox18 . Text );
				ActiveField = 19;
				ResSetColorSelectionDisplay ( 0 , "" );
			}
			else
				textBox18 . Text = "0";
		}
		private void textBox19_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( textBox19 . Text != "" && CheckValid ( textBox19 . Text ) )
			{
				DlgInput . BorderSizeNormal . Right = Convert . ToDouble ( textBox19 . Text );
				ActiveField = 20;
				ResSetColorSelectionDisplay ( 0 , "" );
			}
			else
				textBox19 . Text = "0";
		}
		private void textBox20_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( textBox20 . Text != "" && CheckValid ( textBox20 . Text ) )
			{
				DlgInput . BorderSizeNormal . Bottom = Convert . ToDouble ( textBox20 . Text );
				ActiveField = 21;
				ResSetColorSelectionDisplay ( 0 , "" );
			}
			else
				textBox20 . Text = "0";
		}
		private void fontlarge1_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = fontsizelarge1. Text;
			if ( input != "" )
			{
				if ( CheckValidTextSize ( input ) == "" )
					return;
				if ( input . Length > 2 )
				{
					Utils . DoErrorBeep ( 280 , 100 , 1 );
					Utils . DoErrorBeep ( 220 , 75 , 1 );
					if ( Convert . ToDouble ( input . Substring ( 0 , 2 ) ) < 99 )
						fontsizelarge1 . Text = input . Substring ( 0 , 2 );
					else if ( Convert . ToDouble ( input . Substring ( 1 , 2 ) ) < 99 )
						fontsizelarge1 . Text = input . Substring ( 1 , 2 );
					img2MainText1 . FontSize = Convert . ToDouble ( fontsizelarge1 . Text );
					img2MainText1 . UpdateLayout ( );
					img2MainText2 . FontSize = Convert . ToDouble ( fontsizelarge1 . Text );
					img2MainText2 . UpdateLayout ( );
					SaveButtonColors ( );
					return;
				}

				if ( input . Length > 0 )
				{
					if(Convert.ToDouble(input) > 21)
					{
						Utils . DoErrorBeep ( 250 , 120 , 1 );
						Utils . DoErrorBeep ( 165 , 750 , 1 );
						fontsizelarge1 . Text = "21";

					}
					try
					{
						img2MainText1 . FontSize = Convert . ToDouble ( input );
						img2MainText1 . UpdateLayout ( );
						img2MainText2 . FontSize = Convert . ToDouble ( input );
						img2MainText2 . UpdateLayout ( );
						SaveButtonColors ( );
					}
					catch ( Exception ex )
					{
						fontsizelarge1 . Text = "13";
						return;
					}
				}
				else
					fontsizelarge1 . Text = "13";
			}
			else
				return;
			ActiveField = 22;
		}
		private void fontlarge3_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = fontsizelarge3. Text;
			if ( input != "" )
			{
				if ( CheckValidTextSize ( input ) == "" )
					return;
				if ( input . Length > 2 )
				{
					Utils . DoErrorBeep ( 280 , 100 , 1 );
					Utils . DoErrorBeep ( 220 , 75 , 1 );
					if ( Convert . ToDouble ( input . Substring ( 0 , 2 ) ) < 99 )
						fontsizelarge3 . Text = input . Substring ( 0 , 2 );
					else if ( Convert . ToDouble ( input . Substring ( 1 , 2 ) ) < 99 )
						fontsizelarge3 . Text = input . Substring ( 1 , 2 );
					img2MainText3 . FontSize = Convert . ToDouble ( fontsizelarge3 . Text );
					img2MainText3 . UpdateLayout ( );
					SaveButtonColors ( );
					return;
				}

				if ( input . Length > 0 )
				{
					if ( Convert . ToDouble ( input ) > 21 )
					{
						Utils . DoErrorBeep ( 250 , 120 , 1 );
						Utils . DoErrorBeep ( 165 , 750 , 1 );
						fontsizelarge1 . Text = "21";

					}
					try
					{
						img2MainText3 . FontSize = Convert . ToDouble ( input );
						img2MainText3 . UpdateLayout ( );
						SaveButtonColors ( );
					}
					catch ( Exception ex )
					{
						fontsizelarge3 . Text = "13";
						return;
					}
				}
				else
					fontsizelarge3 . Text = "13";
			}
			else
				return;
			ActiveField = 23;
		}
		private void fontsizesmall1_KeyUp ( object sender , KeyEventArgs e )
		{
			string input = fontsizesmall1 . Text;
			if ( input != "" )
			{
				if ( CheckValidTextSize ( input ) == "" )
					return;
				if ( input . Length > 2 )
				{
					Utils . DoErrorBeep ( 280 , 100 , 1 );
					Utils . DoErrorBeep ( 220 , 75 , 1 );
					if ( Convert . ToDouble ( input . Substring ( 0 , 2 ) ) < 99 )
						fontsizesmall1 . Text = input . Substring ( 0 , 2 );
					else if ( Convert . ToDouble ( input . Substring ( 1 , 2 ) ) < 99 )
						fontsizesmall1 . Text = input . Substring ( 1 , 2 );
					img1MainText1 . FontSize = Convert . ToDouble ( fontsizesmall1 . Text );
					img1MainText1 . UpdateLayout ( );
					return;
				}

				if ( input . Length > 0 )
				{
					try
					{
						img1MainText1 . FontSize = Convert . ToDouble ( input );
						img1MainText1 . UpdateLayout ( );
						SaveButtonColors ( );
					}
					catch ( Exception ex )
					{
						fontsizesmall1 . Text = "13";
						return;
					}
				}
				else
					fontsizesmall1 . Text = "13";
			}
			else
				return;
			ActiveField = 25;
		}
		private void fontsizesmall2_KeyUp ( object sender , KeyEventArgs e )
		{
			string input =fontsizesmall2.Text;

			if ( input != "" )
			{
				if ( CheckValidTextSize ( input ) == "" )
					return;
				if ( input . Length > 0 )
				{
					if ( input . Length > 2 )
					{
						Utils . DoErrorBeep ( 280 , 100 , 1 );
						Utils . DoErrorBeep ( 220 , 75 , 1 );
						if ( Convert . ToDouble ( input . Substring ( 0 , 2 ) ) < 99 )
							fontsizesmall2 . Text = input . Substring ( 0 , 2 );
						else if ( Convert . ToDouble ( input . Substring ( 1 , 2 ) ) < 99 )
							fontsizesmall2 . Text = input . Substring ( 1 , 2 );
						img1MainText2 . FontSize = Convert . ToDouble ( fontsizesmall2 . Text );
						img1MainText2 . UpdateLayout ( );
						SaveButtonColors ( );
						return;
					}
					try
					{
						img1MainText2 . FontSize = Convert . ToDouble ( input );
						img1MainText2 . UpdateLayout ( );
						SaveButtonColors ( );
					}
					catch ( Exception ex )
					{
						fontsizesmall2 . Text = "13";
						return;
					}
				}
				else
					fontsizesmall2 . Text = "13";
			}
			else
				fontsizesmall2 . Text = "13";
			ActiveField = 26;
		}

		#endregion KEY handlers

		#region field focusing (Default buttons)
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
		private void textBox9_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			ActiveField = 10;
		}
		private void textBox10_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			ActiveField = 11;
		}
		private void textBox11_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			ActiveField = 12;
		}
		#endregion dield identifiers    		

		#region mouse down handlers for fields
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
			textBox3 . Focus ( );
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
		private void color10_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox9 . Focus ( );
		}
		private void color11_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox10 . Focus ( );
		}
		private void color12_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox11 . Focus ( );
		}
		private void color13_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox12 . Focus ( );
		}
		private void color14_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox13 . Focus ( );
		}
		private void color15_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox14 . Focus ( );
		}
		private void color16_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox15 . Focus ( );
		}
		private void color17_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox16 . Focus ( );
		}
		private void color18_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox17 . Focus ( );
		}
		private void color19_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox18 . Focus ( );
		}
		private void color20_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox19 . Focus ( );
		}
		private void color21_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			textBox20 . Focus ( );
		}
		private void fontlarge1_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			fontsizelarge1 . Focus ( );
		}
		private void fontlarge3_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			fontsizelarge3 . Focus ( );
		}
		private void fontsizesmall1_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			fontsizesmall1 . Focus ( );
		}
		private void fontsizesmall2_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			fontsizesmall2 . Focus ( );
		}
		#endregion mouse down handlers for fields

		#region inline dialog boses for testing

		private void ShowMinMsgbox ( object sender , RoutedEventArgs e )
		{
			if ( DlgInput . MsgboxMinWin != null )
			{
				DlgInput . MsgboxMinWin . BringIntoView ( );
				DlgInput . MsgboxMinWin . Focus ( );
				return;
			}
			SaveButtonColors ( );
			Utils . Mbox ( this , string1: "This is  the Main Row of text that will contain the relevant information that has caused this dialog to be spawned to display to the end user..." ,
				string2: "",
				caption: "*** This is the SMALLEST Message Box Caption ***" ,
				iconstring: "\\icons\\Information2.png" ,
				Btn1: MB . YES , Btn2: MB . NO , 0 , 0 , defButton: MB . YES , true );
		}

		//***********************************************************************************//
		// Show Smaller/ One Liner Message Box
		//***********************************************************************************//
		private void ShowShortMsgbox ( object sender , RoutedEventArgs e )
		{
			if ( DlgInput . MsgboxSmallWin != null )
			{
				DlgInput . MsgboxSmallWin . BringIntoView ( );
				DlgInput . MsgboxSmallWin . Focus ( );
				return;
			}
			SaveButtonColors ( );
				//string2: "This is the 2nd row of text to contain additional info or to provide a prompt to your user..." ,
			Utils . Mbox ( this , string1: "This is  the Main Row of text that will contain the relevant information that has caused this dialog to be spawned to display to the End User...." ,
				string2:"This is the 2nd row of text to contain additional info ." ,
				caption: "*** This is the DEFAULT short MessageBox Box Caption ***" ,
				iconstring: "\\icons\\blue-info-icon.png" ,
		Btn1: MB . YES , Btn2: MB . NO, Btn3:MB.CANCEL, Btn4:0, defButton: MB . YES , false );
		}
		//***********************************************************************************//
		// Show full size Message Box
		//***********************************************************************************//
		private void ShowMsgbox ( object sender , RoutedEventArgs e )
		{
			if ( DlgInput . MsgboxWin != null )
			{
				DlgInput . MsgboxWin . BringIntoView ( );
				DlgInput . MsgboxWin . Focus ( );
				return;
			}
			SaveButtonColors ( );
			DlgInput . stringin = "this is  the string passed in...";
			DlgInput . intin = 12345;
			string[] file = {"","","","" };
			file [ 0 ] = "\\icons\\green-tick.png";
			file [ 1 ] = "\\icons\\red-cross-icon.png";
			file [ 2 ] = "\\icons\\blue-info-icon.png";
			file [ 3 ] = "\\icons\\3d-yellow-shriek.png";

			//contrtol border color for testing	   (0 - 3)
			int selfile = 0;

			Utils . Mssg (
				caption: "*** This is  the FULL Size Message Box Caption  ***" ,
				"This is  the Main Row of text that will contain the relevant information that has caused this dialog to be spawned to display to the end user..." ,
				string2: "This is the 2nd row of text to contain additional info or to provide a prompt to your user..." ,
				string3: "This 3rd row of text can prompt your user to take the best decision" ,
				title: "" ,
				iconstring: file [ selfile ] ,
				Btn1: 1 ,
				Btn2: 2 ,
				Btn3: 3 ,
				Btn4: 4 ,
				btn1Text: "" ,
				btn2Text: "Get on with it" ,
				btn3Text: "Bale out" ,
				btn4Text: "" ,
				usedialog: false
		);
		}
		#endregion inline dialog boses for testing

		//private void Grab_MouseMove ( object sender , MouseEventArgs e )
		//{
		//	//if ( e . LeftButton == MouseButtonState . Pressed )
		//	//	Utils . Grab_MouseMove ( sender , e );
		//	//e . Handled = true;
		//}

		#region Mouse OVER  movement effects handlers (MouseOver)
		//************************************** Start default buttons *****************************//
		private void Button1_MouseEnter ( object sender , MouseEventArgs e )
		{     //Default
			if ( DlgInput . UseDarkMode )
			{
				Button1 . Background = DlgInput . mousebackgroundDark;
				Button1Text . Foreground = DlgInput . mouseforegroundDark;
				Button1 . BorderBrush = DlgInput . mouseborderDark;
			}
			else
			{
				Button1 . Background = DlgInput . Btnmousebackground;
				Button1Text . Foreground = DlgInput . Btnmouseforeground;
				Button1 . BorderBrush = DlgInput . Btnborder;
			}
			Button1 . BorderThickness = DlgInput . BorderSizeDefault;
			Button1 . UpdateLayout ( );
		}
		private void Button1_MouseLeave ( object sender , MouseEventArgs e )
		{     //Default
			if ( DlgInput . UseDarkMode )
			{
				Button1 . Background = DlgInput . defbtnbackgroundDark;
				Button1Text . Foreground = DlgInput . defbtnforegroundDark;
				Button1 . BorderBrush = DlgInput . BtnborderDark;
			}
			else
			{
				Button1 . Background = DlgInput . defbtnbackground;
				Button1Text . Foreground = DlgInput . defbtnforeground;
				Button1 . BorderBrush = DlgInput . Btnborder;
			}
			Button1 . BorderThickness = DlgInput . BorderSizeNormal;
			Button1 . UpdateLayout ( );
		}
		private void Button3_MouseEnter ( object sender , MouseEventArgs e )
		{      //Default
			if ( DlgInput . UseDarkMode )
			{
				Button3 . Background = DlgInput . mousebackgroundDark;
				Button3 . Foreground = DlgInput . mouseforegroundDark;
				Button3 . BorderBrush = DlgInput . mouseborderDark;
			}
			else
			{
				Button3 . Background = DlgInput . Btnmousebackground;
				Button3 . Foreground = DlgInput . Btnmouseforeground;
				Button3 . BorderBrush = DlgInput . Btnborder;
			}
			Button3 . BorderThickness = DlgInput . BorderSizeDefault;
			Button3 . UpdateLayout ( );
		}
		private void Button3_MouseLeave ( object sender , MouseEventArgs e )
		{     //Default
			if ( DlgInput . UseDarkMode )
			{
				Button3 . Background = DlgInput . defbtnbackgroundDark;
				Button3 . Foreground = DlgInput . defbtnforegroundDark;
				Button3 . BorderBrush = DlgInput . BtnborderDark;
			}
			else
			{
				Button3 . Background = DlgInput . defbtnbackground;
				Button3 . Foreground = DlgInput . defbtnforeground;
				Button3 . BorderBrush = DlgInput . Btnborder;
			}
			Button3 . BorderThickness = DlgInput . BorderSizeNormal;
			Button3 . UpdateLayout ( );
		}
		//************************************** end default buttons *****************************//

		//************************************** Start Normal buttons *****************************//
		private void Button2_MouseEnter ( object sender , MouseEventArgs e )
		{     // Normal
			if ( DlgInput . UseDarkMode )
			{
				Button2 . Background = DlgInput . mousebackgroundDark;
				Button2Text . Foreground = DlgInput . mouseforegroundDark;
				Button2 . BorderBrush = DlgInput . mouseborderDark;
			}
			else
			{
				Button2 . Background = DlgInput . Btnmousebackground;
				Button2Text . Foreground = DlgInput . Btnmouseforeground;
				Button2 . BorderBrush = DlgInput . Btnborder;
			}
			Button2 . BorderThickness = DlgInput . BorderSizeDefault;
			Button2 . UpdateLayout ( );
		}
		private void Button2_MouseLeave ( object sender , MouseEventArgs e )
		{     //Normal
			if ( DlgInput . UseDarkMode )
			{
				Button2 . Background = DlgInput . btnbackgroundDark;
				Button2Text . Foreground = DlgInput . btnforegroundDark;
				Button2 . BorderBrush = DlgInput . BtnborderDark;
			}
			else
			{
				Button2 . Background = DlgInput . btnbackground;
				Button2Text . Foreground = DlgInput . btnforeground;
				Button2 . BorderBrush = DlgInput . Btnborder;
			}
			Button2 . BorderThickness = DlgInput . BorderSizeNormal;
			Button2 . UpdateLayout ( );
		}
		private void Button22_MouseEnter ( object sender , MouseEventArgs e )
		{      // Normal
			if ( DlgInput . UseDarkMode )
			{
				Button22 . Background = DlgInput . mousebackgroundDark;
				Button22Text . Foreground = DlgInput . mouseforegroundDark;
				Button22 . BorderBrush = DlgInput . mouseborderDark;
			}
			else
			{
				Button22 . Background = DlgInput . Btnmousebackground;
				Button22Text . Foreground = DlgInput . Btnmouseforeground;
				Button22 . BorderBrush = DlgInput . Btnborder;
			}
			Button22 . BorderThickness = DlgInput . BorderSizeDefault;
			Button22 . UpdateLayout ( );
		}
		private void Button22_MouseLeave ( object sender , MouseEventArgs e )
		{     //Normal
			if ( DlgInput . UseDarkMode )
			{
				Button22 . Background = DlgInput . btnbackgroundDark;
				Button22Text . Foreground = DlgInput . btnforegroundDark;
				Button22 . BorderBrush = DlgInput . BtnborderDark;
			}
			else
			{
				Button22 . Background = DlgInput . btnbackground;
				Button22Text . Foreground = DlgInput . btnforeground;
				Button22 . BorderBrush = DlgInput . Btnborder;
			}
			Button22 . BorderThickness = DlgInput . BorderSizeNormal;
			Button22 . UpdateLayout ( );
		}
		private void Button23_MouseEnter ( object sender , MouseEventArgs e )
		{     //Normal
			if ( DlgInput . UseDarkMode )
			{
				Button23 . Background = DlgInput . mousebackgroundDark;
				Button23Text . Foreground = DlgInput . mouseforegroundDark;
				Button23 . BorderBrush = DlgInput . mouseborderDark;
			}
			else
			{
				Button23 . Background = DlgInput . Btnmousebackground;
				Button23Text . Foreground = DlgInput . Btnmouseforeground;
				Button23 . BorderBrush = DlgInput . Btnborder;
			}
			//			Button23 . BorderBrush = DlgInput . Btnborder;
			Button23 . BorderThickness = DlgInput . BorderSizeDefault;
			Button23 . UpdateLayout ( );
		}
		private void Button23_MouseLeave ( object sender , MouseEventArgs e )
		{
			if ( DlgInput . UseDarkMode )
			{
				Button23 . Background = DlgInput . btnbackgroundDark;
				Button23Text . Foreground = DlgInput . btnforegroundDark;
				Button23 . BorderBrush = DlgInput . BtnborderDark;
			}
			else
			{
				Button23 . Background = DlgInput . btnbackground;
				Button23Text . Foreground = DlgInput . btnforeground;
				Button23 . BorderBrush = DlgInput . Btnborder;
			}
			Button23 . BorderThickness = DlgInput . BorderSizeNormal;
			//			Button23 . BorderBrush = DlgInput . BtnborderDark;
			Button23 . UpdateLayout ( );
		}
		private void Button4_MouseEnter ( object sender , MouseEventArgs e )
		{     //Normal
			if ( DlgInput . UseDarkMode )
			{
				Button4 . Background = DlgInput . mousebackgroundDark;
				Button4 . Foreground = DlgInput . mouseforegroundDark;
				Button4 . BorderBrush = DlgInput . mouseborderDark;
			}
			else
			{
				Button4 . Background = DlgInput . Btnmousebackground;
				Button4 . Foreground = DlgInput . Btnmouseforeground;
				Button4 . BorderBrush = DlgInput . Btnborder;
			}
			//			Button4. BorderBrush = DlgInput . Btnborder;
			Button4 . BorderThickness = DlgInput . BorderSizeDefault;
			Button4 . UpdateLayout ( );
		}
		private void Button4_MouseLeave ( object sender , MouseEventArgs e )
		{     //Normal
			if ( DlgInput . UseDarkMode )
			{
				Button4 . Background = DlgInput . btnbackgroundDark;
				Button4 . Foreground = DlgInput . btnforegroundDark;
				Button4 . BorderBrush = DlgInput . BtnborderDark;
			}
			else
			{
				Button4 . Background = DlgInput . btnbackground;
				Button4 . Foreground = DlgInput . btnforeground;
				Button4 . BorderBrush = DlgInput . Btnborder;
			}
			Button4 . BorderThickness = DlgInput . BorderSizeNormal;
			//			Button4 . BorderBrush = DlgInput . BtnborderDark;
			Button4 . UpdateLayout ( );
		}
		//************************************** End Normal buttons *****************************//

		#endregion MouseoVER  movement effects handlers

		#region Refresh/ color change  methods

		// force repaint of all BUTTONS
		private void RefreshAllObjects ( )
		{
			if ( IsLoaded == false )
				return;
			//			ResetAllColors ( );
			//Button3 & Button 1 are the  Default buttons)
			// order across   is
			//  Small Dlg  :  ***3***,  4, ,
			//			// Large Dlg  : 23 ,  2, ***1***,  22

			//			Console . WriteLine ( "In RefreshAllObjects ..." );
			if ( DlgInput . UseDarkMode )
			{
				//----------------------------------------------------------------------------------------------//
				//DEFAULT BTN - Large Dlg
				Button1 . Background = DlgInput . defbtnbackgroundDark;
				Button1Text . Foreground = DlgInput . defbtnforegroundDark;
				Button1 . BorderThickness = DlgInput . BorderSizeNormal;
				Button1 . BorderBrush = DlgInput . BtnborderDark;
				//----------------------------------------------------------------------------------------------//
				//DEFAULT BTN - Small Dlg
				Button3 . Background = DlgInput . defbtnbackgroundDark;
				Button3 . Foreground = DlgInput . defbtnforegroundDark;
				Button3 . BorderThickness = DlgInput . BorderSizeNormal;
				Button3 . BorderBrush = DlgInput . BtnborderDark;
				//----------------------------------------------------------------------------------------------//
				// STD buttons - Small Dlg 
				Button23 . Background = DlgInput . btnbackgroundDark;
				Button23Text . Foreground = DlgInput . btnforeground;

				Button2 . Background = DlgInput . btnbackgroundDark;
				Button2Text . Foreground = DlgInput . btnforeground;

				Button22 . Background = DlgInput . btnbackgroundDark;
				Button22Text . Foreground = DlgInput . btnforeground;
				//----------------------------------------------------------------------------------------------//

				// STD buttons - Large Dlg 
				Button2 . Background = DlgInput . btnbackgroundDark;
				Button4 . Background = DlgInput . btnbackgroundDark;
				Button22 . Background = DlgInput . btnbackgroundDark;
				Button23 . Background = DlgInput . btnbackgroundDark;

				Button2 . BorderBrush = DlgInput . BtnborderDark;
				Button4 . BorderBrush = DlgInput . BtnborderDark;
				Button22 . BorderBrush = DlgInput . BtnborderDark;
				Button23 . BorderBrush = DlgInput . BtnborderDark;

				Button2Text . Foreground = DlgInput . btnforegroundDark;
				Button4 . Foreground = DlgInput . btnforegroundDark;
				Button22Text . Foreground = DlgInput . btnforegroundDark;
				Button23Text . Foreground = DlgInput . btnforegroundDark;
				//----------------------------------------------------------------------------------------------//
			}
			else
			{
				//----------------------------------------------------------------------------------------------//
				//DEFAULT BTN - Small Dlg
				Button3 . Background = DlgInput . defbtnbackground;
				Button3 . Foreground = DlgInput . defbtnforeground;
				Button3 . BorderThickness = DlgInput . BorderSizeNormal;
				Button3 . BorderBrush = DlgInput . Btnborder;
				//----------------------------------------------------------------------------------------------//
				//DEFAULT BTN - Large Dlg
				Button1 . Background = DlgInput . defbtnbackground;
				Button1Text . Foreground = DlgInput . defbtnforeground;
				Button1 . BorderBrush = DlgInput . Btnborder;
				//----------------------------------------------------------------------------------------------//

				// Large Dlg - std buttons
				Button4 . Background = DlgInput . btnbackground;
				Button4 . Foreground = DlgInput . btnforeground;
				Button4 . Background = DlgInput . btnbackground;
				Button4 . BorderThickness = DlgInput . BorderSizeNormal;
				Button4 . BorderBrush = DlgInput . Btnborder;
				Button2 . BorderThickness = DlgInput . BorderSizeNormal;
				Button2 . BorderBrush = DlgInput . Btnborder;
				Button22 . BorderThickness = DlgInput . BorderSizeNormal;
				Button22 . BorderBrush = DlgInput . Btnborder;
				Button23 . BorderThickness = DlgInput . BorderSizeNormal;
				Button23 . BorderBrush = DlgInput . Btnborder;

				Button23 . Background = DlgInput . btnbackground;
				Button23Text . Foreground = DlgInput . btnforeground;

				Button2 . Background = DlgInput . btnbackground;
				Button2Text . Foreground = DlgInput . btnforeground;

				Button22 . Background = DlgInput . btnbackground;
				Button22Text . Foreground = DlgInput . btnforeground;
			}

			Button1 . Refresh ( );
			Button2 . Refresh ( );
			Button3 . Refresh ( );
			Button4 . Refresh ( );
			this . Refresh ( );
		}
		// Reset the entire windows colors as we switch from normal to dark mode
		public void ResetAllColors ( )
		{
			if ( IsLoaded == false )
				return;
			Console . WriteLine ( "In ResetAllColors ..." );

			//			SaveButtonColors ( );
			ReadDlgInput ( );
			Button1 . UpdateLayout ( );
			Button2 . UpdateLayout ( );
			Button3 . UpdateLayout ( );
			Button4 . UpdateLayout ( );
			this . UpdateLayout ( );
		}

		#region Sliders
		private void trans_ValueChanged ( object sender , RoutedPropertyChangedEventArgs<double> e )
		{
			ResSetColorSelectionDisplay ( e . NewValue , "TRANS" );
		}
		private void SRed_ValueChanged ( object sender , RoutedPropertyChangedEventArgs<double> e )
		{
			ResSetColorSelectionDisplay ( e . NewValue , "RED" );
		}
		private void SGreen_ValueChanged ( object sender , RoutedPropertyChangedEventArgs<double> e )
		{
			ResSetColorSelectionDisplay ( e . NewValue , "GREEN" );
		}
		private void SBlue_ValueChanged ( object sender , RoutedPropertyChangedEventArgs<double> e )
		{
			ResSetColorSelectionDisplay ( e . NewValue , "BLUE" );
		}
		#endregion Sliders

		#endregion Refresh methods

		#region support methods

		private void SetSliders ( string color )
		{
			string[] bytes = {"","","",""};
			if ( color == "" )
				color = "#FFFFFFFF";
			if ( color . Length < 9 || color [ 0 ] != '#' )
				return;
			bytes [ 0 ] = color . Substring ( 1 , 2 );
			bytes [ 1 ] = color . Substring ( 3 , 2 );
			bytes [ 2 ] = color . Substring ( 5 , 2 );
			bytes [ 3 ] = color . Substring ( 7 , 2 );
			trans . Value = Convert . ToInt16 ( bytes [ 0 ] , 16 );
			SRed . Value = Convert . ToInt16 ( bytes [ 1 ] , 16 );
			SGreen . Value = Convert . ToInt16 ( bytes [ 2 ] , 16 );
			SBlue . Value = Convert . ToInt16 ( bytes [ 3 ] , 16 );

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

		private void SaveMsgbox ( object sender , RoutedEventArgs e )
		{
			Window win;
			SaveButtonColors ( );
			if ( DlgInput . MsgboxWin != null )
			{
				win = DlgInput . MsgboxWin;
				win . UpdateLayout ( );
				win . Refresh ( );
			}
		}
		#endregion support methods

		#region Utility (low level) mehods

		// Returns a new Hex color string (based on value from slider mostly)
		private string CalculateNewColor ( double value , string caller )
		{
			string output="", temp="", clr="";
			byte t=0, r=0,g=0,b=0;

			double val=0.00;
			if ( IsLoaded == false )
				return "";
			//			Console . WriteLine ( "In CalculateNewColor ..." );

			// Find out the active field we are working with
			temp = GetActiveValue ( );
			if ( temp == "" )
				temp = "#FFFF00FF";
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
		// return current selected item as a color string (#FF112233)
		private string GetActiveValue ( )
		{
			string output="";
			if ( IsLoaded == false )
				return "";
			//			Console . WriteLine ( "In GetActiveValue ..." );
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
			else if ( ActiveField == 10 )
				output = textBox9 . Text;
			else if ( ActiveField == 11 )
				output = textBox10 . Text;
			else if ( ActiveField == 12 )
				output = textBox11 . Text;
			else if ( ActiveField == 13 )
				output = textBox12 . Text;
			else if ( ActiveField == 14 )
				output = textBox13 . Text;
			else if ( ActiveField == 15 )
				output = textBox14 . Text;
			else if ( ActiveField == 16 )
				output = textBox15 . Text;
			else if ( ActiveField == 17 )
				output = textBox16 . Text;
			return output;
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

		#endregion Utility (low level) mehods

		#region field Set focus
		private void textBox_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 1;
			//			ResetDefaultFieldColors ( 1 );
			SetSliders ( textBox . Text );
		}
		private void textBox1_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 2;
			//			ResetDefaultFieldColors ( 2 );
			SetSliders ( textBox1 . Text );
		}
		private void textBox2_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 3;
			//			ResetDefaultFieldColors ( 3 );
			SetSliders ( textBox2 . Text );
		}
		private void textBox3_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 4;
			//			ResetDefaultFieldColors ( 4 );
			SetSliders ( textBox3 . Text );
		}
		private void textBox4_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 5;
			//			ResetDefaultFieldColors ( 5 );
			SetSliders ( textBox4 . Text );
		}
		private void textBox5_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 6;
			//			ResetDefaultFieldColors ( 6 );
			SetSliders ( textBox5 . Text );
		}
		private void textBox6_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 7;
			//			ResetDefaultFieldColors ( 7 );
			SetSliders ( textBox6 . Text );
		}
		private void textBox7_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 8;
			//			ResetDefaultFieldColors ( 8 );
			SetSliders ( textBox7 . Text );
		}
		private void textBox8_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 9;
			//			ResetDefaultFieldColors ( 9 );
			SetSliders ( textBox8 . Text );
		}
		private void textBox9_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 10;
			//			ResetDefaultFieldColors ( 10 );
			SetSliders ( textBox9 . Text );
		}
		private void textBox10_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 11;
			//			ResetDefaultFieldColors ( 11 );
			SetSliders ( textBox10 . Text );
		}
		private void textBox11_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 12;
			//			ResetDefaultFieldColors ( 12 );
			SetSliders ( textBox11 . Text );
		}

		private void textBox12_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 13;
			//			ResetDefaultFieldColors ( 13 );
			SetSliders ( textBox12 . Text );
		}
		private void textBox13_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 14;
			//			ResetDefaultFieldColors ( 14 );
			SetSliders ( textBox13 . Text );
		}
		private void textBox14_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 15;
			//			ResetDefaultFieldColors ( 15 );
			SetSliders ( textBox14 . Text );
		}
		private void textBox15_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 16;
			//			ResetDefaultFieldColors ( 16 );
			SetSliders ( textBox15 . Text );
		}
		private void textBox16_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 17;
			//			ResetDefaultFieldColors ( 17 );
			SetSliders ( textBox16 . Text );
		}
		private void textBox17_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 18;
			textBox17 . SelectionLength = 1;
		}
		private void textBox18_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 19;
			textBox18 . SelectionLength = 1;
		}
		private void textBox19_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 20;
			textBox19 . SelectionLength = 1;
		}
		private void textBox20_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 21;
			textBox20 . SelectionLength = 1;
		}
		private void fontlarge1_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 22;
			fontsizelarge1 . SelectionLength = 1;
		}
		private void fontlarge3_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 23;
			fontsizelarge3 . SelectionLength = 1;
		}
		private void fontsizesmall1_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 24;
			//			fontsizesmall1 . SelectionLength = 1;
		}
		private void fontsizesmall2_GotFocus ( object sender , RoutedEventArgs e )
		{
			ActiveField = 25;
			//			fontsizesmall2. SelectionLength = 1;
		}

		#endregion field focus

		#region Data Display / Debug Methods
		private void ListDps ( object sender , RoutedEventArgs e )
		{
			string output="";
			if ( IsLoaded == false )
				return;
			Console . WriteLine ( "In ListDps ..." );
			//output = GetValue ( Msgbox . DlgBackGroundProperty ) . ToString ( ) + "\n";
			//output += GetValue ( Msgbox . DlgForeGroundProperty ) . ToString ( ) + "\n";
			//output += GetValue ( Msgbox . BtnBackGroundProperty ) . ToString ( ) + "\n";
			//output += GetValue ( Msgbox . BtnForeGroundProperty ) . ToString ( ) + "\n";
			//output += GetValue ( Msgbox . MouseoverBackGroundProperty ) . ToString ( ) + "\n";
			//output += GetValue ( Msgbox . MouseoverForeGroundProperty ) . ToString ( ) + "\n";
			//output += GetValue ( Msgbox . BorderColorProperty ) . ToString ( ) + "\n";
			//info . Text = output;
			output = DlgInput . dlgbackground . ToString ( ) + "\n";
			output += DlgInput . dlgforeground . ToString ( ) + "\n";
			output += DlgInput . btnbackground . ToString ( ) + "\n";
			output += DlgInput . btnforeground . ToString ( ) + "\n";
			output += DlgInput . Btnmousebackground . ToString ( ) + "\n";
			output += DlgInput . Btnmouseforeground . ToString ( ) + "\n";
			output += DlgInput . Btnborder . ToString ( ) + "\n";
			output += DlgInput . defbtnbackground . ToString ( ) + "\n";
			output += DlgInput . defbtnforeground . ToString ( ) + "\n";
			//Memvars . Text = output;
		}
		#endregion Data Display Methods

		#region Load/Save config from disk
		// Reset text display and therefore color panel
		private void ReadDlgInput ( )
		{
			SolidColorBrush sb;
			if ( File . Exists ( @"Messageboxes.dat" ) == false )
			{
				//				Utils . Mbox ( this , string1: "Error - The Message Boxes configuration file is not available !" , string2: "Therefore the system will start with defaullt settings for all MessageBox colours" , caption: "Message Box initialization faillure" , iconstring: "\\icons\\red-cross-icon.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
				ResetDefaultFieldColors ( 1 );
			}
			else
			{

				string input = File . ReadAllText ( @"Messageboxes.dat" );
				string[] fields = input.Split('\n');
				if ( fields . Length < 13 )
				{

					Utils . Mbox ( this , string1: "Major error - The System message box configuration file is missing or corrupt !" , string2: "" , caption: "" , iconstring: "\\icons\\red-cross-icon.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
					//					MessageBox . Show ( $"Major error - The System message box configuration file is missing or corrupt !" );
					return;
				}
				int indx= 0;

				foreach ( var item in fields )
				{
					switch ( indx++ )
					{
						#region dialog box
						case 0:
							DlgInput . dlgbackground = Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . DlgBackGroundProperty , DlgInput . dlgbackground );
							textBox . Text = DlgInput . dlgbackground . BrushtoText ( );
							break;
						case 1:
							DlgInput . dlgforeground = Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . DlgForeGroundProperty , DlgInput . dlgforeground );
							textBox1 . Text = DlgInput . dlgforeground . BrushtoText ( );
							break;
						#endregion dialog box

						#region normal buttons
						// BUTTON COLORS
						case 2:
							DlgInput . btnbackground = Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . BtnBackGroundProperty , DlgInput . btnbackground );
							textBox2 . Text = DlgInput . btnbackground . BrushtoText ( );
							Button4 . Background = DlgInput . btnbackground;
							Button3 . Background = DlgInput . btnbackground;
							break;
						case 3:
							DlgInput . btnforeground = Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . BtnForeGroundProperty , DlgInput . btnforeground );
							textBox3 . Text = DlgInput . btnforeground . BrushtoText ( );
							Button4 . Foreground = DlgInput . btnforeground;
							Button3 . Foreground = DlgInput . btnforeground;
							break;
						case 4:     // standard Dialog border color
							DlgInput . Btnborder = Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . BorderColorProperty , DlgInput . Btnborder );
							textBox4 . Text = DlgInput . Btnborder . BrushtoText ( );
							break;

						// STD MOUSE OVER COLORS
						case 5:
							DlgInput . Btnmousebackground = Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . MouseoverBackGroundProperty , DlgInput . Btnmousebackground );
							textBox5 . Text = DlgInput . Btnmousebackground . BrushtoText ( );
							break;
						case 6:
							DlgInput . Btnmouseforeground = Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . Btnmouseforeground );
							textBox6 . Text = DlgInput . Btnmouseforeground . BrushtoText ( );
							break;

						// DEFAULT BUTTON COLORS
						case 7:
							DlgInput . defbtnbackground = Utils . GetNewBrush ( item );
							////SetCurrentValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . mouseforeground );
							textBox7 . Text = DlgInput . defbtnbackground . BrushtoText ( );
							break;
						case 8:
							DlgInput . defbtnforeground = Utils . GetNewBrush ( item );
							////SetCurrentValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . mouseforeground );
							textBox8 . Text = DlgInput . defbtnforeground . BrushtoText ( );
							break;
						#endregion normal buttons

						// BORDER THICKNESS - GLOBAL SETTING
						case 9:
							string[] flds = item.Split(',');
							Thickness th = new Thickness(Convert.ToInt32(flds[0]), Convert . ToInt32 ( flds [ 1 ] ) , Convert . ToInt32 ( flds [ 2 ] ) , Convert . ToInt32 ( flds [ 3 ] ) );
							textBox17 . Text = th . Left . ToString ( );
							textBox18 . Text = th . Top . ToString ( );
							textBox19 . Text = th . Right . ToString ( );
							textBox20 . Text = th . Bottom . ToString ( );
							DlgInput . BorderSizeNormal = th;
							//Bordersize = th;
							break;

						#region FLAGS
						// FLAG SETTINGS
						case 10:
							DlgInput . UseIcon = item == "T" ? true : false;
							break;
						case 11:
							DlgInput . isClean = item == "T" ? true : false;
							break;
						case 12:
							DlgInput . UseDarkMode = item . Contains ( "DMY" ) ? true : false;
							//darkmode . IsChecked = DlgInput . UseDarkMode;
							break;
						#endregion FLAGS

						//*******************************//
						// DARK MODE START
						//*******************************//
						#region Dark mode values
						case 13:    // dark mode NORMAL BUTTON SHADOW color
							DlgInput . BtnborderDark = Utils . GetNewBrush ( item );
							textBox9 . Text = DlgInput . BtnborderDark . BrushtoText ( );
							break;
						case 14:    // dark mode NORMAL BUTTON background color
							DlgInput . btnbackgroundDark = Utils . GetNewBrush ( item );
							textBox10 . Text = DlgInput . btnbackgroundDark . BrushtoText ( );
							break;
						case 15:    // dark mode NORMAL BUTTON foreground color
							DlgInput . btnforegroundDark = Utils . GetNewBrush ( item );
							textBox11 . Text = DlgInput . btnforegroundDark . BrushtoText ( );
							break;

						// NORMAL BUTTON COLORS
						case 16:    // dark mode MOUSEOVER SHADOW color
							DlgInput . mouseborderDark = Utils . GetNewBrush ( item );
							textBox12 . Text = DlgInput . mouseborderDark . BrushtoText ( );
							break;
						case 17:    // dark mode mouseover BACKGROUND color
							DlgInput . mousebackgroundDark = Utils . GetNewBrush ( item );
							textBox13 . Text = DlgInput . mousebackgroundDark . BrushtoText ( );
							break;
						case 18:    // dark mode mouseover FOREGROUND color
							DlgInput . mouseforegroundDark = Utils . GetNewBrush ( item );
							textBox14 . Text = DlgInput . mouseforegroundDark . BrushtoText ( );
							break;

						// DARK MODE : DEFAULT BUTTON COLORS
						case 19:    // dark mode DEFAULT BTN BACKGROUND color
							DlgInput . defbtnbackgroundDark = Utils . GetNewBrush ( item );
							textBox15 . Text = DlgInput . defbtnbackgroundDark . BrushtoText ( );
							break;
						case 20:    // dark mode DEFAULT BUTTON FOREGROUND color
							DlgInput . defbtnforegroundDark = Utils . GetNewBrush ( item );
							textBox16 . Text = DlgInput . defbtnforegroundDark . BrushtoText ( );
							break;
						case 21:       // Def buttons border size
							if ( item == "" )
								th = new Thickness ( Convert . ToInt32 ( 1 ) , Convert . ToInt32 ( 4 ) , Convert . ToInt32 ( 1 ) , Convert . ToInt32 ( 1 ) );
							else
							{
								flds = item . Split ( ',' );
								th = new Thickness ( Convert . ToInt32 ( flds [ 0 ] ) , Convert . ToInt32 ( flds [ 1 ] ) , Convert . ToInt32 ( flds [ 2 ] ) , Convert . ToInt32 ( flds [ 3 ] ) );
							}
							DlgInput . BorderSizeDefault = th;
							//						MouseoverBordersize = th;
							break;

						case 22:       // small dialog  row 1  text
							if ( item == "" )
								fontsizesmall1 . Text = "13;";
							else
								fontsizesmall1 . Text = item . Trim ( );
							img1MainText1 . FontSize = Convert . ToDouble ( item );
							break;
						case 23:       // small dialog  row 2  text
							if ( item == "" )
								fontsizesmall2 . Text = "13;";
							else
								fontsizesmall2 . Text = item . Trim ( );
							img1MainText2 . FontSize = Convert . ToDouble ( item );
							break;
						case 24:       // large dialog  row 1  text
							if ( item == "" )
								fontsizelarge1 . Text = "13;";
							else
								fontsizelarge1 . Text = item . Trim ( );
								img2MainText1 . FontSize = Convert . ToDouble ( item );
								img2MainText2. FontSize = Convert . ToDouble ( item );
							break;
						case 25:       // large dialog  row 3  text
							if ( item == "" )
								fontsizelarge3 . Text = "13;";
							else
								fontsizelarge3 . Text = item . Trim ( );
							img2MainText3 . FontSize = Convert . ToDouble ( item );
							break;

							#endregion Dark mode values
					}
				}
			}

			string dm = DlgInput . UseDarkMode == true ? "Yes" : "No";

			// All correct 8/1/22

			Console . WriteLine ( $"MsgBox  configuraton : Data read in from disk ....\n"
				+ "DlgInput Data\n"
				+ $"Dlg background :			[{DlgInput . dlgbackground}]\n"
				+ $"Dlg foreground :			[{DlgInput . dlgforeground}]\n"
				+ $"Btn Background :			[{DlgInput . btnbackground}]\n"
				+ $"Btn Foreground :			[{DlgInput . btnforeground}]\n"
				+ $"Btn Border :				[{DlgInput . Btnborder}]\n"
				+ $"Btn Mouseover Background :	[{DlgInput . Btnmousebackground}]\n"
				+ $"Btn Mouseover Foreground :	[{DlgInput . Btnmouseforeground}]\n"
				+ $"Btn DefBackground :			[{DlgInput . defbtnbackground}]\n"
				+ $"Btn DefForeground :			[{DlgInput . defbtnforeground}]\n"
				+ $"Btn Border Size :			[{DlgInput . BorderSizeNormal . Top}, {DlgInput . BorderSizeNormal . Left},{DlgInput . BorderSizeNormal . Right},{DlgInput . BorderSizeNormal . Bottom}],\n"
				+ "\n"
				+ $"Use Dark mode:				[{dm}]\n"
				+ $"Btn Border :				[{DlgInput . BtnborderDark}]\n"
				+ $"Btn Background :			[{DlgInput . btnbackgroundDark}]\n"
				+ $"Btn Foreground :			[{DlgInput . btnforegroundDark}]\n"
				+ $"Btn Border :				[{DlgInput . mouseborderDark}]\n"
				+ $"Btn Mouseover Background :	[{DlgInput . mousebackgroundDark}]\n"
				+ $"Btn Mouseover Foreground :	[{DlgInput . mouseforegroundDark}]\n"
				+ $"Def Btn Background :		[{DlgInput . defbtnbackgroundDark}]\n"
				+ $"Def Btn Foreground :		[{DlgInput . defbtnforegroundDark}]\n"
				+ $"Dlg text  sizes    :			[{fontsizesmall1 . Text + ", " + fontsizesmall2 . Text + ",\n" + fontsizelarge1 . Text + ", " + fontsizelarge3 . Text }]\n"
				);
			textBox . Focus ( );
			//ListDps ( this , null );
		}
		private void SaveButtonColors ( )
		{
			// Save colors to both types of MessageBoxes

			Brush brush;
			bool b = false;
			string output="";

			if ( IsLoaded == false )
				return;
			Console . WriteLine ( "In SaveButtonColors ..." );

			#region Dialog

			DlgInput . dlgbackground = color1 . Background as Brush;
			output += color1 . Background . ToString ( ) + "\n";

			DlgInput . dlgforeground = color2 . Background as Brush;
			output += color2 . Background . ToString ( ) + "\n";
			#endregion Dialog

			#region Normal mode
			DlgInput . btnbackground = color3 . Background as Brush;
			output += color3 . Background . ToString ( ) + "\n";

			DlgInput . btnforeground = color4 . Background as Brush;
			output += color4 . Background . ToString ( ) + "\n";

			DlgInput . Btnborder = color5 . Background as Brush;
			output += color5 . Background . ToString ( ) + "\n";

			DlgInput . Btnmousebackground = color6 . Background as Brush;
			output += color6 . Background . ToString ( ) + "\n";

			DlgInput . Btnmouseforeground = color7 . Background as Brush;
			output += color7 . Background . ToString ( ) + "\n";

			DlgInput . defbtnbackground = color8 . Background as Brush;
			output += color8 . Background . ToString ( ) + "\n";

			DlgInput . defbtnforeground = color9 . Background as Brush;
			output += color9 . Background . ToString ( ) + "\n";

			output += textBox17 . Text + "," + textBox18 . Text + "," + textBox19 . Text + "," + textBox20 . Text + "\n";

			#region flags
			b = DlgInput . UseIcon;
			output += ( b == true ? "T" : "F" ) + "\n";
			b = DlgInput . isClean;
			output += ( b == true ? "T" : "F" ) + "\n";
			if ( DlgInput . UseDarkMode == true )
				output += "DMY\n";
			else
				output += "DMN\n";

			#endregion flags

			#endregion Normal mode

			#region Dark mode
			// DARK MODE SETTINGS
			DlgInput . BtnborderDark = color10?.Background as Brush;
			output += color10 . Background?.ToString ( ) + "\n";

			DlgInput . btnbackgroundDark = color11 . Background as Brush;
			output += color11 . Background?.ToString ( ) + "\n";

			DlgInput . btnforegroundDark = color12 . Background as Brush;
			output += color12 . Background?.ToString ( ) + "\n";

			DlgInput . mouseborderDark = color13?.Background as Brush;
			output += color13 . Background?.ToString ( ) + "\n";

			DlgInput . mousebackgroundDark = color14 . Background as Brush;
			output += color14 . Background?.ToString ( ) + "\n";

			DlgInput . mouseforegroundDark = color15 . Background as Brush;
			output += color15 . Background?.ToString ( ) + "\n";

			DlgInput . defbtnbackgroundDark = color16 . Background as Brush;
			output += color16 . Background?.ToString ( ) + "\n";

			DlgInput . defbtnforegroundDark = color17 . Background as Brush;
			output += color17 . Background?.ToString ( ) + "\n";

			output += DlgInput . BorderSizeDefault + "\n";
			// Text size of dialog text 
			output += fontsizesmall1 . Text + "\n";
			output += fontsizesmall2 . Text + "\n";
			output += fontsizelarge1 . Text + "\n";
			output += fontsizelarge3 . Text + "\n";

			#endregion Dark mode

			// Save to disk file
			File . WriteAllText ( @"Messageboxes.dat" , output );

			Console . WriteLine ( $"Data written out to disk ....\n"
				+ $"Dlg background :			[{DlgInput . dlgbackground}]\n"
				+ $"Dlg foreground :			[{DlgInput . dlgforeground}]\n"
				+ $"Btn Background :			[{DlgInput . btnbackground}]\n"
				+ $"Btn Foreground :			[{DlgInput . btnforeground}]\n"
				+ $"Btn Border :				[{DlgInput . Btnborder}]\n"
				+ $"Btn Mouseover Background :	[{DlgInput . Btnmousebackground}]\n"
				+ $"Btn Mouseover Foreground :	[{DlgInput . Btnmouseforeground}]\n"
				+ $"Btn DefBackground :			[{DlgInput . defbtnbackground}]\n"
				+ $"Btn DefForeground :			[{DlgInput . defbtnforeground}]\n"
				+ $"Btn Border Size :			[{DlgInput . BorderSizeNormal . ToString ( )}],\n\n"
				+ $"Dark Mode		 :			[{DlgInput . UseDarkMode}]\n\n"
				+ $"Btn Border :				[{DlgInput . BtnborderDark}]\n"
				+ $"Btn Background :			[{DlgInput . btnbackgroundDark}]\n"
				+ $"Btn Foreground :			[{DlgInput . btnforegroundDark}]\n"
				+ $"Btn Border :				[{DlgInput . mouseborderDark}]\n"
				+ $"Btn Mouseover Background :	[{DlgInput . mousebackgroundDark}]\n"
				+ $"Btn Mouseover Foreground :	[{DlgInput . mouseforegroundDark}]\n"
				+ $"Def Btn Background :		[{DlgInput . defbtnbackgroundDark}]\n"
				+ $"Def Btn Foreground :		[{DlgInput . defbtnforegroundDark}]\n"
				+ $"Def Btn Border Size :			[{DlgInput . BorderSizeDefault . ToString ( )}]\n\n"
		);
		}
		private void RefreshDlgTextFields ( Brush color )
		{
			img1MainText1 . Foreground = color;
			img1MainText2 . Foreground = color;
			img2MainText1 . Foreground = color;
			img2MainText2 . Foreground = color;
			img2MainText3 . Foreground = color;
		}

		#endregion Load/Save config from disk

		#region Visual Quality methods

		private void darkmode_Click ( object sender , RoutedEventArgs e )
		{
			if ( IsLoaded == false )
				return;
			Console . WriteLine ( "In darkmode_Click ..." );

			if ( DlgInput . UseDarkMode == false )
			{
				DlgInput . UseDarkMode = true;
//				darkmode . Content = "  Dark Mode --> Click for Normal Mode settings";
				Image1Border . Background = FindResource ( "Black1" ) as SolidColorBrush;
				Image2Border . Background = FindResource ( "Black1" ) as SolidColorBrush;
				img1MainText1 . Background = FindResource ( "Black1" ) as SolidColorBrush;
				img1MainText2 . Background = FindResource ( "Black1" ) as SolidColorBrush;
				img2MainText1 . Background = FindResource ( "Black1" ) as SolidColorBrush;
				img2MainText2 . Background = FindResource ( "Black1" ) as SolidColorBrush;
				img2MainText3 . Background = FindResource ( "Black1" ) as SolidColorBrush;

				img1MainText1 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				img1MainText2 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				img2MainText1 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				img2MainText2 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				img2MainText3 . Foreground = FindResource ( "White0" ) as SolidColorBrush;

				BorderColor = DlgInput . BtnborderDark;
				Button1Text . Foreground = DlgInput . defbtnforegroundDark;

				//default btn
				Button1 . Background = DlgInput . defbtnbackgroundDark;
				Button3 . Background = DlgInput . defbtnbackgroundDark;
				// Normal btns
				Button23 . Background = DlgInput . btnbackgroundDark;
				Button2 . Background = DlgInput . btnbackgroundDark;
				Button22 . Background = DlgInput . btnbackgroundDark;
				Button2Text . Foreground = DlgInput . btnforegroundDark;
				Button4 . Background = DlgInput . btnbackgroundDark;
				Button4 . Foreground = DlgInput . btnforegroundDark;
				Button3 . Foreground = DlgInput . btnforegroundDark;

				//default btn
				Button1 . BorderBrush = DlgInput . BtnborderDark;
				Button22 . BorderBrush = DlgInput . BtnborderDark;
				Button23 . BorderBrush = DlgInput . BtnborderDark;
				Button2 . BorderBrush = DlgInput . BtnborderDark;
				Button4 . BorderBrush = DlgInput . BtnborderDark;
				Button3 . BorderBrush = DlgInput . BtnborderDark;
				Button4 . BorderBrush = DlgInput . BtnborderDark;
				Button3 . BorderBrush = DlgInput . BtnborderDark;
				ToggleMode ( );
				RefreshAllObjects ( );
			}
			else
			{
				DlgInput . UseDarkMode = false;
//				darkmode . Content = "  Normal Mode --> Click for Dark Mode settings";
				Image1Border . Background = DlgInput . dlgbackground;
				Image2Border . Background = DlgInput . dlgbackground;
				img1MainText1 . Background = DlgInput . dlgbackground;
				img1MainText2 . Background = DlgInput . dlgbackground;
				img2MainText1 . Background = DlgInput . dlgbackground;
				img2MainText2 . Background = DlgInput . dlgbackground;
				img2MainText3 . Background = DlgInput . dlgbackground;

				img1MainText1 . Foreground = DlgInput . dlgforeground;
				img1MainText2 . Foreground = DlgInput . dlgforeground;
				img2MainText1 . Foreground = DlgInput . dlgforeground;
				img2MainText2 . Foreground = DlgInput . dlgforeground;
				img2MainText3 . Foreground = DlgInput . dlgforeground;
				BorderColor = DlgInput . Btnborder;
				Button1Text . Foreground = DlgInput . btnforeground;
				// Large Dlg
				Button23 . Background = DlgInput . btnbackground;
				Button2 . Background = DlgInput . btnbackground;
				//default btn
				Button1 . Background = DlgInput . defbtnbackground;
				Button1Text . Foreground = DlgInput . defbtnforeground;
				Button22 . Background = DlgInput . btnbackground;

				Button2Text . Foreground = DlgInput . btnforeground;
				Button22Text . Foreground = DlgInput . btnforeground;
				Button23Text . Foreground = DlgInput . btnforeground;
				Button4 . Background = DlgInput . btnbackground;
				Button4 . Foreground = DlgInput . btnforeground;
				//default btn
				Button3 . Background = DlgInput . defbtnbackground;
				Button3 . Foreground = DlgInput . defbtnforeground;

				//default btn
				Button1 . BorderBrush = DlgInput . Btnborder;
				Button22 . BorderBrush = DlgInput . Btnborder;
				Button23 . BorderBrush = DlgInput . Btnborder;
				Button2 . BorderBrush = DlgInput . Btnborder;
				Button4 . BorderBrush = DlgInput . Btnborder;
				Button3 . BorderBrush = DlgInput . Btnborder;
				Button4 . BorderBrush = DlgInput . Btnborder;
				Button3 . BorderBrush = DlgInput . Btnborder;
				ToggleMode ( );
				RefreshAllObjects ( );
			}
		}

		// Handle enable/disable various fieds to match edit mode we are in
		private void ToggleMode ( )
		{
			if ( DlgInput . UseDarkMode == true )
			{
				//-------------------------------------------//
				// SWITCHING TO DARK MODE
				// Hide All Normal  setup data
				//-------------------------------------------//

				// NORMAL STUFF - LABELS
				// Dim Normal labels
				label . Opacity = 0.1;
				label0 . Opacity = 0.1;
				label1 . Opacity = 0.1;
				label2 . Opacity = 0.1;
				label3 . Opacity = 0.1;
				label4 . Opacity = 0.1;
				label5 . Opacity = 0.1;
				label6 . Opacity = 0.1;
				label7 . Opacity = 0.1;
				label8 . Opacity = 0.1;

				// NORMAL STUFF - TEXT FIELDS
				// disable Normal text fields
				textBox . IsEnabled = false;
				textBox1 . IsEnabled = false;
				textBox2 . IsEnabled = false;
				textBox3 . IsEnabled = false;
				textBox4 . IsEnabled = false;
				textBox5 . IsEnabled = false;
				textBox6 . IsEnabled = false;
				textBox7 . IsEnabled = false;
				textBox8 . IsEnabled = false;
				// NORMAL STUFF
				// Dim Normal text fields
				textBox . Opacity = 0.2;
				textBox1 . Opacity = 0.2;
				textBox2 . Opacity = 0.2;
				textBox3 . Opacity = 0.2;
				textBox4 . Opacity = 0.2;
				textBox5 . Opacity = 0.2;
				textBox6 . Opacity = 0.2;
				textBox7 . Opacity = 0.2;
				textBox8 . Opacity = 0.2;

				// NORMAL STUFF - COLOR BOXES
				// Disable Normal  Color boxes
				color1 . IsEnabled = false;
				color2 . IsEnabled = false;
				color3 . IsEnabled = false;
				color4 . IsEnabled = false;
				color5 . IsEnabled = false;
				color6 . IsEnabled = false;
				color7 . IsEnabled = false;
				color8 . IsEnabled = false;
				color9 . IsEnabled = false;

				// NORMAL STUFF - COLOR BOXES
				// Dim Nermal Color boxes
				color1 . Opacity = 0.2;
				color2 . Opacity = 0.2;
				color3 . Opacity = 0.2;
				color4 . Opacity = 0.2;
				color5 . Opacity = 0.2;
				color6 . Opacity = 0.2;
				color7 . Opacity = 0.2;
				color8 . Opacity = 0.2;
				color9 . Opacity = 0.2;

				//=====================================================================//
				//-------------------------------------------//
				// SWITCHING TO DARK MODE
				// Show All DM setup data
				//-------------------------------------------//

				// DARK MODE  STUFF
				label01 . Opacity = 1.0;
				label9 . Opacity = 1.0;
				label10 . Opacity = 1.0;
				label11 . Opacity = 1.0;
				label12 . Opacity = 1.0;
				label13 . Opacity = 1.0;
				label14 . Opacity = 1.0;
				label15 . Opacity = 1.0;
				label16 . Opacity = 1.0;

				// DARK MODE STUFF
				textBox9 . IsEnabled = true;
				textBox10 . IsEnabled = true;
				textBox11 . IsEnabled = true;
				textBox12 . IsEnabled = true;
				textBox13 . IsEnabled = true;
				textBox14 . IsEnabled = true;
				textBox15 . IsEnabled = true;
				textBox16 . IsEnabled = true;

				// Show  DM color text fields
				textBox9 . Opacity = 1.0;
				textBox10 . Opacity = 1.0;
				textBox11 . Opacity = 1.0;
				textBox12 . Opacity = 1.0;
				textBox13 . Opacity = 1.0;
				textBox14 . Opacity = 1.0;
				textBox15 . Opacity = 1.0;
				textBox16 . Opacity = 1.0;

				// Show DM Color panels
				color10 . Opacity = 1.0;
				color11 . Opacity = 1.0;
				color12 . Opacity = 1.0;
				color13 . Opacity = 1.0;
				color14 . Opacity = 1.0;
				color15 . Opacity = 1.0;
				color16 . Opacity = 1.0;
				color17 . Opacity = 1.0;
				//darkmode . Content = " -> Click for Normal Mode settings";
				darkmode . Opacity = 0.2;
				darkmode . IsEnabled = false;
				normalmode . Opacity = 1.0;
				normalmode . IsEnabled = true;
				textBox9 . Focus ( );

				//=====================================================================//
			}
			else
			{
				//-------------------------------------------//
				// SWITCHING TO NORMAL
				//-------------------------------------------//

				//-------------------------------------------//
				// HIDE DM AREA 1ST
				//-------------------------------------------//

				// Hide  DM Labels
				label . Opacity = 0.2;
				label0 . Opacity = 0.2;
				label01 . Opacity = 0.2;
				label9 . Opacity = 0.2;
				label10 . Opacity = 0.2;
				label11 . Opacity = 0.2;
				label12 . Opacity = 0.2;
				label13 . Opacity = 0.2;
				label14 . Opacity = 0.2;
				label15 . Opacity = 0.2;
				label16 . Opacity = 0.2;

				// Disable DM Color text fields
				textBox9 . IsEnabled = false;
				textBox10 . IsEnabled = false;
				textBox11 . IsEnabled = false;
				textBox12 . IsEnabled = false;
				textBox13 . IsEnabled = false;
				textBox14 . IsEnabled = false;
				textBox15 . IsEnabled = false;
				textBox16 . IsEnabled = false;
				// Dim  DM color text fields
				textBox9 . Opacity = 0.2;
				textBox10 . Opacity = 0.2;
				textBox11 . Opacity = 0.2;
				textBox12 . Opacity = 0.2;
				textBox13 . Opacity = 0.2;
				textBox14 . Opacity = 0.2;
				textBox15 . Opacity = 0.2;
				textBox16 . Opacity = 0.2;

				// Disable DM color boxes
				color10 . IsEnabled = false;
				color12 . IsEnabled = false;
				color13 . IsEnabled = false;
				color14 . IsEnabled = false;
				color15 . IsEnabled = false;
				color16 . IsEnabled = false;
				color17 . IsEnabled = false;
				// Dim DM color fields
				color10 . Opacity = 0.2;
				color11 . Opacity = 0.2;
				color12 . Opacity = 0.2;
				color13 . Opacity = 0.2;
				color14 . Opacity = 0.2;
				color15 . Opacity = 0.2;
				color16 . Opacity = 0.2;
				color17 . Opacity = 0.2;

				//-------------------------------------------//
				// THEN SHOW NORMAL AREA
				//-------------------------------------------//

				// Show Normal labels
				label1 . Opacity = 1.0;
				label2 . Opacity = 1.0;
				label3 . Opacity = 1.0;
				label4 . Opacity = 1.0;
				label5 . Opacity = 1.0;
				label6 . Opacity = 1.0;
				label7 . Opacity = 1.0;
				label8 . Opacity = 1.0;

				// Show Normal labels
				label . Opacity = 1.0;
				label0 . Opacity = 1.0;
				label1 . Opacity = 1.0;
				label2 . Opacity = 1.0;
				label3 . Opacity = 1.0;
				label4 . Opacity = 1.0;
				label5 . Opacity = 1.0;
				label6 . Opacity = 1.0;
				label7 . Opacity = 1.0;
				label8 . Opacity = 1.0;

				// Enable Normal text fields
				textBox . IsEnabled = true;
				textBox1 . IsEnabled = true;
				textBox2 . IsEnabled = true;
				textBox3 . IsEnabled = true;
				textBox4 . IsEnabled = true;
				textBox5 . IsEnabled = true;
				textBox6 . IsEnabled = true;
				textBox7 . IsEnabled = true;
				textBox8 . IsEnabled = true;

				// Show Normal text fields
				textBox . Opacity = 1.0;
				textBox1 . Opacity = 1.0;
				textBox2 . Opacity = 1.0;
				textBox3 . Opacity = 1.0;
				textBox4 . Opacity = 1.0;
				textBox5 . Opacity = 1.0;
				textBox6 . Opacity = 1.0;
				textBox7 . Opacity = 1.0;
				textBox8 . Opacity = 1.0;


				color1 . IsEnabled = true;
				color2 . IsEnabled = true;
				color3 . IsEnabled = true;
				color4 . IsEnabled = true;
				color5 . IsEnabled = true;
				color6 . IsEnabled = true;
				color7 . IsEnabled = true;
				color8 . IsEnabled = true;
				color9 . IsEnabled = true;

				color1 . Opacity = 1.0;
				color2 . Opacity = 1.0;
				color3 . Opacity = 1.0;
				color4 . Opacity = 1.0;
				color5 . Opacity = 1.0;
				color6 . Opacity = 1.0;
				color7 . Opacity = 1.0;
				color8 . Opacity = 1.0;
				color9 . Opacity = 1.0;

				normalmode . Opacity = 0.2;
				normalmode . IsEnabled = false;
				darkmode . Opacity = 1.0;
				darkmode . IsEnabled = true;
				//darkmode . Content = " -> Click for Dark Mode settings";
				textBox . Focus ( );
			}
		}
		private void UpdateButtonShadowSize ( )
		{
			Button1 . BorderThickness = DlgInput . BorderSizeNormal;
			Button2 . BorderThickness = DlgInput . BorderSizeNormal;
			Button3 . BorderThickness = DlgInput . BorderSizeNormal;
			Button4 . BorderThickness = DlgInput . BorderSizeNormal;
			Button22 . BorderThickness = DlgInput . BorderSizeNormal;
			Button23 . BorderThickness = DlgInput . BorderSizeNormal;

			Button1 . UpdateLayout ( );
			Button2 . UpdateLayout ( );
			Button3 . UpdateLayout ( );
			Button4 . UpdateLayout ( );
			Button22 . UpdateLayout ( );
			Button23 . UpdateLayout ( );
		}
		#endregion Visual Quality methods

		#region border shadow size change text fields

		private void textBox17_TextChanged ( object sender , TextChangedEventArgs e )
		{
			if ( textBox17 . Text == "" || CheckValid ( textBox17 . Text ) == false )
				return;
			if ( Convert . ToInt32 ( textBox17 . Text ) > 9 )
			{
				textBox17 . Text = textBox17 . Text . Substring ( 0 , 1 );
			}
			ActiveField = 18;
			ResSetColorSelectionDisplay ( 0 , "" );
			textBox17 . SelectAll ( );
			textBox17 . UpdateLayout ( );
		}
		private void textBox18_TextChanged ( object sender , TextChangedEventArgs e )
		{
			if ( textBox18 . Text == "" || CheckValid ( textBox18 . Text ) == false )
				return;
			if ( Convert . ToInt32 ( textBox18 . Text ) > 9 )
			{
				textBox18 . Text = textBox18 . Text . Substring ( 0 , 1 );
			}
			ActiveField = 19;
			ResSetColorSelectionDisplay ( 0 , "" );
			textBox18 . SelectAll ( );
			textBox18 . UpdateLayout ( );
		}
		private void textBox19_TextChanged ( object sender , TextChangedEventArgs e )
		{
			if ( textBox19 . Text == "" || CheckValid ( textBox19 . Text ) == false )
				return;
			if ( Convert . ToInt32 ( textBox19 . Text ) > 9 )
			{
				textBox19 . Text = textBox19 . Text . Substring ( 0 , 1 );
			}
			ActiveField = 20;
			ResSetColorSelectionDisplay ( 0 , "" );
			//			textBox19 . SelectionLength = textBox19 . Text.Length;
			textBox19 . SelectAll ( );
			textBox19 . UpdateLayout ( );
		}
		private void textBox20_TextChanged ( object sender , TextChangedEventArgs e )
		{
			if ( textBox20 . Text == "" || CheckValid ( textBox20 . Text ) == false )
				return;
			if ( Convert . ToInt32 ( textBox20 . Text ) > 9 )
			{
				textBox20 . Text = textBox20 . Text . Substring ( 0 , 1 );
			}
			ActiveField = 21;
			ResSetColorSelectionDisplay ( 0 , "" );
			textBox20 . SelectAll ( );
			textBox20 . UpdateLayout ( );
		}

		#endregion border shadow text fields

		#region unused TextChanged methods
		private void fontlarge1_TextChanged ( object sender , TextChangedEventArgs e )
		{
		}
		private void fontlarge2_TextChanged ( object sender , TextChangedEventArgs e )
		{
		}
		private void fontlarge3_TextChanged ( object sender , TextChangedEventArgs e )
		{
		}
		private void fontsizesmall1_TextChanged ( object sender , TextChangedEventArgs e )
		{
		}
		private void fontsizesmall2_TextChanged ( object sender , TextChangedEventArgs e )
		{
		}
		#endregion unused TextChanged methods


		#region Screen Refresh method
		//*********************************************//
		// Called every time a color is changed
		//*********************************************//
		private void ResSetColorSelectionDisplay ( double value , string caller )
		{
			Thickness thickness = new Thickness();
			;
			if ( IsLoaded == false )
				return;
			//			Console . WriteLine ( "In ResSetColorSelectionDisplay  ..." );
			switch ( ActiveField )
			{
				case 1:          // Dialog background 
					textBox . Text = CalculateNewColor ( value , caller );
					color1 . UpdateLayout ( );
					DlgInput . dlgbackground = color1 . Background;
					if ( DlgInput . UseDarkMode == false )
					{
						Image1Border . Background = color1 . Background;
						Image2Border . Background = color1 . Background;
						img1MainText1 . Background = color1 . Background;
						img1MainText2 . Background = color1 . Background;
						img2MainText1 . Background = color1 . Background;
						img2MainText2 . Background = color1 . Background;
						img2MainText3 . Background = color1 . Background;
					}
					break;
				case 2:          // Dialog foreground 
					textBox1 . Text = CalculateNewColor ( value , caller );
					textBox1 . Refresh ( );
					color2 . UpdateLayout ( );
					//					img1CaptionRow . Foreground = color2 . Background;
					//					img2CaptionRow . Foreground = color2 . Background;
					DlgInput . dlgforeground = color2 . Background;
					if ( DlgInput . UseDarkMode == false )
						RefreshDlgTextFields ( color2 . Background );
					this . UpdateLayout ( );
					break;

				//-----------------------------//
				// Std Button colors
				//-----------------------------//

				case 3:          // std dlg BUTTON background 
					textBox2 . Text = CalculateNewColor ( value , caller );
					color3 . UpdateLayout ( );
					DlgInput . btnbackground = color3 . Background;
					if ( DlgInput . UseDarkMode == false )
					{
						Button2 . Background = color3 . Background;
						Button4 . Background = color3 . Background;
						Button22 . Background = color3 . Background;
						Button23 . Background = color3 . Background;
					}
					break;
				case 4:          // std dlg BUTTON foreground 
					textBox3 . Text = CalculateNewColor ( value , caller );
					color4 . UpdateLayout ( );
					DlgInput . btnforeground = color4 . Background;
					if ( DlgInput . UseDarkMode == false )
					{
						Button4 . Foreground = color4 . Background;
						Button2Text . Foreground = color4 . Background;
						Button22Text . Foreground = color4 . Background;
						Button23Text . Foreground = color4 . Background;
					}
					break;
				case 5:          // std BUTTON  : SHADOW
					textBox4 . Text = CalculateNewColor ( value , caller );
					color5 . UpdateLayout ( );
					DlgInput . Btnborder = color5 . Background;
					if ( DlgInput . UseDarkMode == false )
					{
						Button1 . BorderThickness = DlgInput . BorderSizeNormal;
						Button1 . BorderBrush = DlgInput . Btnborder;
						Button3 . BorderThickness = DlgInput . BorderSizeNormal;
						Button3 . BorderBrush = DlgInput . Btnborder;

						Button2 . BorderThickness = DlgInput . BorderSizeNormal;
						Button2 . BorderBrush = DlgInput . Btnborder;
						Button4 . BorderThickness = DlgInput . BorderSizeNormal;
						Button4 . BorderBrush = DlgInput . Btnborder;
						Button22 . BorderThickness = DlgInput . BorderSizeNormal;
						Button22 . BorderBrush = DlgInput . Btnborder;
						Button23 . BorderThickness = DlgInput . BorderSizeNormal;
						Button23 . BorderBrush = DlgInput . Btnborder;
					}
					break;

				//---------------------------------//
				// Std Mouse over settings
				//---------------------------------//
				case 6:          // std dlg mouseover BUTTON background 
					textBox5 . Text = CalculateNewColor ( value , caller );
					color6 . UpdateLayout ( );
					DlgInput . Btnmousebackground = color6 . Background;
					break;
				case 7:          // std dlg mouseover BUTTON foreground 
					textBox6 . Text = CalculateNewColor ( value , caller );
					color7 . UpdateLayout ( );
					DlgInput . defbtnforeground = color7 . Background;
					break;

				//----------------------------------------------------//
				// Normal DEFAULT BUTTON COLORS
				//---------------------------------------------------//
				case 8:          // dlg DEFAULT BUTTON background 
					textBox7 . Text = CalculateNewColor ( value , caller );
					color8 . UpdateLayout ( );
					DlgInput . defbtnbackground = color8 . Background;
					if ( DlgInput . UseDarkMode == false )
					{
						Button1 . Background = DlgInput . defbtnbackground;
						Button3 . Background = DlgInput . defbtnbackground;
					}

					break;
				case 9:     // DEF btn Foreground 
					textBox8 . Text = CalculateNewColor ( value , caller );
					color9 . UpdateLayout ( );
					DlgInput . defbtnforeground = color9 . Background;
					if ( DlgInput . UseDarkMode == false )
					{
						Button1Text . Foreground = DlgInput . defbtnforeground;
						Button3 . Foreground = DlgInput . defbtnforeground;
					}
					break;

				//--------------------------------------------------------------------------------------------------------------------------------------------------------------------//
				// DARK MODE START
				//--------------------------------------------------------------------------------------------------------------------------------------------------------------------//
				case 10:         // BUTTON Dark mode  normal button Shadow
					textBox9 . Text = CalculateNewColor ( value , caller );
					color10 . UpdateLayout ( );
					DlgInput . BtnborderDark = color10 . Background;
					if ( DlgInput . UseDarkMode == true )
					{
						Button1 . BorderBrush = color10 . Background;
						Button3 . BorderBrush = color10 . Background;
						Button2 . BorderBrush = color10 . Background;
						Button4 . BorderBrush = color10 . Background;
						Button22 . BorderBrush = color10 . Background;
						Button23 . BorderBrush = color10 . Background;
					}

					break;

				case 11:        // DARKMODE BUTTON background
					textBox10 . Text = CalculateNewColor ( value , caller );
					color11 . UpdateLayout ( );
					DlgInput . btnbackgroundDark = color11 . Background;
					if ( DlgInput . UseDarkMode == true )
					{
						Button2 . Background = color11 . Background;
						Button4 . Background = color11 . Background;
						Button22 . Background = color11 . Background;
						Button23 . Background = color11 . Background;
					}
					break;
				case 12:        // DARKMODE BUTTON foreground
					textBox11 . Text = CalculateNewColor ( value , caller );
					color12 . UpdateLayout ( );
					DlgInput . btnforegroundDark = color12 . Background;
					if ( DlgInput . UseDarkMode == true )
					{
						Button2Text . Foreground = color12 . Background;
						Button4 . Foreground = color12 . Background;
						Button22Text . Foreground = color12 . Background;
						Button23Text . Foreground = color12 . Background;
					}
					break;

				//------------------------------//
				// Dark mode mouseover
				//------------------------------//

				case 13:        // DARKMODE BUTTON 	-   Normal Shadow
					textBox12 . Text = CalculateNewColor ( value , caller );
					color13 . UpdateLayout ( );
					DlgInput . BtnborderDark = color13 . Background;
					if ( DlgInput . UseDarkMode == true )
					{
						//Button1 . BorderBrush = color13 . Background;
						//Button3 . BorderBrush = color13 . Background;
						//Button2 . BorderBrush = color13 . Background;
						//Button4 . BorderBrush = color13 . Background;
						//Button22 . BorderBrush = color13 . Background;
						//Button23 . BorderBrush = color13 . Background;
					}
					break;
				case 14:        // DARKMODE BUTTON  	-  mouseover background
					textBox13 . Text = CalculateNewColor ( value , caller );
					color14 . UpdateLayout ( );
					DlgInput . mousebackgroundDark = color14 . Background;
					break;
				case 15:        // DARKMODE BUTTON  	-  mouseover foreground
					textBox14 . Text = CalculateNewColor ( value , caller );
					color15 . UpdateLayout ( );
					DlgInput . mouseforegroundDark = color15 . Background;
					break;

				//-------------------------------------------------//
				// Dark mode - Default button  colors
				//-------------------------------------------------//

				case 16:        // DARKMODE BUTTON  	- Default background
					textBox15 . Text = CalculateNewColor ( value , caller );
					color16 . UpdateLayout ( );
					DlgInput . defbtnbackgroundDark = color16 . Background;
					Button1 . Background = DlgInput . defbtnbackgroundDark;
					Button3 . Background = DlgInput . defbtnbackgroundDark;
					break;
				case 17:        // DARKMODE BUTTON  	-  Default foreground
					textBox16 . Text = CalculateNewColor ( value , caller );
					color17 . UpdateLayout ( );
					DlgInput . defbtnforegroundDark = color17 . Background;
					Button1Text . Foreground = DlgInput . defbtnforegroundDark;
					Button3 . Foreground = DlgInput . defbtnforegroundDark;
					break;

				// Normal Shadow size fields
				case 18:        // Border Left value
					if ( textBox17 . Text != "" && CheckValid ( textBox17 . Text ) )
					{
						thickness . Left = Convert . ToDouble ( textBox17 . Text );
						DlgInput . BorderSizeNormal . Left = thickness . Left;
						UpdateButtonShadowSize ( );
					}
					break;
				case 19:        // Border top value
					if ( textBox18 . Text != "" && CheckValid ( textBox18 . Text ) )
					{
						thickness . Top = Convert . ToDouble ( textBox18 . Text );
						DlgInput . BorderSizeNormal . Top = thickness . Top;
						UpdateButtonShadowSize ( );
					}
					break;
				case 20:        // Border rightt value
					if ( textBox19 . Text != "" )
					{
						thickness . Right = Convert . ToDouble ( textBox19 . Text );
						DlgInput . BorderSizeNormal . Right = thickness . Right;
						UpdateButtonShadowSize ( );
					}
					break;
				case 21:        // Border Bottom value
					if ( textBox20 . Text != "" )
					{
						thickness . Bottom = Convert . ToDouble ( textBox20 . Text );
						DlgInput . BorderSizeNormal . Bottom = thickness . Bottom;
						UpdateButtonShadowSize ( );
					}
					break;
			}

			if ( DlgInput . MsgboxWin != null )
			{
				Msgbox w = DlgInput . MsgboxWin ;
				if ( w != null )
					w . UpdateDialog ( );
			}

			if ( DlgInput . MsgboxSmallWin != null )
			{
				Msgboxs x = DlgInput . MsgboxSmallWin ;
				if ( x != null )
					x . UpdateDialog ( );
			}
		}
		#endregion Screen Refresh method
		private void ListDlgInput ( )
		{
			string output = "";
			output += $"DLGINPUT Values\n\n";
			output += $"dlgbackground				{DlgInput . dlgbackground . ToString ( )}\n";
			output += $"dlgforeground				{DlgInput . dlgforeground . ToString ( ) }\n";
			output += $"btnbackground				{DlgInput . btnbackground . ToString ( )}\n";
			output += $"btnforeground				{DlgInput . btnforeground . ToString ( )}\n";
			output += $"Btnborder					{DlgInput . Btnborder . ToString ( )}\n";
			output += $"Btnmousebackground			{DlgInput . Btnmousebackground . ToString ( )}\n";
			output += $"Btnmouseforeground			{DlgInput . Btnmouseforeground . ToString ( ) }\n";
			output += $"defbtnbackground			{DlgInput . defbtnbackground . ToString ( )}\n";
			output += $"defbtnforeground			{DlgInput . defbtnforeground . ToString ( )}\n";
			output += $"BtnborderDark				{DlgInput . BtnborderDark . ToString ( )}\n";
			output += $"btnforegroundDark			{DlgInput . btnforegroundDark . ToString ( )}\n";
			output += $"btnbackgroundDark			{DlgInput . btnbackgroundDark . ToString ( )}\n";
			output += $"defbtnforegroundDark		{DlgInput . defbtnforegroundDark . ToString ( ) }\n";
			output += $"defbtnbackgroundDark		{DlgInput . defbtnbackgroundDark . ToString ( )}\n";
			output += $"mouseborderDark				{DlgInput . mouseborderDark . ToString ( )}\n";
			output += $"mousebackgroundDark			{DlgInput . mousebackgroundDark . ToString ( ) }\n";
			output += $"mouseforegroundDark			{DlgInput . mouseforegroundDark . ToString ( )}\n\n";

			Console . WriteLine ( output );

		}

		#region  create default settings 
		// Set defaullt settings colors for all  items
		private void ResetDefaultFieldColors ( int activefield )
		{
			if ( IsLoaded == false )
				return;
			Console . WriteLine ( "In ResetDefaultFieldColors ..." );
			textBox . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox1 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox1 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox2 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox2 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox3 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox3 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox4 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox4 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox5 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox5 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox6 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox6 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox7 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox7 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox8 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox8 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox9 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox9 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox10 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox10 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox11 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox11 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox12 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox12 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox13 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox13 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox14 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox14 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox15 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox15 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			textBox16 . Background = Utils . GetNewBrush ( "#FFFFFFFF" );
			textBox16 . Foreground = Utils . GetNewBrush ( "#FF000000" );
			//switch ( activefield )
			//	{
			//		case 1:
			//			textBox . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 2:
			//			textBox1 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox1 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 3:
			//			textBox2 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox2 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 4:
			//			textBox3 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox3 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 5:
			//			textBox4 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox4 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 6:
			//			textBox5 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox5 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 7:
			//			textBox6 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox6 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 8:
			//			textBox7 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox7 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 9:
			//			textBox8 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox8 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 10:
			//			textBox9 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox9 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 11:
			//			textBox10 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox10 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 12:
			//			textBox11 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox11 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 13:
			//			textBox12 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox12 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 14:
			//			textBox13 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox13 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 15:
			//			textBox14 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox14 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 16:
			//			textBox15 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox15 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 17:
			//			textBox16 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox16 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 18:
			//			textBox17 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox17 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 19:
			//			textBox18 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox18 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//		case 20:
			//			textBox19 . Background = ( Brush ) FindResource ( "White0" ) as Brush;
			//			textBox19 . Foreground = ( Brush ) FindResource ( "Red0" ) as Brush;
			//			break;
			//	}
		}

		private void CreateDefaultDlgInputSettings ( )
		{
			DlgInput . dlgbackground = Utils . GetNewBrush ( "#FFFFFFFF" );     // White
			textBox . Text = DlgInput . dlgbackground . BrushtoText ( );
			DlgInput . dlgforeground = Utils . GetNewBrush ( "#FF100035" );   // Black
			textBox1 . Text = DlgInput . dlgforeground . BrushtoText ( );
			DlgInput . btnbackground = Utils . GetNewBrush ( "#FF75C8E0" );   // Cyan
			textBox2 . Text = DlgInput . btnbackground . BrushtoText ( );
			DlgInput . btnforeground = Utils . GetNewBrush ( "#FF000000" );   // Black
			textBox3 . Text = DlgInput . btnforeground . BrushtoText ( );
			DlgInput . Btnborder = Utils . GetNewBrush ( "#FFB57BB0" ); // purple
			textBox4 . Text = DlgInput . Btnborder . BrushtoText ( );
			DlgInput . Btnmousebackground = Utils . GetNewBrush ( "#FF0000F9" );          // dark blue
			textBox5 . Text = DlgInput . Btnmousebackground . BrushtoText ( );
			DlgInput . Btnmouseforeground = Utils . GetNewBrush ( "#FFFFFFFF" );      //White
			textBox6 . Text = DlgInput . Btnmouseforeground . BrushtoText ( );
			DlgInput . defbtnbackground = Utils . GetNewBrush ( "#FFFF0018" );      // Red
			textBox7 . Text = DlgInput . defbtnbackground . BrushtoText ( );
			DlgInput . defbtnforeground = Utils . GetNewBrush ( "#FFFFFFFF" );      // White
			textBox8 . Text = DlgInput . defbtnforeground . BrushtoText ( );

			Button4 . Background = DlgInput . defbtnbackground;
			Button3 . Background = DlgInput . btnbackground;
			Button3 . Foreground = DlgInput . btnforeground;
			Button4 . Foreground = DlgInput . btnforeground;

			Thickness th =NormalBordersize;
			DlgInput . BorderSizeNormal = NormalBordersize;
			textBox17 . Text = th . Left . ToString ( );
			textBox18 . Text = th . Top . ToString ( );
			textBox19 . Text = th . Right . ToString ( );
			textBox20 . Text = th . Bottom . ToString ( );

			DlgInput . BorderSizeDefault = MouseoverBordersize;

			DlgInput . UseIcon = "#FFFF0018" == "T" ? true : false;
			DlgInput . isClean = "#FFFF0018" == "T" ? true : false;
			// Set it   to Normal mode for startup
			DlgInput . UseDarkMode = "DMN" . Contains ( "DMY" ) ? true : false;
//			darkmode . IsChecked = DlgInput . UseDarkMode;                                                        // No
			DlgInput . BtnborderDark = Utils . GetNewBrush ( "#FF35D3DF" );                     // cyan
			textBox9 . Text = DlgInput . BtnborderDark . BrushtoText ( );
			DlgInput . btnbackgroundDark = Utils . GetNewBrush ( "#FF2EA604" );           // Green
			textBox10 . Text = DlgInput . btnbackgroundDark . BrushtoText ( );
			DlgInput . btnforegroundDark = Utils . GetNewBrush ( "#FEFF469D" );                 // Purple
			textBox11 . Text = DlgInput . btnforegroundDark . BrushtoText ( );
			DlgInput . mouseborderDark = Utils . GetNewBrush ( "#FEF3CCE1" );             // Light purple
			textBox12 . Text = DlgInput . mouseborderDark . BrushtoText ( );
			DlgInput . mousebackgroundDark = Utils . GetNewBrush ( "#FE3552E6" );   // Blue
			textBox13 . Text = DlgInput . mousebackgroundDark . BrushtoText ( );
			DlgInput . mouseforegroundDark = Utils . GetNewBrush ( "#FEE5E4EA" );    // White
			textBox14 . Text = DlgInput . mouseforegroundDark . BrushtoText ( );
			DlgInput . defbtnbackgroundDark = Utils . GetNewBrush ( "#FEAD6A99" );  //Dark purple
			textBox15 . Text = DlgInput . defbtnbackgroundDark . BrushtoText ( );
			DlgInput . defbtnforegroundDark = Utils . GetNewBrush ( "#FFEAFF09" );        //yellow
			textBox16 . Text = DlgInput . defbtnforegroundDark . BrushtoText ( );

			// Force it to start in Normal mode
			DlgInput . UseDarkMode = false;
			ToggleMode ( );
			// Save these defaults - JIC
			//			SaveButtonColors ( );
		}

		#endregion  create default settings 

		#region  numeric text field verification methods
		private bool CheckValid ( string value )
		{
			// Validate  border size entries (0-9)
			int validcount = 0;
			if ( value . Length > 1 )
			{
				Utils . DoErrorBeep ( 280 , 20 , 1 );
				Utils . DoErrorBeep ( 220 , 50 , 1 );
				value = value . Substring ( 0 , 1 );
			}
			if ( ValidBorderSizes . Contains ( value ) == false )
			{
				return false;
				Utils . DoErrorBeep ( 280 , 100 , 1 );
				Utils . DoErrorBeep ( 200 , 250 , 1 );
			}
			return true;
		}
		private string CheckValidTextSize ( string value )
		{
			// Validate  border size entries (0-9)
			int validcount = 0;
			if ( value . Length > 2 )
				value = value . Substring ( 0 , 2 );
			if ( ValidBorderSizes . Contains ( value [ 0 ] ) == false )
			{
				Utils . DoErrorBeep ( 320 , 100 , 1 );
				Utils . DoErrorBeep ( 250 , 270 , 1 );
				return value;
			}
			if ( value . Length == 2 && ValidBorderSizes . Contains ( value [ 1 ] ) == false )
			{
				Utils . DoErrorBeep ( 280 , 100 , 1 );
				Utils . DoErrorBeep ( 200 , 250 , 1 );
				return value . Substring ( 0 , 1 );
			}
			if ( value . Length > 2 )
			{
				Utils . DoErrorBeep ( 280 , 20 , 1 );
				Utils . DoErrorBeep ( 220 , 50 , 1 );
				value = value . Substring ( 0 , 2 );
			}
			return value;
		}
		#endregion  numeric text field verification methods

		private void Img1Bannerrow_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		{
			string currentcolor = img1CaptionRow . Background.ToString();
			string newcolor="";
			newcolor = FindResource ( "Red6" ) . ToString ( );
			if ( currentcolor == newcolor )
			{
				img1CaptionRow . Background = FindResource ( "Green6" ) as SolidColorBrush;
				img1CaptionRow . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
				Image1Border . BorderBrush = FindResource ( "Green6" ) as SolidColorBrush;
				Icon1 . Source = new BitmapImage ( new Uri ( "\\icons\\3d green tick base.png" , UriKind . Relative ) );
				img1CaptionRow . Text = "Caption bar for a 'Progress'  style Dialog box";
			}
			newcolor = FindResource ( "Green6" ) . ToString ( );
			if ( currentcolor == newcolor )
			{
				img1CaptionRow . Background = FindResource ( "Yellow0" ) as SolidColorBrush;
				img1CaptionRow . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
				Image1Border . BorderBrush = FindResource ( "Yellow0" ) as SolidColorBrush;
				Icon1 . Source = new BitmapImage ( new Uri ( "\\icons\\3d-yellow-shriek.png" , UriKind . Relative ) );
				img1CaptionRow . Text = "Caption bar for a Warning style Dialog box";
			}
			newcolor = FindResource ( "Blue6" ) . ToString ( );
			if ( currentcolor == newcolor )
			{
				img1CaptionRow . Background = FindResource ( "Red6" ) as SolidColorBrush;
				img1CaptionRow . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				Image1Border . BorderBrush = FindResource ( "Red6" ) as SolidColorBrush;
				Icon1 . Source = new BitmapImage ( new Uri ( "\\icons\\3d red cross.png" , UriKind . Relative ) );
				img1CaptionRow . Text = "Caption bar for an Error style Dialog box";
			}
			newcolor = FindResource ( "Yellow0" ) . ToString ( );
			if ( currentcolor == newcolor )
			{
				img1CaptionRow . Background = FindResource ( "Blue6" ) as SolidColorBrush;
				img1CaptionRow . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				Image1Border . BorderBrush = FindResource ( "Blue6" ) as SolidColorBrush;
				Icon1 . Source = new BitmapImage ( new Uri ( "\\icons\\blue-info-icon.png" , UriKind . Relative ) );
				img1CaptionRow . Text = "Caption bar for an Information style Dialog box";
			}
			Icon1 . Height = 85;
			Icon1 . Width = 85;
			Icon1 . Visibility = Visibility . Visible;
			Icon1 . UpdateLayout ( );
		}
		private void Img2Bannerrow_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		{
			string currentcolor = img2CaptionRow . Background.ToString();
			string newcolor="";
			newcolor = FindResource ( "Red6" ) . ToString ( );
			if ( currentcolor == newcolor )
			{
				img2CaptionRow . Background = FindResource ( "Green6" ) as SolidColorBrush;
				img2CaptionRow . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
				Image2Border . BorderBrush = FindResource ( "Green6" ) as SolidColorBrush;
				Icon2 . Source = new BitmapImage ( new Uri ( "\\icons\\3d green tick base.png" , UriKind . Relative ) );
				img2CaptionRow . Text = "Caption bar for a 'Progress'  style Dialog box";
			}
			newcolor = FindResource ( "Green6" ) . ToString ( );
			if ( currentcolor == newcolor )
			{
				img2CaptionRow . Background = FindResource ( "Yellow0" ) as SolidColorBrush;
				img2CaptionRow . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
				Image2Border . BorderBrush = FindResource ( "Yellow0" ) as SolidColorBrush;
				Icon2 . Source = new BitmapImage ( new Uri ( "\\icons\\3d-yellow-shriek.png" , UriKind . Relative ) );
				img2CaptionRow . Text = "Caption bar for a Warning style Dialog box";
			}
			newcolor = FindResource ( "Blue6" ) . ToString ( );
			if ( currentcolor == newcolor )
			{
				img2CaptionRow . Background = FindResource ( "Red6" ) as SolidColorBrush;
				img2CaptionRow . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				Image2Border . BorderBrush = FindResource ( "Red6" ) as SolidColorBrush;
				Icon2 . Source = new BitmapImage ( new Uri ( "\\icons\\3d red cross.png" , UriKind . Relative ) );
				img2CaptionRow . Text = "Caption bar for an Error style Dialog box";
			}
			newcolor = FindResource ( "Yellow0" ) . ToString ( );
			if ( currentcolor == newcolor )
			{
				img2CaptionRow . Background = FindResource ( "Blue6" ) as SolidColorBrush;
				img2CaptionRow . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				Image2Border . BorderBrush = FindResource ( "Blue6" ) as SolidColorBrush;
				Icon2 . Source = new BitmapImage ( new Uri ( "\\icons\\blue-info-icon.png" , UriKind . Relative ) );
				img2CaptionRow . Text = "Caption bar for an Information style Dialog box";
			}
			Icon2 . Height = 85;
			Icon2 . Width = 85;
			Icon2 . Visibility = Visibility . Visible;
			Icon2 . UpdateLayout ( );
		}
	}
}
