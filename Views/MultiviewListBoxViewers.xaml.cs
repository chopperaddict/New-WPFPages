//===============================================//
// It is best to leave this as USEASYNC					27/10/21
//===============================================//
#define USEASYNC
//#undef USEASYNC
#define USECOPYDB
//#undef USECOPYDB
using Microsoft . CSharp . RuntimeBinder;

using Newtonsoft . Json . Linq;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Reflection;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Threading;

using WPFPages . ViewModels;

//===============================================//
// NB The Async search methods all work quite well !!! 27/10/21
//===============================================//

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for UserListBoxViewer.xaml
	/// This has both Datagrids and Lisboxes for Bank & Customer & can switch between view
	/// plus the grids have an ACTIVE search on custNo 
	/// </summary>
	public partial class MultiviewListBoxViewers : Window
	{
		#region Declarations
		// Declare all 3 of the local Db pointers
		public static ObservableCollection<BankAccountViewModel >SqlBankAccounts= new ObservableCollection<BankAccountViewModel >();
		public static ObservableCollection<CustomerViewModel>SqlCustAccounts= new ObservableCollection<CustomerViewModel>();
		public static ObservableCollection<DetailsViewModel>SqlDetAccounts= new ObservableCollection<DetailsViewModel>();

		public static CollectionView SqlBankView;
		public static CollectionView SqlCustView;
		public static CollectionView SqlDetView;
		//		public static BankCollection SqlBankcollection = new BankCollection();
		//		public BankCollection BackupBankcollection = new BankCollection();
		//		public AllCustomers SqlCustcollection = new AllCustomers();

		public static List<BankAccountViewModel> BankList = new List<BankAccountViewModel>();
		public List<CustomerViewModel> CustList = new List<CustomerViewModel>();
		public List<DetailsViewModel> DetList = new List<DetailsViewModel>();

		private static Dictionary <int, int>bankdict = new Dictionary<int, int>();
		private static Dictionary <int, int>custdict = new Dictionary<int, int>();
		private static Dictionary <int, int>detdict = new Dictionary<int, int>();
		private static int  lastBankSearchValue = 0;
		private static int  lastCustSearchValue = 0;
		private static int  lastDetSearchValue = 0;


		private static readonly DataGridColumn dataGridColumn   ;
		private DataGridColumn[] DGBankColumnsCollection = {dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,
			dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn };
		private DataGridColumn[] DGCustColumnsCollection
			= {dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,
			dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn ,dataGridColumn };
		private DataGridColumn[] DGDetasilsColumnsCollection = {dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn,
			dataGridColumn,dataGridColumn,dataGridColumn,dataGridColumn };

		private bool ExpandAll = false;
		private bool MouseLeftBtnDown = false;
		//CURRENT Foreground color
		private Brush CurrentForeColor = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x00));  // current foreground =black
		private Brush CurrentBackColor = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));  // current foreground =White
		private string CurrentCellName = "";
		private bool areItemsExpanded;
		public object FilterBankData { get; private set; }

		public double uclistboxheight = 0;
		public double uclistbox2height = 0;

		private static DataGrid BankGrid;
		private static DataGrid CustGrid;
		private static ListBox Uclistbox;
		private static ListBox Uclistbox2;
		private static TextBox ErrorLabel;

		private bool IsLeftButtonDown = false;
		private static Point _startPoint
		{
			get; set;
		}
		private static bool ScrollBarMouseMove
		{
			get; set;
		}

		#endregion Declarations

		#region full properties

		private double dg1Width;

		public double Dg1Width
		{
			get { return dg1Width; }
			set { dg1Width = value; }
		}

		public bool AreItemsExpanded
		{
			get
			{
				return areItemsExpanded;
			}
			set
			{
				areItemsExpanded = value;
			}
		}

		private double rowheight = 25;
		public double Rowheight
		{
			get
			{
				return rowheight;
			}
			set
			{
				rowheight = value;
			}
		}
		private string tbBalance = "";
		public string TbBalance
		{
			get
			{
				return tbBalance;
			}
			set
			{
				tbBalance = value;
			}
		}
		// CURRENT Background color
		private Brush tbCurrentBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
		public Brush TbCurrentBrush
		{
			get
			{
				return tbCurrentBrush;
			}
			set
			{
				tbCurrentBrush = value;
			}
		}

		private int currentindex;
		public int CurrentIndex
		{
			get
			{
				return currentindex;
			}
			set
			{
				currentindex = value;
			}
		}

		private bool isdirty;
		public bool IsDirty
		{
			get
			{
				return isdirty;
			}
			set
			{
				isdirty = value;
			}
		}
		private int gridSelection;
		public int GridSelection
		{
			get
			{
				return gridSelection;
			}
			set
			{
				gridSelection = value;
			}
		}
		private int listSelection;
		public int ListSelection
		{
			get
			{
				return listSelection;
			}
			set
			{
				listSelection = value;
			}
		}

		private int gridSelection2;
		public int GridSelection2
		{
			get
			{
				return gridSelection2;
			}
			set
			{
				gridSelection2 = value;
			}
		}
		private int listSelection2;
		public int ListSelection2
		{
			get
			{
				return listSelection2;
			}
			set
			{
				listSelection2 = value;
			}
		}

		private bool itemExpanded = true;
		public bool ItemExpanded
		{
			get
			{
				return itemExpanded;
			}
			set
			{
				itemExpanded = value;
			}
		}
		private bool isSelected;
		public bool IsSelected
		{
			get
			{
				return isSelected;
			}
			set
			{
				isSelected = value;
			}
		}
		#endregion full properties

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged ( string PropertyName )
		{
			if ( null != PropertyChanged )
			{
				PropertyChanged ( this ,
					new PropertyChangedEventArgs ( PropertyName ) );
			}
		}
		#endregion INotifyPropertyChanged Members

		#region Dependency properties


		//============================================================//
		/// <summary>
		/// Used to trigger Search thru Datagrids for matching CustNo records
		/// </summary>
		public string SearchText
		{
			get { return ( string ) GetValue ( SearchTextProperty ); }
			set { SetValue ( SearchTextProperty , value ); }
		}
		// Using a DependencyProperty as the backing store for SearchText.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SearchTextProperty =
			DependencyProperty.Register("SearchText", typeof(string), typeof(MultiviewListBoxViewers), new PropertyMetadata("", new PropertyChangedCallback(OnsearchTextChanged)));
		private static void OnsearchTextChanged ( DependencyObject d , DependencyPropertyChangedEventArgs e )
		{
			DoSearchAsync ( e . NewValue );
		}
		//============================================================//

		//public string Search
		//{
		//	get { return ( string ) GetValue ( SearchProperty ); }
		//	set { SetValue ( SearchProperty , value ); }
		//}
		//// Using a DependencyProperty as the backing store for Search.  This enables animation, styling, binding, etc...
		//public static readonly DependencyProperty SearchProperty =
		//	DependencyProperty.Register("Search", typeof(string), typeof(MultiviewListBoxViewers), new PropertyMetadata(""), DoSearchAsync);

		#region Search via Dp value (Search) 
		private static Action <object, object> action = new Action <object, object> (PerformSearchAsync);
		//		private static bool DoSearchAsync ( object value )
		private static bool DoSearchAsync ( object value )
		{
			// NB: WE do NOT get the full window in this call !!
#if USEASYNC
			Stopwatch swmain = new Stopwatch();
			swmain . Start ( );
			int result =0;
			if ( ErrorLabel == null )
				return true;
			if ( ( string ) value != "" )
			{
				PerformSearchAsync ( value , null );
				//Application . Current . Dispatcher . Invoke ( ( ) =>
				//{
				//	// Calls PerformSearchAsync) via Action delegate
				//	action ( value , null );
				//} );
			}
			else
			{
				// Reset grids to index 0 as search field is empty
				Application . Current . Dispatcher . Invoke ( ( ) =>
				{
					BankGrid . SelectedIndex = 0;
					CustGrid . SelectedIndex = 0;
					//					BankGrid . Refresh ( );
					//					CustGrid . Refresh ( );
					ErrorLabel . Visibility = Visibility . Collapsed;
					BankGrid . BringIntoView ( );
					//					BankGrid . ScrollIntoView ( 0 );
					Utils . SetGridRowSelectionOn ( BankGrid , 0 );
					CustGrid . BringIntoView ( );
					//					CustGrid . ScrollIntoView ( 0 );
					Utils . SetGridRowSelectionOn ( CustGrid , 0 );

				} );
			}
			bool b = true;
			swmain . Stop ( );
			Console . WriteLine ( $"TOTAL TIME TAKEN WAS {swmain . ElapsedMilliseconds} milliseconds" );
			return true;
#else
			// This is the active NON ASYNC method called
			int result =0;
			if ( ErrorLabel == null )
				return true;
			if((string)value != "")
				result = PerformSearch ( value, null );
			if ( result == -1 )
			{
				Utils . DoErrorBeep ( );
				ErrorLabel . Visibility = Visibility . Visible;
				ErrorLabel . Refresh ( );
			}
			else
			{
				ErrorLabel . Visibility = Visibility . Collapsed;
				ErrorLabel . Refresh ( );
			}		
			return true;
#endif

		}
		#endregion Search via Dp value (Search) 

		#region VisibilityDP declaration &  handler
		public Visibility Searchvisibility
		{
			get { return ( Visibility ) GetValue ( SearchvisibilityProperty ); }
			set { SetValue ( SearchvisibilityProperty , value ); }
		}
		// Using a DependencyProperty as the backing store for Searchvisibility.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SearchvisibilityProperty =
			DependencyProperty.Register("Searchvisibility", typeof(Visibility), typeof(MultiviewListBoxViewers), new PropertyMetadata(Visibility.Collapsed, checkvis), DoVisibility);
		private static void checkvis ( DependencyObject d , DependencyPropertyChangedEventArgs e )
		{
			if ( ( Visibility ) e . NewValue == Visibility . Collapsed )
				return;
		}
		private static bool DoVisibility ( object value )
		{
			if ( ( Visibility ) value == Visibility . Collapsed )
				value = Visibility . Visible;
			else
				value = Visibility . Collapsed;
			return true;
		}
		#endregion VisibilityDP declaration &  handler

		#endregion Dependency properties

