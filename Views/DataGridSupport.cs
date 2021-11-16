using System . ComponentModel;
using System . Windows . Controls . Primitives;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows;
using System . Windows . Data;
using System;
using System . Collections . Generic;
using System . Runtime . InteropServices . WindowsRuntime;

namespace WPFPages
{

	/// <summary>
	///  This class is a method from the Web to access the selected Cell in any DataGrid
	///  We need to call this from the MouseRightButtonUp Handler
	///  
	/// 		private void DataGrid_MouseRightButtonUp (object sender,	MouseButtonEventArgs e)
	/// </summary>
	public class DataGridSupport
	{
		// Maximum allowabke sort options allowed to avoid invlaid column settings
		private static int MAXBANKSORTS=2;
		private static int MAXCUSTOMERSORTS=2;
		private static int MAXDETAILSSORTS=2;


		#region Datagrid Columns sorting methods
		public static void SortBankColumns ( DataGrid dgrid , DataGridColumn[] DGColumnsCollection, int SortType = -1 , int [ ] SortArray = null )
		{
			if ( SortType > MAXBANKSORTS )
				return;
			if ( DGColumnsCollection [ 0 ] == null )
			{																												     
				Console . WriteLine ( $"SORTBANKCOLUMNS - DataGridColumns contains NULLS for grid [{dgrid . Name}] !!" );
				return;
			}
			List <DataGridColumn> colnames = new List<DataGridColumn>();
			foreach ( var item in DGColumnsCollection )
			{
				item . Header = "  " + ( string ) item . Header . ToString ( ) . Trim ( );
				colnames . Add (  item );
			}
			if ( colnames . Count == 0 )
				return;
			//Resort columns into order I want
			dgrid . Columns . Clear ( );
			if ( SortArray != null && SortArray . Length > 0 && SortArray . Length == 7 )
			{
				SortByCustomOrder ( dgrid , colnames , SortArray );
				return;
			}
			if ( SortType == -1 || SortType == 0 )
			{
				//default setup	    (preferred)
				dgrid . Columns . Add ( DGColumnsCollection [ 0 ] );// Id
				dgrid . Columns . Add ( DGColumnsCollection [ 3 ] );//	CustNo
				dgrid . Columns . Add ( DGColumnsCollection [ 4 ] );//	BankNo
				dgrid . Columns . Add ( DGColumnsCollection [ 5 ] );//	AcType
				dgrid . Columns . Add ( DGColumnsCollection [ 6 ] );//	Balance
				dgrid . Columns . Add ( DGColumnsCollection [ 7 ] );//	Intrate
				dgrid . Columns . Add ( DGColumnsCollection [ 1 ] );//	ODate
				dgrid . Columns . Add ( DGColumnsCollection [ 2 ] );//	CDate
			}
			else if ( SortType == 1 )
			{
				dgrid . Columns . Add ( colnames [ 0 ] );// Id
				dgrid . Columns . Add ( colnames [ 4 ] );//	AcType
				dgrid . Columns . Add ( colnames [ 2 ] );//	CustNo
				dgrid . Columns . Add ( colnames [ 1 ] );//	BankNo
				dgrid . Columns . Add ( colnames [ 5 ] );//	Balance
				dgrid . Columns . Add ( colnames [ 3 ] );//	Intrate
				dgrid . Columns . Add ( colnames [ 6 ] );//	ODate
				dgrid . Columns . Add ( colnames [ 7 ] );//	CDate
			}
			else if ( SortType == 2 )
			{
				dgrid . Columns . Add ( colnames [ 5 ] );//	Balance
				dgrid . Columns . Add ( colnames [ 2 ] );//	CustNo
				dgrid . Columns . Add ( colnames [ 1 ] );//	BankNo
				dgrid . Columns . Add ( colnames [ 4 ] );//	AcType
				dgrid . Columns . Add ( colnames [ 3 ] );//	Intrate
				dgrid . Columns . Add ( colnames [ 6 ] );//	ODate
				dgrid . Columns . Add ( colnames [ 7 ] );//	CDate
				dgrid . Columns . Add ( colnames [ 0 ] );// Id
			}
		}


