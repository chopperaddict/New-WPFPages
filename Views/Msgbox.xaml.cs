
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

using WPFPages . AttachedProperties;
using WPFPages . Converts;
using WPFPages . Views;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for Msgbox.xaml
	/// </summary>
	public partial class Msgbox : Window
	{
		System . Drawing . Image image { set; get; }

		#region rowdata

		string CaptionString { get; set; }
		string Row1String { get; set; }
		string Row2String { get; set; }
		string Row3String { get; set; }

		#endregion rowdata

		#region buttondata
		private string Btn1Text { get; set; }
		private string Btn2Text { get; set; }
		private string Btn3Text { get; set; }
		private string Btn4Text { get; set; }
		private Brush Borderbrush { get; set; }

		private int[] btnsarray = new int[4];
		private string [ ] btnstextarray = { "","","",""};

		static Brush Btnbackground;               // Std button Background
		static Brush Btnforeground;               // Std button Foreground
		static Brush BtnMbackground;        // Mouseover button Background
		static Brush BtnMforeground;        // Mouseover button Foreground
		static Brush DefBtnbackground;        // Defaullt Button Background
		static Brush DefBtnforeground;         // Default Button Foreground
		static Brush bordercolor;                       // Default Button border color

		#endregion buttondata

		private bool  IsTabbing=false;
		private int  DefaultButton = 0;
		private int  CurrentButton = 0;
		private Border DefBorder;
		private bool isShiftDown = false;
		private int TabPass=0;

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
		//private void ConfigureCurrentColors ( )
		//{
		//	//Setup our attached properties from DlgInput structure
		//	SetValue( BackGroundProperty, DlgInput . bground );
		//	SetValue( ForegroundProperty, DlgInput . bforeground );
		//	SetValue ( MouseoverForeGroundProperty, DlgInput . mousefground );
		//	SetValue ( MouseoverBackGroundProperty, DlgInput . mousebground );
		//	SetValue (  BorderSizeProperty, DlgInput . border );
		//}
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
			ReadMsgboxData ( );
			CheckForInitialDefaultSettings ( );
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
							btnstextarray [ indx++ ] = btn1Text;
						break;
					case 1:
						if ( btn2Text != "" )
							btnstextarray [ indx++ ] = btn2Text;
						break;
					case 2:
						if ( btn3Text != "" )
							btnstextarray [ indx++ ] = btn3Text;
						break;
					case 3:
						if ( btn4Text != "" )
							btnstextarray [ indx++ ] = btn4Text;
						break;
				}
			}
			object obj = DlgInput . obj;
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
			CheckForFinalDefaultSettings ( iconstring );
			this . MouseWheel += Custom_MouseWheel;
			MouseMove += Utils . Grab_MouseMove;
			KeyDown += Window_PreviewKeyDown;
		}
		private void Window_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . F11 )
			{
				if ( Utils . ControlsHitList . Count == 0 )
					return;
				Utils . Grabscreen ( this , Utils . ControlsHitList [ 0 ] . VisualHit , null , sender as Control );
			}
		}

		//Set up the basic Dlg colors etc
		public void CheckForInitialDefaultSettings ( )
		{

			Console . WriteLine ( $"Processing CheckForInitialDefaultSettings in Msgbox.cs...." );
			if ( DlgInput . UseDarkMode )
			{
				DlgInput . dlgbackground = "#FF000000" . ToSolidBrush ( );
				DlgInput . Btnborder = FindResource ( "Yellow1" ) as Brush;
				DlgInput . mouseforeground = FindResource ( "White0" ) as Brush;
				DlgInput . btnbackground = FindResource ( "Blue1" ) as Brush;
				DlgInput . btnforeground = FindResource ( "White0" ) as Brush;
				if ( DlgInput . Btnborder == null )
					DlgInput . Btnborder = "#E9FFF100" . ToSolidBrush ( ); // Yellow
				DlgInput . mousebackground = FindResource ( "Yellow1" ) as Brush;
			}
			else
			{
				if ( DlgInput . dlgbackground == null )
					DlgInput . dlgbackground = FindResource ( "White6" ) as Brush;
				if ( DlgInput . mouseforeground == null )
					DlgInput . mouseforeground = FindResource ( "Black0" ) as Brush;
				//DlgInput . border = FindResource ( "Black0" ) as Brush;
				if ( DlgInput . btnbackground == null )
					DlgInput . btnbackground = FindResource ( "Red1" ) as Brush;
				if ( DlgInput . btnforeground == null )
					DlgInput . btnforeground = FindResource ( "White0" ) as Brush;
				if ( DlgInput . Btnborder == null )
					DlgInput . Btnborder = FindResource ( "White0" ) as Brush;
				if ( DlgInput . mousebackground == null )
					DlgInput . mousebackground = FindResource ( "Black3" ) as Brush;
			}
			Btnbackground = DlgInput . btnbackground;
			Btnforeground = DlgInput . btnforeground;
			BtnMbackground = DlgInput . mousebackground;
			BtnMforeground = DlgInput . mouseforeground;
			DefBtnbackground = DlgInput . defbtnbackground;
			DefBtnforeground = DlgInput . defbtnforeground;
			bordercolor = DlgInput . Btnborder;

			Background = DlgInput . dlgbackground;
			BoxGrid . Background = DlgInput . dlgbackground;

			Row1 . Foreground = DlgInput . dlgforeground;
			Row2 . Foreground = DlgInput . dlgforeground;
			Row3 . Foreground = DlgInput . dlgforeground;

			Button1 . Background = DlgInput . btnbackground;
			Button2 . Background = DlgInput . btnbackground;
			Button3 . Background = DlgInput . btnbackground;
			Button4 . Background = DlgInput . btnbackground;

			Button1 . BorderBrush = DlgInput . Btnborder;
			Button2 . BorderBrush = DlgInput . Btnborder;
			Button3 . BorderBrush = DlgInput . Btnborder;
			Button4 . BorderBrush = DlgInput . Btnborder;

			Button1 . BorderThickness = DlgInput . BorderSize;
			Button2 . BorderThickness = DlgInput . BorderSize;
			Button3 . BorderThickness = DlgInput . BorderSize;
			Button4 . BorderThickness = DlgInput . BorderSize;

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
		private void CheckForFinalDefaultSettings ( string iconstring )
		{
			if ( DlgInput . UseIcon && iconstring == "" )
			{
				if ( DlgInput . iconstring != "" )
					iconstring = DlgInput . iconstring;
				else
					DlgInput . iconstring = "\\icons\\check-mark-icon-5375.png";
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
		}

		#endregion Setup

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
				if ( Btn == 1 )   // OK Button
				{
					if ( Button1Text != null )
						Button1Text . Text = Button1Text . Text == "" ? Button1Text . Text : "OK";
					if ( DefaultButton == Btn )
						DefBorder = Button1;
					UpdateButtonX ( Btn , Button1 , false );
				}
				else if ( Btn == 2 )  // YES Button
				{
					if ( Button2Text != null )
						Button2Text . Text = Button2Text . Text == "" ? Button2Text . Text : "Yes";
					if ( DefaultButton == Btn )
						DefBorder = Button2;
					UpdateButtonX ( Btn , Button2 , false );
				}
				else if ( Btn == 3 )   // NO Button
				{
					if ( Button3Text != null )
						Button3Text . Text = Button3Text . Text == "" ? Button3Text . Text : "No";
					if ( DefaultButton == Btn )
						DefBorder = Button3;
					UpdateButtonX ( Btn , Button3 , false );
				}
				else if ( Btn == 4 )   // CANCEL Button
				{
					if ( Button4Text != null )
						Button4Text . Text = Button4Text . Text == "" ? Button4Text . Text : "Cancel";
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
			else
			{
				if ( CurrentButton == btn && DefaultButton == btn )
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
		private void SetBtnStatus ( int btnNum , Border Btn , bool isdef , bool ismouseover )
		{
			Thickness th = new Thickness();
			if ( IsTabbing )
			{
				if ( isdef == false )
				{     // Leaving a normal button, Reset to standard colors ?
					if ( TabPass == 1 )
					{
						// Resetting from being  the focused button
						Btn . Background = Btnbackground;
						SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
						// mouse is over button
						th . Left = 2;
						th . Top = 1;
						th . Right = 2;
						th . Bottom = 5;
					}
					else if ( TabPass == 2 )
					{
						// Setting up focus of NEW  Default button
						Btn . Background = DefBtnbackground;
						SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
						// mouse is over button
						th . Left = 2;
						th . Top = 1;
						th . Right = 2;
						th . Bottom = 5;
						//th . Left = 0;
						//th . Top = 0;
						//th . Right = 0;
						//th . Bottom = 0;
					}
				}
				else
				{
					// LEAVING a default button due to keystroke
					//Set back to DEFAULT setting
					if ( TabPass == 1 )
					{
						Btn . Background = Btnbackground;
						SetButtonForeground ( Btn , false , ismouseover );
						th . Left = 2;
						th . Top = 1;
						th . Right = 2;
						th . Bottom = 5;
					}
					else
					{
						Btn . Background = DefBtnbackground;
						SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
						th . Left = 2;
						th . Top = 1;
						th . Right = 2;
						th . Bottom = 5;
					}
				}
				Btn . BorderThickness = th;
			}
			else if ( ismouseover )
			{
				if ( isdef )
				{
					// Only used by DEFAULT  button
					Btn . Background = BtnMbackground;
					SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
					if ( ismouseover )
					{
						// mousehas entered and is over a Default button
						th . Left = 2;
						th . Top = 1;
						th . Right = 2;
						th . Bottom = 2;
					}
					else
					{
						// mouse has left Default button
						// This is the  DEFAULT BUTTONs standard setting
						th . Left = 2;
						th . Top = 2;
						th . Right = 2;
						th . Bottom = 5;
					}
				}
				else
				{
					// Mouse ENTERING a non default button
					//WORKING
					// NOT default button, so it is probably a mouseover entry  ?
					Btn . Background = BtnMbackground;
					SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
					th . Left = 1;
					th . Top = 3;
					th . Right = 1;
					th . Bottom = 1;
				}
				Btn . BorderThickness = th;
			}
			else
			{
				//Called when mouse leaves button
				if ( isdef == false )
				{
					// Mouse EXITING a non default button
					//WORKING
					Btn . Background = Btnbackground;
					SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
					th . Left = 1;
					th . Top = 1;
					th . Right = 1;
					th . Bottom = 5;
					Btn . BorderThickness = th;
				}
				else
				{
					// ?????
					// is the default button
					Btn . Background = DefBtnbackground;
					SetButtonForeground ( Btn , btnNum == DefaultButton , ismouseover );
					th . Left = 2;
					th . Top = 2;
					th . Right = 2;
					th . Bottom = 5;
					Btn . BorderThickness = th;
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
					Button1Text . Foreground = Btnforeground;
				}
				else if ( DefaultButton == 1 )
					{
						// IS the defaullt button
						if ( ismouseover )
					{
						Button1Text . Foreground = BtnMforeground;
					}
					else
					{
						Button1Text . Foreground = DefBtnforeground;
					}
				}
				else
				{
					// Not the defaullt button
					if ( ismouseover )
					{
						Button1Text . Foreground = BtnMforeground;
					}
					else
					{
						Button1Text . Foreground = Btnforeground;
					}
				}
			}
			if ( brdr . Name . Contains ( "2" ) )
			{
				if ( DefaultButton == 0 )
				{
					// We are just resetting to normal
					Button2Text . Foreground = Btnforeground;
				}
				else if ( DefaultButton == 2 )
				{
					// IS the defaullt button
					if ( ismouseover )
					{
						Button2Text . Foreground = DefBtnforeground;
					}
					else
					{
						Button2Text . Foreground = DefBtnforeground;
					}
				}
				else
				{
					// Not the defaullt button
					if ( ismouseover )
					{
						Button2Text . Foreground = BtnMforeground;
					}
					else
					{
						Button2Text . Foreground = Btnforeground;
					}
				}
			}
			if ( brdr . Name . Contains ( "3" ) )
			{
				if ( DefaultButton == 0 )
				{
					// We are just resetting to normal
					Button3Text . Foreground = Btnforeground;
				}
				else if ( DefaultButton == 3 )
				{
					// IS the defaullt button
					if ( ismouseover )
					{
						Button3Text . Foreground = BtnMforeground;
					}
					else
					{
						Button3Text . Foreground = DefBtnforeground;
					}
				}
				else
				{
					// Not the defaullt button
					if ( ismouseover )
					{
						Button3Text . Foreground = BtnMforeground;
					}
					else
					{
						Button3Text . Foreground = Btnforeground;
					}
				}
			}
			if ( brdr . Name . Contains ( "4" ) )
			{
				if ( DefaultButton == 0 )
				{
					// We are just resetting to normal
					Button4Text . Foreground = Btnforeground;
				}
				else if ( DefaultButton == 4 )
				{
					// IS the defaullt button
					if ( ismouseover )
					{
						Button4Text . Foreground = DefBtnforeground;
					}
					else
					{
						Button4Text . Foreground = DefBtnforeground;
					}
				}
				else
				{
					// Not the defaullt button
					if ( ismouseover )
					{
						Button4Text . Foreground = BtnMforeground;
					}
					else
					{
						Button4Text . Foreground = Btnforeground;
					}
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
			if ( e . Key == Key . LeftShift || e . Key == Key . RightShift )
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
		#endregion RoutedEvents

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

		private void Window_Deactivated ( object sender , EventArgs e )
		{
			this . Deactivated += ( s , e ) => this . Activate ( );
		}

		#region DP's

		public Brush DlgBackGround
		{
			get { return ( Brush ) GetValue ( DlgBackGroundProperty ); }
			set { SetValue ( DlgBackGroundProperty , value ); }
		}

		// Using a DependencyProperty as the backing store for BackGround.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DlgBackGroundProperty =
			DependencyProperty.Register("DlgBackGround", typeof(Brush), typeof(Msgbox), new PropertyMetadata((Brush)default),DlgBackgroundChanged);

		private static bool DlgBackgroundChanged ( object value )
		{
			Console . WriteLine ( $"DP : DlgBackground set to {value}" );
			return true;
		}

		public Brush DlgForeGround
		{
			get { return ( Brush ) GetValue ( DlgForeGroundProperty ); }
			set { SetValue ( DlgForeGroundProperty , value ); }
		}

		// Using a DependencyProperty as the backing store for DlgForeGround.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DlgForeGroundProperty =
			DependencyProperty.Register("DlgForeGround", typeof(Brush ), typeof(Msgbox), new PropertyMetadata(new SolidColorBrush (Colors.Black)));

		public Brush BtnBackGround
		{
			get { return ( Brush ) GetValue ( BtnBackGroundProperty ); }
			set { SetValue ( BtnBackGroundProperty , value ); }
		}
		// Using a DependencyProperty as the backing store for bkground.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BtnBackGroundProperty =
			DependencyProperty.Register("BtnBackGround", typeof(Brush), typeof(Msgbox), new PropertyMetadata(new SolidColorBrush (Colors.Violet)), BtnBackgroundChanged);

		private static bool BtnBackgroundChanged ( object value )
		{
			Console . WriteLine ( $"DP : BtnBackground set to {value}" );
			return true;
		}

		public Brush BtnForeGround
		{
			get { return ( Brush ) GetValue ( BtnForeGroundProperty ); }
			set { SetValue ( BtnForeGroundProperty , value ); }
		}
		// Using a DependencyProperty as the backing store for ForeGround.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BtnForeGroundProperty =
			DependencyProperty.Register("BtnForeGround", typeof(Brush), typeof(Msgbox), new PropertyMetadata(new SolidColorBrush (Colors.White)));

		public Brush MouseoverBackGround
		{
			get { return ( Brush ) GetValue ( MouseoverBackGroundProperty ); }
			set { SetValue ( MouseoverBackGroundProperty , value ); }
		}
		// Using a DependencyProperty as the backing store for MouseoverBackGround.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MouseoverBackGroundProperty =
			DependencyProperty.Register("MouseoverBackGround", typeof(Brush), typeof(Msgbox), new PropertyMetadata(new SolidColorBrush (Colors.Green)));

		public Brush MouseoverForeGround
		{
			get { return ( Brush ) GetValue ( MouseoverForeGroundProperty ); }
			set { SetValue ( MouseoverForeGroundProperty , value ); }
		}
		// Using a DependencyProperty as the backing store for MouseoverForeGround.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MouseoverForeGroundProperty =
			DependencyProperty.Register("MouseoverForeGround", typeof(Brush), typeof(Msgbox), new PropertyMetadata(new SolidColorBrush (Colors.White)));

		public Brush BorderColor
		{
			get { return ( Brush ) GetValue ( BorderColorProperty ); }
			set { SetValue ( BorderColorProperty , value ); }
		}
		// Using a DependencyProperty as the backing store for BorderColor.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BorderColorProperty =
			DependencyProperty.Register("BorderColor", typeof(Brush), typeof(Msgbox), new PropertyMetadata((Brush)default));

		public Thickness BorderSize
		{
			get { return ( Thickness ) GetValue ( BorderSizeProperty ); }
			set { SetValue ( BorderSizeProperty , value ); }
		}

		// Using a DependencyProperty as the backing store for BorderSize.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BorderSizeProperty =
			DependencyProperty.Register("BorderSize", typeof(Thickness ), typeof(Msgbox), new PropertyMetadata((Thickness )default));


		#endregion DP's

		#region routed events
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
		#endregion routed events


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
			b . Background = Btnbackground;
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
		private void ReadMsgboxData ( )
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
						SetCurrentValue ( Msgbox . DlgBackGroundProperty , DlgInput . dlgbackground );
						break;
					case 1:
						DlgInput . dlgforeground = Utils . GetNewBrush ( item );
						SetCurrentValue ( Msgbox . DlgForeGroundProperty , DlgInput . dlgforeground );
						break;
					case 2:
						DlgInput . btnbackground = Utils . GetNewBrush ( item );
						SetCurrentValue ( Msgbox . BtnBackGroundProperty , DlgInput . btnbackground );
						break;
					case 3:
						DlgInput . btnforeground = Utils . GetNewBrush ( item );
						SetCurrentValue ( Msgbox . BtnForeGroundProperty , DlgInput . btnforeground );
						break;
					case 4:
						DlgInput . Btnborder = Utils . GetNewBrush ( item );
						SetCurrentValue ( Msgbox . BorderColorProperty , DlgInput . Btnborder );
						break;
					case 5:
						DlgInput . mousebackground = Utils . GetNewBrush ( item );
						SetCurrentValue ( Msgbox . MouseoverBackGroundProperty , DlgInput . mousebackground );
						break;
					case 6:
						DlgInput . mouseforeground = Utils . GetNewBrush ( item );
						SetCurrentValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . mouseforeground );
						break;
					case 7:
						DlgInput . defbtnbackground = Utils . GetNewBrush ( item );
						//SetCurrentValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . mouseforeground );
						break;
					case 8:
						DlgInput . defbtnforeground = Utils . GetNewBrush ( item );
						//SetCurrentValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . mouseforeground );
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

			updateVars ( );
		}

		public static void updateVars ( )
		{
			Btnbackground = DlgInput . btnbackground;
			Btnforeground = DlgInput . btnforeground;
			BtnMbackground = DlgInput . mousebackground;
			BtnMforeground = DlgInput . mouseforeground;
			DefBtnbackground = DlgInput . defbtnbackground;
			DefBtnforeground = DlgInput . defbtnforeground;
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

			//DefaultButton . Focus ( );
			//if ( DefaultButton == Button1 )
			//{
			//	Button2 . Focus ( );
			//	Button1 . Focus ( );
			//}
			//if ( DefaultButton == Button2 )
			//{
			//	Button3 . Focus ( );
			//	Button2 . Focus ( );
			//}
			//if ( DefaultButton == Button3 )
			//{
			//	Button4 . Focus ( );
			//	Button3 . Focus ( );
			//}
			//if ( DefaultButton == Button4 )
			//{
			//	Button3 . Focus ( );
			//	Button4 . Focus ( );
			//}
		}

		#endregion NOT IN USE

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
	}
}


