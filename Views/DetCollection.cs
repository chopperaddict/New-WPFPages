﻿using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Data;
using WPFPages . Properties;
using WPFPages . ViewModels;

// Used By SqlDbViewer
namespace WPFPages . Views
{
	public class DetCollection : ObservableCollection<DetailsViewModel>
	{
		//Declare a global pointer to Observable Details Collection
		public static DetCollection Detcollection = new DetCollection ( );
		public static DetCollection DetViewerDbcollection = new DetCollection ( );
		public static DetCollection EditDbDetcollection = new DetCollection ( );
		public static DetCollection MultiDetcollection = new DetCollection ( );
		public static DetCollection SqlViewerDetcollection = new DetCollection ( );
		public static DetCollection Detinternalcollection = new DetCollection ( );

		public static DataTable dtDetails = new DataTable ( );
		public Stopwatch st;
		public static bool USEFULLTASK = true;
		public static bool Notify = false;

		private readonly object LockDetReadData = new object ( );
		private readonly object LockDetLoadData = new object ( );




		#region CONSTRUCTOR
		/// <summary>
		/// Get a View
		/// </summary>
//		private ICollectionView _DetviewerView;
		//public ICollectionView DetViewerView
		//{
		//	get { return _DetviewerView; }
		//}

		//public int CurrentItem { get; internal set; }
		//public Action CurrentChanged { get; internal set; }

		public DetCollection ( )
		{
		}

		#endregion CONSTRUCTOR

		public async static Task<DetCollection> LoadDet ( DetCollection dc, int ViewerType = 1, bool NotifyAll = false )
		{
			Notify = NotifyAll;
			// Want to see if we ever get here
			Debugger . Break ();
			try
			{

				if ( USEFULLTASK )
				{
					DetCollection Detinternalcollection = new DetCollection ( );
					await Detinternalcollection . LoadDetailsTaskInSortOrderAsync ( );
					return ( DetCollection ) null;// Detinternalcollection;
				}
				else
				{
					//				Debug . WriteLine ( $"\n ***** Loading Details Data from disk (using Abbreviated AWAIT Control system)*****\n" );
					DetCollection d = new DetCollection ( );
					await d . LoadDetailsDataSql ( ) . ConfigureAwait ( false );
					if ( dtDetails . Rows . Count > 0 )
						await LoadDetTest ( Detinternalcollection ) . ConfigureAwait ( false );

					// We now have the ONE AND ONLY pointer the the Bank data in variable Bankcollection
					Flags . DetCollection = Detinternalcollection;
					if ( Flags . IsMultiMode == false )
					{
						// Finally fill and return The global Dataset
						SelectViewer ( ViewerType, Detinternalcollection );
						return null;// Detinternalcollection;
					}
					else
					{
						// return the "working  copy" pointer, it has  filled the relevant collection to match the viewer
						return null; // Detinternalcollection;
					}
				}


				//Debug . WriteLine ( $"\n ***** Loading Details Data from disk *****\n" );
				//await ProcessRequest ( ViewerType );

				//if ( Flags . IsMultiMode == false )
				//{
				//	DetCollection db = new DetCollection ( );
				//	SelectViewer ( ViewerType, Detcollection );
				//	return db;
				//}
				//else
				//{
				//	// return the "working  copy" pointer, it has  filled the relevant collection to match the viewer
				//	return null;
				//}
			}
			catch ( Exception ex )
			{
				Console . WriteLine ( $"Details  Load Exception : {ex . Message}, {ex . Data}" );
				return null;
			}
		}

		/// <summary>
		/// Controls the way we load the Details data
		/// </summary>
		/// <param name="ViewerType"></param>
		/// <returns></returns>
		private static async Task<DetCollection> ProcessRequest ( int ViewerType )
		{
			if ( USEFULLTASK )
			{
				DetCollection Detinternalcollection = new DetCollection ( );
				await Detinternalcollection . LoadDetailsTaskInSortOrderAsync ( );
				return (DetCollection)null;// Detinternalcollection;
			}
			else
			{
				//				Debug . WriteLine ( $"\n ***** Loading Details Data from disk (using Abbreviated AWAIT Control system)*****\n" );
				DetCollection d = new DetCollection ( );
				await d . LoadDetailsDataSql ( ) . ConfigureAwait ( false );
				if ( dtDetails . Rows . Count > 0 )
					await LoadDetTest ( Detinternalcollection ) . ConfigureAwait ( false );

				// We now have the ONE AND ONLY pointer the the Bank data in variable Bankcollection
				Flags . DetCollection = Detinternalcollection;
				if ( Flags . IsMultiMode == false )
				{
					// Finally fill and return The global Dataset
					SelectViewer ( ViewerType, Detinternalcollection );
					return null;// Detinternalcollection;
				}
				else
				{
					// return the "working  copy" pointer, it has  filled the relevant collection to match the viewer
					return null; // Detinternalcollection;
				}
			}
		}