		public static void SortCustomerColumns ( DataGrid dgrid , DataGridColumn [ ] DGColumnsCollection , int SortType = -1 , int [ ] SortArray = null )
		{
			if ( SortType > MAXCUSTOMERSORTS )
				return;
			if ( DGColumnsCollection [ 0 ] == null )
			{
				Console . WriteLine ( $"DataGridColumns contains NULLS !!" );
				return;
			}
			List <DataGridColumn> colnames = new List<DataGridColumn>();
			foreach ( var item in DGColumnsCollection )
			{
				item . Header = "  " + ( string ) item . Header . ToString ( ).Trim();
				colnames . Add ( item );
			}
			if ( colnames . Count == 0 )
				return;
			//Resort columns into order I want
			dgrid . Columns . Clear ( );
			if ( SortArray != null && SortArray . Length > 0 && SortArray . Length == 14 )
			{
				SortByCustomOrder ( dgrid , colnames , SortArray );
				return;
			}
			if ( SortType == -1 || SortType == 0 )
			{
				//Default setup (preferred)
				dgrid . Columns . Add ( DGColumnsCollection [ 0 ] );//	Id
				dgrid . Columns . Add ( DGColumnsCollection [ 1 ] );//	CustNo
				dgrid . Columns . Add ( DGColumnsCollection [ 2 ] );//	BankNo
				dgrid . Columns . Add ( DGColumnsCollection [ 3 ] ); //	AcTyoe
				dgrid . Columns . Add ( DGColumnsCollection [ 4 ] ); //	FName
				dgrid . Columns . Add ( DGColumnsCollection [ 5 ] ); // LName
				dgrid . Columns . Add ( DGColumnsCollection [ 6 ] ); //	Addr1
				dgrid . Columns . Add ( DGColumnsCollection [ 7 ] );  //	Addr2
				dgrid . Columns . Add ( DGColumnsCollection [ 8 ] );  //Town
				dgrid . Columns . Add ( DGColumnsCollection [ 9 ] );  // County
				dgrid . Columns . Add ( DGColumnsCollection [ 10 ] ); //PCode
				dgrid . Columns . Add ( DGColumnsCollection [ 11 ] ); //Phone
				dgrid . Columns . Add ( DGColumnsCollection [ 12 ] ); //Mobile
				dgrid . Columns . Add ( DGColumnsCollection [ 13 ] ); //  ODate
				dgrid . Columns . Add ( DGColumnsCollection [ 14 ] ); //	CDate
			}
			else if ( SortType == 1 )
			{
				dgrid . Columns . Add ( DGColumnsCollection [ 4 ] );//	BankNo
				dgrid . Columns . Add ( DGColumnsCollection [ 3 ] );//	CustNo
				dgrid . Columns . Add ( DGColumnsCollection [ 5 ] ); //	AcType
				dgrid . Columns . Add ( DGColumnsCollection [ 7 ] ); // LName
				dgrid . Columns . Add ( DGColumnsCollection [ 6 ] ); //	FName
				dgrid . Columns . Add ( DGColumnsCollection [ 12 ] ); //PCode
				dgrid . Columns . Add ( DGColumnsCollection [ 10 ] );  //Town
				dgrid . Columns . Add ( DGColumnsCollection [ 11 ] );  // County
				dgrid . Columns . Add ( DGColumnsCollection [ 9 ] );  //	Addr2
				dgrid . Columns . Add ( DGColumnsCollection [ 8 ] ); //	Addr1
				dgrid . Columns . Add ( DGColumnsCollection [ 13 ] ); //Phone
				dgrid . Columns . Add ( DGColumnsCollection [ 14 ] ); //Mobile
				dgrid . Columns . Add ( DGColumnsCollection [ 1 ] ); //  ODate
				dgrid . Columns . Add ( DGColumnsCollection [ 2 ] ); //	CDate
				dgrid . Columns . Add ( DGColumnsCollection [ 0 ] );//	Id
			}
			else if ( SortType == 2 )
			{
				dgrid . Columns . Add ( DGColumnsCollection [ 3 ] );//	CustNo
				dgrid . Columns . Add ( DGColumnsCollection [ 10 ] );  //Town
				dgrid . Columns . Add ( DGColumnsCollection [ 11 ] );  // County
				dgrid . Columns . Add ( DGColumnsCollection [ 8 ] ); //	Addr1
				dgrid . Columns . Add ( DGColumnsCollection [ 9 ] );  //	Addr2
				dgrid . Columns . Add ( DGColumnsCollection [ 12 ] ); //PCode
				dgrid . Columns . Add ( DGColumnsCollection [ 7 ] ); // LName
				dgrid . Columns . Add ( DGColumnsCollection [ 6 ] ); //	FName
				dgrid . Columns . Add ( DGColumnsCollection [ 13 ] ); //Phone
				dgrid . Columns . Add ( DGColumnsCollection [ 14 ] ); //Mobile
				dgrid . Columns . Add ( DGColumnsCollection [ 4 ] );//	BankNo
				dgrid . Columns . Add ( DGColumnsCollection [ 5 ] ); //	AcTyoe
				dgrid . Columns . Add ( DGColumnsCollection [ 0 ] );//	Id
				dgrid . Columns . Add ( DGColumnsCollection [ 1 ] ); //  ODate
				dgrid . Columns . Add ( DGColumnsCollection [ 2 ] ); //	CDate
			}
		}
		public static void SortDetailsColumns ( DataGrid dgrid , DataGridColumn [ ] DGColumnsCollection , int SortType = -1 , int [ ] SortArray = null )
		{
			if ( SortType > MAXDETAILSSORTS )
				return;
			if ( DGColumnsCollection [ 0 ] == null )
			{
				Console . WriteLine ( $"DataGridColumns contains NULLS !!" );
				return;
			}
			List <DataGridColumn> colnames = new List<DataGridColumn>();
			foreach ( var item in DGColumnsCollection )
			{
				item . Header = "  " + ( string ) item . Header . ToString ( ) . Trim ( );
				colnames . Add ( item );
			}
			if ( colnames . Count == 0 )
				return;
			//Resort columns into order I want
			dgrid . Columns . Clear ( );
			if ( SortArray != null && SortArray . Length > 0 && SortArray . Length == 7 )
			{
				SortByCustomOrder ( dgrid , colnames , SortArray );
				return;
			}
			if ( SortType == -1 || SortType == 0 )
			{
				//default setup	    (preferred)
				dgrid . Columns . Add ( DGColumnsCollection [ 0 ] );// Id
				dgrid . Columns . Add ( DGColumnsCollection [ 3 ] );//	CustNo
				dgrid . Columns . Add ( DGColumnsCollection [ 4 ] );//	BankNo
				dgrid . Columns . Add ( DGColumnsCollection [ 5 ] );//	AcType
				dgrid . Columns . Add ( DGColumnsCollection [ 6 ] );//	Balance
				dgrid . Columns . Add ( DGColumnsCollection [ 7 ] );//	Intrate
				dgrid . Columns . Add ( DGColumnsCollection [ 1 ] );//	ODate
				dgrid . Columns . Add ( DGColumnsCollection [ 2 ] );//	CDate
			}
			else if ( SortType == 1 )
			{
				dgrid . Columns . Add ( colnames [ 0 ] );// Id
				dgrid . Columns . Add ( colnames [ 4 ] );//	AcType
				dgrid . Columns . Add ( colnames [ 2 ] );//	CustNo
				dgrid . Columns . Add ( colnames [ 1 ] );//	BankNo
				dgrid . Columns . Add ( colnames [ 5 ] );//	Balance
				dgrid . Columns . Add ( colnames [ 3 ] );//	Intrate
				dgrid . Columns . Add ( colnames [ 6 ] );//	ODate
				dgrid . Columns . Add ( colnames [ 7 ] );//	CDate
			}
			else if ( SortType == 2 )
			{
				dgrid . Columns . Add ( colnames [ 5 ] );//	Balance
				dgrid . Columns . Add ( colnames [ 2 ] );//	CustNo
				dgrid . Columns . Add ( colnames [ 1 ] );//	BankNo
				dgrid . Columns . Add ( colnames [ 4 ] );//	AcType
				dgrid . Columns . Add ( colnames [ 3 ] );//	Intrate
				dgrid . Columns . Add ( colnames [ 6 ] );//	ODate
				dgrid . Columns . Add ( colnames [ 7 ] );//	CDate
				dgrid . Columns . Add ( colnames [ 0 ] );// Id
			}
			else if ( SortArray != null && SortArray . Length > 0 && SortArray . Length == 7 )
			{
				SortByCustomOrder ( dgrid , colnames , SortArray );
			}
		}
		private static void SortByCustomOrder ( DataGrid dgrid , List<DataGridColumn> colnames , int [ ] SortArray )
		{
			if ( SortArray . Length == 7 )
			{
				dgrid . Columns . Add ( colnames [ SortArray [ 0 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 1 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 2 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 3 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 4 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 5 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 6 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 7 ] ] );
			}
			else
			{
				dgrid . Columns . Add ( colnames [ SortArray [ 0 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 1 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 2 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 3 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 4 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 5 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 6 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 7 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 8 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 9 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 10 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 11 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 12 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 13 ] ] );
				dgrid . Columns . Add ( colnames [ SortArray [ 14 ] ] );
			}
		}

