
using Microsoft . SqlServer . Management . HadrModel;

using System;
using System . Collections . ObjectModel;
using System . IO;
using System . Security . RightsManagement;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Markup;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;

using WPFPages . AttachedProperties;

namespace WPFPages . Views
{
	public partial class Msgboxs : Window
	{
		private bool  IsTabbing=false;
		private int  CurrentButton = 0;
		private Border DefBorder;
		private bool isShiftDown = false;
		private int TabPass=0;
		private bool IsDarkMode { get; set; }
		private bool IsMinSize { get; set; }

		#region keys hit Properties
		public static string key1 { get; set; }
		public static string key2 { get; set; }
		public static string key3 { get; set; }
		public static string key4 { get; set; }
		#endregion keyhits

		#region row text data Properties

		string CaptionString { get; set; }
		string Row1String { get; set; }
		string Row2String { get; set; }
		string Row3String { get; set; }

		#endregion rowdata

		#region button text Properties
		string Btn1Text { get; set; }
		string Btn2Text { get; set; }
		string Btn3Text { get; set; }
		string Btn4Text { get; set; }
		System . Windows . Media . Brush Borderbrush { get; set; }

		int[] btnsarray = new int[4];
		string [ ] btnstextarray = { "","","",""};
		Button DefaultButton;


		static Brush Btnbackground;               // Std button Background
		static Brush Btnforeground;               // Std button Foreground
		static Brush BtnMbackground;        // Mouseover button Background
		static Brush BtnMforeground;        // Mouseover button Foreground
		static Brush DefBtnbackground;        // Defaullt Button Background
		static Brush DefBtnforeground;         // Default Button Foreground

		#endregion buttondata

		public Brush BorderColor { get; set; }
		public Thickness Bordersize { get; set; }
		public Thickness MouseBordersize { get; set; }
		public bool UseMinSize { get; set; }
		public static string Imagesource { get; set; }

		#region Setup  / Close down

