using System;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq . Expressions;
using System . Security . AccessControl;
using System . Security . Cryptography;
using System . Threading;
using System . Threading . Tasks;
using System . Timers;
using System . Windows;
using System . Windows . Controls . Primitives;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Threading;

using WPFPages . ViewModels;

// Used by DetailsDbViewer
namespace WPFPages . Views
{
	public class DetailCollection : ObservableCollection<DetailsViewModel>
	{
		public static bool Notify = false;
		public static string Caller = "";

		public DetailCollection ( )
		{
		}

		public ObservableCollection<DetailsViewModel> DetCollection = new ObservableCollection<DetailsViewModel> ( );

		// working entity to load data into
		public static DetCollection internalcollection = new DetCollection();
		public static DataTable dtDetails = new DataTable ( );
		private static Stopwatch timer = new Stopwatch();
		public static ObservableCollection<DetailsViewModel> LoadDet ( string caller , int ViewerType = 1 , bool NotifyAll = false )
		{
			bool result = false;
			Notify = NotifyAll;
			Caller = caller;
			try
			{					
				//internalcollection = null;
				internalcollection = new DetCollection ( );
				timer . Start ( );
				dtDetails . Clear ( );
				 LoadDetailsTaskInSortOrderAsync ( internalcollection );
				Console . WriteLine ( $" DETAILS DB - ALL DONE - EXITING DETAILS SQL load : Time elapsed = {timer.ElapsedMilliseconds}" );
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"Error in Details Load system : {ex . Message} + {ex . Data} Time elapsed = {timer . ElapsedMilliseconds}" );
			}
			timer . Stop ( );
			timer . Reset ( );
			return internalcollection;
			;
		}


		public static bool LoadDetailsTaskInSortOrderAsync ( DetCollection internalcollection )
		{
			if ( dtDetails . Rows . Count > 0 )
				dtDetails . Clear ( );

			Debug . WriteLine ( $"DETCOLLECTION : Calling LOADDETAILSDATASQL(),  Time elapsed = {timer . ElapsedMilliseconds}" );
			if ( LoadDetailsDataSql ( ) . Rows . Count == 0 )
				return false;
			Debug . WriteLine ( $"DETCOLLECTION : Completed LOADDETAILSDATASQL()" );
			Debug . WriteLine ( $"DETCOLLECTION : Calling LOADDETCOLLECTION() Time elapsed = {timer . ElapsedMilliseconds}" );
			Debug . WriteLine ( $"DETCOLLECTION : Completed LOADDETCOLLECTION()  Time elapsed = { timer . ElapsedMilliseconds}" );
			if ( LoadDetCollection ( internalcollection ) . Count == 0 )
				return false;
			else
				return true;
		}

		/// Handles the actual conneciton ot SQL to load the Details Db data required
		/// </summary>
		/// <returns></returns>
		//**************************************************************************************************************************************************************//
		public static DataTable LoadDetailsDataSql ( bool isMultiMode = false )
		{
			SqlConnection con;
			string ConString = "";
			string filterline = "";
			ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			Debug . WriteLine ( $"Making new SQL connection in DETAILSCOLLECTION,  Time elapsed = {timer . ElapsedMilliseconds}" );
			
			con = new SqlConnection ( ConString );
			try
			{
				Debug . WriteLine ( $"Using new SQL connection in DETAILSCOLLECTION" );
				using ( con )
				{
					if ( Flags . IsMultiMode )
					{
						// Create a valid Query Command string including any active sort ordering
						filterline = $"SELECT * FROM SECACCOUNTS WHERE CUSTNO IN "
							+ $"(SELECT CUSTNO FROM SECACCOUNTS  "
							+ $" GROUP BY CUSTNO"
							+ $" HAVING COUNT(*) > 1) ORDER BY ";
						filterline = Utils . GetDataSortOrder ( filterline );
					}
					else if ( Flags . FilterCommand != "" )
					{
						filterline = Flags . FilterCommand;
					}
					else
					{
						// Create a valid Query Command string including any active sort ordering
						filterline = "Select * from SecAccounts  order by ";
						filterline = Utils . GetDataSortOrder ( filterline );
					}
					SqlCommand cmd = new SqlCommand ( filterline , con );
					SqlDataAdapter sda = new SqlDataAdapter ( cmd );
					sda . Fill ( dtDetails );
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"DETAILS : ERROR in LoadDetailsDataSql(): Failed to load Details Details - {ex . Message}, {ex . Data}" );
				MessageBox . Show ( $"DETAILS : ERROR in LoadDetailsDataSql(): Failed to load Details Details - {ex . Message}, {ex . Data}" );
			}
			finally
			{
				Console . WriteLine ($" SQL data loaded : Time elapsed = {timer . ElapsedMilliseconds}" );
				con . Close ( );
			}
			return dtDetails;
		}

