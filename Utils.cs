#define SHOWWINDOWDATA
using Microsoft . Win32;

using System;
using System . Collections . Generic;
using System . Configuration;
using System . Data;
using System . Diagnostics;
using System . IO;
using System . Runtime . CompilerServices;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Controls . Primitives;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;

using WPFPages . Properties;
using WPFPages . ViewModels;
using WPFPages . Views;

namespace WPFPages
{
	/// <summary>
	/// Class to handle various utility functions such as fetching 
	/// Style/Templates/Brushes etc to Set/Reset control styles 
	/// from various Dictionary sources for use in "code behind"
	/// </summary>
	public class Utils
	{
		public static Action<DataGrid, int> GridInitialSetup = Utils . SetUpGridSelection;
		//		public static Func<bool, BankAccountViewModel, CustomerViewModel, DetailsViewModel> IsMatched = CheckRecordMatch; 
		public static Func<object, object, bool> IsRecordMatched = Utils . CompareDbRecords;

		// list each window that wants to support control capture needs to have so
		// mousemove can add current item under cursor to the list, and then F11 will display it.
		public static List<HitTestResult> ControlsHitList = new List<HitTestResult>();

		#region structures
		public struct bankrec
		{
			public string custno
			{
				get; set;
			}
			public string bankno
			{
				get; set;
			}
			public int actype
			{
				get; set;
			}
			public decimal intrate
			{
				get; set;
			}
			public decimal balance
			{
				get; set;
			}
			public DateTime odate
			{
				get; set;
			}
			public DateTime cdate
			{
				get; set;
			}
		}
		#endregion structures

		#region play tunes / sounds
		// Declare the first few notes of the song, "Mary Had A Little Lamb".
		// Define the frequencies of notes in an octave, as well as
		// silence (rest).
		protected enum Tone
		{
			REST = 0,
			GbelowC = 196,
			A = 220,
			Asharp = 233,
			B = 247,
			C = 262,
			Csharp = 277,
			D = 294,
			Dsharp = 311,
			E = 330,
			F = 349,
			Fsharp = 370,
			G = 392,
			Gsharp = 415,
		}

		// Define the duration of a note in units of milliseconds.
		protected enum Duration
		{
			WHOLE = 1600,
			HALF = WHOLE / 2,
			QUARTER = HALF / 2,
			EIGHTH = QUARTER / 2,
			SIXTEENTH = EIGHTH / 2,
		}

		protected struct Note
		{
			Tone toneVal;
			Duration durVal;

			// Define a constructor to create a specific note.
			public Note ( Tone frequency , Duration time )
			{
				toneVal = frequency;
				durVal = time;
			}

			// Define properties to return the note's tone and duration.
			public Tone NoteTone
			{
				get
				{
					return toneVal;
				}
			}
			public Duration NoteDuration
			{
				get
				{
					return durVal;
				}
			}
		}
		public static void PlayMary ( )
		{
			Note [ ] Mary =
				{
						    new Note(Tone.B, Duration.QUARTER),
						    new Note(Tone.A, Duration.QUARTER),
						    new Note(Tone.GbelowC, Duration.QUARTER),
						    new Note(Tone.A, Duration.QUARTER),
						    new Note(Tone.B, Duration.QUARTER),
						    new Note(Tone.B, Duration.QUARTER),
						    new Note(Tone.B, Duration.HALF),
						    new Note(Tone.A, Duration.QUARTER),
						    new Note(Tone.A, Duration.QUARTER),
						    new Note(Tone.A, Duration.HALF),
						    new Note(Tone.B, Duration.QUARTER),
						    new Note(Tone.D, Duration.QUARTER),
						    new Note(Tone.D, Duration.HALF)
				};
			// Play the song
			Play ( Mary );
		}
		// Play the notes in a song.
		protected static void Play ( Note [ ] tune )
		{
			foreach ( Note n in tune )
			{
				if ( n . NoteTone == Tone . REST )
					Thread . Sleep ( ( int ) n . NoteDuration );
				else
					Console . Beep ( ( int ) n . NoteTone , ( int ) n . NoteDuration );
			}
		}
		// Define a note as a frequency (tone) and the amount of
		//// time (duration) the note plays.
		//public static Task DoBeep ( int freq = 180, int count = 300, bool swap = false )
		//{
		//	int tone = freq;
		//	int duration = count;
		//	int x = 0;
		//	Task t = new Task ( ( ) => x = 1 );
		//	if ( Flags . UseBeeps )
		//	{
		//		if ( swap )
		//		{
		//			tone = ( tone / 4 ) * 3;
		//			duration = ( count * 5 ) / 2;
		//			t = Task . Factory . StartNew ( ( ) => Console . Beep ( freq, count ) )
		//				. ContinueWith ( Action => Console . Beep ( tone, duration ) );
		//			Thread . Sleep ( 500 );
		//		}
		//		else
		//		{
		//			tone = ( tone / 4 ) * 3;
		//			duration = ( count * 5 ) / 2;
		//			t = Task . Factory . StartNew ( ( ) => Console . Beep ( tone, duration ) )
		//				. ContinueWith ( Action => Console . Beep ( freq, count ) );
		//			Thread . Sleep ( 500 );
		//		}
		//	}
		//	else
		//	{
		//		Task task = Task . Factory . StartNew ( ( ) => Console . WriteLine ( ) );
		//		t = task ,TaskScheduler . FromCurrentSynchronizationContext ( );
		//			}

		//	TaskScheduler . FromCurrentSynchronizationContext ( ));
		//	return t;
		//}
		public static void DoSingleBeep ( int freq = 280 , int count = 300 , int repeat = 1 )
		{
			//			int x = 0;
			//			int i = 0;
			//Lambda test
			//			Task t = new Task ( ( ) => x = 1 );
			if ( Flags . UseBeeps )
			{
				for ( int i = 0 ; i < repeat ; i++ )
				{
					Console . Beep ( freq , count );
					Thread . Sleep ( 200 );
				}
				//else
				//	t = Task . Factory . StartNew ( ( ) => Console . WriteLine ( ) );
				//			return t;
			}
		}
		public static Task DoErrorBeep ( int freq = 280 , int count = 100 , int repeat = 3 )
		{
			//			int x = 0;
			//			Task t = new Task ( ( ) => x = 1 );
			if ( Flags . UseBeeps )
			{
				for ( int i = 0 ; i < repeat ; i++ )
				{
					Console . Beep ( freq , count );
				}
				Thread . Sleep ( 100 );
			}
			return null;
		}

		#endregion play tunes / sounds


		// Record the names of the method that called this one in an iterative tree.
		public static string trace ( string prompt = "" )
		{
			// logs all the calls made upwards in a tree
			string output="", tmp="";
			int indx = 1;
			var v  = new StackTrace ( 0 );
			if ( prompt != "" )
				output = prompt + "\nStackTrace :\n";
			while ( true )
			{
				try
				{

					tmp = v . GetFrame ( indx++ ) . GetMethod ( ) . Name + '\n';
					if ( tmp . Contains ( "Invoke" ) || tmp . Contains ( "RaiseEvent" ) )
						break;
					else
						output += tmp;
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( "Crashed...\n" );
					output += "\nCrashed...\n";
					break;
				}
			}
			Console . WriteLine ( $"\n{output}\n" );
			return $"\n{output}\n";
		}
		public static void Mbox ( Window win , string string1 = "" , string string2 = "" , string caption = "" , string iconstring = "" , int Btn1 = 1 , int Btn2 = 0 , int Btn3 = 0 , int Btn4 = 0 , int defButton = 1 , bool minsize = false , bool modal = false )
		{
			// We NEED to remove any \r as part of \r\n as textboxes ONLY accept \n on its own for Cr/Lf
			string1 = ParseforCR ( string1 );
			Msgboxs m = new Msgboxs( string1:string1,  string2:string2, caption:caption ,Btn1:Btn1, Btn2 : Btn2, Btn3 : Btn3, Btn4 : Btn4, defButton : defButton , iconstring:iconstring, MinSize:minsize , modal:modal);
			//			m . Owner = win;

			if ( modal == false )
				m . Show ( );
			else
				m . ShowDialog ( );
		}