		static bool CheckMsgBoxData ( )
		{
			if ( DlgInput . isClean )
			{
				DlgInput . resetdata = false;
				DlgInput . resultboolin = false;
				DlgInput . UseDarkMode = false;
				DlgInput . UseIcon = true;
				DlgInput . intin = 0;
				DlgInput . stringin = "";
				DlgInput . obj = null;
				DlgInput . dlgbackground = "#9DFFFFFB" . ToSolidBrush ( );
				DlgInput . Btnmouseforeground = "#FF000000" . ToSolidBrush ( );
				DlgInput . Btnborder = "#C1000000" . ToSolidBrush ( );
				DlgInput . btnforeground = null;
				DlgInput . btnbackground = null;
				DlgInput . iconstring = "";
				DlgInput . image = null;
			}
			return true;
		}
		//private void ConfigureCurrentColors ( )
		//{
		//	//Setup our attached prooperties from DlgInput structure
		//	SetValue( BkGroundProperty, DlgInput . bground );
		//	Msgbox . SetForeGround ( this , DlgInput . mousefground );
		//	Msgbox . SetMouseoverBackGround ( this , DlgInput . mousebground );
		//	Msgbox . SetMouseoverForeGround ( this , DlgInput . bforeground );
		//	Msgbox . SetBorderColor ( this , DlgInput . border );
		//	Msgbox . SetBorderSize ( this , DlgInput . BorderSize );
		//}
		public Msgboxs ( ) { }
		public Msgboxs (
			string caption = "" ,
			string string1 = "" ,
			string string2 = "" ,
			string iconstring = "" ,
			int Btn1 = 1 ,
			int Btn2 = 0 ,
			int defButton = 1 ,
			bool MinSize = false ,
			bool modal = false )
		{
			InitializeComponent ( );
			Utils . SetupWindowDrag ( this );
			bool result = CheckMsgBoxData ( );
			//			ConfigureCurrentColors ( ); 
			LoadWindow (
			 caption ,
			string1 ,
			string2 ,
			"" ,
		    iconstring ,
			 defButton ,
			 Btn1 ,
			 Btn2 ,
			 0 ,
			 0 ,
			 "" ,
			 "" ,
			 "" ,
			 "" ,
			 MinSize ,
			 modal = modal
				);
		}
		public Msgboxs (
			string caption ,
			string string1 ,
			string string2 = "" ,
			string title = "" ,
			string iconstring = "" ,
			int defButton = 1 ,
			int Btn1 = 1 ,
			int Btn2 = 0 ,
			int Btn3 = 0 ,
			int Btn4 = 0 ,
			string btn1Text = "" ,
			string btn2Text = "" ,
			string btn3Text = "" ,
			string btn4Text = "" ,
			bool MinSize = false ,
			bool modal = false
			)
		{

			InitializeComponent ( );
			Imagesource = iconstring;
			Utils . SetupWindowDrag ( this );
			//ReadMsgboxData ( 1 );
			//ReadMsgboxData ( 2 );
			Imagesource = iconstring;
			IsMinSize = MinSize;
			bool result = CheckMsgBoxData ( );
			//			ConfigureCurrentColors ( );
			LoadWindow (
			 caption = caption ,
			string1 = string1 ,
			string2 = string2 ,
			title = title ,
			iconstring = iconstring ,
			 defButton = defButton ,
			 Btn1 = Btn1 ,
			 Btn2 = Btn2 ,
			 Btn3 = Btn3 ,
			 Btn4 = Btn4 ,
			 btn1Text ,
			 btn2Text ,
			 btn3Text ,
			 btn4Text ,
			 MinSize ,
			 modal
				);

		}
		private void LoadWindow (
		string caption ,
		string string1 ,
		string string2 ,
		string title ,
		string iconstring ,
		int defButton ,
		int Btn1 ,
		int Btn2 ,
		int Btn3 ,
		int Btn4 ,
		string btn1Text ,
		string btn2Text ,
		string btn3Text ,
		string btn4Text ,
		bool MinSize ,
		bool modal
			)
		{
			// liit widht/height for OK only short message version
			if ( File . Exists ( @"Messageboxes.dat" ) == false )
				return;
			ReadMsgboxData ( 1 );
			ReadMsgboxData ( 2 );
			CheckForInitialDefaultSettings ( );
			DlgInput . MsgboxSmallWin = this;
			UseMinSize = MinSize;
			Imagesource = iconstring;
			MouseBordersize = new Thickness { Left = 1 , Top = 4 , Right = 1 , Bottom = 1 };
			// sort out the icon
			//if ( Imagesource . ToUpper ( ) . Contains ( "EXCLAIM" ) )
			//	path = @"/icons/Exclamation.png";
			//else if ( Imagesource . ToUpper ( ) . Contains ( "ERROR" ) )
			//	path = @"/icons/Error2.png";
			//else if ( Imagesource . ToUpper ( ) . Contains ( "WARN" ) )
			//	path = @"/icons/Warning2.png";
			//else if ( Imagesource . ToUpper ( ) . Contains ( "INFO" ) )
			//	path = @"/icons/information.png";

			string  path  = iconstring;
			BoxIcon . Source = new BitmapImage ( new Uri ( path , UriKind . RelativeOrAbsolute ) );

			if ( modal == false )
				Topmost = true;
			btnsarray [ 0 ] = Btn1;
			btnsarray [ 1 ] = Btn2;
			btnsarray [ 2 ] = Btn3;
			btnsarray [ 3 ] = Btn4;

			int indx=0;
			for ( int x = 0 ; x < 4 ; x++ )
			{
				switch ( x )
				{
					case 0:
						if ( btn1Text != "" )
						{
							btnstextarray [ indx++ ] = btn1Text;
							key1 = btn1Text . Substring ( 0 , 1 ) . ToUpper ( );
						}
						break;
					case 1:
						if ( btn2Text != "" )
						{
							btnstextarray [ indx++ ] = btn2Text;
							key2 = btn2Text . Substring ( 0 , 1 ) . ToUpper ( );
						}
						break;
					case 2:
						if ( btn3Text != "" )
						{
							btnstextarray [ indx++ ] = btn3Text;
							key3 = btn3Text . Substring ( 0 , 1 ) . ToUpper ( );
						}
						break;
					case 3:
						if ( btn4Text != "" )
						{
							btnstextarray [ indx++ ] = btn4Text;
							key4 = btn4Text . Substring ( 0 , 1 ) . ToUpper ( );
						}
						break;
				}
			}
			//			object obj = DlgInput . obj;
			// set up the buttons text
			if ( caption != "" )
				Caption . Text = caption;
			if ( btn1Text != "" )
				Btn1Text = btn1Text;
			if ( btn2Text != "" )
				Btn2Text = btn2Text;
			if ( btn3Text != "" )
				Btn3Text = btn3Text;
			if ( btn4Text != "" )
				Btn4Text = btn4Text;

			if ( btn1Text != "" ) // OK
			{
				Btn1Text = btn1Text;
				Button1 . Content = btn1Text;
				key1 = btn1Text . Substring ( 0 , 1 );
				btnstextarray [ 0 ] = btn1Text;
			}
			else
			{
				Btn1Text = "Ok";
				btn1Text = Btn1Text;
				//				Button1 . Content = btn1Text;
				key1 = "O";
				btnstextarray [ 0 ] = Btn1Text;
			}

			if ( btn2Text != "" )   // YES`
			{
				Btn2Text = btn2Text;
				//				Button2 . Content = btn2Text;
				key2 = btn2Text . Substring ( 0 , 1 );
				btnstextarray [ 1 ] = btn2Text;
			}
			else
			{
				Btn2Text = "Yes";
				btn2Text = Btn2Text;
				//				Button2 . Content = btn2Text;
				key2 = "Y";
				btnstextarray [ 1 ] = Btn2Text;
			}

			if ( btn3Text != "" )   // NO``
			{
				Btn3Text = btn3Text;
				//				Button3 . Content = btn3Text;
				key3 = btn3Text . Substring ( 0 , 1 );
				btnstextarray [ 2 ] = btn3Text;
			}
			else
			{
				Btn3Text = "No";
				btn3Text = Btn3Text;
				//				Button3 . Content = btn3Text;
				key3 = "N";
				btnstextarray [ 2 ] = Btn3Text;
			}

			if ( btn4Text != "" )         // Cancel
			{
				Btn4Text = btn4Text;
				//				Button4 . Content = btn4Text;
				key4 = btn4Text . Substring ( 0 , 1 );
				btnstextarray [ 3 ] = btn4Text;
			}
			else
			{
				Btn4Text = "Cancel";
				btn4Text = Btn4Text;
				//				Button4 . Content = btn4Text;
				key4 = "C";
				btnstextarray [ 3 ] = Btn4Text;
			}

			Button1 . Visibility = Visibility . Collapsed;
			Button2 . Visibility = Visibility . Collapsed;
			Button3 . Visibility = Visibility . Collapsed;
			Button4 . Visibility = Visibility . Collapsed;
			// Set up information strings 
			Row1String = string1;
			Row2String = string2;
			Row1 . Text = Row1String;
			Row2 . Text = Row2String;

			//Set Title bar
			this . Title = title == "" ? this . Title : title;
			// now decide which buttons are to be shown from a total of 4
			for ( int i = 0 ; i < 4 ; i++ )
			{
				switch ( i )
				{
					case 0:
						if ( btnsarray [ i ] == Btn1 && btnsarray [ i ] != 0 )
						{
							SetUpButtons ( btnsarray [ i ] , btnstextarray [ i ] , defButton );
							if ( btnsarray [ i ] == 1 )
								Button1 . Visibility = Visibility . Visible;
							else if ( btnsarray [ i ] == 2 )
								Button2 . Visibility = Visibility . Visible;
							else if ( btnsarray [ i ] == 3 )
								Button3 . Visibility = Visibility . Visible;
							else if ( btnsarray [ i ] == 4 )
								Button4 . Visibility = Visibility . Visible;
						}
						break;
					case 1:
						{
							if ( btnsarray [ i ] == Btn2 && btnsarray [ i ] != 0 )
								SetUpButtons ( btnsarray [ i ] , btnstextarray [ i ] , defButton );
							if ( btnsarray [ i ] == 1 )
								Button1 . Visibility = Visibility . Visible;
							else if ( btnsarray [ i ] == 2 )
								Button2 . Visibility = Visibility . Visible;
							else if ( btnsarray [ i ] == 3 )
								Button3 . Visibility = Visibility . Visible;
							else if ( btnsarray [ i ] == 4 )
								Button4 . Visibility = Visibility . Visible;
						}
						break;
					case 2:
						{
							if ( btnsarray [ i ] == Btn3 && btnsarray [ i ] != 0 )
								SetUpButtons ( btnsarray [ i ] , btnstextarray [ i ] , defButton );
							if ( btnsarray [ i ] == 1 )
								Button1 . Visibility = Visibility . Visible;
							else if ( btnsarray [ i ] == 2 )
								Button2 . Visibility = Visibility . Visible;
							else if ( btnsarray [ i ] == 3 )
								Button3 . Visibility = Visibility . Visible;
							else if ( btnsarray [ i ] == 4 )
								Button4 . Visibility = Visibility . Visible;
						}
						break;
					case 3:
						{
							if ( btnsarray [ i ] == Btn4 && btnsarray [ i ] != 0 )
								SetUpButtons ( btnsarray [ i ] , btnstextarray [ i ] , defButton );
							if ( btnsarray [ i ] == 1 )
								Button1 . Visibility = Visibility . Visible;
							else if ( btnsarray [ i ] == 2 )
								Button2 . Visibility = Visibility . Visible;
							else if ( btnsarray [ i ] == 3 )
								Button3 . Visibility = Visibility . Visible;
							else if ( btnsarray [ i ] == 4 )
								Button4 . Visibility = Visibility . Visible;
						}
						break;
				}
			}
			CheckForFinalDefaultSettings ( iconstring );

			if ( DefaultButton != null )
				DefaultButton . Focus ( );

			MouseMove += Grab_MouseMove;
			KeyDown += Window_PreviewKeyDown;

			// Setup the correct text for each active button
			if ( Btn1 > 0 )
			{
				switch ( Btn1 )
				{
					case 1:
						Button1 . Content = btnstextarray [ 0 ];
						break;
					case 2:
						Button2 . Content = btnstextarray [ 1 ];
						break;
					case 3:
						Button3 . Content = btnstextarray [ 2 ];
						break;
					case 4:
						Button4 . Content = btnstextarray [ 3 ];
						break;
				}
			}
			if ( Btn2 > 0 )
			{
				switch ( Btn2 )
				{
					case 1:
						Button1 . Content = btnstextarray [ 0 ];
						break;
					case 2:
						Button2 . Content = btnstextarray [ 1 ];
						break;
					case 3:
						Button3 . Content = btnstextarray [ 2 ];
						break;
					case 4:
						Button4 . Content = btnstextarray [ 3 ];
						break;
				}
			}
			if ( Btn3 > 0 )
			{
				switch ( Btn3 )
				{
					case 1:
						Button1 . Content = btnstextarray [ 0 ];
						break;
					case 2:
						Button2 . Content = btnstextarray [ 1 ];
						break;
					case 3:
						Button3 . Content = btnstextarray [ 2 ];
						break;
					case 4:
						Button3 . Content = btnstextarray [ 3 ];
						break;
				}
			}
			if ( Btn4 > 0 )
			{
				switch ( Btn4 )
				{
					case 1:
						Button1 . Content = btnstextarray [ 0 ];
						break;
					case 2:
						Button2 . Content = btnstextarray [ 1 ];
						break;
					case 3:
						Button3 . Content = btnstextarray [ 2 ];
						break;
					case 4:
						Button4 . Content = btnstextarray [ 3 ];
						break;
				}
			}
			// setup color of caption and dlg border to suit typew of dialog we are showing
			if ( iconstring . ToUpper ( ) . Contains ( "EXCLAIM" ) )
				HiliteBorder . BorderBrush = FindResource ( "Yellow0" ) as SolidColorBrush;
			else if ( iconstring . ToUpper ( ) . Contains ( "ERROR" ) )
			{
				HiliteBorder . BorderBrush = FindResource ( "Red6" ) as SolidColorBrush;
				Caption.Foreground = FindResource ( "White0" ) as SolidColorBrush;
			}
			else if ( iconstring . ToUpper ( ) . Contains ( "WARN" ) )
				HiliteBorder . BorderBrush = FindResource ( "Orange0" ) as SolidColorBrush;
			else if ( iconstring . ToUpper ( ) . Contains ( "INFO" ) )
				HiliteBorder . BorderBrush = FindResource ( "Green6" ) as SolidColorBrush;
			Caption . Background = HiliteBorder . BorderBrush;
			BoxIcon . Visibility = Visibility . Visible;
			// Force it to min size if only one row of text
			if ( string2 == "" )
				IsMinSize = true;
		}