		//**************************************************************************************************************************************************************//
		public static DetCollection LoadDetCollection ( DetCollection internalcollection )
		{
			int count = 0;
			try
			{
				Console . WriteLine ( $" Loading Datable with {dtDetails.Rows.Count} records: Time elapsed = {timer . ElapsedMilliseconds}" );
				internalcollection . Clear ( );
				for ( int i = 0 ; i < dtDetails . Rows . Count ; i++ )
				{
					internalcollection . Add ( new DetailsViewModel
					{
						Id = Convert . ToInt32 ( dtDetails . Rows [ i ] [ 0 ] ) ,
						BankNo = dtDetails . Rows [ i ] [ 1 ] . ToString ( ) ,
						CustNo = dtDetails . Rows [ i ] [ 2 ] . ToString ( ) ,
						AcType = Convert . ToInt32 ( dtDetails . Rows [ i ] [ 3 ] ) ,
						Balance = Convert . ToDecimal ( dtDetails . Rows [ i ] [ 4 ] ) ,
						IntRate = Convert . ToDecimal ( dtDetails . Rows [ i ] [ 5 ] ) ,
						ODate = Convert . ToDateTime ( dtDetails . Rows [ i ] [ 6 ] ) ,
						CDate = Convert . ToDateTime ( dtDetails . Rows [ i ] [ 7 ] ) ,
					} );
					count = i;
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"DETAILS : ERROR in  LoadDetCollection() : loading Details into ObservableCollection \"DetCollection\" : [{ex . Message}] : {ex . Data} ...." );
				MessageBox . Show ( $"DETAILS : ERROR in  LoadDetCollection() : loading Details into ObservableCollection \"DetCollection\" : [{ex . Message}] : {ex . Data} ...." );
				return null;
			}
			finally
			{
				if ( Notify )
				{
					EventControl . TriggerDetDataLoaded ( null ,
						new LoadedEventArgs
						{
							CallerType = "SQLSERVER" ,
							CallerDb = Caller ,
							DataSource = (object)internalcollection ,
							RowCount = internalcollection . Count
						} );
				}
			}
			Console . WriteLine ( $" DETAILS DB Loading () ALL FINISHED :  Records = [{internalcollection . Count}], Time elapsed = {timer . ElapsedMilliseconds}" );
			return internalcollection;
		}
		public static DataTable LoadDetailsDirect ( DataTable dtDetails , string Sqlcommand = "Select* from SecAccounts order by CustNo, BankNo" )
		{
			SqlConnection con;
			string ConString = "";
			string commandline = "";
			dtDetails . Clear ( );
			ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			Debug . WriteLine ( $"Making new SQL connection in DETAILSCOLLECTION" );
			con = new SqlConnection ( ConString );
			try
			{
				Debug . WriteLine ( $"Using new SQL connection in DETAILSCOLLECTION" );
				using ( con )
				{
					// Create a valid Query Command string including any active sort ordering
					commandline = "Select * from SecAccounts  order by ";
					commandline = Utils . GetDataSortOrder ( commandline );
					SqlCommand cmd = new SqlCommand ( commandline, con );
					SqlDataAdapter sda = new SqlDataAdapter ( cmd );

					DetCollection bptr = new DetCollection ( );
					sda . Fill ( dtDetails );
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"DETAILS : ERROR in LoadDetailsDataSql(): Failed to load Details Details - {ex . Message}, {ex . Data}" );
				MessageBox . Show ( $"DETAILS : ERROR in LoadDetailsDataSql(): Failed to load Details Details - {ex . Message}, {ex . Data}" );
			}
			finally
			{
				con . Close ( );
			}
			return dtDetails;
		}