		#region startup/load data / load collection (Detinternalcollection)

		// Entry point for all data load/Reload
		CancellationTokenSource cts = new CancellationTokenSource ( );

		//**************************************************************************************************************************************************************//
		public async Task<DetCollection> LoadDetailsTaskInSortOrderAsync ( bool b = false, int row = 0, bool notifyAll = false )
		{
			Notify = notifyAll;

			if ( dtDetails . Rows . Count > 0 )
				dtDetails . Clear ( );

			if ( Detinternalcollection . Items . Count > 0 )
				Detinternalcollection . ClearItems ( );

			#region process code to load data

			Task t1 = Task . Run (
				async ( ) =>
					{
						//							Debug . WriteLine ( $"Before starting initial Task.Run() : Thread = { Thread . CurrentThread . ManagedThreadId}" );
						await LoadDetailsDataSql ( b );
						//							Debug . WriteLine ( $"After initial Task.Run() : Thread = { Thread . CurrentThread . ManagedThreadId}" );
					}
				);
			t1 . ContinueWith
			(
				async ( Detinternalcollection ) =>
				{
					await LoadDetCollection ( row, b );
				}, TaskScheduler . FromCurrentSynchronizationContext ( )
			 );

			#endregion process code to load data

			#region Success//Error reporting/handling

			// Now handle "post processing of errors etc"
			//This will ONLY run if there were No Exceptions  and it ALL ran successfully!!
			t1 . ContinueWith (
				( Detinternalcollection ) =>
				{
					Debug . WriteLine ( $"DETAILS : Task.Run() Completed : Status was [ {Detinternalcollection . Status} ]." );
				}, CancellationToken . None, TaskContinuationOptions . OnlyOnRanToCompletion, TaskScheduler . FromCurrentSynchronizationContext ( )
			);
			//This will iterate through ALL of the Exceptions that may have occured in the previous Tasks
			// but ONLY if there were any Exceptions !!
			t1 . ContinueWith (
				( Detinternalcollection ) =>
				{
					AggregateException ae = t1 . Exception . Flatten ( );
					Debug . WriteLine ( $"Exception in DetCollection data processing \n" );
					foreach ( var item in ae . InnerExceptions )
					{
						Debug . WriteLine ( $"DetCollection : Exception : {item . Message}, : {item . Data}" );
					}
				}, CancellationToken . None, TaskContinuationOptions . NotOnRanToCompletion, TaskScheduler . FromCurrentSynchronizationContext ( )
			);

			//			Debug . WriteLine ($"DETAILS : END OF PROCESSING & Error checking functionality\nDETAILS : *** Detcollection total = {Detcollection.Count} ***\n\n");
			#endregion Success//Error reporting/handling

			Flags . DetCollection = Detinternalcollection;
			return Detinternalcollection;
		}

		/// Handles the actual conneciton ot SQL to load the Details Db data required
		/// </summary>
		/// <returns></returns>
		//**************************************************************************************************************************************************************//
		public async Task<bool> LoadDetailsDataSql ( bool isMultiMode = false )
		{
//			Stopwatch st = new Stopwatch ( );

			try
			{
//				st . Start ( );
				SqlConnection con;
				string ConString = "";
				string commandline = "";
				ConString = ( string ) Settings . Default [ "BankSysConnectionString" ];
				con = new SqlConnection ( ConString );
				using ( con )
				{
					if ( Flags . IsMultiMode )
					{
						// Create a valid Query Command string including any active sort ordering
						commandline = $"SELECT * FROM SECACCOUNTS WHERE CUSTNO IN "
							+ $"(SELECT CUSTNO FROM SECACCOUNTS  "
							+ $" GROUP BY CUSTNO"
							+ $" HAVING COUNT(*) > 1) ORDER BY ";
						commandline = Utils . GetDataSortOrder ( commandline );
					}
					else if ( Flags . FilterCommand != "" )
					{
						commandline = Flags . FilterCommand;
					}
					else
					{
						// Create a valid Query Command string including any active sort ordering
						commandline = "Select * from SecAccounts  order by ";
						commandline = Utils . GetDataSortOrder ( commandline );
					}
					SqlCommand cmd = new SqlCommand ( commandline, con );
					SqlDataAdapter sda = new SqlDataAdapter ( cmd );

					DetCollection bptr = new DetCollection ( );
					//					lock ( bptr . LockDetReadData )
					//					{
					sda . Fill ( dtDetails );
					//					}
//					st . Stop ( );
//					//					Debug . WriteLine ( $"DETAILS : Sql data loaded  [{dtDetails . Rows . Count}] row(s) into Details DataTable in {( double ) st . ElapsedMilliseconds / ( double ) 1000}...." );
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"DETAILS : ERROR in LoadDetailsDataSql(): Failed to load Details Details - {ex . Message}, {ex . Data}" );
				MessageBox . Show ( $"DETAILS : ERROR in LoadDetailsDataSql(): Failed to load Details Details - {ex . Message}, {ex . Data}" );
				return false;
			}
			return true;
		}

