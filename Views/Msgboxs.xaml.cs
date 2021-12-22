
using System;
using System . Collections . ObjectModel;

using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;

using WPFPages . AttachedProperties;

namespace WPFPages . Views
{
	public partial class Msgboxs : Window
	{

		#region rowdata

		string CaptionString { get; set; }
		string Row1String { get; set; }
		string Row2String { get; set; }
		string Row3String { get; set; }

		#endregion rowdata

		#region buttondata
		string Btn1Text { get; set; }
		string Btn2Text { get; set; }
		string Btn3Text { get; set; }
		string Btn4Text { get; set; }
		Brush Borderbrush { get; set; }

		int[] btnsarray = new int[4];
		string [ ] btnstextarray = { "","","",""};
		Button DefaultButton;

		#endregion buttondata

		#region Setup

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
				DlgInput . mouseforeground = "#FF000000" . ToSolidBrush ( );
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
			int defButton = 1 )
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
			 MB . NNULL ,
			 MB . NNULL ,
			 "" ,
			 "" ,
			 "" ,
			 ""
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
			string btn4Text = ""
			)
		{

			InitializeComponent ( );
			Utils . SetupWindowDrag ( this );
			bool result = CheckMsgBoxData ( );
//			ConfigureCurrentColors ( );
			LoadWindow (
			 caption ,
			string1 ,
			string2 = "" ,
			title = "" ,
			iconstring = "" ,
			 defButton = MB . OK ,
			 Btn1 = MB . OK ,
			 Btn2 = MB . NNULL ,
			 Btn3 = MB . NNULL ,
			 Btn4 = MB . NNULL ,
			 btn1Text ,
			 btn2Text ,
			 btn3Text ,
			 btn4Text
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
		string btn4Text
			)
		{
			CheckForInitialDefaultSettings ( );
			btnsarray [ 0 ] = Btn1;
			btnsarray [ 1 ] = Btn2;
			btnsarray [ 2 ] = Btn3;
			btnsarray [ 3 ] = Btn4;
			//iconstring = "\\icons\\check-mark-icon-5375 . png";
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

			DefaultButton . Focus ( );
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
						DefaultButton . Focus ( );
					}
					Button4 . IsCancel = true;
					UpdateButtonX ( Button4 );
				}
			}
			DefaultButton . Focus ( );
		}

		private void UpdateButtonX ( Button Btn )
		{
			Btn . Height = 35;
			Btn . Width = 100;
			Btn . Visibility = Visibility . Visible;
			Thickness th = new Thickness();
			th . Right = 10;
			Btn . Margin = th;
			if ( Btn . IsDefault == true )
			{
				Btn . BorderBrush = DlgInput . Btnborder;
				th . Left = 0;
				th . Right = 0;
				th . Top = 0;
				th . Bottom = 5;
				Btn . BorderThickness = th;
			}
			else
			{
				//b . BorderBrush = FindResource ( "Black0" ) as Brush;
				Btn . BorderBrush = DlgInput . Btnborder;
				th . Left = 0;
				th . Right = 0;
				th . Top = 0;
				th . Bottom = 0;
				Btn . BorderThickness = th;
			}

			// Set the buttons up so they opoerate
			Btn . UpdateLayout ( );
		}

		//Set up the basic Dlg colors etc
		void CheckForInitialDefaultSettings ( )
		{

			if ( DlgInput . UseDarkMode )
			{
				DlgInput . dlgbackground = "#FF000000" . ToSolidBrush ( );
				DlgInput . dlgforeground = "#FFFFFFFF" . ToSolidBrush ( );
				DlgInput . Btnborder = FindResource ( "Yellow1" ) as Brush;
				DlgInput . mouseforeground = FindResource ( "White0" ) as Brush;
				DlgInput . btnbackground = FindResource ( "Blue1" ) as Brush;
				DlgInput . btnforeground = FindResource ( "White0" ) as Brush;
				if ( DlgInput . Btnborder == null )
					DlgInput . Btnborder = "#E9FFF100" . ToSolidBrush ( ); // Yellow
			}
			else
			{
				DlgInput . dlgbackground = FindResource ( "White4" ) as Brush;
				DlgInput . dlgforeground = FindResource ( "Black0" ) as Brush;
				DlgInput . mouseforeground = FindResource ( "Black0" ) as Brush;
				//DlgInput . border = FindResource ( "Black0" ) as Brush;
				if ( DlgInput . btnbackground == null )
					DlgInput . btnbackground = FindResource ( "Red1" ) as Brush;
				if ( DlgInput . btnforeground == null )
					DlgInput . btnforeground = FindResource ( "White0" ) as Brush;
				if ( DlgInput . Btnborder == null )
					DlgInput . Btnborder = FindResource ( "White0" ) as Brush;
			}
			this . BoxGrid . Background = DlgInput . dlgbackground;
			this . Caption . Background = DlgInput . dlgbackground;
			this . Row1 . Background = DlgInput . dlgbackground;
			this . Row2 . Background = DlgInput . dlgbackground;
			this . Row1 . Foreground = DlgInput . dlgforeground;
			this . Row2 . Foreground = DlgInput . dlgforeground;

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

			// set  our DP properties
			//Dialog background
			//SetValue ( Msgbox.BkGroundProperty , DlgInput . bground );
			//// buttons
			//SetValue ( Msgbox . MouseoverForeGroundProperty , DlgInput . mousefground );
			//SetValue ( Msgbox . BorderSizeProperty , DlgInput . border );
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

		#endregion Setup

		#region Routed Event handlers ?
		public RoutedEvent ProcessOKBtn ( )
		{
			Console . WriteLine ( "OK button clicked" );
			Dlgresult . returnint = 1;
			this . Close ( );
			return null;
		}
		public RoutedEvent ProcessYesBtn ( )
		{
			Console . WriteLine ( "Yes button clicked" );
			Dlgresult . returnint = 2;
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
			if ( v != null )
				Console . WriteLine ( $"genericClass object :{v . field1}..." );
			else
				Console . WriteLine ( "Not  a genericClass object..." );
			ObservableCollection < GenericClass > vv= DlgInput . obj as ObservableCollection< GenericClass>;
			if ( vv != null )
				Console . WriteLine ( $"ObservableColllection identified : count = {vv . Count}" );
			else
				Console . WriteLine ( "Not  a ObservableColllection object..." );
			Dlgresult . obj = DlgInput . obj;
			this . Close ( );
			return null;
		}
		public RoutedEvent ProcessCancelBtn ( )
		{
			Console . WriteLine ( "Cancel button clicked" );
			Dlgresult . returnint = 4;
			this . Close ( );
			return null;
		}
		#endregion Routed Event handlers ?

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
			bool changed= false;

			if ( e . Key == Key . Tab || e . Key == Key . Right )
			{
				if ( Button2 . Visibility == Visibility . Visible )
				{
					Button2 . IsDefault = true;
					UpdateButtonX ( Button2 );
					changed = true;
				}
				else if ( Button3 . Visibility == Visibility . Visible )
				{
					Button3 . IsDefault = true;
					UpdateButtonX ( Button3 );
					changed = true;
				}
				else if ( Button4 . Visibility == Visibility . Visible )
				{
					Button4 . IsDefault = true;
					UpdateButtonX ( Button4 );
					changed = true;
				}
				if ( changed == false )
				{
					Button1 . IsDefault = true;
					UpdateButtonX ( Button1 );
				}
				else
				{
					Button1 . IsDefault = false;
					UpdateButtonX ( Button1 );
				}
			}
			else if ( e . Key == Key . Left )
			{
				if ( Button4 . Visibility == Visibility . Visible )
				{
					Button4 . IsDefault = true;
					UpdateButtonX ( Button4 );
					changed = true;
				}
				else if ( Button3 . Visibility == Visibility . Visible )
				{
					Button3 . IsDefault = true;
					UpdateButtonX ( Button3 );
					changed = true;
				}
				if ( Button2 . Visibility == Visibility . Visible )
				{
					Button2 . IsDefault = true;
					UpdateButtonX ( Button2 );
					changed = true;
				}
				if ( changed == false )
				{
					Button1 . IsDefault = true;
					UpdateButtonX ( Button1 );
				}
				else
				{
					Button1 . IsDefault = false;
					UpdateButtonX ( Button1 );
				}
			}
		}
		private void Button2_KeyDown ( object sender , KeyEventArgs e )
		{
			bool changed = false;
			if ( e . Key == Key . Tab || e . Key == Key . Right )
			{
				if ( Button3 . Visibility == Visibility . Visible )
				{
					Button3 . IsDefault = true;
					UpdateButtonX ( Button3 );
					changed = true;
				}
				else if ( Button4 . Visibility == Visibility . Visible )
				{
					Button4 . IsDefault = true;
					UpdateButtonX ( Button4 );
					changed = true;
				}
				else if ( Button1 . Visibility == Visibility . Visible )
				{
					Button1 . IsDefault = true;
					UpdateButtonX ( Button1 );
					changed = true;
				}
				if ( changed == false )
				{
					Button2 . IsDefault = true;
					UpdateButtonX ( Button2 );
				}
				else
				{
					Button2 . IsDefault = false;
					UpdateButtonX ( Button2 );
				}
			}
			else if ( e . Key == Key . Left )
			{
				if ( Button1 . Visibility == Visibility . Visible )
				{
					Button1 . IsDefault = true;
					UpdateButtonX ( Button1 );
					changed = true;
				}
				else if ( Button4 . Visibility == Visibility . Visible )
				{
					Button4 . IsDefault = true;
					UpdateButtonX ( Button4 );
					changed = true;
				}
				else if ( Button3 . Visibility == Visibility . Visible )
				{
					Button3 . IsDefault = true;
					UpdateButtonX ( Button3 );
					changed = true;
				}
				if ( changed == false )
				{
					Button2 . IsDefault = true;
					UpdateButtonX ( Button2 );
				}
				else
				{
					Button2 . IsDefault = false;
					UpdateButtonX ( Button2 );
				}
			}
		}
		private void Button3_KeyDown ( object sender , KeyEventArgs e )
		{
			bool changed=false;
			if ( e . Key == Key . Tab || e . Key == Key . Right )
			{
				if ( Button4 . Visibility == Visibility . Visible )
				{
					Button4 . IsDefault = true;
					UpdateButtonX ( Button4 );
					changed = true;
				}
				else if ( Button1 . Visibility == Visibility . Visible )
				{
					Button1 . IsDefault = true;
					UpdateButtonX ( Button1 );
					changed = true;
				}
				else if ( Button2 . Visibility == Visibility . Visible )
				{
					Button2 . IsDefault = true;
					UpdateButtonX ( Button2 );
					changed = true;
				}
				if ( changed == false )
				{
					Button3 . IsDefault = true;
					UpdateButtonX ( Button3 );
					changed = true;
				}
				else
				{
					Button3 . IsDefault = false;
					UpdateButtonX ( Button3 );
				}
			}
			else if ( e . Key == Key . Left )
			{
				if ( Button2 . Visibility == Visibility . Visible )
				{
					Button2 . IsDefault = true;
					UpdateButtonX ( Button2 );
					changed = true;
				}
				else if ( Button1 . Visibility == Visibility . Visible )
				{
					Button1 . IsDefault = true;
					UpdateButtonX ( Button1 );
					changed = true;
				}
				else if ( Button4 . Visibility == Visibility . Visible )
				{
					Button4 . IsDefault = true;
					UpdateButtonX ( Button4 );
					changed = true;
				}
				if ( changed == false )
				{
					Button3 . IsDefault = true;
					UpdateButtonX ( Button3 );
				}
				else
				{
					Button3 . IsDefault = false;
					UpdateButtonX ( Button3 );
				}
			}
		}
		private void Button4_KeyDown ( object sender , KeyEventArgs e )
		{
			bool changed=false;
			if ( e . Key == Key . Tab || e . Key == Key . Right )
			{
				if ( Button1 . Visibility == Visibility . Visible )
				{
					Button1 . IsDefault = true;
					UpdateButtonX ( Button1 );
					changed = true;
				}
				else if ( Button2 . Visibility == Visibility . Visible )
				{
					Button2 . IsDefault = true;
					UpdateButtonX ( Button2 );
					changed = true;
				}
				else if ( Button3 . Visibility == Visibility . Visible )
				{
					Button3 . IsDefault = true;
					UpdateButtonX ( Button3 );
					changed = true;
				}
				if ( changed == false )
				{
					Button4 . IsDefault = true;
					UpdateButtonX ( Button4 );
				}
				else
				{
					Button4 . IsDefault = false;
					UpdateButtonX ( Button4 );
				}
			}
			else if ( e . Key == Key . Left )
			{
				if ( Button3 . Visibility == Visibility . Visible )
				{
					Button3 . IsDefault = true;
					UpdateButtonX ( Button3 );
					changed = true;
				}
				else if ( Button2 . Visibility == Visibility . Visible )
				{
					Button2 . IsDefault = true;
					UpdateButtonX ( Button2 );
					changed = true;
				}
				else if ( Button1 . Visibility == Visibility . Visible )
				{
					Button1 . IsDefault = true;
					UpdateButtonX ( Button1 );
					changed = true;
				}

				if ( changed == false )
				{
					Button4 . IsDefault = true;
					UpdateButtonX ( Button4 );
				}
				else
				{
					Button4 . IsDefault = false;
					UpdateButtonX ( Button4 );
				}
			}
		}

		#endregion Button Handlers

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

		
		// Force the dialog to Keep Focus inside  the caller app (flashes if another gets focus)
		private void Window_Deactivated ( object sender , EventArgs e )
		{
			this . Deactivated += ( s , e ) => this . Activate ( );
		}

		//#region DP's
		//public Brush DlgBackGround
		//{
		//	get { return ( Brush ) GetValue ( DlgBackGroundProperty ); }
		//	set { SetValue ( DlgBackGroundProperty , value ); }
		//}

		//// Using a DependencyProperty as the backing store for BackGround.  This enables animation, styling, binding, etc...
		//public static readonly DependencyProperty DlgBackGroundProperty =
		//	DependencyProperty.Register("DlgBackGround", typeof(Brush), typeof(Msgbox), new PropertyMetadata((Brush)default));

		//public Brush DlgForeGround
		//{
		//	get { return ( Brush ) GetValue ( DlgForeGroundProperty ); }
		//	set { SetValue ( DlgForeGroundProperty , value ); }
		//}
		//// Using a DependencyProperty as the backing store for DlgForeGround.  This enables animation, styling, binding, etc...
		//public static readonly DependencyProperty DlgForeGroundProperty =
		//	    DependencyProperty.Register("DlgForeGround", typeof(Brush ), typeof(Msgbox), new PropertyMetadata((Brush)default));

		//public Brush BkGround
		//{
		//	get { return ( Brush ) GetValue ( BkGroundProperty ); }
		//	set { SetValue ( BkGroundProperty , value ); }
		//}
		//// Using a DependencyProperty as the backing store for bkground.  This enables animation, styling, binding, etc...
		//public static readonly DependencyProperty BkGroundProperty =
		//	DependencyProperty.Register("BkGround", typeof(Brush), typeof(Msgboxs), new PropertyMetadata((Brush)default));

		//public Brush ForeGround
		//{
		//	get { return ( Brush ) GetValue ( ForeGroundProperty ); }
		//	set { SetValue ( ForeGroundProperty , value ); }
		//}
		//// Using a DependencyProperty as the backing store for ForeGround.  This enables animation, styling, binding, etc...
		//public static readonly DependencyProperty ForeGroundProperty =
		//	DependencyProperty.Register("ForeGround", typeof(Brush), typeof(Msgboxs), new PropertyMetadata((Brush)default));

		//public Brush MouseoverBackGround
		//{
		//	get { return ( Brush ) GetValue ( MouseoverBackGroundProperty ); }
		//	set { SetValue ( MouseoverBackGroundProperty , value ); }
		//}
		//// Using a DependencyProperty as the backing store for MouseoverBackGround.  This enables animation, styling, binding, etc...
		//public static readonly DependencyProperty MouseoverBackGroundProperty =
		//	DependencyProperty.Register("MouseoverBackGround", typeof(Brush), typeof(Msgboxs), new PropertyMetadata((Brush)default));

		//public Brush MouseoverForeGround
		//{
		//	get { return ( Brush ) GetValue ( MouseoverForeGroundProperty ); }
		//	set { SetValue ( MouseoverForeGroundProperty , value ); }
		//}
		//// Using a DependencyProperty as the backing store for MouseoverForeGround.  This enables animation, styling, binding, etc...
		//public static readonly DependencyProperty MouseoverForeGroundProperty =
		//	DependencyProperty.Register("MouseoverForeGround", typeof(Brush), typeof(Msgboxs), new PropertyMetadata((Brush)default));

		//public Brush BorderColor
		//{
		//	get { return ( Brush ) GetValue ( BorderColorProperty ); }
		//	set { SetValue ( BorderColorProperty , value ); }
		//}
		//// Using a DependencyProperty as the backing store for BorderColor.  This enables animation, styling, binding, etc...
		//public static readonly DependencyProperty BorderColorProperty =
		//	DependencyProperty.Register("BorderColor", typeof(Brush), typeof(Msgboxs), new PropertyMetadata((Brush)default));

		//public Thickness BorderSize
		//{
		//	get { return ( Thickness ) GetValue ( BorderSizeProperty ); }
		//	set { SetValue ( BorderSizeProperty , value ); }
		//}

		//// Using a DependencyProperty as the backing store for BorderSize.  This enables animation, styling, binding, etc...
		//public static readonly DependencyProperty BorderSizeProperty =
		//	DependencyProperty.Register("BorderSize", typeof(Thickness ), typeof(Msgboxs), new PropertyMetadata((Thickness )default));

		//#endregion DP's



	}
}

