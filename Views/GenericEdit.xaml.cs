using Dapper;

using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Data . SqlClient;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;
using System . Xml . Linq;

//using WPFLibrary2021;

using WPFPages . ViewModels;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for GenericEdit.xaml
	/// </summary>
	public partial class GenericEdit : Window
	{
		public  static string tblname { get; set; }

		public static List<string>Datafields = new List<string>();
		public static GenericClass gc = new GenericClass();
		public GenericEdit ( )
		{
			InitializeComponent ( );
			Library1 . SetupWindowDrag ( this );
			this. DataContext = this.
			Fld2 . Focus ( );
			Show ( );
		}

		private void Button_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void Button_Click_1 ( object sender , RoutedEventArgs e )
		{
			this . Close ( );
		}

		private void gotfocus ( object sender , KeyboardFocusChangedEventArgs e )
		{
			//TextBox tb = sender as TextBox;
			//tb . SelectionLength = tb . Text . Length;
			//e . Handled = true;
		}

		private void gotmfocus ( object sender , MouseButtonEventArgs e )
		{
			//TextBox tb = sender as TextBox;
			//tb . SelectionLength = tb . Text . Length;
			//tb . Refresh ( );
			//e . Handled = true;

		}

		private void gotxfocus ( object sender , MouseButtonEventArgs e )
		{
			//TextBox tb = sender as TextBox;
			//tb . SelectionLength = tb . Text . Length;
			//tb . Refresh ( );
			//e . Handled = true;

		}

		private void lostfocus ( object sender , KeyboardFocusChangedEventArgs e )
		{
			//TextBox tb = sender as TextBox;
			//tb . SelectionLength = 0;
			//tb . Refresh ( );
			//e . Handled = true;

		}
		private void UpdateFromCurrentFields ( )
		{
			int max = Datafields.Count;
			int count=0;
			Datafields [ 0 ] = Fld1 . Text;
			if ( ++count == max ) return;
			Datafields [ 1 ] = Fld2 . Text;
			if ( ++count == max )
				return;
			Datafields [ 2 ] = Fld3 . Text;
			if ( ++count == max )
				return;
			Datafields [ 3 ] = Fld4 . Text;
			if ( ++count == max )
				return;
			Datafields [ 4 ] = Fld5 . Text;
			if ( ++count == max )
				return;
			Datafields [ 5 ] = Fld6 . Text;
			if ( ++count == max )
				return;
			Datafields [ 6 ] = Fld7 . Text;
			if ( ++count == max )
				return;
			Datafields [ 7 ] = Fld8 . Text;
			if ( ++count == max )
				return;
			Datafields [ 8 ] = Fld9. Text;
			if ( ++count == max )
				return;
			Datafields [ 9 ] = Fld10 . Text;
			if ( ++count == max )
				return;
			Datafields [ 10 ] = Fld11 . Text;
			if ( ++count == max )
				return;
			Datafields [ 11 ] = Fld12 . Text;
			if ( ++count == max )
				return;
			Datafields [ 12 ] = Fld13 . Text;
			if ( ++count == max )
				return;
			Datafields [ 13 ] = Fld14 . Text;
			if ( ++count == max )
				return;
			Datafields [ 14] = Fld15 . Text;
			if ( ++count == max )
				return;
			Datafields [ 15 ] = Fld16 . Text;
			if ( ++count == max )
				return;
			Datafields [ 16 ] = Fld17 . Text;
			if ( ++count == max )
				return;
			Datafields [ 17 ] = Fld18 . Text;
			if ( ++count == max )
				return;
			Datafields [ 18 ] = Fld19 . Text;
			if ( ++count == max )
				return;
			Datafields [ 19 ] = Fld20 . Text;
		}
		private void Save_Click ( object sender , RoutedEventArgs e )
		{
			// create a new GenericClass record from the data we have in the window
			// Works fine 28/12/21
			int count=0;
			// Save  as a Db Table named "GenericTable"
			UpdateFromCurrentFields ( );
			foreach ( var item in Datafields )
			{
				if ( count == 0 )
					gc . field1 = item;
				if ( count == 1 )
					gc . field2 = item;
				if ( count == 2 )
					gc . field3 = item;
				if ( count == 3 )
					gc . field4 = item;
				if ( count == 4 )
					gc . field5 = item;
				if ( count == 5 )
					gc . field6 = item;
				if ( count == 6 )
					gc . field7 = item;
				if ( count == 7 )
					gc . field8 = item;
				if ( count == 8 )
					gc . field9 = item;
				if ( count == 9 )
					gc . field10 = item;
				if ( count == 10 )
					gc . field11 = item;
				if ( count == 11 )
					gc . field12 = item;
				if ( count == 12 )
					gc . field13 = item;
				if ( count == 13 )
					gc . field14 = item;
				if ( count == 14 )
					gc . field15 = item;
				if ( count == 15 )
					gc . field16 = item;
				if ( count == 16 )
					gc . field17 = item;
				if ( count == 17 )
					gc . field18 = item;
				if ( count == 18 )
					gc . field19 = item;
				if ( count == 19 )
					gc . field20 = item;
				count++;
			}
			if ( count > 0 )
			{
				// Get the data from the selected Db and display it in generic grid
				// Generic call that wil return the results of any valid SQL select command as an Observable colection<GenericClass>
				Dictionary < string, string > dic = new Dictionary<string, string>();
				GenericClass gcc = new GenericClass();
				ObservableCollection< GenericClass > generic = new ObservableCollection<GenericClass> ( );
				string errmsg="";

				// check if table already exists ?
				if ( tblname . Contains ( "Generic" ) == false )
					tblname = $"Generic{tblname}";
				generic = DapperGeneric<Dictionary<string , string> , GenericClass , bool> . CreateFromDictionary (
					 dic ,
					gcc ,
					$"select * from {tblname}" ,
					 ref errmsg );
				if ( errmsg != "" )
				{
					Mouse . OverrideCursor = Cursors . Arrow;
					this . Topmost = false;
					Utils . Mssg ( "" ,  string1: $"This Table Edit System uses 'special' tables wiith 'Generic' at the front of them to avoid trashing original data....\n" , 
						string2: $"Do you want to go ahead and create a  table named [{tblname.ToUpper()}] & save this record to it ?" , 
						string3: "Proceeding with this will NOT change  the original Db Table in any way !" ,
						title: "" , iconstring: "" ,
						defButton: MB.YES , Btn1: MB.YES , Btn2: MB.NO, Btn3: 0 , Btn4: 0 ,
						btn1Text: "" , btn2Text: "" , btn3Text: "" , btn4Text: "" );

//					Utils . Mbox ( this , string1: $"This Edit uses tables named wiith 'Generic' at the front to avoid trashing original data...." , string2: $"Do you want to go ahead and create a  table named Generic{tblname} & save this record to it ?" , caption: "" , 
//						iconstring: "\\icons\\error2.png" , Btn1: MB . YES , Btn2: MB . NO , defButton: MB . YES );
					if(Dlgresult.returnint >2)
					return;
					// Create a new db with selected name
					string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
					IEnumerable  resultDb;

					using ( IDbConnection db = new SqlConnection ( ConString ) )
					{
						var parameters = new{ Arg1=tblname};
						var reslt = db . Query ( "spCreateGenericDb",  parameters, commandType: CommandType . StoredProcedure );
						if(reslt == null)
						{
							// failed.....
						}
						else
						{
							// got a new db , so just drop thru to update!!!
						}
					}
				}
				if ( dic . Count > 0 )
				{
					Mouse . OverrideCursor = Cursors . Arrow;
					Utils . Mbox ( this , string1: "A table with the name 'GenericTable' already exists, do you want to overwrite it ?" , string2: "" , caption: "Data Exists" , iconstring: "\\icons\\Information.png" , Btn1: MB . YES , Btn2: MB . NO , defButton: MB . YES );
					if ( Dlgresult . returnint == 2 )
					{
						// yes, so just update the existing 
						UpdateGenericDb ( );
					}
					else
						return;
				}
				// we can go ahead and CREATE it here
				// 1st we update the selected record
				Update_Click ( sender , null );
			}
		}
		public static bool UpdateGenericDb ( )
		{
			//Call Dapper to update / Create the new Table
			DapperSupport . UpdateGenericDb ( gc );
			return true;
		}

		private void Update_Click ( object sender , RoutedEventArgs e )
		{
			// update a GenericClass record from the data we have in the window
			int count=0;

			if ( tblname . Contains ( "Generic" ) == false)
			{
				Utils . Mbox ( this , string1: $"The table you are editing is a NON Generic table named '{tblname}', This record will be saved into a table named Generic{tblname}. " , string2: $"Do you want to go ahead and add it  to the new generic table ?" , caption: "Data Exists" , iconstring: "\\icons\\Information.png" , Btn1: MB . YES , Btn2: MB . NO , defButton: MB . NO );
				if ( Dlgresult . returnint == 3 ) // NO
					return;
			}

			// Save  in Db Table named "GenericTable"
			foreach ( var item in Datafields )
			{
				if ( count == 0 )
					gc . field1 = item;
				if ( count == 1 )
					gc . field2 = item;
				if ( count == 2 )
					gc . field3 = item;
				if ( count == 3 )
					gc . field4 = item;
				if ( count == 4 )
					gc . field5 = item;
				if ( count == 5 )
					gc . field6 = item;
				if ( count == 6 )
					gc . field7 = item;
				if ( count == 7 )
					gc . field8 = item;
				if ( count == 8 )
					gc . field9 = item;
				if ( count == 9 )
					gc . field10 = item;
				if ( count == 10 )
					gc . field11 = item;
				if ( count == 11 )
					gc . field12 = item;
				if ( count == 12 )
					gc . field13 = item;
				if ( count == 13 )
					gc . field14 = item;
				if ( count == 14 )
					gc . field15 = item;
				if ( count == 15 )
					gc . field16 = item;
				if ( count == 16 )
					gc . field17 = item;
				if ( count == 17 )
					gc . field18 = item;
				if ( count == 18 )
					gc . field19 = item;
				if ( count == 19 )
					gc . field20 = item;
				count++;
			}
			if ( count > 0 )
			{
				// Get the data from the selected Db and display it in generic grid
				// Generic call that wil return the results of any valid SQL select command as an Observable colection<GenericClass>
				Dictionary < string, string > dic = new Dictionary<string, string>();
				GenericClass gcc = new GenericClass();
				ObservableCollection< GenericClass > generic = new ObservableCollection<GenericClass> ( );
				string errmsg="";

				if ( tblname . Contains ( "Generic" ) == false )
					tblname = $"Generic{tblname}";
				generic = DapperGeneric<Dictionary<string , string> , GenericClass , bool> . CreateFromDictionary (
					 dic ,
					gcc ,
					$"select * from {tblname}" ,
					 ref errmsg );
				if ( errmsg != "" )
				{
				}
				if ( dic . Count > 0 )
				{
					Mouse . OverrideCursor = Cursors . Arrow;
					Utils . Mbox ( this , string1: $"A table with the name '{tblname}' already exists, do you want to overwrite it ?" , string2: "" , caption: "Data Exists" , iconstring: "\\icons\\Information.png" , Btn1: MB . YES , Btn2: MB . NO , defButton: MB . YES );
					if ( Dlgresult . returnint == 2 )
					{
						// yes, so just update the existing 
						UpdateGenericDb ( );

						if ( tblname . Contains ( "Generic" ) == false )
							tblname = $"Generic{tblname}";
						DapperSupport . SaveGenericDb (
						gc ,
						tblname );
						Mouse . OverrideCursor = Cursors . Arrow;
						Utils . Mbox ( this , string1: $"The record has been added to the table named {tblname} successfully..." , string2: "" , caption: "" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
						return;
					}
					else
						return;
				}
				else
				{
					// save it all
					if ( tblname . Contains ( "Generic" ) == false )
						tblname = $"Generic{tblname}";
					if(DapperSupport . SaveGenericDb (
					gc , tblname
					 ) == false)
					{
						Mouse . OverrideCursor = Cursors . Arrow;
						Utils . Mbox ( this , string1: "An error occured while attempting to update the current record" , string2: "" , caption: "" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK , Btn2: MB . NNULL, defButton: MB . OK );
					}
					else
					{
						Mouse . OverrideCursor = Cursors . Arrow;
						Utils . Mbox ( this , string1: $"The record has been added to the table named {tblname} successfully..." , string2: "" , caption: "" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
//
//						Utils . Mbox ( this , string1: $"A table named {tblname} has been created and the new record added to it" , string2: "" , caption: "" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
					}
					return;
				}
			}
			return;
		}

		private void GenEditWindow_Loaded ( object sender , RoutedEventArgs e )
		{
			Tblname . Text = tblname;

		}
	}
}
