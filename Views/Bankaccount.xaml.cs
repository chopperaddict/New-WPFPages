using System;
using System . ComponentModel;
using System . Diagnostics;
using System . Linq;
using System . Runtime . Remoting . Messaging;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Input;
using System . Windows . Media;

using WPFPages . ViewModels;
namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for Bankaccount.xaml
	/// </summary>
	public partial class Bankaccount : Window
	{
		private  BankCollection BankViewcollection = null;
		private BankAccountViewModel bankvm = new BankAccountViewModel();
		private  int TemplateType = 0;
		static int counter = 0;
		private int ActypeFiltervalue = 4;
		private  decimal BalanceFiltervalue = (decimal)23000;
		private  decimal IntRateFiltervalue= (decimal)1.35;
		//Testing Actions
		private Action<object, string, int> Myaction;
		Action<string, int> Myremotelambda;

		Func<string, int, bool> MyFunc;



		// Get our personal Collection view of the Db
		Stopwatch timer = new Stopwatch();
		private ICollectionView BankviewerView
		{
			get; set;
		}
		public Bankaccount ( )
		{
			InitializeComponent ( );
			this . DataContext = BankviewerView;
			// This STOPS all those infuriating debug messages from appearing
			// Add it to any window you do not want these messages to show in
			System . Diagnostics . PresentationTraceSources . DataBindingSource . Switch . Level = System . Diagnostics . SourceLevels . Critical;
			EventControl . ViewSharingChanged += EventControl_ViewSharingChanged;
			EventControl . BankDataLoaded += EventControl_BankDataLoaded;
			//BankviewerView . Filter += new FilterEventHandler ( filtermethod );
			ActionTesting ( );
			FuncTesting ( );
			changefiltertype . Items . Add ( "ACTYPE" );
			changefiltertype . Items . Add ( "INTEREST" );
			changefiltertype . Items . Add ( "BALANCE" );
		}
		private async void bankgrid_Loaded ( object sender , RoutedEventArgs e )
		{
			bool isloaded = false;
			timer . Start ( );
			Utils . SetupWindowDrag ( this );
			ToggleViewStatus . IsChecked = Flags . UseSharedView;

			if ( Flags . UseSharedView )
			{
				if ( BankAccountViewModel . BankCollectionView != null )
				{
					if ( BankAccountViewModel . BankCollectionView . IsEmpty == false )
					{
						//Use  the available App wide View ,so we cannot add/remove Sort Descriptions/Filters etc
						BankviewerView = BankAccountViewModel . BankCollectionView;
						this . BankGrid . ItemsSource = BankviewerView;
						this . ListGrid . ItemsSource = BankviewerView;
						this . Listview . ItemsSource = BankviewerView;
						isloaded = true;
					}
					else
						Utils . LoadBankDbGeneric ( bvm: BankViewcollection , Notify: true );
					//await BankCollection . LoadBank ( BankViewcollection , "BankAccount" , 99 , true );
				}
				else
					Utils . LoadBankDbGeneric ( bvm: BankViewcollection , Notify: true );
//				await BankCollection . LoadBank ( BankViewcollection , "BankAccount" , 99 , true );
			}
			else
			{
				Utils . LoadBankDbGeneric ( bvm: BankViewcollection , Notify: true );
//				await BankCollection . LoadBank ( BankViewcollection , "BankAccount" , 99 , true );
			}
		}

		private void Button_Click ( object sender , RoutedEventArgs e )
		{
			// Change the listbox DataTemplate in use
			var v = ListGrid .ItemTemplate;
			// get info on current DT
			DataTemplate dt = ListGrid .ItemTemplate;
			dt = FindResource ( "BankDataTemplate1" ) as DataTemplate;
			if ( TemplateType == 0 )
			{
				ListGrid . ItemTemplate = FindResource ( "BankDataTemplate1" ) as DataTemplate;
				Listview . ItemTemplate = FindResource ( "BankDataTemplate1" ) as DataTemplate;
				TemplateType = 1;
			}
			else
			{
				ListGrid . ItemTemplate = FindResource ( "BankDataTemplate2" ) as DataTemplate;
				Listview . ItemTemplate = FindResource ( "BankDataTemplate2" ) as DataTemplate;
				TemplateType = 0;
			}
		}

		private void EventControl_BankDataLoaded ( object sender , LoadedEventArgs e )
		{
			// Event handler for BankDataLoaded
			if ( e . DataSource == null )
				return;
			// ONLY proceeed if we triggered the new data request
			if ( e . CallerDb != "BankAccount" )
				return;

			this . BankGrid . ItemsSource = null;
			this . BankGrid . Items . Clear ( );
			this . ListGrid . ItemsSource = null;
			this . ListGrid . Items . Clear ( );
			this . Listview . ItemsSource = null;
			this . Listview . Items . Clear ( );
			BankViewcollection = e . DataSource as BankCollection;
			//Add this View to our Views Collection - WORKS !!!
			BankAccountViewModel . BankCollectionView = BankviewerView;
			//Assign our view to the recently loaded data
			BankviewerView = CollectionViewSource . GetDefaultView ( BankViewcollection );

			this . BankGrid . ItemsSource = BankviewerView;
			this . ListGrid . ItemsSource = BankviewerView;
			this . Listview . ItemsSource = BankviewerView;

			// Add our sort's
			if ( Flags . UseSharedView == false )
			{
				// Set our grids items source
				//Not using shared Views, so see if 
				if ( Flags . UseSharedView && BankviewerView . SortDescriptions . Count == 0 )
				{
					BankviewerView . SortDescriptions . Add ( new SortDescription ( "AcType" , ListSortDirection . Ascending ) );
					BankviewerView . SortDescriptions . Add ( new SortDescription ( "IntRate" , ListSortDirection . Descending ) );
					BankviewerView . Refresh ( );
				}
				else if ( Flags . UseSharedView == false )
				{
					BankviewerView . SortDescriptions . Add ( new SortDescription ( "AcType" , ListSortDirection . Ascending ) );
					BankviewerView . SortDescriptions . Add ( new SortDescription ( "IntRate" , ListSortDirection . Descending ) );
				}
			}
			BankviewerView . Filter = new Predicate<object> ( Actypefiltermethod );
			// Setup the grid's
			this . BankGrid . SelectedIndex = 0;
			this . BankGrid . SelectedItem = 0;
			this . BankGrid . UpdateLayout ( );
			this . BankGrid . Refresh ( );
			Mouse . OverrideCursor = Cursors . Arrow;
			timer . Stop ( );
			Console . WriteLine ( $"BANKACCOUNT: Bank Data fully loaded from Sql in {timer . ElapsedMilliseconds} milliseconds\n" );

		}

		#region CollectionView handlers

		public delegate bool filterconditions ( BankAccountViewModel bvm );

		private async void EventControl_ViewSharingChanged ( object sender , NotifyAllViewersOfViewSharingStatus e )
		{
			if ( timer . IsRunning == false )
			{
				timer . Start ( );
				counter++;
			}
			else if ( counter > 1 )
			{
				timer . Stop ( );
				counter = 0;
				return;
			}
			if ( e . Sender != "BankAccount" )
			{
				ToggleViewStatus . IsChecked = e . IsShared;
				BankGrid . ItemsSource = null;
				//Force it to reload using the correct SortOrder
				Console . WriteLine ( $"BANKACCOUNT: EventControl ViewSharing loading Db via SQL, counter = {counter}" );
				await BankCollection . LoadBank ( BankViewcollection , "BankAccount" , 99 , true );
			}
		}
		private async void ToggleViewStatus_Checked ( object sender , RoutedEventArgs e )
		{
			Console . WriteLine ( $"BANKACCOUNT : View Toggle CHECKED called" );
			EventControl . TriggerViewSharingChanged ( sender ,
				  new NotifyAllViewersOfViewSharingStatus
				  {
					  IsShared = true ,
					  Sender = "BankAccount"
				  } );
			BankGrid . ItemsSource = null;
			ListGrid . ItemsSource = null;
			await BankCollection . LoadBank ( BankViewcollection , "BankAccount" , 99 , true );
			Flags . UseSharedView = true;
		}

		private async void ToggleViewStatus_Unchecked ( object sender , RoutedEventArgs e )
		{
			Console . WriteLine ( $"BANKACCOUNT : View Toggle UNCHECKED called" );
			EventControl . TriggerViewSharingChanged ( sender ,
				  new NotifyAllViewersOfViewSharingStatus
				  {
					  IsShared = false ,
					  Sender = "BankAccount"
				  } );
			BankGrid . ItemsSource = null;
			ListGrid . ItemsSource = null;
			await BankCollection . LoadBank ( BankViewcollection , "BankAccount" , 99 , true );
			Flags . UseSharedView = false;
		}

		#endregion CollectionView handlers

		private async void Reload_Click ( object sender , RoutedEventArgs e )
		{
			BankGrid . ItemsSource = null;
			ListGrid . ItemsSource = null;
			Listview . ItemsSource = null;
			await BankCollection . LoadBank ( BankViewcollection , "BankAccount" , 99 , true );
		}
		private void Close_Click ( object sender , RoutedEventArgs e )
		{
			this . Close ( );
		}

		private void BankGrid_Selected ( object sender , RoutedEventArgs e )
		{
			BankAccountViewModel bvm = BankviewerView .CurrentItem as BankAccountViewModel;
			BankviewerView . Refresh ( );
			BankGrid . Refresh ( );
			ListGrid . Refresh ( );
		}
		#region Action testing
		private void ActionTesting ( )
		{
			// Testing Action<T> "Myaction" declared at top  of file: takes x arguments, but MUST NOT return anything
			Myaction = output;
			string s ="Ian is good";
			int i = 123417;
			Action<string, int> Mylambda  = (s, i) => Console . WriteLine ($"{s} {i}");
			Action<string> Mystring  = s => Console . WriteLine ($"{s}");
			// 3 different ways  to use this Action
			// Call an external method
			Myremotelambda = lambdaoutput;
			//Call the "delegated" method directly as an external method
			lambdaoutput ( "sss" , 5 );
			// handle as a direct lambda
			Myremotelambda = ( s , i ) =>
			{

				Console . WriteLine ( "ssssad" + s + i );
			};
			// Call as a method
			Myremotelambda ( "sss" , 5 );

			//Setup TWO Actions calling existing (delegate) methods
			Action<string, int> action = new Action<string, int>(remotelambda);
			Action<string, int> action2 = new Action<string, int>(lambdaoutput);
			action ( "here we go" , 56720 );
			action2 ( "there we were" , 836720 );
		}
		//As a Lambda expression

		private void lambdaoutput ( string s , int i )
		{
			if ( s . Contains ( i . ToString ( ) ) )
				Console . WriteLine ( i );
			else
				Console . WriteLine ( s );
		}

		private void remotelambda ( string s , int i )
		{ Console . WriteLine ( $"{s} {i}" ); }

		/// Test of an Action with THREE parameters		
		private void output ( object obj , string str , int i )
		{//do nothing - testing only}	
		}
		#endregion Action testing}

		#region Func Testing
		private bool FuncTesting ( )
		{
			bool result = false;
			//MyFunc is declared at top level as <string, int, bool> so returns a bool value
			//Assign Func to a remote method
			MyFunc = contains;
			// Call that assigned method
			result = contains ( "dfsgsfhdh5df3hf2dh" , 5 );
			Console . WriteLine ( $"result1 = {result}" );
			// use it directly
			string str = "the 23567 352 string";
			int i = 234;
			MyFunc = ( str , i ) => str . Contains ( i . ToString ( ) );
			bool result2 = MyFunc ( "456gdfds" , 4 );
			Console . WriteLine ( $"result2 = {result2}" );
			return result;
		}

		private bool contains ( string s , int i )
		{
			return s . Contains ( i . ToString ( ) );

		}
		#endregion Func Testing

		#region Filtering

		#region Filterig Button methods
		private void ResetFilter_Click ( object sender , RoutedEventArgs e )
		{
			if ( ( string ) AcTypeBtn . Content == "Go !!" )
			{
				AcTypeBtn_Cancel . Visibility = Visibility . Collapsed;
				FilterBtn . IsEnabled = true;
				AcTypeBtn . Content = "Set AcType";
				AcTypeBtn . Background = FindResource ( "HeaderBrushGreen" ) as Brush;
				AcTypeBtn . Foreground = FindResource ( "Red4" ) as Brush;
				actypeStackpanel . Visibility = Visibility . Collapsed;
				balanceStackpanel . Visibility = Visibility . Collapsed;
				intrateStackpanel . Visibility = Visibility . Collapsed;
				BankviewerView . Filter = new Predicate<object> ( Actypefiltermethod );
				ListGrid . Refresh ( );
				BankGrid . Refresh ( );
				Listview . Refresh ( );
			}
			else
			{
				FilterBtn . IsEnabled = false;
				AcTypeBtn . Content = "Go !!";
				AcTypeBtn . Background = FindResource ( "Red5" ) as Brush;
				AcTypeBtn . Foreground = FindResource ( "White0" ) as Brush;
				if ( ( string ) changefiltertype . SelectedItem == "ACTYPE" )
				{
					actypeStackpanel . Visibility = Visibility . Visible;
					AcTypeBtn_Cancel . Visibility = Visibility . Visible;
				}
				else if ( ( string ) changefiltertype . SelectedItem == "INTRATE" )
				{
					intrateStackpanel . Visibility = Visibility . Visible;
					AcTypeBtn_Cancel . Visibility = Visibility . Visible;
				}
				else if ( ( string ) changefiltertype . SelectedItem == "BALANCE" )
				{
					balanceStackpanel . Visibility = Visibility . Visible;
					AcTypeBtn_Cancel . Visibility = Visibility . Visible;
				}
			}
		}
		private void ResetFilterType_Click ( object sender , RoutedEventArgs e )
		{
			// Change tye of filter in use
			string arg = changefiltertype.Text;
			FilterBtn . IsEnabled = false;
			BankviewerView . SortDescriptions . Clear ( );

			if ( arg == "ACTYPE" )
				BankviewerView . Filter = new Predicate<object> ( Actypefiltermethod );
			else if ( arg == "INTEREST" )
				BankviewerView . Filter = new Predicate<object> ( Intratefiltermethod );
			else if ( arg == "BALANCE" )
				BankviewerView . Filter = new Predicate<object> ( Balancefiltermethod );

			BankviewerView . SortDescriptions . Add ( new SortDescription ( "AcType" , ListSortDirection . Ascending ) );
			BankviewerView . SortDescriptions . Add ( new SortDescription ( "IntRate" , ListSortDirection . Descending ) );
			BankviewerView . Refresh ( );

			ListGrid . Refresh ( );
			BankGrid . Refresh ( );
			Listview . Refresh ( );
		}
		#endregion Filterig Button methods

		#region Filter Predicates
		private bool Actypefiltermethod ( object obj )
		{
			BankAccountViewModel bvm = obj as BankAccountViewModel;
			Console . WriteLine ( $"Testing  for {bvm . AcType} == {ActypeFiltervalue} as AcType" );
			return bvm . AcType >= Convert . ToInt32 ( filtertypelow . Text ) && bvm . AcType <= Convert . ToInt32 ( filtertypehigh . Text );
		}
		private bool Intratefiltermethod ( object obj )
		{
			BankAccountViewModel bvm = obj as BankAccountViewModel;
			Console . WriteLine ( $"Testing  for {bvm . IntRate} == {IntRateFiltervalue} as AcType" );
			return bvm . IntRate >= IntRateFiltervalue;
		}
		private bool Balancefiltermethod ( object obj )
		{
			BankAccountViewModel bvm = obj as BankAccountViewModel;
			Console . WriteLine ( $"Testing  for {bvm . IntRate} == {BalanceFiltervalue} as AcType" );
			return bvm . Balance >= BalanceFiltervalue;
		}
		#endregion Filter Predicates

		#endregion Filtering

		private void changefiltertype_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			string sel = changefiltertype . SelectedItem . ToString ( );
			FilterBtn . IsEnabled = true;
		}

		private void CancelFilter_Click ( object sender , RoutedEventArgs e )
		{
			FilterBtn . IsEnabled = true;
			AcTypeBtn . Content = "Set AcType";
			AcTypeBtn . Background = FindResource ( "HeaderBrushGreen" ) as Brush;
			AcTypeBtn . Foreground = FindResource ( "Red4" ) as Brush;
			actypeStackpanel . Visibility = Visibility . Collapsed;
			AcTypeBtn_Cancel . Visibility = Visibility . Collapsed;

		}

		private void LbDragStart ( object sender , DragEventArgs e )
		{
			// ListBox Drag operation started
			DataObject dobj = e . Data as DataObject;

		}

		private void ListBoxItem_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
		{
			// ListBox Drag operation started
			TextBlock dobj = e .OriginalSource  as TextBlock;
			if ( dobj == null )
				return;
			bankvm = dobj . DataContext as BankAccountViewModel;
			// we  now have the current item as a data record 
			// To test this, lets add it back into the same listbox !!
			// THIS WORKS WELL
			// 1st, we have to null out the view to avoid an Error
			BankviewerView = null;
			//create a new ListBoxItem
			ListBoxItem lbi = new ListBoxItem();
			// fill it with the data from the item we have mousedown over
			lbi . Content = bankvm;
			// Add it to our data collection
			BankViewcollection . Add ( lbi . Content as BankAccountViewModel );
			//Assign our view back, but to the new data with added record
			BankviewerView = CollectionViewSource . GetDefaultView ( BankViewcollection );
			//Update listbox on screen			
			ListGrid . Refresh ( );
			BankGrid . Refresh ( );
			Listview . Refresh ( );
			e . Handled = true;
		}

		/// <summary>
		/// EXAMPLE CODE ONLY
		/// How to access current record's data (from ListBox in this case)  in c#
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListBox_OnPreviewMouseUp ( object sender , MouseButtonEventArgs e )
		{
			var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
			if ( item != null )
			{
				// ListBox item clicked - do some cool things here
				// ListBox Drag operation started
				TextBlock dobj = e .OriginalSource  as TextBlock;
				if ( dobj == null )
					return;
				bankvm = dobj . DataContext as BankAccountViewModel;
				// we  now have the current item as a data record 
				// To test this, lets add it back into the same listbox !!
				// THIS WORKS WELL
				// 1st, we have to null out the view to avoid an Error
				BankviewerView = null;
				//create a new ListBoxItem
				ListBoxItem lbi = new ListBoxItem();
				// fill it with the data from the item we have mousedown over
				lbi . Content = bankvm;
				// Add it to our data collection
				//				BankViewcollection . Add ( lbi . Content as BankAccountViewModel );
				BankViewcollection . Add ( lbi . Content as BankAccountViewModel );
				//Assign our view back, but to the new data with added record
				BankviewerView = CollectionViewSource . GetDefaultView ( BankViewcollection );
				//Update listbox on screen			
				ListGrid . Refresh ( );
				BankGrid . Refresh ( );
				Listview . Refresh ( );
				e . Handled = true;
			}
		}
		private void ListView_OnPreviewMouseUp ( object sender , MouseButtonEventArgs e )
		{
			TextBlock dobj = e .OriginalSource  as TextBlock;
			if ( dobj == null )
				return;
			bankvm = dobj . DataContext as BankAccountViewModel;
			// we  now have the current item as a data record 
			BankviewerView = null;
			ListViewItem lbi = new ListViewItem ();
			lbi . Content = bankvm;
			BankViewcollection . Add ( lbi . Content as BankAccountViewModel );
			BankviewerView = CollectionViewSource . GetDefaultView ( BankViewcollection );
			//Update listbox on screen			
			BankGrid . Refresh ( );
			ListGrid . Refresh ( );
			Listview . Refresh ( );
			e . Handled = true;
		}
		private void DataGrid_OnPreviewMouseUp ( object sender , MouseButtonEventArgs e )
		{
			TextBlock dobj = e .OriginalSource  as TextBlock;
			if ( dobj == null )
				return;
			bankvm = dobj . DataContext as BankAccountViewModel;
			// we  now have the current item as a data record 
			BankviewerView = null;
			ListBoxItem lbi = new ListBoxItem();
			lbi . Content = bankvm;
			BankViewcollection . Add ( lbi . Content as BankAccountViewModel );
			BankviewerView = CollectionViewSource . GetDefaultView ( BankViewcollection );
			//Update listbox on screen			
			BankGrid . Refresh ( );
			ListGrid . Refresh ( );
			Listview . Refresh ( );
			e . Handled = true;
		}

		private void ListBox_OnPreviewDragEnter ( object sender , DragEventArgs e )
		{
			TextBlock dobj = e .OriginalSource  as TextBlock;
			//			if ( dobj == null )
			return;
			bankvm = dobj . DataContext as BankAccountViewModel;
			// we  now have the current item as a data record 
			// To test this, lets add it back into the same listbox !!
			// THIS WORKS WELL
			// 1st, we have to null out the view to avoid an Error
			BankviewerView = null;
			//create a new ListBoxItem
			ListBoxItem lbi = new ListBoxItem();
			// fill it with the data from the item we have mousedown over
			lbi . Content = bankvm;
			// Add it to our data collection
			//				BankViewcollection . Add ( lbi . Content as BankAccountViewModel );
			BankViewcollection . Add ( lbi . Content as BankAccountViewModel );
			//Assign our view back, but to the new data with added record
			BankviewerView = CollectionViewSource . GetDefaultView ( BankViewcollection );
			//Update listbox on screen			
			ListGrid . Refresh ( );
			Listview . Refresh ( );
			e . Handled = true;
		}
		#region Drag Activity

		#endregion Drag Activity

		private void ListView_OnPreviewDragEnter ( object sender , DragEventArgs e )
		{

		}

		private void Toggle_Click ( object sender , RoutedEventArgs e )
		{
			if ( ListGrid . Visibility == Visibility . Visible )
			{
				ListGrid . Visibility = Visibility . Collapsed;
				Listview . Visibility = Visibility . Visible;
			}
			else
			{
				ListGrid . Visibility = Visibility . Visible;
				Listview . Visibility = Visibility . Collapsed;
			}
		}
		#region Drag mouse handlers
		private Point startPoint = new Point();
		private bool IsLeftBtnDown=false;
		private void ListBox_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		{
			startPoint = e . GetPosition ( null );
			IsLeftBtnDown = true;
		}
		private void DataGrid_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		{
			startPoint = e . GetPosition ( null );
			IsLeftBtnDown = true;
		}
		#endregion Drag mouse handlers

		#region Mouse MOVE handlers
		private void ListBox_PreviewMouseMove ( object sender , MouseEventArgs e )
		{
			ListBoxItem lb = null;

			if ( IsLeftBtnDown == true && e . LeftButton == MouseButtonState . Pressed )
			{
				lb = sender as ListBoxItem;
				if ( lb == null )
				{
					IsLeftBtnDown = false;
					return;
				}
				// Get the dragged ListViewItem
				ListBoxItem listBox = sender as ListBoxItem;
				if ( listBox == null )
				{
					IsLeftBtnDown = false;
					return;
				}
				ListBoxItem listBoxItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
				bankvm = listBoxItem . Content as BankAccountViewModel;
				// Initialize the drag & drop operation
				DataObject dragData = new DataObject("BANKACCOUNT", bankvm);
				dragData . SetText ( bankvm.ToString());
				dragData .SetData(bankvm );
				//string dataFormat = DataFormats . Text;
				//DataObject dataObject = new DataObject ( dataFormat, bankvm );
				DragDrop . DoDragDrop ( listBoxItem , dragData , DragDropEffects . Copy );
				IsLeftBtnDown = false;
			}

		}
		private void DataGrid_PreviewMouseMove ( object sender , MouseEventArgs e )
		{
			DataGridRow lb = null;

			if ( IsLeftBtnDown == true && e . LeftButton == MouseButtonState . Pressed )
			{
				lb = sender as DataGridRow;
				if ( lb == null )
				{
					IsLeftBtnDown = false;
					return;
				}
				// Get the dragged ListViewItem
				DataGridRow listBox = sender as DataGridRow;
				if ( listBox == null )
				{
					IsLeftBtnDown = false;
					return;
				}
				DataGridRow listBoxItem = FindAncestor<DataGridRow>((DependencyObject)e.OriginalSource);
				bankvm = listBoxItem . Item as BankAccountViewModel;
				// Initialize the drag & drop operation
				DataObject dragData = new DataObject();
				dragData . SetText ( "bankaccount" );
				dragData . SetData( bankvm );
				DragDrop . DoDragDrop ( listBoxItem , dragData , DragDropEffects . Copy );
				IsLeftBtnDown = false;
			}
		}
		#endregion Mouse MOVE handlers

		#region DROP handlers
		private void DataGridRow_Drop ( object sender , DragEventArgs e )
		{
			string [] str = e . Data . GetFormats ( );
			if ( e . Data . GetDataPresent ( "BANKACCOUNT" ) )
			{
				BankAccountViewModel bvm= e.Data.GetData("BANKACCOUNT") as BankAccountViewModel;
				DataGridRow dg = sender as DataGridRow;
				BankviewerView = null;
				BankViewcollection . Add ( bvm as BankAccountViewModel );
				//Assign our view back, but to the new data with added record
				BankviewerView = CollectionViewSource . GetDefaultView ( BankViewcollection );
				BankGrid . Refresh ( );
				ListGrid . Refresh ( );
				Listview . Refresh ( );
				Cursor = Cursors . Arrow;
			}
		}
		private void ListBox_Drop ( object sender , DragEventArgs e )
		{
			if ( e . Data . GetDataPresent ( "BANKACCOUNT" ) )
			{
				BankAccountViewModel bvm= e.Data.GetData("BANKACCOUNT") as BankAccountViewModel;
				DataGridRow dg = sender as DataGridRow;
				BankviewerView = null;
				BankViewcollection . Add ( bvm as BankAccountViewModel );
				//Assign our view back, but to the new data with added record
				BankviewerView = CollectionViewSource . GetDefaultView ( BankViewcollection );
				BankGrid . Refresh ( );
				ListGrid . Refresh ( );
				Listview . Refresh ( );
				Cursor = Cursors . Arrow;
			}
		}
		#endregion DROP handlers

		// Helper to search up the VisualTree
		private static T FindAncestor<T> ( DependencyObject current )
		    where T : DependencyObject
		{
			do
			{
				if ( current is T )
				{
					return ( T ) current;
				}
				current = VisualTreeHelper . GetParent ( current );
			}
			while ( current != null );
			return null;
		}
	}
}