#if USEASYNC
		// Nothing in here ...,.
#else
		private static void DoSearch ( DependencyObject d , DependencyPropertyChangedEventArgs e )
		{
			// NB: WE do get the full window in this call !!
			//int result =await PerformSearchAsync ( d , e );
			int result = PerformSearch ( d , e );
			if ( result == -1 )
			{
				if ( ErrorLabel . Visibility == Visibility . Collapsed )
				{
					Utils . DoErrorBeep ( );
					ErrorLabel . Visibility = Visibility . Visible;
					ErrorLabel . Refresh ( );
				}
			}
			else
			{
				if ( ErrorLabel . Visibility == Visibility . Visible )
				{
					ErrorLabel . Visibility = Visibility . Collapsed;
					ErrorLabel . Refresh ( );
				}
			}
		}
#endif

#if USEASYNC

		#region This is the MAIN Async Search Method : PerformAsyncSearch
		private static void SearchAction ( string type , int lastindex , bool isvalid = false )
		{
			if ( type == "LOW" )
			{
				BankGrid . SelectedIndex = 0;
				//				BankGrid . BringIntoView ( );
				//				BankGrid . ScrollIntoView ( 0 );
				Utils . SetGridRowSelectionOn ( BankGrid , 0 );
				BankGrid . UpdateLayout ( );

				// Save last successful find
				lastBankSearchValue = 0;
				CustGrid . SelectedIndex = 0;
				//				CustGrid . BringIntoView ( );
				//				CustGrid . ScrollIntoView ( 0 );
				Utils . SetGridRowSelectionOn ( CustGrid , 0 );
				CustGrid . UpdateLayout ( );
				// Save last successful find
				lastCustSearchValue = 0;
				if ( isvalid == false )
				{
					Utils . DoErrorBeep ( );
					Application . Current . Dispatcher . Invoke ( ( ) =>
					{
						ErrorLabel . Visibility = Visibility . Visible;
						ErrorLabel . UpdateLayout ( );
					} );
				}
				else
				{
					if ( ErrorLabel . Visibility == Visibility . Visible )
					{
						Application . Current . Dispatcher . Invoke ( ( ) =>
					    {
						    ErrorLabel . Visibility = Visibility . Collapsed;
						    ErrorLabel . UpdateLayout ( );
					    } );
					}
				}
			}
			else
			{
				BankGrid . SelectedIndex = lastindex;
				//				BankGrid . BringIntoView ( );
				//				BankGrid . ScrollIntoView ( lastindex );
				Utils . SetGridRowSelectionOn ( BankGrid , lastindex );
				BankGrid . UpdateLayout ( );
				CustGrid . SelectedIndex = lastindex;
				//				CustGrid . BringIntoView ( );
				//				CustGrid . ScrollIntoView ( lastindex );
				Utils . SetGridRowSelectionOn ( CustGrid , lastindex );
				CustGrid . UpdateLayout ( );
				// Save last successful find
				lastBankSearchValue = lastindex;
				Application . Current . Dispatcher . Invoke ( ( ) =>
				{
					ErrorLabel . Visibility = Visibility . Visible;
					//					ErrorLabel . BringIntoView ( );
					ErrorLabel . UpdateLayout ( );
					Utils . DoErrorBeep ( );
				} );
			}
		}

		//################################/################################//
		//#########################SEARCHING#############################//
		//################################/################################//

		private static void PerformSearchAsync ( object d , object e )
		{
			#region Search Setup
			int banktot = bankdict.Count;
			int custtot = custdict.Count;
			int result = 0;
			int bankhigh= 0;
			int custhigh = 0;
			KeyValuePair<int, int> BankMinValue = bankdict.First();
			KeyValuePair <int, int>CustMinValue = custdict.First();

			BankMinValue = bankdict . Last ( );
			CustMinValue = custdict . Last ( );
			bankhigh = BankMinValue . Key;
			custhigh = CustMinValue . Key;
			bool error = false;
			bool foundbank = false;
			bool foundcust = false;
			//			string srchterm="";
			string spaces = "000000";
			//			string custmatch="";
			string temp="";
			int counter = 0, currentcustno=0;
			if ( d == null || ( string ) d == "" )
				return;
			int custnotofind=int.Parse(d.ToString());
			// check if the search field value has REDUCED (Backspaced) & reset start point if it has
			if ( custnotofind < lastCustSearchValue )
				lastCustSearchValue = 0;
			if ( custnotofind < lastBankSearchValue )
				lastBankSearchValue = 0;
			ItemCollection ic;
			if ( BankGrid . SelectedIndex == -1 )
				BankGrid . SelectedIndex = 0;
			if ( CustGrid . SelectedIndex == -1 )
				CustGrid . SelectedIndex = 0;
			if ( BankGrid . SelectedIndex == BankGrid . Items . Count - 1 )
				BankGrid . SelectedIndex = 0;
			if ( CustGrid . SelectedIndex == CustGrid . Items . Count - 1 )
				CustGrid . SelectedIndex = 0;
			ic = BankGrid . Items;
			temp = custnotofind . ToString ( );
			if ( temp . Length < 7 )
			{
				temp += spaces . Substring ( 0 , 7 - temp . Length );
				custnotofind = int . Parse ( temp );
			}

			#endregion Search Setup

			#region BANK INITIAL SEARCH
			int IndexToSet = 0;
			int CustIndexToSet = 0;
			string findstr= d.ToString();
			ErrorLabel . UpdateLayout ( );
			counter = 0;
			KeyValuePair<int, int> first=bankdict.First();
			KeyValuePair<int, int> last=bankdict.Last();

			// Quick checks first
			if ( custnotofind < first . Key )
			{
				if ( first . Key . ToString ( ) . StartsWith ( findstr ) )
					SearchAction ( "LOW" , first . Value , true );
				else
					SearchAction ( "LOW" , first . Value );
				Console . WriteLine ( $"LOW filter hit..." );
				return;
			}
			if ( custnotofind > last . Key )
			{
				Stopwatch sw = new Stopwatch();
				sw . Start ( );
				SearchAction ( "HIGH" , last . Value );
				Console . WriteLine ( $"HIGH filter hit..." );
				sw . Stop ( );
				Console . WriteLine ( $"Filter 1 took {sw . ElapsedMilliseconds} milliseconds" );
				return;
			}

			//Need to try harder !!!!
			#region Bank Searching
			if ( bankdict . ContainsKey ( custnotofind ) )
			{
				Stopwatch sw = new Stopwatch();
				sw . Start ( );
				// YES it is there, so get it's value and set grid  up correctly
				bankdict . TryGetValue ( custnotofind , out result );
				Console . WriteLine ( $"Bank Found immediately at {result}" );
				IndexToSet = result;
				lastBankSearchValue = result;
				counter = result;
				foundbank = true;
				if ( ErrorLabel . Visibility == Visibility . Visible )
				{
					ErrorLabel . Visibility = Visibility . Collapsed;          // This is just the TextBox, NOT the full window
														     //					ErrorLabel . BringIntoView ( );
					ErrorLabel . UpdateLayout ( );
				}
				sw . Stop ( );
				Console . WriteLine ( $"Search 1 took {sw . ElapsedMilliseconds} milliseconds" );
				goto CustomerSearch;
			}
			else
			{
				Stopwatch sw = new Stopwatch();
				sw . Start ( );

				KeyValuePair<int, int> currlastvalue =bankdict.Last();
				if ( custnotofind > currlastvalue . Key )
				{
					IndexToSet = currlastvalue . Value;
					foundbank = true;
					// Save last successful find
					lastBankSearchValue = counter;
					Application . Current . Dispatcher . Invoke ( ( ) =>
					{
						ErrorLabel . Visibility = Visibility . Visible;          // This is just the TextBox, NOT the full window
															   //						ErrorLabel . BringIntoView ( );
						ErrorLabel . UpdateLayout ( );
						Utils . DoErrorBeep ( );
					} );
					sw . Stop ( );
					Console . WriteLine ( $"Search 2 took {sw . ElapsedMilliseconds} milliseconds" );
					goto CustomerSearch;
				}
				else
				{
					counter = 0;
					sw = new Stopwatch ( );
					sw . Start ( );
					Console . WriteLine ( $"Starting LASTBANKSEARCHVALUE at {lastBankSearchValue}..." );
					//					foreach ( var item in bankdict . Keys )
					//					{
					for ( int x = lastBankSearchValue ; x < bankdict . Count - 1 ; x++ )
					{
						var curr = bankdict . ElementAt ( x );
						if ( curr . Key == custnotofind )
						//							if ( item > custnotofind )
						{
							// We have just passed the nearest value, so select the nearest one
							IndexToSet = counter;
							foundbank = true;
							lastBankSearchValue = counter;
							break;
						}
						counter++;
					}
					sw . Stop ( );
					Console . WriteLine ( $"Search 3 took {sw . ElapsedMilliseconds} milliseconds" );
				}
			}

		#endregion Bank Searching

		#endregion BANK INITIAL SEARCH

		CustomerSearch:

			if ( foundbank == false )
			{
				IndexToSet = 0;
			}

			if ( counter >= banktot - 1 )
				error = true;

			#region Customer Searching

			#region CUSTOMER SEARCH

			counter = 0;
			// see if our Dictionary contains the curremt search value
			if ( custdict . ContainsKey ( custnotofind ) )
			{
				custdict . TryGetValue ( custnotofind , out result );
				Console . WriteLine ( $"Customer Found immediately at {result}" );
				CustIndexToSet = result;
				lastCustSearchValue = result;
				counter = result;
				foundcust = true;
			}
			else
			{
				//No match in Dictionary
				Stopwatch sw = new Stopwatch();
				sw . Start ( );
				Console . WriteLine ( $"Starting LASTCUSTSEARCHVALUEat {lastCustSearchValue}..." );
				counter = 0;

				for ( int x = lastCustSearchValue ; x < custdict . Count - 1 ; x++ )
				{
					var curr = custdict . ElementAt ( x );
					if ( curr . Key == custnotofind )
					{
						CustIndexToSet = curr . Value;
						foundbank = true;
						// Save last successful find
						lastCustSearchValue = curr . Value;
						foundcust = true;
						break;
					}
				}
				foreach ( int item in custdict . Keys )
				{
					if ( item > custnotofind )
					{
						CustIndexToSet = counter;
						foundbank = true;
						// Save last successful find
						lastCustSearchValue = counter;
						//counter = result;
						foundcust = true;
						break;
					}
					counter++;
				}
				//Console . WriteLine ( $"Ending LASTCUSTSEARCHVALUE reset to {lastCustSearchValue}..." );
				Console . WriteLine ( $"Search 4 took {sw . ElapsedMilliseconds} milliseconds" );
				counter = lastCustSearchValue;
			}
			if ( foundcust == false )
			{
				CustIndexToSet = 0;
			}
			#endregion Customer Searching

			#endregion CUSTOMER SEARCH

			// Update grid display
			Refreshgrids ( IndexToSet , CustIndexToSet );

			#region PROCESS SEARCH RESULTS
			// Handle error if no match found and entered value is > any CustNo in the system
			if ( !foundbank && !foundcust )
			{
				if (
					( BankGrid . SelectedIndex > 0 || CustGrid . SelectedIndex > 0 )
					|| ( custnotofind > bankhigh && custnotofind > custhigh ) )
				{
					Application . Current . Dispatcher . Invoke ( ( ) =>
					{
						ErrorLabel . Visibility = Visibility . Visible;          // This is just the TextBox, NOT the full window
															   //						ErrorLabel . BringIntoView ( );
						ErrorLabel . UpdateLayout ( );
						Utils . DoErrorBeep ( );
					} );

				}
				else
				{
					if ( ErrorLabel . Visibility == Visibility . Visible )
					{
						ErrorLabel . Visibility = Visibility . Collapsed;          // This is just the TextBox, NOT the full window
															     //						ErrorLabel . BringIntoView ( );
						ErrorLabel . UpdateLayout ( );
					}
				}
				Console . WriteLine ( $"No matching records exist...." );
			}
			//			else
			currentcustno = custnotofind;

			//			else
			//				ErrorLabel . Visibility = Visibility . Collapsed;	     // This is just the TextBlock, NOT the full window

			Console . WriteLine ( $"lastBankSearchValue [principle : {currentcustno} +  {lastBankSearchValue }], lastCustSearchValue = {lastCustSearchValue}" );
			return;

			#endregion PROCESS SEARCH RESULTS
		}


		private static void Refreshgrids ( int IndexToSet , int CustIndexToSet )
		{
			BankGrid . SelectedIndex = IndexToSet;
			//			BankGrid . BringIntoView ( );
			//			BankGrid . ScrollIntoView ( IndexToSet );
			Utils . SetGridRowSelectionOn ( BankGrid , IndexToSet );
			//			BankGrid . Refresh ( );
			CustGrid . SelectedIndex = CustIndexToSet;
			lastCustSearchValue = CustIndexToSet;
			//			CustGrid . BringIntoView ( );
			//			CustGrid . ScrollIntoView ( CustIndexToSet );
			Utils . SetGridRowSelectionOn ( CustGrid , CustIndexToSet );
			//			CustGrid . UpdateLayout ( );
		}

		#endregion This is the MAIN Async Search Method : PerformAsyncSearch