		public static DetCollection LoadDetailsCollectionDirect ( DetCollection collection , DataTable dtDetails )
		{
			int count = 0;
			try
			{
				for ( int i = 0 ; i < dtDetails . Rows . Count ; i++ )
				{
					collection . Add ( new DetailsViewModel
					{
						Id = Convert . ToInt32 ( dtDetails . Rows [ i ] [ 0 ] ) ,
						BankNo = dtDetails . Rows [ i ] [ 1 ] . ToString ( ) ,
						CustNo = dtDetails . Rows [ i ] [ 2 ] . ToString ( ) ,
						AcType = Convert . ToInt32 ( dtDetails . Rows [ i ] [ 3 ] ) ,
						Balance = Convert . ToDecimal ( dtDetails . Rows [ i ] [ 4 ] ) ,
						IntRate = Convert . ToDecimal ( dtDetails . Rows [ i ] [ 5 ] ) ,
						ODate = Convert . ToDateTime ( dtDetails . Rows [ i ] [ 6 ] ) ,
						CDate = Convert . ToDateTime ( dtDetails . Rows [ i ] [ 7 ] )
					} );
					count = i;
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"DETAILS : ERROR in  LoadDetCollection() : loading Details into ObservableCollection \"DetCollection\" : [{ex . Message}] : {ex . Data} ...." );
				MessageBox . Show ( $"DETAILS : ERROR in  LoadDetCollection() : loading Details into ObservableCollection \"DetCollection\" : [{ex . Message}] : {ex . Data} ...." );
				return null;
			}
			finally
			{
				if ( Notify )
				{
					EventControl . TriggerDetDataLoaded ( null ,
						new LoadedEventArgs
						{
							CallerType = "SQLSERVER" ,
							CallerDb = Caller ,
							DataSource = internalcollection ,
							RowCount = internalcollection . Count
						} );
				}
			}
			return collection;
		}

		#region EXPORT FUNCTIONS TO READ/WRITE CSV files
		public static DataTable LoadDetailsExportData ( )
		{
			DataTable dt = new DataTable ( );
			SqlConnection con;
			string ConString = "";
			string commandline = "";
			ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			Debug . WriteLine ( $"Making new SQL connection in DETAILSCOLLECTION" );
			con = new SqlConnection ( ConString );
			try
			{
				Debug . WriteLine ( $"Using new SQL connection in DETAILSCOLLECTION" );
				using ( con )
				{
					if ( Flags . IsMultiMode )
					{
						//	// Create a valid Query Command string including any active sort ordering
						//	commandline = $"SELECT * FROM SECACCOUNTS WHERE CUSTNO IN "
						//		+ $"(SELECT CUSTNO FROM SECACCOUNTS  "
						//		+ $" GROUP BY CUSTNO"
						//		+ $" HAVING COUNT(*) > 1) ORDER BY ";
						//	commandline = Utils . GetDataSortOrder ( commandline );
						//}
						//else if ( Flags . FilterCommand != "" )
						//{
						//	commandline = Flags . FilterCommand;
					}
					else
					{
						// Create a valid Query Command string including any active sort ordering
						commandline = "Select * from SecAccounts  order by ";
						commandline = Utils . GetDataSortOrder ( commandline );
					}
					SqlCommand cmd = new SqlCommand ( commandline, con );
					SqlDataAdapter sda = new SqlDataAdapter ( cmd );
					sda . Fill ( dt );
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"DETAILS : ERROR in LoadDetailsDataSql(): Failed to load Details Details - {ex . Message}, {ex . Data}" );
				MessageBox . Show ( $"DETAILS : ERROR in LoadDetailsDataSql(): Failed to load Details Details - {ex . Message}, {ex . Data}" );
			}
			finally
			{
				con . Close ( );
			}
			return dt;
		}