		private void Window_Loaded ( object sender , RoutedEventArgs e )
		{
			// liit widht/height for OK only short message version
			int textlen = Row1.Text.Length + Row2.Text.Length;
			
			if ( textlen < 250 && Row2.Text == "")
			{
				// just one row of text	 < 250 chars
				//if ( Row1 . FontSize > 13 )
				//{
					Row1 . Height = 90;
					switch ( Row1 . FontSize )
					{
						case 9:	// OK
						Row1 . Height = 45;
						Row2 . Height = 10;
						this . Height = 100;
						this . Width = 365;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -60 , right: 0 , bottom: 0 );
						break;
						case 10:	// OK
						Row1 . Height = 45;
						Row2 . Height = 10;
						this . Height = 100;
						this . Width = 365;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -60 , right: 0 , bottom: 0 );
						break;
						case 11:		// OK
							Row1 . Height = 45;
							Row2 . Height = 10;
							this . Height = 120;
							this . Width = 385;
							BtnWrap . Margin = new Thickness ( left: 0 , top: -60 , right: 0 , bottom: 0 );
							break;
						case 12:	// OK
							Row1 . Height = 50;
							Row2 . Height = 10;
							this . Height = 120;
							this . Width = 400;
							BtnWrap . Margin = new Thickness ( left: 0 , top: -60 , right: 0 , bottom: 0 );
						break;
						case 13:	// OK
							Row1 . Height = 55;
							Row2 . Height = 10;
							this . Height = 130;
							this . Width = 440;
							BtnWrap . Margin = new Thickness ( left: 0 , top: -60 , right: 0 , bottom: 0 );
							break;	// OK
						case 14:
							Row1 . Height = 60;
							Row2 . Height = 10;
							this . Height = 150;
							this . Width = 525;
							BtnWrap . Margin = new Thickness ( left: 0 , top: -65 , right: 0 , bottom: 0 );
							break;
						case 15:	// OK
							Row1 . Height = 70;
							Row2 . Height = 10;
							this . Height = 160;
							this . Width = 610;
							BtnWrap . Margin = new Thickness ( left: 0 , top: -65 , right: 0 , bottom: 0 );
							break;
						case 16:	// OK
							Row1 . Height = 70;
							Row2 . Height = 10;
							this . Height = 160;
							this . Width = 600;
							BtnWrap . Margin = new Thickness ( left: 0 , top: -65 , right: 0 , bottom: 0 );
							break;
						case 17:	//OK
							Row1 . Height = 90;
							Row2 . Height = 10;
							this . Height = 180;
							this . Width = 575;
							BtnWrap . Margin = new Thickness ( left: 0 , top: -65 , right: 0 , bottom: 0 );
							break;
						case 18:		//OK
							Row1 . Height = 90;
							Row2 . Height = 10;
							this . Height = 200;
							this . Width = 750;
							BtnWrap . Margin = new Thickness ( left: 0 , top: -40 , right: 0 , bottom: 0 );
							break;
						case 19:	// OK
							Row1 . Height = 90;
							Row2 . Height = 10;
							this . Height = 200;
							this . Width = 750;
							BtnWrap . Margin = new Thickness ( left: 0 , top: -40 , right: 0 , bottom: 0 );
							break;
						case 20:		// OK
							Row1 . Height = 90;
							Row2 . Height = 10;
							this . Height = 200;
							this . Width = 750;
							BtnWrap . Margin = new Thickness ( left: 0 , top: -40 , right: 0 , bottom: 0 );
							break;
						case 21:
							this . Height = 250;
							this . Width = 535;
							BtnWrap . Margin = new Thickness ( left: 0 , top: -60 , right: 0 , bottom: 0 );
							break;
					}
					//is . Width = 450;
					// Move buttons UP in dialog ?
				//}
				//else
				//{
				//	Row1 . Height = 70;
				//	this . Height = 160;
				//	this . Width = 430;
				//	// Move buttons UP in dialog ?
				//	BtnWrap . Margin = new Thickness ( left: 0 , top: -60 , right: 0 , bottom: 0 );
				//}
				//Row2 . Height = 10;
//				}
			}
			else if ( textlen < 250 && Row2 . Text != "" )
			{
				// Two rows of text but row1 < 250 chars
				if ( Row1 . FontSize > 13 )
				{
					// LARGE font
					Row1 . Height = 120;
					Row2 . Height = 40;
					this . Height = 230;
					this . Width = 525;
					BtnWrap . Margin = new Thickness ( left: 0 , top: -15 , right: 0 , bottom: 0 );
				}
				else
				{	// SMALLER  font
					Row1 . Height = 80;
					Row2 . Height = 40;
					this . Height = 220;
					this . Width = 525;
					BtnWrap . Margin = new Thickness ( left: 0 , top: -20 , right: 0 , bottom: 0 );
				}
			}
			else
			{
				// has 2 rows of text
				if ( textlen < 250 )
				{
					Row1 . Height = 80;
					this . Height = 230;
					this . Width = 425;
					BtnWrap . Margin = new Thickness ( left: 0 , top: 13 , right: 0 , bottom: 0 );
				}
				else
				{
					//textlen > 250 chars
					if ( Row1 . FontSize > 13 )
					{
						Row1 . Height = 60;
						this . Height = 220;
						this . Width = 525;
						BtnWrap . Margin = new Thickness ( left: 0 , top: 0 , right: 0 , bottom: 0 );
					}
					else
					{
						Row1 . Height = 50;
						this . Height = 220;
						this . Width = 475;
						BtnWrap . Margin = new Thickness ( left: 0 , top: 0 , right: 0 , bottom: 0 );
					}
					//				Row2 . Height = 50;
				}
				//if ( IsMinSize )
				//	Height = 180;
				// Move buttons up in dialog ?
			}