		#endregion Datagrid Columns sorting methods
		public static int GetDataGridRowFromTree ( MouseButtonEventArgs e , out DataGridRow RowData )
		{
			int currentRow = -1;
			DependencyObject dep = ( DependencyObject ) e . OriginalSource;
			dep = VisualTreeHelper . GetParent ( dep );

			//I have found that we can still get the current row
			{
				DataGridCell cell = dep as DataGridCell;
				// navigate further up the tree
				while ( ( dep != null ) && !( dep is DataGridRow ) )
				{
					dep = VisualTreeHelper . GetParent ( dep );
				}
				DataGridRow row = dep as DataGridRow;
				currentRow = FindRowIndex ( row );
				RowData = row;
				return currentRow;
			}

		}

		public static object GetCellContent ( object sender , MouseButtonEventArgs e , string CurrentDb , out int currentRow , out int currentColumn , out string columnName , out object rowdata )
		{
			currentRow = -1;
			currentColumn = -1;
			columnName = "";
			rowdata = null;
			object returnData = null;
			DependencyObject dep = (DependencyObject)e.OriginalSource;
			DataGridCell cell = null;
			// iteratively traverse the visual tree
			//while ((dep != null) &&
			//!(dep is DataGridCell) &&
			//!(dep is DataGridColumnHeader))
			////Orignal code block
			//{