		//**************************************************************************************************************************************************************//
		public static async Task<DetCollection> LoadDetCollection ( int row, bool DoNotify = true )
		{
			int count = 0;
//			DetCollection bptr = new DetCollection ( );
			//			lock ( bptr . LockDetLoadData )
			//			{
			try
			{
				for ( int i = 0 ; i < dtDetails . Rows . Count ; i++ )
				{
					Detinternalcollection . Add ( new DetailsViewModel
					{
						Id = Convert . ToInt32 ( dtDetails . Rows [ i ] [ 0 ] ),
						BankNo = dtDetails . Rows [ i ] [ 1 ] . ToString ( ),
						CustNo = dtDetails . Rows [ i ] [ 2 ] . ToString ( ),
						AcType = Convert . ToInt32 ( dtDetails . Rows [ i ] [ 3 ] ),
						Balance = Convert . ToDecimal ( dtDetails . Rows [ i ] [ 4 ] ),
						IntRate = Convert . ToDecimal ( dtDetails . Rows [ i ] [ 5 ] ),
						ODate = Convert . ToDateTime ( dtDetails . Rows [ i ] [ 6 ] ),
						CDate = Convert . ToDateTime ( dtDetails . Rows [ i ] [ 7 ] )
					} );
					count = i;
				}
				//				Debug . WriteLine ( $"DETAILS : Sql data loaded into Details ObservableCollection \"Detinternalcollection\" [{count}] ...." );
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
					//					OnDetDataLoaded ( Detcollection , row );
					EventControl . TriggerDetDataLoaded ( null,
						new LoadedEventArgs
						{
							CallerType = "SQLDETAILSSERVER",
							CallerDb = "DETAILS",
							DataSource = Detinternalcollection,
							RowCount = Detinternalcollection . Count
						} );
				}
				///					Flags . DetCollection = Detinternalcollection;
			}
			return Detinternalcollection;
			//			}
		}