#else
		#region Non Async Search Methods
		// NON ASYNC Search

		/// <summary>
		/// Complex Search algorithm that reads a TextField entry char by char and "finds it" in both the datagrids
		/// it displays a "Not Found" message if the value entered is > than the values in the datagrids, but NOT
		/// if the value is reduced & is saller than  the smallest datagrid value.
		/// It ONLY searches the CustNo field currently
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		private static int PerformSearch ( object d , object  e )
		{

		#region Search Setup
			TextBlock tb = d as TextBlock;
			int banktot = bankdict.Count;
			int custtot = custdict.Count;
			int result = 0;
			int banklow = 0;
			int custlow = 0;
			KeyValuePair<int, int> BankMinValue = bankdict.First();
			KeyValuePair <int, int>CustMinValue = custdict.First();
			if ( lastBankSearchValue > 0 )
				banklow = lastBankSearchValue;
			else
				banklow = BankMinValue . Key;

			if ( lastCustSearchValue > 0 )
				custlow = CustMinValue . Key;
			else
				custlow = lastCustSearchValue;

			bool error = false;
			bool foundbank = false;
			bool foundcust = false;
			string srchterm="";
			string spaces = "000000";
			string custmatch="";
			string temp="";
			int counter = 0, currentcustno=0;
			if ( d == null || ( string ) d== "" )
				return 0;
			int custnotofind=int.Parse(d.ToString());
			// check if the search field value has REDUCED (Backspaced) & reset start point if it has
			if ( custnotofind < lastCustSearchValue )
				lastCustSearchValue = 0;
			if ( custnotofind < lastBankSearchValue )
				lastBankSearchValue = 0;
			ItemCollection ic;
			if ( BankGrid . SelectedIndex == -1 )
				BankGrid . SelectedIndex = 0;
			if ( CustGrid . SelectedIndex == -1 )
				CustGrid . SelectedIndex = 0;
			if ( BankGrid . SelectedIndex == BankGrid . Items . Count - 1 )
				BankGrid . SelectedIndex = 0;
			if ( CustGrid . SelectedIndex == CustGrid . Items . Count - 1 )
				CustGrid . SelectedIndex = 0;
			BankAccountViewModel bvm =  new BankAccountViewModel();
			CustomerViewModel cvm =  new CustomerViewModel ();
			if ( bvm == null )
				return 0;
			ic = BankGrid . Items;
			temp = custnotofind . ToString ( );
			if ( temp . Length < 7 )
			{
				temp += spaces . Substring ( 0 , 7 - temp . Length );
				custnotofind = int . Parse ( temp );
			}

		#endregion Setup

		#region Search
			srchterm = d. ToString ( );
			counter = 0;
			if ( bankdict . ContainsKey ( custnotofind ) )
			{
				// YES it is there, so get it's value and set grid  up correctly
				bankdict . TryGetValue ( custnotofind , out result );
				Console . WriteLine ( $"Bank Found at {result}" );
				BankGrid . SelectedIndex = result;
				BankGrid . BringIntoView ( );
				BankGrid . ScrollIntoView ( result );
				Utils . SetGridRowSelectionOn ( BankGrid , result );
				BankGrid . Refresh ( );
				lastBankSearchValue = result;
				counter = result;
				foundbank = true;
			}
			else
			{
				counter = lastBankSearchValue;
				Console . WriteLine ( $"Starting LASTBANKSEARCHVALUE at {lastBankSearchValue}..." );
				for ( int x = lastBankSearchValue ; x < bankdict . Count - 1 ; x++ )
				{
					if ( currentcustno > custnotofind )
					{
						BankGrid . SelectedIndex = x;
						BankGrid . BringIntoView ( );
						BankGrid . ScrollIntoView ( x );
						Utils . SetGridRowSelectionOn ( BankGrid , x );
						BankGrid . Refresh ( );
						foundbank = true;
						// Save last successful find
						lastBankSearchValue = x;
						break;
					}
					counter++;
				}
				Console . WriteLine ( $"Ending LASTBANKSEARCHVALUE reset to {lastBankSearchValue}..." );
			}
			if ( foundbank == false )
			{
				BankGrid . SelectedIndex = 0;
				BankGrid . BringIntoView ( );
				BankGrid . ScrollIntoView ( 0 );
				Utils . SetGridRowSelectionOn ( BankGrid , 0 );
				BankGrid . Refresh ( );
			}

			if ( counter >= banktot - 1 )
				error = true;
		
			counter = 0;
			if ( custdict . ContainsKey ( custnotofind ) )
			{
				custdict . TryGetValue ( custnotofind , out result );
				Console . WriteLine ( $"Customer Found at {result}" );
				CustGrid . SelectedIndex = result;
				CustGrid . BringIntoView ( );
				CustGrid . ScrollIntoView ( result );
				Utils . SetGridRowSelectionOn ( CustGrid , result );
				CustGrid . UpdateLayout ( );
				lastCustSearchValue = result;
				counter = result;
				foundcust = true;	   				
			}
			else
			{
				counter = lastCustSearchValue;
				Console . WriteLine ( $"Starting LASTCUSTSEARCHVALUEat {lastCustSearchValue}..." );
				for ( int x = lastCustSearchValue ; x < custdict . Count - 1 ; x++ )
				{
					if ( currentcustno > custnotofind )
					{
						CustGrid . SelectedIndex = x;
						CustGrid . BringIntoView ( );
						CustGrid . ScrollIntoView ( x );
						Utils . SetGridRowSelectionOn ( CustGrid , x );
						CustGrid . UpdateLayout ( );
						foundbank = true;
						// Save last successful find
						lastCustSearchValue = x;
						counter = result;
						foundcust = true;
						break;
					}
					counter++;
				}
				Console . WriteLine ( $"Ending LASTCUSTSEARCHVALUE reset to {lastCustSearchValue}..." );
			}
			if ( foundcust == false )
			{
				CustGrid . SelectedIndex = 0;
				lastCustSearchValue = 0;
				CustGrid . BringIntoView ( );
				CustGrid . ScrollIntoView ( 0 );
				Utils . SetGridRowSelectionOn ( CustGrid , 0 );
				CustGrid . UpdateLayout ( );
			}
		#endregion Search

		#region SEARCH RESULTS
			// Handle error if no match found and entered value is > any CustNo in the system
			if ( !foundbank && !foundcust )
			{
				if (
					( BankGrid . SelectedIndex > 0 || CustGrid . SelectedIndex > 0 )
					|| custnotofind > banklow && custnotofind > custlow )
				{
					return -1;
				}
				Console . WriteLine ( $"No matching records exist...." );
			}
//			else
//				ErrorLabel . Visibility = Visibility . Collapsed;	     // This is just the TextBlock, NOT the full window

			Console . WriteLine ( $"lastBankSearchValue = {lastBankSearchValue }, lastCustSearchValue = {lastCustSearchValue}" );
			return 1;
		#endregion SEARCH RESULTS


		}
		#endregion Non Async Search ethods