		public static void Mssg (
				string caption = "" ,
				string string1 = "" ,
				string string2 = "" ,
				string string3 = "" ,
				string title = "" ,
				string iconstring = "" ,
				int defButton = 1 ,
				int Btn1 = 1 ,
				int Btn2 = 2 ,
				int Btn3 = 3 ,
				int Btn4 = 4 ,
				string btn1Text = "" ,
				string btn2Text = "" ,
				string btn3Text = "" ,
				string btn4Text = "" ,
				bool usedialog = true
			     )
		{
			Msgbox msg = new Msgbox(
				caption:caption ,
				string1:string1,
				string2:string2,
				string3:string3,
				title:title,
				Btn1:Btn1,
				Btn2 : Btn2,
				Btn3 : Btn3,
				Btn4 : Btn4,
				defButton : defButton ,
				iconstring:iconstring,
				btn1Text:btn1Text,
				btn2Text:btn2Text,
				btn3Text:btn3Text,
				btn4Text:btn4Text );
			//msg . Owner = win;
			if ( usedialog )
				msg . ShowDialog ( );
			else
				msg . Show ( );
		}

		public static string convertToHex ( double temp )
		{
			int intval = ( int ) Convert . ToInt32 ( temp );
			string hexval = intval . ToString ( "X" );
			return hexval;
		}
		//Working well 4/8/21
		/// <summary>
		/// Accepts color in Colors.xxxx format = "Blue" etc
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Brush BrushFromColors ( Color color )
		{
			Brush brush = new SolidColorBrush ( color );
			return brush;
		}
		//Working well 4/8/21
		/// <summary>
		/// Accpets string in "#XX00FF00" or similar
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Brush BrushFromHashString ( string color )
		{
			//Must start with  '#'
			string s = color . ToString ( );
			if ( !s . Contains ( "#" ) )
				return Utils . BrushFromColors ( Colors . Transparent );
			Brush brush = ( Brush ) new BrushConverter ( ) . ConvertFromString ( color );
			return brush;
		}
		public static bool CheckForExistingGuid ( Guid guid )
		{
			bool retval = false;
			for ( int x = 0 ; x < Flags . DbSelectorOpen . ViewersList . Items . Count ; x++ )
			{
				ListBoxItem lbi = new ListBoxItem ( );
				//lbi.Tag = viewer.Tag;
				lbi = Flags . DbSelectorOpen . ViewersList . Items [ x ] as ListBoxItem;
				if ( lbi . Tag == null )
					return retval;
				Guid g = ( Guid ) lbi . Tag;
				if ( g == guid )
				{
					retval = true;
					break;
				}
			}
			return retval;
		}
		public static bool CheckRecordMatch ( BankAccountViewModel bvm , CustomerViewModel cvm , DetailsViewModel dvm )
		{
			bool result = false;
			if ( bvm != null && cvm != null )
			{
				if ( bvm . CustNo == cvm . CustNo )
					result = true;
			}
			else if ( bvm != null && dvm != null )
			{
				if ( bvm . CustNo == dvm . CustNo )
					result = true;
			}
			else if ( cvm != null && dvm != null )
			{
				if ( cvm . CustNo == dvm . CustNo )
					result = true;
			}
			return result;
		}
		public static bool CompareDbRecords ( object obj1 , object obj2 )
		{
			bool result = false;
			BankAccountViewModel bvm = new BankAccountViewModel ( );
			CustomerViewModel cvm = new CustomerViewModel ( );
			DetailsViewModel dvm = new DetailsViewModel ( );
			//bvm = null;
			//cvm = null;
			//dvm = null;
			if ( obj1 == null || obj2 == null )
				return result;
			if ( obj1 . GetType ( ) == bvm . GetType ( ) )
				bvm = obj1 as BankAccountViewModel;
			if ( obj1 . GetType ( ) == cvm . GetType ( ) )
				cvm = obj1 as CustomerViewModel;
			if ( obj1 . GetType ( ) == dvm . GetType ( ) )
				dvm = obj1 as DetailsViewModel;

			if ( obj2 . GetType ( ) == bvm . GetType ( ) )
				bvm = obj2 as BankAccountViewModel;
			if ( obj2 . GetType ( ) == cvm . GetType ( ) )
				cvm = obj2 as CustomerViewModel;
			if ( obj2 . GetType ( ) == dvm . GetType ( ) )
				dvm = obj2 as DetailsViewModel;

			if ( bvm != null && cvm != null )
			{
				if ( bvm . CustNo == cvm . CustNo )
					result = true;
			}
			else if ( bvm != null && dvm != null )
			{
				if ( bvm . CustNo == dvm . CustNo )
					result = true;
			}
			else if ( cvm != null && dvm != null )
			{
				if ( cvm . CustNo == dvm . CustNo )
					result = true;
			}
			result = false;
			return result;
		}
		public static string ConvertInputDate ( string datein )
		{
			string YYYMMDD = "";
			string [ ] datebits;
			// This filter will strip off the "Time" section of an excel date
			// and return us a valid YYYY/MM/DD string
			char [ ] ch = { '/', ' ' };
			datebits = datein . Split ( ch );
			if ( datebits . Length < 3 )
				return datein;

			// check input to see if it needs reversing ?
			if ( datebits [ 0 ] . Length == 4 )
				YYYMMDD = datebits [ 0 ] + "/" + datebits [ 1 ] + "/" + datebits [ 2 ];
			else
				YYYMMDD = datebits [ 2 ] + "/" + datebits [ 1 ] + "/" + datebits [ 0 ];
			return YYYMMDD;
		}
		public static BankAccountViewModel CreateBankRecordFromString ( string type , string input )
		{
			int index = 0;
			BankAccountViewModel bvm = new BankAccountViewModel ( );
			char [ ] s = { ',' };
			string [ ] data = input . Split ( s );
			string donor = data [ 0 ];
			try
			{
				DateTime dt;
				if ( type == "BANK" || type == "DETAILS" )
				{
					// This WORKS CORRECTLY 12/6/21 when called from n SQLDbViewer DETAILS grid entry && BANK grid entry					
					// this test confirms the data layout by finding the Odate field correctly
					// else it drops thru to the Catch branch
					dt = Convert . ToDateTime ( data [ 7 ] );
					//We can have any type of record in the string recvd
					index = 1;  // jump the data type string
					bvm . Id = int . Parse ( data [ index++ ] );
					bvm . CustNo = data [ index++ ];
					bvm . BankNo = data [ index++ ];
					bvm . AcType = int . Parse ( data [ index++ ] );
					bvm . IntRate = decimal . Parse ( data [ index++ ] );
					bvm . Balance = decimal . Parse ( data [ index++ ] );
					bvm . ODate = Convert . ToDateTime ( data [ index++ ] );
					bvm . CDate = Convert . ToDateTime ( data [ index ] );
					return bvm;
				}
				else if ( type == "CUSTOMER" )
				{
					// this test confirms the data layout by finding the Odate field correctly
					// else it drops thru to the Catch branch
					dt = Convert . ToDateTime ( data [ 5 ] );
					// We have a customer record !!
					//Check to see if the data includes the data type in it
					//As we have to parse it diffrently if not - see index....
					index = 1;
					bvm . Id = int . Parse ( data [ index++ ] );
					bvm . CustNo = data [ index++ ];
					bvm . BankNo = data [ index++ ];
					bvm . AcType = int . Parse ( data [ index++ ] );
					bvm . ODate = Convert . ToDateTime ( data [ index++ ] );
					bvm . CDate = Convert . ToDateTime ( data [ index ] );
				}
				return bvm;
			}
			catch
			{
				//Check to see if the data includes the data type in it
				//As we have to parse it diffrently if not - see index....
				index = 0;
				try
				{
					int x = int . Parse ( donor );
					// if we get here, it IS a NUMERIC VALUE
					index = 0;
				}
				catch
				{
					//its probably the Data Type string, so ignore it for our Data creation processing
					index = 1;
				}
				//We have a CUSTOMER record
				bvm . Id = int . Parse ( data [ index++ ] );
				bvm . CustNo = data [ index++ ];
				bvm . BankNo = data [ index++ ];
				bvm . AcType = int . Parse ( data [ index++ ] );
				bvm . ODate = Convert . ToDateTime ( data [ index++ ] );
				bvm . CDate = Convert . ToDateTime ( data [ index ] );
				return bvm;
			}
		}
		public static BankDragviewModel CreateBankGridRecordFromString ( string input )
		{
			int index = 1;
			string type = "";
			//			BankAccountViewModel bvm = new BankAccountViewModel ( );
			BankDragviewModel bvm = new BankDragviewModel ( );
			CustomerDragviewModel cvm = new CustomerDragviewModel ( );

			char [ ] s = { ',' };
			string [ ] data = input . Split ( s );
			string donor = data [ 0 ];
			try
			{
				DateTime dt;
				type = data [ 0 ];
				if ( type == "BANKACCOUNT" || type == "BANK" || type == "DETAILS" )
				{
					// This WORKS CORRECTLY 12/6/21 when called from n SQLDbViewer DETAILS grid entry && BANK grid entry					
					// this test confirms the data layout by finding the Odate field correctly
					// else it drops thru to the Catch branch
					dt = Convert . ToDateTime ( data [ 7 ] );
					//We can have any type of record in the string recvd
					index = 1;  // jump the data type string
					bvm . RecordType = type;
					bvm . Id = int . Parse ( data [ index++ ] );
					bvm . CustNo = data [ index++ ];
					bvm . BankNo = data [ index++ ];
					bvm . AcType = int . Parse ( data [ index++ ] );
					bvm . IntRate = decimal . Parse ( data [ index++ ] );
					bvm . Balance = decimal . Parse ( data [ index++ ] );
					bvm . ODate = Convert . ToDateTime ( data [ index++ ] );
					bvm . CDate = Convert . ToDateTime ( data [ index ] );
					return bvm;
				}
			}
			catch
			{
				//Check to see if the data includes the data type in it
				//As we have to parse it diffrently if not - see index....
				index = 0;
				try
				{
					int x = int . Parse ( donor );
					// if we get here, it IS a NUMERIC VALUE
					index = 0;
				}
				catch ( Exception ex )
				{
					//its probably the Data Type string, so ignore it for our Data creation processing
					index = 1;
				}
				//We have a CUSTOMER record
				bvm . RecordType = type;
				bvm . Id = int . Parse ( data [ index++ ] );
				bvm . CustNo = data [ index++ ];
				bvm . BankNo = data [ index++ ];
				bvm . AcType = int . Parse ( data [ index++ ] );
				bvm . ODate = Convert . ToDateTime ( data [ index++ ] );
				bvm . CDate = Convert . ToDateTime ( data [ index ] );
				return bvm;
			}
			return bvm;
		}
		public static RenderTargetBitmap CreateControlImage ( FrameworkElement control , string filename = "" , bool savetodisk = false , GrabImageArgs ga = null )
		{
			if ( control == null )
				return null;
			// Get the Visual (Control) itself and the size of the Visual and its descendants.
			// This is the clever bit that gets the requested control, not the full window
			Rect rect = VisualTreeHelper.GetDescendantBounds(control);

			// Make a DrawingVisual to make a screen
			// representation of the control.
			DrawingVisual dv = new DrawingVisual();

			// Fill a rectangle the same size as the control
			// with a brush containing images of the control.
			using ( DrawingContext ctx = dv . RenderOpen ( ) )
			{
				VisualBrush brush = new VisualBrush(control);
				ctx . DrawRectangle ( brush , null , new Rect ( rect . Size ) );
			}

			// Make a bitmap and draw on it.
			int width = (int)control.ActualWidth;
			int height = (int)control.ActualHeight;
			if ( height == 0 || width == 0 )
				return null;
			RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
			rtb . Render ( dv );
			if ( savetodisk && filename != "" )
				SaveImageToFile ( rtb , filename );
			return rtb;
		}
		public static CustomerDragviewModel CreateCustGridRecordFromString ( string input )
		{
			int index = 0;
			string type = "";
			//			BankAccountViewModel bvm = new BankAccountViewModel ( );
			CustomerDragviewModel cvm = new CustomerDragviewModel ( );

			char [ ] s = { ',' };
			string [ ] data = input . Split ( s );
			string donor = data [ 0 ];
			try
			{
				DateTime dt;
				type = data [ 0 ];
				// this test confirms the data layout by finding the Dob field correctly
				// else it drops thru to the Catch branch
				dt = Convert . ToDateTime ( data [ 10 ] );
				// We have a customer record !!
				//Check to see if the data includes the data type in it
				//As we have to parse it diffrently if not - see index....
				index = 0;
				cvm . RecordType = type;
				cvm . Id = int . Parse ( data [ index++ ] );
				cvm . CustNo = data [ index++ ];
				cvm . BankNo = data [ index++ ];
				cvm . AcType = int . Parse ( data [ index++ ] );

				cvm . FName = data [ index++ ];
				cvm . LName = data [ index++ ];
				cvm . Town = data [ index++ ];
				cvm . County = data [ index++ ];
				cvm . PCode = data [ index++ ];

				cvm . Dob = Convert . ToDateTime ( data [ index++ ] );
				cvm . ODate = Convert . ToDateTime ( data [ index++ ] );
				cvm . CDate = Convert . ToDateTime ( data [ index ] );
				return cvm;
			}
			catch
			{
				//Check to see if the data includes the data type in it
				//As we have to parse it diffrently if not - see index....
				index = 0;
				try
				{
					int x = int . Parse ( donor );
					// if we get here, it IS a NUMERIC VALUE
					index = 0;
				}
				catch ( Exception ex )
				{
					//its probably the Data Type string, so ignore it for our Data creation processing
					index = 1;
				}
			}
			return cvm;
		}
		public static CustomerViewModel CreateCustomerRecordFromString ( string input )
		{
			int index = 1;
			CustomerViewModel cvm = new CustomerViewModel ( );
			char [ ] s = { ',' };
			string [ ] data = input . Split ( s );
			string donor = data [ 0 ];
			//Check to see if the data includes the data type in it
			//As we have to parse it diffrently if not - see index....
			//if ( donor . Length > 3 )
			//        index =0 ;
			//We have the sender type in the string recvd
			cvm . Id = int . Parse ( data [ index++ ] );
			cvm . CustNo = data [ index++ ];
			cvm . BankNo = data [ index++ ];
			cvm . AcType = int . Parse ( data [ index++ ] );
			cvm . FName = data [ index++ ];
			cvm . LName = data [ index++ ];
			cvm . Addr1 = data [ index++ ];
			cvm . Addr2 = data [ index++ ];
			cvm . Town = data [ index++ ];
			cvm . County = data [ index++ ];
			cvm . PCode = data [ index++ ];
			cvm . Phone = data [ index++ ];
			cvm . Mobile = data [ index++ ];
			cvm . Dob = DateTime . Parse ( data [ index++ ] );
			cvm . ODate = DateTime . Parse ( data [ index++ ] );
			cvm . CDate = DateTime . Parse ( data [ index ] );
			return cvm;
		}
		public static DetailsViewModel CreateDetailsRecordFromString ( string input )
		{
			int index = 0;
			DetailsViewModel bvm = new DetailsViewModel ( );
			char [ ] s = { ',' };
			string [ ] data = input . Split ( s );
			string donor = data [ 0 ];
			//Check to see if the data includes the data type in it
			//As we have to parse it diffrently if not - see index....
			if ( donor . Length > 3 )
				index = 1;
			bvm . Id = int . Parse ( data [ index++ ] );
			bvm . CustNo = data [ index++ ];
			bvm . BankNo = data [ index++ ];
			bvm . AcType = int . Parse ( data [ index++ ] );
			bvm . IntRate = decimal . Parse ( data [ index++ ] );
			bvm . Balance = decimal . Parse ( data [ index++ ] );
			bvm . ODate = DateTime . Parse ( data [ index++ ] );
			bvm . CDate = DateTime . Parse ( data [ index ] );
			return bvm;
		}
		public static string CreateDragDataFromRecord ( BankDragviewModel bvm )
		{
			if ( bvm == null )
				return "";
			string datastring = "";
			datastring = bvm . RecordType + ",";
			datastring += bvm . Id + ",";
			datastring += bvm . CustNo + ",";
			datastring += bvm . BankNo + ",";
			datastring += bvm . AcType . ToString ( ) + ",";
			datastring += bvm . IntRate . ToString ( ) + ",";
			datastring += bvm . Balance . ToString ( ) + ",";
			datastring += "'" + bvm . CDate . ToString ( ) + "',";
			datastring += "'" + bvm . ODate . ToString ( ) + "',";
			return datastring;
		}
		public static string CreateFullCsvTextFromRecord ( BankAccountViewModel bvm , DetailsViewModel dvm , CustomerViewModel cvm = null , bool IncludeType = true )
		{
			if ( bvm == null && cvm == null && dvm == null )
				return "";
			string datastring = "";
			if ( bvm != null )
			{
				// Handle a BANK Record
				if ( IncludeType )
					datastring = "BANKACCOUNT";
				datastring += bvm . Id + ",";
				datastring += bvm . CustNo + ",";
				datastring += bvm . BankNo + ",";
				datastring += bvm . AcType . ToString ( ) + ",";
				datastring += bvm . IntRate . ToString ( ) + ",";
				datastring += bvm . Balance . ToString ( ) + ",";
				datastring += "'" + bvm . CDate . ToString ( ) + "',";
				datastring += "'" + bvm . ODate . ToString ( ) + "',";
			}
			else if ( dvm != null )
			{
				if ( IncludeType )
					datastring = "DETAILS,";
				datastring += dvm . Id + ",";
				datastring += dvm . CustNo + ",";
				datastring += dvm . BankNo + ",";
				datastring += dvm . AcType . ToString ( ) + ",";
				datastring += dvm . IntRate . ToString ( ) + ",";
				datastring += dvm . Balance . ToString ( ) + ",";
				datastring += "'" + dvm . CDate . ToString ( ) + "',";
				datastring += dvm . ODate . ToString ( ) + ",";
			}
			else if ( cvm != null )
			{
				if ( IncludeType )
					datastring = "CUSTOMER,";
				datastring += cvm . Id + ",";
				datastring += cvm . CustNo + ",";
				datastring += cvm . BankNo + ",";
				datastring += cvm . AcType . ToString ( ) + ",";
				datastring += "'" + cvm . CDate . ToString ( ) + "',";
				datastring += cvm . ODate . ToString ( ) + ",";
			}
			return datastring;
		}
		public static bool DataGridHasFocus ( DependencyObject instance )
		{
			//how to fibnd out whether a datagrid has focus or not to handle key previewers
			IInputElement focusedControl = FocusManager . GetFocusedElement ( instance );
			if ( focusedControl == null )
				return true;
			string compare = focusedControl . ToString ( );
			if ( compare . ToUpper ( ) . Contains ( "DATAGRID" ) )
				return true;
			else
				return false;
		}
		public static DependencyObject FindChild ( DependencyObject o , Type childType )
		{
			DependencyObject foundChild = null;
			if ( o != null )
			{
				int childrenCount = VisualTreeHelper . GetChildrenCount ( o );
				for ( int i = 0 ; i < childrenCount ; i++ )
				{
					var child = VisualTreeHelper . GetChild ( o, i );
					if ( child . GetType ( ) != childType )
					{
						foundChild = FindChild ( child , childType );
						//if(foundChild == null)
						//        FindChild ( child, childType );
					}
					else
					{
						foundChild = child;
						break;
					}
				}
			}
			return foundChild;
		}
		public static T FindChild<T> ( DependencyObject parent , string childName )
			  where T : DependencyObject
		{
			// Confirm parent and childName are valid. 
			if ( parent == null )
				return null;
			T foundChild = null;
			int childrenCount = VisualTreeHelper . GetChildrenCount ( parent );
			for ( int i = 0 ; i < childrenCount ; i++ )
			{
				var child = VisualTreeHelper . GetChild ( parent, i );
				// If the child is not of the request child type child
				T childType = child as T;
				if ( childType == null )
				{
					// recursively drill down the tree
					foundChild = FindChild<T> ( child , childName );
					// If the child is found, break so we do not overwrite the found child. 
					if ( foundChild != null )
						break;
				}
				else if ( !string . IsNullOrEmpty ( childName ) )
				{
					var frameworkElement = child as FrameworkElement;
					// If the child's name is set for search
					if ( frameworkElement != null && frameworkElement . Name == childName )
					{
						// if the child's name is of the request name
						foundChild = ( T ) child;
						break;
					}
				}
				else
				{
					// child element found.
					foundChild = ( T ) child;
					break;
				}
			}
			return foundChild;
		}
		public static int FindMatchingRecord ( string Custno , string Bankno , DataGrid Grid , string currentDb = "" )
		{
			int index = 0;
			if ( currentDb == "BANKACCOUNT" )
			{
				foreach ( var item in Grid . Items )
				{
					BankAccountViewModel cvm = item as BankAccountViewModel;
					if ( cvm == null )
						break;
					if ( cvm . CustNo == Custno && cvm . BankNo == Bankno )
					{
						break;
					}
					index++;
				}
				if ( index == Grid . Items . Count )
					index = -1;
				return index;
			}
			else if ( currentDb == "CUSTOMER" )
			{
				foreach ( var item in Grid . Items )
				{
					CustomerViewModel cvm = item as CustomerViewModel;
					if ( cvm == null )
						break;
					if ( cvm . CustNo == Custno && cvm . BankNo == Bankno )
					{
						break;
					}
					index++;
				}
				if ( index == Grid . Items . Count )
					index = -1;
				return index;
			}
			else if ( currentDb == "DETAILS" )
			{
				foreach ( var item in Grid . Items )
				{
					DetailsViewModel dvm = item as DetailsViewModel;
					if ( dvm == null )
						break;
					if ( dvm . CustNo == Custno && dvm . BankNo == Bankno )
					{
						break;
					}
					index++;
				}
				if ( index == Grid . Items . Count )
					index = -1;
				return index;
			}
			return -1;
		}
		public static T FindVisualChildByName<T> ( DependencyObject parent , string name ) where T : DependencyObject
		{
			for ( int i = 0 ; i < VisualTreeHelper . GetChildrenCount ( parent ) ; i++ )
			{
				var child = VisualTreeHelper . GetChild ( parent, i );
				string controlName = child . GetValue ( Control . NameProperty ) as string;
				if ( controlName == name )
				{
					return child as T;
				}
				else
				{
					T result = FindVisualChildByName<T> ( child, name );
					if ( result != null )
						return result;
				}
			}
			return null;
		}
		public static T FindVisualParent<T> ( UIElement element ) where T : UIElement
		{
			UIElement parent = element;
			while ( parent != null )
			{
				var correctlyTyped = parent as T;
				if ( correctlyTyped != null )
				{
					return correctlyTyped;
				}
				parent = VisualTreeHelper . GetParent ( parent ) as UIElement;
			}
			return null;
		}
		public static parentItem FindVisualParent<parentItem> ( DependencyObject obj ) where parentItem : DependencyObject
		{
			DependencyObject parent = VisualTreeHelper . GetParent ( obj );
			while ( parent != null && !parent . GetType ( ) . Equals ( typeof ( parentItem ) ) )
			{
				parent = VisualTreeHelper . GetParent ( parent );
			}
			return parent as parentItem;
		}
		public static bool FindWindowFromTitle ( string searchterm , ref Window handle )
		{
			bool result = false;
			foreach ( Window window in Application . Current . Windows )
			{
				if ( window . Title . ToUpper ( ) . Contains ( searchterm . ToUpper ( ) ) )
				{
					handle = window;
					result = true;
					break;
				}
			}
			return result;
		}
		public static Brush GetBrush ( string parameter )
		{
			if ( parameter == "BLUE" )
				return Brushes . Blue;
			else if ( parameter == "RED" )
				return Brushes . Red;
			else if ( parameter == "GREEN" )
				return Brushes . Green;
			else if ( parameter == "CYAN" )
				return Brushes . Cyan;
			else if ( parameter == "MAGENTA" )
				return Brushes . Magenta;
			else if ( parameter == "YELLOW" )
				return Brushes . Yellow;
			else if ( parameter == "WHITE" )
				return Brushes . White;
			else
			{
				//We appear to have received a Brushes Resource Name, so return that Brushes value
				Brush b = ( Brush ) Utils . GetDictionaryBrush ( parameter . ToString ( ) );
				return b;
			}
		}
		public static Brush GetBrushFromInt ( int value )
		{
			switch ( value )
			{
				case 0:
					return ( Brushes . White );
				case 1:
					return ( Brushes . Yellow );
				case 2:
					return ( Brushes . Orange );
				case 3:
					return ( Brushes . Red );
				case 4:
					return ( Brushes . Magenta );
				case 5:
					return ( Brushes . Gray );
				case 6:
					return ( Brushes . Aqua );
				case 7:
					return ( Brushes . Azure );
				case 8:
					return ( Brushes . Brown );
				case 9:
					return ( Brushes . Crimson );
				case 10:
					return ( Brushes . Transparent );
			}
			return ( Brush ) null;
		}
		public static string GetDataSortOrder ( string commandline )
		{
			if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . DEFAULT )
				commandline += "Custno, BankNo";
			else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . ID )
				commandline += "ID";
			else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . BANKNO )
				commandline += "BankNo, CustNo";
			else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . CUSTNO )
				commandline += "CustNo";
			else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . ACTYPE )
				commandline += "AcType";
			else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . DOB )
				commandline += "Dob";
			else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . ODATE )
				commandline += "Odate";
			else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . CDATE )
				commandline += "Cdate";
			return commandline;
		}
		public static Brush GetDictionaryBrush ( string brushname )
		{
			Brush brs = null;
			try
			{
				brs = System . Windows . Application . Current . FindResource ( brushname ) as Brush;
			}
			catch
			{

			}
			return brs;
		}
		public static string GetExportFileName ( string filespec = "" )
		// opens  the common file open dialog
		{
			OpenFileDialog ofd = new OpenFileDialog ( );
			ofd . InitialDirectory = @"C:\Users\ianch\Documents\";
			ofd . CheckFileExists = false;
			ofd . AddExtension = true;
			ofd . Title = "Select name for Exported data file.";
			if ( filespec . ToUpper ( ) . Contains ( "XL" ) )
				ofd . Filter = "Excel Spreadsheets (*.xl*) | *.xl*";
			else if ( filespec . ToUpper ( ) . Contains ( "CSV" ) )
				ofd . Filter = "Comma seperated data (*.csv) | *.csv";
			else if ( filespec . ToUpper ( ) . Contains ( "*.*" ) )
				ofd . Filter = "All Files (*.*) | *.*";
			if ( filespec . ToUpper ( ) . Contains ( "PNG" ) )
				ofd . Filter = "Image (*.png*) | *.pb*";
			else if ( filespec == "" )
			{
				ofd . Filter = "All Files (*.*) | *.*";
				ofd . DefaultExt = ".CSV";
			}
			ofd . FileName = filespec;
			ofd . ShowDialog ( );
			string fnameonly = ofd . SafeFileName;
			return ofd . FileName;
		}
		public static string GetImportFileName ( string filespec )
		// opens  the common file open dialog
		{
			OpenFileDialog ofd = new OpenFileDialog ( );
			ofd . InitialDirectory = @"C:\Users\ianch\Documents\";
			ofd . CheckFileExists = true;
			if ( filespec . ToUpper ( ) . Contains ( "XL" ) )
				ofd . Filter = "Excel Spreadsheets (*.xl*) | *.xl*";
			else if ( filespec . ToUpper ( ) . Contains ( "CSV" ) )
				ofd . Filter = "Comma seperated data (*.csv) | *.csv";
			else if ( filespec . ToUpper ( ) . Contains ( "*.*" ) || filespec == "" )
				ofd . Filter = "All Files (*.*) | *.*";
			ofd . AddExtension = true;
			ofd . ShowDialog ( );
			return ofd . FileName;
		}
		public static Brush GetNewBrush ( string color )
		{
			if ( color == "" )
				return null;
			if ( color [ 0 ] != '#' )
				color = "#" + color;
			return ( Brush ) new BrushConverter ( ) . ConvertFrom ( color );
		}
		public static string GetPrettyGridStatistics ( DataGrid Grid , int current )
		{
			string output = "";
			if ( current != -1 )
				output = $"{current} / {Grid . Items . Count}";
			else
				output = $"0 / {Grid . Items . Count}";
			return output;
		}
		public static ControlTemplate GetDictionaryControlTemplate ( string tempname )
		{
			ControlTemplate ctmp = System . Windows . Application . Current . FindResource ( tempname ) as ControlTemplate;
			return ctmp;
		}
		public static Style GetDictionaryStyle ( string tempname )
		{
			Style ctmp = System . Windows . Application . Current . FindResource ( tempname ) as Style;
			return ctmp;
		}
		public static object GetTemplateControl ( Control RectBtn , string CtrlName )
		{
			var template = RectBtn . Template;
			object v = template . FindName ( CtrlName, RectBtn ) as object;
			return v;
		}
		public static void GetWindowHandles ( )
		{
#if SHOWWINDOWDATA
			Console . WriteLine ( $"Current Windows\r\n" + "===============" );
			foreach ( Window window in System . Windows . Application . Current . Windows )
			{
				if ( ( string ) window . Title != "" && ( string ) window . Content != "" )
				{
					Console . WriteLine ( $"Title:  {window . Title },\r\nContent - {window . Content}" );
					Console . WriteLine ( $"Name = [{window . Name}]\r\n" );
				}
			}
#endif
		}
		public static void Grab_MouseMove ( object sender , MouseEventArgs e )
		{
			Point pt = e.GetPosition((UIElement)sender);
			HitTestResult hit = VisualTreeHelper . HitTest ( ( Visual ) sender, pt );
			if ( hit?.VisualHit != null )
			{
				if ( ControlsHitList . Count != 0 )
				{
					if ( hit . VisualHit == ControlsHitList [ 0 ] . VisualHit )
						return;
				}
				ControlsHitList . Clear ( );
				ControlsHitList . Add ( hit );
			}
		}
		public static void Grab_Object ( object sender , Point pt )
		{
			//Point pt = e.GetPosition((UIElement)sender);
			HitTestResult hit = VisualTreeHelper . HitTest ( ( Visual ) sender, pt );
			if ( hit?.VisualHit != null )
			{
				if ( ControlsHitList . Count != 0 )
				{
					if ( hit . VisualHit == ControlsHitList [ 0 ] . VisualHit )
						return;
				}
				ControlsHitList . Clear ( );
				ControlsHitList . Add ( hit );
			}
		}
		public static void Grabscreen ( Window parent , object obj , GrabImageArgs args , Control ctrl = null )
		{
			UIElement ui = obj as UIElement;
			UIElement OBJ=new UIElement();
			int indx = 0;
			bool success = false;
			// try to step up the visual tree ?
			do
			{
				indx++;
				if ( indx > 30 || indx < 0 )
					break;
				switch ( indx )
				{
					case 1:
						OBJ = FindVisualParent<DataGrid> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 2:
						OBJ = FindVisualParent<Button> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 3:
						OBJ = FindVisualParent<Slider> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 4:
						OBJ = FindVisualParent<ListView> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 5:
						OBJ = FindVisualParent<ListBox> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 6:
						OBJ = FindVisualParent<ComboBox> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 7:
						OBJ = FindVisualParent<WrapPanel> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 8:
						OBJ = FindVisualParent<CheckBox> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 9:
						OBJ = FindVisualParent<DataGridRow> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 10:
						OBJ = FindVisualParent<DataGridCell> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 11:
						OBJ = FindVisualParent<Canvas> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 12:
						OBJ = FindVisualParent<GroupBox> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 13:
						OBJ = FindVisualParent<ProgressBar> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 14:
						OBJ = FindVisualParent<Ellipse> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 15:
						OBJ = FindVisualParent<RichTextBox> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 16:
						OBJ = FindVisualParent<TextBlock> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 17:
						//if(ui Equals TextBox)
						if ( ui == null )
							break;
						DependencyObject v = new DependencyObject();
						DependencyObject prev = new DependencyObject();
						//OBJ = FindVisualParent<TextBox> ( ui );
						do
						{
							v = VisualTreeHelper . GetParent ( ui );
							if ( v == null )
								break;
							prev = v;
							TextBox tb = v as TextBox;
							if ( tb != null && tb . Text . Length > 0 )
							{
								Console . WriteLine ( $"UI = {tb . Text}" );
								OBJ = tb as UIElement;
								success = true;
								break;
							}
							Console . WriteLine ( $"UI = {v . ToString ( )}" );
							//							if ( v . ToString ( ) . Contains ( ".TextBox" ) )
							//if ( v . ToString ( ) . Contains ( ".TextBox" ) )
							//{
							//	OBJ = prev as UIElement;
							//	success = true;
							//	break;
							//}
							//else
							ui = v as UIElement;
						} while ( true );
						break;
					case 18:
						OBJ = FindVisualParent<ContentPresenter> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 19:
						OBJ = FindVisualParent<Grid> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 20:
						OBJ = FindVisualParent<Window> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 21:
						OBJ = FindVisualParent<ScrollContentPresenter> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					case 22:
						OBJ = FindVisualParent<Rectangle> ( ui );
						if ( OBJ != null )
							success = true;
						break;
					//case 23:
					//	OBJ = FindVisualParent<TextBoxLineDrawingVisual> ( ui );
					//	if ( OBJ != null )
					//		success = true;
					//	break;
					default:
						OBJ = ui;
						success = false;
						break;
				}
				if ( success == true && OBJ != null )
					break;
			} while ( true );
			if ( success == false )
				return;
			Console . WriteLine ( $"Element Identified for display = : [{OBJ . ToString ( )}]" );
			//string str = OBJ . ToString ( );
			//if ( str . Contains ( ".TextBox" ) )
			//{
			//	var v  = OBJ.GetType();
			//	Console . WriteLine ( $"Type is {v}" );
			//}
			////OBJ = OBJ . Text;
			var bmp = Utils . CreateControlImage ( OBJ as FrameworkElement );
			if ( bmp == null )
				return;
			Utils . SaveImageToFile ( ( RenderTargetBitmap ) bmp , "C:\\WPFPages-11nov21\\Icons\\Grabimage.png" , "PNG" );
			Grabviewer gv = new Grabviewer(parent,ctrl, bmp);
			//Setup the  image in our viewer
			gv . Grabimage . Source = bmp;
			gv . Title = "C:\\WPFPages-11nov21\\Icons\\Grabimage.png";
			gv . Title += $"  :  RESIZE window to magnify the contents ...";
			gv . Show ( );
			// Save to disk file
		}
		public static void HandleCtrlFnKeys ( bool key1 , KeyEventArgs e )
		{
			if ( key1 && e . Key == Key . F5 )
			{
				// list Flags in Console
				Utils . GetWindowHandles ( );
				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( key1 && e . Key == Key . F6 )  // CTRL + F6
			{
				// list various Flags in Console
				Debug . WriteLine ( $"\nCTRL + F6 pressed..." );
				Flags . UseBeeps = !Flags . UseBeeps;
				e . Handled = true;
				key1 = false;
				Debug . WriteLine ( $"Flags.UseBeeps reset to  {Flags . UseBeeps }" );
				return;
			}
			else if ( key1 && e . Key == Key . F7 )  // CTRL + F7
			{
				// list various Flags in Console
				Debug . WriteLine ( $"\nCTRL + F7 pressed..." );
				Flags . PrintDbInfo ( );
				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( key1 && e . Key == Key . F8 )     // CTRL + F8
			{
				Debug . WriteLine ( $"\nCTRL + F8 pressed..." );
				EventHandlers . ShowSubscribersCount ( );
				e . Handled = true;
				key1 = false;
				return;
			}
			else if ( key1 && e . Key == Key . F9 )     // CTRL + F9
			{
				Debug . WriteLine ( "\nCtrl + F9 NOT Implemented" );
				key1 = false;
				return;

			}
			else if ( key1 && e . Key == Key . System )     // CTRL + F10
			{
				// Major  listof GV[] variables (Guids etc]
				Debug . WriteLine ( $"\nCTRL + F10 pressed..." );
				Flags . ListGridviewControlFlags ( 1 );
				key1 = false;
				e . Handled = true;
				return;
			}
			else if ( key1 && e . Key == Key . F11 )  // CTRL + F11
			{
				// list various Flags in Console
				Debug . WriteLine ( $"\nCTRL + F11 pressed..." );
				Flags . PrintSundryVariables ( );
				e . Handled = true;
				key1 = false;
				return;
			}
		}
		public static bool HitTestScrollBar ( object sender , MouseButtonEventArgs e )
		{
			//			HitTestResult hit = VisualTreeHelper . HitTest ( ( Visual ) sender, e . GetPosition ( ( IInputElement ) sender ) );
			//			return hit . VisualHit . GetVisualAncestor<ScrollBar> ( ) != null;
			object original = e . OriginalSource;
			try
			{
				if ( !original . GetType ( ) . Equals ( typeof ( ScrollBar ) ) )
				{
					if ( original . GetType ( ) . Equals ( typeof ( DataGrid ) ) )
					{
						Console . WriteLine ( "DataGrid is clicked" );
					}
					else if ( FindVisualParent<ScrollBar> ( original as DependencyObject ) != null )
					{
						//scroll bar is clicked
						return true;
					}
					return false;
					;
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"Error in HitTest ScriollBar Function (Utils-1010({ex . Data}" );
			}
			return true;
		}
		public static bool HitTestHeaderBar ( object sender , MouseButtonEventArgs e )
		{
			//			HitTestResult hit = VisualTreeHelper . HitTest ( ( Visual ) sender, e . GetPosition ( ( IInputElement ) sender ) );
			//			return hit . VisualHit . GetVisualAncestor<ScrollBar> ( ) != null;
			object original = e . OriginalSource;

			if ( !original . GetType ( ) . Equals ( typeof ( DataGridColumnHeader ) ) )
			{
				if ( original . GetType ( ) . Equals ( typeof ( DataGrid ) ) )
				{
					Console . WriteLine ( "DataGrid is clicked" );
				}
				else if ( FindVisualParent<DataGridColumnHeader> ( original as DependencyObject ) != null )
				{
					//Header bar is clicked
					return true;
				}
				return false;
				;
			}
			return true;
		}
		public static void LoadBankDbGeneric ( BankCollection bvm , string caller = "" , bool Notify = false , int lowvalue = -1 , int highvalue = -1 , int maxrecords = -1 )
		{
			if ( maxrecords == -1 )
				BankCollection . LoadBank ( cc: bvm , caller: "BankAccount" , ViewerType: 99 , NotifyAll: Notify );
			else
			{
				DataTable dtBank  = new DataTable();
				dtBank = BankCollection . LoadSelectedBankData ( Min: lowvalue , Max: highvalue , Tot: maxrecords );
				bvm = BankCollection . LoadSelectedCollection ( bankCollection: bvm , max: -1 , dtBank: dtBank , Notify: Notify );
			}

		}
		private static string ParseforCR ( string input )
		{
			string output="";
			if ( input . Length == 0 )
				return input;
			if ( input . Contains ( "\r\n" ) )
			{
				do
				{
					string[] fields = input.Split('\r');
					foreach ( var item in fields )
					{
						output += item;
					}
					if ( output . Contains ( "\r" ) == false )
						break;
				} while ( true );
			}
			else
				return input;
			return output;
		}
		public static void ReadAllConfigSettings ( )
		{
			try
			{
				var appSettings = ConfigurationManager . AppSettings;

				if ( appSettings . Count == 0 )
				{
					Console . WriteLine ( "AppSettings is empty." );
				}
				else
				{
					foreach ( var key in appSettings . AllKeys )
					{
						Console . WriteLine ( "Key: {0} Value: {1}" , key , appSettings [ key ] );
					}
				}
			}
			catch ( ConfigurationErrorsException )
			{
				Console . WriteLine ( "Error reading app settings" );
			}
		}
		public static string ReadConfigSetting ( string key )
		{
			string result = "";
			try
			{
				var appSettings = ConfigurationManager . AppSettings;
				result = appSettings [ key ] ?? "Not Found";
				Console . WriteLine ( result );
			}
			catch ( ConfigurationErrorsException )
			{
				Console . WriteLine ( "Error reading app settings" );
			}
			return result;
		}
		/// <summary>
		/// Creates a BMP from any control passed into it   ???
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static RenderTargetBitmap RenderBitmap ( Visual element , double objwidth = 0 , double objheight = 0 , string filename = "" , bool savetodisk = false )
		{
			double topLeft = 0;
			double topRight = 0;
			int width = 0;
			int height =0;

			if ( element == null )
				return null;
			Rect bounds = VisualTreeHelper.GetDescendantBounds(element);
			if ( objwidth == 0 )
				width = ( int ) bounds . Width;
			if ( objheight == 0 )
				height = ( int ) bounds . Height;
			double dpiX = 96; // this is the magic number
			double dpiY = 96; // this is the magic number

			PixelFormat pixelFormat = PixelFormats . Default;
			VisualBrush elementBrush = new VisualBrush ( element );
			DrawingVisual visual = new DrawingVisual ( );
			DrawingContext dc = visual . RenderOpen ( );

			dc . DrawRectangle ( elementBrush , null , new Rect ( topLeft , topRight , width , height ) );
			dc . Close ( );
			RenderTargetBitmap rtb = new RenderTargetBitmap ( (int)(bounds.Width * dpiX / 96.0), (int)(bounds.Height * dpiY / 96.0), dpiX, dpiY, PixelFormats.Pbgra32 );
			DrawingVisual dv = new DrawingVisual();
			using ( DrawingContext ctx = dv . RenderOpen ( ) )
			{
				VisualBrush vb = new VisualBrush(element);
				ctx . DrawRectangle ( vb , null , new Rect ( new Point ( ) , bounds . Size ) );
			}
			rtb . Render ( dv );

			if ( savetodisk && filename != "" )
				SaveImageToFile ( rtb , filename );
			return rtb;
		}
		// allows any image to be saved as PNG/GIF/JPG format, defaullt is PNG
		public static void SaveImageToFile ( RenderTargetBitmap bmp , string file , string imagetype = "PNG" )
		{
			string[] items;
			// Make a PNG encoder.
			if ( bmp == null )
				return;
			if ( file == "" && imagetype != "" )
				file = @"J:\users\ianch\pictures\defaultimage";
			items = file . Split ( '.' );
			file = items [ 0 ];
			if ( imagetype == "PNG" )
				file += ".png";
			else if ( imagetype == "GIF" )
				file += ".gif";
			else if ( imagetype == "JPG" )
				file += ".jpg";
			try
			{
				using ( FileStream fs = new FileStream ( file ,
						    FileMode . Create , FileAccess . Write , FileShare . ReadWrite ) )
				{
					if ( imagetype == "PNG" )
					{
						PngBitmapEncoder encoder = new PngBitmapEncoder();
						encoder . Frames . Add ( BitmapFrame . Create ( bmp ) );
						encoder . Save ( fs );
					}
					else if ( imagetype == "GIF" )
					{
						GifBitmapEncoder encoder = new GifBitmapEncoder ();
						encoder . Frames . Add ( BitmapFrame . Create ( bmp ) );
						encoder . Save ( fs );
					}
					else if ( imagetype == "JPG" || imagetype == "JPEG" )
					{
						JpegBitmapEncoder encoder = new JpegBitmapEncoder ();
						encoder . Frames . Add ( BitmapFrame . Create ( bmp ) );
						encoder . Save ( fs );
					}
					fs . Close ( );
				}
			}
			catch ( Exception ex )
			{
				Utils . Mbox ( null , string1: "The image could not be saved for the following reason " , string2: $"{ex . Message}" , caption: "" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
			}
		}
		public static void SaveProperty ( string setting , string value )
		{
			try
			{
				if ( value . ToUpper ( ) . Contains ( "TRUE" ) )
					Settings . Default [ setting ] = true;
				else if ( value . ToUpper ( ) . Contains ( "FALSE" ) )
					Settings . Default [ setting ] = false;
				else
					Settings . Default [ setting ] = value;
				Settings . Default . Save ( );
				Settings . Default . Upgrade ( );
				ConfigurationManager . RefreshSection ( setting );
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"Unable to save property {setting} of [{value}]\nError was {ex . Data}, {ex . Message}, Stack trace = \n{ex . StackTrace}" );
			}
		}
		/// <summary>
		/// MASTER UPDATE METHOD
		/// This handles repositioning of a selected item in any grid perfectly
		/// </summary>
		/// <param name="grid"></param>
		/// <param name="row"></param>
		/// <summary>
		/// Metohd that almost GUARANTESS ot force a record into view in any DataGrid
		/// /// This is called by method above - MASTER Updater Method
		/// </summary>
		/// <param name="dGrid"></param>
		/// <param name="row"></param>
		public static void ScrollRecordInGrid ( DataGrid dGrid , int row )
		{
			if ( dGrid . CurrentItem == null )
				return;
			dGrid . UpdateLayout ( );
			dGrid . ScrollIntoView ( dGrid . Items . Count - 1 );
			dGrid . UpdateLayout ( );
			dGrid . ScrollIntoView ( row );
			dGrid . UpdateLayout ( );
			Utils . ScrollRecordIntoView ( dGrid , row );
		}

		public static void ScrollLBRecordIntoView ( ListBox Dgrid , int CurrentRecord )
		{
			// Works well 26/5/21

			//update and scroll to bottom first
			Dgrid . SelectedIndex = ( int ) CurrentRecord;
			Dgrid . SelectedItem = ( int ) CurrentRecord;
			Dgrid . UpdateLayout ( );
			Dgrid . ScrollIntoView ( Dgrid . Items . Count - 1 );
			Dgrid . UpdateLayout ( );
			Dgrid . ScrollIntoView ( Dgrid . SelectedItem );
			Dgrid . UpdateLayout ( );
		}
		public static void ScrollRecordIntoView ( DataGrid Dgrid , int CurrentRecord )
		{
			// Works well 26/5/21
			double currentTop = 0;
			double currentBottom = 0;
			if ( CurrentRecord == -1 )
				return;
			if ( Dgrid . Name == "CustomerGrid" || Dgrid . Name == "DataGrid1" )
			{
				currentTop = Flags . TopVisibleBankGridRow;
				currentBottom = Flags . BottomVisibleBankGridRow;
			}
			else if ( Dgrid . Name == "BankGrid" || Dgrid . Name == "DataGrid2" )
			{
				currentTop = Flags . TopVisibleCustGridRow;
				currentBottom = Flags . BottomVisibleCustGridRow;
			}
			else if ( Dgrid . Name == "DetailsGrid" || Dgrid . Name == "DetailsGrid" )
			{
				currentTop = Flags . TopVisibleDetGridRow;
				currentBottom = Flags . BottomVisibleDetGridRow;
			}     // Believe it or not, it takes all this to force a scrollinto view correctly

			if ( Dgrid == null || Dgrid . Items . Count == 0 || Dgrid . SelectedItem == null )
				return;

			//update and scroll to bottom first
			Dgrid . SelectedIndex = ( int ) CurrentRecord;
			Dgrid . SelectedItem = ( int ) CurrentRecord;
			Dgrid . UpdateLayout ( );
			Dgrid . ScrollIntoView ( Dgrid . Items . Count - 1 );
			Dgrid . UpdateLayout ( );
			Dgrid . ScrollIntoView ( Dgrid . SelectedItem );
			Dgrid . UpdateLayout ( );
			Flags . CurrentSqlViewer?.SetScrollVariables ( Dgrid );
		}
		//Generic form of Selection forcing code below
		public static void SelectTextBoxText ( TextBox txtbox )
		{
			txtbox . SelectionLength = txtbox . Text . Length;
			txtbox . SelectionStart = 0;
			txtbox . SelectAll ( );
		}
		public static void SetGridRowSelectionOn ( DataGrid dgrid , int index )
		{
			if ( dgrid . Items . Count > 0 && index != -1 )
			{
				try
				{
					// clear anny selection on current record in datagrid
					DataGridRow r = dgrid . ItemContainerGenerator . ContainerFromIndex ( dgrid.SelectedIndex ) as DataGridRow;
					if ( r != null )
						r . IsSelected = false;
					//Setup new selected index
					dgrid . SelectedIndex = index;
					dgrid . SelectedItem = index;
					dgrid . UpdateLayout ( );
					dgrid . BringIntoView ( );
					dgrid . ScrollIntoView ( dgrid . Items [ index ] );
					r = dgrid . ItemContainerGenerator . ContainerFromIndex ( index ) as DataGridRow;
					if ( r != null )
					{
						// Select new record
						r . IsSelected = false;
						r . IsSelected = true;
					}
				}
				catch ( Exception ex )
				{
					Debug . WriteLine ( $"{ex . Message}, {ex . Data}" );
				}
			}
		}
		public static void SetSelectedItemFirstRow ( object dataGrid , object selectedItem )
		{
			//If target datagrid Empty, throw exception
			if ( dataGrid == null )
			{
				throw new ArgumentNullException ( "Target none" + dataGrid + "Cannot convert to DataGrid" );
			}
			//Get target DataGrid，If it is empty, an exception will be thrown
			System . Windows . Controls . DataGrid dg = dataGrid as System . Windows . Controls . DataGrid;
			if ( dg == null )
			{
				throw new ArgumentNullException ( "Target none" + dataGrid + "Cannot convert to DataGrid" );
			}
			//If the data source is empty, return
			if ( dg . Items == null || dg . Items . Count < 1 )
			{
				return;
			}

			dg . SelectedItem = selectedItem;
			dg . CurrentColumn = dg . Columns [ 0 ];
			dg . ScrollIntoView ( dg . SelectedItem , dg . CurrentColumn );
		}
		public static void SetUpGListboxSelection ( ListBox grid , int row = 0 )
		{
			//			bool inprogress = false;
			//			int scrollrow = 0;
			if ( row == -1 )
				row = 0;
			// This triggers the selection changed event
			grid . SelectedIndex = row;
			grid . SelectedItem = row;
			//			grid . SetDetailsVisibilityForItem ( grid . SelectedItem, Visibility . Visible );
			grid . SelectedIndex = row;
			grid . SelectedItem = row;
			Utils . ScrollLBRecordIntoView ( grid , row );
			grid . UpdateLayout ( );
			grid . Refresh ( );
			//			var v = grid .VerticalAlignment;
		}
		public static void SetUpGridSelection ( DataGrid grid , int row = 0 )
		{
			//			bool inprogress = false;
			//			int scrollrow = 0;
			if ( row == -1 )
				row = 0;
			// This triggers the selection changed event
			grid . SelectedIndex = row;
			grid . SelectedItem = row;
			//			grid . SetDetailsVisibilityForItem ( grid . SelectedItem, Visibility . Visible );
			grid . SelectedIndex = row;
			grid . SelectedItem = row;
			Utils . ScrollRecordIntoView ( grid , row );
			grid . UpdateLayout ( );
			grid . Refresh ( );
			//			var v = grid .VerticalAlignment;
		}

		public static void IsMouseMove ( object sender , MouseEventArgs e )
		{
			e . Handled = true;
			if ( e . RightButton == MouseButtonState . Pressed )
				return;
		}

		public static void SetupWindowDrag ( Window inst )
		{
			try
			{
				//Handle the button NOT being the left mouse button
				// which will crash the DragMove Fn.....
				MouseButtonState mbs =   Mouse. RightButton ;
				Console . WriteLine ( $"{mbs . ToString ( )}" );
				if ( mbs == MouseButtonState . Pressed )
					return;
				inst . MouseDown += delegate
				{
					{
						try
						{
							inst?.DragMove ( );
						}
						catch ( Exception ex )
						{
							return;
						}
					}
				};
			}
			catch ( Exception ex )
			{
				return;
			}
		}


		//********************************************************************************************************************************************************************************//
		public static void NewCookie_Click ( object sender , RoutedEventArgs e )
		//********************************************************************************************************************************************************************************//
		{
			NewCookie nc = new NewCookie(sender as Window);
			nc . ShowDialog ( );
			defvars . CookieAdded = false;
		}

	}
}
