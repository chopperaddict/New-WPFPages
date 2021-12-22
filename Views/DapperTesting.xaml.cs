using Microsoft . SqlServer . Management . Smo;

using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Configuration;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Diagnostics . Eventing . Reader;
using System . Linq;
using System . Runtime . Remoting . Metadata . W3cXsd2001;
using System . Security . AccessControl;
using System . Text;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Controls . Primitives;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Interop;
using System . Windows . Media;
using System . Windows . Media . Converters;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;
using System . Windows . Threading;

using WPFPages . UserControls;
using WPFPages . ViewModels;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for DapperTesting.xaml
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
	public partial class DapperTesting : Window

	#region DECLARATIONS
	{
		public static ObservableCollection<BankCombinedViewModel> bcvm = new ObservableCollection<BankCombinedViewModel>();
		public static ObservableCollection<BankAccountViewModel> bvm = new ObservableCollection<BankAccountViewModel>();
		public static ObservableCollection<CustomerViewModel> cvm = new ObservableCollection<CustomerViewModel>();
		public static ObservableCollection<GenericClass> selectedgenerics = new ObservableCollection<GenericClass>();
		public static ObservableCollection<DetailsViewModel> dvm = new ObservableCollection<DetailsViewModel>();
		private bool IsGenericListResult = false;
		private List<string> genericlist = new List<string>   ();
		private string ActiveLoadMethod = "USESDAPPERSTDPROCEDURES";
		public  DispatcherTimer timer = new DispatcherTimer();
		private bool UseAsyncLoading = true;
		private int startsecs = 0;
		private int endsecs = 0;
		int [ ] args= {0,0,0};

		// Temp declaration
		bool CreateCombinedDb = true;
		static private int ToggleBtnStatus { get; set; }
		bool[] status = { false , false , false } ;
		public struct ToggleFlags
		{
			public int current;
		}
		ToggleFlags tFlags = new ToggleFlags();

		Msgboxs mbox = new Msgboxs();
		public double Checked
		{
			get
			{ return ( double ) GetValue ( CheckedProperty ); }
			set { SetValue ( CheckedProperty , value ); }
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		public static readonly DependencyProperty CheckedProperty =
				DependencyProperty . Register ( "Checked",
				typeof ( int ),
				typeof ( DapperTesting),
				new PropertyMetadata ( ( int) 0 ), OnCheckedPropertyChanged );

		#endregion DECLARATIONS

		#region BASIC SETUP
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private static bool OnCheckedPropertyChanged ( object value )
		{
			int x = Convert.ToInt32(value);
			ToggleBtnStatus = ( x );

			return true;
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		public DapperTesting ( )
		{
			InitializeComponent ( );
		}
		public override void OnApplyTemplate ( )
		{
			base . OnApplyTemplate ( );

			if ( Template != null )
			{
				var v = this . GetTemplateChild ( "MyEllipse" );
			}

			//UpdateStates ( false ); // Not sure what this does ?
			return;
		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void Timer_Tick ( object sender , EventArgs e )
		{
			endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
		}

		#endregion BASIC SETUP

		#region STARTUP/CLOSEDOWN  METHODS
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void Window_Loaded ( object sender , RoutedEventArgs e )
		{
			this . Show ( );
			Mouse . OverrideCursor = Cursors . Wait;
			// Handle window dragging
			Utils . SetupWindowDrag ( this );

			//HwndSource source = (HwndSource)HwndSource.FromVisual(GenericGrid1);
			args [ 0 ] = 0;
			args [ 1 ] = 1700000;
			args [ 2 ] = 0;
			UseAsync . IsChecked = true;
			// Setup toggle button status  to indeterminate (0)
			tFlags . current = 0;
			Flags . USEADOWITHSTOREDPROCEDURES = true;
			EventControl . BankDataLoaded += EventControl_BankDataLoaded;
			EventControl . CustDataLoaded += EventControl_CustDataLoaded;
			EventControl . DetDataLoaded += EventControl_DetDataLoaded;
			//			timer . Interval = TimeSpan . FromSeconds ( 1 );
			timer . Interval = TimeSpan . FromMilliseconds ( 1 );
			timer . Tick += Timer_Tick;
			ActiveLoadMethod = "USESDAPPERSTOREDPROCEDURE";
			BankDb . Text = "BANKACCOUNT";
			CustDb . Text = "CUSTOMER";
			DetDb . Text = "SECACCOUNTS";
			UseStdDapper . IsChecked = true;
			UseDapperStoredProc . IsChecked = false;
			UseStoredProc . IsChecked = false;
			CurrBank . Content = BankDb . Text . ToUpper ( );
			CurrCust . Content = CustDb . Text . ToUpper ( );
			CurrDet . Content = DetDb . Text . ToUpper ( );
			startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
			timer . Start ( );
			LoadDbs ( );
			LoadGrids ( );
			timer . Stop ( );
			endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
			//if ( endsecs - startsecs < 0 )
			//	Debugger . Break ( );
			LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
			GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid1 . Background = FindResource ( "Red5" ) as SolidColorBrush;

			tFlags . current = 2;
			ToggleBtn_Click ( null , null );
			SetDummyGridEntries ( );

			GenericGrid1 . Focus ( );
			Mouse . OverrideCursor = Cursors . Arrow;
			string errormsg="";

			//testing
			//			ObservableCollection<GenericClass> gc = new ObservableCollection<GenericClass>();
			////			ObservableCollection<BankAccountViewModel> newcollection = new ObservableCollection<BankAccountViewModel>();
			//			ObservableCollection<GenericClass> gencollection = new ObservableCollection<GenericClass>();
			//			List<GenericClass> list = new List<GenericClass>();
			//			// converts the host db (bvm) to an ObservableCollection <GenericClass> gc
			//			DapperGeneric<GenericClass , ObservableCollection<GenericClass>> . ExecuteSPFullGenericClass (
			//			     ref gencollection,
			//			     true,
			//			     ref gencollection ,
			//			     "select * from bankaccount" ,
			//			     "" ,
			//			     "" ,
			//			     "" ,
			//			     ref list ,
			//			     out errormsg );
			//			foreach ( var item in list )
			//			{
			//				gencollection . Add ( item as GenericClass );
			//			}

			// We get the data back in the List<CustomerViewModel>
			//ObservableCollection<GenericClass> gc = new ObservableCollection<GenericClass>();
			//ObservableCollection<BankAccountViewModel > newcollection = new ObservableCollection<BankAccountViewModel >();
			//List<GenericClass> list = new List<GenericClass>();
			//List<BankAccountViewModel > typelist = new List<BankAccountViewModel >();
			//// converts the host db (bvm) to an ObservableCollection <GenericClass> gc
			//DapperGeneric<BankAccountViewModel , ObservableCollection<BankAccountViewModel> , List<GenericClass>> . ExecuteSPFullGenericClass (
			//     ref newcollection ,
			//     false ,
			//	 ref gc,
			//     "select top(50)* from customer" ,
			//     "" ,
			//     "" ,
			//     "" ,
			//     ref typelist ,
			//	ref list,
			//     out errormsg );
			////Create our new collection from list
			//foreach ( var item in list )
			//{
			//	newcollection . Add ( item as GenericClass );
			//}

			//UniversalGrid . ItemsSource = null;
			//UniversalGrid . ItemsSource = newcollection ;	 			
			//UniversalGrid . Visibility = Visibility . Visible;
			//tFlags . current = 0;
			//ToggleBtn_Click(null,null);
		}
		#endregion STARTUP/CLOSEDOWN  METHODS

		#region Async Data loaded handlers
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void EventControl_BankDataLoaded ( object sender , LoadedEventArgs e )
		{
			bvm = e . DataSource as ObservableCollection<BankAccountViewModel>;
			GenericGrid1 . ItemsSource = bvm;
			GenericGrid1 . UpdateLayout ( );
			GenericGrid1 . Background = FindResource ( "Red5" ) as SolidColorBrush;
			timer . Stop ( );
			endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
			//if ( endsecs - startsecs < 0 )
			//	Debugger . Break ( );
			LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
			BankCount . Text = bvm . Count . ToString ( );
			CustCount . Text = cvm . Count . ToString ( );
			DetCount . Text = dvm . Count . ToString ( );
			CurrBank . Content = BankDb . Text . ToUpper ( );
		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void EventControl_CustDataLoaded ( object sender , LoadedEventArgs e )
		{
			cvm = e . DataSource as ObservableCollection<CustomerViewModel>;
			GenericGrid2 . ItemsSource = cvm;
			GenericGrid2 . UpdateLayout ( );
			GenericGrid2 . Background = FindResource ( "Red5" ) as SolidColorBrush;
			timer . Stop ( );
			endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
			//if ( endsecs - startsecs < 0 )
			//	Debugger . Break ( );
			LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
			BankCount . Text = bvm . Count . ToString ( );
			CustCount . Text = cvm . Count . ToString ( );
			DetCount . Text = dvm . Count . ToString ( );
			CurrCust . Content = CustDb . Text . ToUpper ( );
		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void EventControl_DetDataLoaded ( object sender , LoadedEventArgs e )
		{
			dvm = e . DataSource as ObservableCollection<DetailsViewModel>;
			GenericGrid3 . ItemsSource = dvm;
			GenericGrid3 . UpdateLayout ( );
			GenericGrid3 . Background = FindResource ( "Red5" ) as SolidColorBrush;
			timer . Stop ( );
			endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
			//if ( endsecs - startsecs < 0 )							  
			//	Debugger . Break ( );
			LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
			BankCount . Text = bvm . Count . ToString ( );
			CustCount . Text = cvm . Count . ToString ( );
			DetCount . Text = dvm . Count . ToString ( );
			CurrDet . Content = DetDb . Text . ToUpper ( );
		}
		#endregion Async Data loaded handlers

		#region BUTTON HANDLERS
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void Standardbutton_Click ( object sender , RoutedEventArgs e )
		{
			Mouse . OverrideCursor = Cursors . Wait;
			Flags . USECOPYDATA = false;
			LoadDbs ( );
			LoadGrids ( );
			Mouse . OverrideCursor = Cursors . Arrow;
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void Backupbutton_Click ( object sender , RoutedEventArgs e )
		{
			Mouse . OverrideCursor = Cursors . Wait;
			Flags . USECOPYDATA = true;
			//			Stopwatch sw = new Stopwatch();
			startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;

			timer . Start ( );
			LoadDbs ( );
			LoadGrids ( );
			timer . Stop ( );
			endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
			//if ( endsecs - startsecs < 0 )
			//	Debugger . Break ( );
			LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
			Mouse . OverrideCursor = Cursors . Arrow;
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void button4_Click ( object sender , RoutedEventArgs e )
		{
			this . Close ( );
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void button5_Click ( object sender , RoutedEventArgs e )
		{
			// Reload ALL Dbs button
			Mouse . OverrideCursor = Cursors . Wait;
			//			Stopwatch sw = new Stopwatch();

			timer . Start ( );
			startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
			GenericGrid1 . ItemsSource = null;
			GenericGrid2 . ItemsSource = null;
			GenericGrid3 . ItemsSource = null;
			GenericGrid1 . Items . Clear ( );
			GenericGrid2 . Items . Clear ( );
			GenericGrid3 . Items . Clear ( );
			GenericGrid1 . UpdateLayout ( );
			GenericGrid2 . UpdateLayout ( );
			GenericGrid3 . UpdateLayout ( );
			GenericGrid1 . Refresh ( );
			GenericGrid2 . Refresh ( );
			GenericGrid3 . Refresh ( );
			LoadDbs ( );
			LoadGrids ( );
			if ( UseAsyncLoading == false )
			{
				timer . Stop ( );
				endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				//if ( endsecs - startsecs < 0 )
				//	Debugger . Break ( );
				LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
				BankCount . Text = bvm . Count . ToString ( );
				CustCount . Text = cvm . Count . ToString ( );
				DetCount . Text = dvm . Count . ToString ( );
			}
			Mouse . OverrideCursor = Cursors . Arrow;
			GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid1 . Focus ( );
			CurrBank . Content = BankDb . Text . ToUpper ( );
			CurrCust . Content = CustDb . Text . ToUpper ( );
			CurrDet . Content = DetDb . Text . ToUpper ( );
		}

		#endregion BUTTON HANDLERS

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private async void LoadDbs ( )
		{
			args [ 0 ] = 0;
			args [ 1 ] = 0;
			args [ 2 ] = 0;
			if ( MaxRecords . Text != "*" && MaxRecordsToLoad . IsChecked == true )
			{
				if ( MaxRecords . Text == "*" )
				{
					MaxRecords . Text = "";
					args [ 2 ] = 0;
				}
				else
				{
					if ( MaxRecords . Text == "" )
						args [ 2 ] = 0;
					else
						args [ 2 ] = int . Parse ( MaxRecords . Text );
				}
			}
			if ( UseAsyncLoading )
			{
				bool resut  = await DapperSupport . GetBankObsCollectionAsync ( bvm ,
					"" ,
					BankDb . Text ,
					UseSort . IsChecked == true ? OrderString . Text : "" ,
					UseConditions . IsChecked == true ? Conditions . Text : "" ,
					true ,
					true ,
					  false ,
					"DAPPERTESTING" ,
					args );
			}
			else
			{
				bool resut  = await DapperSupport . GetBankObsCollectionAsync ( bvm ,
					"" ,
					BankDb . Text ,
					UseSort . IsChecked == true ? OrderString . Text : "" ,
					UseConditions . IsChecked == true ? Conditions . Text : "" ,
					true ,
					true ,
					  false ,
					"DAPPERTESTING" ,
					args );
			}
			if ( UseAsyncLoading )
			{
				cvm = DapperSupport . GetCustObsCollection ( cvm ,
				"" ,
				CustDb . Text ,
				     UseSort . IsChecked == true ? OrderString . Text : "" ,
				     UseConditions . IsChecked == true ? Conditions . Text : "" ,
				true ,
					true ,
					false ,
					"DAPPERTESTING" ,
					args );
			}
			else
			{
				cvm = DapperSupport . GetCustObsCollection ( cvm ,
				"" ,
				CustDb . Text ,
				     UseSort . IsChecked == true ? OrderString . Text : "" ,
				     UseConditions . IsChecked == true ? Conditions . Text : "" ,
				true ,
					true ,
					false ,
					"DAPPERTESTING" ,
					args );
			}
			if ( UseAsyncLoading )
			{
				bool resut  = await DapperSupport . GetDetailsObsCollectionAsync ( dvm , "" , DetDb . Text ,
				   UseSort . IsChecked == true ? OrderString . Text : "" ,
				UseConditions . IsChecked == true ? Conditions . Text : "" ,
				true ,
				true ,
				false ,
				"DAPPERTESTING" ,
				args );
			}
			else
			{
				dvm = DapperSupport . GetDetailsObsCollection ( dvm , "" , DetDb . Text ,
				   UseSort . IsChecked == true ? OrderString . Text : "" ,
				UseConditions . IsChecked == true ? Conditions . Text : "" ,
				true ,
				true ,
				false ,
				"DAPPERTESTING" ,
				args );
			}
		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void LoadGrids ( )
		{
			GenericGrid1 . ItemsSource = bvm;
			GenericGrid2 . ItemsSource = cvm;
			GenericGrid3 . ItemsSource = dvm;
			GenericGrid1 . SelectedIndex = 0;
			GenericGrid2 . SelectedIndex = 0;
			GenericGrid3 . SelectedIndex = 0;
			GenericGrid1 . UpdateLayout ( );
			GenericGrid2 . UpdateLayout ( );
			GenericGrid3 . UpdateLayout ( );
			BankCount . Text = bvm . Count . ToString ( );
			CustCount . Text = cvm . Count . ToString ( );
			DetCount . Text = dvm . Count . ToString ( );
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void GenericGrid1_RowEditEnding ( object sender , DataGridRowEditEndingEventArgs e )
		{
			SQLHandlers sqlh = new SQLHandlers ( );
			sqlh . UpdateAllDb ( "BANKACCOUNT" , e );
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void GenericGrid2_RowEditEnding ( object sender , DataGridRowEditEndingEventArgs e )
		{
			SQLHandlers sqlh = new SQLHandlers ( );
			sqlh . UpdateAllDb ( "CUSTOMER" , e );
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void GenericGrid3_RowEditEnding ( object sender , DataGridRowEditEndingEventArgs e )
		{
			SQLHandlers sqlh = new SQLHandlers ( );
			sqlh . UpdateAllDb ( "DETAILS" , e );
		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void UseStdDapper_Click ( object sender , RoutedEventArgs e )
		{
			Flags . USESDAPPERSTDPROCEDURES = false;
			Flags . USEDAPPERWITHSTOREDPROCEDURE = false;
			Flags . USEADOWITHSTOREDPROCEDURES = true;
			UseDapperStoredProc . IsChecked = false;
			UseStoredProc . IsChecked = false;
			e . Handled = true;
		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void UseStoredProc_Click ( object sender , RoutedEventArgs e )
		{
			Flags . USESDAPPERSTDPROCEDURES = false;
			Flags . USEDAPPERWITHSTOREDPROCEDURE = true;
			Flags . USEADOWITHSTOREDPROCEDURES = false;
			UseDapperStoredProc . IsChecked = false;
			UseStdDapper . IsChecked = false;
			e . Handled = true;
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void UseDapperStoredProc_Click ( object sender , RoutedEventArgs e )
		{
			Flags . USESDAPPERSTDPROCEDURES = true;
			Flags . USEADOWITHSTOREDPROCEDURES = false;
			Flags . USEDAPPERWITHSTOREDPROCEDURE = false;
			UseStoredProc . IsChecked = false;
			UseStdDapper . IsChecked = false;
			e . Handled = true;
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void BankDb_MouseEnter ( object sender , MouseEventArgs e )
		{
			BankDb . SelectAll ( );
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void CustDb_MouseEnter ( object sender , MouseEventArgs e )
		{
			CustDb . SelectAll ( );
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void DetDb_MouseEnter ( object sender , MouseEventArgs e )
		{
			DetDb . SelectAll ( );
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void DetDb_KeyDown ( object sender , KeyEventArgs e )
		{
			//	if(e.Key == Key.Enter)

		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void MaxRecordsToLoad_Click ( object sender , RoutedEventArgs e )
		{
			if ( MaxRecords . Text != "*" )
			{
				try
				{
					if ( int . Parse ( MaxRecords . Text ) > 0 )
						args [ 2 ] = int . Parse ( MaxRecords . Text );
					else
						args [ 2 ] = 0;
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL Error {ex . Message}, {ex . Data}" );
				}
			}
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private async void ReloadBank_Click ( object sender , RoutedEventArgs e )
		{
			args [ 0 ] = 0;
			args [ 1 ] = 0;
			args [ 2 ] = 0;
			if ( MaxRecords . Text != "*" && MaxRecordsToLoad . IsChecked == true )
			{
				if ( MaxRecords . Text == "*" )
				{
					MaxRecords . Text = "";
					args [ 2 ] = 0;
				}
				else
				{
					if ( MaxRecords . Text == "" )
						args [ 2 ] = 0;
					else
						args [ 2 ] = int . Parse ( MaxRecords . Text );
				}
			}

			GenericGrid1 . ItemsSource = null;
			GenericGrid1 . Items . Clear ( );
			GenericGrid1 . UpdateLayout ( );
			GenericGrid1 . Refresh ( );
			if ( UseAsyncLoading )
			{
				timer . Start ( );
				startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				bool  result = await DapperSupport . GetBankObsCollectionAsync ( bvm ,
				"" ,
				BankDb . Text ,
				UseSort . IsChecked == true ? OrderString . Text : "" ,
				UseConditions . IsChecked == true ? Conditions . Text : "" ,
				true ,
				true ,
				  false ,
				"DAPPERTESTING" ,
				args );
			}
			else
			{
				timer . Start ( );
				startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				bvm = DapperSupport . GetBankObsCollection ( bvm ,
				"" ,
				BankDb . Text ,
				UseSort . IsChecked == true ? OrderString . Text : "" ,
				UseConditions . IsChecked == true ? Conditions . Text : "" ,
				true ,
				true ,
				  false ,
				"DAPPERTESTING" ,
				args );

				GenericGrid1 . ItemsSource = bvm;
				GenericGrid1 . SelectedIndex = 0;
				GenericGrid1 . UpdateLayout ( );
				BankCount . Text = bvm . Count . ToString ( );
				timer . Stop ( );
				endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				//if ( endsecs - startsecs < 0 )
				//	Debugger . Break ( );
				LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
				GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
				GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
				GenericGrid1 . Background = FindResource ( "Red5" ) as SolidColorBrush;
				GenericGrid1 . Focus ( );
			}
			CurrBank . Content = BankDb . Text . ToUpper ( );
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private async void ReloadCust_Click ( object sender , RoutedEventArgs e )
		{
			args [ 0 ] = 0;
			args [ 1 ] = 0;
			args [ 2 ] = 0;
			if ( MaxRecords . Text != "*" && MaxRecordsToLoad . IsChecked == true )
			{
				if ( MaxRecords . Text == "*" )
				{
					MaxRecords . Text = "";
					args [ 2 ] = 0;
				}
				else
				{
					if ( MaxRecords . Text == "" )
						args [ 2 ] = 0;
					else
						args [ 2 ] = int . Parse ( MaxRecords . Text );
				}
			}

			GenericGrid2 . ItemsSource = null;
			GenericGrid2 . Items . Clear ( );
			GenericGrid2 . UpdateLayout ( );
			GenericGrid2 . Refresh ( );

			if ( UseAsyncLoading )
			{
				timer . Start ( );
				startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				GenericGrid2 . ItemsSource = null;
				GenericGrid2 . Items . Clear ( );
				GenericGrid2 . UpdateLayout ( );
				GenericGrid2 . Refresh ( );
				bool result = await DapperSupport . GetCustObsCollectionAsync ( cvm ,
				"" ,
				CustDb . Text ,
				     UseSort . IsChecked == true ? OrderString . Text : "" ,
				     UseConditions . IsChecked == true ? Conditions . Text : "" ,
				true ,
					true ,
					false ,
					"DAPPERTESTING" ,
					args );
			}
			else
			{
				timer . Start ( );
				startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				cvm = DapperSupport . GetCustObsCollection ( cvm ,
				"" ,
				CustDb . Text ,
				     UseSort . IsChecked == true ? OrderString . Text : "" ,
				     UseConditions . IsChecked == true ? Conditions . Text : "" ,
				true ,
					true ,
					false ,
					"DAPPERTESTING" ,
					args );
				GenericGrid2 . ItemsSource = null;
				GenericGrid2 . Items . Clear ( );
				GenericGrid2 . UpdateLayout ( );
				GenericGrid2 . Refresh ( );
				GenericGrid2 . ItemsSource = cvm;
				GenericGrid2 . SelectedIndex = 0;
				GenericGrid2 . UpdateLayout ( );
				CustCount . Text = cvm . Count . ToString ( );
				timer . Stop ( );
				endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				//if ( endsecs - startsecs < 0 )
				//	Debugger . Break ( );
				LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
				GenericGrid2 . Background = FindResource ( "Red5" ) as SolidColorBrush;
				GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
				GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
				GenericGrid2 . Focus ( );
			}
			CurrCust . Content = CustDb . Text . ToUpper ( );
		}

		private async void ReloadDet_Click ( object sender , RoutedEventArgs e )
		{
			args [ 0 ] = 0;
			args [ 1 ] = 0;
			args [ 2 ] = 0;
			if ( MaxRecords . Text != "*" && MaxRecordsToLoad . IsChecked == true )
			{
				if ( MaxRecords . Text == "*" )
				{
					MaxRecords . Text = "";
					args [ 2 ] = 0;
				}
				else
				{
					if ( MaxRecords . Text == "" )
						args [ 2 ] = 0;
					else
						args [ 2 ] = int . Parse ( MaxRecords . Text );
				}
			}

			timer . Start ( );
			startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
			GenericGrid3 . ItemsSource = null;
			GenericGrid3 . Items . Clear ( );
			GenericGrid3 . UpdateLayout ( );
			GenericGrid3 . Refresh ( );
			if ( UseAsyncLoading )
			{
				timer . Start ( );
				startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				bool result  = await DapperSupport . GetDetailsObsCollectionAsync ( dvm , "" , DetDb . Text ,
				   UseSort . IsChecked == true ? OrderString . Text : "" ,
				UseConditions . IsChecked == true ? Conditions . Text : "" ,
				true ,
				true ,
				true ,
				"DAPPERTESTING" ,
				args );
			}
			else
			{
				startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				dvm = DapperSupport . GetDetailsObsCollection ( dvm , "" , DetDb . Text ,
				   UseSort . IsChecked == true ? OrderString . Text : "" ,
				UseConditions . IsChecked == true ? Conditions . Text : "" ,
				true ,
				true ,
				false ,
				"DAPPERTESTING" ,
				args );
				GenericGrid3 . ItemsSource = null;
				GenericGrid3 . Items . Clear ( );
				GenericGrid3 . UpdateLayout ( );
				GenericGrid3 . Refresh ( );
				GenericGrid3 . ItemsSource = dvm;
				GenericGrid3 . SelectedIndex = 0;
				GenericGrid3 . UpdateLayout ( );
				DetCount . Text = dvm . Count . ToString ( );
				timer . Stop ( );
				endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				//if ( endsecs - startsecs < 0 )
				//	Debugger . Break ( );
				LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
				GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
				GenericGrid3 . Background = FindResource ( "Red5" ) as SolidColorBrush;
				GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
				GenericGrid3 . Focus ( );
			}
			CurrDet . Content = DetDb . Text . ToUpper ( );
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private async void LoadMulti_Click ( object sender , RoutedEventArgs e )
		{
			// Load the Multi Account holder ALONE 
			args [ 0 ] = 0;
			args [ 1 ] = 0;
			args [ 2 ] = 0;
			if ( MaxRecords . Text != "*" && MaxRecordsToLoad . IsChecked == true )
			{
				if ( MaxRecords . Text == "*" )
				{
					MaxRecords . Text = "";
					args [ 2 ] = 0;
				}
				else
				{
					if ( MaxRecords . Text != "" )
						args [ 2 ] = int . Parse ( MaxRecords . Text );
				}
			}
			GenericGrid1 . ItemsSource = null;
			GenericGrid1 . Items . Clear ( );
			GenericGrid1 . UpdateLayout ( );
			GenericGrid1 . Refresh ( );
			timer . Start ( );
			startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
			if ( UseAsyncLoading )
			{
				bool result = await DapperSupport . GetMultiBankCollectionAsync ( bvm ,
					"" ,
					"Bankaccount" ,
					UseSort . IsChecked == true ? OrderString . Text : "" ,
					UseConditions . IsChecked == true ? Conditions . Text : "" ,
					true ,
					"DAPPERTESTING" ,
					args
				);
			}
			else
			{
				bvm = DapperSupport . GetMultiBankCollection ( bvm ,
				"" ,
				"Bankaccount" ,
				UseSort . IsChecked == true ? OrderString . Text : "" ,
				UseConditions . IsChecked == true ? Conditions . Text : "" ,
				false ,
				"DAPPERTESTING" ,
				args
					);
				GenericGrid1 . ItemsSource = bvm;
				GenericGrid1 . SelectedIndex = 0;
				GenericGrid1 . UpdateLayout ( );
				BankCount . Text = bvm . Count . ToString ( );
				Flags . USEDAPPERWITHSTOREDPROCEDURE = false;
				GenericGrid1 . Background = FindResource ( "Red5" ) as SolidColorBrush;
				GenericGrid1 . Focus ( );
				timer . Stop ( );
				endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				//if ( endsecs - startsecs < 0 )
				//	Debugger . Break ( );
				LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
			}
			CurrBank . Content = BankDb . Text . ToUpper ( );
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void UseAsync_Click ( object sender , RoutedEventArgs e )
		{
			if ( UseAsync . IsChecked == true )
				UseAsyncLoading = true;
			else
				UseAsyncLoading = false;
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private async void UseSelectClause ( object sender , RoutedEventArgs e )
		{
			/* 
			 * Large method that handles processing of "normal program created" queries and user created SQL queries
			 * 	It calls various different methods, both normal and Async to process these requests for speed comparisons
			 */
			string command= ManualSelect.Text.ToUpper().Trim();
			if ( ManualSelect . Text == "" )
			{
				MessageBox . Show ( "Please enter a valid SQL query statement before trying to execute it..." , "Input Error" );
				ManualSelect . Focus ( );
				return;
			}

			Dictionary <string, CustomerViewModel> dict = new Dictionary<string, CustomerViewModel>();
			foreach ( var item in cvm )
			{
				dict . Add ( item . Id . ToString ( ) , item );
			}

			if ( ManualBtnText . Text == "Create Sql Query :-" )
			{
				ManualBtnText . Text = "      Perform  Query :-";
				ManualBtnText . Foreground = FindResource ( "White0" ) as SolidColorBrush;
				ManualSelect . IsEnabled = true;
				ManualSelect . Text = "Enter valid SQL Query here...";
				//Push text to top to allow for wrapping
				ManualSelect . Padding = new Thickness ( 0 , 0 , 0 , 0 );
				ManualSelect . SelectionLength = ManualSelect . Text . Length;
				ManualSelect . Focus ( );
				ManualSelect . Refresh ( );
				return;
			}
			if ( command . Contains ( "USER:" ) || command . Contains ( "ENTER VALID SQL" ) || command != "" )
			{
				string errormsg="";
				if ( command . Length < 8 || command . Contains ( "ENTER VALID SQL" ) )
				{
					if ( command . Contains ( "ENTER VALID SQL" ) )
					{
						ManualSelect . IsEnabled = true;
						ManualSelect . Padding = new Thickness ( 0 , 0 , 0 , 0 );
						ManualSelect . Focus ( );
					}
					else
					{
						MessageBox . Show ( "Your query does not appear to be valid..." , "Input Error" );
						ManualSelect . Focus ( );
					}
					Mouse . OverrideCursor = Cursors . Arrow;
					return;
				}
				Mouse . OverrideCursor = Cursors . Wait;
				if ( UseAsyncLoading )
				{
					timer . Start ( );
					startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					if ( command . Contains ( "USER:" ) )
						command = command . Substring ( 5 );
					UniversalGrid . Visibility = Visibility . Visible;
					UniversalGrid . ItemsSource = null;
					UniversalGrid . Refresh ( );
					ObservableCollection<GenericClass> generics = new();

					try
					{
						if ( command . ToUpper ( ) . Contains ( "SELECT " ) )
						{
							// Generic call that wil return the results of any valid SQL select command as an Observable colection<GenericClass>
							Dictionary < string, string > dic = new Dictionary<string, string>();
							GenericClass gcc = new GenericClass();
							string errmsg="";
							generics = DapperGeneric<Dictionary<string , string> , GenericClass , bool> . CreateFromDictionary (
								 dic ,
								gcc ,
							command ,
							ref errmsg
								);
							if ( errmsg != "" )
							{
								MessageBox . Show ( $"The SQL Query you entered returned the following Error ?\n\n[{errmsg . ToUpper ( )}]" , "SQL error?" );
								Mouse . OverrideCursor = Cursors . Arrow;
								return;
							}
						}
						else if ( SqlServerCommands . CheckforStoredProcedure ( command ) )
						{
							//We have checked for this SP above and  it does exist
							string args = "";
							string[] fields = command.Split(' ');
							if ( fields . Length > 1 )
								args = fields [ 1 ] . Trim ( );
							SqlServerCommands  sqc = new SqlServerCommands ( );
							// call with false to STOP the method flling fields and datagrids in dappersupport/SqlServerCommans
							generics = sqc . ExecuteStoredProcedure ( command , null , "" , args , e , false );
							if ( generics . Count == 0 )
							{
								MessageBox . Show ( $"The Stored procedure [{command . ToUpper ( )}]\n\nyou entered has been executed successfully !\nbut sadly it returned ZERO data records?\n\nPerhaps the procedure performs other tasks\\nand is NOT supposed to return any data ?" , "Stored Procedure Information?" );
								Mouse . OverrideCursor = Cursors . Arrow;
								return;
							}
						}
						else
						{
							MessageBox . Show ( $"The query you have entered entered has not been recognised !\n\n[{command . ToUpper ( )}]\n\nif it is not an exisiting Stored Procedure, so please Check/Correct the query you entered ?" , "Manual Query Error ?" );
							Mouse . OverrideCursor = Cursors . Arrow;
							return;
						}
						//Display the data
						UniversalGrid . ItemsSource = null;
						UniversalGrid . Items . Clear ( );
						SqlServerCommands sqlc = new SqlServerCommands();

						// Caution : This loads the data into the Datarid with only the selected rows
						// //visible in the grid so do NOT repopulate the grid after making this call
						sqlc . LoadActiveRowsOnlyInGrid ( UniversalGrid , generics , DapperSupport . GetGenericColumnCount ( generics ) );
						BankCombinedGrid . Visibility = Visibility . Collapsed;
						UniversalGrid . Visibility = Visibility . Visible;
						UniversalGrid . UpdateLayout ( );
						UniversalGrid . SelectedIndex = 0;
						UniversalGrid . Focus ( );
						CloseGenGridBtn . Opacity = 1;
					}
					catch ( Exception ex )
					{
						MessageBox . Show ( $"The error below (DapperTesting:913) was returned by the Dappersupport library !\n\n[{ex . Message}]\n{ex . Data}\n\nPlease Check the syntax of the query entered ?" , "Dapper Call Error ?" );
						Console . WriteLine ( $"The error below was returned by the Dappersupport library !\n\n[{errormsg}]\n\nPlease Check the path taken?" );
						ManualSelect . IsEnabled = true;
						ManualSelect . Focus ( );
					}
					timer . Stop ( );
					endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
					Mouse . OverrideCursor = Cursors . Arrow;
					if ( generics . Count == 0 )
					{
						if ( errormsg . ToUpper ( ) . Contains ( "SQLERROR" ) )
							MessageBox . Show ( $"The error below was returned by the query entered !\n\n[{errormsg}]\n\nPlease Check/Correct the query you entered ?" , "Manual Query Error ?" );
						else
							MessageBox . Show ( "No records were returned by the query entered !\n\nPlease check the content & syntax of the query entered ?" , "Manual Query Error ?" );
						ManualSelect . IsEnabled = true;
						ManualSelect . Focus ( );
						Mouse . OverrideCursor = Cursors . Arrow;
						return;
					}
					else
						ExtractData . IsEnabled = true;

					Mouse . OverrideCursor = Cursors . Arrow;
					return;
				}
				else
				{
					timer . Start ( );
					startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					command = command . Substring ( 5 );
					UniversalGrid . Visibility = Visibility . Visible;
					UniversalGrid . ItemsSource = null;
					UniversalGrid . Refresh ( );
					List<Dictionary<string, string>> OutData = new List<Dictionary<string, string>>();
					List<string> DbData = new List<string>(0);
					List<string> ReceivedDbData = new List<string>();
					try
					{
						ReceivedDbData = DapperSupport . GetGenericCollection ( DbData , command , true , "" );
						Console . WriteLine ( $"ReceivedDbData contains {ReceivedDbData . Count} records" );
						CreateDatabase ( UniversalGrid , ReceivedDbData );
					}
					catch ( Exception ex )
					{
						MessageBox . Show ( $"The error below (DapperTesting:953) was returned by the Dappersupport library !\n\n[{ex . Message}]\n{ex . Data}\n\nPlease Check the path taken?" , "Dapper Call Error ?" );
						Console . WriteLine ( $"The error below was returned by the Dappersupport library !\n\n[{errormsg}]\n\nPlease Check the path taken?" );
					}
					CloseGenGridBtn . Opacity = 1;
					timer . Stop ( );
					endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
					Mouse . OverrideCursor = Cursors . Arrow;
					return;

				}
			}
			if ( Flags . USEDAPPERWITHSTOREDPROCEDURE == true )
			{
				MessageBox . Show ( $"Manual Commands cannot be processed by Stored Procedures, \nplease change the Mode in use to ONE of the other 2 options..." );
				return;
			}
			if ( command . Contains ( "FROM" ) && command . Contains ( "BANK" ) )
			{
				Mouse . OverrideCursor = Cursors . Wait;
				GenericGrid1 . ItemsSource = null;
				GenericGrid1 . Items . Clear ( );
				GenericGrid1 . UpdateLayout ( );
				GenericGrid1 . Refresh ( );
				if ( UseAsyncLoading )
				{
					timer . Start ( );
					startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					try
					{
						bool result = await DapperSupport . GetBankObsCollectionAsync ( bvm ,
						ManualSelect . Text , DetDb . Text ,
					   "" ,
					"" ,
					false ,
					false ,
					true ,
					"DAPPERTESTING" ,
					args );
						if ( result == false )
						{
							GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
							GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
							GenericGrid1 . Background = FindResource ( "Red5" ) as SolidColorBrush;
							MessageBox . Show ( $"The Phrase you have entered for the BankAccount Db was NOT valid, Please correct and try again... " );
						}
					}
					catch ( Exception ex )
					{
						MessageBox . Show ( $"The error below (DapperTesting:997) was returned by the Dappersupport library !\n\n[{ex . Message}]\n{ex . Data}\n\nPlease Check the path taken?" , "Dapper Call Error ?" );
						Console . WriteLine ( $"The error below was returned by the Dappersupport library !\n\n[[{ex . Message}]\n\nPlease Check the path taken?" );
					}
				}
				else
				{
					timer . Start ( );
					startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					bvm = DapperSupport . GetBankObsCollection ( bvm ,
					ManualSelect . Text , DetDb . Text ,
					  "" ,
					"" ,
					false ,
					false ,
					false ,
					"DAPPERTESTING" ,
					args );
					GenericGrid1 . ItemsSource = bvm;
					GenericGrid1 . SelectedIndex = 0;
					GenericGrid1 . UpdateLayout ( );
					BankCount . Text = bvm . Count . ToString ( );
					Flags . USEDAPPERWITHSTOREDPROCEDURE = false;
					GenericGrid1 . Background = FindResource ( "Red5" ) as SolidColorBrush;
					GenericGrid1 . Focus ( );
					timer . Stop ( );
					endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					//if ( endsecs - startsecs < 0 )
					//	Debugger . Break ( );
					LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
				}
				Mouse . OverrideCursor = Cursors . Arrow;
			}
			else if ( command . Contains ( "FROM" ) && command . Contains ( "CUST" ) )
			{
				if ( Flags . USEDAPPERWITHSTOREDPROCEDURE == true )
				{
					MessageBox . Show ( $"Manual Commands cannot be processed by Stored Procdeures, \nplease change the Mode in use to 1 of the other 2 options..." );
					return;
				}
				Mouse . OverrideCursor = Cursors . Wait;
				GenericGrid2 . ItemsSource = null;
				GenericGrid2 . Items . Clear ( );
				GenericGrid2 . UpdateLayout ( );
				GenericGrid2 . Refresh ( );
				if ( UseAsyncLoading )
				{
					timer . Start ( );
					startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					GenericGrid2 . ItemsSource = null;
					GenericGrid2 . Items . Clear ( );
					GenericGrid2 . UpdateLayout ( );
					GenericGrid2 . Refresh ( );
					bool result = await DapperSupport . GetCustObsCollectionAsync ( cvm ,
					ManualSelect . Text , DetDb . Text ,
					   "" ,
					"" ,
					false ,
					false ,
					false ,
					"DAPPERTESTING" ,
					args );
					if ( result == false )
					{
						GenericGrid2 . Background = FindResource ( "Red0" ) as SolidColorBrush;
						GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
						GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
						MessageBox . Show ( $"The Phrase you have entered for the Customer Db was NOT valid, Please correct and try again... " );
					}
				}
				else
				{
					timer . Start ( );
					startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					cvm = DapperSupport . GetCustObsCollection ( cvm ,
					ManualSelect . Text , DetDb . Text ,
					   "" ,
					"" ,
					false ,
					false ,
					false ,
					"DAPPERTESTING" ,
					args );
					GenericGrid2 . ItemsSource = null;
					GenericGrid2 . Items . Clear ( );
					GenericGrid2 . UpdateLayout ( );
					GenericGrid2 . Refresh ( );
					GenericGrid2 . ItemsSource = cvm;
					GenericGrid2 . SelectedIndex = 0;
					GenericGrid2 . UpdateLayout ( );
					CustCount . Text = cvm . Count . ToString ( );
					timer . Stop ( );
					endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					//if ( endsecs - startsecs < 0 )
					//	Debugger . Break ( );
					LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
					GenericGrid2 . Background = FindResource ( "Red5" ) as SolidColorBrush;
					GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
					GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
					GenericGrid2 . Focus ( );
				}
				Mouse . OverrideCursor = Cursors . Arrow;
			}
			else if ( command . Contains ( "FROM" ) && ( command . Contains ( "DET" ) || command . Contains ( "SECACCOUNT" ) ) )
			{
				if ( Flags . USEDAPPERWITHSTOREDPROCEDURE == true )
				{
					MessageBox . Show ( $"Manual Commands cannot be processed by Stored Procdeures, \nplease change the Mode in use to 1 of the other 2 options..." );
					return;
				}
				Mouse . OverrideCursor = Cursors . Wait;
				GenericGrid3 . ItemsSource = null;
				GenericGrid3 . Items . Clear ( );
				GenericGrid3 . UpdateLayout ( );
				GenericGrid3 . Refresh ( );
				if ( UseAsyncLoading )
				{
					timer . Start ( );
					startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					bool result  = await DapperSupport . GetDetailsObsCollectionAsync ( dvm ,
					ManualSelect . Text , DetDb . Text ,
					   "" ,
					"" ,
					false ,
					false ,
					false ,
					"DAPPERTESTING" ,
					args );
					if ( result == false )
					{
						GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
						GenericGrid3 . Background = FindResource ( "Red5" ) as SolidColorBrush;
						GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
						MessageBox . Show ( $"The Phrase you have entered for the Details Db was NOT valid, Please correct and try again... " );
					}
				}
				else
				{
					timer . Start ( );
					startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					dvm = DapperSupport . GetDetailsObsCollection ( dvm ,
					ManualSelect . Text , DetDb . Text ,
					   "" ,
					"" ,
					false ,
					false ,
					false ,
					"DAPPERTESTING" ,
					args );
					GenericGrid3 . ItemsSource = null;
					GenericGrid3 . Items . Clear ( );
					GenericGrid3 . UpdateLayout ( );
					GenericGrid3 . Refresh ( );
					GenericGrid3 . ItemsSource = dvm;
					GenericGrid3 . SelectedIndex = 0;
					GenericGrid3 . UpdateLayout ( );
					DetCount . Text = dvm . Count . ToString ( );
					timer . Stop ( );
					endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
					GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
					GenericGrid3 . Background = FindResource ( "Red5" ) as SolidColorBrush;
					GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
					GenericGrid3 . Focus ( );
				}
			}
			Mouse . OverrideCursor = Cursors . Arrow;
		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void CloseGenericWindow ( object sender , RoutedEventArgs e )
		{
			// Close Generid selection Grid
			UniversalGrid . Visibility = Visibility . Collapsed;
			UniversalGrid . ItemsSource = null;
			UniversalGrid . Refresh ( );
			BankCombinedGrid . Visibility = Visibility . Collapsed;
			BankCombinedGrid . ItemsSource = null;
			BankCombinedGrid . Refresh ( );
			// rest Db entries to dummy entries
			SetDummyGridEntries ( );
			tFlags . current = 2;
			ToggleBtn_Click ( null , null );
			CloseGenGridBtn . Opacity = 0.6;
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void UseDapper_MouseEnter ( object sender , MouseButtonEventArgs e )
		{
			UseManualDapper . IsEnabled = true;
		}

		// Toggle Button  handlers
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void Button_Indeterminate ( object sender , RoutedEventArgs e )
		{
			// Blue ellipse visible	  - startup condition - Both grids closed
			// NO special grids visible
			UniversalGrid . Visibility = Visibility . Collapsed;
			UniversalGrid . Refresh ( );
			BankCombinedGrid . Visibility = Visibility . Collapsed;
			BankCombinedGrid . Refresh ( );
			GridsLabel . Text = "Standard view\nClick for Bank+Customer grid";
			GenericGrid1 . Focus ( );
			ToggleBtn . IsChecked = null;
			//ToggleBtn . Background = FindResource ( "EllipseBluegradientbackground" ) as LinearGradientBrush;
			ToggleBtn . Refresh ( );
		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void Button_Checked ( object sender , RoutedEventArgs e )
		{
			// Green ellipse visible  - Combined Grid visible
			// 2nd in sequence Blue - Green - Red
			UniversalGrid . Visibility = Visibility . Collapsed;
			UniversalGrid . Refresh ( );
			BankCombinedGrid . Visibility = Visibility . Visible;
			BankCombinedGrid . Refresh ( );
			GridsLabel . Text = "Combined data Grid \nClick again for standard grids";
			BankCombinedGrid . Focus ( );
			ToggleBtn . IsChecked = true;
			//ToggleBtn . Background = FindResource ( "EllipseGreengradientbackground" ) as LinearGradientBrush;
			ToggleBtn . Refresh ( );
		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void Button_Unchecked ( object sender , RoutedEventArgs e )
		{
			// Red  ellipse visible	 - Universal Grid visible
			// 3rd in sequence
			BankCombinedGrid . Visibility = Visibility . Collapsed;
			BankCombinedGrid . Refresh ( );
			UniversalGrid . Visibility = Visibility . Visible;
			UniversalGrid . Refresh ( );
			GridsLabel . Text = "Manual Query result\nClick button to show Combined Db Grid";
			UniversalGrid . Focus ( );
			ToggleBtn . IsChecked = false;
			//ToggleBtn . Background = FindResource ( "EllipseRedgradientbackground" ) as LinearGradientBrush;
			ToggleBtn . Refresh ( );
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private async void LoadCombined_Click ( object sender , RoutedEventArgs e )
		{
			timer . Start ( );
			startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
			bcvm = await DapperSupport . CreateBankCombinedAsync ( bcvm ,
			 "" ,
		     false );
			if ( bcvm != null )
			{
				Console . WriteLine ( "BankCombined Db Created/Recreated successfully..." );
				BankCombinedGrid . ItemsSource = bcvm;
				BankCombinedGrid . UpdateLayout ( );
				BankCombinedGrid . Refresh ( );
				BankCombinedGrid . Visibility = Visibility . Visible;
				GridsLabel . Text = "Combined data Grid \nClick button to hide special grids";
				timer . Stop ( );
				endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
				tFlags . current = 1;
				ToggleBtn_Click ( null , null );
			}
			else
			{
				BankCombinedGrid . ItemsSource = bcvm;
				BankCombinedGrid . UpdateLayout ( );
				BankCombinedGrid . Refresh ( );
				BankCombinedGrid . Visibility = Visibility . Visible;
				timer . Stop ( );
				endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
				tFlags . current = 1;
				ToggleBtn_Click ( null , null );
			}

		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void ToggleBtn_Click ( object sender , RoutedEventArgs e )
		{
			if ( tFlags . current == 0 )      // None
			{
				tFlags . current = 1;        // move to Universal
				Button_Unchecked ( null , null );
			}
			else if ( tFlags . current == 1 )      // Generic/Universal
			{
				tFlags . current = 2;         // move to Combined
				Button_Checked ( null , null );
			}
			else if ( tFlags . current == 2 ) // Combined
			{
				tFlags . current = 0;          // move to standard
				Button_Indeterminate ( null , null );
			}

		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		void SetDummyGridEntries ( )
		{
			BankCombinedGrid . ItemsSource = null;
			BankCombinedViewModel bcv = new BankCombinedViewModel();
			bcv . FName = "Combined Db has not been loaded ....";
			bcvm . Clear ( );
			bcvm . Add ( bcv );
			BankCombinedGrid . ItemsSource = bcvm;

			UniversalGrid . ItemsSource = null;
			GenericClass  gc = new GenericClass ( );
			gc . field5 = "No Generic Query has been performed....";
			ObservableCollection<GenericClass> gcollection = new ObservableCollection<GenericClass>();
			gcollection . Clear ( );
			gcollection . Add ( gc );
			UniversalGrid . ItemsSource = gcollection;
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void Extract_Click ( object sender , RoutedEventArgs e )
		{
			int index = 0;
			string output = "";
			GenericClass gc = new GenericClass();
			if ( IsGenericListResult )
			{

				var dg =MessageBox . Show ( $"The Generic table already contains a set of extracted records.\n\n"
					+ "You cannot perform an extract again from these records, but \nyou can use the Save button to Save/Resave the contents to\na new Sql Table.\n\n"
					+ "Would you like to Clear the contents of the data grid instead?" , "User selectio Error !" , MessageBoxButton . YesNo);
				if ( dg == MessageBoxResult . Yes )
				{
					UniversalGrid . ItemsSource = null;
					UniversalGrid . Items . Clear ( );
					UniversalGrid . Refresh ( );
					UniversalGrid . Visibility = Visibility . Collapsed;

					IsGenericListResult = false;
					SaveData . IsEnabled = false;
					ExtractBankDbSaveName . Text = "Db Name ...";
					ExtractBankDbSaveName . Opacity = 0.4;
					ExtractBankDbSaveName . Foreground = FindResource ( "Black4" ) as SolidColorBrush;

				}
				return;
			}
			selectedgenerics . Clear ( );
			// Create a new generic Db containing selected records only
			if ( UniversalGrid . SelectedItems . Count > 0 )
			{
				foreach ( var data in UniversalGrid . SelectedItems )
				{
					string[] temp = data.ToString().Split(',');
					if ( temp . Length > 0 )
						output = ParseDataGridRowToString ( temp );
					else
					{
						DataGridRow row = UniversalGrid.ItemContainerGenerator.ContainerFromIndex (index++) as DataGridRow;
						string s = row.Item.ToString();
						string[] fields = row.Item.ToString().Split(',');
						output = ParseDataGridRowToString ( fields );
					}
					//Add data to the generic collection
					CreateAddGenericRecord ( selectedgenerics , output );
				}
				UniversalGrid . ItemsSource = null;
				UniversalGrid . ItemsSource = selectedgenerics;
				UniversalGrid . Refresh ( );
				SaveData . IsEnabled = true;
				IsGenericListResult = true;
				ExtractBankDbSaveName . Opacity = 1.0;
				ExtractBankDbSaveName . Foreground = FindResource ( "Red5" ) as SolidColorBrush;
			}
		}
		#region KEY HANDLERS
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void ManualSelect_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
				UseSelectClause ( sender , null );
		}
		#endregion KEY HANDLERS

		#region FOCUS HANDLERS
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void GenericGrid1_GotFocus ( object sender , RoutedEventArgs e )
		{
			GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid1 . Background = FindResource ( "Red5" ) as SolidColorBrush;
			BankCombinedGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
			UniversalGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void GenericGrid2_GotFocus ( object sender , RoutedEventArgs e )
		{
			GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid2 . Background = FindResource ( "Red5" ) as SolidColorBrush;
			BankCombinedGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
			UniversalGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void GenericGrid3_GotFocus ( object sender , RoutedEventArgs e )
		{
			GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid3 . Background = FindResource ( "Red5" ) as SolidColorBrush;
			BankCombinedGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
			UniversalGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void UniversalGrid_GotFocus ( object sender , RoutedEventArgs e )
		{
			GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
			BankCombinedGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
			UniversalGrid . Background = FindResource ( "Red5" ) as SolidColorBrush;

		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private void BankCombinedGrid_GotFocus ( object sender , RoutedEventArgs e )
		{
			GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
			BankCombinedGrid . Background = FindResource ( "Red5" ) as SolidColorBrush;
			UniversalGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
		}
		#endregion FOCUS HANDLERS

		#region Generic data parsing methods
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		public string ParseDataGridRowToString ( string [ ] fields )
		{
			String output="";
			int index = 0;
			foreach ( var item in fields )
			{
				string[] temp= item . Split ( '=' );
				if ( temp . Length > 0 )
				{
					if ( temp [ 1 ] . Trim ( ) . Contains ( "}" ) )
					{
						if ( ( temp [ 1 ] . Trim ( ) . Contains ( " " ) ) || ( temp [ 1 ] . Trim ( ) . Contains ( "/" ) && temp [ 1 ] . Trim ( ) . Contains ( ":" ) ) ) // Date/Time
							output += "'" + temp [ 1 ] . Substring ( 0 , temp [ 1 ] . Length - 1 ) . Trim ( ) + "',";
						else
							output += temp [ 1 ] . Substring ( 0 , temp [ 1 ] . Length - 1 ) . Trim ( ) + ",";
					}
					else
					{
						if ( ( temp [ 1 ] . Trim ( ) . Contains ( " " ) ) || ( temp [ 1 ] . Trim ( ) . Contains ( "/" ) && temp [ 1 ] . Trim ( ) . Contains ( ":" ) ) ) // Date/Time
							output += "'" + temp [ 1 ] + "',";
						else
							output += temp [ 1 ] + ",";
					}
				}
				else
				{
					if ( item . Trim ( ) . Contains ( "{" ) )
					{
						if ( ( item . Contains ( " " ) ) || ( item . Contains ( "/" ) && item . Contains ( ":" ) ) ) // Date/Time
							output += "'" + item . Substring ( 1 , item . Length - 1 ) . Trim ( ) + "',";
						else
							output += item . Substring ( 1 , item . Length - 1 ) . Trim ( ) + ",";
					}
					else if ( item . Contains ( "}" ) )
					{
						if ( ( item . Contains ( " " ) ) || ( item . Contains ( "/" ) && item . Contains ( ":" ) ) ) // Date/Time
							output += "'" + item . Substring ( 0 , item . Length - 1 ) . Trim ( ) + "',";
						else
							output += item . Substring ( 0 , item . Length - 1 ) . Trim ( ) + ",";
					}
					else
					{
						if ( ( item . Contains ( " " ) ) || ( item . Contains ( "/" ) && item . Contains ( ":" ) ) ) // Date/Time
							output += "'" + item . Trim ( ) + "',";
						else
							output += item . Trim ( ) + ",";
					}
				}
			}
			output = output . Substring ( 0 , output . Length - 1 ) . Trim ( );
			return output;
		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private async void SaveDb_Click ( object sender , RoutedEventArgs e )
		{
			// save data we have selected from another db in 'selectedgenerics'
			bool success=true;
			string dbname = ExtractBankDbSaveName.Text;
			string errorstring="";
			ExtractBankDbSaveName . Opacity = 1.0;
			if ( dbname == "" || ExtractBankDbSaveName . Text . ToUpper ( ) . Contains ( "DB NAME" ) )
			{
				MessageBox . Show ( "You MUST provide the name of the new table in the field to the right..." , "Query syntax error" );
				return;
			}
			try
			{
				string[] args ={ "","",""};
				string createcommand=$" (Id INT IDENTITY(1,1) NOT NULL,";

				// 1st we need to DROP any existing table
				args [ 0 ] = dbname;
				Task<int>  result = DapperSupport . PerformSqlExecuteCommandAsync ( "spDropTable",args);
				if ( errorstring != "" )
				{
					MessageBox . Show ( $"An error was encounterd as shown below...\n\n[{errorstring}]" , "SQL Query Error" );
					success = false;
				}

				// Now we  need to create our fields list for the CREATE command
				int columnstotal = DapperSupport. GetGenericColumnCount ( selectedgenerics );
				for ( int i = 1 ; i <= columnstotal ; i++ )
				{
					createcommand += $" field{i} NVARCHAR(100), ";
				}

				// Now we can CREATE a new Db to match our new Db data structure
				string tmp = createcommand.Trim().Substring(0, createcommand.Trim().Length-1);
				createcommand = tmp += " )";
				args [ 0 ] = dbname;
				args [ 1 ] = createcommand;
				result = DapperSupport . PerformSqlExecuteCommandAsync ( "spCreateTable" , args );
				if ( errorstring != "" )
				{
					MessageBox . Show ( $"An error was encountered as shown below...\n\n[{errorstring}]" , "SQL Query Error" );
					success = false;
				}
				// Now INSERT DATA
				//First, we create the(fields clause) of the Insert command for the new table 
				createcommand = "( ";
				for ( int i = 1 ; i <= columnstotal ; i++ )
				{
					createcommand += $" field{i}, ";
				}
				tmp = createcommand . Trim ( ) . Substring ( 0 , createcommand . Trim ( ) . Length - 1 );
				createcommand = tmp += " )  ";
				args [ 1 ] = createcommand;

				// now create the VALUES clause
				GenericClass gc = new GenericClass();
				foreach ( var item in selectedgenerics )
				{
					string fldvalue="";
					gc = item;
					createcommand = "";
					for ( int i = 1 ; i <= columnstotal ; i++ )
					{
						fldvalue = GetDataValue ( gc , i ) . Trim ( );
						if ( fldvalue . Contains ( "}" ) )
							fldvalue = fldvalue . Substring ( 0 , fldvalue . Length - 1 );
						else if ( fldvalue . Contains ( "'" ) )
							fldvalue = fldvalue . Substring ( 1 , fldvalue . Length - 2 );
						// We need t warp data in quotes as it can be anything, including having spaces in it !!
						createcommand += $" '{fldvalue}', ";
					}

					tmp = createcommand . Trim ( ) . Substring ( 0 , createcommand . Trim ( ) . Length - 1 );
					args [ 2 ] = tmp;
					result = DapperSupport . PerformSqlExecuteCommandAsync ( "spInsertSpecifiedRow" , args );
					if ( errorstring != "" )
					{
						MessageBox . Show ( $"An error was encounterd as shown below...\n\n[{errorstring}]" , "SQL Query Error" );
						success = false;
					}
				}
				if ( success )
				{
					MessageBox . Show ( $"The data in the table above has been saved successfully as\n\nTable [{dbname . ToUpper ( )}]" , "SQL Processing System" );
					//					SaveData . Content = "Finished...";
				}
			}
			catch ( Exception ex )
			{
				// This catches any exceptions that dapper suppport may return as well as we "throw" them down there
				if ( errorstring == "" )
				{
					Console . WriteLine ( $"Error in saving data : [{ex . Message}]" );
					MessageBox . Show ( $"The data has NOT been saved. Error reported is :\n\n[{ex . Message}]" , "SQL Processing Error" );
				}
				else
				{
					Console . WriteLine ( $"Error in saving data : \n\n[{errorstring}]\n\n[{ex . Message}]" );
					MessageBox . Show ( $"The data has NOT been saved. Error reported is :\n\n[{ex . Message}]" , "SQL Processing Error" );
				}
			}

		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		public string GetDataValue ( GenericClass gc , int index )
		{
			string fldval = "";
			switch ( index )
			{
				case 1:
					fldval = gc . field1;
					break;
				case 2:
					fldval = gc . field2;
					break;
				case 3:
					fldval = gc . field3;
					break;
				case 4:
					fldval = gc . field4;
					break;
				case 5:
					fldval = gc . field5;
					break;
				case 6:
					fldval = gc . field6;
					break;
				case 7:
					fldval = gc . field7;
					break;
				case 8:
					fldval = gc . field8;
					break;
				case 9:
					fldval = gc . field9;
					break;
				case 10:
					fldval = gc . field10;
					break;
				case 11:
					fldval = gc . field11;
					break;
				case 12:
					fldval = gc . field12;
					break;
				case 13:
					fldval = gc . field13;
					break;
				case 14:
					fldval = gc . field14;
					break;
				case 15:
					fldval = gc . field15;
					break;
				case 16:
					fldval = gc . field16;
					break;
				case 17:
					fldval = gc . field17;
					break;
				case 18:
					fldval = gc . field18;
					break;
				case 19:
					fldval = gc . field19;
					break;
				case 20:
					fldval = gc . field20;
					break;
			}
			return fldval;
		}
		/// <summary>
		/// Generic method to create an  observableCollection<GenericClass> for an unkown data set retreived via Dapper sql Query
		/// The GenericClass Class has 20 fields so data can be parsed from a string into the relevant fields
		/// and used as ItemsSource for a datagrid
		/// </summary>
		/// <param name="dgrid"></param>
		/// <param name="ReceivedDbData"></param>
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		public ObservableCollection<GenericClass> CreateDatabase ( DataGrid dgrid , List<string> ReceivedDbData , int returnDb = 1 )
		{
			string datain="";
			// Post process data string received 
			ObservableCollection<GenericClass> genericcollection = new ObservableCollection<GenericClass>();
			for ( int x = 0 ; x < ReceivedDbData . Count ; x++ )
			{
				datain = ReceivedDbData [ x ];
				string[] fields = datain.Split(',');
				GenericClass genclass = new GenericClass();
				for ( int z = 0 ; z < fields . Length - 1 ; z++ )
				{
					string[] inner = fields[z].Split('=');
					try
					{
						switch ( z + 1 )
						{
							case 1:
								genclass . field1 = inner [ 1 ];
								break;
							case 2:
								genclass . field2 = inner [ 1 ];
								break;
							case 3:
								genclass . field3 = inner [ 1 ];
								break;
							case 4:
								genclass . field4 = inner [ 1 ];
								break;
							case 5:
								genclass . field5 = inner [ 1 ];
								break;
							case 6:
								genclass . field6 = inner [ 1 ];
								break;
							case 7:
								genclass . field7 = inner [ 1 ];
								break;
							case 8:
								genclass . field8 = inner [ 1 ];
								break;
							case 9:
								genclass . field9 = inner [ 1 ];
								break;
							case 10:
								genclass . field10 = inner [ 1 ];
								break;
							case 11:
								genclass . field11 = inner [ 1 ];
								break;
							case 12:
								genclass . field12 = inner [ 1 ];
								break;
							case 13:
								genclass . field13 = inner [ 1 ];
								break;
							case 14:
								genclass . field14 = inner [ 1 ];
								break;
							case 15:
								genclass . field15 = inner [ 1 ];
								break;
							case 16:
								genclass . field16 = inner [ 1 ];
								break;
							case 17:
								genclass . field17 = inner [ 1 ];
								break;
							case 18:
								genclass . field18 = inner [ 1 ];
								break;
							case 19:
								genclass . field19 = inner [ 1 ];
								break;
							case 20:
								genclass . field20 = inner [ 1 ];
								break;
						}
					}
					catch ( Exception ex )
					{
						Console . WriteLine ( $"createBd error : - {ex . Message}" );
					}

				}
				genericcollection . Add ( genclass );
			}
			tFlags . current = 1;
			ToggleBtn_Click ( null , null );

			if ( returnDb <= 2 )
			{
				UniversalGrid . ItemsSource = null;
				UniversalGrid . Items . Clear ( );
				UniversalGrid . ItemsSource = genericcollection;
				UniversalGrid . SelectedIndex = 0;
				UniversalGrid . Visibility = Visibility . Visible;
				UniversalGrid . Refresh ( );
				UniversalGrid . Focus ( );
				if ( returnDb == 2 )
					return genericcollection;
				else
					return null;
			}
			else if ( returnDb == 3 )
				return genericcollection;
			else
				return null;
			// *** NB: *** This uses the StringWrapper Class at bottom fo this file to get the content of the input List<string> so they display in a datagrid
			//foreach ( var item in collection )
			//{}
			//				UniversalGrid . ItemsSource = DbData . Select ( s => new { Value = s } ) . ToList ( );
		}
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
		public ObservableCollection<GenericClass> CreateAddGenericRecord ( ObservableCollection<GenericClass> generics , string data )
		{
			string[] fields;
			int indx=1;
			GenericClass gc = new GenericClass();
			fields = data . Split ( ',' );
			indx = 1;
			foreach ( var field in fields )
			{
				string [] tmp;
				tmp = field . Split ( '=' );
				if ( tmp . Length == 2 )
				{
					switch ( indx++ )
					{
						case 1:
							gc . field1 = tmp [ 1 ] . Trim ( );
							break;
						case 2:
							gc . field2 = tmp [ 1 ] . Trim ( );
							break;
						case 3:
							gc . field3 = tmp [ 1 ] . Trim ( );
							break;
						case 4:
							gc . field4 = tmp [ 1 ] . Trim ( );
							break;
						case 5:
							gc . field5 = tmp [ 1 ] . Trim ( );
							break;
						case 6:
							gc . field6 = tmp [ 1 ] . Trim ( );
							break;
						case 7:
							gc . field7 = tmp [ 1 ] . Trim ( );
							break;
						case 8:
							gc . field8 = tmp [ 1 ] . Trim ( );
							break;
						case 9:
							gc . field9 = tmp [ 1 ] . Trim ( );
							break;
						case 10:
							gc . field10 = tmp [ 1 ] . Trim ( );
							break;
						case 11:
							gc . field11 = tmp [ 1 ] . Trim ( );
							break;
						case 12:
							gc . field12 = tmp [ 1 ] . Trim ( );
							break;
						case 13:
							gc . field13 = tmp [ 1 ] . Trim ( );
							break;
						case 14:
							gc . field14 = tmp [ 1 ] . Trim ( );
							break;
						case 15:
							gc . field15 = tmp [ 1 ] . Trim ( );
							break;
						case 16:
							gc . field16 = tmp [ 1 ] . Trim ( );
							break;
						case 17:
							gc . field17 = tmp [ 1 ] . Trim ( );
							break;
						case 18:
							gc . field18 = tmp [ 1 ] . Trim ( );
							break;
						case 19:
							gc . field19 = tmp [ 1 ] . Trim ( );
							break;
						case 20:
							gc . field20 = tmp [ 1 ] . Trim ( );
							break;
					}
				}
				else
				{
					switch ( indx++ )
					{
						case 1:
							gc . field1 = field . Trim ( );
							break;
						case 2:
							gc . field2 = field . Trim ( );
							break;
						case 3:
							gc . field3 = field . Trim ( );
							break;
						case 4:
							gc . field4 = field . Trim ( );
							break;
						case 5:
							gc . field5 = field . Trim ( );
							break;
						case 6:
							gc . field6 = field . Trim ( );
							break;
						case 7:
							gc . field7 = field . Trim ( );
							break;
						case 8:
							gc . field8 = field . Trim ( );
							break;
						case 9:
							gc . field9 = field . Trim ( );
							break;
						case 10:
							gc . field10 = field . Trim ( );
							break;
						case 11:
							gc . field11 = field . Trim ( );
							break;
						case 12:
							gc . field12 = field . Trim ( );
							break;
						case 13:
							gc . field13 = field . Trim ( );
							break;
						case 14:
							gc . field14 = field . Trim ( );
							break;
						case 15:
							gc . field15 = field . Trim ( );
							break;
						case 16:
							gc . field16 = field . Trim ( );
							break;
						case 17:
							gc . field17 = field . Trim ( );
							break;
						case 18:
							gc . field18 = field . Trim ( );
							break;
						case 19:
							gc . field19 = field . Trim ( );
							break;
						case 20:
							gc . field20 = field . Trim ( );
							break;
					}
				}
			}
			generics . Add ( gc );
			return generics;
		}


		#endregion Generic data parsing methods

		private void CloseGrids ( object sender , RoutedEventArgs e )
		{
			UniversalGrid . Visibility = Visibility . Collapsed;
			BankCombinedGrid . Visibility = Visibility . Collapsed;
		}

		private void ManualSelect_LostFocus ( object sender , RoutedEventArgs e )
		{
			if ( ManualSelect . Text == "Enter valid SQL Query here..." || ManualSelect . Text == "" )
			{
				ManualSelect . IsEnabled = false;
				UseManualDapper . Content = "Enter Manual Command :-";
				ManualBtnText . Text = "Enter valid SQL Query here...";
				ManualSelect . Padding = new Thickness ( 0 , 15 , 0 , 0 );

			}
		}

		private void OntopChkbox_Click ( object sender , RoutedEventArgs e )
		{
			if ( OntopChkbox . IsChecked == true )
				this . Topmost = true;
			else
				this . Topmost = false;
		}

		private void DbList_Loaded ( object sender , RoutedEventArgs e )
		{
			int currindex = 0;
			ObservableCollection <GenericClass>Generics= new ObservableCollection<GenericClass>();
			SqlServerCommands  ssc = new  SqlServerCommands();
			ssc . ExecuteStoredProcedure ( "spGetTablesList" , Generics , "" , "" , null );
			DbList . Items . Clear ( );
			foreach ( var item in Generics )
			{
				DbList . Items . Add ( item . field1 );
			}
			DbList . SelectedIndex = currindex == -1 ? 0 : currindex;
			DbList . SelectedItem = DbList . SelectedIndex;
			DbList . AllowDrop = true;
			DbList . IsEditable = true;
			DbList . MaxHeight = 120;
			//DbCopiedResult . Text = $"SQL Command [{SqlCommand}] completed successfully...";
		}

		private void DbList_MouseRtBtnUp ( object sender , MouseButtonEventArgs e )
		{
			// Get the data from the selected Db and display it in generic grid
			// Generic call that wil return the results of any valid SQL select command as an Observable colection<GenericClass>
			e . Handled = true;
//			goto TestMsgbox;

			Dictionary < string, string > dic = new Dictionary<string, string>();
			GenericClass gcc = new GenericClass();
			ObservableCollection< GenericClass > generic = new ObservableCollection<GenericClass> ( );
			string errmsg="";
			generic = DapperGeneric<Dictionary<string , string> , GenericClass , bool> . CreateFromDictionary (
				 dic ,
				gcc ,
				$"select * from {DbList . SelectedItem . ToString ( )}" ,
				 ref errmsg );

			if ( errmsg != "" )
			{
				MessageBox . Show ( $"The SQL Query you entered returned the following Error ?\n\n[{errmsg . ToUpper ( )}]" , "SQL error?" );
				Mouse . OverrideCursor = Cursors . Arrow;
				goto TestMsgbox;
			}
			if ( generic . Count == 0 )
			{
				//MessageBox . Show ( $"The selected Data table \n\n[{DbList.SelectedItem.ToString().ToUpper()}] \n\nwas read successfully but returned Zero records" , "SQL error?" );

				Utils . Mbox ( this,
					string1: $"The selected Data table \n\n[{DbList . SelectedItem . ToString ( ) . ToUpper ( )}] \n\nwas read successfully but returned Zero records" ,
					caption: "Sql Error" ,
					Btn1: mb . OK ,
					Btn2: mb . NNULL,
					defButton: mb . OK);

				Mouse . OverrideCursor = Cursors . Arrow;
			}

			UniversalGrid . ItemsSource = null;
			UniversalGrid . Items . Clear ( );
			UniversalGrid . ItemsSource = generic;
			UniversalGrid . SelectedIndex = 0;
			UniversalGrid . Visibility = Visibility . Visible;
			UniversalGrid . Refresh ( );
			UniversalGrid . Focus ( );
			tFlags . current = 0;
			ToggleBtn_Click ( null , null );
			
			
			return;


		TestMsgbox:
			// How to get a Brush from my Extensions Class
			// WORKS
			Brush b = "#FF00FF00".ToSolidBrush();
			// How to get a LinerGradientBrush from my Extensions Class
			// WORKS
			Brush l = "TextWhiteToBlackHorizontal4" .ToLinearGradientBrush();

			// Set the parameter required for the message boxes
			DlgInput . resetdata = true;
			DlgInput . UseIcon = true;
			DlgInput . UseDarkMode = false;
			//DlgInput . resultboolin = false;
			//DlgInput . intin = 0;
			//DlgInput . stringin = "";
			//DlgInput . obj = null;
			//DlgInput . bground = "#9DFFFFFB" . ToSolidBrush ( );
			//DlgInput . fground = "#FF000000" . ToSolidBrush ( );
			//DlgInput . border = "#C1000000" . ToSolidBrush ( );
			//DlgInput . Buttonforeground = null;
			//DlgInput . Buttonbackground = null;
			//DlgInput . iconstring = "";
			//DlgInput . image = null;

			int go= 1;
			if ( go == 1 )
			{
				// Snippet  is <msgbf>
				Msgbox mbox  = new Msgbox(
				caption: "*** SQL Query Error ***" ,
				string1:$"This is the Middle, and main row of data used to \ncreate the information provided.\nThis is  the duplicate to make it longer than the window should be able to use, and  this is  This is  the duplicate to make it longer than the window should be able to use" ,
				string2: "string  2 goes here" ,
				string3:"This is string3, a full width footer style row that can be used,",
				title:"",
				iconstring:"\\icons\\text-message.png" ,
				defButton:2,
				Btn1:1,
				Btn2:2 ,
				Btn3:3,
				Btn4:4,
				btn1Text:"",
				btn2Text:"Get on with it" ,
				btn3Text:"Bale out quick"   ,
				btn4Text:""
			     );
				mbox . ShowDialog ( );
			}
			else if ( go == 2 )
			{
				Msgbox mboxs  = new Msgbox(
				caption: "*** SQL Query Error ***" ,
				string1:$"This is the Middle, and main row of data used to \ncreate the information provided.\nThis is  the duplicate to make it longer than the window should be able to use, and  this is  This is  the duplicate to make it longer than the window should be able to use" ,
				string2: "" ,
				title:"",
				iconstring:"\\icons\\check-mark-icon-5375.png" ,
				defButton: 1,
				Btn1:3,
				Btn2: 4 ,
				btn1Text:"",
				btn2Text:"Get on with it" ,
				btn3Text:"Bale out quick"   ,
				btn4Text:""

			     );
				mboxs . ShowDialog ( );
			}
			else if ( go == 3 )
			{
				// Snippet  is <mcfg>
				//DlgInput . resetdata = false;
				//DlgInput . resultboolin = false;
				//DlgInput . UseDarkMode = false;
				//DlgInput . UseIcon = true;
				//DlgInput . intin = 0;
				//DlgInput . stringin = "";
				//DlgInput . obj = null;
				//DlgInput . bground = "#9DFFFFFB" . ToSolidBrush ( );
				//DlgInput . fground = "#FF000000" . ToSolidBrush ( );
				//DlgInput . border = "#C1000000" . ToSolidBrush ( );
				//DlgInput . Buttonforeground = null;
				//DlgInput . Buttonbackground = null;
				//DlgInput . iconstring = "";
				//DlgInput . image = null;
				 
				// Snippet  is <mss>
				Utils . Mbox (this,
					string1: "long Message to show how it wraps in a default message box like this ...." ,
					string2: "",
					caption: "" ,
					iconstring: "\\icons\\Information.png" ,
					Btn1: MB . OK,
					Btn2: MB . CANCEL ,
					defButton: MB . OK	);




			}
		}

		private void DbList_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
		{
			DbList . IsDropDownOpen = true;

		}

		private void DbList_MouseRightButtonDown ( object sender , MouseButtonEventArgs e )
		{
			string s = e . OriginalSource . ToString ( );
		}
	}

	#region extension classes
	class StringWrapper
	{
		string Value { get; set; }
	}
	[Serializable]
	public class GenericClass
	{
		public GenericClass ( )
		{

		}
		public string field1 { get; set; }
		public string field2 { get; set; }
		public string field3 { get; set; }
		public string field4 { get; set; }
		public string field5 { get; set; }
		public string field6 { get; set; }
		public string field7 { get; set; }
		public string field8 { get; set; }
		public string field9 { get; set; }
		public string field10 { get; set; }
		public string field11 { get; set; }
		public string field12 { get; set; }
		public string field13 { get; set; }
		public string field14 { get; set; }
		public string field15 { get; set; }
		public string field16 { get; set; }
		public string field17 { get; set; }
		public string field18 { get; set; }
		public string field19 { get; set; }
		public string field20 { get; set; }

		//public IEnumerator GetEnumerator ( )
		//{
		//	return this . GetEnumerator ( );
		//}

	}

	public class GenericHeaders
	{
		string Header { get; set; }
	}
	public class GenericFields
	{
		string FieldName { get; set; }
	}
	#endregion extension classes

}
