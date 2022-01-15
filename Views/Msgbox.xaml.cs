
using Microsoft . SqlServer . Management . Dmf;
using Microsoft . SqlServer . Management . Smo;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . IO;
using System . Runtime . Remoting . Channels;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Converters;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;

using WPFPages . AttachedProperties;
using WPFPages . Converts;
using WPFPages . Views;

using static System . Net . Mime . MediaTypeNames;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for Msgbox.xaml
	/// </summary>
	public partial class Msgbox : Window
	{
		System . Drawing . Image image { set; get; }
		public static string Iconstring { get; set; }
		//public Thickness bordersize { get; set; }
		//public Thickness borderSizeDefault { get; set; }

		#region rowdata

		string CaptionString { get; set; }
		string Row1String { get; set; }
		string Row2String { get; set; }
		string Row3String { get; set; }

		#endregion rowdata

		#region button text data Properties
		private string Btn1Text { get; set; }
		private string Btn2Text { get; set; }
		private string Btn3Text { get; set; }
		private string Btn4Text { get; set; }


		#endregion buttondata

		#region General declarations (Brushes and button arrays)
		//private Brush Borderbrush { get; set; }
		//private Brush BorderbrushDefault { get; set; }
		//static Brush Btnbackground;               // Std button Background
		//static Brush Btnforeground;               // Std button Foreground
		//static Brush BtnMbackground;        // Mouseover button Background
		//static Brush BtnMforeground;        // Mouseover button Foreground
		//static Brush DefBtnbackground;        // Defaullt Button Background
		//static Brush DefBtnforeground;         // Default Button Foreground


		private int[] btnsarray = new int[4];
		private string [ ] btnstextarray = { "","","",""};

		private bool  IsTabbing=false;
		private int  CurrentButton = 0;
		private int  DefaultButton = 0;
		private Border DefBorder;
		private bool isShiftDown = false;
		private int TabPass=0;

		#endregion General declarations

		#region keyhits
		public static string key1 { get; set; }
		public static string key2 { get; set; }
		public static string key3 { get; set; }
		public static string key4 { get; set; }
		#endregion keyhits

		#region Setup
		public Msgbox ( ) { }

		public Msgbox (
				string caption ,
				string string1 ,
				string string2 = "" ,
				string string3 = "" ,
				string title = "" ,
				string iconstring = "" ,
				int defButton = 1 ,
				int Btn1 = 1 ,
				int Btn2 = 0 ,
				int Btn3 = 0 ,
				int Btn4 = 0 ,
				string btn1Text = "OK" ,
				string btn2Text = "" ,
				string btn3Text = "" ,
				string btn4Text = ""

				)
		{
			InitializeComponent ( );
			Utils . SetupWindowDrag ( this );
			//			if ( DlgInput . resetdata )
			//				MainWindow . ResetMsgBox ( );
			//Already handled
			//			MainWindow . ReadMsgboxData (  );
			//			ConfigureCurrentColors ( );
			LoadWindow (
			 caption ,
			string1 ,
			string2 ,
			string3 ,
			title ,
			iconstring ,
			 defButton ,
			 Btn1 ,
			 Btn2 ,
			 Btn3 ,
			 Btn4 ,
			 btn1Text ,
			 btn2Text ,
			 btn3Text ,
			 btn4Text
				);
			DlgInput . MsgboxWin = this;
		}
		private void LoadWindow (
			string caption ,
			string string1 ,
			string string2 ,
			string string3 ,
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
			string btn4Text
				)
		{
			// get data for current configuration parameters from disk file

			ReadMsgboxData ( 1 );
			ReadMsgboxData ( 2 );
			CheckForInitialDefaultSettings ( );
			Iconstring = iconstring;
			DlgInput . MsgboxWin = this;
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
			object obj = DlgInput . obj;
			// set up the buttons text
			if ( caption != "" )
				Caption . Text = caption;

			#region setup button text
			if ( btn1Text != "" ) // OK
			{
				Btn1Text = btn1Text;
				key1 = btn1Text . Substring ( 0 , 1 );
				btnstextarray [ 0 ] = btn1Text;
			}
			else
			{
				Btn1Text = "Ok";
				btn1Text = Btn1Text;
				key1 = "O";
				btnstextarray [ 0 ] = Btn1Text;
			}

			if ( btn2Text != "" )   // YES`
			{
				Btn2Text = btn2Text;
				key2 = btn2Text . Substring ( 0 , 1 );
				btnstextarray [ 1 ] = btn2Text;
			}
			else
			{
				Btn2Text = "Yes";
				btn2Text = Btn2Text;
				key2 = "Y";
				btnstextarray [ 1 ] = Btn2Text;
			}

			if ( btn3Text != "" )   // NO``
			{
				Btn3Text = btn3Text;
				key3 = btn3Text . Substring ( 0 , 1 );
				btnstextarray [ 2 ] = btn3Text;
			}
			else
			{
				Btn3Text = "No";
				btn3Text = Btn3Text;
				key3 = "N";
				btnstextarray [ 2 ] = Btn3Text;
			}

			if ( btn4Text != "" )         // Cancel
			{
				Btn4Text = btn4Text;
				key4 = btn4Text . Substring ( 0 , 1 );
				btnstextarray [ 3 ] = btn4Text;
			}
			else
			{
				Btn4Text = "Cancel";
				btn4Text = Btn4Text;
				key4 = "C";
				btnstextarray [ 3 ] = Btn4Text;
			}

			#endregion setup button text

			CurrentButton = defButton;
			DefaultButton = CurrentButton;

			// Set up information strings 
			Row1String = string1;
			Row2String = string2;
			Row3String = string3;
			Row1 . Text = Row1String;
			Row2 . Text = Row2String;
			Row3 . Text = Row3String;

			//Set Title bar
			this . Title = title == "" ? this . Title : title;
			Button1 . Visibility = Visibility . Collapsed;
			Button2 . Visibility = Visibility . Collapsed;
			Button3 . Visibility = Visibility . Collapsed;
			Button4 . Visibility = Visibility . Collapsed;
			// now decide which buttons are to be shown from a total of 4
			for ( int i = 0 ; i < 4 ; i++ )
			{
				switch ( i )
				{
					case 0:
						if ( btnsarray [ i ] == Btn1 && btnsarray [ i ] != 0 )
							SetUpButtons ( btnsarray [ i ] , btnstextarray [ i ] , defButton );
						break;
					case 1:
						if ( btnsarray [ i ] == Btn2 && btnsarray [ i ] != 0 )
							SetUpButtons ( btnsarray [ i ] , btnstextarray [ i ] , defButton );
						break;
					case 2:
						if ( btnsarray [ i ] == Btn3 && btnsarray [ i ] != 0 )
							SetUpButtons ( btnsarray [ i ] , btnstextarray [ i ] , defButton );
						break;
					case 3:
						if ( btnsarray [ i ] == Btn4 && btnsarray [ i ] != 0 )
							SetUpButtons ( btnsarray [ i ] , btnstextarray [ i ] , defButton );
						break;
				}
			}
			// Check  for iconstring !!
			CheckForFinalDefaultSettings ( Iconstring );
			string path = "";
			// sort out the icon
			string icostr = Iconstring . ToUpper ( );
			if ( icostr . Contains ( "EXCLAIM" ) || icostr . Contains ( "GREEN" ) )
				HiliteBorder . BorderBrush = FindResource ( "Green6" ) as SolidColorBrush;
			
			else if ( icostr  . Contains ( "ERROR" ) || icostr . Contains ( "RED" ))
			{
				HiliteBorder . BorderBrush = FindResource ( "Red6" ) as SolidColorBrush;
				Caption.Foreground = FindResource ( "White0" ) as SolidColorBrush;
			}
			
			else if ( icostr . Contains ( "WARN" ) || icostr . Contains ( "YELLOW" ) )
				HiliteBorder . BorderBrush = FindResource ( "Yellow0" ) as SolidColorBrush;
			
			else if ( icostr . Contains ( "INFO" ) || icostr . Contains ( "BLUE" ))
				HiliteBorder . BorderBrush = FindResource ( "Blue6" ) as SolidColorBrush;

			Caption . Background = HiliteBorder . BorderBrush;
//			BoxIcon . Source = new BitmapImage ( new Uri ( Iconstring , UriKind . RelativeOrAbsolute ) );
//			BoxIcon . UpdateLayout ( );

			//if ( icostr . Contains ( "INFO" ) || icostr. ToUpper ( ) . Contains ( "EXCLAIM" ) )
			//	Iconstring = "INFO";
			//else if ( icostr . Contains ( "WARN" ) )
			//	Iconstring = "WARN";
			//else if ( icostr . Contains ( "ERROR" ) )
			//	Iconstring = "INFOERROR";
//			SetupCaptionColor ( Iconstring );
//			BoxIcon . Visibility = Visibility . Visible;

			this . MouseWheel += Custom_MouseWheel;
			MouseMove += Grab_MouseMove;
			KeyDown += Window_PreviewKeyDown;
//			BoxIcon . Visibility = Visibility . Visible;
		}

		private void SetupCaptionColor ( string errtype )
		{
			if ( errtype . ToUpper ( ) . Contains ( "ERROR" ) )
				Caption . Background = new SolidColorBrush ( Colors . Red );
			else if ( errtype . ToUpper ( ) . Contains ( "INFO" ) )
				Caption . Background = FindResource( "Green6") as SolidColorBrush; 
			else if ( errtype . ToUpper ( ) . Contains ( "WARN" ) )
				Caption . Background = new SolidColorBrush ( Colors . Orange );
		}


		#endregion Setup

		//Set up the basic Dlg colors etc
		#region initialization
		public void CheckForInitialDefaultSettings ( )
		{

			Console . WriteLine ( $"Processing CheckForInitialDefaultSettings in Msgbox.cs...." );
			if ( DlgInput . UseDarkMode )
			{
				// always set these fr dark mode
				DlgInput . dlgbackground = "#F5374340" . ToSolidBrush ( );         // off Black
				DlgInput . dlgforeground = FindResource ( "White0" ) as Brush;
				if ( DlgInput . Btnborder == null )
					DlgInput . Btnborder = FindResource ( "Yellow1" ) as Brush;
				if ( DlgInput . Btnmouseforeground == null )
					DlgInput . Btnmouseforeground = FindResource ( "White0" ) as Brush;
				if ( DlgInput . btnbackground == null )
					DlgInput . btnbackground = FindResource ( "Red1" ) as Brush;
				if ( DlgInput . btnforeground == null )
					DlgInput . btnforeground = FindResource ( "White0" ) as Brush;
				//				DlgInput . Btnborder = "#FD09CACE" . ToSolidBrush ( ); // Cyan

				//DlgInput . Btnborder = FindResource ( "Yellow1" ) as Brush;
				//DlgInput . dlgbackground = "#FF000000" . ToSolidBrush ( );
				//DlgInput . mouseforeground = FindResource ( "White0" ) as Brush;
				//if ( DlgInput . btnforeground == null )
				//	DlgInput . btnforeground = FindResource ( "White0" ) as Brush;
				//if ( DlgInput . Btnborder == null )
				//	DlgInput . Btnborder = FindResource ( "White0" ) as Brush;
				//if ( DlgInput . mousebackground == null )
				//	DlgInput . mousebackground = FindResource ( "Black3" ) as Brush;
			}
			else
			{
				// Set defaults in case the config file is missng/corrupted
				if ( DlgInput . dlgbackground == null )
					DlgInput . dlgbackground = FindResource ( "White6" ) as Brush;
				if ( DlgInput . Btnmouseforeground == null )
					DlgInput . Btnmouseforeground = FindResource ( "Black0" ) as Brush;
				//DlgInput . border = FindResource ( "Black0" ) as Brush;
				if ( DlgInput . btnbackground == null )
					DlgInput . btnbackground = FindResource ( "Red1" ) as Brush;
				if ( DlgInput . btnforeground == null )
					DlgInput . btnforeground = FindResource ( "White0" ) as Brush;
				if ( DlgInput . Btnborder == null )
					DlgInput . Btnborder = FindResource ( "White0" ) as Brush;
				if ( DlgInput . Btnmousebackground == null )
					DlgInput . Btnmousebackground = FindResource ( "Black3" ) as Brush;
			}
			//Borderbrush = DlgInput . Btnborder;
			//BorderBrush = DlgInput . Btnborder;
			//Btnbackground = DlgInput . btnbackground;
			//Btnforeground = DlgInput . btnforeground;
			//// mouse over colors
			//BtnMbackground = DlgInput . Btnmousebackground;
			//BtnMforeground = DlgInput . Btnmouseforeground;

			//DefBtnbackground = DlgInput . defbtnbackground;
			//DefBtnforeground = DlgInput . defbtnforeground;

			this . Background = DlgInput . dlgbackground;
			BoxGrid . Background = DlgInput . dlgbackground;

			Row1 . Foreground = DlgInput . dlgforeground;
			Row2 . Foreground = DlgInput . dlgforeground;
			Row3 . Foreground = DlgInput . dlgforeground;
			if ( DlgInput . UseDarkMode )
			{
				// special background for Dark mode
				Row1 . Background = DlgInput . dlgbackground;
				Row2 . Background = DlgInput . dlgbackground;
				Row3 . Background = DlgInput . dlgbackground;
				//				Caption . Background = DlgInput . dlgbackground;
				// Dialog box main text color
				Row1 . Foreground = DlgInput . dlgforeground;
				Row2 . Foreground = DlgInput . dlgforeground;
				Row3 . Foreground = DlgInput . dlgforeground;
			}
			else
			{
				// Dialog box main text color
				Row1 . Foreground = DlgInput . dlgforeground;
				Row2 . Foreground = DlgInput . dlgforeground;
				Row3 . Foreground = DlgInput . dlgforeground;
			}

			Button1 . Background = DlgInput . btnbackground;
			Button2 . Background = DlgInput . btnbackground;
			Button3 . Background = DlgInput . btnbackground;
			Button4 . Background = DlgInput . btnbackground;

			BorderBrush = DlgInput . BtnborderDark;
			Button1 . BorderBrush = DlgInput . BtnborderDark;
			Button2 . BorderBrush = DlgInput . BtnborderDark;
			Button3 . BorderBrush = DlgInput . BtnborderDark;
			Button4 . BorderBrush = DlgInput . BtnborderDark;

			//defaullt buttons
			Button1 . BorderThickness = DlgInput . BorderSizeNormal;
			Button3 . BorderThickness = DlgInput . BorderSizeNormal;
			Button2 . BorderThickness = DlgInput . BorderSizeNormal;
			Button4 . BorderThickness = DlgInput . BorderSizeNormal;


			Button1Text . Foreground = DlgInput . btnforeground;
			Button2Text . Foreground = DlgInput . btnforeground;
			Button3Text . Foreground = DlgInput . btnforeground;
			Button4Text . Foreground = DlgInput . btnforeground;

			Button1 . Refresh ( );
			Button2 . Refresh ( );
			Button3 . Refresh ( );
			Button4 . Refresh ( );
			this . Refresh ( );
		}
		private void CheckForFinalDefaultSettings ( string Iconstring )
		{
			if ( DlgInput . UseIcon && Iconstring == "" )
			{
				if ( DlgInput . iconstring != "" )
					Iconstring = DlgInput . iconstring;
				else
					DlgInput . iconstring = "\\icons\\check-mark-icon-5375.png";
				BoxIcon . Source = new BitmapImage ( new Uri ( Iconstring , UriKind . Relative ) );
				BoxIcon . Visibility = Visibility . Visible;
			}
			else if ( DlgInput . UseIcon && Iconstring != "" )
			{
				BoxIcon . Source = new BitmapImage ( new Uri ( Iconstring , UriKind . Relative ) );
				BoxIcon . Visibility = Visibility . Visible;
				BoxIcon . Height = 85;
				BoxIcon . Width = 85;
			}
			else if ( DlgInput . UseIcon == false)
				BoxIcon . Visibility = Visibility . Collapsed;
			else
				BoxIcon . Visibility = Visibility . Collapsed;
		}

		#endregion initialization

		#region main button color handling
		/// <summary>
		/// Does the hard work of initial configuring each button
		/// </summary>
		/// <param name="Btn"></param>
		/// <param name="btntext"></param>
		/// <param name="defButton"></param>
		private void SetUpButtons ( int Btn , string btntext , int defButton )
		{
			if ( defButton == Btn )
			{
				DefaultButton = Btn;
			}
			if ( Btn >= 0 && Btn <= 4 )
			{
				if ( Btn == 1 )   // OK Button  && DEFAULT button
				{
					if ( Btn1Text != null )
						Button1Text . Text = Btn1Text != "" ? Btn1Text : "OK";
					if ( DefaultButton == Btn )
						DefBorder = Button1;
					UpdateButtonX ( Btn , Button1 , false );
				}
				else if ( Btn == 2 )  // YES Button
				{
					if ( Btn2Text != null )
						Button2Text . Text = Btn2Text != "" ? Btn2Text : "Yes";
					if ( DefaultButton == Btn )
						DefBorder = Button2;
					UpdateButtonX ( Btn , Button2 , false );
				}
				else if ( Btn == 3 )   // NO Button
				{
					if ( Btn3Text != null )
						Button3Text . Text = Btn3Text != "" ? Btn3Text : "No";
					if ( DefaultButton == Btn )
						DefBorder = Button3;
					UpdateButtonX ( Btn , Button3 , false );
				}
				else if ( Btn == 4 )   // CANCEL Button
				{
					if ( Btn4Text != null )
						Button4Text . Text = Btn4Text != "" ? Btn4Text : "Cancel";
					if ( DefaultButton == Btn )
						DefBorder = Button4;
					UpdateButtonX ( Btn , Button4 , false );
				}
			}
		}

		private void UpdateButtonX ( int btn , Border Btn , bool istabbing )
		{
			Btn . Height = 35;
			Btn . Width = 90;
			Btn . Visibility = Visibility . Visible;

			// set the gap between buttons
			Thickness th = new Thickness();
			th . Right = 10;
			Btn . Margin = th;

			if ( IsTabbing )
			{
				// Called ONCE when key is pressed
				if ( btn == DefaultButton )       // we are LEAVING a default button
				{
					// Clear current button
					if ( TabPass == 0 )
						TabPass = 1;
					else if ( TabPass == 1 )
						TabPass = 2;
					else if ( TabPass == 2 )
						TabPass = 0;
					// Reset default to new  button
					DefaultButton = 0;
					SetBtnStatus ( btn , Btn , true , IsMouseOver );
					if ( TabPass == 2 )
						TabPass = 0;
				}
				else
				{
					// Called when tabbing away from normal buttons - 
					if ( TabPass == 0 )
						TabPass = 1;
					else if ( TabPass == 1 )
						TabPass = 2;
					else if ( TabPass == 2 )
						TabPass = 0;
					// Reset default to new  button
					DefaultButton = btn;
					SetBtnStatus ( btn , Btn , false , IsMouseOver );
					if ( TabPass == 2 )
						TabPass = 0;
				}
			}
			else             // Not tabbing
			{
				if ( CurrentButton == btn && DefaultButton == btn )                 // Def
					SetBtnStatus ( btn , Btn , true , IsTabbing ? false : IsMouseOver );
				else if ( CurrentButton == btn && DefaultButton != btn )
					SetBtnStatus ( btn , Btn , false , IsTabbing ? true : IsMouseOver );
				else if ( CurrentButton != btn && DefaultButton == btn )
					SetBtnStatus ( btn , Btn , true , IsTabbing ? false : IsMouseOver );
				else if ( CurrentButton != btn && DefaultButton != btn )
					SetBtnStatus ( btn , Btn , false , IsTabbing ? false : IsMouseOver );
				else
					SetBtnStatus ( btn , Btn , false , IsTabbing ? false : IsMouseOver );
				// Set the buttons up so they operate nicely
				Btn . Refresh ( );
				Btn . UpdateLayout ( );
			}
		}
		// Handle each buttons visual effcts
		private void SetBtnStatus ( int btnNum , Border Btn , bool isdef , bool ismouseover )
		{
			Thickness th = new Thickness();
			if ( IsTabbing )
			{
				//************************//
				// NORMAL BUTTONS
				//************************//
				if ( isdef == false )
				{     // Leaving a normal button, Reset to standard colors ?
					if ( TabPass == 1 )
					{
						// Resetting to Normal from being  the focused button
						Btn . Background = DlgInput . Btnmousebackground;
						Btn . BorderBrush = DlgInput . Btnborder;
						SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
						// mouse is over button
						Btn . BorderThickness = DlgInput . BorderSizeNormal;
					}
					else if ( TabPass == 2 )
					{
						// Setting up focus of NEW  Default button
						Btn . Background = DlgInput . defbtnbackground;
						Btn . BorderBrush = DlgInput . Btnborder;
						SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
						// mouse is over button
						Btn . BorderThickness = DlgInput . BorderSizeDefault;
					}
				}
				else
				{
					// LEAVING a default button due to keystroke
					//Set back to NON DEFAULT setting
					if ( TabPass == 1 )
					{
						Btn . Background = DlgInput . btnbackground;
						Btn . BorderBrush = DlgInput . Btnborder;
						SetButtonForeground ( Btn , false , ismouseover );
						Btn . BorderThickness = DlgInput . BorderSizeDefault;
					}
					else
					{
						Btn . Background = DlgInput . btnbackground;
						Btn . BorderBrush = DlgInput . Btnborder;
						SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
						Btn . BorderThickness = DlgInput . BorderSizeDefault;
					}
				}
			}
			else if ( ismouseover )
			{
				//**************************//
				// DEFAULT  BUTTONS
				//**************************//
				if ( isdef )
				{
					// Only used by DEFAULT  button  when mouse is over it 
					if ( DlgInput . UseDarkMode )
					{
						Btn . Background = DlgInput . mousebackgroundDark;
						Btn . BorderBrush = DlgInput . Btnborder;
						Button1Text . Foreground = DlgInput . mouseforegroundDark;
					}
					else
					{
						Btn . Background = DlgInput . Btnmousebackground;
						Btn . BorderBrush = DlgInput . Btnborder;
						Button1Text . Foreground = DlgInput .Btnmouseforeground;
					}
					//Btn . Background = DlgInput . defbtnbackground;
					SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
					if ( ismouseover )
					{
						// mouse has entered and is over a Default button
						Btn . BorderThickness = DlgInput . BorderSizeDefault;
					}
					else
					{
						// mouse has left Default button
						// This is the  DEFAULT BUTTON standard setting
						Btn . BorderThickness = DlgInput . BorderSizeNormal;
					}
				}
				else
				{
					// Mouse ENTERING a non default button
					//WORKING					
					Btn . Background = DlgInput . Btnmousebackground;
					Btn . BorderBrush = DlgInput . Btnborder;
					Button1Text . Foreground = DlgInput . Btnmouseforeground;
					SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
					Btn . BorderThickness = DlgInput . BorderSizeDefault;
				}
			}
			else
			{
				// Not Mouse over
				//Called when mouse leaves button
				if ( isdef == false )
				{
					// Mouse EXITING a non default button
					//WORKING
					Btn . Background = DlgInput . btnbackground;
					Btn . BorderBrush = DlgInput . BtnborderDark;
					SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
					Btn . BorderThickness = DlgInput . BorderSizeNormal;
				}
				else
				{
					//It  is the default button
					Btn . Background = DlgInput . defbtnbackground;
					Btn . BorderBrush = DlgInput . Btnborder;
					SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
					Btn . BorderThickness = DlgInput . BorderSizeNormal;
				}
			}
			Btn . UpdateLayout ( );
		}

		private void SetButtonForeground ( Border brdr , bool isdefault , bool ismouseover )
		{
			if ( brdr . Name . Contains ( "1" ) )
			{
				if ( DefaultButton == 0 )
				{
					// We are just resetting to normal
					Button1Text . Foreground = DlgInput . Btnmouseforeground;

				}
				else if ( DefaultButton == 1 )
				{
					// IS the defaullt button
					if ( ismouseover )
						Button1Text . Foreground = DlgInput . Btnmouseforeground;
					else
						Button1Text . Foreground = DlgInput . defbtnforeground;
				}
				else
				{
					// Not the defaullt button
					if ( ismouseover )
						Button1Text . Foreground = DlgInput . defbtnforeground;
					else
						Button1Text . Foreground = DlgInput . btnforegroundDark;
				}
			}
			else if ( brdr . Name . Contains ( "2" ) )
			{
				if ( DefaultButton == 0 )
				{     // We are just resetting to normal
					Button2Text . Foreground = DlgInput . Btnmouseforeground;
				}
				else if ( DefaultButton == 2 )
				{
					// IS the defaullt button
					if ( ismouseover )
						Button2Text . Foreground = DlgInput . defbtnforeground;
					else
						Button2Text . Foreground = DlgInput . Btnmouseforeground;
				}
				else
				{
					// Not the defaullt button
					if ( ismouseover )
						Button2Text . Foreground = DlgInput . Btnmouseforeground;
					else
						Button2Text . Foreground = DlgInput . btnforegroundDark;
				}
			}
			else if ( brdr . Name . Contains ( "3" ) )
			{
				if ( DefaultButton == 0 )
				{
					// We are just resetting to normal
					Button3Text . Foreground = DlgInput . Btnmouseforeground;
				}
				else if ( DefaultButton == 3 )
				{
					// IS the defaullt button
					if ( ismouseover )
						Button3Text . Foreground = DlgInput . defbtnforeground;
					else
						Button3Text . Foreground = DlgInput . btnforeground;
				}
				else
				{
					// Not the defaullt button
					if ( ismouseover )
						Button3Text . Foreground = DlgInput . Btnmouseforeground;
					else
						Button3Text . Foreground = DlgInput . btnforegroundDark;
				}
			}
			else if ( brdr . Name . Contains ( "4" ) )
			{
				if ( DefaultButton == 0 )
				{
					// We are just resetting to normal
					Button4Text . Foreground = DlgInput . btnforeground;
				}
				else if ( DefaultButton == 4 )
				{
					// IS the defaullt button
					if ( ismouseover )
						Button4Text . Foreground = DlgInput . defbtnforeground;
					else
						Button4Text . Foreground = DlgInput . btnforeground;
				}
				else
				{
					// Not the defaullt button
					if ( ismouseover )
						Button4Text . Foreground = DlgInput . Btnmouseforeground;
					else
						Button4Text . Foreground = DlgInput . btnforegroundDark;
				}
			}
			return;
		}

		#endregion main button color handling

		#region Routed Event Button handlers ?
		public RoutedEvent ProcessOKBtn ( )
		{
			Console . WriteLine ( "OK button clicked" );
			Dlgresult . returnint = 1;
			DlgInput . MsgboxWin = null;
			this . Close ( );
			return null;
		}
		public RoutedEvent ProcessYesBtn ( )
		{
			Console . WriteLine ( "Yes button clicked" );
			Dlgresult . returnint = 2;
			DlgInput . MsgboxWin = null;
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
			//GenericClass v = DlgInput.obj as GenericClass;
			//if ( v != null )
			//	Console . WriteLine ( $"genericClass object :{v . field1}..." );
			//else
			//	Console . WriteLine ( "Not  a genericClass object..." );
			//ObservableCollection < GenericClass > vv= DlgInput . obj as ObservableCollection< GenericClass>;
			//if ( vv != null )
			//	Console . WriteLine ( $"ObservableColllection identified : count = {vv . Count}" );
			//else
			//	Console . WriteLine ( "Not  a ObservableColllection object..." );
			Dlgresult . obj = DlgInput . obj;
			DlgInput . MsgboxWin = null;
			this . Close ( );
			return null;
		}
		public RoutedEvent ProcessCancelBtn ( )
		{
			Console . WriteLine ( "Cancel button clicked" );
			Dlgresult . returnint = 4;
			DlgInput . MsgboxWin = null;
			this . Close ( );
			return null;
		}
		#endregion Routed Event handlers ?

		#region Button Action Handlers
		//********************//
		// ProcessYes  event
		//********************//
		// Called when btn1 is clicked
		private void Button1_ProcessOK ( object sender , RoutedEventArgs e )
		{
			RaiseProcessOKEvent ( );
			ProcessOKBtn ( );
		}

		private void Button2_ProcessYes ( object sender , RoutedEventArgs e )
		{
			RaiseProcessYesEvent ( );
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
		#endregion Button Handlers

		#region Button Tab/Arrow movement

		private void Button1_KeyDown ( object sender , KeyEventArgs e )
		{
			bool changed= false;
			Button b = sender as Button;
			//Console . WriteLine ( $"Button being pressed is [{b . Name . ToUpper ( )}]" );
			// move FORWARD

			if ( e . Key == Key . Enter )
			{
				if ( DefBorder == Button1 )
					Button1_ProcessOK ( null , null );
				else if ( DefBorder == Button2 )
					Button2_ProcessYes ( null , null );
				else if ( DefBorder == Button3 )
					Button3_ProcessNo ( null , null );
				else if ( DefBorder == Button4 )
					Button4_ProcessCancel ( null , null );
			}
			if (
				( ( e . Key == Key . Tab || e . Key == Key . Right ) && isShiftDown == false )
				||
				( ( e . Key == Key . Tab || e . Key == Key . Left ) && isShiftDown == true )
				)
			{
				CurrentButton = 0;

				UpdateButtonX ( 1 , Button1 , IsTabbing );

				if ( Button2 . Visibility == Visibility . Visible )
				{
					CurrentButton = 2;
					UpdateButtonX ( 2 , Button2 , IsTabbing );
					Button2 . Focus ( );
					changed = true;
				}
				else if ( Button3 . Visibility == Visibility . Visible )
				{
					CurrentButton = 3;
					UpdateButtonX ( 3 , Button3 , IsTabbing );
					Button3 . Focus ( );
					changed = true;
				}
				else if ( Button4 . Visibility == Visibility . Visible )
				{
					CurrentButton = 4;
					UpdateButtonX ( 4 , Button4 , IsTabbing );
					Button4 . Focus ( );
					changed = true;
				}
				if ( changed == false )
				{
					CurrentButton = 1;
					UpdateButtonX ( 1 , Button1 , IsTabbing );
					Button1 . Focus ( );
				}
			}
			else if (
				( ( e . Key == Key . Tab || e . Key == Key . Left ) && isShiftDown == false )
				||
				( ( e . Key == Key . Tab || e . Key == Key . Right ) && isShiftDown == true )
				)
			// move BACKWARD
			{
				UpdateButtonX ( 1 , Button1 , IsTabbing );

				if ( Button4 . Visibility == Visibility . Visible )
				{
					CurrentButton = 4;
					UpdateButtonX ( 4 , Button4 , IsTabbing );
					Button4 . Focus ( );
					changed = true;
				}
				else if ( Button3 . Visibility == Visibility . Visible )
				{
					CurrentButton = 3;
					UpdateButtonX ( 3 , Button3 , IsTabbing );
					Button3 . Focus ( );
					changed = true;
				}
				else if ( Button2 . Visibility == Visibility . Visible )
				{
					CurrentButton = 2;
					UpdateButtonX ( 2 , Button2 , IsTabbing );
					Button2 . Focus ( );
					changed = true;
				}
				if ( changed == false )
				{
					CurrentButton = 1;
					UpdateButtonX ( 1 , Button1 , IsTabbing );
					Button1 . Focus ( );
				}
			}
		}
		private void Button2_KeyDown ( object sender , KeyEventArgs e )
		{
			bool changed = false;
			Border b = sender as Border;
			//			Console . WriteLine ( $"Button being pressed is [{b . Name . ToUpper ( )}]" );
			// move FORWARD
			if (
				( ( e . Key == Key . Tab || e . Key == Key . Right ) && isShiftDown == false )
				||
				( ( e . Key == Key . Tab || e . Key == Key . Left ) && isShiftDown == true )
				)
			{
				UpdateButtonX ( 2 , Button2 , IsTabbing );

				if ( Button3 . Visibility == Visibility . Visible )
				{
					CurrentButton = 3;
					UpdateButtonX ( 3 , Button3 , IsTabbing );
					Button3 . Focus ( );
					changed = true;
				}
				else if ( Button4 . Visibility == Visibility . Visible )
				{
					CurrentButton = 4;
					UpdateButtonX ( 4 , Button4 , IsTabbing );
					Button4 . Focus ( );
					changed = true;
				}
				else if ( Button1 . Visibility == Visibility . Visible )
				{
					CurrentButton = 1;
					UpdateButtonX ( 1 , Button1 , IsTabbing );
					Button1 . Focus ( );
					changed = true;
				}
				if ( changed == false )
				{
					CurrentButton = 2;
					UpdateButtonX ( 2 , Button2 , IsTabbing );
					Button2 . Focus ( );
				}
			}
			else if ( ( ( e . Key == Key . Tab || e . Key == Key . Left ) && isShiftDown == false )
				||
				( ( e . Key == Key . Tab || e . Key == Key . Right ) && isShiftDown == true )
				)

			{
				UpdateButtonX ( 2 , Button2 , IsTabbing );

				if ( Button1 . Visibility == Visibility . Visible )
				{
					CurrentButton = 1;
					UpdateButtonX ( 1 , Button1 , IsTabbing );
					Button1 . Focus ( );
					changed = true;
				}
				else if ( Button4 . Visibility == Visibility . Visible )
				{
					CurrentButton = 4;
					UpdateButtonX ( 4 , Button4 , IsTabbing );
					Button4 . Focus ( );
					changed = true;
				}
				else if ( Button3 . Visibility == Visibility . Visible )
				{
					CurrentButton = 3;
					UpdateButtonX ( 3 , Button3 , IsTabbing );
					Button3 . Focus ( );
					changed = true;
				}
				if ( changed == false )
				{
					CurrentButton = 2;
					UpdateButtonX ( 2 , Button2 , IsTabbing );
					Button2 . Focus ( );
				}
			}
		}
		private void Button3_KeyDown ( object sender , KeyEventArgs e )
		{
			bool changed=false;
			Button b = sender as Button;
			//nsole . WriteLine ( $"Button being pressed is [{b . Name . ToUpper ( )}]" );
			// move FORWARD
			if (
				( ( e . Key == Key . Tab || e . Key == Key . Right ) && isShiftDown == false )
				||
				( ( e . Key == Key . Tab || e . Key == Key . Left ) && isShiftDown == true )
				)
			{
				UpdateButtonX ( 3 , Button3 , false );

				if ( Button4 . Visibility == Visibility . Visible )
				{
					CurrentButton = 4;
					UpdateButtonX ( 4 , Button4 , IsTabbing );
					changed = true;
					Button4 . Focus ( );
				}
				else if ( Button1 . Visibility == Visibility . Visible )
				{
					CurrentButton = 1;
					changed = true;
					UpdateButtonX ( 1 , Button1 , IsTabbing );
					Button1 . Focus ( );
				}
				else if ( Button2 . Visibility == Visibility . Visible )
				{
					CurrentButton = 2;
					UpdateButtonX ( 2 , Button2 , IsTabbing );
					Button2 . Focus ( );
					changed = true;
				}
				if ( changed == false )
				{
					CurrentButton = 3;
					UpdateButtonX ( 3 , Button3 , IsTabbing );
					Button3 . Focus ( );
				}
			}
			else if ( ( ( e . Key == Key . Tab || e . Key == Key . Left ) && isShiftDown == false )
				||
				( ( e . Key == Key . Tab || e . Key == Key . Right ) && isShiftDown == true )
				)
			{
				UpdateButtonX ( 3 , Button3 , IsTabbing );
				if ( Button2 . Visibility == Visibility . Visible )
				{
					CurrentButton = 2;
					UpdateButtonX ( 2 , Button2 , IsTabbing );
					changed = true;
					Button2 . Focus ( );
				}
				else if ( Button1 . Visibility == Visibility . Visible )
				{
					CurrentButton = 1;
					UpdateButtonX ( 1 , Button1 , IsTabbing );
					changed = true;
					Button1 . Focus ( );
				}
				else if ( Button4 . Visibility == Visibility . Visible )
				{
					CurrentButton = 4;
					UpdateButtonX ( 4 , Button4 , IsTabbing );
					changed = true;
					Button4 . Focus ( );
				}
				if ( changed == false )
				{
					CurrentButton = 3;
					UpdateButtonX ( 3 , Button3 , IsTabbing );
					Button3 . Focus ( );
				}
			}
		}
		private void Button4_KeyDown ( object sender , KeyEventArgs e )
		{
			bool changed=false;
			Button b = sender as Button;
			// move FORWARD
			//Console . WriteLine ( $"Button being pressed is [{b . Name . ToUpper ( )}]" );
			if (
				( ( e . Key == Key . Tab || e . Key == Key . Right ) && isShiftDown == false )
				||
				( ( e . Key == Key . Tab || e . Key == Key . Left ) && isShiftDown == true )
				)
			{
				UpdateButtonX ( 4 , Button4 , IsTabbing );

				if ( Button1 . Visibility == Visibility . Visible )
				{
					CurrentButton = 1;
					UpdateButtonX ( 1 , Button1 , IsTabbing );
					changed = true;
					Button1 . Focus ( );
				}
				else if ( Button2 . Visibility == Visibility . Visible )
				{
					CurrentButton = 2;
					UpdateButtonX ( 2 , Button2 , IsTabbing );
					changed = true;
					Button2 . Focus ( );
				}
				else if ( Button3 . Visibility == Visibility . Visible )
				{
					CurrentButton = 3;
					UpdateButtonX ( 3 , Button3 , IsTabbing );
					changed = true;
					Button3 . Focus ( );
				}
				if ( changed == false )
				{
					CurrentButton = 4;
					UpdateButtonX ( 4 , Button4 , IsTabbing );
					Button4 . Focus ( );
				}
			}
			else if ( ( ( e . Key == Key . Tab || e . Key == Key . Left ) && isShiftDown == false )
				||
				( ( e . Key == Key . Tab || e . Key == Key . Right ) && isShiftDown == true )
				)
			{
				UpdateButtonX ( 4 , Button4 , IsTabbing );
				if ( Button3 . Visibility == Visibility . Visible )
				{
					CurrentButton = 3;
					UpdateButtonX ( 3 , Button3 , IsTabbing );
					changed = true;
					Button3 . Focus ( );
				}
				else if ( Button2 . Visibility == Visibility . Visible )
				{
					CurrentButton = 2;
					UpdateButtonX ( 2 , Button2 , IsTabbing );
					changed = true;
					Button2 . Focus ( );
				}
				else if ( Button1 . Visibility == Visibility . Visible )
				{
					CurrentButton = 1;
					UpdateButtonX ( 1 , Button1 , IsTabbing );
					changed = true;
					Button1 . Focus ( );
				}

				if ( changed == false )
				{
					CurrentButton = 4;
					UpdateButtonX ( 4 , Button4 , IsTabbing );
					Button4 . Focus ( );
				}
			}
		}

		#endregion Button Tab/Arrow movement

		#region window level key handling
		// Check for SHIFT key !!
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
		private void Window_KeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . LeftShift || e . Key == Key . RightShift )
			{
				if ( e . IsUp )
					isShiftDown = false;
			}
		}
		private void Button1_MouseMove ( object sender , MouseEventArgs e )
		{
			//Button b = sender as Button;
			//b . Background = FindResource ( "Yellow0" )  as Brush;
			//b . UpdateLayout ( );
		}
		#endregion level key handling

		#region RoutedEvents

		//********************//
		// ProcessOK event
		//********************//
		// Create a custom routed event by first registering a RoutedEventID
		// This event uses the bubbling routing strategy
		public static readonly RoutedEvent ProcessOKEvent = EventManager.RegisterRoutedEvent(
	  "ProcessOK", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Msgbox));

		// Provide CLR accessors for the event
		public event RoutedEventHandler ProcessOK
		{
			add { AddHandler(ProcessOKEvent, value); }
			remove { RemoveHandler(ProcessOKEvent, value); }
		}
		protected virtual void RaiseProcessOKEvent ( )
		{
			RoutedEventArgs args = new
		RoutedEventArgs(ProcessOKEvent);
			RaiseEvent ( args );
		}

		// This method raises the ProcessYes  event
		//void RaiseProcessOKEvent ( )
		//{
		//	RoutedEventArgs newEventArgs = new RoutedEventArgs(ProcessOKEvent);
		//	RaiseEvent ( newEventArgs );
		//}

		//********************//
		// ProcessYes event
		//********************//
		// Create a custom routed event by first registering a RoutedEventID
		// This event uses the bubbling routing strategy
		public static readonly RoutedEvent ProcessYesEvent = EventManager.RegisterRoutedEvent(
	  "ProcessYes", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Msgbox));

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
	  "ProcessNo", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Msgbox));

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
	  "ProcessCancel", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Msgbox));

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

		protected virtual void RaiseMouseWheelEvent ( )
		{
			RoutedEventArgs args = new RoutedEventArgs(CustomWheelEvent);
			RaiseEvent ( args );
		}

		public static readonly RoutedEvent CustomWheelEvent =
	   EventManager.RegisterRoutedEvent("MyCustomWheelRotate", RoutingStrategy.Bubble,
	   typeof(RoutedEventHandler), typeof(Msgbox));

		/* Just like Dependency Properties, routed events are also like wrapper over underlying RoutedEvent instance 
		 * and they wrap through a set of getter-setter methods. 
		 */
		public event RoutedEventHandler MyCustomWheelRotate
		{
			add { AddHandler(CustomWheelEvent, value); }
			remove { RemoveHandler(CustomWheelEvent, value); }
		}

		#endregion RoutedEvents


		private void Window_Deactivated ( object sender , EventArgs e )
		{
			this . Deactivated += ( s , e ) => this . Activate ( );
		}
   		private void Custom_MouseWheel ( object sender , RoutedEventArgs e )
		{
			RaiseMouseWheelEvent ( );
		}

		#region border->Button Click
		private void Border1_ProcessOK ( object sender , MouseButtonEventArgs e )
		{
			Button1_ProcessOK ( null , null );
		}
		private void Border2_ProcessYes ( object sender , MouseButtonEventArgs e )
		{
			Button2_ProcessYes ( null , null );
		}
		private void Border3_ProcessNo ( object sender , MouseButtonEventArgs e )
		{
			Button3_ProcessNo ( null , null );
		}
		private void Border4_ProcessCancel ( object sender , MouseButtonEventArgs e )
		{
			Button4_ProcessCancel ( null , null );
		}
		#endregion border->Button Click

		#region button mouseover handlers

		private void Button_MouseEnter ( object sender , MouseEventArgs e )
		{
			Border b = sender as Border;
			if ( b . Name . Contains ( "1" ) )
			{
				if ( DefaultButton == 1 )
					SetBtnStatus ( 1 , b , true , true );
				else
					SetBtnStatus ( 1 , b , false , true );
			}
			else if ( b . Name . Contains ( "2" ) )
			{
				if ( DefaultButton == 2 )
					SetBtnStatus ( 2 , b , true , true );
				else
					SetBtnStatus ( 2 , b , false , true );
			}
			else if ( b . Name . Contains ( "3" ) )
			{
				if ( DefaultButton == 3 )
					SetBtnStatus ( 3 , b , true , true );
				else
					SetBtnStatus ( 3 , b , false , true );
			}
			else if ( b . Name . Contains ( "4" ) )
			{
				if ( DefaultButton == 4 )
					SetBtnStatus ( 4 , b , true , true );
				else
					SetBtnStatus ( 4 , b , false , true );
			}
			b . UpdateLayout ( );
		}
		private void Button_MouseLeave ( object sender , MouseEventArgs e )
		{
			Border b = sender as Border;
			b . Background = DlgInput . btnbackground;
			b . UpdateLayout ( );
			TabPass = 2;
			if ( b == DefBorder )
				SetBtnStatus ( GetButtonNumberFromName ( b . Name ) , b , DefaultButton == GetButtonNumberFromName ( b . Name ) , false );
			else
				SetBtnStatus ( GetButtonNumberFromName ( b . Name ) , b , DefaultButton == GetButtonNumberFromName ( b . Name ) , false );
			TabPass = 0;
		}

		#endregion button mouseover handlers

		#region Load data
		// Read configuration in from disk fie
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
					string[] flds =  fields[9].Split(',');
					DlgInput . BorderSizeNormal = new Thickness ( Convert . ToInt32 ( flds [ 0 ] ) , Convert . ToInt32 ( flds [ 1 ] ) , Convert . ToInt32 ( flds [ 2 ] ) , Convert . ToInt32 ( flds [ 3 ] ) );
					return;
				}

				if ( DlgInput . UseDarkMode == false )
				{
					switch ( indx++ )
					{
						case 0:
							DlgInput . dlgbackground = Utils . GetNewBrush ( item );
							break;
						case 1:
							DlgInput . dlgforeground = Utils . GetNewBrush ( item );
							break;
						case 2:
							DlgInput . btnbackground = Utils . GetNewBrush ( item );
							break;
						case 3:
							DlgInput . btnforeground = Utils . GetNewBrush ( item );
							break;
						case 4:
							DlgInput . Btnborder = Utils . GetNewBrush ( item );
							break;
						case 5:
							DlgInput . Btnmousebackground = Utils . GetNewBrush ( item );
							break;
						case 6:
							DlgInput . Btnmouseforeground = Utils . GetNewBrush ( item );
							break;
						case 7:
							DlgInput . defbtnbackground = Utils . GetNewBrush ( item );
							break;
						case 8:
							DlgInput . defbtnforeground = Utils . GetNewBrush ( item );
							break;
						case 10:
							DlgInput . UseIcon = item == "T" ? true : false;
							break;
						case 9:
							string[] flds = item.Split(',');
							DlgInput . BorderSizeNormal = new Thickness ( Convert . ToInt32 ( flds [ 0 ] ) , Convert . ToInt32 ( flds [ 1 ] ) , Convert . ToInt32 ( flds [ 2 ] ) , Convert . ToInt32 ( flds [ 3 ] ) );
							break;
						case 11:
							DlgInput . isClean = item == "T" ? true : false;
							break;
						case 12:
							DlgInput . UseDarkMode = item . Contains ( "DMY" ) ? true : false;
							break;
						case 21:
							flds = item . Split ( ',' );
							DlgInput . BorderSizeDefault = new Thickness ( Convert . ToInt32 ( flds [ 0 ] ) , Convert . ToInt32 ( flds [ 1 ] ) , Convert . ToInt32 ( flds [ 2 ] ) , Convert . ToInt32 ( flds [ 3 ] ) );
							break;
						case 24:
							if ( item == "" )
								Row1 . FontSize = 13;
							else
								Row1 . FontSize = Convert . ToDouble ( item );
								Row2 . FontSize = Convert . ToDouble ( item );
							break;
						case 25:
							if ( item == "" )
								Row3 . FontSize = 13;
							else
								Row3 . FontSize = Convert . ToDouble ( item );
							break;
					}
				}
				else
				{
					switch ( indx++ )
					{
						case 0:
							DlgInput . dlgbackground = Utils . GetNewBrush ( item );
							break;
						case 1:
							DlgInput . dlgforeground = Utils . GetNewBrush ( item );
							break;
						case 9:
							string[] flds = item.Split(',');
							DlgInput . BorderSizeNormal = new Thickness ( Convert . ToInt32 ( flds [ 0 ] ) , Convert . ToInt32 ( flds [ 1 ] ) , Convert . ToInt32 ( flds [ 2 ] ) , Convert . ToInt32 ( flds [ 3 ] ) );
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
						case 13:
							DlgInput . BtnborderDark = Utils . GetNewBrush ( item );
							break;
						case 14:
							DlgInput . btnbackground = Utils . GetNewBrush ( item );
							break;
						case 15:
							DlgInput . btnforeground = Utils . GetNewBrush ( item );
							break;
						case 16:
							DlgInput . Btnborder = Utils . GetNewBrush ( item );
							break;
						case 17:
							DlgInput . Btnmousebackground = Utils . GetNewBrush ( item );
							break;
						case 18:
							DlgInput . Btnmouseforeground = Utils . GetNewBrush ( item );
							break;
						case 19:
							DlgInput . defbtnbackground = Utils . GetNewBrush ( item );
							break;
						case 20:
							DlgInput . defbtnforeground = Utils . GetNewBrush ( item );
							break;
						case 21:
							flds = item . Split ( ',' );
							Thickness th = new Thickness ( Convert . ToInt32 ( flds [ 0 ] ) , Convert . ToInt32 ( flds [ 1 ] ) , Convert . ToInt32 ( flds [ 2 ] ) , Convert . ToInt32 ( flds [ 3 ] ) );
							DlgInput . BorderSizeDefault = th;
							break;
						case 24:
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
						case 25:      
							if ( item == "" )
								Row3 . FontSize = 13;
							else
								Row3 . FontSize = Convert . ToDouble ( item );
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

		}

		#endregion Load data

		#region Utilities

		private int GetButtonNumberFromName ( string btnname )
		{
			if ( btnname . Contains ( "1" ) )
				return 1;
			else if ( btnname . Contains ( "2" ) )
				return 2;
			else if ( btnname . Contains ( "3" ) )
				return 3;
			else if ( btnname . Contains ( "4" ) )
				return 4;
			return 0;
		}

		#endregion Utilities

		#region NOT IN USE
		private void msgbox_Loaded ( object sender , RoutedEventArgs e )
		{
			// liit widht/height for OK only short message version
			int textlen = Row1.Text.Length + Row2.Text.Length;
			if ( textlen < 250 && Row2 . Text == "" )
			{
				// just one row of text	 < 250 chars
				//if ( Row1 . FontSize > 13 )
				//{
				Row1 . Height = 90;
				switch ( Row1 . FontSize )
				{
					case 9:  
						Row1 . Height = 45;
						Row2 . Height = 10;
						this . Height = 100;
						this . Width = 365;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -60 , right: 0 , bottom: 0 );
						break;
					case 10:  
						Row1 . Height = 45;
						Row2 . Height = 10;
						this . Height = 100;
						this . Width = 365;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -60 , right: 0 , bottom: 0 );
						break;
					case 11:      
						Row1 . Height = 45;
						Row2 . Height = 10;
						this . Height = 120;
						this . Width = 385;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -60 , right: 0 , bottom: 0 );
						break;
					case 12:  
						Row1 . Height = 50;
						Row2 . Height = 10;
						this . Height = 120;
						this . Width = 400;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -60 , right: 0 , bottom: 0 );
						break;
					case 13: 
						Row1 . Height = 55;
						Row2 . Height = 10;
						this . Height = 130;
						this . Width = 440;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -60 , right: 0 , bottom: 0 );
						break;   
					case 14:
						Row1 . Height = 60;
						Row2 . Height = 10;
						this . Height = 150;
						this . Width = 525;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -65 , right: 0 , bottom: 0 );
						break;
					case 15:  
						Row1 . Height = 70;
						Row2 . Height = 10;
						this . Height = 160;
						this . Width = 610;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -65 , right: 0 , bottom: 0 );
						break;  // OK
						Row1 . Height = 70;
						Row2 . Height = 10;
						this . Height = 160;
						this . Width = 600;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -65 , right: 0 , bottom: 0 );
						break;
					case 17:   
						Row1 . Height = 90;
						Row2 . Height = 10;
						this . Height = 180;
						this . Width = 575;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -65 , right: 0 , bottom: 0 );
						break;
					case 18:       
						Row1 . Height = 90;
						Row2 . Height = 10;
						this . Height = 200;
						this . Width = 750;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -40 , right: 0 , bottom: 0 );
						break;
					case 19:   
						Row1 . Height = 90;
						Row2 . Height = 10;
						this . Height = 200;
						this . Width = 750;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -40 , right: 0 , bottom: 0 );
						break;
					case 20:       
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
			}
			else if ( textlen < 250 && Row2 . Text != "" )
			{
				// just one row of text	 < 250 chars
				//if ( Row1 . FontSize > 13 )
				//{
				Row1 . Height = 90;
				switch ( Row1 . FontSize )
				{
					case 9:	// OK
						Row1 . Height = 35;
						Row2 . Height = 20;
						this . Height = 260;
						this . Width = 465;
						BtnWrap . Margin = new Thickness ( left: 0 , top: 0 , right: 0 , bottom: 0 );
						break;
					case 10:
						Row1 . Height = 35;
						Row2 . Height = 40;
						this . Height = 260;
						this . Width = 485;
						BtnWrap . Margin = new Thickness ( left: 0 , top: 0 , right: 0 , bottom: 0 );
						break;
					case 11:
						Row1 . Height = 45;
						Row2 . Height = 30;
						Row3 . Height = 30;
						this . Height = 265;
						this . Width = 465;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -105 , right: 0 , bottom: 0 );
						break;
					case 12:
						Row1 . Height = 45;
						Row2 . Height = 30;
						Row3 . Height = 30;
						this . Height = 265;
						this . Width = 475;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -10 , right: 0 , bottom: 0 );
						break;
					case 13:
						Row1 . Height = 55;
						Row2 . Height = 40;
						Row3 . Height = 40;
						this . Height = 265;
						this . Width = 505;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -10 , right: 0 , bottom: 0 );
						break;
					case 14:	// OK
						Row1 . Height = 65;
						Row2 . Height = 40;
						Row3 . Height = 40;
						this . Height = 265;
						this . Width = 535;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -10 , right: 0 , bottom: 0 );
						break;
					case 15:
						Row1 . Height = 80;
						Row2 . Height = 40;
						Row3 . Height = 40;
						this . Height = 265;
						this . Width = 560;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -5 , right: 0 , bottom: 0 );
						break;  // OK
					case 16:
						Row1 . Height = 80;
						Row2 . Height = 40;
						Row3 . Height = 40;
						this . Height = 255;
						this . Width = 580;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -5 , right: 0 , bottom: 0 );
						break;
					case 17:
						Row1 . Height = 80;
						Row2 . Height = 40;
						Row3 . Height = 40;
						this . Height = 285;
						this . Width = 560;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -5 , right: 0 , bottom: 0 );
						break;
					case 18:
						Row1 . Height = 80;
						Row2 . Height =60;
						Row3 . Height = 40;
						this . Height = 325;
						this . Width = 570;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -5 , right: 0 , bottom: 0 );
						break;
					case 19:
						Row1 . Height = 80;
						Row2 . Height = 60;
						Row3 . Height = 40;
						this . Height = 325;
						this . Width = 570;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -5 , right: 0 , bottom: 0 );
						break;
					case 20:
						Row1 . Height = 80;
						Row2 . Height = 60;
						Row3 . Height = 40;
						this . Height = 345;
						this . Width = 590;
						BtnWrap . Margin = new Thickness ( left: 0 , top:10 , right: 0 , bottom: 0 );
						break;
					case 21:
						Row1 . Height = 80;
						Row2 . Height = 60;
						Row3 . Height = 40;
						this . Height = 350;
						this . Width = 620;
						BtnWrap . Margin = new Thickness ( left: 0 , top: -5 , right: 0 , bottom: 0 );
						break;
				}
			}
			else
			{
				Row1 . Height = 90;
				Row2 . Height = 60;
				this . Height = 300;
				this . Width = 465;
				// Move buttons up in dialog ?
				//				BoxGridRow6 . Height = 65;
				Thickness th = BtnWrap . Margin;
				th.Top = -15;
				BtnWrap . Margin = th;
				
			}
		}

		#endregion NOT IN USE

		#region Focus support
		// Temporary method to set focus to a button
		private void Button1_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
		{
			Button1 . Focus ( );
			CurrentButton = 1;
		}
		private void Button2_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
		{
			Button2 . Focus ( );
			CurrentButton = 2;
		}
		private void Button3_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
		{
			Button3 . Focus ( );
			CurrentButton = 3;
		}
		private void Button4_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
		{
			Button4 . Focus ( );
			CurrentButton = 4;
		}
		#endregion Focus support

		#region button actions
		private void Border1_ProcessOK ( object sender , KeyEventArgs e )
		{
			Button1_ProcessOK ( null , null );
		}
		private void Border2_ProcessYes ( object sender , KeyEventArgs e )
		{
			Button2_ProcessYes ( null , null );
		}

		private void Border3_ProcessNo ( object sender , KeyEventArgs e )
		{
			Button3_ProcessNo ( null , null );
		}

		private void Border4_ProcessCancel ( object sender , KeyEventArgs e )
		{
			Button4_ProcessCancel ( null , null );
		}
		#endregion button actions

		#region Mouse movement
		private void ChecksMouseMove ( object sender , MouseEventArgs e )
		{
			e . Handled = true;
			if ( e . RightButton == MouseButtonState . Pressed )
				return;
		}

		private void Grab_MouseMove ( object sender , MouseEventArgs e )
		{
			if ( e . LeftButton == MouseButtonState . Pressed )
				Utils . Grab_MouseMove ( sender , e );
			e . Handled = true;
		}
		#endregion Mouse movement

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
			if ( DefBorder != Button1 )
			{
				Button1 . Background = DlgInput . btnbackground;
				Button1Text . Foreground = DlgInput . btnforeground;
			}
			else
			{
				Button1 . Background = DlgInput . defbtnbackground;
				Button1Text . Foreground = DlgInput . defbtnforeground;
			}
			Button1 . BorderBrush = DlgInput . Btnborder;

			if ( DefBorder != Button2 )
			{
				Button2 . Background = DlgInput . btnbackground;
				Button2Text . Foreground = DlgInput . btnforeground;
			}
			else
			{
				Button2 . Background = DlgInput . defbtnbackground;
				Button2Text . Foreground = DlgInput . defbtnforeground;
			}
			Button2 . BorderBrush = DlgInput . Btnborder;
			if ( DefBorder != Button3 )
			{
				Button3 . Background = DlgInput . btnbackground;
				Button3Text . Foreground = DlgInput . btnforeground;
			}
			else
			{
				Button3 . Background = DlgInput . defbtnbackground;
				Button3Text . Foreground = DlgInput . defbtnforeground;
			}
			Button3 . BorderBrush = DlgInput . Btnborder;
			if ( DefBorder != Button4 )
			{
				Button4 . Background = DlgInput . btnbackground;
				Button4Text . Foreground = DlgInput . btnbackground;
			}
			else
			{
				Button4 . Background = DlgInput . defbtnbackground;
				Button4Text . Foreground = DlgInput . defbtnforeground;
			}
			Button4 . BorderBrush = DlgInput . Btnborder;
			Button2 . Refresh ( );
			Button3 . Refresh ( );
			Button4 . Refresh ( );

			#endregion Button update colors

			this . UpdateLayout ( );
			this . Refresh ( );
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

		#region NOT USED

		//NOT USED
		private int GetNextActiveButton ( int active )
		{
			int newbtn = 0;
			for ( int x = 0 ; x < 4 ; x++ )
			{
				if ( btnsarray [ x ] == active && x < 4 )
				{
					for ( int y = x + 1 ; y < 4 ; y++ )
					{
						if ( btnsarray [ y ] != 0 )
							newbtn = y;
						break;
					}
					if ( newbtn != 0 )
						break;
				}
			}

			return btnsarray [ newbtn ];
		}
		//NOT USED
		private void SetNextBtn ( int nextbtn )
		{
			//if ( nextbtn == 1 )
			//{
			//	Button1 . IsDefault = true;
			//	UpdateButtonX ( Button1border , Button1 );
			//}
			//else if ( nextbtn == 2 )
			//{
			//	Button2 . IsDefault = true;
			//	UpdateButtonX ( Button2border , Button2 );
			//}
			//else if ( nextbtn == 3 )
			//{
			//	Button3 . IsDefault = true;
			//	UpdateButtonX ( Button3border , Button3 );
			//}
			//else if ( nextbtn == 4 )
			//{
			//	Button4 . IsDefault = true;
			//	UpdateButtonX ( Button4border , Button4 );
			//}
		}
		#endregion NOT USED
	}
}