			// Handle  width for multiple buttons
			if ( Button1 . Visibility == Visibility . Visible
				&& Button2 . Visibility == Visibility . Visible
				&& Button3 . Visibility == Visibility . Visible
				&& Button4 . Visibility == Visibility . Visible )
				this . Width = 525;
			else if ( Button1 . Visibility == Visibility . Visible
				&& Button2 . Visibility == Visibility . Visible
				&& ( Button3 . Visibility == Visibility . Visible || Button4 . Visibility == Visibility . Visible ) )
				this . Width = 565;
		}

		#endregion Setup

		#region initialize colors

		//Set up the basic Dlg colors etc
		public void CheckForInitialDefaultSettings ( )
		{

			//			Console . WriteLine ( $"Processing CheckForInitialDefaultSettings in Msgbox.cs...." );
			if ( IsDarkMode )
			{
				// always set threse 2
				DlgInput . dlgbackground = "#F5374340" . ToSolidBrush ( );         // off Black
				DlgInput . dlgforeground = FindResource ( "White0" ) as Brush;

				DlgInput . Btnmouseforeground = FindResource ( "White0" ) as Brush;
				if ( DlgInput . btnbackground == null )
					DlgInput . btnbackground = "#86FF5100" . ToSolidBrush ( ); // orange
				if ( DlgInput . btnforeground == null )
					DlgInput . btnforeground = FindResource ( "Black0" ) as Brush;
				if ( DlgInput . Btnmousebackground == null )
					DlgInput . Btnmousebackground = FindResource ( "Black3" ) as Brush;

				DlgInput . defbtnbackground = "#86FF5100" . ToSolidBrush ( );
				DlgInput . defbtnforeground = "#ffffffff" . ToSolidBrush ( );
				// Always set this for dark mode
				DlgInput . Btnborder = "#FD09CACE" . ToSolidBrush ( ); // Cyan
			}
			else
			{
				if ( DlgInput . dlgbackground == null )
					DlgInput . dlgbackground = FindResource ( "White6" ) as Brush;
				if ( DlgInput . dlgforeground == null )
					DlgInput . dlgforeground = FindResource ( "Black0" ) as Brush;
				if ( DlgInput . btnforeground == null )
					DlgInput . btnforeground = FindResource ( "White0" ) as Brush;
				if ( DlgInput . Btnmousebackground == null )
					DlgInput . Btnmousebackground = FindResource ( "Black3" ) as Brush;
				if ( DlgInput . Btnmouseforeground == null )
					DlgInput . Btnmouseforeground = FindResource ( "Black0" ) as Brush;
				if ( DlgInput . Btnborder == null )
					DlgInput . Btnborder = FindResource ( "#FD09CACE" ) as Brush;
			}
			Borderbrush = DlgInput . Btnborder;
			BorderBrush = DlgInput . Btnborder;
			BorderColor = DlgInput . Btnborder;

			Btnbackground = DlgInput . btnbackground;
			Btnforeground = DlgInput . btnforeground;
			BtnMbackground = DlgInput . Btnmousebackground;
			BtnMforeground = DlgInput . Btnmouseforeground;
			DefBtnbackground = DlgInput . defbtnbackground;
			DefBtnforeground = DlgInput . defbtnforeground;
			//			BorderBrush = DlgInput . Btnborder;

			Background = DlgInput . dlgbackground;
			BoxGrid . Background = DlgInput . dlgbackground;

			Row1 . Foreground = DlgInput . dlgforeground;
			Row2 . Foreground = DlgInput . dlgforeground;
			//			Row3 . Foreground = DlgInput . dlgforeground;
			if ( IsDarkMode )
			{
				Row1 . Background = DlgInput . dlgbackground;
				Row2 . Background = DlgInput . dlgbackground;
				//				Caption . Background = DlgInput . dlgbackground;
				Row1 . Foreground = DlgInput . dlgforeground;
				Row2 . Foreground = DlgInput . dlgforeground;
			}
			else
			{
				Row1 . Background = DlgInput . dlgbackground;
				Row2 . Background = DlgInput . dlgbackground;
				//				Caption . Background = DlgInput . dlgbackground;
			}
			Button1 . Background = DlgInput . btnbackground;
			Button2 . Background = DlgInput . btnbackground;
			Button3 . Background = DlgInput . btnbackground;
			Button4 . Background = DlgInput . btnbackground;
			Button1 . Foreground = DlgInput . btnforeground;
			Button2 . Foreground = DlgInput . btnforeground;
			Button3 . Foreground = DlgInput . btnforeground;
			Button4 . Foreground = DlgInput . btnforeground;

			Button1 . BorderBrush = DlgInput . Btnborder;
			Button2 . BorderBrush = DlgInput . Btnborder;
			Button3 . BorderBrush = DlgInput . Btnborder;
			Button4 . BorderBrush = DlgInput . Btnborder;

			//			if ( DlgInput . BorderSize == null )
			//				DlgInput . BorderSize = new Thickness { Left = 1 , Top = 2 , Right = 2 , Bottom = 5 };
			Button1 . BorderThickness = DlgInput . BorderSizeNormal;
			Button2 . BorderThickness = DlgInput . BorderSizeNormal;
			Button3 . BorderThickness = DlgInput . BorderSizeNormal;
			Button4 . BorderThickness = DlgInput . BorderSizeNormal;

			//Button1Text . Foreground = DlgInput . btnforeground;
			//Button2Text . Foreground = DlgInput . btnforeground;
			//Button3Text . Foreground = DlgInput . btnforeground;
			//Button4Text . Foreground = DlgInput . btnforeground;

			Button1 . Refresh ( );
			Button2 . Refresh ( );
			Button3 . Refresh ( );
			Button4 . Refresh ( );
			string iconstring = BoxIcon.Source.ToString();
			if ( iconstring . ToUpper ( ) . Contains ( "INFO" ) )
				iconstring = "INFO";
			else if ( iconstring . ToUpper ( ) . Contains ( "WARN" ) )
				iconstring = "WARN";
			else if ( iconstring . ToUpper ( ) . Contains ( "ERROR" ) )
				iconstring = "INFOERROR";
			SetupCaptionColor ( iconstring );
			this . Refresh ( );
		}

		private void SetupCaptionColor ( string type )
		{
			switch ( type . ToUpper ( ) )
			{
				case "INFO":
					Caption . Background = FindResource ( "Green6" ) as SolidColorBrush;
					break;
				case "WARN":
					Caption . Background = new SolidColorBrush ( Colors . Orange );
					break;
				case "IERROR":
					Caption . Background = new SolidColorBrush ( Colors . Red );
					break;
			}
		}

		#endregion intcolors
		private void Grab_MouseMove ( object sender , MouseEventArgs e )
		{
			if ( e . LeftButton == MouseButtonState . Pressed )
				Utils . Grab_MouseMove ( sender , e );
			e . Handled = true;
		}

		private void ChecksMouseMove ( object sender , MouseEventArgs e )
		{
			e . Handled = true;
			if ( e . RightButton == MouseButtonState . Pressed )
				return;
		}

		/// <summary>
		/// Does the hard work of configuring the viusual appearance of each button
		/// </summary>
		/// <param name="Btn"></param>
		/// <param name="btntext"></param>
		/// <param name="defButton"></param>
		private void SetUpButtons ( int Btn , string btntext , int defButton )
		{
			if ( Btn >= 0 && Btn <= 4 )
			{
				if ( Btn == 1 )   // OK Button
				{
					if ( btntext != null )
						Button1 . Content = btntext != "" ? btntext : "OK";
					if ( defButton == Btn )
					{
						Button1 . IsDefault = true;
						DefaultButton = Button1;
						CurrentButton = Btn;
						if ( DefaultButton != null )
							DefaultButton . Focus ( );
					}
					UpdateButtonX ( Button1 );
				}
				if ( Btn == 2 )  // YES Button
				{
					if ( btntext != null )
						Button2 . Content = btntext != "" ? btntext : "Yes";
					if ( defButton == Btn )
					{
						Button2 . IsDefault = true;
						DefaultButton = Button2;
						CurrentButton = Btn;
						if ( DefaultButton != null )
							DefaultButton . Focus ( );
					}
					UpdateButtonX ( Button2 );
				}
				if ( Btn == 3 )   // NO Button
				{
					if ( btntext != null )
						Button3 . Content = btntext != "" ? btntext : "No";
					if ( defButton == Btn )
					{
						Button3 . IsDefault = true;
						DefaultButton = Button3;
						CurrentButton = Btn;
						if ( DefaultButton != null )
							DefaultButton . Focus ( );
					}
					UpdateButtonX ( Button3 );
				}
				if ( Btn == 4 )   // CANCEL Button
				{
					if ( btntext != null )
						Button4 . Content = btntext != "" ? btntext : "Cancel";
					if ( defButton == Btn )
					{
						Button4 . IsDefault = true;
						DefaultButton = Button4;
						CurrentButton = Btn;
						if ( DefaultButton != null )
							DefaultButton . Focus ( );
					}
					Button4 . IsCancel = true;
					UpdateButtonX ( Button4 );
				}
			}
		}

		private void UpdateButtonX ( Button Btn )
		{
			//Btn . Height = 35;
			//Btn . Width = 100;
			Btn . Visibility = Visibility . Visible;
			//Thickness th = new Thickness();
			//th . Right = 10;
			//Btn . Margin = th;
			if ( Btn . IsDefault == true )
			{
				// setting  up Default button colors
				if ( DlgInput . UseDarkMode )
				{
					Btn . BorderBrush = DlgInput . mouseborderDark;
					Btn . Background = DlgInput . defbtnbackgroundDark;
					Btn . Foreground = DlgInput . defbtnforegroundDark;
					Btn . BorderThickness = DlgInput . BorderSizeDefault;
				}
				else
				{
					Btn . BorderBrush = DlgInput . Btnborder;
					Btn . Background = DlgInput . defbtnbackground;
					Btn . Foreground = DlgInput . defbtnforeground;
					Btn . BorderThickness = DlgInput . BorderSizeNormal;
				}
			}
			else
			{
				// Normal button
				//				b . BorderBrush = FindResource ( "Black0" ) as Brush;
				Btn . BorderBrush = DlgInput . Btnborder;
				Btn . Background = DlgInput . btnbackground;
				Btn . Foreground = DlgInput . btnforeground;
				Btn . BorderThickness = DlgInput . BorderSizeNormal;
			}

			// Set the buttons up so they opoerate
			Btn . UpdateLayout ( );
		}

		private void CheckForFinalDefaultSettings ( string iconstring )
		{
			// set up dlg icon
			if ( DlgInput . UseIcon && iconstring == "" )
			{
				if ( DlgInput . iconstring != "" )
					iconstring = DlgInput . iconstring;
				else
					iconstring = DlgInput . iconstring = "\\icons\\Information.png";
				BoxIcon . Source = new BitmapImage ( new Uri ( iconstring , UriKind . Relative ) );
				BoxIcon . Visibility = Visibility . Visible;
			}
			else if ( DlgInput . UseIcon && iconstring != "" )
			{
				BoxIcon . Source = new BitmapImage ( new Uri ( iconstring , UriKind . Relative ) );
				BoxIcon . Visibility = Visibility . Visible;
			}
			else
			{
				BoxIcon . Visibility = Visibility . Collapsed;
				//Wrappanel . GridColumn = 1;
			}
			//if ( (double)BoxIcon . Height == double.NaN )
			//	BoxIcon . Height = Wrappanel . Height;
			//if ( BoxIcon . Width== double . NaN )
			//	BoxIcon . Width= Wrappanel . Height;
		}

		#region Button Handlers
		//********************//
		// ProcessYes  event
		//********************//
		// Called when btn1 is clicked
		private void Button1_ProcessOK ( object sender , RoutedEventArgs e )
		{
			//			RaiseProcessOKEvent ( );
			ProcessOKBtn ( );
		}
		private void Button2_ProcessYes ( object sender , RoutedEventArgs e )
		{
			//			RaiseProcessYesEvent ( );
			ProcessYesBtn ( );
		}
		private void Button3_ProcessNo ( object sender , RoutedEventArgs e )
		{
			ProcessNoBtn ( );
		}
		private void Button4_ProcessCancel ( object sender , RoutedEventArgs e )
		{
			ProcessCancelBtn ( );
		}
		private void Button1_ProcessOK ( object sender , MouseButtonEventArgs e )
		{
			ProcessOKBtn ( );
		}
		private void Button2_ProcessYes ( object sender , MouseButtonEventArgs e )
		{
			ProcessYesBtn ( );

		}
		private void Button3_ProcessNo ( object sender , MouseButtonEventArgs e )
		{
			ProcessNoBtn ( );

		}
		private void Button4_ProcessCancel ( object sender , MouseButtonEventArgs e )
		{
			ProcessCancelBtn ( );

		}

		private void Button1_KeyDown ( object sender , KeyEventArgs e )
		{
			bool changed=false;
			Button newbtn = null;
			if ( e . Key == Key . Enter )
			{
				e . Handled = true;
				{
					DlgInput . MsgboxSmallWin = null;
					this . Close ( );
				}
			}
			else if ( e . Key == Key . Escape )
			{
				e . Handled = true;
				DlgInput . MsgboxSmallWin = null;
				this . Close ( );
			}

			else if ( e . Key == Key . Tab || e . Key == Key . Right )
			{
				int newsel = 0;
				newbtn = GetPrevButton ( CurrentButton , "RIGHT" , out newsel );
				CurrentButton = newsel;
				changed = true;
			}
			else if ( e . Key == Key . Left )
			{
				int newsel = 0;
				newbtn = GetPrevButton ( CurrentButton , "LEFT" , out newsel );
				CurrentButton = newsel;
				changed = true;
			}
			ResetButtonStatus ( newbtn , Button1 );
			e . Handled = true;
		}
		private void Button2_KeyDown ( object sender , KeyEventArgs e )
		{
			bool changed = false;
			Button newbtn = null;
			if ( e . Key == Key . Enter )
			{
				e . Handled = true;
				DlgInput . MsgboxSmallWin = null;
				this . Close ( );
			}
			else if ( e . Key == Key . Escape )
			{
				e . Handled = true;
				DlgInput . MsgboxSmallWin = null;
				this . Close ( );
			}
			else if ( e . Key == Key . Tab || e . Key == Key . Right )
			{
				int newsel = 0;
				newbtn = GetPrevButton ( CurrentButton , "RIGHT" , out newsel );
				CurrentButton = newsel;
				//				UpdateButtonX ( newbtn );
				changed = true;
			}
			else if ( e . Key == Key . Left )
			{
				int newsel = 0;
				newbtn = GetPrevButton ( CurrentButton , "LEFT" , out newsel );
				CurrentButton = newsel;
				//				UpdateButtonX ( newbtn );
				changed = true;
			}
			ResetButtonStatus ( newbtn , Button2 );
			e . Handled = true;
		}

		private void Button3_KeyDown ( object sender , KeyEventArgs e )
		{
			bool changed=false;
			Button newbtn = null;
			if ( e . Key == Key . Enter )
			{
				e . Handled = true;
				DlgInput . MsgboxSmallWin = null;
				this . Close ( );
			}
			else if ( e . Key == Key . Escape )
			{
				e . Handled = true;
				DlgInput . MsgboxSmallWin = null;
				this . Close ( );
			}

			else if ( e . Key == Key . Tab || e . Key == Key . Right )
			{
				int newsel = 0;
				newbtn = GetPrevButton ( CurrentButton , "RIGHT" , out newsel );
				CurrentButton = newsel;
				changed = true;
			}
			else if ( e . Key == Key . Left )
			{
				int newsel = 0;
				newbtn = GetPrevButton ( CurrentButton , "LEFT" , out newsel );
				CurrentButton = newsel;
				changed = true;
			}
			ResetButtonStatus ( newbtn , Button3 );
			e . Handled = true;
		}
		private void Button4_KeyDown ( object sender , KeyEventArgs e )
		{
			bool changed=false;
			Button newbtn = null;
			if ( e . Key == Key . Enter )
			{
				e . Handled = true;
				DlgInput . MsgboxSmallWin = null;
				this . Close ( );
			}
			else if ( e . Key == Key . Escape )
			{
				e . Handled = true;
				DlgInput . MsgboxSmallWin = null;
				this . Close ( );
			}

			else if ( e . Key == Key . Tab || e . Key == Key . Right )
			{
				int newsel = 0;
				newbtn = GetPrevButton ( CurrentButton , "RIGHT" , out newsel );
				CurrentButton = newsel;
				changed = true;
			}
			else if ( e . Key == Key . Left )
			{
				int newsel = 0;
				newbtn = GetPrevButton ( CurrentButton , "LEFT" , out newsel );
				CurrentButton = newsel;
				changed = true;
			}
			ResetButtonStatus ( newbtn , Button4 );
			e . Handled = true;
		}
		private Button GetPrevButton ( int currentbtn , string direction , out int newsel )
		{
			int reslt=0;
			newsel = 0;
			if ( direction == "LEFT" )
			{
				// moving left
				for ( int x = 3 ; x >= 0 ; x-- )
				{
					if ( btnsarray [ x ] == currentbtn )
					{
						if ( x == 0 )
						{
							if ( btnsarray [ 3 ] != 0 )
								reslt = btnsarray [ 3 ];
							else if ( btnsarray [ 2 ] != 0 )
								reslt = btnsarray [ 2 ];
							else if ( btnsarray [ 1 ] != 0 )
								reslt = btnsarray [ 1 ];
							break;
						}
						else
						{
							reslt = btnsarray [ x - 1 ];
							break;
						}
					}
				}
			}
			else
			{
				// moving right}
				for ( int x = 0 ; x < 4 ; x++ )
				{
					if ( btnsarray [ x ] == currentbtn )
					{
						if ( x == 3 )
						{
							// at end, so next button must be 1st by wrapping around
							reslt = btnsarray [ 0 ];
							break;
						}
						else
						{
							// Move forward to find next button
							int y = x;
							do
							{
								if ( btnsarray [ y + 1 ] > 0 )
								{
									reslt = btnsarray [ y + 1 ];
									break;
								}
								y++;
							} while ( y < 3 );
							if ( y == 3 )
							{
								reslt = btnsarray [ 0 ];
							}
							break;
						}
					}
				}
			}
			switch ( reslt )
			{
				case 1:
					newsel = 1;
					return Button1;
				case 2:
					newsel = 2;
					return Button2;
				case 3:
					newsel = 3;
					return Button3;
				case 4:
					newsel = 4;
					return Button4;
			}
			return null;
		}
		private void ResetButtonStatus ( Button newbtn , Button oldbtn )
		{
			bool changed = false;
			if ( newbtn == null || oldbtn == null )
				return;
			if ( newbtn == Button1 )
			{
				Button1 . IsDefault = true;
				UpdateButtonX ( Button1 );
				changed = true;
			}
			else if ( newbtn == Button2 )
			{
				Button2 . IsDefault = true;
				UpdateButtonX ( Button2 );
				changed = true;
			}
			else if ( newbtn == Button3 )
			{
				Button3 . IsDefault = true;
				UpdateButtonX ( Button3 );
				changed = true;
			}
			else if ( newbtn == Button4 )
			{
				Button4 . IsDefault = true;
				UpdateButtonX ( Button4 );
				changed = true;
			}
			if ( changed == false )
			{
				newbtn . IsDefault = true;
				UpdateButtonX ( Button3 );
			}
			else
			{
				oldbtn . IsDefault = false;
				UpdateButtonX ( oldbtn );
			}
		}

		private void UpdateButtonX ( int a , Button Button1 , bool IsTabbing )
		{ }

		#endregion Button Handlers

		// Force the dialog to Keep Focus inside  the caller app (flashes if another gets focus)
		private void Window_Deactivated ( object sender , EventArgs e )
		{
			this . Deactivated += ( s , e ) => this . Activate ( );
		}
		private void Window_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . LeftShift || e . Key == Key . RightShift )
			{
				if ( e . IsUp )
					isShiftDown = false;
			}
		}

		private void Window_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			e . Handled = true;
			if ( e . Key == Key . F11 )
			{
				var pos = Mouse . GetPosition ( this);
				Utils . Grab_Object ( sender , pos );
				if ( Utils . ControlsHitList . Count == 0 )
					return;
				Utils . Grabscreen ( this , Utils . ControlsHitList [ 0 ] . VisualHit , null , sender as Control );
			}
			if ( e . Key == Key . Enter )
			{
				if ( CurrentButton == 1 )
					Button1_ProcessOK ( null , null );
				else if ( CurrentButton == 2 )
					Button2_ProcessYes ( null , null );
				else if ( CurrentButton == 3 )
					Button3_ProcessNo ( null , null );
				else if ( CurrentButton == 4 )
					Button4_ProcessCancel ( null , null );
				e . Handled = true;
			}
			// Check for Hot keys & Shift pressed
			Key key = e . Key;
			string currkey  =key . ToString ( );
			if ( currkey . ToUpper ( ) == key1 )
			{
				Button1_ProcessOK ( null , null );
			}
			else if ( currkey . ToUpper ( ) == key2 )
			{
				Button2_ProcessYes ( null , null );
			}
			else if ( currkey . ToUpper ( ) == key3 )
			{
				Button3_ProcessNo ( null , null );
			}
			else if ( currkey . ToUpper ( ) == key4 )
			{
				Button4_ProcessCancel ( null , null );
			}
			else if ( e . Key == Key . LeftShift || e . Key == Key . RightShift )
			{
				if ( e . IsDown )
					isShiftDown = true;
			}
			// Check for exit keys
			if ( key == Key . Enter )
			{
				DlgInput . intin = 1;  // ?????
				DlgInput . MsgboxSmallWin = null;
				this . Close ( );
				e . Handled = true;
			}
			else if ( key == Key . Escape )
			{
				DlgInput . intin = 0;  // ?????
				DlgInput . MsgboxSmallWin = null;
				this . Close ( );
				e . Handled = true;
			}
			// see which other keys have been  hit (tab/arrow etc)
			if ( key == Key . Tab && isShiftDown )         // Tab with shift, so move button selection left
			{
				if ( CurrentButton == 1 )
					;
				e . Handled = true;
			}
			else if ( key == Key . Tab && isShiftDown == false )        // Tab without shift, so move button selection right
			{
				//DlgInput . intin = 0;  // ?????
				//this . Close ( );
				//e . Handled = true;
			}


			if ( CurrentButton == 1 )
			{
				Button1_KeyDown ( Button1 , e );
				e . Handled = true;
				IsTabbing = false;
				return;
			}
			else if ( CurrentButton == 2 )
			{
				Button2_KeyDown ( sender , e );
				e . Handled = true;
				IsTabbing = false;
				return;
			}
			else if ( CurrentButton == 3 )
			{
				Button3_KeyDown ( Button3 , e );
				e . Handled = true;
				IsTabbing = false;
				return;
			}
			else if ( CurrentButton == 4 )
			{
				Button4_KeyDown ( Button4 , e );
				e . Handled = true;
				IsTabbing = false;
				return;
			}
			IsTabbing = false;
		}

		private void Window_KeyDown ( object sender , KeyEventArgs e )
		{
			IsTabbing = true;

			if ( e . Key == Key . Enter )
			{
				if ( CurrentButton == 1 )
					Button1_ProcessOK ( null , null );
				else if ( CurrentButton == 2 )
					Button2_ProcessYes ( null , null );
				else if ( CurrentButton == 3 )
					Button3_ProcessNo ( null , null );
				else if ( CurrentButton == 4 )
					Button4_ProcessCancel ( null , null );
				e . Handled = true;
			}
			// Check for Hot keys
			Key key = e . Key;
			string currkey  =key . ToString ( );
			if ( currkey . ToUpper ( ) == key1 )
			{
				Button1_ProcessOK ( null , null );
			}
			else if ( currkey . ToUpper ( ) == key2 )
			{
				Button2_ProcessYes ( null , null );
			}
			else if ( currkey . ToUpper ( ) == key3 )
			{
				Button3_ProcessNo ( null , null );
			}
			else if ( currkey . ToUpper ( ) == key4 )
			{
				Button4_ProcessCancel ( null , null );
			}
			else if ( e . Key == Key . LeftShift || e . Key == Key . RightShift )
			{
				if ( e . IsDown )
					isShiftDown = true;
			}

			if ( CurrentButton == 1 )
			{
				Button1_KeyDown ( Button1 , e );
				e . Handled = true;
				IsTabbing = false;
				return;
			}
			else if ( CurrentButton == 2 )
			{
				Button2_KeyDown ( sender , e );
				e . Handled = true;
				IsTabbing = false;
				return;
			}
			else if ( CurrentButton == 3 )
			{
				Button3_KeyDown ( Button3 , e );
				e . Handled = true;
				IsTabbing = false;
				return;
			}
			else if ( CurrentButton == 4 )
			{
				Button4_KeyDown ( Button4 , e );
				e . Handled = true;
				IsTabbing = false;
				return;
			}
			IsTabbing = false;
		}

		private void ReadMsgboxData ( int mode )
		{
			SolidColorBrush sb;
			string input = File . ReadAllText ( @"Messageboxes.dat" );
			string[] fields = input.Split('\n');
			int indx= 0;
			foreach ( var item in fields )
			{
				if ( mode == 1 )
				{
					IsDarkMode = fields [ 12 ] == "DMY" ? true : false;
					return;
				}
				if ( DlgInput . UseDarkMode == false )
				{
					// standard mode load
					switch ( indx++ )
					{
						case 0:
							DlgInput . dlgbackground = Utils . GetNewBrush ( item );
							break;
						case 1:
							DlgInput . dlgforeground = Utils . GetNewBrush ( item );
							break;
						case 2:        // Button background
							DlgInput . btnbackground = Utils . GetNewBrush ( item );
							break;
						case 3:        // Button foreground
							DlgInput . btnforeground = Utils . GetNewBrush ( item );
							break;
						case 4:        // Button border
							DlgInput . Btnborder = Utils . GetNewBrush ( item );
							BorderBrush = DlgInput . Btnborder;
							break;
						case 5:        // Button background (Mouseover)
							DlgInput . Btnmousebackground = Utils . GetNewBrush ( item );
							break;
						case 6:        // Button foreground (Mouseover)
							DlgInput . Btnmouseforeground = Utils . GetNewBrush ( item );
							break;
						case 7:         // DEFAULT Button background (Mouseover)
							DlgInput . defbtnbackground = Utils . GetNewBrush ( item );
							break;
						case 8:         // DEFAULT Button foreground (Mouseover)
							DlgInput . defbtnforeground = Utils . GetNewBrush ( item );
							break;
						case 9:
							string[] flds = item.Split(',');
							DlgInput . thickness = new Thickness ( Convert . ToInt32 ( flds [ 0 ] ) , Convert . ToInt32 ( flds [ 1 ] ) , Convert . ToInt32 ( flds [ 2 ] ) , Convert . ToInt32 ( flds [ 3 ] ) );
							Bordersize = DlgInput . thickness;
							break;
						case 10:
							DlgInput . UseIcon = item == "T" ? true : false;
							break;
						case 11:
							DlgInput . isClean = item == "T" ? true : false;
							break;
						case 12:
							DlgInput . UseDarkMode = item . Contains ( "DMY" ) ? true : false;
							break;
						case 22:       // Def buttons border size
							if ( item == "" )
							{
								Row1 . FontSize = 13;
								Row2 . FontSize = 13;
							}
							else
							{
								Row1 . FontSize = Convert . ToDouble ( item );
								Row2 . FontSize = Convert . ToDouble ( item );
							}
							break;
						case 23:       // Def buttons border size
							if ( item == "" )
								Row2 . FontSize = 13;
							else
								Row2 . FontSize = Convert . ToDouble ( item );
							break;
					}
				}
				else
				{
					// Dark mode load
					switch ( indx++ )
					{
						case 0:
							DlgInput . dlgbackground = Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . DlgBackGroundProperty , DlgInput . dlgbackground );
							break;

						case 1:
							DlgInput . dlgforeground = Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . DlgForeGroundProperty , DlgInput . dlgforeground );
							break;

						case 9:
							string[] flds = item.Split(',');
							DlgInput . thickness = new Thickness ( Convert . ToInt32 ( flds [ 0 ] ) , Convert . ToInt32 ( flds [ 1 ] ) , Convert . ToInt32 ( flds [ 2 ] ) , Convert . ToInt32 ( flds [ 3 ] ) );
							Bordersize = DlgInput . thickness;
							break;
						case 10:
							DlgInput . UseIcon = item == "T" ? true : false;
							break;
						case 11:
							DlgInput . isClean = item == "T" ? true : false;
							break;
						case 12:
							DlgInput . UseDarkMode = item . Contains ( "DMY" ) ? true : false;
							break;
						case 13:        // Button border Dark
							DlgInput . BtnborderDark = Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . BtnBackGroundProperty , DlgInput . BtnborderDark );
							break;

						case 14:        // Button background
							DlgInput . btnbackgroundDark= Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . BtnBackGroundProperty , DlgInput . BtnborderDark );
							break;

						case 15:        // Button foreground
							DlgInput . btnforegroundDark = Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . BtnBackGroundProperty , DlgInput . btnbackgroundDark );
							break;

						case 16:        // Button mouseover shadow
							DlgInput . mouseborderDark = Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . BtnForeGroundProperty , DlgInput . btnforegroundDark );
							break;

						case 17:        // Button mouseover foreground
							DlgInput . mousebackgroundDark = Utils . GetNewBrush ( item );
							Borderbrush = DlgInput . mouseforegroundDark;
							//SetCurrentValue ( Msgbox . BorderColorProperty , DlgInput . BtnborderDark );
							break;

						case 18:        // Button background (Mouseover)
							DlgInput . mouseforegroundDark= Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . MouseoverBackGroundProperty , DlgInput . mousebackgroundDark );
							break;

						case 19:        // Button foreground (Mouseover)
							DlgInput . defbtnbackgroundDark= Utils . GetNewBrush ( item );
							//SetCurrentValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . mouseforegroundDark );
							break;

						case 20:         // DEFAULT Button background (Mouseover)							
							DlgInput.defbtnforegroundDark= Utils . GetNewBrush ( item );
							break;

						case 21:       // Def buttons border size
							flds = item . Split ( ',' );
							Thickness th = new Thickness ( Convert . ToInt32 ( flds [ 0 ] ) , Convert . ToInt32 ( flds [ 1 ] ) , Convert . ToInt32 ( flds [ 2 ] ) , Convert . ToInt32 ( flds [ 3 ] ) );
							DlgInput . BorderSizeDefault = th;
							break;
						case 22:       // Def buttons border size
							if ( item == "" )
							{
								Row1 . FontSize = 13;
								Row2 . FontSize = 13;
							}
							else
							{
								Row1 . FontSize = Convert . ToDouble ( item );
								Row2 . FontSize = Convert . ToDouble ( item );
							}
							break;
						case 23:       // Def buttons border size
							if ( item == "" )
								Row2 . FontSize = 13;
							else
								Row2 . FontSize = Convert . ToDouble ( item );
							break;
					}
				}

			}


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
						+ $"Def Btn Border Size :			[{DlgInput . BorderSizeDefault . Top}, {DlgInput . BorderSizeDefault . Left},{DlgInput . BorderSizeDefault . Right},{DlgInput . BorderSizeDefault . Bottom}],\n\n"
						);

			updateVars ( );
		}
		public static void updateVars ( )
		{
			//Btnbackground = DlgInput . btnbackground;
			//Btnforeground = DlgInput . btnforeground;
			//BtnMbackground = DlgInput . mousebackground;
			//BtnMforeground = DlgInput . mouseforeground;
			//DefBtnbackground = DlgInput . defbtnbackground;
			//DefBtnforeground = DlgInput . defbtnforeground;
		}
		public void UpdateDialog ( )
		{
			// Dialog settings
			BoxGrid . Background = DlgInput . dlgbackground;
			//			Caption . Background = DlgInput . dlgbackground;
			Row1 . Background = DlgInput . dlgbackground;
			Row2 . Background = DlgInput . dlgbackground;
			Row1 . Foreground = DlgInput . dlgforeground;
			Row2 . Foreground = DlgInput . dlgforeground;

			#region Button update colors
			//Button settings
			if ( DefaultButton != Button1 )
			{
				Button1 . Background = DlgInput . btnbackground;
				Button1 . Foreground = DlgInput . btnforeground;
			}
			else
			{
				Button1 . Background = DlgInput . defbtnbackground;
				Button1 . Foreground = DlgInput . defbtnforeground;
			}
			Button1 . BorderBrush = DlgInput . Btnborder;

			if ( DefaultButton != Button2 )
			{
				Button2 . Background = DlgInput . btnbackground;
				Button2 . Foreground = DlgInput . btnforeground;
			}
			else
			{
				Button2 . Background = DlgInput . defbtnbackground;
				Button2 . Foreground = DlgInput . defbtnforeground;
			}
			Button2 . BorderBrush = DlgInput . Btnborder;
			if ( DefaultButton != Button3 )
			{
				Button3 . Background = DlgInput . btnbackground;
				Button3 . Foreground = DlgInput . btnforeground;
			}
			else
			{
				Button3 . Background = DlgInput . defbtnbackground;
				Button3 . Foreground = DlgInput . defbtnforeground;
			}
			Button3 . BorderBrush = DlgInput . Btnborder;
			if ( DefaultButton != Button4 )
			{
				Button4 . Background = DlgInput . btnbackground;
				Button4 . Foreground = DlgInput . btnbackground;
			}
			else
			{
				Button4 . Background = DlgInput . defbtnbackground;
				Button4 . Foreground = DlgInput . defbtnforeground;
			}
			Button4 . BorderBrush = DlgInput . Btnborder;
			Button2 . Refresh ( );
			Button3 . Refresh ( );
			Button4 . Refresh ( );

			#endregion Button update colors

			this . UpdateLayout ( );
			this . Refresh ( );
		}

		#region RoutedEvents

		//********************//
		// ProcessOK event
		//********************//
		// Create a custom routed event by first registering a RoutedEventID
		// This event uses the bubbling routing strategy
		public static readonly RoutedEvent ProcessOKEvent = EventManager.RegisterRoutedEvent(
	  "ProcessOKs", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Msgbox));

		// Provide CLR accessors for the event
		public event RoutedEventHandler ProcessOK
		{
			add { AddHandler(ProcessOKEvent, value); }
			remove { RemoveHandler(ProcessOKEvent, value); }
		}
		// This method raises the ProcessYes  event
		void RaiseProcessOKEvent ( )
		{
			RoutedEventArgs newEventArgs = new RoutedEventArgs(Msgbox.ProcessOKEvent);
			RaiseEvent ( newEventArgs );
		}

		//********************//
		// ProcessYes event
		//********************//
		// Create a custom routed event by first registering a RoutedEventID
		// This event uses the bubbling routing strategy
		public static readonly RoutedEvent ProcessYesEvent = EventManager.RegisterRoutedEvent(
	  "ProcessYess", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Msgbox));

		// Provide CLR accessors for the event
		public event RoutedEventHandler ProcessYes
		{
			add { AddHandler(ProcessYesEvent, value); }
			remove { RemoveHandler(ProcessYesEvent, value); }
		}
		// This method raises the ProcessYes  event
		void RaiseProcessYesEvent ( )
		{
			RoutedEventArgs newEventArgs = new RoutedEventArgs(Msgbox.ProcessYesEvent);
			RaiseEvent ( newEventArgs );
		}

		//********************//
		// ProcessNo event	  		
		//********************//

		// Create a custom routed event by first registering a RoutedEventID
		// This event uses the bubbling routing strategy
		public static readonly RoutedEvent ProcessNoEvent = EventManager.RegisterRoutedEvent(
	  "ProcessNos", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Msgbox));

		// Provide CLR accessors for the event
		public event RoutedEventHandler ProcessNo
		{
			add { AddHandler(ProcessNoEvent, value); }
			remove { RemoveHandler(ProcessNoEvent, value); }
		}
		// This method raises the Tap event
		void RaiseProcessNoEvent ( )
		{
			RoutedEventArgs newEventArgs = new RoutedEventArgs(Msgbox.ProcessNoEvent);
			RaiseEvent ( newEventArgs );
		}

		//********************//
		// ProcessCancel event
		//********************//

		// Create a custom routed event by first registering a RoutedEventID
		// This event uses the bubbling routing strategy
		public static readonly RoutedEvent ProcessCancelEvent = EventManager.RegisterRoutedEvent(
	  "ProcessCancels", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Msgbox));

		// Provide CLR accessors for the event
		public event RoutedEventHandler ProcessCancel
		{
			add { AddHandler(ProcessCancelEvent, value); }
			remove { RemoveHandler(ProcessCancelEvent, value); }
		}
		// This method raises the Tap event
		void RaiseProcessCancelEvent ( )
		{
			RoutedEventArgs newEventArgs = new RoutedEventArgs(Msgbox.ProcessCancelEvent);
			RaiseEvent ( newEventArgs );
		}
		#endregion RoutedEvents

		#region Routed Event handlers ?
		public RoutedEvent ProcessOKBtn ( )
		{
			Console . WriteLine ( "OK button clicked" );
			Dlgresult . returnint = 1;
			DlgInput . MsgboxSmallWin = null;
			this . Close ( );
			return null;
		}
		public RoutedEvent ProcessYesBtn ( )
		{
			Console . WriteLine ( "Yes button clicked" );
			Dlgresult . returnint = 2;
			DlgInput . MsgboxSmallWin = null;
			this . Close ( );
			return null;
		}
		public RoutedEvent ProcessNoBtn ( )
		{
			Console . WriteLine ( "No button clicked" );
			Dlgresult . returnint = 3;
			Dlgresult . returnstring = "a string - honestly";
			Dlgresult . returnerror = "No Error";
			Dlgresult . result = false;
			GenericClass v = DlgInput.obj as GenericClass;
			//			if ( v != null )
			//				Console . WriteLine ( $"genericClass object :{v . field1}..." );
			//			else
			//				Console . WriteLine ( "Not  a genericClass object..." );
			ObservableCollection < GenericClass > vv= DlgInput . obj as ObservableCollection< GenericClass>;
			//if ( vv != null )
			//	Console . WriteLine ( $"ObservableColllection identified : count = {vv . Count}" );
			//else
			//	Console . WriteLine ( "Not  a ObservableColllection object..." );
			Dlgresult . obj = DlgInput . obj;
			DlgInput . MsgboxSmallWin = null;
			this . Close ( );
			return null;
		}
		public RoutedEvent ProcessCancelBtn ( )
		{
			Console . WriteLine ( "Cancel button clicked" );
			Dlgresult . returnint = 4;
			DlgInput . MsgboxSmallWin = null;
			this . Close ( );
			return null;
		}
		#endregion Routed Event handlers ?

		private void Button1_MouseEnter ( object sender , MouseEventArgs e )
		{
			Button  b = sender as Button;
			if ( b . Name . Contains ( "1" ) )
			{
				if ( b == Button1 )
					SetBtnStatus ( b , true , true );
				else
					SetBtnStatus ( b , false , true );
			}
			else if ( b . Name . Contains ( "2" ) )
			{
				if ( b == Button2 )
					SetBtnStatus ( b , true , true );
				else
					SetBtnStatus ( b , false , true );
			}
			else if ( b . Name . Contains ( "3" ) )
			{
				if ( b == Button3 )
					SetBtnStatus ( b , true , true );
				else
					SetBtnStatus ( b , false , true );
			}
			else if ( b . Name . Contains ( "4" ) )
			{
				if ( b == Button4 )
					SetBtnStatus ( b , true , true );
				else
					SetBtnStatus ( b , false , true );
			}
			b . UpdateLayout ( );
		}

		private void Button_MouseEnter ( object sender , MouseEventArgs e )
		{
			Button bttn = sender as Button;
			SetBtnStatus ( bttn , bttn == DefaultButton , true );
		}

		private void Button_MouseLeave ( object sender , MouseEventArgs e )
		{
			Button bttn = sender as Button;
			SetBtnStatus ( bttn , bttn == DefaultButton , false );
		}

		private void SetBtnStatus ( Button Btn , bool isdef , bool ismouseover )
		{
			Thickness th = new Thickness();
			if ( IsTabbing )
			{
				if ( isdef == false )
				{     // Leaving a normal button, Reset to standard colors ?
					if ( TabPass == 1 )
					{
						// Resetting from being  the focused button
						// mouse is over button
						Btn . Background = Btnbackground;
						Btn . BorderBrush = Borderbrush;
						Btn . BorderThickness = Bordersize;
					}
					else if ( TabPass == 2 )
					{
						// Setting up focus of NEW  Default button
						// mouse is over button
						Btn . Background = DefBtnbackground;
						Btn . BorderBrush = Borderbrush;
						Btn . BorderThickness = MouseBordersize;
					}
				}
				else
				{
					// LEAVING a default button due to keystroke
					//Set back to DEFAULT setting
					if ( TabPass == 1 )
					{
						Btn . Background = Btnbackground;
						Btn . BorderBrush = Borderbrush;
						Btn . BorderThickness = Bordersize;
					}
					else
					{
						Btn . Background = DefBtnbackground;
						Btn . BorderBrush = Borderbrush;
						Btn . BorderThickness = Bordersize;
					}
				}
			}
			else if ( ismouseover )
			{
				// Mouseover/Default settings
				if ( isdef )
				{
					// Only used by DEFAULT  button
					// mouse has entered and is over a Default button
					if ( ismouseover )
					{
						Btn . Background = BtnMbackground;
						Btn . BorderBrush = BorderBrush;
						Btn . Foreground = DlgInput . Btnmouseforeground;
						Btn . BorderThickness = MouseBordersize;
					}
					else
					{
						// This is the  DEFAULT BUTTONs standard setting
						Btn . BorderThickness = Bordersize;
					}
				}
				else
				{
					// Mouse ENTERING a non default button
					//WORKING
					// Set it default colors
					Btn . Background = BtnMbackground;
					Btn . BorderBrush = BorderBrush;
					Btn . Foreground = DlgInput . Btnmouseforeground;
					Btn . BorderThickness = MouseBordersize;
				}
			}
			else
			{
				//Called when mouse leaves button
				if ( isdef == false )
				{
					// Mouse EXITING a non default button
					//WORKING
					Btn . Background = Btnbackground;
					Btn . BorderBrush = BorderBrush;
					Btn . Foreground = Btnforeground;
					Btn . BorderThickness = Bordersize;
				}
				else
				{
					// LEAVING Button
					// This IS the a default button
					if ( DlgInput . UseDarkMode )
					{
						Btn . Background = DlgInput . btnbackgroundDark;
						Btn . BorderBrush = DlgInput . mouseborderDark;
						Btn . Foreground = DlgInput . btnforegroundDark;
						Btn . BorderThickness = DlgInput . BorderSizeNormal;
					}
					else
					{
						Btn . Background = DlgInput . defbtnbackground;
						Btn . BorderBrush = DlgInput . Btnborder;
						Btn . Foreground = DlgInput . defbtnforeground;
						Btn . BorderThickness = DlgInput . BorderSizeNormal;
					}
				}
			}
			Btn . UpdateLayout ( );
		}
		private void msgbox_SizeChanged ( object sender , SizeChangedEventArgs e )
		{
			Size  size = e.NewSize;
			if ( size . Width >= this . MaxWidth )
			{
				e . Handled = true;
				return;
			}
			if ( size . Height >= this . MaxHeight )
			{
				e . Handled = true;
				return;
			}
		}

	}
}

