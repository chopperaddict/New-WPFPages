using Microsoft . SqlServer . Management . Smo;

using System;
using System . Collections . Generic;
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

using WPFLibrary2021;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for FullGridViewer.xaml
	/// </summary>
	public partial class FullGridViewer : Window
	{
		#region Porperties
		public static int reccount { get; set; }
		public static string Tablename { get; set; }
		#endregion Porperties

		public List<string> genfields = new List<string>();
		public  GenericClass gc = new GenericClass();
		public string currec = "";
			public FullGridViewer ( )
		{
			InitializeComponent ( );
			Library1 . SetupWindowDrag ( this );
			defvars . FullViewer = true;
			DataContext = this;
			UniversalGrid . Loaded += UniversalGrid_Loaded1;
		}

		public void UniversalGrid_Loaded1 ( object sender , RoutedEventArgs e )
		{
			//reccount = UniversalGrid . Items . Count;
			//Reccount . Text = reccount . ToString ( );
			//Reccount . Refresh ( );
		}
		#region keyhandlers
		private void Close_Click ( object sender , RoutedEventArgs e )
		{
			e . Handled = true;
			this . Close ( );
		}
		private void Save_Click ( object sender , RoutedEventArgs e )
		{
			e . Handled = true;

		}
		private void Print_Click ( object sender , RoutedEventArgs e )
		{
			e . Handled = true;

		}
		#endregion keyhandlers

		public static void ParseGenericRow ( string dgr , out List<string> genfields )
		{
			genfields = new List<string> ( );
			string output="";
			string [] input;
			string[] flds = dgr.Split(',');
			foreach ( var item in flds )
			{
				input = item . Split ( '=' );
				do
				{
					if ( input [ 1 ] . Contains ( "}" ) )
					{
						input [ 1 ] = input [ 1 ] . Substring ( 0 , input [ 1 ] . Length - 1 );
					}
					else
						break;
				} while ( true );
				genfields . Add ( input [ 1 ] . Trim ( ) );
			}
		}

		private void UniversalGrid_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
		{

			Utils . Mssg ( caption: "" , string1: "Do you want to edit this record ?" , title: "Db Edit Facility" ,
						  defButton: 2 , Btn1: 2 , Btn2: 3 , Btn3: 0 , Btn4: 0 ,
						  btn2Text: "Yes Please" , btn3Text: "No Thanks" );
			if ( Dlgresult . returnint <= 2 )
			{
				// YES  returned by user
				int indx = 1, total=0;
				bool alldone = false;
				currec  = UniversalGrid . SelectedItem?.ToString();
				if(currec == null)
				{
					Mouse . OverrideCursor = Cursors . Arrow;
					Utils . Mbox ( this , string1: "It appears that you have not selected a record to be edited !" , string2: "Please click on the record you want to edit and then right click on it..." , caption: "Selection Error" , iconstring: "\\icons\\error.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
					return;
				}
				else if ( currec== "WPFPages.Views.GenericClass" )
					return;
				else if ( currec == "")
				{
					Mouse . OverrideCursor = Cursors . Arrow;
					Utils . Mbox ( this , string1: "It appears that you have not selected a record to be edited !" , string2: "Please click on the record you want to edit and then right click on it..." , caption: "Selection Error" , iconstring: "\\icons\\error.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
					return;
				}
				
				// Parse the data from the string received from caller
				genfields . Clear ( );
				ParseGenericRow ( currec, out genfields );
				
				// Load window  to process record data
				GenericEdit ge = new GenericEdit();
				ge . Show ( );
				indx = 1;
				foreach ( var item in genfields )
				{
					if ( indx >= 19 )
						break;
					if ( genfields [ indx - 1 ] == null )
						break;
					#region case structure
					switch ( indx )
					{
						case 1:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld1 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								//ge.
								indx++;
							}
							else
								indx++;
							break;

						case 2:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld2 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 3:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld3 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 4:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld4 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 5:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld5 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 6:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld6 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 7:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld7 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 8:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld8 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 9:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld9 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							break;
						case 10:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld10 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							break;
						case 11:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld11 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 12:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld12 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 13:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld13 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 14:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld14 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 15:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld15 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 16:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld16 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 17:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld17 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 18:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld18 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 19:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld19 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						case 20:
							if ( genfields [ indx - 1 ] != "" )
							{
								ge . Fld20 . Text = genfields [ indx - 1 ];
								GenericEdit . Datafields . Add ( genfields [ indx - 1 ] );
								indx++;
							}
							else
								indx++;
							break;
						default:
							break;

					}
		#endregion case structure

				}
				ShowFields ( indx - 1 , genfields , ge );
				GenericEdit. tblname = DbNameLabel . Text;
			}
			Mouse . OverrideCursor = Cursors . Arrow;
		}
		private void ShowFields ( int max , List<string> genfields , GenericEdit ge )
		{
			int indx= 0;
			for ( int x = max + 1 ; x <= 20 ; x++ )
			{
				HideFld ( ge , x );
			}
		}
		private void HideFld ( GenericEdit ge , int indx )
		{
			#region case structure
			switch ( indx )
			{
				case 1:
					ge . Fld1 . Visibility = Visibility . Hidden;
					break;
				case 2:
					ge . Fld2 . Visibility = Visibility . Hidden;
					break;
				case 3:
					ge . Fld3 . Visibility = Visibility . Hidden;
					break;
				case 4:
					ge . Fld4 . Visibility = Visibility . Hidden;
					break;
				case 5:
					ge . Fld5 . Visibility = Visibility . Hidden;
					break;
				case 6:
					ge . Fld6 . Visibility = Visibility . Hidden;
					break;
				case 7:
					ge . Fld7 . Visibility = Visibility . Hidden;
					break;
				case 8:
					ge . Fld8 . Visibility = Visibility . Hidden;
					break;
				case 9:
					ge . Fld9 . Visibility = Visibility . Hidden;
					break;
				case 10:
					ge . Fld10 . Visibility = Visibility . Hidden;
					break;
				case 11:
					ge . Fld11 . Visibility = Visibility . Hidden;
					break;
				case 12:
					ge . Fld12 . Visibility = Visibility . Hidden;
					break;
				case 13:
					ge . Fld13 . Visibility = Visibility . Hidden;
					break;
				case 14:
					ge . Fld14 . Visibility = Visibility . Hidden;
					break;
				case 15:
					ge . Fld15 . Visibility = Visibility . Hidden;
					break;
				case 16:
					ge . Fld16 . Visibility = Visibility . Hidden;
					break;
				case 17:
					ge . Fld17 . Visibility = Visibility . Hidden;
					break;
				case 18:
					ge . Fld18 . Visibility = Visibility . Hidden;
					break;
				case 19:
					ge . Fld19 . Visibility = Visibility . Hidden;
					break;
				case 20:
					ge . Fld20 . Visibility = Visibility . Hidden;
					break;
			}
			#endregion case structure
		}
	}
}
