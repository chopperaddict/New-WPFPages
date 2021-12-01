using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Configuration;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Runtime . Remoting . Metadata . W3cXsd2001;
using System . Text;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Controls . Primitives;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
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
	public partial class DapperTesting : Window
	{
		ObservableCollection<BankCombinedViewModel> bcvm = new ObservableCollection<BankCombinedViewModel>();
		ObservableCollection<BankAccountViewModel> bvm = new ObservableCollection<BankAccountViewModel>();
		ObservableCollection<CustomerViewModel> cvm = new ObservableCollection<CustomerViewModel>();
		ObservableCollection<DetailsViewModel> dvm = new ObservableCollection<DetailsViewModel>();
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
//			public bool[] status ;
			public int current;
		}
		ToggleFlags tFlags = new ToggleFlags();
		public double Checked
		{
			get
			{ return ( double ) GetValue ( CheckedProperty ); }
			set { SetValue ( CheckedProperty , value ); }
		}

		public static readonly DependencyProperty CheckedProperty =
				DependencyProperty . Register ( "Checked",
				typeof ( int ),
				typeof ( DapperTesting),
				new PropertyMetadata ( ( int) 0 ), OnCheckedPropertyChanged );

		private static bool OnCheckedPropertyChanged ( object value )
		{
			int x = Convert.ToInt32(value);
			ToggleBtnStatus = ( x );

			return true;
		}

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
		private void Window_Loaded ( object sender , RoutedEventArgs e )
		{
			this . Show ( );
			Mouse . OverrideCursor = Cursors . Wait;
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
		}

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

		private void Timer_Tick ( object sender , EventArgs e )
		{
			endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
		}

		private void button_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void Standardbutton_Click ( object sender , RoutedEventArgs e )
		{
			Mouse . OverrideCursor = Cursors . Wait;
			Flags . USECOPYDATA = false;
			LoadDbs ( );
			LoadGrids ( );
			Mouse . OverrideCursor = Cursors . Arrow;
		}

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
			//if ( CreateCombinedDb )
			//{
			//	bool result = await DapperSupport.CreateBankCombinedAsync (  bcvm ,
			//      "" ,
			//    false );
			//	if ( result )
			//	{
			//		Console . WriteLine ( "BankCombined Db Created/Recreated successfully..." );
			//		bcvm = DapperSupport . GetBankCombinedDb ( bcvm ,
			//		"" ,
			//		"BankCombined" ,
			//		"" ,
			//		"" ,
			//		false ,
			//		false ,
			//		"DAPPERTESTING" ,
			//		args );

			//		BankCombinedGrid . ItemsSource = bcvm;
			//		BankCombinedGrid . UpdateLayout ( );
			//		BankCombinedGrid . Refresh (  );
			//		BankCombinedGrid . Visibility = Visibility . Visible;
			//	}
			//}
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

		private void button3_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void button4_Click ( object sender , RoutedEventArgs e )
		{
			this . Close ( );
		}

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

		private void GenericGrid1_RowEditEnding ( object sender , DataGridRowEditEndingEventArgs e )
		{
			SQLHandlers sqlh = new SQLHandlers ( );
			sqlh . UpdateAllDb ( "BANKACCOUNT" , e );
		}

		private void GenericGrid2_RowEditEnding ( object sender , DataGridRowEditEndingEventArgs e )
		{
			SQLHandlers sqlh = new SQLHandlers ( );
			sqlh . UpdateAllDb ( "CUSTOMER" , e );
		}

		private void GenericGrid3_RowEditEnding ( object sender , DataGridRowEditEndingEventArgs e )
		{
			SQLHandlers sqlh = new SQLHandlers ( );
			sqlh . UpdateAllDb ( "DETAILS" , e );
		}

		private void UseStdDapper_Click ( object sender , RoutedEventArgs e )
		{
			Flags . USESDAPPERSTDPROCEDURES = false;
			Flags . USEDAPPERWITHSTOREDPROCEDURE = false;
			Flags . USEADOWITHSTOREDPROCEDURES = true;
			UseDapperStoredProc . IsChecked = false;
			UseStoredProc . IsChecked = false;
			e . Handled = true;
		}
		private void UseStoredProc_Click ( object sender , RoutedEventArgs e )
		{
			Flags . USESDAPPERSTDPROCEDURES = false;
			Flags . USEDAPPERWITHSTOREDPROCEDURE = true;
			Flags . USEADOWITHSTOREDPROCEDURES = false;
			UseDapperStoredProc . IsChecked = false;
			UseStdDapper . IsChecked = false;
			e . Handled = true;
		}

		private void UseDapperStoredProc_Click ( object sender , RoutedEventArgs e )
		{
			Flags . USESDAPPERSTDPROCEDURES = true;
			Flags . USEADOWITHSTOREDPROCEDURES = false;
			Flags . USEDAPPERWITHSTOREDPROCEDURE = false;
			UseStoredProc . IsChecked = false;
			UseStdDapper . IsChecked = false;
			e . Handled = true;
		}

		private void BankDb_MouseEnter ( object sender , MouseEventArgs e )
		{
			BankDb . SelectAll ( );
		}

		private void CustDb_MouseEnter ( object sender , MouseEventArgs e )
		{
			CustDb . SelectAll ( );
		}

		private void DetDb_MouseEnter ( object sender , MouseEventArgs e )
		{
			DetDb . SelectAll ( );
		}

		private void DetDb_KeyDown ( object sender , KeyEventArgs e )
		{
			//	if(e.Key == Key.Enter)

		}

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

		private void GenericGrid1_GotFocus ( object sender , RoutedEventArgs e )
		{
			GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid1 . Background = FindResource ( "Red5" ) as SolidColorBrush;
			BankCombinedGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
			UniversalGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
		}

		private void GenericGrid2_GotFocus ( object sender , RoutedEventArgs e )
		{
			GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid2 . Background = FindResource ( "Red5" ) as SolidColorBrush;
			BankCombinedGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
			UniversalGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
		}

		private void GenericGrid3_GotFocus ( object sender , RoutedEventArgs e )
		{
			GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid3 . Background = FindResource ( "Red5" ) as SolidColorBrush;
			BankCombinedGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
			UniversalGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
		}
		private void UniversalGrid_GotFocus ( object sender , RoutedEventArgs e )
		{
			GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
			BankCombinedGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
			UniversalGrid . Background = FindResource ( "Red5" ) as SolidColorBrush;

		}

		private void BankCombinedGrid_GotFocus ( object sender , RoutedEventArgs e )
		{
			GenericGrid2 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid3 . Background = FindResource ( "White0" ) as SolidColorBrush;
			GenericGrid1 . Background = FindResource ( "White0" ) as SolidColorBrush;
			BankCombinedGrid . Background = FindResource ( "Red5" ) as SolidColorBrush;
			UniversalGrid . Background = FindResource ( "White0" ) as SolidColorBrush;
		}

		private void UseAsync_Click ( object sender , RoutedEventArgs e )
		{
			if ( UseAsync . IsChecked == true )
				UseAsyncLoading = true;
			else
				UseAsyncLoading = false;
		}

		private async void UseSelectClause ( object sender , RoutedEventArgs e )
		{
			string command= ManualSelect.Text.ToUpper().Trim();
			if ( ManualBtnText . Text == "Create Manual  Command :-" )
			{
				ManualBtnText . Text = "Use Manual Command ...";
				ManualSelect . Text = "User: ";
				ManualSelect . CaretIndex = ManualSelect . Text . Length;
				ManualSelect . Focus ( );
				return;
			}
			if ( command . Contains ( "USER:" ) )
			{
				Mouse . OverrideCursor = Cursors . Wait;
				if ( UseAsyncLoading )
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
					var result = await DapperSupport . GetGenericCollectionAsync ( DbData , command , true , "" );
					Console . WriteLine ( $"ReceivedDbData contains {DbData . Count} records" );
					CreateDatabase ( UniversalGrid , DbData );
					CloseGenGridBtn . Opacity = 1;
					timer . Stop ( );
					endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					//if(endsecs - startsecs < 0)
					//		Debugger . Break ( );
					LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
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
					ReceivedDbData = DapperSupport . GetGenericCollection ( DbData , command , true , "" );
					Console . WriteLine ( $"ReceivedDbData contains {ReceivedDbData . Count} records" );
					CreateDatabase ( UniversalGrid , ReceivedDbData );
					CloseGenGridBtn . Opacity = 1;
					timer . Stop ( );
					endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
					//if ( endsecs - startsecs < 0 )
					//	Debugger . Break ( );
					LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
					Mouse . OverrideCursor = Cursors . Arrow;
					return;

				}
			}
			if ( Flags . USEDAPPERWITHSTOREDPROCEDURE == true )
			{
				MessageBox . Show ( $"Manual Commands cannot be processed by Stored Procdeures, \nplease change the Mode in use to 1 of the other 2 options..." );
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

		private void ManualSelect_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . Enter )
				UseSelectClause ( sender , null );
		}

		/// <summary>
		/// Generic method to create a database for an unkown data set retreived via Dapper sql Query
		/// it uses a GenericClass Class that has 20 fields so data can be parsed from a string into fields
		/// and used as ItemsSource for a datagrid
		/// </summary>
		/// <param name="dgrid"></param>
		/// <param name="ReceivedDbData"></param>
		public   void CreateDatabase ( DataGrid dgrid , List<string> ReceivedDbData )
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
					catch ( Exception ex ) { }

				}
				genericcollection . Add ( genclass );
			}
			tFlags . current = 1;
			ToggleBtn_Click ( null , null );
			
			
			UniversalGrid . ItemsSource = genericcollection;
			UniversalGrid . SelectedIndex = 0;
			UniversalGrid . Visibility = Visibility . Visible;
			UniversalGrid . Refresh ( );
			UniversalGrid . Focus ( );

			// *** NB: *** This uses the StringWrapper Class at bottom fo this file to get the content of the input List<string> so they display in a datagrid
			//foreach ( var item in collection )
			//{}
			//				UniversalGrid . ItemsSource = DbData . Select ( s => new { Value = s } ) . ToList ( );
		}

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

		private void ManualSelect_MouseEnter ( object sender , MouseEventArgs e )
		{

		}

		private void UseDapper_MouseEnter ( object sender , MouseButtonEventArgs e )
		{
			UseManualDapper . IsEnabled = true;
		}

		// Toggle Button  handlers
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
			ToggleBtn . Refresh ( );
		}
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
			ToggleBtn . Refresh ( );
		}
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
			ToggleBtn . Refresh ( );
		}

		private async void LoadCombined_Click ( object sender , RoutedEventArgs e )
		{
			//DependencyObject dpo = new DependencyObject ( );
			//object  dobj = dpo . GetValue ( DapperTesting . CheckedProperty);
			//int offset = System.Convert.ToInt32(dobj);
			timer . Start ( );
			startsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
			bool result = await DapperSupport.CreateBankCombinedAsync (  bcvm ,
			 "" ,
		     false );
			if ( result )
			{
				Console . WriteLine ( "BankCombined Db Created/Recreated successfully..." );
				bcvm = DapperSupport . GetBankCombinedDb (
					bcvm ,
					"" ,
					"BankCombined " ,
					UseSort . IsChecked == true ? OrderString . Text : "" ,
					UseConditions . IsChecked == true ? Conditions . Text : "" ,
					true ,
					false ,
					"DAPPERTESTING" ,
					args );

				BankCombinedGrid . ItemsSource = bcvm;
				BankCombinedGrid . UpdateLayout ( );
				BankCombinedGrid . Refresh ( );
				BankCombinedGrid . Visibility = Visibility . Visible;
				GridsLabel . Text = "Combined data Grid \nClick button to hide special grids";
				timer . Stop ( );
				endsecs = DateTime . Now . Second * 1000 + DateTime . Now . Millisecond;
				LoadTime . Text = ( endsecs - startsecs ) . ToString ( ) + " Milliseconds";
				tFlags . current = 1;
				ToggleBtn_Click (null,null );
			}

		}
		private void ToggleBtn_Click ( object sender , RoutedEventArgs e )
		{
			if ( tFlags . current == 0 )      // None
			{
				tFlags . current = 1;        // move to Universal
				Button_Unchecked( null , null );
			}
			else if ( tFlags . current == 1 )      // Generic/Universal
			{
				tFlags .current = 2;		// move to Combined
				Button_Checked ( null , null );
			}
			else if ( tFlags . current == 2 ) // Combined
			{
				tFlags. current = 0;          // move to standard
				Button_Indeterminate ( null , null );
			}

		}
		
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


	}

	#region extension classes
	class StringWrapper
	{
		string Value { get; set; }
	}
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