		public static async Task<DetCollection> LoadDetTest ( DetCollection Detinternalcollection )
		{
			int count = 0;
			try
			{
				for ( int i = 0 ; i < dtDetails . Rows . Count ; i++ )
				{
					Detinternalcollection . Add ( new DetailsViewModel
					{
						Id = Convert . ToInt32 ( dtDetails . Rows [ i ] [ 0 ] ),
						BankNo = dtDetails . Rows [ i ] [ 1 ] . ToString ( ),
						CustNo = dtDetails . Rows [ i ] [ 2 ] . ToString ( ),
						AcType = Convert . ToInt32 ( dtDetails . Rows [ i ] [ 3 ] ),
						Balance = Convert . ToDecimal ( dtDetails . Rows [ i ] [ 4 ] ),
						IntRate = Convert . ToDecimal ( dtDetails . Rows [ i ] [ 5 ] ),
						ODate = Convert . ToDateTime ( dtDetails . Rows [ i ] [ 6 ] ),
						CDate = Convert . ToDateTime ( dtDetails . Rows [ i ] [ 7 ] )
					} );
					count = i;
				}
				Debug . WriteLine ( $"DETAILS : Sql data loaded into Details ObservableCollection \"DetCollection\" [{count}] ...." );
				Flags . DetCollection = Detinternalcollection;
				return Detinternalcollection;
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"DETAILS : ERROR in  LoadDetCollection() : loading Details into ObservableCollection \"DetCollection\" : [{ex . Message}] : {ex . Data} ...." );
				MessageBox . Show ( $"DETAILS : ERROR in  LoadDetCollection() : loading Details into ObservableCollection \"DetCollection\" : [{ex . Message}] : {ex . Data} ...." );
				return null;
			}
		}
		//**************************************************************************************************************************************************************//

		#endregion startup/load data / load collection (Detinternalcollection)
		public static bool SelectViewer ( int ViewerType, DetCollection tmp )
		{
			bool result = false;
			switch ( ViewerType )
			{
				case 1:
					SqlViewerDetcollection = tmp;
					result = true;
					break;
				case 2:
					EditDbDetcollection = tmp;
					result = true;
					break;
				case 3:
					MultiDetcollection = tmp;
					result = true;
					break;
				case 4:
					DetViewerDbcollection = tmp;
					result = true;
					break;
				//case 5:
				//	CustViewerDbcollection = tmp;
				//	result = true;
				//	break;
				//case 6:
				//	DetViewerDbcollection = tmp;
				//	result = true;
				//	break;
				//case 7:
				//	SqlViewerCustcollection = tmp;
				//	result = true;
				//	break;
				//case 8:
				//	SqlViewerDetcollection = tmp;
				//	result = true;
				//	break;
				case 9:
					//					= tmp;
					result = true;
					break;
			}
			return result;
		}
		public static bool UpdateDetailsDb ( DetailsViewModel sa , string CallerType)
		{

			SqlConnection con;
			string ConString = "";
			ConString = ( string ) Settings . Default [ "BankSysConnectionString" ];
			//			@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = 'C:\USERS\IANCH\APPDATA\LOCAL\MICROSOFT\MICROSOFT SQL SERVER LOCAL DB\INSTANCES\MSSQLLOCALDB\IAN1.MDF'; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
			con = new SqlConnection ( ConString );
			try
			{
				//We need to update BOTH BankAccount AND DetailsViewModel to keep them in parallel
				using ( con )
				{
					con . Open ( );

					if (CallerType == "CUSTOMER")
					{
						SqlCommand cmd = new SqlCommand("UPDATE SecAccounts SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype,  " +
						"ODATE=@odate, CDATE=@cdate where CUSTNO = @custno AND BANKNO = @bankno", con);
						cmd.Parameters.AddWithValue("@id", Convert.ToInt32(sa.Id));
						cmd.Parameters.AddWithValue("@bankno", sa.BankNo.ToString());
						cmd.Parameters.AddWithValue("@custno", sa.CustNo.ToString());
						cmd.Parameters.AddWithValue("@actype", Convert.ToInt32(sa.AcType));
						cmd.Parameters.AddWithValue("@odate", Convert.ToDateTime(sa.ODate));
						cmd.Parameters.AddWithValue("@cdate", Convert.ToDateTime(sa.CDate));
						cmd.ExecuteNonQuery();
						Debug.WriteLine("SQL Update of SecAccounts successful...");
					}
					else
					{
						SqlCommand cmd = new SqlCommand("UPDATE SecAccounts SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, BALANCE=@balance, " +
						"INTRATE=@intrate, ODATE=@odate, CDATE=@cdate where CUSTNO = @custno AND BANKNO = @bankno", con);
						cmd.Parameters.AddWithValue("@id", Convert.ToInt32(sa.Id));
						cmd.Parameters.AddWithValue("@bankno", sa.BankNo.ToString());
						cmd.Parameters.AddWithValue("@custno", sa.CustNo.ToString());
						cmd.Parameters.AddWithValue("@actype", Convert.ToInt32(sa.AcType));
						cmd.Parameters.AddWithValue("@balance", Convert.ToDecimal(sa.Balance));
						cmd.Parameters.AddWithValue("@intrate", Convert.ToDecimal(sa.IntRate));
						cmd.Parameters.AddWithValue("@odate", Convert.ToDateTime(sa.ODate));
						cmd.Parameters.AddWithValue("@cdate", Convert.ToDateTime(sa.CDate));
						cmd.ExecuteNonQuery();
						Debug.WriteLine("SQL Update of SecAccounts successful...");
					}

				}
			}
			catch ( Exception ex )
			{ Console . WriteLine ( $"DETAILS Update FAILED : {ex . Message}, {ex . Data}" ); }
			finally
			{
				con . Close ( );
			}
			return true;
		}

		public static bool IsCompleted ( )
		{
			return false;
		}

		public static bool GetResult ( )
		{
			return true;
		}
		#region TEST CODE
		//**************************************************************************************************************************************************************//
		//Test codeonly
		//**************************************************************************************************************************************************************//
		#endregion TEST CODE
	}
}
/*
 * 
				Debug . WriteLine ( $"\nCalling dat aload system in DetCollection  : Thread = { Thread . CurrentThread . ManagedThreadId}" );
				Task t1 = Task . Run( 
					async ( ) =>
						{
//							Debug . WriteLine ( $"Before starting initial Task.Run() : Thread = { Thread . CurrentThread . ManagedThreadId}" );
							await LoadDetailsDataSql();
//							Debug . WriteLine ( $"After initial Task.Run() : Thread = { Thread . CurrentThread . ManagedThreadId}" );
						}
				) . ContinueWith (
					{
//					Debug . WriteLine ( $"Before starting second Task.Run() : Thread = { Thread . CurrentThread . ManagedThreadId}" );
					async ( Detcollection ) => await LoadDetCollection ( ),
						TaskScheduler . FromCurrentSynchronizationContext ( )
//					Debug . WriteLine ( $"After second Task.Run() : Thread = { Thread . CurrentThread . ManagedThreadId}" );
				}
				);
* */