#endif

		#region Constructor and Loaded/Unloaded/Closing()
		public MultiviewListBoxViewers ( )
		{
			InitializeComponent ( );
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Wait;

		}
		private void Window_Loaded ( object sender , RoutedEventArgs e )
		{
			int counter = 0;
			EventControl . BankDataLoaded += EventControl_BankDataLoaded;
			EventControl . CustDataLoaded += EventControl_CustDataLoaded;
			this . Show ( );
			// Get a pointer ot the Search Error label so we can manipulte it in Active Search
			ErrorLabel = this . SearchError;
			Utils . SetupWindowDrag ( this );
			Flags . SqlBankActive = true;
			//Populate Column sorting listbox
			ColumnSelection . Items . Add ( 0 );
			ColumnSelection . Items . Add ( 1 );
			ColumnSelection . Items . Add ( 2 );
			// Sort Columns to default
			foreach ( var item in datagrid . Columns )
			{
				DGBankColumnsCollection [ counter++ ] = item;
			}
			DataGridSupport . SortBankColumns ( datagrid , DGBankColumnsCollection );
			counter = 0;
			foreach ( var item in datagrid2 . Columns )
			{
				DGCustColumnsCollection [ counter++ ] = item;
			}
			DataGridSupport . SortCustomerColumns ( datagrid2 , DGCustColumnsCollection );

			// Load data  from both dbs'
			int min = Convert . ToInt32 ( MinValue . Text );
			int max = Convert . ToInt32 ( MaxValue . Text );
			int tot = Convert . ToInt32 ( MaxRecords . Text );
			DataTable dtBank = new DataTable();
			DbList_LoadBtnPressed ( sender , null );
			DbList_LoadBtnPressed2 ( sender , null );

			uclistboxheight = UCListbox . ActualHeight;
			uclistbox2height = UCListbox2 . ActualHeight;
			dg1Width = datagrid . ActualWidth;

			// get pointers to various controls
			BankGrid = datagrid;
			CustGrid = datagrid2;
			Uclistbox = UCListbox;
			Uclistbox2 = UCListbox2;
			//ColumnSelection . HorizontalAlignment = HorizontalAlignment . Left;
			//			CheckTypes ( );

			// Use Descriptor to get handle of DP so we can monitor changes in  the Search field
			// Uou cna do this  for any Dp to get access to whatever property changes youy want to monitor
			DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(TextBox.TextProperty, typeof(TextBox));
			if ( dpd != null )
			{ dpd . AddValueChanged ( ViewFilterCondition , OnSearchTextChanged ); }
		}

		private void OnSearchTextChanged ( object sender , EventArgs e )
		{
			TextBox tb = sender as TextBox;
			DoSearchAsync ( tb . Text );
			//			MessageBox . Show ( "The value of the Text property of the TextBlock was changed!" );
		}

		private void Window_Closed ( object sender , EventArgs e )
		{
			EventControl . BankDataLoaded -= EventControl_BankDataLoaded;
			EventControl . CustDataLoaded -= EventControl_CustDataLoaded;
			UnloadDataGridColumns ( );
		}
		#endregion Constructor and Loaded()

		#region ReLoad both Db's

		private async void DbList_LoadBtnPressed ( object sender , MouseButtonEventArgs e )
		{
			int min = 0, max = 0, tot = 0;
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Wait;
			datagrid . ItemsSource = null;
			datagrid . Items . Clear ( );
			// clear listbox.
			UCListbox . ItemsSource = null;
			UCListbox . Items . Clear ( );

			SqlBankAccounts . Clear ( );
			min = Convert . ToInt32 ( MinValue . Text );
			max = Convert . ToInt32 ( MaxValue . Text );
			tot = Convert . ToInt32 ( MaxRecords . Text );
			int[] args= { min, max, tot};
			if ( Flags . USECOPYDATA )
			{

				SqlBankAccounts = DapperSupport . GetBankObsCollectionWithDict ( collection: SqlBankAccounts , Dict: out bankdict , wantDictionary: false , DbNameToLoad: "NewBank" , Notify: true , Caller: "MultiLBViewer" , args: args );
				tbBankDb . Text = Flags . COPYBANKDATANAME;
			}
			else
			{
				SqlBankAccounts = DapperSupport . GetBankObsCollectionWithDict ( collection: SqlBankAccounts , Dict: out bankdict , wantDictionary: false , Notify: true , Caller: "MultiLBViewer" , args: args );
				tbBankDb . Text = "BankAccount";
			}
			SqlBankView = CollectionViewSource . GetDefaultView ( SqlBankAccounts ) as CollectionView;
			datagrid . ItemsSource = SqlBankView;
			UCListbox . ItemsSource = SqlBankView;
			BankCount . Text = SqlBankView . Count . ToString ( );
			datagrid . SelectedIndex = 0;             // Create Dictionary  for Bank.CustNo
			datagrid . Refresh ( );
			datagrid . UpdateLayout ( );
			UCListbox . Refresh ( );
			UCListbox . UpdateLayout ( );
			int index = 0;
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
		}

		/// <summary>
		/// Load Customer data using Dapper
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DbList_LoadBtnPressed2 ( object sender , MouseButtonEventArgs e )
		{
			int min = 0, max = 0, tot = 0;
			datagrid2 . ItemsSource = null;
			datagrid2 . Items . Clear ( );
			UCListbox2 . ItemsSource = null;
			UCListbox2 . Items . Clear ( );
			UCListbox2 . UpdateLayout ( );
			datagrid2 . UpdateLayout ( );
			UCListbox2 . UpdateLayout ( );
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Wait;
			SqlCustAccounts?.Clear ( );
			min = Convert . ToInt32 ( MinValue . Text );
			max = Convert . ToInt32 ( MaxValue . Text );
			tot = Convert . ToInt32 ( MaxRecords . Text );
			//			AllCustomers . LoadCust ( SqlCustcollection , "MULTIVIEWLISTBOXVIEWERS" , start: min , end: max , max: tot , NotifyAll: true );
			string SelectString="";
			if ( min > 0 && max > 0 && tot > 0 )
				SelectString = $"Select Top ({tot}) * from Customer where CustNo >= {min} AND CustNo <= {max} order by CustNo, BankNo  ";
			else if ( min == 0 && max == 0 && tot > 0 )
				SelectString = $"Select Top ({tot}) * from Customer order by CustNo, BankNo  ";

			int [] args= {min, max, tot };
			//			Dictionary <int, int> Dict2;

			if ( Flags . USECOPYDATA )
			{
				SqlCustAccounts = DapperSupport . GetCustObsCollectionWithDict ( collection: SqlCustAccounts ,
					Dict: out Dictionary<int , int> custdict ,
					false ,
					DbNameToLoad: "NewCust" ,
					SqlCommand: SelectString ,
					Caller: "MultiLBViewer" ,
					NotifyCaller: true ,
					args: args );
					tbCustDb . Text = Flags . COPYCUSTDATANAME;
			}
			else
			{
				SqlCustAccounts = DapperSupport . GetCustObsCollectionWithDict ( collection: SqlCustAccounts , Dict: out custdict , wantDictionary: false , NotifyCaller: true , Caller: "MultiLBViewer" , args: args );
				tbCustDb . Text = "Customer";
			}
			SqlCustView = CollectionViewSource . GetDefaultView ( SqlCustAccounts ) as CollectionView;
			datagrid2 . ItemsSource = SqlCustView;
			UCListbox2 . ItemsSource = SqlCustView;
			datagrid2 . SelectedIndex = 0;
			datagrid2 . Refresh ( );
			datagrid2 . UpdateLayout ( );
			UCListbox2 . SelectedIndex = 0;
			UCListbox2 . Refresh ( );
			UCListbox2 . UpdateLayout ( );
			CustCount . Text = SqlCustView . Count . ToString ( );
			int index = 0;
			ReloadDictionaries ( "CUSTOMER" );
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
		}
		#endregion ReLoad both Db's

		#region Load Data Post after Trigger
		private void EventControl_BankDataLoaded ( object sender , LoadedEventArgs e )
		{
			bool privateMethod = false;
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Wait;
			Debug . WriteLine ( $"\n*** Loading Bank data in UserListboxViewer after BankDataLoaded trigger\n" );
			SqlBankAccounts = e . DataSource as ObservableCollection<BankAccountViewModel>;
			//Get a View  of the Bank Data so we can sort and filter
			SqlBankView = CollectionViewSource . GetDefaultView ( SqlBankAccounts ) as CollectionView;
			if ( privateMethod )
			{
				// Using my own filter class
				var  filter = new ViewFilter ( SqlBankView);
				// Set the filter to data entry field on the window
				filter . FilterExpression = "CustNo >= int.Parse(ViewFilterCondition.Text) AND CustNo < 1057000";
				//filter .FilterExpression = ViewFilterCondition .Text;
				datagrid . ItemsSource = SqlBankView;
				UCListbox . ItemsSource = SqlBankView;
			}
			else
			{
				// Std method  to filter
				datagrid . ItemsSource = SqlBankView;
				datagrid . SelectedIndex = 0;
				UCListbox . ItemsSource = SqlBankView;
			}
			datagrid . SelectedIndex = 0;
			UCListbox . SelectedIndex = 0;
			datagrid . UpdateLayout ( );
			// Create Dictionary  for Customer.CustNo
			if ( bankdict . Count == 0 )
				ReloadDictionaries ( "BANK" );
			BankCount . Text = SqlBankView . Count . ToString ( );
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
		}

		/// <summary>
		/// Filter condition for BankAccount Daa
		/// </summary>
		/// <param name="bankaccount"></param>
		/// <returns></returns>
		private bool FiterBankData ( int custno )
		{
			return true;
		}

		private void EventControl_CustDataLoaded ( object sender , LoadedEventArgs e )
		{
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Wait;
			SqlCustAccounts = e . DataSource as ObservableCollection<CustomerViewModel>;
			datagrid2 . ItemsSource = SqlCustAccounts;
			SqlCustView = CollectionViewSource . GetDefaultView ( SqlCustAccounts ) as CollectionView;
			datagrid2 . ItemsSource = SqlCustView;
			UCListbox2 . ItemsSource = SqlCustView;
			datagrid2 . SelectedIndex = 0;
			UCListbox2 . SelectedIndex = 0;
			CustCount . Text = SqlCustView . Count . ToString ( );
			Console . WriteLine ( $"Customer data just Loaded : datagrid2 count = { datagrid2 . Items . Count}" );
			datagrid2 . Visibility = Visibility . Visible;
			datagrid2 . BringIntoView ( );
			CustCount . Text = SqlCustView . Count . ToString ( );
			datagrid2 . UpdateLayout ( );
			if ( custdict . Count == 0 )
				ReloadDictionaries ( "CUSTOMER" );
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
		}
		#endregion Load Data Post Trigger

		#region Original DATA LOAD/UNLOAD FUNCTIONS
		//public static BankCollection LoadBankTest ( BankCollection temp , DataTable dtBank )
		//{
		//	//example of using an Action

		//	int x = dtBank.Rows.Count;
		//	//			int i = 0;
		//	Func<int, int, bool> action = (i, x) =>
		//    {
		//     while (i++ < x)
		//     {
		//	     temp.Add(new BankAccountViewModel
		//	     {
		//		     Id = Convert.ToInt32(dtBank.Rows[i][0]),
		//		     BankNo = dtBank.Rows[i][1].ToString(),
		//		     CustNo = dtBank.Rows[i][2].ToString(),
		//		     AcType = Convert.ToInt32(dtBank.Rows[i][3]),
		//		     Balance = Convert.ToDecimal(dtBank.Rows[i][4]),
		//		     IntRate = Convert.ToDecimal(dtBank.Rows[i][5]),
		//		     ODate = Convert.ToDateTime(dtBank.Rows[i][6]),
		//		     CDate = Convert.ToDateTime(dtBank.Rows[i][7]),
		//	     });
		//     }
		//     return true;
		//    };
		//	action ( 0 , x );

		//	return null;
		//}

		/// <summary>
		/// Have to clear clumns collection otherwise it barfs on reloading after closure f the window
		/// </summary>
		private void UnloadDataGridColumns ( )
		{
			ObservableCollection<DataGridColumn> dgc = datagrid.Columns;
			dgc . Clear ( );
			ObservableCollection<DataGridColumn> dgc2 = datagrid2.Columns;
			dgc2 . Clear ( );
		}

		#endregion DATA LOAD FUNCTIONS

		#region Key Handlers
		private void TextBox_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			BankAccountViewModel bvm = new BankAccountViewModel();
			// we are in a TextBlock or TextBox of the ListView
			if ( e . Key == Key . Enter || e . Key == Key . Tab )
			{
				string t = sender.GetType().ToString();
				if ( t . Contains ( "TextBox" ) || t . Contains ( "TextBlock" ) )
				{
					// in a listview !!
					if ( listSelection > -1 )
						bvm = UCListbox . SelectedItem as BankAccountViewModel;
				}
				if ( bvm != null )
					BankCollection . UpdateBankDb ( bvm , "BANKACCOUNT" );

				EventControl . TriggerViewerDataUpdated ( this , new LoadedEventArgs
				{
					CallerType = "USERLISTBOXVIEWER" ,
					CallerDb = "BANKACCOUNT" ,
					DataSource = SqlBankAccounts ,
					RowCount = UCListbox . SelectedIndex ,
					Bankno = bvm . BankNo ,
					Custno = bvm . CustNo
				} );
			}

		}
		#endregion Key Handlers

		#region Record selection handlers for grid and Listbox
		private void UCListbox_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			// Store in a class variable
			CurrentIndex = UCListbox . SelectedIndex;
			ListSelection = CurrentIndex;
			Debug . WriteLine ( $"Index is set to {CurrentIndex}" );
		}
		private void UCListbox_SelectionChanged2 ( object sender , SelectionChangedEventArgs e )
		{
			CurrentIndex = UCListbox2 . SelectedIndex;
			ListSelection2 = CurrentIndex;
			Debug . WriteLine ( $"Index of  Customer ListView is set to {CurrentIndex}" );
		}


		private void datagrid_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			DataGrid dg = new DataGrid();
			dg = e . Source as DataGrid;
			var dgr = dg.Items.CurrentItem;
			GridSelection = dg . SelectedIndex;
			ListSelection = GridSelection;
			var template = datagrid;
			TextBlock tb = (TextBlock)this.datagrid.FindName("custno2");
			Brush newbrush = new SolidColorBrush(Color.FromArgb(255, (byte)255, (byte)255, (byte)255));
		}
		#endregion Record selection handlers for grid and Listbox

		#region Mouse handlers	     	
		private async void ClearBtn_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		{
			DbList_LoadBtnPressed ( sender , null );
			// This loads the Customers Db
			DbList_LoadBtnPressed2 ( sender , null );

		}
		//private void Datagrid_PreviewMouseMove ( object sender , MouseEventArgs e )
		//{
		//	string dataFormat = "";
		//	string str = "";
		//	TextBlock tb = new TextBlock();
		//	tb = sender as TextBlock;

		//	if ( tb != null )
		//	{
		//		CurrentBackColor = tb . Background;
		//		CurrentForeColor = tb . Foreground;
		//		CurrentCellName = tb . Name;
		//	}
		//	if ( MouseLeftBtnDown == true )
		//	{
		//		bool isbank = false;
		//		ObservableCollection<DataGridColumn> dgc = datagrid.Columns;
		//		foreach ( var item in dgc )
		//		{
		//			string a = item.Header as String;

		//			if ( a . Contains ( "Balance" ) )
		//			{
		//				isbank = true;
		//				break;
		//			}
		//		}
		//		if ( isbank )
		//		{
		//			// must be a Bank grid
		//			BankAccountViewModel bvm = new BankAccountViewModel();
		//			string t1 = sender.GetType().ToString();
		//			if ( t1 . Contains ( "ListView" ) )
		//				bvm = UCListbox . SelectedItem as BankAccountViewModel;
		//			else
		//				bvm = datagrid . SelectedItem as BankAccountViewModel;
		//			str = GetExportRecords . CreateTextFromRecord ( bvm , null , null , true , false );
		//			dataFormat = DataFormats . Text;
		//		}
		//		else
		//		{
		//			// must be a Customer grid
		//			CustomerViewModel cvm = new CustomerViewModel ();
		//			string t1 = sender.GetType().ToString();
		//			if ( t1 . Contains ( "ListView" ) )
		//				cvm = UCListbox . SelectedItem as CustomerViewModel;
		//			else
		//				cvm = datagrid . SelectedItem as CustomerViewModel;
		//			str = GetExportRecords . CreateTextFromRecord ( null , null , cvm , true , false );
		//			dataFormat = DataFormats . Text;
		//		}

		//		DataObject dataObject = new DataObject(dataFormat, str);
		//	}
		//	MouseLeftBtnDown = false;
		//}
		//private void datagrid_PreviewMouseMove2 ( object sender , MouseEventArgs e )
		//{
		//	string dataFormat = "";
		//	string str = "";
		//	TextBlock tb = new TextBlock();
		//	tb = sender as TextBlock;

		//	if ( tb != null )
		//	{
		//		CurrentBackColor = tb . Background;
		//		CurrentForeColor = tb . Foreground;
		//		CurrentCellName = tb . Name;
		//	}
		//	if ( MouseLeftBtnDown == true )
		//	{
		//		bool isbank = false;
		//		ObservableCollection<DataGridColumn> dgc = datagrid.Columns;
		//		foreach ( var item in dgc )
		//		{
		//			string a = item.Header as String;

		//			if ( a . Contains ( "Balance" ) )
		//			{
		//				isbank = true;
		//				break;
		//			}
		//		}
		//		if ( isbank )
		//		{
		//			// must be a Bank grid
		//			BankAccountViewModel bvm = new BankAccountViewModel();
		//			string t1 = sender.GetType().ToString();
		//			if ( t1 . Contains ( "ListView" ) )
		//				bvm = UCListbox . SelectedItem as BankAccountViewModel;
		//			else
		//				bvm = datagrid . SelectedItem as BankAccountViewModel;
		//			str = GetExportRecords . CreateTextFromRecord ( bvm , null , null , true , false );
		//			dataFormat = DataFormats . Text;
		//		}
		//		else
		//		{
		//			// must be a Customer grid
		//			CustomerViewModel cvm = new CustomerViewModel ();
		//			string t1 = sender.GetType().ToString();
		//			if ( t1 . Contains ( "ListView" ) )
		//				cvm = UCListbox . SelectedItem as CustomerViewModel;
		//			else
		//				cvm = datagrid . SelectedItem as CustomerViewModel;
		//			str = GetExportRecords . CreateTextFromRecord ( null , null , cvm , true , false );
		//			dataFormat = DataFormats . Text;
		//		}
		//		DataObject dataObject = new DataObject(dataFormat, str);
		//	}
		//	MouseLeftBtnDown = false;
		//}
		private void ListView_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
		{
			ContextMenu cm = this.FindResource("ContextMenu2") as ContextMenu;
			cm . PlacementTarget = sender as ListView;
			cm . IsOpen = true;
		}
		private void _Border_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			//Click inside ListView Item
			Border brdr = sender as Border;
		}
		private void ToggleBtn_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		{
			// switch between Datagrid and ListView
			if ( UCListbox . Visibility == Visibility . Visible )
			{
				//Switch  to DataGrid view
				Utils . SetUpGridSelection ( datagrid , ListSelection );
				// Toggle viz of the grid -v- view
				UCListbox . Visibility = Visibility . Collapsed;
				UCListbox2 . Visibility = Visibility . Collapsed;
				datagrid . Visibility = Visibility . Visible;
				datagrid2 . Visibility = Visibility . Visible;
				datagrid . SelectedIndex = ListSelection;
				datagrid . SelectedItem = GridSelection;
				if ( datagrid . SelectedItem != null )
					datagrid . ScrollIntoView ( datagrid . SelectedItem );
				this . Refresh ( );
				datagrid . Width = dg1Width;
				datagrid . Focus ( );
				datagrid . Refresh ( );
				datagrid . SelectedIndex = GridSelection;
				datagrid . SelectedItem = GridSelection;
				ColumnSelection . Visibility = Visibility . Visible;
			}
			else
			{
				//Switch  to ListView view
				dg1Width = datagrid . ActualWidth;
				datagrid . Visibility = Visibility . Collapsed;
				datagrid2 . Visibility = Visibility . Collapsed;
				UCListbox . Visibility = Visibility . Visible;
				UCListbox2 . Visibility = Visibility . Visible;
				this . Refresh ( );
				UCListbox . SelectedIndex = ListSelection;
				UCListbox . SelectedItem = GridSelection;
				UCListbox . Refresh ( );
				UCListbox2 . Refresh ( );
				if ( UCListbox . SelectedItem != null )
					UCListbox . ScrollIntoView ( UCListbox . SelectedItem );
				UCListbox . Focus ( );
				ColumnSelection . Visibility = Visibility . Hidden;
			}
		}
		private void DbListbox_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		{
			try
			{
				ListSelection = UCListbox . SelectedIndex;
				CurrentIndex = UCListbox . SelectedIndex;
				ListSelection = CurrentIndex;
				UCListbox . SelectedItem = CurrentIndex;
			}
			catch ( Exception ex ) { }
		}
		private void DbListbox_PreviewMouseLeftButtonDown2 ( object sender , MouseButtonEventArgs e )
		{
			try
			{
				ListSelection = UCListbox2 . SelectedIndex;
				CurrentIndex = UCListbox2 . SelectedIndex;
				ListSelection2 = CurrentIndex;
				this . IsSelected = true;
			}
			catch ( Exception ex ) { }
		}
		//private void Datagrid_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		//{
		//	string t = sender.GetType().ToString();
		//	if ( t . Contains ( "DataGrid" ) )
		//	{
		//		GridSelection = datagrid . SelectedIndex;
		//		ListSelection = GridSelection;
		//		UCListbox . SelectedIndex = ListSelection;
		//		MouseLeftBtnDown = true;
		//	}
		//}
		private void LbItem_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		{
			ListSelection = UCListbox . SelectedIndex;
			GridSelection = ListSelection;
			MouseLeftBtnDown = true;
		}
		private void LbItem_PreviewMouseLeftButtonDown2 ( object sender , MouseButtonEventArgs e )
		{
			ListSelection = UCListbox2 . SelectedIndex;
			GridSelection = ListSelection;
			this . IsSelected = true;
		}
		private void UCListbox_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
		{
			ListBoxItem lbi = new ListBoxItem();
			ListBox lv = sender as ListBox;
			int sel = lv.SelectedIndex;
			lbi = ( ListBoxItem ) UCListbox . ItemContainerGenerator . ContainerFromIndex ( UCListbox . SelectedIndex );
			ListSelection = UCListbox . SelectedIndex;
			GridSelection = ListSelection;
		}

		#endregion Mouse handlers

		#region Visual control methods
		private ListView GetParent ( Visual v )
		{
			while ( v != null )
			{
				v = VisualTreeHelper . GetParent ( v ) as Visual;
				if ( v is ListView )
					break;
			}
			return v as ListView;
		}
		public void FindChildren<T> ( List<T> results , DependencyObject startNode )
		  where T : DependencyObject
		{
			int count = VisualTreeHelper.GetChildrenCount(startNode);
			for ( int i = 0 ; i < count ; i++ )
			{
				DependencyObject current = VisualTreeHelper.GetChild(startNode, i);
				if ( ( current . GetType ( ) ) . Equals ( typeof ( T ) ) || ( current . GetType ( ) . GetTypeInfo ( ) . IsSubclassOf ( typeof ( T ) ) ) )
				{
					T asType = (T)current;
					results . Add ( asType );
				}
				FindChildren<T> ( results , current );
			}
		}
		private FrameworkElement FindByName ( string name , FrameworkElement root )
		{
			Stack<FrameworkElement> tree = new Stack<FrameworkElement>();
			tree . Push ( root );

			while ( tree . Count > 0 )
			{
				FrameworkElement current = tree.Pop();
				if ( current . Name == name )
					return current;

				int count = VisualTreeHelper.GetChildrenCount(current);
				for ( int i = 0 ; i < count ; ++i )
				{
					DependencyObject child = VisualTreeHelper.GetChild(current, i);
					if ( child is FrameworkElement )
						tree . Push ( ( FrameworkElement ) child );
				}
			}

			return null;
		}

		#endregion Visual control methods

		#region Column Sorting
		private void Columns_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			ListBox lb = sender as ListBox;
			var  Content = lb . SelectedItem;
			var selection = int.Parse(Content.ToString());
			if ( selection >= 0 && selection <= 2 )
			{
				//					int[] sortorder = { 2,3,1,5,4,7,6,0};
				DataGridSupport . SortBankColumns ( datagrid , DGBankColumnsCollection , selection );
				datagrid . Refresh ( );
				DataGridSupport . SortCustomerColumns ( datagrid2 , DGCustColumnsCollection , selection );
				datagrid2 . Refresh ( );
			}
		}
		#endregion Column Sorting

		#region LINQ methods
		private void Linq1_Click ( object sender , RoutedEventArgs e )
		{
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Wait;
			LinqResults lq = new LinqResults();
			var accounts = from items in SqlBankAccounts
					   where (items.AcType == 1)
					   orderby items.CustNo
					   select items;
			BankCollection vm = new BankCollection();
			foreach ( var item in accounts )
			{
				vm . Add ( item );
			}
			datagrid . ItemsSource = null;
			datagrid . Items . Clear ( );
			datagrid . ItemsSource = vm;
			UCListbox . ItemsSource = null;
			UCListbox . Items . Clear ( );
			UCListbox . ItemsSource = vm;
			BankCount . Text = vm . Count . ToString ( );

			var caccounts = from items in SqlCustAccounts
					    where (items.AcType == 1)
					    orderby items.CustNo
					    select items;
			ObservableCollection<CustomerViewModel>cvm = new ObservableCollection<CustomerViewModel> ( );
			foreach ( var item in caccounts )
			{
				cvm . Add ( item );
			}
			datagrid2 . ItemsSource = null;
			datagrid2 . Items . Clear ( );
			datagrid2 . ItemsSource = cvm;
			UCListbox2 . ItemsSource = null;
			UCListbox2 . Items . Clear ( );
			UCListbox2 . ItemsSource = cvm;
			CustCount . Text = cvm . Count . ToString ( );
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
		}

		private void Linq2_Click ( object sender , RoutedEventArgs e )
		{
			//select items;
			//			BackupBankcollection = SqlBankcollection;
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Wait;
			var accounts = from items in SqlBankAccounts
					   where (items.AcType == 2)
					   orderby items.CustNo
					   select items;
			BankCollection vm = new BankCollection();
			foreach ( var item in accounts )
			{
				vm . Add ( item );
			}
			datagrid . ItemsSource = null;
			datagrid . Items . Clear ( );
			datagrid . ItemsSource = vm;
			UCListbox . ItemsSource = null;
			UCListbox . Items . Clear ( );
			UCListbox . ItemsSource = vm;
			BankCount . Text = vm . Count . ToString ( );

			var caccounts = from items in SqlCustAccounts
					    where (items.AcType == 2)
					    orderby items.CustNo
					    select items;
			ObservableCollection<CustomerViewModel>cvm = new ObservableCollection<CustomerViewModel> ( );
			foreach ( var item in caccounts )
			{
				cvm . Add ( item );
			}
			datagrid2 . ItemsSource = null;
			datagrid2 . Items . Clear ( );
			datagrid2 . ItemsSource = cvm;
			UCListbox2 . ItemsSource = null;
			UCListbox2 . Items . Clear ( );
			UCListbox2 . ItemsSource = cvm;
			CustCount . Text = cvm . Count . ToString ( );
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
		}

		private void Linq3_Click ( object sender , RoutedEventArgs e )
		{
			//select items;
			//			BackupBankcollection = SqlBankcollection;
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Wait;
			var accounts = from items in SqlBankAccounts
					   where (items.AcType == 3)
					   orderby items.CustNo
					   select items;
			BankCollection vm = new BankCollection();
			foreach ( var item in accounts )
			{
				vm . Add ( item );
			}
			datagrid . ItemsSource = null;
			datagrid . Items . Clear ( );
			datagrid . ItemsSource = vm;
			UCListbox . ItemsSource = null;
			UCListbox . Items . Clear ( );
			UCListbox . ItemsSource = vm;
			BankCount . Text = vm . Count . ToString ( );

			var caccounts = from items in SqlCustAccounts
					    where (items.AcType == 3)
					    orderby items.CustNo
					    select items;
			ObservableCollection<CustomerViewModel>cvm = new ObservableCollection<CustomerViewModel> ( );
			foreach ( var item in caccounts )
			{
				cvm . Add ( item );
			}
			datagrid2 . ItemsSource = null;
			datagrid2 . Items . Clear ( );
			datagrid2 . ItemsSource = cvm;
			UCListbox2 . ItemsSource = null;
			UCListbox2 . Items . Clear ( );
			UCListbox2 . ItemsSource = cvm;
			CustCount . Text = cvm . Count . ToString ( );
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
		}
		private void Linq4_Click ( object sender , RoutedEventArgs e )
		{
			//select items;
			//			BackupBankcollection = SqlBankcollection;
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Wait;
			var accounts = from items in SqlBankAccounts
					   where (items.AcType == 4)
					   orderby items.CustNo
					   select items;
			BankCollection vm = new BankCollection();
			foreach ( var item in accounts )
			{
				vm . Add ( item );
			}
			datagrid . ItemsSource = null;
			datagrid . Items . Clear ( );
			datagrid . ItemsSource = vm;
			UCListbox . ItemsSource = null;
			UCListbox . Items . Clear ( );
			UCListbox . ItemsSource = vm;
			BankCount . Text = vm . Count . ToString ( );

			var caccounts = from items in SqlCustAccounts
					    where (items.AcType == 4)
					    orderby items.CustNo
					    select items;
			ObservableCollection<CustomerViewModel>cvm = new ObservableCollection<CustomerViewModel> ( );
			foreach ( var item in caccounts )
			{
				cvm . Add ( item );
			}
			datagrid2 . ItemsSource = null;
			datagrid2 . Items . Clear ( );
			datagrid2 . ItemsSource = cvm;
			UCListbox2 . ItemsSource = null;
			UCListbox2 . Items . Clear ( );
			UCListbox2 . ItemsSource = cvm;
			CustCount . Text = cvm . Count . ToString ( );
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
		}

		/// <summary>
		/// Create a subset that only includes those cust acs with >1 bankaccounts
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Linq5_Click ( object sender , RoutedEventArgs e )
		{
			//select All the items first;
			//			BackupBankcollection = SqlBankcollection;
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Wait;
			var accounts = from items in SqlBankAccounts orderby items.CustNo, items.BankNo select items;
			//Next Group collection on CustNo
			var grouped = accounts.GroupBy(b => b.CustNo);

			//Now filter content down to only those a/c's with multiple Bank A/c's
			var sel = from g in grouped
				    where g.Count() > 1
				    select g;

			// Finally, iterate thru the list of grouped CustNo's matching to CustNo in the full BankAccounts data
			// giving us ONLY the full records for any records that have > 1 Bank accounts
			List<BankAccountViewModel> output = new List<BankAccountViewModel>();

			foreach ( var item1 in sel )
			{
				foreach ( var item2 in accounts )
				{
					if ( item2 . CustNo . ToString ( ) == item1 . Key )
					{
						output . Add ( item2 );
					}
				}
			}
			BankCollection vm = new BankCollection();
			foreach ( var item in output )
			{
				vm . Add ( item );
			}
			datagrid . ItemsSource = null;
			datagrid . Items . Clear ( );
			datagrid . ItemsSource = vm;
			UCListbox . ItemsSource = null;
			UCListbox . Items . Clear ( );
			UCListbox . ItemsSource = vm;
			BankCount . Text = vm . Count . ToString ( );

			var caccounts = from items in SqlCustAccounts orderby items.CustNo, items.BankNo select items;
			//Next Group collection on CustNo
			var cgrouped = accounts.GroupBy(b => b.CustNo);

			//Now filter content down to only those a/c's with multiple Bank A/c's
			var csel = from g in grouped
				     where g.Count() > 1
				     select g;

			// Finally, iterate thru the list of grouped CustNo's matching to CustNo in the full Customer  data
			// giving us ONLY the full records for any records that have > 1 Bank accounts
			List<CustomerViewModel> coutput = new List<CustomerViewModel>();

			foreach ( var item1 in csel )
			{
				foreach ( var citem2 in caccounts )
				{
					if ( citem2 . CustNo . ToString ( ) == item1 . Key )
					{
						coutput . Add ( citem2 );
					}
				}
			}

			ObservableCollection<CustomerViewModel>cvm = new ObservableCollection<CustomerViewModel> ( );
			foreach ( var item in caccounts )
			{
				cvm . Add ( item );
			}
			datagrid2 . ItemsSource = null;
			datagrid2 . Items . Clear ( );
			datagrid2 . ItemsSource = cvm;
			UCListbox2 . ItemsSource = null;
			UCListbox2 . Items . Clear ( );
			UCListbox2 . ItemsSource = cvm;
			CustCount . Text = cvm . Count . ToString ( );
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
		}
		//*************************************************************************************************************//
		// Turn filter OFF
		/// <summary>
		/// Reset our viewer to FULL record display by reloading  the Db from disk - JIC
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Linq6_Click ( object sender , RoutedEventArgs e )
		{
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Wait;
			if ( SqlBankAccounts . Count == 0 )
			{
				SqlBankAccounts = null;
				UCListbox . ItemsSource = null;
				Flags . SqlCustActive = true;
				SqlBankAccounts . Clear ( );
				//				SqlBankAccounts = DapperSupport . GetBankObsCollectionWithDict ( SqlBankAccounts , Dict: out bankdict , wantDictionary: false , Caller: "MultiLBViewer" );
				if ( Flags . USECOPYDATA )
				{

					SqlBankAccounts = DapperSupport . GetBankObsCollectionWithDict ( collection: SqlBankAccounts ,
					Dict: out Dictionary<int , int> bankdict ,
					false ,
					DbNameToLoad: "NewBank" ,
					Caller: "MultiLBViewer" ,
					Notify: true );
					tbBankDb . Text = "BankAccount";

				}
				else
				{
					SqlBankAccounts = DapperSupport . GetBankObsCollectionWithDict ( collection: SqlBankAccounts , Dict: out bankdict , wantDictionary: false , DbNameToLoad: "NewBank" , Notify: true , Caller: "MultiLBViewer" );
					tbBankDb . Text = Flags . COPYBANKDATANAME;
				}
				SqlBankView = CollectionViewSource . GetDefaultView ( SqlBankAccounts ) as CollectionView;
			}

			datagrid . ItemsSource = SqlBankView;
			UCListbox . ItemsSource = SqlBankView;
			BankCount . Text = SqlBankView . Count . ToString ( );
			datagrid . SelectedIndex = 0;
			UCListbox . SelectedIndex = 0;
			UCListbox . UpdateLayout ( );
			BankCount . Text = SqlBankView . Count . ToString ( );

			if ( SqlCustAccounts . Count == 0 )
			{
				SqlCustAccounts = null;
				UCListbox . ItemsSource = null;
				Flags . SqlCustActive = true;
				SqlCustAccounts . Clear ( );

				if ( Flags . USECOPYDATA )
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollectionWithDict ( collection: SqlCustAccounts ,
					Dict: out Dictionary<int , int> custdict ,
					false ,
					DbNameToLoad: "NewCust" ,
					Caller: "MultiLBViewer" ,
					NotifyCaller: true );
					tbCustDb . Text = Flags . COPYCUSTDATANAME;

				}
				else
				{
					SqlCustAccounts = DapperSupport . GetCustObsCollectionWithDict ( SqlCustAccounts , Dict: out custdict , wantDictionary: false , Caller: "MultiLBViewer" );
					tbCustDb . Text = "Customer";
				}
				// doesnt work !!!!  27/10/21
				SqlCustView = CollectionViewSource . GetDefaultView ( SqlCustAccounts ) as CollectionView;
			}

			datagrid2 . ItemsSource = SqlCustAccounts;
			UCListbox2 . ItemsSource = SqlCustAccounts;
			CustCount . Text = SqlCustAccounts . Count . ToString ( );
			datagrid2 . SelectedIndex = 0;
			UCListbox2 . SelectedIndex = 0;
			UCListbox2 . UpdateLayout ( );
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
		}



		#endregion LINQ methods

		private void ViewJsonRecord_Click ( object sender , RoutedEventArgs e )
		{
			//============================================//
			//MENU ITEM 'Read and display JSON File'
			//============================================//
			//causing debugger to break 5/10/21

			string Output = "";
			Mouse . OverrideCursor = Cursors . Wait;
			BankAccountViewModel bvm = this . UCListbox . SelectedItem as BankAccountViewModel;
			Output = JsonSupport . CreateShowJsonText ( true , "BANKACCOUNT" , bvm , "BankAccountViewModel" );
			MessageBox . Show ( Output , "Currently selected record in JSON format" , MessageBoxButton . OK , MessageBoxImage . Information , MessageBoxResult . OK );
		}

		#region UNUSED stuff
		private void CheckTypes ( object obj = null , object ctrl = null )
		{

			//Type [ ] types = { typeof(Example), typeof(NestedClass),
			// typeof(INested), typeof(S) };

			Type[] types = { typeof ( MultiviewListBoxViewers),
				typeof(DataGrid),
				typeof(Button),
				typeof(ListView),
				typeof(ListBox),
				typeof(TextBox),
				typeof(TextBlock)};
			if ( ctrl != null )
			{

			}
			foreach ( var t in types )
			{
				Console . WriteLine ( "Attributes for type {0}:" , t . Name );

				TypeAttributes attr = t.Attributes;

				// To test for visibility attributes, you must use the visibility mask.
				TypeAttributes visibility = attr & TypeAttributes.VisibilityMask;
				switch ( visibility )
				{
					case TypeAttributes . NotPublic:
						Console . WriteLine ( "   ...is not public" );
						break;
					case TypeAttributes . Public:
						Console . WriteLine ( "   ...is public" );
						break;
					case TypeAttributes . NestedPublic:
						Console . WriteLine ( "   ...is nested and public" );
						break;
					case TypeAttributes . NestedPrivate:
						Console . WriteLine ( "   ...is nested and private" );
						break;
					case TypeAttributes . NestedFamANDAssem:
						Console . WriteLine ( "   ...is nested, and inheritable only within the assembly" +
						   "\n         (cannot be declared in C#)" );
						break;
					case TypeAttributes . NestedAssembly:
						Console . WriteLine ( "   ...is nested and internal" );
						break;
					case TypeAttributes . NestedFamily:
						Console . WriteLine ( "   ...is nested and protected" );
						break;
					case TypeAttributes . NestedFamORAssem:
						Console . WriteLine ( "   ...is nested and protected internal" );
						break;
				}

				// Use the layout mask to test for layout attributes.
				TypeAttributes layout = attr & TypeAttributes.LayoutMask;
				switch ( layout )
				{
					case TypeAttributes . AutoLayout:
						Console . WriteLine ( "   ...is AutoLayout" );
						break;
					case TypeAttributes . SequentialLayout:
						Console . WriteLine ( "   ...is SequentialLayout" );
						break;
					case TypeAttributes . ExplicitLayout:
						Console . WriteLine ( "   ...is ExplicitLayout" );
						break;
				}

				// Use the class semantics mask to test for class semantics attributes.
				TypeAttributes classSemantics = attr & TypeAttributes.ClassSemanticsMask;
				switch ( classSemantics )
				{
					case TypeAttributes . Class:
						if ( t . IsValueType )
						{
							Console . WriteLine ( "   ...is a value type" );
						}
						else
						{
							Console . WriteLine ( "   ...is a class" );
						}
						break;
					case TypeAttributes . Interface:
						Console . WriteLine ( "   ...is an interface" );
						break;
				}

				if ( ( attr & TypeAttributes . Abstract ) != 0 )
				{
					Console . WriteLine ( "   ...is abstract" );
				}

				if ( ( attr & TypeAttributes . Sealed ) != 0 )
				{
					Console . WriteLine ( "   ...is sealed" );
				}

				Console . WriteLine ( );
			}
		}
		#endregion UNUSED stuff

		#region DRAG DROP CODE
		private void datagrid_PreviewDragEnter ( object sender , DragEventArgs e )
		{
			e . Effects = ( DragDropEffects ) DragDropEffects . Move;
		}
		private void datagrid_PreviewMouseLeftButtonup ( object sender , MouseButtonEventArgs e )
		{
			ScrollBarMouseMove = false;
			this . IsSelected = false;
		}
		private void datagrid_PreviewMouseLeftButtondown ( object sender , MouseButtonEventArgs e )
		{
			// Gotta make sure it is not anywhere in the Scrollbar we clicked on 
			if ( Utils . HitTestScrollBar ( sender , e ) )
			{
				ScrollBarMouseMove = true;
				return;
			}
			if ( Utils . HitTestHeaderBar ( sender , e ) )
				return;

			_startPoint = e . GetPosition ( null );
			// Make sure the left mouse button is pressed down so we are really moving a record
			if ( e . LeftButton == MouseButtonState . Pressed )
			{
				IsLeftButtonDown = true;
			}
			string t = sender.GetType().ToString();
			object b = e.OriginalSource;
			if ( t . Contains ( "DataGrid" ) )// && b.Name == "DGR_Border")
			{
				DataGrid dg = sender as DataGrid;
				if ( dg . Name == "datagrid" )
				{
					GridSelection = datagrid . SelectedIndex;
					ListSelection = GridSelection;
					UCListbox . SelectedIndex = ListSelection;
				}
				else
				{
					GridSelection2 = datagrid2 . SelectedIndex;
					ListSelection2 = GridSelection2;
					UCListbox2 . SelectedIndex = ListSelection;
				}
			}
			else
			{
				ListSelection = UCListbox . SelectedIndex;
				GridSelection = ListSelection;
				datagrid . SelectedIndex = ListSelection;
			}

		}
		private void datagrid_PreviewMouseLeftButtondown2 ( object sender , MouseButtonEventArgs e )
		{
			// Gotta make sure it is not anywhere in the Scrollbar we clicked on 
			if ( Utils . HitTestScrollBar ( sender , e ) )
			{
				ScrollBarMouseMove = true;
				return;
			}
			if ( Utils . HitTestHeaderBar ( sender , e ) )
				return;

			_startPoint = e . GetPosition ( null );
			// Make sure the left mouse button is pressed down so we are really moving a record
			if ( e . LeftButton == MouseButtonState . Pressed )
			{
				IsLeftButtonDown = true;
			}
			string t = sender.GetType().ToString();
			object b = e.OriginalSource;
			if ( t . Contains ( "DataGrid" ) )
			{
				DataGrid dg = sender as DataGrid;
				if ( dg . Name == "datagrid" )
				{
					GridSelection = datagrid . SelectedIndex;
					ListSelection = GridSelection;
					UCListbox . SelectedIndex = ListSelection;
				}
				else
				{
					GridSelection2 = datagrid2 . SelectedIndex;
					ListSelection2 = GridSelection2;
					UCListbox2 . SelectedIndex = ListSelection2;
				}
			}
			else
			{
				ListSelection2 = UCListbox2 . SelectedIndex;
				GridSelection2 = ListSelection2;
				datagrid2 . SelectedIndex = ListSelection2;
			}
		}
		private void datagrid_PreviewMouseMove ( object sender , MouseEventArgs e )
		{
			bool IsCust = false;
			Point mousePos = e.GetPosition(null);
			Vector diff = _startPoint - mousePos;
			string t1 = sender.GetType().ToString();
			if ( e . LeftButton == MouseButtonState . Pressed &&
			    Math . Abs ( diff . X ) > SystemParameters . MinimumHorizontalDragDistance ||
			    Math . Abs ( diff . Y ) > SystemParameters . MinimumVerticalDragDistance )
			{
				if ( IsLeftButtonDown && e . LeftButton == MouseButtonState . Pressed )
				{
					bool isvalid = false;
					if ( t1 . Contains ( "ListView" ) )
					{
						isvalid = true;
						ListView dg = sender as ListView ;
						if ( dg . Name == "UCListbox2" )
							IsCust = true;
					}
					else if ( t1 . Contains ( "DataGrid" ) )
					{
						isvalid = true;
						DataGrid dg = sender as DataGrid;
						if ( dg . Name == "datagrid2" )
							IsCust = true;
					}
					if ( isvalid )
					{
						if ( ScrollBarMouseMove )
							return;
						// We are dragging from the DETAILS grid
						//Working string version
						if ( IsCust == false )
						{
							BankAccountViewModel bvm = new BankAccountViewModel();
							if ( t1 . Contains ( "ListView" ) )
								bvm = UCListbox . SelectedItem as BankAccountViewModel;
							else
								bvm = datagrid . SelectedItem as BankAccountViewModel;
							string str = GetExportRecords.CreateTextFromRecord(bvm, null, null, true, false);
							string dataFormat = DataFormats.Text;
							DataObject dataObject = new DataObject(dataFormat, str);
							DragDrop . DoDragDrop (
							datagrid ,
							dataObject ,
							DragDropEffects . Copy );
						}
						else
						{
							CustomerViewModel cvm = new CustomerViewModel();
							if ( t1 . Contains ( "ListView" ) )
								cvm = UCListbox2 . SelectedItem as CustomerViewModel;
							else
								cvm = datagrid2 . SelectedItem as CustomerViewModel;
							string str = GetExportRecords.CreateTextFromRecord(null, null, cvm, true, false);
							string dataFormat = DataFormats.Text;
							DataObject dataObject = new DataObject(dataFormat, str);
							DragDrop . DoDragDrop (
							datagrid ,
							dataObject ,
							DragDropEffects . Copy );
						}
						IsLeftButtonDown = false;
					}
				}
			}
		}
		private void DbList_ShowDropGrid ( object sender , MouseButtonEventArgs e )
		{
			DropDataGridData ddg = new DropDataGridData();
			ddg . Show ( );
		}
		private void CloseBtn_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		{
			this . Close ( );
		}
		private void LbItem_PreviewMouseLeftButtonUp2 ( object sender , MouseButtonEventArgs e )
		{
			this . IsSelected = false;
		}
		private void LbItem_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			this . IsSelected = false;
		}
		private void DbListbox_PreviewMouseLeftButtonUp2 ( object sender , MouseButtonEventArgs e )
		{
			this . IsSelected = false;
		}
		private void DbListbox_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			this . IsSelected = false;
		}

		#endregion DRAGDROP

		private void ResizeUp ( object sender , MouseButtonEventArgs e )
		{
			datagrid . RowHeight += 1;
			datagrid2 . RowHeight += 1;

		}

		private void ResizeDn ( object sender , MouseButtonEventArgs e )
		{
			datagrid . RowHeight -= 1;
			datagrid2 . RowHeight -= 1;
		}

		private void datagrid_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
		{
			ContextMenu cm = this.FindResource("ContextMenu2") as ContextMenu;
			cm . PlacementTarget = sender as ListView;
			cm . IsOpen = true;

		}

		/// <summary>
		/// Creates a Dictionaryon CustNo + Datagrid index from the current Db that we have already loaded
		/// </summary>
		/// <param name="DictType"></param>
		private void ReloadDictionaries ( string DictType )
		{
			Mouse . OverrideCursor = System . Windows . Input . Cursors . Arrow;
			int index = 0;
			if ( DictType == "BANK" )
			{
				bankdict . Clear ( );
				BankAccountViewModel bm = new BankAccountViewModel();
				foreach ( var item in SqlBankAccounts )
				{
					bm = item as BankAccountViewModel;
					try
					{
						bankdict . Add ( int . Parse ( bm . CustNo ) , index++ );
					}
					catch ( Exception ex )
					{
						//						Console . WriteLine ( $"Failed to create BankAccount Dictionary : {ex . Message}, {ex . Data}" );
					}
				}
			}
			else if ( DictType == "CUSTOMER" )
			{
				custdict . Clear ( );
				index = 0;
				CustomerViewModel cm = new CustomerViewModel ();
				foreach ( var item in SqlCustView )
				{
					cm = item as CustomerViewModel;
					if ( cm == null )
						break;
					try
					{
						custdict . Add ( int . Parse ( cm . CustNo ) , index++ );
					}
					catch ( Exception ex )
					{
						//						Console . WriteLine ( $"Failed to create Customer Dictionary for {cm . CustNo} : {ex . Message}, {ex . Data}" );
					}
				}
			}
			else if ( DictType == "DETAILS" )
			{
				detdict . Clear ( );
				index = 0;
				DetailsViewModel cm = new DetailsViewModel ();
				foreach ( var item in SqlDetView )
				{
					cm = item as DetailsViewModel;
					if ( cm == null )
						break;
					try
					{
						detdict . Add ( int . Parse ( cm . CustNo ) , index++ );
					}
					catch ( Exception ex )
					{
						//						Console . WriteLine ( $"Failed to create Detai;ls Dictionary for {cm . CustNo} : {ex . Message}, {ex . Data}" );
					}
				}
			}
		}

		private void CopyDb_Click ( object sender , RoutedEventArgs e )
		{
			// Copy any Db to a new copy Db
			var result = DapperSupport . CreateDbCopy ( "BankAccount" , "NewBank" ) ;
			if ( result )
				MessageBox . Show ( $"NewBank.dbo has been created and populated successfully from BankAccount.dbo" );
			else
				MessageBox . Show ( $"NewBank.dbo already exists, so the requsted copy could not continue...." );
		}

		private void UpdateBank_Click ( object sender , RoutedEventArgs e )
		{
			if ( Flags . USECOPYDATA )
			{

				if ( DapperSupport . UpdateBankDb ( datagrid , "newbank" ) )
				{
					MessageBox . Show ( $"[NewBank] Db has been updated successfully" );
				}
			}
			else
			{
				if ( DapperSupport . UpdateBankDb ( datagrid ) )
				{
					MessageBox . Show ( $"Selected Db has been updated successfully" );
				}
			}
		}
		private void UpdateCustomer_Click ( object sender , RoutedEventArgs e )
		{
			if ( Flags . USECOPYDATA )
			{
				if ( DapperSupport . UpdateCustomersDb ( datagrid2 , "newcust" ) )
				{
					MessageBox . Show ( $"[NewCust] Db has been updated successfully" );
				}
			}
			else
			{
				if ( DapperSupport . UpdateCustomersDb ( datagrid2 ) )
				{
					MessageBox . Show ( $"Selected Db has been updated successfully" );
				}
			}
		}

	}
}