			//My trial version to handle datePicker
			while ( ( dep != null ) &&
			!( dep is DataGridCell ) &&
			!( dep is DatePicker ) &&
			!( dep is DataGridRow ) &&
			!( dep is DataGridColumnHeader ) )
			//Orignal code block
			{
				//This is called when there is a problem such as a DatePicker column  being received
				dep = VisualTreeHelper . GetParent ( dep );

				//I have found that we can still get the current row
				//{
				//	DataGridCell cell = dep as DataGridCell;
				//	// navigate further up the tree
				//	while ((dep != null) && !(dep is DataGridRow))
				//	{
				//		dep = VisualTreeHelper.GetParent (dep);
				//	}
				//	DataGridRow row = dep as DataGridRow;
				//	currentRow = FindRowIndex (row);
				//	break;
				//}
			}

			if ( dep == null )
				return returnData;

			if ( dep is DataGridColumnHeader )
			//Orignal code block
			{
				DataGridColumnHeader columnHeader = dep as DataGridColumnHeader;
				// do something
				return columnHeader as object;
			}
			else
			//***********************************//
			//MY TEST code block
			{
				if ( dep is DatePicker )
				{
					//Never hit.....
					DataGridRow dgr = dep as DataGridRow;
					return dgr . Item;
				}
				else if ( dep is DataGridCell )
				{
					// loo0ks like a DatePicker was clicked on - we cant handle these
					//					DependencyProperty dp = new DependencyProperty ();
					object dgcell = dep.GetType();
					string g = dgcell.ToString ();
					//this is done later on anyway
					//cell = dep as DataGridCell;
				}
				else if ( dep is DataGridRow )
				{
					// looks like a DatePicker was clicked on - we cant handle these
					DataGridRow dgr = dep as DataGridRow;
					return dgr . Item;
				}
				DataGridColumn dgc = dep as DataGridColumn;
				cell = dep as DataGridCell;
			}
			//***********************************//
			if ( dep is DataGridCell )
			//Orignal code block
			{
				cell = dep as DataGridCell;
				DataGridColumn  dgc = cell.Column;
				//Get Header's Column text
				string colheader = dgc.Header as string;
				columnName = colheader;
				//Get column offset
				int colindex = dgc.DisplayIndex;
				Type t = dgc.GetType ();
				//				dgc.GetValue ();
				// navigate further up the tree
				while ( ( dep != null ) && !( dep is DataGridRow ) )
				{
					dep = VisualTreeHelper . GetParent ( dep );
					if ( dep is DataGridCell )
					{
						cell = dep as DataGridCell;
					}
					//					if (dep is TextBlock)
					//					{
					//						cell = dep as TextBlock;
					//					}
				}

				DataGridRow row = dep as DataGridRow;

				object cellData = ExtractBoundValue (row, cell, out  currentColumn);
				return ( object ) cellData;
			}
			return ( object ) null;
		}
		private static int FindRowIndex ( DataGridRow row )
		{
			DataGrid dataGrid = ItemsControl.ItemsControlFromItemContainer (row) as DataGrid;

			int index = dataGrid.ItemContainerGenerator.IndexFromContainer (row);
			return index;
		}
		//private static int FindColumnIndex (DataGridColumn col)
		//{
		//	DataGrid dataGrid = ItemsControl.ItemsControlFromItemContainer (col) as DataGrid;

		//	int index = dataGrid.ItemContainerGenerator.IndexFromContainer (col);
		//	return index;
		//}

		private static object ExtractBoundValue ( DataGridRow row , DataGridCell cell , out int column )
		{
			column = -1;
			// find the column that this cell belongs to
			DataGridBoundColumn col =   cell.Column as DataGridBoundColumn;
			if ( col == null )
				return null;
			column = cell . Column . DisplayIndex;
			// find the property that this column is bound to
			Binding binding = col.Binding as Binding;
			string boundPropertyName = binding.Path.Path;

			// find the object that is related to this row
			object data = row.Item;
			if ( data == null )
				return null;
			// extract the property value
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties (data);
			if ( properties == null )
				return null;

			PropertyDescriptor property = properties[boundPropertyName];
			object value = property.GetValue (data);

			return value;
		}
	}

}