		public static int ExportDetData ( string path , string dbType )
		{
			int count = 0;
			string output = "";

			// Read data in from disk first as a DataTable dt
			DataSet ds = new DataSet ( );

			DataTable dt = new DataTable ( );
			dt = LoadDetailsExportData ( );
			ds . Tables . Add ( dt );
			//£££££££££££££££££££££££££££££££££££££££££££
			// This works just fine with no external binding.
			// The data is  now accessible in ds.Tables[0].Rows
			// NB DATA ACCESS FORMAT IS [ $"{objRow["CustNo"]}"  ]
			//££££££££££££££££££££££££££££££££££££££££££££
			Console . WriteLine ( $"Writing results of SQL enquiry to {path} ..." );
			foreach ( DataRow objRow in ds . Tables [ 0 ] . Rows )
			{
				output += ParseDbRow ( "DETAILS" , objRow );
				count++;
			}
			System . IO . File . WriteAllText ( path , output );
			Console . WriteLine ( $"Export of {count - 1} records from the [ {dbType} ] Db has been completed successfully." );
			return count;
		}

		//===============================================================================
		/// <summary>
		/// Method to output CSV data correctly formatted
		/// </summary>
		/// <param name="dbType"></param>
		/// <param name="objRow"></param>
		/// <returns></returns>
		public static string ParseDbRow ( string dbType , DataRow objRow )
		{
			string tmp = "", s = "";
			string [ ] odat, cdat, revstr;
			if ( dbType == "DETAILS" )
			{
				char [ ] ch = { ' ' };
				char [ ] ch2 = { '/' };
				s = $"{objRow [ "Odate" ] . ToString ( )}', '";
				odat = s . Split ( ch );
				string odate = odat [ 0 ];
				// now reverse it  to YYYY/MM/DD format as this is what SQL understands
				revstr = odate . Split ( ch2 );
				odate = revstr [ 2 ] + "/" + revstr [ 1 ] + "/" + revstr [ 0 ];
				// thats  the Open date handled - now do close data
				s = $"{objRow [ "cDate" ] . ToString ( )}', '";
				cdat = s . Split ( ch );   // split date on '/'
				string cdate = cdat [ 0 ];
				// now reverse it  to YYYY/MM/DD format as this is what SQL understands
				revstr = cdate . Split ( ch2 );
				cdate = revstr [ 2 ] + "/" + revstr [ 1 ] + "/" + revstr [ 0 ];
				string acTypestr = objRow [ "AcType" ] . ToString ( ) . Trim ( );

				// Formats data correctly for CSV output, with single quotes around dates converted to YYYY/MM/DD format as required by SQL Server
				tmp = $"{objRow [ "Id" ] . ToString ( )}, "
					+ $"{objRow [ "BankNo" ] . ToString ( )}, "
					+ $"{objRow [ "CustNo" ] . ToString ( )}, "
					+ $"{acTypestr},"
					+ $"{objRow [ "Balance" ] . ToString ( )}, "
					+ $"{objRow [ "Intrate" ] . ToString ( )}, "
					+ $"'{odate}', "
					+ $"'{cdate}'\r\n";
			}
			return tmp;
		}
		#endregion EXPORT FUNCTIONS TO READ/WRITE CSV files
	}
}
