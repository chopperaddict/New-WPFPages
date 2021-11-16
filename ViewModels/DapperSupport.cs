using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Data . Common;
using System . Data . SqlClient;
using System . Data . SqlTypes;
using System . Diagnostics;
using System . Globalization;
using System . Linq;
using System . Reflection;
using System . Security . Policy;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Controls . Primitives;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Markup;
using System . Xml . Linq;

using Dapper;

using WPFPages . Properties;
using WPFPages . ViewModels;
using WPFPages . Views;

namespace WPFPages . ViewModels
{
	/// <summary>
	/// Method to load all Db's via SQL using DAPPER to save complexity
	///  Author : ianch
	/// Created : 10/27/2021 8:12:36 AM
	/// </summary>
	///	Methods to load SQ data using Dapper library foir speed and simpicity
	public static class DapperSupport
	{
		#region Bank Db Data Loading methods

		#region LoadMultiBankAccounts

		#region ASYNC Multi account load method
		/// <summary>
		/// ASYNC version that Loads JUST the Customers that have More than ONE Bank Account with this bank
		/// </summary>
		/// <param name="collection">ObservableCollection<BankAccount></BankAccount></param>
		/// <param name="SqlCommand"></param>
		/// <param name="DbNameToLoad">The Bank Account  to load from</param>
		/// <param name="Notify">Trigger notification or not</param>
		/// <param name="Caller">Our Caller  Window name</param>
		/// <returns></returns>
		public async static Task<bool> GetMultiBankCollectionAsync ( ObservableCollection<BankAccountViewModel> collection ,
			string SqlCommand = "" ,
			string DbNameToLoad = "" ,
			string Orderby = "" ,
			string Conditions = "" ,
			bool Notify = false ,
			string Caller = "" ,
			int [ ] args = null )
		{
			ObservableCollection<BankAccountViewModel> bvmcollection = new ObservableCollection<BankAccountViewModel>();
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			string[] ValidFields=
			{
				"ID",
				"CUSTNO",
				"BANKNO",
				"ACTYPE",
				"INTRATE" ,
				"BALANCE" ,
				"ODATE" ,
				"CDATE"
				};
			string[] errorcolumns;
			using ( IDbConnection db = new SqlConnection ( ConString ) )
			{
				// Utility Support Methods to validate data
				if ( ValidateSortConditionColumns ( ValidFields , "Bank" , Orderby , Conditions , out errorcolumns ) == false )
				{
					if ( Orderby . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
					{
						MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Column name has been \nidentified in the Sorting Clause provided.\n\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}.\n\nTherefore No Sort will be performed for this Db" );
						Orderby = "";
					}
					else if ( Conditions . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
					{
						MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Match clause or Column Name \nhas been identified in the Data Selection Clause.\n\nThe Invalid item identified was :\n\t{errorcolumns [ 0 ]}\n\nTherefore No Data Matching will be performed for this Db" );
						Conditions = "";
					}
					else
					{
						MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but the Loading of the BankAccount Db is being aborted due to \na non existent Column name.\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}" );
						return false;
					}
				}

				// make sure order by clause is correctly formatted
				if ( Orderby . Trim ( ) != "" )
				{
					if ( Orderby . ToUpper ( ) . Contains ( "ORDER BY " ) == false )
					{
						Orderby = " Order by " + Orderby;
					}
				}
				if ( Conditions != "" )
				{
					if ( Conditions . ToUpper ( ) . Contains ( "WHERE" ) == false )
						Conditions = " Where " + Conditions;
				}

				try
				{
					// Use DAPPER to to load Bank data using Stored Procedure
					try
					{
						var Args = new { DbName = "" , Arg = " " , Conditions = "" , SortBy = "" };
						SqlCommand = $"spLoadMultiBankAccountsOnly";
						if ( args [ 2 ] == 0 )
							Args = new { DbName = $" {DbNameToLoad} " , Arg = $" * " , Conditions = $" {Conditions} " , SortBy = $" {Orderby}" };
						else if ( args [ 2 ] > 0 )
							Args = new { DbName = $" {DbNameToLoad} " , Arg = $" Top ({args [ 2 ] . ToString ( )}) *  " , Conditions = $" {Conditions}  " , SortBy = $" {Orderby}" };
						// This syntax WORKS CORRECTLY
						var result  = db . Query<BankAccountViewModel>( SqlCommand , Args,null,false, null,CommandType.StoredProcedure).ToList();

						Console . WriteLine ( result );
						foreach ( var item in result )
						{
							bvmcollection . Add ( item );
						}
						Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {result . Count} records successfuly" );
					}
					catch ( Exception ex )
					{
						Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  FAILED : {ex . Message}" );
					}
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  FAILED : {ex . Message}" );
				}
			}
			EventControl . TriggerBankDataLoaded ( null ,
				new LoadedEventArgs
				{
					CallerType = "DAPPERSUPPORT" ,
					CallerDb = Caller ,
					DataSource = bvmcollection ,
					RowCount = bvmcollection . Count
				} );
			return true;
		}

		#endregion ASYNC Multi account load method

		#region Normal Multi account load method

		/// <summary>
		/// Loads JUST the Customers that have More than ONE Bank Account with this bank
		/// </summary>
		/// <param name="collection">ObservableCollection<BankAccount></BankAccount></param>
		/// <param name="SqlCommand"></param>
		/// <param name="DbNameToLoad">The Bank Account  to load from</param>
		/// <param name="Notify">Trigger notification or not</param>
		/// <param name="Caller">Our Caller  Window name</param>
		/// <returns></returns>
		public static ObservableCollection<BankAccountViewModel> GetMultiBankCollection ( ObservableCollection<BankAccountViewModel> collection ,
			string SqlCommand = "" ,
			string DbNameToLoad = "" ,
			string Orderby = "" ,
			string Conditions = "" ,
			bool Notify = false ,
			string Caller = "" ,
			int [ ] args = null )
		{
			ObservableCollection<BankAccountViewModel> bvmcollection = new ObservableCollection<BankAccountViewModel>();
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			string[] ValidFields=
			{
				"ID",
				"CUSTNO",
				"BANKNO",
				"ACTYPE",
				"INTRATE" ,
				"BALANCE" ,
				"ODATE" ,
				"CDATE"
				};
			string[] errorcolumns;
			using ( IDbConnection db = new SqlConnection ( ConString ) )
			{
				// Utility Support Methods to validate data
				if ( ValidateSortConditionColumns ( ValidFields , "Bank" , Orderby , Conditions , out errorcolumns ) == false )
				{
					if ( Orderby . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
					{
						MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Column name has been \nidentified in the Sorting Clause provided.\n\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}.\n\nTherefore No Sort will be performed for this Db" );
						Orderby = "";
					}
					else if ( Conditions . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
					{
						MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Match clause or Column Name \nhas been identified in the Data Selection Clause.\n\nThe Invalid item identified was :\n\t{errorcolumns [ 0 ]}\n\nTherefore No Data Matching will be performed for this Db" );
						Conditions = "";
					}
					else
					{
						MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but the Loading of the BankAccount Db is being aborted due to \na non existent Column name.\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}" );
						return null;
					}
				}

				// make sure order by clause is correctly formatted
				if ( Orderby . Trim ( ) != "" )
				{
					if ( Orderby . ToUpper ( ) . Contains ( "ORDER BY " ) == false )
					{
						Orderby = " Order by " + Orderby;
					}
				}
				if ( Conditions != "" )
				{
					if ( Conditions . ToUpper ( ) . Contains ( "WHERE" ) == false )
						Conditions = " Where " + Conditions;
				}

				try
				{
					// Use DAPPER to to load Bank data using Stored Procedure
					try
					{
						var Args = new { DbName = "" , Arg = " " , Conditions = "" , SortBy = "" };
						SqlCommand = $"spLoadMultiBankAccountsOnly";
						if ( args [ 2 ] == 0 )
							Args = new { DbName = $" {DbNameToLoad} " , Arg = $" * " , Conditions = $" {Conditions} " , SortBy = $" {Orderby}" };
						else if ( args [ 2 ] > 0 )
							Args = new { DbName = $" {DbNameToLoad} " , Arg = $" Top ({args [ 2 ] . ToString ( )}) *  " , Conditions = $" {Conditions}  " , SortBy = $" {Orderby}" };
						// This syntax WORKS CORRECTLY
						var result  = db . Query<BankAccountViewModel>( SqlCommand , Args,null,false, null,CommandType.StoredProcedure).ToList();

						Console . WriteLine ( result );
						foreach ( var item in result )
						{
							bvmcollection . Add ( item );
						}
						Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {result . Count} records successfuly" );
					}
					catch ( Exception ex )
					{
						Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  FAILED : {ex . Message}" );
					}
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  FAILED : {ex . Message}" );
				}
			}
			if ( Notify )
			{
				EventControl . TriggerBankDataLoaded ( null ,
					new LoadedEventArgs
					{
						CallerType = "SQLSERVER" ,
						CallerDb = Caller ,
						DataSource = bvmcollection ,
						RowCount = bvmcollection . Count
					} );
			}
			return bvmcollection;
		}
		#endregion Normal Multi account load method

		#endregion LoadMultiBankAccounts

		#region ASYNC  BankAccount Data Loading methods

		public async static Task<bool> GetBankObsCollectionAsync ( ObservableCollection<BankAccountViewModel> collection ,
			string SqlCommand = "" ,
			string DbNameToLoad = "" ,
			string Orderby = "" ,
			string Conditions = "" ,
			bool wantSort = false ,
			bool wantDictionary = false ,
			bool Notify = false ,
			string Caller = "" ,
			int [ ] args = null )
		{
			ObservableCollection<BankAccountViewModel> bvmcollection = new ObservableCollection<BankAccountViewModel>();
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			string[] ValidFields=
			{
				"ID",
				"CUSTNO",
				"BANKNO",
				"ACTYPE",
				"INTRATE" ,
				"BALANCE" ,
				"ODATE" ,
				"CDATE"
				};
			string[] errorcolumns;

			// Use defaullt Db if none received frm caller
			if ( DbNameToLoad == "" )
				DbNameToLoad = "BankAccount";

			// Utility Support Methods to validate data
			if ( ValidateSortConditionColumns ( ValidFields , "Bank" , Orderby , Conditions , out errorcolumns ) == false )
			{
				if ( Orderby . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Column name has been \nidentified in the Sorting Clause provided.\n\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}.\n\nTherefore No Sort will be performed for this Db" );
					Orderby = "";
				}
				else if ( Conditions . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Match clause or Column Name \nhas been identified in the Data Selection Clause.\n\nThe Invalid item identified was :\n\t{errorcolumns [ 0 ]}\n\nTherefore No Data Matching will be performed for this Db" );
					Conditions = "";
				}
				else
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but the Loading of the BankAccount Db is being aborted due to \na non existent Column name.\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}" );
					return false;
				}
			}

			// make sure order by clause is correctly formatted
			if ( Orderby . Trim ( ) != "" )
			{
				if ( Orderby . ToUpper ( ) . Contains ( "ORDER BY " ) == false )
				{
					Orderby = " Order by " + Orderby;
				}
			}

			if ( Conditions != "" )
			{
				if ( Conditions . ToUpper ( ) . Contains ( "WHERE" ) == false )
					Conditions = " Where " + Conditions;
			}

			if ( Flags . USEADOWITHSTOREDPROCEDURES )
			{
				//====================================================
				// Use standard ADO.Net to to load Bank data to run Stored Procedure
				//====================================================
				BankAccountViewModel bvm= new BankAccountViewModel();
				string Con = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
				SqlConnection sqlCon=null;

				// Works with default command 31/10/21
				// works with Records limited 31/10/21
				// works with Selection conditions limited 31/10/21
				// works with Sort conditions 31/10/21
				try
				{
					using ( sqlCon = new SqlConnection ( Con ) )
					{
						SqlCommand sql_cmnd;
						sqlCon . Open ( );
						if ( SqlCommand != "" )
							sql_cmnd = new SqlCommand ( SqlCommand , sqlCon );
						else
						{
							sql_cmnd = new SqlCommand ( "dbo.spLoadBankAccountComplex " , sqlCon );

							sql_cmnd . CommandType = CommandType . StoredProcedure;
							// Now handle parameters
							sql_cmnd . Parameters . AddWithValue ( "@DbName" , SqlDbType . NVarChar ) . Value = DbNameToLoad;
							if ( args . Length > 0 )
							{
								if ( args [ 2 ] > 0 )
								{
									string limits = $" Top ({args[2]}) * ";
									sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = limits;
								}
								else
									sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = " * ";
							}
							else
								sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = " * ";

							sql_cmnd . Parameters . AddWithValue ( "@SortBy" , SqlDbType . NVarChar ) . Value = Orderby;
							sql_cmnd . Parameters . AddWithValue ( "@Conditions" , SqlDbType . NVarChar ) . Value = Conditions;
						}
						var sqlDr = sql_cmnd . ExecuteReader ( );
						while ( sqlDr . Read ( ) )
						{
							bvm . Id = int . Parse ( sqlDr [ "ID" ] . ToString ( ) );
							bvm . CustNo = sqlDr [ "CustNo" ] . ToString ( );
							bvm . BankNo = sqlDr [ "BankNo" ] . ToString ( );
							bvm . AcType = int . Parse ( sqlDr [ "ACTYPE" ] . ToString ( ) );
							bvm . Balance = Decimal . Parse ( sqlDr [ "BALANCE" ] . ToString ( ) );
							bvm . IntRate = Decimal . Parse ( sqlDr [ "INTRATE" ] . ToString ( ) );
							bvm . ODate = DateTime . Parse ( sqlDr [ "ODATE" ] . ToString ( ) );
							bvm . CDate = DateTime . Parse ( sqlDr [ "CDATE" ] . ToString ( ) );
							bvmcollection . Add ( bvm );
							bvm = new BankAccountViewModel ( );
						}
						sqlDr . Close ( );
						Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {bvmcollection . Count} records successfuly" );
					}
					sqlCon . Close ( );
				}
				catch ( Exception ex )

				{
					Console . WriteLine ( $"Sql Error, {ex . Message}, {ex . Data}" );
					return false;
				}
			}
			else
			{
				//====================================
				// Use STD DAPPER QUERY to load Bank data
				//====================================
				IEnumerable < BankAccountViewModel> bvmi;
				using ( IDbConnection db = new SqlConnection ( ConString ) )
				{
					try
					{
						// Use DAPPER to to load Bank data using Stored Procedure
						if ( Flags . USEDAPPERWITHSTOREDPROCEDURE )
						{
							try
							{
								var Args = new { DbName = "" , Arg = " " , Conditions = "" , SortBy = "" };
								SqlCommand = $"spLoadBankAccountComplex";
								if ( args [ 2 ] == 0 )
									Args = new { DbName = $"{DbNameToLoad}" , Arg = $" * " , Conditions = $"{Conditions}" , SortBy = $"{ Orderby}" };
								else if ( args [ 2 ] > 0 )
									Args = new { DbName = $"{DbNameToLoad}" , Arg = $"Top ({args [ 2 ] . ToString ( )}) * " , Conditions = $"{Conditions}" , SortBy = $"{Orderby}" };
								// This syntax WORKS CORRECTLY
								var result  = db . Query<BankAccountViewModel>( SqlCommand , Args,null,false, null,CommandType.StoredProcedure).ToList();
								Console . WriteLine ( result );
								foreach ( var item in result )
								{
									bvmcollection . Add ( item );
								}
								Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {result . Count} records successfuly" );
							}
							catch ( Exception ex )
							{
								Console . WriteLine ( $"BANK  DB ERROR : {ex . Message}" );
							}
						}
						else if ( Flags . USESDAPPERSTDPROCEDURES == true )
						{
							//====================================
							// Use standard DAPPER code to load Bank data
							//====================================
							if ( Conditions != "" )
							{
								if ( args [ 2 ] > 0 )
									SqlCommand = $" Select top ({args [ 2 ]}) * from {DbNameToLoad} {Conditions} {Orderby}";
								else
									SqlCommand = $" Select * from {DbNameToLoad} {Conditions} {Orderby}";
							}
							else
							{
								if ( Conditions == "" && Orderby == "" && args [ 0 ] == 0 && args [ 1 ] == 0 && args [ 2 ] == 0 )   // we dont even  have args for total records
									SqlCommand = $" Select * from {DbNameToLoad} ";
								else if ( args [ 0 ] != 0 || args [ 1 ] != 0 || args [ 2 ] != 0 )   // we do have args for total records
								{
									if ( args [ 2 ] == 0 )       // no limit on how many records to get
									{
										SqlCommand = $" Select * from {DbNameToLoad} ";
										if ( Conditions != "" )
											SqlCommand += $" {Conditions} ";
										else if ( args [ 1 ] != 0 )
											SqlCommand += $" where CustNo >= { args [ 0 ]} AND CustNo <= { args [ 1 ]} ";
									}
									else if ( args [ 2 ] > 0 && args [ 1 ] == 0 )
										SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} ";
									else if ( args [ 1 ] > 0 )// All 3 args are received
										SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
									else
										SqlCommand = $" Select * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
								}
								else if ( Conditions != "" )  // We have conditions
									SqlCommand = $"Select * from {DbNameToLoad} {Conditions} ";
								else if ( args == null || args . Length == 0 )    // No args or conditions, so use generic command
									SqlCommand = $"Select * from {DbNameToLoad}  ";
								// Final Trap to ensure we have a valid command line
								if ( SqlCommand == "" )
									SqlCommand = $" Select * from {DbNameToLoad} ";

								if ( wantSort )
									SqlCommand += $" {Orderby}";
							}
							// Read data via Dapper into list<BVM> cos Dapper uses Linq, so we cannot get other types returned
							bvmi = db . Query<BankAccountViewModel> ( SqlCommand );

							foreach ( var item in bvmi )
							{
								bvmcollection . Add ( item );
							}
							collection = bvmcollection;
						}
					}
					catch ( Exception ex )
					{
						Console . WriteLine ( $"SQL DAPPER error : {ex . Message}, {ex . Data}" );
						return false;
					}
					finally
					{
						Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {bvmcollection . Count} records successfuly" );
					}
				}
			}
			EventControl . TriggerBankDataLoaded ( null ,
				new LoadedEventArgs
				{
					CallerType = "SQLSERVER" ,
					CallerDb = Caller ,
					DataSource = bvmcollection ,
					RowCount = bvmcollection . Count
				} );
			return true;
		}
		#endregion ASYNC BankAccount Data Loading methods

		#region Standard BankAccount Data Loading methods

		public static ObservableCollection<BankAccountViewModel> GetBankObsCollection ( ObservableCollection<BankAccountViewModel> collection ,
		string SqlCommand = "" ,
		string DbNameToLoad = "" ,
		string Orderby = "" ,
		string Conditions = "" ,
		bool wantSort = false ,
		bool wantDictionary = false ,
		bool Notify = false ,
		string Caller = "" ,
		int [ ] args = null )
		{
			ObservableCollection<BankAccountViewModel> bvmcollection = new ObservableCollection<BankAccountViewModel>();
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			string[] ValidFields=
			{
				"ID",
				"CUSTNO",
				"BANKNO",
				"ACTYPE",
				"INTRATE" ,
				"BALANCE" ,
				"ODATE" ,
				"CDATE"
				};
			string[] errorcolumns;

			// Use defaullt Db if none received frm caller
			if ( DbNameToLoad == "" )
				DbNameToLoad = "BankAccount";

			// Utility Support Methods to validate data
			if ( ValidateSortConditionColumns ( ValidFields , "Bank" , Orderby , Conditions , out errorcolumns ) == false )
			{
				if ( Orderby . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Column name has been \nidentified in the Sorting Clause provided.\n\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}.\n\nTherefore No Sort will be performed for this Db" );
					Orderby = "";
				}
				else if ( Conditions . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Match clause or Column Name \nhas been identified in the Data Selection Clause.\n\nThe Invalid item identified was :\n\t{errorcolumns [ 0 ]}\n\nTherefore No Data Matching will be performed for this Db" );
					Conditions = "";
				}
				else
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but the Loading of the BankAccount Db is being aborted due to \na non existent Column name.\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}" );
					return null;
				}
			}

			// make sure order by clause is correctly formatted
			if ( Orderby . Trim ( ) != "" )
			{
				if ( Orderby . ToUpper ( ) . Contains ( "ORDER BY " ) == false )
				{
					Orderby = " Order by " + Orderby;
				}
			}

			if ( Conditions != "" )
			{
				if ( Conditions . ToUpper ( ) . Contains ( "WHERE" ) == false )
					Conditions = " Where " + Conditions;
			}

			if ( Flags . USEADOWITHSTOREDPROCEDURES )
			{
				//====================================================
				// Use standard ADO.Net to to load Bank data to run Stored Procedure
				//====================================================
				BankAccountViewModel bvm= new BankAccountViewModel();
				string Con = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
				SqlConnection sqlCon=null;

				// Works with default command 31/10/21
				// works with Records limited 31/10/21
				// works with Selection conditions limited 31/10/21
				// works with Sort conditions 31/10/21
				try
				{
					using ( sqlCon = new SqlConnection ( Con ) )
					{
						SqlCommand sql_cmnd;
						sqlCon . Open ( );
						if ( SqlCommand != "" )
							sql_cmnd = new SqlCommand ( SqlCommand , sqlCon );
						else
						{
							sql_cmnd = new SqlCommand ( "dbo.spLoadBankAccountComplex " , sqlCon );
							sql_cmnd . CommandType = CommandType . StoredProcedure;
							// Now handle parameters
							sql_cmnd . Parameters . AddWithValue ( "@DbName" , SqlDbType . NVarChar ) . Value = DbNameToLoad;
							if ( args . Length > 0 )
							{
								if ( args [ 2 ] > 0 )
								{
									string limits = $" Top ({args[2]}) * ";
									sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = limits;
								}
								else
									sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = " * ";
							}
							else
								sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = " * ";

							sql_cmnd . Parameters . AddWithValue ( "@SortBy" , SqlDbType . NVarChar ) . Value = Orderby;
							sql_cmnd . Parameters . AddWithValue ( "@Conditions" , SqlDbType . NVarChar ) . Value = Conditions;
						}
						var sqlDr = sql_cmnd . ExecuteReader ( );
						while ( sqlDr . Read ( ) )
						{
							bvm . Id = int . Parse ( sqlDr [ "ID" ] . ToString ( ) );
							bvm . CustNo = sqlDr [ "CustNo" ] . ToString ( );
							bvm . BankNo = sqlDr [ "BankNo" ] . ToString ( );
							bvm . AcType = int . Parse ( sqlDr [ "ACTYPE" ] . ToString ( ) );
							bvm . Balance = Decimal . Parse ( sqlDr [ "BALANCE" ] . ToString ( ) );
							bvm . IntRate = Decimal . Parse ( sqlDr [ "INTRATE" ] . ToString ( ) );
							bvm . ODate = DateTime . Parse ( sqlDr [ "ODATE" ] . ToString ( ) );
							bvm . CDate = DateTime . Parse ( sqlDr [ "CDATE" ] . ToString ( ) );
							bvmcollection . Add ( bvm );
							bvm = new BankAccountViewModel ( );
						}
						sqlDr . Close ( );
						Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {bvmcollection . Count} records successfuly" );
					}
					sqlCon . Close ( );
				}
				catch ( Exception ex )

				{
					Console . WriteLine ( $"Sql Error, {ex . Message}, {ex . Data}" );
				}
			}
			else
			{
				//====================================
				// Use STD DAPPER QUERY to load Bank data
				//====================================
				IEnumerable < BankAccountViewModel> bvmi;
				using ( IDbConnection db = new SqlConnection ( ConString ) )
				{
					try
					{
						// Use DAPPER to to load Bank data using Stored Procedure
						if ( Flags . USEDAPPERWITHSTOREDPROCEDURE )
						{
							try
							{
								var Args = new { DbName = "" , Arg = " " , Conditions = "" , SortBy = "" };
								SqlCommand = $"spLoadBankAccountComplex";
								if ( args [ 2 ] == 0 )
									Args = new { DbName = $"{DbNameToLoad}" , Arg = $" * " , Conditions = $"{Conditions}" , SortBy = $"{ Orderby}" };
								else if ( args [ 2 ] > 0 )
									Args = new { DbName = $"{DbNameToLoad}" , Arg = $"Top ({args [ 2 ] . ToString ( )}) * " , Conditions = $"{Conditions}" , SortBy = $"{Orderby}" };
								// This syntax WORKS CORRECTLY
								var result  = db . Query<BankAccountViewModel>( SqlCommand , Args,null,false, null,CommandType.StoredProcedure).ToList();
								Console . WriteLine ( result );
								foreach ( var item in result )
								{
									bvmcollection . Add ( item );
								}
								Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {result . Count} records successfuly" );
							}
							catch ( Exception ex )
							{
								Console . WriteLine ( $"BANK  DB ERROR : {ex . Message}" );
							}
						}
						else if ( Flags . USESDAPPERSTDPROCEDURES == true )
						{
							//====================================
							// Use standard DAPPER code to load Bank data
							//====================================
							if ( Conditions != "" )
							{
								if ( args [ 2 ] > 0 )
									SqlCommand = $" Select top ({args [ 2 ]}) * from {DbNameToLoad} {Conditions} {Orderby}";
								else
									SqlCommand = $" Select * from {DbNameToLoad} {Conditions} {Orderby}";
							}
							else
							{
								if ( Conditions == "" && Orderby == "" && args [ 0 ] == 0 && args [ 1 ] == 0 && args [ 2 ] == 0 )   // we dont even  have args for total records
									SqlCommand = $" Select * from {DbNameToLoad} ";
								else if ( args [ 0 ] != 0 || args [ 1 ] != 0 || args [ 2 ] != 0 )   // we do have args for total records
								{
									if ( args [ 2 ] == 0 )       // no limit on how many records to get
									{
										SqlCommand = $" Select * from {DbNameToLoad} ";
										if ( Conditions != "" )
											SqlCommand += $" {Conditions} ";
										else if ( args [ 1 ] != 0 )
											SqlCommand += $" where CustNo >= { args [ 0 ]} AND CustNo <= { args [ 1 ]} ";
									}
									else if ( args [ 2 ] > 0 && args [ 1 ] == 0 )
										SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} ";
									else if ( args [ 1 ] > 0 )// All 3 args are received
										SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
									else
										SqlCommand = $" Select * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
								}
								else if ( Conditions != "" )  // We have conditions
									SqlCommand = $"Select * from {DbNameToLoad} {Conditions} ";
								else if ( args == null || args . Length == 0 )    // No args or conditions, so use generic command
									SqlCommand = $"Select * from {DbNameToLoad}  ";
								// Final Trap to ensure we have a valid command line
								if ( SqlCommand == "" )
									SqlCommand = $" Select * from {DbNameToLoad} ";

								if ( wantSort )
									SqlCommand += $" {Orderby}";
							}
							// Read data via Dapper into list<BVM> cos Dapper uses Linq, so we cannot get other types returned
							bvmi = db . Query<BankAccountViewModel> ( SqlCommand );

							foreach ( var item in bvmi )
							{
								bvmcollection . Add ( item );
							}
							collection = bvmcollection;
						}
					}
					catch ( Exception ex )
					{
						Console . WriteLine ( $"SQL DAPPER error : {ex . Message}, {ex . Data}" );
					}
					finally
					{
						Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {bvmcollection . Count} records successfuly" );
					}
				}
			}
			if ( Notify )
			{
				EventControl . TriggerBankDataLoaded ( null ,
					new LoadedEventArgs
					{
						CallerType = "SQLSERVER" ,
						CallerDb = Caller ,
						DataSource = bvmcollection ,
						RowCount = bvmcollection . Count
					} );
			}
			collection = bvmcollection;
			return bvmcollection;
		}
		#endregion Standard BankAccount Data Loading methods

		#region BankAccount Data with Dictionary Loading methods

		public static ObservableCollection<BankAccountViewModel> GetBankObsCollectionWithDict ( ObservableCollection<BankAccountViewModel> collection ,
			out Dictionary<int , int> Dict ,
			bool wantDictionary = false ,
			string SqlCommand = "" ,
			string DbNameToLoad = "" ,
			bool wantSort = false ,
			bool Notify = false ,
			string Caller = "" ,
			int [ ] args = null )
		{
			ObservableCollection<BankAccountViewModel> bvmcollection = new ObservableCollection<BankAccountViewModel>();
			List<BankAccountViewModel> bvmlist = new List<BankAccountViewModel>();
			Dictionary <int, int> BankDict = new Dictionary<int, int>();


			Dict = BankDict;
			//collection = bvmcollection;
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			if ( DbNameToLoad == "" )
				DbNameToLoad = "BankAccount";

			using ( IDbConnection db = new SqlConnection ( ConString ) )
			{
				try
				{
					if ( SqlCommand == "" && args != null )   // no command line received, but we do have args
					{
						if ( args [ 2 ] == 0 )       // no limit on how many records to get
							SqlCommand = $" Select * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
						else  // All 3 args are received
							SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
					}
					else if ( SqlCommand == "" && args == null )    // No inforeceived at all, so use generic command
						SqlCommand = "Select * from {DbNameToLoad} ";

					if ( wantSort )
						SqlCommand += $" order by CustNo, BankNo";
					// Read data via Dapper into list<BVM> cos Dapper uses Linq, so we cannot get other types returned
					bvmlist = db . Query<BankAccountViewModel> ( SqlCommand ) . ToList ( );
					if ( bvmlist . Count > 0 )
					{
						// We want a ObservableCollection<BankAccountViewModel>, so create it here, and also a dictionary<int, int>
						int counter = 0;
						foreach ( var item in bvmlist )
						{
							bvmcollection . Add ( item );
							if ( wantDictionary )
							{
								if ( BankDict . ContainsKey ( int . Parse ( item . CustNo ) ) == false )
									BankDict . Add ( int . Parse ( item . CustNo ) , counter++ );
							}
						}
					}
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER error : {ex . Message}, {ex . Data}" );
				}
				finally
				{
					Console . WriteLine ( $"SQL DAPPER {DbNameToLoad} loaded : {bvmcollection . Count} records successfuly" );
				}
			}
			if ( Notify )
			{
				EventControl . TriggerBankDataLoaded ( null ,
					new LoadedEventArgs
					{
						CallerType = "SQLSERVER" ,
						CallerDb = Caller ,
						DataSource = bvmcollection ,
						RowCount = bvmcollection . Count
					} );
			}
			collection = bvmcollection;
			return bvmcollection;
		}
		#endregion BankAccount Data with Dictionary Loading methods

		#region BankAccount Data with Dictionary as List<> Loading methods

		public static ObservableCollection<BankAccountViewModel> LoadBankDataToList (
			ObservableCollection<BankAccountViewModel> BankCollection ,
			out Dictionary<int , int> Dict ,
			bool wantDictionary = false ,
			string SqlCommand = "" ,
			string DbNameToLoad = "" ,
			bool wantSort = false ,
			string Caller = "" ,
			bool NotifyCaller = false ,
			int [ ] args = null )
		{
			ObservableCollection < BankAccountViewModel >DbData = new ObservableCollection<BankAccountViewModel>();
			List<BankAccountViewModel> bvmlist = new List<BankAccountViewModel>();
			Dictionary <int, int> BankDict = new Dictionary<int, int>();
			int counter = 0;

			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			Dict = BankDict;
			if ( BankCollection == null )
				BankCollection = DbData;

			if ( DbNameToLoad == "" )
				DbNameToLoad = "BankAccount";
			using ( IDbConnection db = new SqlConnection ( ConString ) )
			{
				try
				{
					if ( SqlCommand == "" && args != null )   // no command line received, but we do have args
					{
						if ( args [ 2 ] == 0 )       // no limit on how many records to get
							SqlCommand = $" Select * from {DbNameToLoad}  where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
						else  // All 3 args are received
							SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
					}
					else if ( SqlCommand == "" && args == null )    // No inforeceived at all, so use generic command
						SqlCommand = "Select * from {DbNameToLoad} ";

					if ( wantSort )
						SqlCommand += $" order by CustNo, BankNo";

					bvmlist = db . Query<BankAccountViewModel> ( SqlCommand ) . ToList ( );

					if ( bvmlist . Count > 0 )
					{
						foreach ( var item in bvmlist )
						{
							if ( BankDict . ContainsKey ( int . Parse ( item . BankNo ) ) == false )
								BankDict . Add ( int . Parse ( item . BankNo ) , counter++ );
						}
					}
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER error in DAPPERSUPPPORT. LOADBANKDATAVIALIST: {ex . Message}, {ex . Data}" );
				}
				finally
				{
					Console . WriteLine ( $"SQL DAPPER {DbNameToLoad} loaded : {BankCollection . Count} records successfuly" );
				}
			}
			//if ( NotifyCaller )
			//{
			//	EventControl . TriggerBankDataLoaded ( null ,
			//		new LoadedEventArgs
			//		{
			//			CallerType = "SQLSERVER" ,
			//			CallerDb = Caller ,
			//			DataSource = BankCollection ,
			//			RowCount = BankCollection . Count
			//		} );
			//}
			return BankCollection;
		}
		#endregion BankAccount Data with Dictionary as List<> Loading methods

		#endregion Banks Db Data Loading methods

		#region Customer Db Data Loading methods

		#region  ASYNC  Load Customer Db Standard (obsCollection) Method

		public async static Task<bool> GetCustObsCollectionAsync ( ObservableCollection<CustomerViewModel> collection ,
			string SqlCommand = "" ,
			string DbNameToLoad = "" ,
			string Orderby = "" ,
			string Conditions = "" ,
			bool wantSort = false ,
			bool wantDictionary = false ,
			bool Notify = false ,
			string Caller = "" ,
			int [ ] args = null )

		{
			ObservableCollection<CustomerViewModel> cvmcollection = new ObservableCollection<CustomerViewModel>();
			IEnumerable<CustomerViewModel> cvm ;
			string[] ValidFields=
			{
				"ID",
				"CUSTNO",
				"BANKNO",
				"ACTYPE",
				"FNAME" ,
				"LNAME" ,
				"ADDR1" ,
				"ADDR2" ,
				"TOWN" ,
				"COUNTY",
				"PCODE" ,
				"PHONE" ,
				"MOBILE",
				"ODATE" ,
				"CDATE"
				};
			string[] errorcolumns;
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];

			if ( DbNameToLoad == "" )
				DbNameToLoad = "Customer";


			// Utility Support Methods to validate data
			if ( ValidateSortConditionColumns ( ValidFields , "Bank" , Orderby , Conditions , out errorcolumns ) == false )
			{
				if ( Orderby . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Column name has been \nidentified in the Sorting Clause provided.\n\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}.\n\nTherefore No Sort will be performed for this Db" );
					Orderby = "";
				}
				else if ( Conditions . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Match clause or Column Name \nhas been identified in the Data Selection Clause.\n\nThe Invalid item identified was :\n\t{errorcolumns [ 0 ]}\n\nTherefore No Data Matching will be performed for this Db" );
					Conditions = "";
				}
				else
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but the Loading of the BankAccount Db is being aborted due to \na non existent Column name.\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}" );
					return false;
				}
			}

			// make sure order by clause is correctly formatted
			if ( Orderby . Trim ( ) != "" )
			{
				if ( Orderby . ToUpper ( ) . Contains ( "ORDER BY " ) == false )
				{
					Orderby = " Order by " + Orderby;
				}
			}

			if ( Conditions != "" )
			{
				if ( Conditions . ToUpper ( ) . Contains ( "WHERE" ) == false )
					Conditions = " Where " + Conditions;
			}
			if ( Flags . GETMULTIACCOUNTS )
			{

			}
			if ( Flags . USEADOWITHSTOREDPROCEDURES )
			{
				//====================================================
				// Use standard ADO.Net to to load Bank data to run Stored Procedure
				//====================================================
				CustomerViewModel cvmi = new CustomerViewModel ( );
				string Con = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
				SqlConnection sqlCon=null;

				// Works with default command 31/10/21
				// works with Records limited 31/10/21
				// works with Selection conditions limited 31/10/21
				// works with Sort conditions 31/10/21
				try
				{
					using ( sqlCon = new SqlConnection ( Con ) )
					{
						SqlCommand sql_cmnd;
						sqlCon . Open ( );
						if ( SqlCommand != "" )
							sql_cmnd = new SqlCommand ( SqlCommand , sqlCon );
						else
						{
							sql_cmnd = new SqlCommand ( "dbo.spLoadCustomersComplex " , sqlCon );
							sql_cmnd . CommandType = CommandType . StoredProcedure;
							sql_cmnd . Parameters . AddWithValue ( "@DbName" , SqlDbType . NVarChar ) . Value = DbNameToLoad;
							if ( args . Length > 0 )
							{
								if ( args [ 2 ] > 0 )
								{
									string limits = $" Top ({args[2]}) * ";
									sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = limits;
								}
								else
									sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = " * ";
							}
							else
								sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = " * ";

							sql_cmnd . Parameters . AddWithValue ( "@SortBy" , SqlDbType . NVarChar ) . Value = Orderby;
							sql_cmnd . Parameters . AddWithValue ( "@Conditions" , SqlDbType . NVarChar ) . Value = Conditions;
						}
						// Handle  max records, if any
						var sqlDr = sql_cmnd . ExecuteReader ( );
						while ( sqlDr . Read ( ) )
						{
							cvmi . Id = int . Parse ( sqlDr [ "ID" ] . ToString ( ) );
							cvmi . CustNo = sqlDr [ "CUSTNO" ] . ToString ( );
							cvmi . BankNo = sqlDr [ "BANKNO" ] . ToString ( );
							cvmi . AcType = int . Parse ( sqlDr [ "ACTYPE" ] . ToString ( ) );
							cvmi . FName = sqlDr [ "FNAME" ] . ToString ( );
							cvmi . LName = sqlDr [ "LNAME" ] . ToString ( );
							cvmi . Addr1 = sqlDr [ "ADDR1" ] . ToString ( );
							cvmi . Addr2 = sqlDr [ "ADDR2" ] . ToString ( );
							cvmi . Town = sqlDr [ "TOWN" ] . ToString ( );
							cvmi . County = sqlDr [ "COUNTY" ] . ToString ( );
							cvmi . PCode = sqlDr [ "PCODE" ] . ToString ( );
							cvmi . Phone = sqlDr [ "PHONE" ] . ToString ( );
							cvmi . Mobile = sqlDr [ "MOBILE" ] . ToString ( );
							cvmi . ODate = DateTime . Parse ( sqlDr [ "ODATE" ] . ToString ( ) );
							cvmi . CDate = DateTime . Parse ( sqlDr [ "CDATE" ] . ToString ( ) );
							cvmcollection . Add ( cvmi );
							cvmi = new CustomerViewModel ( );
						}
						sqlDr . Close ( );
						Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {cvmcollection . Count} records successfuly" );
					}
					sqlCon . Close ( );
				}
				catch ( Exception ex )

				{
					Console . WriteLine ( $"Sql Error, {ex . Message}, {ex . Data}" );
					return false;
				}
			}
			else
			{
				using ( IDbConnection db = new SqlConnection ( ConString ) )
				{
					if ( Flags . USEDAPPERWITHSTOREDPROCEDURE )
					{
						try
						{
							var Args = new { DbName = "" , Arg = " " , Conditions = "" , SortBy = "" };
							if ( SqlCommand == "" )
							{
								SqlCommand = $"spLoadCustomersComplex";
								if ( args [ 2 ] == 0 )
									Args = new { DbName = $"{DbNameToLoad}" , Arg = $" * " , Conditions = $"{Conditions}" , SortBy = $"{ Orderby}" };
								else if ( args [ 2 ] > 0 )
									Args = new { DbName = $"{DbNameToLoad}" , Arg = $"Top ({args [ 2 ] . ToString ( )}) * " , Conditions = $"{Conditions}" , SortBy = $"{Orderby}" };
								// This syntax WORKS CORRECTLY
							}
							var result  = db . Query<CustomerViewModel>( SqlCommand , Args,null,false, null,CommandType.StoredProcedure).ToList();
							Console . WriteLine ( result );
							foreach ( var item in result )
							{
								cvmcollection . Add ( item );
							}
							Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {result . Count} records successfuly" );
						}
						catch ( Exception ex )
						{
							Console . WriteLine ( $"CUSTOMER DB ERROR : {ex . Message}" );
						}
					}
					else if ( Flags . USESDAPPERSTDPROCEDURES == true )
					{
						try
						{

							if ( Conditions != "" )
							{
								if ( args [ 2 ] > 0 )
									SqlCommand = $" Select top ({args [ 2 ]}) * from {DbNameToLoad} {Conditions} {Orderby}";
								else
									SqlCommand = $" Select * from {DbNameToLoad} {Conditions} {Orderby}";
							}
							else
							{
								if ( Conditions == "" && Orderby == "" && args [ 0 ] == 0 && args [ 1 ] == 0 && args [ 2 ] == 0 )   // we dont even  have args for total records
									SqlCommand = $" Select * from {DbNameToLoad} ";
								else if ( args [ 0 ] != 0 || args [ 1 ] != 0 || args [ 2 ] != 0 )   // we do have args for total records
								{
									if ( args [ 2 ] == 0 )       // no limit on how many records to get
									{
										SqlCommand = $" Select * from {DbNameToLoad} ";
										if ( Conditions != "" )
											SqlCommand += $" {Conditions} ";
										else if ( args [ 1 ] != 0 )
											SqlCommand += $" where CustNo >= { args [ 0 ]} AND CustNo <= { args [ 1 ]} ";
									}
									else if ( args [ 2 ] > 0 && args [ 1 ] == 0 )
										SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} ";
									else if ( args [ 1 ] > 0 )// All 3 args are received
										SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
									else
										SqlCommand = $" Select * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
								}
								else if ( Conditions != "" )  // We have conditions
									SqlCommand = $"Select * from {DbNameToLoad} {Conditions} ";
								else if ( args == null || args . Length == 0 )    // No args or conditions, so use generic command
									SqlCommand = $"Select * from {DbNameToLoad}  ";

								// Final Trap to ensure we have a valid command line
								if ( SqlCommand == "" )
									SqlCommand = $" Select * from {DbNameToLoad} ";

								if ( wantSort )
									SqlCommand += $" {Orderby}";
							}

							cvm = db . Query<CustomerViewModel> ( SqlCommand );

							foreach ( var item in cvm )
							{
								cvmcollection . Add ( item );
							}

						}
						catch ( Exception ex )
						{
							Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  error : {ex . Message}, {ex . Data}" );
							return false;
						}
						finally
						{
							Console . WriteLine ( $"SQL DAPPER {DbNameToLoad} loaded : {cvmcollection . Count} records successfuly" );
						}
					}
				}
			}
			EventControl . TriggerCustDataLoaded ( null ,
				new LoadedEventArgs
				{
					CallerType = "SQLSERVER" ,
					CallerDb = Caller ,
					DataSource = cvmcollection ,
					RowCount = cvmcollection . Count
				} );
			return true;
		}

		#endregion  ASYNC Load Customer Db Standard (obsCollection) Method


		#region  STANDARD Load Customer Db Standard (obsCollection) Method

		public static ObservableCollection<CustomerViewModel> GetCustObsCollection ( ObservableCollection<CustomerViewModel> collection ,
			string SqlCommand = "" ,
			string DbNameToLoad = "" ,
			string Orderby = "" ,
			string Conditions = "" ,
			bool wantSort = false ,
			bool wantDictionary = false ,
			bool Notify = false ,
			string Caller = "" ,
			int [ ] args = null )

		{
			ObservableCollection<CustomerViewModel> cvmcollection = new ObservableCollection<CustomerViewModel>();
			IEnumerable<CustomerViewModel> cvm ;
			string[] ValidFields=
			{
				"ID",
				"CUSTNO",
				"BANKNO",
				"ACTYPE",
				"FNAME" ,
				"LNAME" ,
				"ADDR1" ,
				"ADDR2" ,
				"TOWN" ,
				"COUNTY",
				"PCODE" ,
				"PHONE" ,
				"MOBILE",
				"ODATE" ,
				"CDATE"
				};
			string[] errorcolumns;
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];

			if ( DbNameToLoad == "" )
				DbNameToLoad = "Customer";


			// Utility Support Methods to validate data
			if ( ValidateSortConditionColumns ( ValidFields , "Bank" , Orderby , Conditions , out errorcolumns ) == false )
			{
				if ( Orderby . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Column name has been \nidentified in the Sorting Clause provided.\n\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}.\n\nTherefore No Sort will be performed for this Db" );
					Orderby = "";
				}
				else if ( Conditions . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Match clause or Column Name \nhas been identified in the Data Selection Clause.\n\nThe Invalid item identified was :\n\t{errorcolumns [ 0 ]}\n\nTherefore No Data Matching will be performed for this Db" );
					Conditions = "";
				}
				else
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but the Loading of the BankAccount Db is being aborted due to \na non existent Column name.\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}" );
					return null;
				}
			}

			// make sure order by clause is correctly formatted
			if ( Orderby . Trim ( ) != "" )
			{
				if ( Orderby . ToUpper ( ) . Contains ( "ORDER BY " ) == false )
				{
					Orderby = " Order by " + Orderby;
				}
			}

			if ( Conditions != "" )
			{
				if ( Conditions . ToUpper ( ) . Contains ( "WHERE" ) == false )
					Conditions = " Where " + Conditions;
			}
			if ( Flags . GETMULTIACCOUNTS )
			{

			}
			if ( Flags . USEADOWITHSTOREDPROCEDURES )
			{
				//====================================================
				// Use standard ADO.Net to to load Bank data to run Stored Procedure
				//====================================================
				CustomerViewModel cvmi = new CustomerViewModel ( );
				string Con = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
				SqlConnection sqlCon=null;

				// Works with default command 31/10/21
				// works with Records limited 31/10/21
				// works with Selection conditions limited 31/10/21
				// works with Sort conditions 31/10/21
				try
				{
					using ( sqlCon = new SqlConnection ( Con ) )
					{
						SqlCommand sql_cmnd;
						sqlCon . Open ( );
						if ( SqlCommand != "" )
							sql_cmnd = new SqlCommand ( SqlCommand , sqlCon );
						else
						{
							sql_cmnd = new SqlCommand ( "dbo.spLoadCustomersComplex " , sqlCon );
							sql_cmnd . CommandType = CommandType . StoredProcedure;
							sql_cmnd . Parameters . AddWithValue ( "@DbName" , SqlDbType . NVarChar ) . Value = DbNameToLoad;
							if ( args . Length > 0 )
							{
								if ( args [ 2 ] > 0 )
								{
									string limits = $" Top ({args[2]}) * ";
									sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = limits;
								}
								else
									sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = " * ";
							}
							else
								sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = " * ";

							sql_cmnd . Parameters . AddWithValue ( "@SortBy" , SqlDbType . NVarChar ) . Value = Orderby;
							sql_cmnd . Parameters . AddWithValue ( "@Conditions" , SqlDbType . NVarChar ) . Value = Conditions;
						}
						// Handle  max records, if any
						var sqlDr = sql_cmnd . ExecuteReader ( );
						while ( sqlDr . Read ( ) )
						{
							cvmi . Id = int . Parse ( sqlDr [ "ID" ] . ToString ( ) );
							cvmi . CustNo = sqlDr [ "CUSTNO" ] . ToString ( );
							cvmi . BankNo = sqlDr [ "BANKNO" ] . ToString ( );
							cvmi . AcType = int . Parse ( sqlDr [ "ACTYPE" ] . ToString ( ) );
							cvmi . FName = sqlDr [ "FNAME" ] . ToString ( );
							cvmi . LName = sqlDr [ "LNAME" ] . ToString ( );
							cvmi . Addr1 = sqlDr [ "ADDR1" ] . ToString ( );
							cvmi . Addr2 = sqlDr [ "ADDR2" ] . ToString ( );
							cvmi . Town = sqlDr [ "TOWN" ] . ToString ( );
							cvmi . County = sqlDr [ "COUNTY" ] . ToString ( );
							cvmi . PCode = sqlDr [ "PCODE" ] . ToString ( );
							cvmi . Phone = sqlDr [ "PHONE" ] . ToString ( );
							cvmi . Mobile = sqlDr [ "MOBILE" ] . ToString ( );
							cvmi . ODate = DateTime . Parse ( sqlDr [ "ODATE" ] . ToString ( ) );
							cvmi . CDate = DateTime . Parse ( sqlDr [ "CDATE" ] . ToString ( ) );
							cvmcollection . Add ( cvmi );
							cvmi = new CustomerViewModel ( );
						}
						sqlDr . Close ( );
						Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {cvmcollection . Count} records successfuly" );
					}
					sqlCon . Close ( );
				}
				catch ( Exception ex )

				{
					Console . WriteLine ( $"Sql Error, {ex . Message}, {ex . Data}" );
				}
			}
			else
			{
				using ( IDbConnection db = new SqlConnection ( ConString ) )
				{
					if ( Flags . USEDAPPERWITHSTOREDPROCEDURE )
					{
						try
						{
							var Args = new { DbName = "" , Arg = " " , Conditions = "" , SortBy = "" };
							List<CustomerViewModel>  result = new List<CustomerViewModel>();
							if ( SqlCommand == "" )
							{
								SqlCommand = $"spLoadCustomersComplex";
								if ( args [ 2 ] == 0 )
									Args = new { DbName = $"{DbNameToLoad}" , Arg = $" * " , Conditions = $"{Conditions}" , SortBy = $"{ Orderby}" };
								else if ( args [ 2 ] > 0 )
									Args = new { DbName = $"{DbNameToLoad}" , Arg = $"Top ({args [ 2 ] . ToString ( )}) * " , Conditions = $"{Conditions}" , SortBy = $"{Orderby}" };
								// This syntax WORKS CORRECTLY
							}
							result = db . Query<CustomerViewModel> ( SqlCommand , Args , null , false , null , CommandType . StoredProcedure ) . ToList ( );
							Console . WriteLine ( result );
							foreach ( var item in result )
							{
								cvmcollection . Add ( item );
							}
							Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {result . Count} records successfuly" );
						}
						catch ( Exception ex )
						{
							Console . WriteLine ( $"CUSTOMER DB ERROR : {ex . Message}" );
						}
					}
					else if ( Flags . USESDAPPERSTDPROCEDURES == true )
					{
						try
						{
							if ( SqlCommand == "" )
							{
								if ( Conditions != "" )
								{
									if ( args [ 2 ] > 0 )
										SqlCommand = $" Select top ({args [ 2 ]}) * from {DbNameToLoad} {Conditions} {Orderby}";
									else
										SqlCommand = $" Select * from {DbNameToLoad} {Conditions} {Orderby}";
								}
								else
								{
									if ( Conditions == "" && Orderby == "" && args [ 0 ] == 0 && args [ 1 ] == 0 && args [ 2 ] == 0 )   // we dont even  have args for total records
										SqlCommand = $" Select * from {DbNameToLoad} ";
									else if ( args [ 0 ] != 0 || args [ 1 ] != 0 || args [ 2 ] != 0 )   // we do have args for total records
									{
										if ( args [ 2 ] == 0 )       // no limit on how many records to get
										{
											SqlCommand = $" Select * from {DbNameToLoad} ";
											if ( Conditions != "" )
												SqlCommand += $" {Conditions} ";
											else if ( args [ 1 ] != 0 )
												SqlCommand += $" where CustNo >= { args [ 0 ]} AND CustNo <= { args [ 1 ]} ";
										}
										else if ( args [ 2 ] > 0 && args [ 1 ] == 0 )
											SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} ";
										else if ( args [ 1 ] > 0 )// All 3 args are received
											SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
										else
											SqlCommand = $" Select * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
									}
									else if ( Conditions != "" )  // We have conditions
										SqlCommand = $"Select * from {DbNameToLoad} {Conditions} ";
									else if ( args == null || args . Length == 0 )    // No args or conditions, so use generic command
										SqlCommand = $"Select * from {DbNameToLoad}  ";

									// Final Trap to ensure we have a valid command line
									if ( SqlCommand == "" )
										SqlCommand = $" Select * from {DbNameToLoad} ";

									if ( wantSort )
										SqlCommand += $" {Orderby}";
								}
							}
							cvm = db . Query<CustomerViewModel> ( SqlCommand );

							foreach ( var item in cvm )
							{
								cvmcollection . Add ( item );
							}

						}
						catch ( Exception ex )
						{
							Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  error : {ex . Message}, {ex . Data}" );
						}
						finally
						{
							Console . WriteLine ( $"SQL DAPPER {DbNameToLoad} loaded : {cvmcollection . Count} records successfuly" );
						}
					}
				}
			}
			if ( Notify )
			{
				EventControl . TriggerCustDataLoaded ( null ,
					new LoadedEventArgs
					{
						CallerType = "SQLSERVER" ,
						CallerDb = Caller ,
						DataSource = cvmcollection ,
						RowCount = cvmcollection . Count
					} );
			}
			return cvmcollection;
		}

		#endregion  STANDARD Load Customer Db Standard (obsCollection) Method

		#region Load Customer Db with Dictionary etc (MultiviewViewer only)
		public static ObservableCollection<CustomerViewModel> GetCustObsCollectionWithDict ( ObservableCollection<CustomerViewModel> collection ,
			out Dictionary<int , int> Dict ,
			bool wantDictionary = false ,
			string SqlCommand = "" ,
			string DbNameToLoad = "" ,
			bool wantSort = false ,
			string Caller = "" ,
			bool NotifyCaller = false ,
			int [ ] args = null )

		{
			object  Bankcollection = new object();
			ObservableCollection<CustomerViewModel> DbData = new ObservableCollection<CustomerViewModel>();
			int counter  = 0;
			//			if ( collection == null )
			collection = DbData;
			Dictionary <int, int> CustDict = new Dictionary<int, int>();
			Dict = CustDict;
			List<CustomerViewModel> cvmlist = new List<CustomerViewModel>();
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];

			if ( DbNameToLoad == "" )
				DbNameToLoad = "Customer";

			using ( IDbConnection db = new SqlConnection ( ConString ) )
			{
				try
				{
					if ( SqlCommand == "" && args != null )   // no command line received, but we do have args
					{
						if ( args [ 2 ] == 0 )       // no limit on how many records to get
							SqlCommand = $" Select * from {DbNameToLoad}  where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
						else  // All 3 args are received
							SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
					}
					else if ( SqlCommand == "" && args == null )    // No inforeceived at all, so use generic command
						SqlCommand = $"Select * from {DbNameToLoad} ";

					if ( wantSort && SqlCommand . Contains ( "order by" ) == false )
						SqlCommand += $" order by CustNo, BankNo";

					cvmlist = db . Query<CustomerViewModel> ( SqlCommand ) . ToList ( );

					if ( cvmlist . Count > 0 )
					{
						foreach ( var item in cvmlist )
						{
							DbData . Add ( item );
							if ( CustDict . ContainsKey ( int . Parse ( item . CustNo ) ) == false )
								CustDict . Add ( int . Parse ( item . CustNo ) , counter++ );
						}
						collection = DbData;
					}
					//					Console . WriteLine ( $"SQL DAPPER has loaded : {cvmcollection . Count} Customer  Records" );
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  error : {ex . Message}, {ex . Data}" );
				}
				finally
				{
					Console . WriteLine ( $"SQL DAPPER {DbNameToLoad} loaded : {DbData . Count} records successfuly" );
				}
			}
			// ensure we have the data in obscollection
			collection = DbData;

			if ( NotifyCaller )
			{
				EventControl . TriggerCustDataLoaded ( null ,
					new LoadedEventArgs
					{
						CallerType = "SQLSERVER" ,
						CallerDb = Caller ,
						DataSource = collection ,
						RowCount = collection . Count
					} );
			}
			return collection;
		}
		#endregion Load Cusmoer Db with Dictionary etc (MultiviewViewer only)

		#endregion Customer Db Data Loading methods

		#region Details Db Data Loading methods

		#region ASYNC Details Data Loading methods

		public async static Task<bool> GetDetailsObsCollectionAsync ( ObservableCollection<DetailsViewModel> collection ,
			string SqlCommand = "" ,
			string DbNameToLoad = "" ,
			string Orderby = "" ,
			string Conditions = "" ,
			bool wantSort = false ,
			bool wantDictionary = false ,
			bool Notify = false ,
			string Caller = "" ,
			int [ ] args = null )
		{
			IEnumerable<DetailsViewModel> dvm ;
			ObservableCollection<DetailsViewModel> dvmcollection = new ObservableCollection<DetailsViewModel>();
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];

			string[] ValidFields=
			{
				"ID",
				"CUSTNO",
				"BANKNO",
				"ACTYPE",
				"INTRATE" ,
				"BALANCE" ,
				"ODATE" ,
				"CDATE"
				};
			string[] errorcolumns;

			// Use defaullt Db if none received frm caller
			if ( DbNameToLoad == "" )
				DbNameToLoad = "BankAccount";


			// Utility Support Methods to validate data
			if ( ValidateSortConditionColumns ( ValidFields , "Bank" , Orderby , Conditions , out errorcolumns ) == false )
			{
				if ( Orderby . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Column name has been \nidentified in the Sorting Clause provided.\n\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}.\n\nTherefore No Sort will be performed for this Db" );
					Orderby = "";
				}
				else if ( Conditions . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Match clause or Column Name \nhas been identified in the Data Selection Clause.\n\nThe Invalid item identified was :\n\t{errorcolumns [ 0 ]}\n\nTherefore No Data Matching will be performed for this Db" );
					Conditions = "";
				}
				else
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but the Loading of the BankAccount Db is being aborted due to \na non existent Column name.\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}" );
					return false;
				}
			}

			// make sure order by clause is correctly formatted
			if ( Orderby . Trim ( ) != "" )
			{
				if ( Orderby . ToUpper ( ) . Contains ( "ORDER BY " ) == false )
				{
					Orderby = " Order by " + Orderby;
				}
			}

			if ( Conditions != "" )
			{
				if ( Conditions . ToUpper ( ) . Contains ( "WHERE" ) == false )
					Conditions = " Where " + Conditions;
			}
			if ( DbNameToLoad == "" )
				DbNameToLoad = "SecAccounts";

			if ( Flags . USEADOWITHSTOREDPROCEDURES )
			{
				//====================================================
				// Use standard ADO.Net to to load Bank data to run Stored Procedure
				//====================================================
				DetailsViewModel dvmi = new DetailsViewModel();
				string Con = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
				SqlConnection sqlCon=null;

				// Works with default command 31/10/21
				// works with Records limited 31/10/21
				// works with Selection conditions limited 31/10/21
				// works with Sort conditions 31/10/21
				try
				{
					using ( sqlCon = new SqlConnection ( Con ) )
					{
						SqlCommand sql_cmnd;
						sqlCon . Open ( );
						if ( SqlCommand != "" )
							sql_cmnd = new SqlCommand ( SqlCommand , sqlCon );
						else
						{
							sql_cmnd = new SqlCommand ( "dbo.spLoadDetailsComplex " , sqlCon );
							sql_cmnd . CommandType = CommandType . StoredProcedure;
							sql_cmnd . Parameters . AddWithValue ( "@DbName" , SqlDbType . NVarChar ) . Value = DbNameToLoad;
							if ( args . Length > 0 )
							{
								if ( args [ 2 ] > 0 )
								{
									string limits = $" Top ({args[2]}) * ";
									sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = limits;
								}
								else
									sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = " * ";
							}
							else
								sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = " * ";

							sql_cmnd . Parameters . AddWithValue ( "@SortBy" , SqlDbType . NVarChar ) . Value = Orderby;
							sql_cmnd . Parameters . AddWithValue ( "@Conditions" , SqlDbType . NVarChar ) . Value = Conditions;
						}
						// Handle  max records, if any
						var sqlDr = sql_cmnd . ExecuteReader ( );
						while ( sqlDr . Read ( ) )
						{
							dvmi . Id = int . Parse ( sqlDr [ "ID" ] . ToString ( ) );
							dvmi . CustNo = sqlDr [ "CustNo" ] . ToString ( );
							dvmi . BankNo = sqlDr [ "BankNo" ] . ToString ( );
							dvmi . AcType = int . Parse ( sqlDr [ "ACTYPE" ] . ToString ( ) );
							dvmi . Balance = Decimal . Parse ( sqlDr [ "BALANCE" ] . ToString ( ) );
							dvmi . IntRate = Decimal . Parse ( sqlDr [ "INTRATE" ] . ToString ( ) );
							dvmi . ODate = DateTime . Parse ( sqlDr [ "ODATE" ] . ToString ( ) );
							dvmi . CDate = DateTime . Parse ( sqlDr [ "CDATE" ] . ToString ( ) );
							dvmcollection . Add ( dvmi );
							dvmi = new DetailsViewModel ( );
						}
						sqlDr . Close ( );
						Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {dvmcollection . Count} records successfuly" );
					}
					sqlCon . Close ( );
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"Sql Error, {ex . Message}, {ex . Data}" );
					return false;
				}
			}
			else
			{
				using ( IDbConnection db = new SqlConnection ( ConString ) )
				{
					if ( Flags . USEDAPPERWITHSTOREDPROCEDURE )
					{
						try
						{
							var Args = new { DbName = "" , Arg = " " , Conditions = "" , SortBy = "" };
							if ( SqlCommand == "" )
							{
								SqlCommand = $"spLoadDetailsComplex";
								if ( args [ 2 ] == 0 )
									Args = new { DbName = $"{DbNameToLoad}" , Arg = $" * " , Conditions = $"{Conditions}" , SortBy = $"{ Orderby}" };
								else if ( args [ 2 ] > 0 )
									Args = new { DbName = $"{DbNameToLoad}" , Arg = $"Top ({args [ 2 ] . ToString ( )}) * " , Conditions = $"{Conditions}" , SortBy = $"{Orderby}" };
								// This syntax WORKS CORRECTLY
							}
							var result  = db . Query<DetailsViewModel>( SqlCommand , Args,null,false, null,CommandType.StoredProcedure).ToList();
							Console . WriteLine ( result );
							foreach ( var item in result )
							{
								dvmcollection . Add ( item );
							}
							Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {result . Count} records successfuly" );
						}
						catch ( Exception ex )
						{
							Console . WriteLine ( $"DETAILS DB ERROR : {ex . Message}" );
						}
					}
					else if ( Flags . USESDAPPERSTDPROCEDURES == true )
					{

						try
						{
							if ( SqlCommand == "" )
							{
								if ( Conditions != "" )
								{
									if ( args [ 2 ] > 0 )
										SqlCommand = $" Select top ({args [ 2 ]}) * from {DbNameToLoad} {Conditions} {Orderby}";
									else
										SqlCommand = $" Select * from {DbNameToLoad} {Conditions} {Orderby}";
								}
								else
								{
									if ( Conditions == "" && Orderby == "" && args [ 0 ] == 0 && args [ 1 ] == 0 && args [ 2 ] == 0 )   // we dont even  have args for total records
										SqlCommand = $" Select * from {DbNameToLoad} ";
									else if ( args [ 0 ] != 0 || args [ 1 ] != 0 || args [ 2 ] != 0 )   // we do have args for total records
									{
										if ( args [ 2 ] == 0 )       // no limit on how many records to get
										{
											SqlCommand = $" Select * from {DbNameToLoad} ";
											if ( Conditions != "" )
												SqlCommand += $" {Conditions} ";
											else if ( args [ 1 ] != 0 )
												SqlCommand += $" where CustNo >= { args [ 0 ]} AND CustNo <= { args [ 1 ]} ";
										}
										else if ( args [ 2 ] > 0 && args [ 1 ] == 0 )
											SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} ";
										else if ( args [ 1 ] > 0 )// All 3 args are received
											SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
										else
											SqlCommand = $" Select * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
									}
									else if ( Conditions != "" )  // We have conditions
										SqlCommand = $"Select * from {DbNameToLoad} {Conditions} ";
									else if ( args == null || args . Length == 0 )    // No args or conditions, so use generic command
										SqlCommand = $"Select * from {DbNameToLoad}  ";

									// Final Trap to ensure we have a valid command line
									if ( SqlCommand == "" )
										SqlCommand = $" Select * from {DbNameToLoad} ";

									if ( wantSort )
										SqlCommand += $" {Orderby}";
								}

								// Read data via Dapper into IEnumerable<DbType>
								dvm = db . Query<DetailsViewModel> ( SqlCommand );
								foreach ( var item in dvm )
								{
									dvmcollection . Add ( item );
								}
							}
							else
							{
								// Read data via Dapper into IEnumerable<DbType>
								dvm = db . Query<DetailsViewModel> ( SqlCommand );
								foreach ( var item in dvm )
								{
									dvmcollection . Add ( item );
								}
							}
						}
						catch ( Exception ex )
						{
							Console . WriteLine ( $"SQL DAPPER error : {ex . Message}, {ex . Data}" );
						}
						finally
						{
							Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {dvmcollection . Count} records successfuly" );
						}
					}
				}
			}
			EventControl . TriggerDetDataLoaded ( null ,
					new LoadedEventArgs
					{
						CallerType = "SQLSERVER" ,
						CallerDb = Caller ,
						DataSource = dvmcollection ,
						RowCount = dvmcollection . Count
					} );
			return true;
		}
		#endregion ASYNC Details Data Loading methods

		#region Standard Details Data Loading methods

		public static ObservableCollection<DetailsViewModel> GetDetailsObsCollection ( ObservableCollection<DetailsViewModel> collection ,
			string SqlCommand = "" ,
			string DbNameToLoad = "" ,
			string Orderby = "" ,
			string Conditions = "" ,
			bool wantSort = false ,
			bool wantDictionary = false ,
			bool Notify = false ,
			string Caller = "" ,
			int [ ] args = null )
		{
			IEnumerable<DetailsViewModel> dvm ;
			ObservableCollection<DetailsViewModel> dvmcollection = new ObservableCollection<DetailsViewModel>();
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];

			string[] ValidFields=
			{
				"ID",
				"CUSTNO",
				"BANKNO",
				"ACTYPE",
				"INTRATE" ,
				"BALANCE" ,
				"ODATE" ,
				"CDATE"
				};
			string[] errorcolumns;

			// Use defaullt Db if none received frm caller
			if ( DbNameToLoad == "" )
				DbNameToLoad = "BankAccount";


			// Utility Support Methods to validate data
			if ( ValidateSortConditionColumns ( ValidFields , "Bank" , Orderby , Conditions , out errorcolumns ) == false )
			{
				if ( Orderby . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Column name has been \nidentified in the Sorting Clause provided.\n\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}.\n\nTherefore No Sort will be performed for this Db" );
					Orderby = "";
				}
				else if ( Conditions . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Match clause or Column Name \nhas been identified in the Data Selection Clause.\n\nThe Invalid item identified was :\n\t{errorcolumns [ 0 ]}\n\nTherefore No Data Matching will be performed for this Db" );
					Conditions = "";
				}
				else
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but the Loading of the BankAccount Db is being aborted due to \na non existent Column name.\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}" );
					return null;
				}
			}

			// make sure order by clause is correctly formatted
			if ( Orderby . Trim ( ) != "" )
			{
				if ( Orderby . ToUpper ( ) . Contains ( "ORDER BY " ) == false )
				{
					Orderby = " Order by " + Orderby;
				}
			}

			if ( Conditions != "" )
			{
				if ( Conditions . ToUpper ( ) . Contains ( "WHERE" ) == false )
					Conditions = " Where " + Conditions;
			}
			if ( DbNameToLoad == "" )
				DbNameToLoad = "SecAccounts";

			if ( Flags . USEADOWITHSTOREDPROCEDURES )
			{
				//====================================================
				// Use standard ADO.Net to to load Bank data to run Stored Procedure
				//====================================================
				DetailsViewModel dvmi = new DetailsViewModel();
				string Con = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
				SqlConnection sqlCon=null;

				// Works with default command 31/10/21
				// works with Records limited 31/10/21
				// works with Selection conditions limited 31/10/21
				// works with Sort conditions 31/10/21
				try
				{
					using ( sqlCon = new SqlConnection ( Con ) )
					{
						SqlCommand sql_cmnd;
						sqlCon . Open ( );
						if ( SqlCommand != "" )
							sql_cmnd = new SqlCommand ( SqlCommand , sqlCon );
						else
						{
							sql_cmnd = new SqlCommand ( "dbo.spLoadDetailsComplex " , sqlCon );
							sql_cmnd . CommandType = CommandType . StoredProcedure;
							sql_cmnd . Parameters . AddWithValue ( "@DbName" , SqlDbType . NVarChar ) . Value = DbNameToLoad;
							if ( args . Length > 0 )
							{
								if ( args [ 2 ] > 0 )
								{
									string limits = $" Top ({args[2]}) * ";
									sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = limits;
								}
								else
									sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = " * ";
							}
							else
								sql_cmnd . Parameters . AddWithValue ( "@Arg" , SqlDbType . NVarChar ) . Value = " * ";

							sql_cmnd . Parameters . AddWithValue ( "@SortBy" , SqlDbType . NVarChar ) . Value = Orderby;
							sql_cmnd . Parameters . AddWithValue ( "@Conditions" , SqlDbType . NVarChar ) . Value = Conditions;
						}
						// Handle  max records, if any
						var sqlDr = sql_cmnd . ExecuteReader ( );
						while ( sqlDr . Read ( ) )
						{
							dvmi . Id = int . Parse ( sqlDr [ "ID" ] . ToString ( ) );
							dvmi . CustNo = sqlDr [ "CustNo" ] . ToString ( );
							dvmi . BankNo = sqlDr [ "BankNo" ] . ToString ( );
							dvmi . AcType = int . Parse ( sqlDr [ "ACTYPE" ] . ToString ( ) );
							dvmi . Balance = Decimal . Parse ( sqlDr [ "BALANCE" ] . ToString ( ) );
							dvmi . IntRate = Decimal . Parse ( sqlDr [ "INTRATE" ] . ToString ( ) );
							dvmi . ODate = DateTime . Parse ( sqlDr [ "ODATE" ] . ToString ( ) );
							dvmi . CDate = DateTime . Parse ( sqlDr [ "CDATE" ] . ToString ( ) );
							dvmcollection . Add ( dvmi );
							dvmi = new DetailsViewModel ( );
						}
						sqlDr . Close ( );
						Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {dvmcollection . Count} records successfuly" );
					}
					sqlCon . Close ( );
				}
				catch ( Exception ex )

				{
					Console . WriteLine ( $"Sql Error, {ex . Message}, {ex . Data}" );
				}
			}
			else
			{
				using ( IDbConnection db = new SqlConnection ( ConString ) )
				{
					if ( Flags . USEDAPPERWITHSTOREDPROCEDURE )
					{
						try
						{
							var Args = new { DbName = "" , Arg = " " , Conditions = "" , SortBy = "" };
							if ( SqlCommand == "" )
							{
								SqlCommand = $"spLoadDetailsComplex";
								if ( args [ 2 ] == 0 )
									Args = new { DbName = $"{DbNameToLoad}" , Arg = $" * " , Conditions = $"{Conditions}" , SortBy = $"{ Orderby}" };
								else if ( args [ 2 ] > 0 )
									Args = new { DbName = $"{DbNameToLoad}" , Arg = $"Top ({args [ 2 ] . ToString ( )}) * " , Conditions = $"{Conditions}" , SortBy = $"{Orderby}" };
								// This syntax WORKS CORRECTLY
							}
							var result  = db . Query<DetailsViewModel>( SqlCommand , Args,null,false, null,CommandType.StoredProcedure).ToList();
							Console . WriteLine ( result );
							foreach ( var item in result )
							{
								dvmcollection . Add ( item );
							}
							Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {result . Count} records successfuly" );
						}
						catch ( Exception ex )
						{
							Console . WriteLine ( $"DETAILS DB ERROR : {ex . Message}" );
						}
					}
					else if ( Flags . USESDAPPERSTDPROCEDURES == true )
					{
						try
						{
							if ( SqlCommand == "" )
							{
								if ( Conditions != "" )
								{
									if ( args [ 2 ] > 0 )
										SqlCommand = $" Select top ({args [ 2 ]}) * from {DbNameToLoad} {Conditions} {Orderby}";
									else
										SqlCommand = $" Select * from {DbNameToLoad} {Conditions} {Orderby}";
								}
								else
								{
									if ( Conditions == "" && Orderby == "" && args [ 0 ] == 0 && args [ 1 ] == 0 && args [ 2 ] == 0 )   // we dont even  have args for total records
										SqlCommand = $" Select * from {DbNameToLoad} ";
									else if ( args [ 0 ] != 0 || args [ 1 ] != 0 || args [ 2 ] != 0 )   // we do have args for total records
									{
										if ( args [ 2 ] == 0 )       // no limit on how many records to get
										{
											SqlCommand = $" Select * from {DbNameToLoad} ";
											if ( Conditions != "" )
												SqlCommand += $" {Conditions} ";
											else if ( args [ 1 ] != 0 )
												SqlCommand += $" where CustNo >= { args [ 0 ]} AND CustNo <= { args [ 1 ]} ";
										}
										else if ( args [ 2 ] > 0 && args [ 1 ] == 0 )
											SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} ";
										else if ( args [ 1 ] > 0 )// All 3 args are received
											SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
										else
											SqlCommand = $" Select * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
									}
									else if ( Conditions != "" )  // We have conditions
										SqlCommand = $"Select * from {DbNameToLoad} {Conditions} ";
									else if ( args == null || args . Length == 0 )    // No args or conditions, so use generic command
										SqlCommand = $"Select * from {DbNameToLoad}  ";

									// Final Trap to ensure we have a valid command line
									if ( SqlCommand == "" )
										SqlCommand = $" Select * from {DbNameToLoad} ";

									if ( wantSort )
										SqlCommand += $" {Orderby}";
								}
							}
							// Read data via Dapper into IEnumerable<DbType>
							dvm = db . Query<DetailsViewModel> ( SqlCommand );
							foreach ( var item in dvm )
							{
								dvmcollection . Add ( item );
							}
						}
						catch ( Exception ex )
						{
							Console . WriteLine ( $"SQL DAPPER error : {ex . Message}, {ex . Data}" );
						}
						finally
						{
							Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {dvmcollection . Count} records successfuly" );
						}
					}
				}
			}
			if ( Notify )
			{
				EventControl . TriggerDetDataLoaded ( null ,
					new LoadedEventArgs
					{
						CallerType = "SQLSERVER" ,
						CallerDb = Caller ,
						DataSource = dvmcollection ,
						RowCount = dvmcollection . Count
					} );
			}
			//collection = dvmcollection;
			return dvmcollection;
		}
		#endregion Standard Details Data Loading methods

		#endregion Details Db Data Loading methods

		#region Update Db methods

		#region  Standard Bank Db Update Method

		/// <summary>
		/// Update a complete Db (specifiable) from the datagrid passed in
		/// it also accepts a fully qualified SQLCommand string if required....
		/// </summary>
		/// <param name="dgrid">DataGrid ti update Db from</param>
		/// <param name="DbName">SQL DB name</param>
		/// <param name="SqlCommand">Fully qualified command string</param>
		/// <param name="Conditions">order by params as a string.  Eg : "where Id=@ID</param>
		/// <returns></returns>
		public static bool UpdateBankDb (
			DataGrid dgrid ,
			string DbName = "" ,
			string SqlCommand = "" ,
			string Conditions = "" )
		{
			// Works very well 27/10/21
			bool result = true;
			int indexer = 0;
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			BankAccountViewModel bvm = new BankAccountViewModel();

			if ( DbName == "" )
				DbName = "BankAccount";

			using ( IDbConnection connection = new SqlConnection ( ConString ) )
			{
				try
				{

					for ( int x = 0 ; x < dgrid . Items . Count - 1 ; x++ )
					{
						dgrid . SelectedIndex = x;
						bvm = dgrid . SelectedItem as BankAccountViewModel;
						//This is how to save and use parameters for dapper
						var parameters = new
						{
							id = bvm.Id,
							actype = bvm . AcType,
							intrate = bvm . IntRate,
							balance = bvm . Balance,
							bankno = bvm . BankNo,
							custno = bvm . CustNo,
							odate = bvm . ODate,
							cdate = bvm . CDate
						} ;
						SqlCommand = $" Update  {DbName} set  CustNo=@custno, BankNo =@bankno, AcType = @actype, IntRate=@intrate, Balance=@balance,ODate =@odate, CDate = @cdate where Id=@Id";
						if ( Conditions != "" )
							SqlCommand += Conditions;
						connection . Execute ( @SqlCommand , parameters );
					}

				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER {DbName} Update error : {ex . Message}, {ex . Data}" );
					result = false;

				}
				finally
				{
					if ( result )
						Console . WriteLine ( $"SQL [{DbName . ToUpper ( )}] Db Updated using DAPPER successfuly" );
				}
			}
			return result;
		}
		#endregion  Standard Bank Db Update Method

		#region  Bank Db Record Update Method
		public static bool UpdateSingleBankDb (
			BankAccountViewModel bvm ,
			string DbName = "" ,
			string Conditions = "" )
		{
			// Works very well 27/10/21
			string SqlCommand = "" ;
			bool result = true;
			int indexer = 0;
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];

			if ( DbName == "" )
				DbName = "BankAccount";

			using ( IDbConnection connection = new SqlConnection ( ConString ) )
			{
				try
				{
					//This is how to save and use parameters for dapper
					var parameters =
						new
						{
							id = bvm.Id,
							actype = bvm . AcType,
							intrate = bvm . IntRate,
							balance = bvm . Balance,
							bankno = bvm . BankNo,
							custno = bvm . CustNo,
							odate = bvm . ODate,
							cdate = bvm . CDate
						} ;
					SqlCommand = $" Update  {DbName} set  CustNo=@custno, BankNo =@bankno, AcType = @actype, IntRate=@intrate, Balance=@balance,ODate =@odate, CDate = @cdate ";
					if ( Conditions != "" )
						SqlCommand += Conditions;
					else
						SqlCommand += " where Id =@Id";

					connection . Execute ( @SqlCommand , parameters );
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER {DbName} Update error : {ex . Message}, {ex . Data}" );
					result = false;

				}
				finally
				{
					if ( result )
						Console . WriteLine ( $"SQL [{DbName . ToUpper ( )}] Db record Updated using DAPPER successfuly" );
				}
			}
			return result;
		}
		#endregion  Bank Db Record Update Method

		#region  Standard Customer Db Update Method

		public static bool UpdateCustomersDb (
			DataGrid dgrid ,
			string DbName = "" ,
			string Conditions = "" )
		{
			// Works very well 27/10/21
			string SqlCommand = "" ;
			bool result = true;
			int indexer = 0;
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			CustomerViewModel cvm = new CustomerViewModel ( );

			if ( DbName == "" )
				DbName = "Customer";

			using ( IDbConnection connection = new SqlConnection ( ConString ) )
			{
				try
				{
					for ( int x = 0 ; x < dgrid . Items . Count - 1 ; x++ )
					{
						dgrid . SelectedIndex = x;
						cvm = dgrid . SelectedItem as CustomerViewModel;
						var parameters = new
						{
							id = cvm.Id,
							custno = cvm . CustNo,
							bankno = cvm . BankNo,
							actype = cvm . AcType,
							fname= cvm . FName,
							lname= cvm . LName,
							addr1= cvm . Addr1,
							addr2= cvm . Addr2,
							town= cvm . Town,
							county= cvm . County,
							pcode= cvm . PCode,
							phone= cvm . Phone,
							mobile= cvm . Mobile,
							dob= cvm . Dob,
							odate = cvm . ODate,
							cdate = cvm . CDate
						} ;
						SqlCommand = $" Update  {DbName} set  CustNo=@custno, BankNo =@bankno, AcType = @actype, FName=@fname, LName=@lname, " +
							$" Addr1=@addr1, Addr2=@addr2, Town=@town, County=@County, PCode=@pcode, Phone=@phone, Mobile=@mobile, Dob=@dob, " +
							$"ODate =@odate, CDate = @cdate where Id=@Id ";
						if ( Conditions != "" )
							SqlCommand += Conditions;
						connection . Execute ( @SqlCommand , parameters );
					}
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER {DbName} Update error : {ex . Message}, {ex . Data}" );
					result = false;
				}
				finally
				{
					if ( result )
						Console . WriteLine ( $"SQL [{DbName . ToUpper ( )}] Db Updated using DAPPER successfuly" );
				}
			}
			return result;
		}
		#endregion  Standard Customer Db Update Method

		#region   Customer record Db Update Method

		public static bool UpdateSingleCustomersDb (
			CustomerViewModel cvm ,
			string DbName = "" ,
			string Conditions = "" )
		{
			// Works very well 27/10/21
			string SqlCommand = "" ;
			bool result = true;
			int indexer = 0;
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];

			if ( DbName == "" )
				DbName = "Customer";

			using ( IDbConnection connection = new SqlConnection ( ConString ) )
			{
				try
				{
					var parameters = new
					{
						id = cvm.Id,
						custno = cvm . CustNo,
						bankno = cvm . BankNo,
						actype = cvm . AcType,
						fname= cvm . FName,
						lname= cvm . LName,
						addr1= cvm . Addr1,
						addr2= cvm . Addr2,
						town= cvm . Town,
						county= cvm . County,
						pcode= cvm . PCode,
						phone= cvm . Phone,
						mobile= cvm . Mobile,
						dob= cvm . Dob,
						odate = cvm . ODate,
						cdate = cvm . CDate
					} ;
					SqlCommand = $" Update  {DbName} set  CustNo=@custno, BankNo =@bankno, AcType = @actype, FName=@fname, LName=@lname, " +
						$" Addr1=@addr1, Addr2=@addr2, Town=@town, County=@County, PCode=@pcode, Phone=@phone, Mobile=@mobile, Dob=@dob, " +
						$"ODate =@odate, CDate = @cdate ";
					if ( Conditions != "" )
						SqlCommand += Conditions;
					else
						SqlCommand += " where Id =@Id";
					connection . Execute ( @SqlCommand , parameters );
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER {DbName} Update error : {ex . Message}, {ex . Data}" );
					result = false;
				}
				finally
				{
					if ( result )
						Console . WriteLine ( $"SQL [{DbName . ToUpper ( )}] Db record Updated using DAPPER successfuly" );
				}
			}
			return result;
		}
		#endregion   Customer record Db Update Method

		#region  Standard DetailsDb Update Method

		/// <summary>
		/// Update a complete Db (specifiable) from the datagrid passed in
		/// it also accepts a fully qualified SQLCommand string if required....
		/// </summary>
		/// <param name="dgrid">DataGrid ti update Db from</param>
		/// <param name="DbName">SQL DB name</param>
		/// <param name="SqlCommand">Fully qualified command string</param>
		/// <param name="Conditions">order by params as a string.  Eg : "where Id=@ID</param>
		/// <returns></returns>
		public static bool UpdateDetailsDb (
			DataGrid dgrid ,
			string DbName = "" ,
			string SqlCommand = "" ,
			string Conditions = "" )
		{
			// Works very well 27/10/21
			bool result = true;
			int indexer = 0;
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			DetailsViewModel bvm = new DetailsViewModel();

			if ( DbName == "" )
				DbName = "SecAccounts";

			using ( IDbConnection connection = new SqlConnection ( ConString ) )
			{
				try
				{

					for ( int x = 0 ; x < dgrid . Items . Count - 1 ; x++ )
					{
						dgrid . SelectedIndex = x;
						bvm = dgrid . SelectedItem as DetailsViewModel;
						var parameters = new
						{
							id = bvm.Id,
							actype = bvm . AcType,
							intrate = bvm . IntRate,
							balance = bvm . Balance,
							bankno = bvm . BankNo,
							custno = bvm . CustNo,
							odate = bvm . ODate,
							cdate = bvm . CDate
						} ;
						SqlCommand = $" Update  {DbName} set  CustNo=@custno, BankNo =@bankno, AcType = @actype, IntRate=@intrate, Balance=@balance,ODate =@odate, CDate = @cdate where Id=@Id";
						if ( Conditions != "" )
							SqlCommand += Conditions;
						connection . Execute ( @SqlCommand , parameters );
					}
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER {DbName} Update error : {ex . Message}, {ex . Data}" );
					result = false;
				}
				finally
				{
					if ( result )
						Console . WriteLine ( $"SQL [{DbName . ToUpper ( )}] Db Updated using DAPPER successfuly" );
				}
			}
			return result;
		}
		#endregion  Standard Details Db Update Method

		#region  Details record Db Update Method

		/// <summary>
		/// Update a complete Db (specifiable) from the datagrid passed in
		/// it also accepts a fully qualified SQLCommand string if required....
		/// </summary>
		/// <param name="dgrid">DataGrid ti update Db from</param>
		/// <param name="DbName">SQL DB name</param>
		/// <param name="SqlCommand">Fully qualified command string</param>
		/// <param name="Conditions">order by params as a string.  Eg : "where Id=@ID</param>
		/// <returns></returns>
		public static bool UpdateSingleDetailsDb (
			DetailsViewModel dvm ,
			string DbName = "" ,
			string Conditions = "" )
		{
			// Works very well 27/10/21
			string SqlCommand = "" ;
			bool result = true;
			int indexer = 0;
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];

			if ( DbName == "" )
				DbName = "SecAccounts";

			using ( IDbConnection connection = new SqlConnection ( ConString ) )
			{
				try
				{
					var parameters = new
					{
						id = dvm.Id,
						actype = dvm . AcType,
						intrate = dvm . IntRate,
						balance = dvm . Balance,
						bankno = dvm . BankNo,
						custno = dvm . CustNo,
						odate = dvm . ODate,
						cdate = dvm . CDate
					} ;
					SqlCommand = $" Update  {DbName} set  CustNo=@custno, BankNo =@bankno, AcType = @actype, IntRate=@intrate, Balance=@balance,ODate =@odate, CDate = @cdate  ";
					if ( Conditions != "" )
						SqlCommand += Conditions;
					else
						SqlCommand += " where Id =@Id";

					connection . Execute ( @SqlCommand , parameters );
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER {DbName} Update error : {ex . Message}, {ex . Data}" );
					result = false;
				}
				finally
				{
					if ( result )
						Console . WriteLine ( $"SQL [{DbName . ToUpper ( )}] Db record updated using DAPPER successfuly" );
				}
			}
			return result;
		}
		#endregion  Details record Db Update Method

		/// <summary>
		///  This is a MASSIVE Function that handles updating the Dbs via SQL plus sorting the current grid
		///  out & notifying all other viewers that a change has occurred so they can (& in fact do) update
		///  their own data grids rather nicely right now - 22/4/21
		/// </summary>
		public static void UpdateAllDb ( string CurrentDb , string DbName , DataGrid Bankgrid , DataGrid Custgrid , DataGrid Detgrid )
		{
			/// This ONLY gets called when a cell is edited in THIS viewer



			//			BankAccountViewModel ss = new BankAccountViewModel();
			//			CustomerViewModel cs = new CustomerViewModel();
			//			DetailsViewModel sa = new DetailsViewModel();

			//			Mouse . OverrideCursor = Cursors . Wait;

			//			// Set the control flags so that we know we have changed data when we notify other windows
			//			Flags . UpdateInProgress = true;

			//			// Set a global flag so we know we are in editing mode in the grid
			//			//
			//			//Only called whn an edit has been completed
			//			SQLHandlers sqlh = new SQLHandlers ( );
			//			// These get the row with all the NEW data
			//			if ( CurrentDb == "BANKACCOUNT" )
			//			{
			//				int currow = 0;

			//				currow = Bankgrid . SelectedIndex != -1 ? Bankgrid . SelectedIndex : 0;
			//				ss = Bankgrid . SelectedItem as BankAccountViewModel;
			//			}
			//			else if ( CurrentDb == "CUSTOMER" )
			//			{
			//				int currow = 0;
			//				currow = Custgrid . SelectedIndex != -1 ? Custgrid . SelectedIndex : 0;
			//				cs = Custgrid . SelectedItem as CustomerViewModel;
			//			}
			//			else if ( CurrentDb == "DETAILS" )
			//			{
			//				int currow = 0;
			//				currow = Detgrid . SelectedIndex != -1 ? Detgrid . SelectedIndex : 0;
			//				sa = Detgrid . SelectedItem as DetailsViewModel;
			//			}



			//			if ( CurrentDb == "BANKACCOUNT" || CurrentDb == "DETAILS" )
			//			{
			//				// Editdb is NOT OPEN
			//				SqlCommand cmd = null;
			//				try
			//				{
			//					//Sanity check - are values actualy valid ???
			//					//They should be as Grid vlaidate entries itself !!
			//					int x;
			//					decimal Y;
			//					if ( CurrentDb == "BANKACCOUNT" )
			//					{
			//						//						ss = e.Row.Item as BankAccount;
			//						x = Convert . ToInt32 ( ss . Id );
			//						x = Convert . ToInt32 ( ss . AcType );
			//						//Check for invalid A/C Type
			//						if ( x < 1 || x > 4 )
			//						{
			//							Debug . WriteLine ( $"SQL Invalid A/c type of {ss . AcType} in grid Data" );
			//							Mouse . OverrideCursor = Cursors . Arrow;
			//							MessageBox . Show ( $"Invalid A/C Type ({ss . AcType}) in the Grid !!!!\r\nPlease correct this entry!" );
			//							return;
			//						}
			//						Y = Convert . ToDecimal ( ss . Balance );
			//						Y = Convert . ToDecimal ( ss . IntRate );
			//						//Check for invalid Interest rate
			//						if ( Y > 100 )
			//						{
			//							Debug . WriteLine ( $"SQL Invalid Interest Rate of {ss . IntRate} > 100% in grid Data" );
			//							Mouse . OverrideCursor = Cursors . Arrow;
			//							MessageBox . Show ( $"Invalid Interest rate ({ss . IntRate}) > 100 entered in the Grid !!!!\r\nPlease correct this entry!" );
			//							return;
			//						}
			//						DateTime dtm = Convert . ToDateTime ( ss . ODate );
			//						dtm = Convert . ToDateTime ( ss . CDate );
			//					}
			//					else if ( CurrentDb == "DETAILS" )
			//					{
			//						x = Convert . ToInt32 ( sa . Id );
			//						x = Convert . ToInt32 ( sa . AcType );
			//						//Check for invalid A/C Type
			//						if ( x < 1 || x > 4 )
			//						{
			//							Debug . WriteLine ( $"SQL Invalid A/c type of {sa . AcType} in grid Data" );
			//							Mouse . OverrideCursor = Cursors . Arrow;
			//							MessageBox . Show ( $"Invalid A/C Type ({sa . AcType}) in the Grid !!!!\r\nPlease correct this entry!" );
			//							return;
			//						}
			//						Y = Convert . ToDecimal ( sa . Balance );
			//						Y = Convert . ToDecimal ( sa . IntRate );
			//						//Check for invalid Interest rate
			//						if ( Y > 100 )
			//						{
			//							Debug . WriteLine ( $"SQL Invalid Interest Rate of {sa . IntRate} > 100% in grid Data" );
			//							Mouse . OverrideCursor = Cursors . Arrow;
			//							MessageBox . Show ( $"Invalid Interest rate ({sa . IntRate}) > 100 entered in the Grid !!!!\r\nPlease correct this entry!" );
			//							return;
			//						}
			//						DateTime dtm = Convert . ToDateTime ( sa . ODate );
			//						dtm = Convert . ToDateTime ( sa . CDate );
			//					}
			//					//					string sndr = sender.ToString();
			//				}
			//				catch ( Exception ex )
			//				{
			//					Debug . WriteLine ( $"SQL Invalid grid Data - {ex . Message} Data = {ex . Data}" );
			//					Mouse . OverrideCursor = Cursors . Arrow;
			//					MessageBox . Show ( "Invalid data entered in the Grid !!!! - See Output for details" );
			//					return;
			//				}
			//			}
			//			SqlConnection con;
			//			string ConString = "";
			//			ConString = ( string ) Settings . Default [ "BankSysConnectionString" ];
			//			//			@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = 'C:\USERS\IANCH\APPDATA\LOCAL\MICROSOFT\MICROSOFT SQL SERVER LOCAL DB\INSTANCES\MSSQLLOCALDB\IAN1.MDF'; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
			//			con = new SqlConnection ( ConString );
			//			try
			//			{
			//				//We need to update BOTH BankAccount AND DetailsViewModel to keep them in parallel
			//				using ( con )
			//				{
			//					con . Open ( );

			//					if ( CurrentDb == "BANKACCOUNT" )
			//					{
			//						cmd = new SqlCommand ( "UPDATE BankAccount SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, BALANCE=@balance, INTRATE=@intrate, ODATE=@odate, CDATE=@cdate where CUSTNO = @custno AND BANKNO = @bankno" , con );
			//						cmd . Parameters . AddWithValue ( "@id" , Convert . ToInt32 ( ss . Id ) );
			//						cmd . Parameters . AddWithValue ( "@bankno" , ss . BankNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@custno" , ss . CustNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@actype" , Convert . ToInt32 ( ss . AcType ) );
			//						cmd . Parameters . AddWithValue ( "@balance" , Convert . ToDecimal ( ss . Balance ) );
			//						cmd . Parameters . AddWithValue ( "@intrate" , Convert . ToDecimal ( ss . IntRate ) );
			//						cmd . Parameters . AddWithValue ( "@odate" , Convert . ToDateTime ( ss . ODate ) );
			//						cmd . Parameters . AddWithValue ( "@cdate" , Convert . ToDateTime ( ss . CDate ) );
			//						cmd . ExecuteNonQuery ( );
			//						Debug . WriteLine ( "SQL Update of BankAccounts successful..." );

			//						cmd = new SqlCommand ( "UPDATE SecAccounts SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, BALANCE=@balance, INTRATE=@intrate, ODATE=@odate, CDATE=@cdate where CUSTNO = @custno AND BANKNO = @bankno" , con );
			//						cmd . Parameters . AddWithValue ( "@id" , Convert . ToInt32 ( sa . Id ) );
			//						cmd . Parameters . AddWithValue ( "@bankno" , sa . BankNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@custno" , sa . CustNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@actype" , Convert . ToInt32 ( sa . AcType ) );
			//						cmd . Parameters . AddWithValue ( "@balance" , Convert . ToDecimal ( sa . Balance ) );
			//						cmd . Parameters . AddWithValue ( "@intrate" , Convert . ToDecimal ( sa . IntRate ) );
			//						cmd . Parameters . AddWithValue ( "@odate" , Convert . ToDateTime ( sa . ODate ) );
			//						cmd . Parameters . AddWithValue ( "@cdate" , Convert . ToDateTime ( sa . CDate ) );
			//						cmd . ExecuteNonQuery ( );
			//						Debug . WriteLine ( "SQL Update of SecAccounts successful..." );

			//						cmd = new SqlCommand ( "UPDATE Customer SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, ODATE=@odate, CDATE=@cdate where CUSTNO = @custno AND BANKNO = @bankno" , con );
			//						cmd . Parameters . AddWithValue ( "@id" , Convert . ToInt32 ( sa . Id ) );
			//						cmd . Parameters . AddWithValue ( "@bankno" , sa . BankNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@custno" , sa . CustNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@actype" , Convert . ToInt32 ( sa . AcType ) );
			//						cmd . Parameters . AddWithValue ( "@odate" , Convert . ToDateTime ( sa . ODate ) );
			//						cmd . Parameters . AddWithValue ( "@cdate" , Convert . ToDateTime ( sa . CDate ) );
			//						cmd . ExecuteNonQuery ( );
			//						Debug . WriteLine ( "SQL Update of Customers successful..." );
			//					}
			//					else if ( CurrentDb == "DETAILS" )
			//					{
			//						cmd = new SqlCommand ( "UPDATE BankAccount SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, BALANCE=@balance, INTRATE=@intrate, ODATE=@odate, CDATE=@cdate where CUSTNO = @custno AND BANKNO = @bankno" , con );
			//						cmd . Parameters . AddWithValue ( "@id" , Convert . ToInt32 ( sa . Id ) );
			//						cmd . Parameters . AddWithValue ( "@bankno" , sa . BankNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@custno" , sa . CustNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@actype" , Convert . ToInt32 ( sa . AcType ) );
			//						cmd . Parameters . AddWithValue ( "@balance" , Convert . ToDecimal ( sa . Balance ) );
			//						cmd . Parameters . AddWithValue ( "@intrate" , Convert . ToDecimal ( sa . IntRate ) );
			//						cmd . Parameters . AddWithValue ( "@odate" , Convert . ToDateTime ( sa . ODate ) );
			//						cmd . Parameters . AddWithValue ( "@cdate" , Convert . ToDateTime ( sa . CDate ) );
			//						cmd . ExecuteNonQuery ( );
			//						Debug . WriteLine ( "SQL Update of BankAccounts successful..." );

			//						cmd = new SqlCommand ( "UPDATE SecAccounts SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, BALANCE=@balance, INTRATE=@intrate, ODATE=@odate, CDATE=@cdate where CUSTNO = @custno AND BANKNO = @bankno" , con );
			//						cmd . Parameters . AddWithValue ( "@id" , Convert . ToInt32 ( sa . Id ) );
			//						cmd . Parameters . AddWithValue ( "@bankno" , sa . BankNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@custno" , sa . CustNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@actype" , Convert . ToInt32 ( sa . AcType ) );
			//						cmd . Parameters . AddWithValue ( "@balance" , Convert . ToDecimal ( sa . Balance ) );
			//						cmd . Parameters . AddWithValue ( "@intrate" , Convert . ToDecimal ( sa . IntRate ) );
			//						cmd . Parameters . AddWithValue ( "@odate" , Convert . ToDateTime ( sa . ODate ) );
			//						cmd . Parameters . AddWithValue ( "@cdate" , Convert . ToDateTime ( sa . CDate ) );
			//						cmd . ExecuteNonQuery ( );
			//						Debug . WriteLine ( "SQL Update of SecAccounts successful..." );

			//						cmd = new SqlCommand ( "UPDATE Customer SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, ODATE=@odate, CDATE=@cdate where CUSTNO = @custno AND BANKNO = @bankno" , con );
			//						cmd . Parameters . AddWithValue ( "@id" , Convert . ToInt32 ( sa . Id ) );
			//						cmd . Parameters . AddWithValue ( "@bankno" , sa . BankNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@custno" , sa . CustNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@actype" , Convert . ToInt32 ( sa . AcType ) );
			//						cmd . Parameters . AddWithValue ( "@odate" , Convert . ToDateTime ( sa . ODate ) );
			//						cmd . Parameters . AddWithValue ( "@cdate" , Convert . ToDateTime ( sa . CDate ) );
			//						cmd . ExecuteNonQuery ( );
			//						Debug . WriteLine ( "SQL Update of customers successful..." );
			//					}
			//					if ( CurrentDb == "SECACCOUNTS" )
			//					{
			//						cmd = new SqlCommand ( "UPDATE BankAccount SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, BALANCE=@balance, INTRATE=@intrate, ODATE=@odate, CDATE=@cdate where CUSTNO = @custno AND BANKNO = @bankno" , con );
			//						cmd . Parameters . AddWithValue ( "@id" , Convert . ToInt32 ( ss . Id ) );
			//						cmd . Parameters . AddWithValue ( "@bankno" , ss . BankNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@custno" , ss . CustNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@actype" , Convert . ToInt32 ( ss . AcType ) );
			//						cmd . Parameters . AddWithValue ( "@balance" , Convert . ToDecimal ( ss . Balance ) );
			//						cmd . Parameters . AddWithValue ( "@intrate" , Convert . ToDecimal ( ss . IntRate ) );
			//						cmd . Parameters . AddWithValue ( "@odate" , Convert . ToDateTime ( ss . ODate ) );
			//						cmd . Parameters . AddWithValue ( "@cdate" , Convert . ToDateTime ( ss . CDate ) );
			//						cmd . ExecuteNonQuery ( );
			//						Debug . WriteLine ( "SQL Update of BankAccounts successful..." );

			//						cmd = new SqlCommand ( "UPDATE SecAccounts SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, BALANCE=@balance, INTRATE=@intrate, ODATE=@odate, CDATE=@cdate where CUSTNO = @custno AND BANKNO = @bankno" , con );
			//						cmd . Parameters . AddWithValue ( "@id" , Convert . ToInt32 ( sa . Id ) );
			//						cmd . Parameters . AddWithValue ( "@bankno" , sa . BankNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@custno" , sa . CustNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@actype" , Convert . ToInt32 ( sa . AcType ) );
			//						cmd . Parameters . AddWithValue ( "@balance" , Convert . ToDecimal ( sa . Balance ) );
			//						cmd . Parameters . AddWithValue ( "@intrate" , Convert . ToDecimal ( sa . IntRate ) );
			//						cmd . Parameters . AddWithValue ( "@odate" , Convert . ToDateTime ( sa . ODate ) );
			//						cmd . Parameters . AddWithValue ( "@cdate" , Convert . ToDateTime ( sa . CDate ) );
			//						cmd . ExecuteNonQuery ( );
			//						Debug . WriteLine ( "SQL Update of SecAccounts successful..." );

			//						cmd = new SqlCommand ( "UPDATE Customer SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, ODATE=@odate, CDATE=@cdate where CUSTNO = @custno AND BANKNO = @bankno" , con );
			//						cmd . Parameters . AddWithValue ( "@id" , Convert . ToInt32 ( sa . Id ) );
			//						cmd . Parameters . AddWithValue ( "@bankno" , sa . BankNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@custno" , sa . CustNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@actype" , Convert . ToInt32 ( sa . AcType ) );
			//						cmd . Parameters . AddWithValue ( "@odate" , Convert . ToDateTime ( sa . ODate ) );
			//						cmd . Parameters . AddWithValue ( "@cdate" , Convert . ToDateTime ( sa . CDate ) );
			//						cmd . ExecuteNonQuery ( );
			//						Debug . WriteLine ( "SQL Update of Customers successful..." );
			//					}
			//					StatusBar . Text = "ALL THREE Databases updated successfully....";
			//					Debug . WriteLine ( "ALL THREE Databases updated successfully...." );
			//				}
			//			}
			//			catch ( Exception ex )
			//			{
			//				Debug . WriteLine ( $"SQL Error - {ex . Message} Data = {ex . Data}" );

			//#if SHOWSQLERRORMESSAGEBOX
			//						Mouse . OverrideCursor = Cursors . Arrow;
			//						MessageBox . Show ( "SQL error occurred - See Output for details" );
			//#endif
			//			}
			//			finally
			//			{
			//				Mouse . OverrideCursor = Cursors . Arrow;
			//				con . Close ( );
			//			}




			//			if ( CurrentDb == "CUSTOMER" )
			//			{
			//				if ( e == null && CurrentDb == "CUSTOMER" )
			//					cs = e . Row . Item as CustomerViewModel;

			//				try
			//				{
			//					//Sanity check - are values actualy valid ???
			//					//They should be as Grid vlaidate entries itself !!
			//					int x;
			//					x = Convert . ToInt32 ( cs . Id );
			//					//					string sndr = sender.ToString();
			//					x = Convert . ToInt32 ( cs . AcType );
			//					//Check for invalid A/C Type
			//					if ( x < 1 || x > 4 )
			//					{
			//						Debug . WriteLine ( $"SQL Invalid A/c type of {cs . AcType} in grid Data" );
			//						Mouse . OverrideCursor = Cursors . Arrow;
			//						MessageBox . Show ( $"Invalid A/C Type ({cs . AcType}) in the Grid !!!!\r\nPlease correct this entry!" );
			//						return;
			//					}
			//					DateTime dtm = Convert . ToDateTime ( cs . ODate );
			//					dtm = Convert . ToDateTime ( cs . CDate );
			//					dtm = Convert . ToDateTime ( cs . Dob );
			//				}
			//				catch ( Exception ex )
			//				{
			//					Debug . WriteLine ( $"SQL Invalid grid Data - {ex . Message} Data = {ex . Data}" );
			//					MessageBox . Show ( "Invalid data entered in the Grid !!!! - See Output for details" );
			//					Mouse . OverrideCursor = Cursors . Arrow;
			//					return;
			//				}
			//				SqlConnection con;
			//				string ConString = "";
			//				ConString = ( string ) Settings . Default [ "BankSysConnectionString" ];
			//				//			@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = 'C:\USERS\IANCH\APPDATA\LOCAL\MICROSOFT\MICROSOFT SQL SERVER LOCAL DB\INSTANCES\MSSQLLOCALDB\IAN1.MDF'; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
			//				con = new SqlConnection ( ConString );
			//				try
			//				{
			//					//We need to update BOTH BankAccount AND DetailsViewModel to keep them in parallel
			//					using ( con )
			//					{
			//						con . Open ( );
			//						SqlCommand cmd = new SqlCommand ( "UPDATE Customer SET CUSTNO=@custno, BANKNO=@bankno, ACTYPE=@actype, " +
			//											"FNAME=@fname, LNAME=@lname, ADDR1=@addr1, ADDR2=@addr2, TOWN=@town, COUNTY=@county, PCODE=@pcode," +
			//											"PHONE=@phone, MOBILE=@mobile, DOB=@dob,ODATE=@odate, CDATE=@cdate where CUSTNO = @custno AND BANKNO = @bankno", con );

			//						cmd . Parameters . AddWithValue ( "@id" , Convert . ToInt32 ( cs . Id ) );
			//						cmd . Parameters . AddWithValue ( "@custno" , cs . CustNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@bankno" , cs . BankNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@actype" , Convert . ToInt32 ( cs . AcType ) );
			//						cmd . Parameters . AddWithValue ( "@fname" , cs . FName . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@lname" , cs . LName . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@addr1" , cs . Addr1 . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@addr2" , cs . Addr2 . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@town" , cs . Town . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@county" , cs . County . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@pcode" , cs . PCode . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@phone" , cs . Phone . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@mobile" , cs . Mobile . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@dob" , Convert . ToDateTime ( cs . Dob ) );
			//						cmd . Parameters . AddWithValue ( "@odate" , Convert . ToDateTime ( cs . ODate ) );
			//						cmd . Parameters . AddWithValue ( "@cdate" , Convert . ToDateTime ( cs . CDate ) );
			//						cmd . ExecuteNonQuery ( );
			//						Debug . WriteLine ( "SQL Update of Customers successful..." );

			//						cmd = new SqlCommand ( "UPDATE BankAccount SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, " +
			//							" ODATE=@odate, CDATE=@cdate where CUSTNO = @custno AND BANKNO = @bankno" , con );
			//						cmd . Parameters . AddWithValue ( "@id" , Convert . ToInt32 ( cs . Id ) );
			//						cmd . Parameters . AddWithValue ( "@bankno" , cs . BankNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@custno" , cs . CustNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@actype" , Convert . ToInt32 ( cs . AcType ) );
			//						cmd . Parameters . AddWithValue ( "@odate" , Convert . ToDateTime ( cs . ODate ) );
			//						cmd . Parameters . AddWithValue ( "@cdate" , Convert . ToDateTime ( cs . CDate ) );
			//						cmd . ExecuteNonQuery ( );
			//						Debug . WriteLine ( "SQL Update of BankAccounts successful..." );

			//						cmd = new SqlCommand ( "UPDATE SecAccounts SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, " +
			//							"ODATE=@odate, CDATE=@cdate where CUSTNO=@custno AND BANKNO = @bankno" , con );
			//						cmd . Parameters . AddWithValue ( "@id" , Convert . ToInt32 ( cs . Id ) );
			//						cmd . Parameters . AddWithValue ( "@bankno" , cs . BankNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@custno" , cs . CustNo . ToString ( ) );
			//						cmd . Parameters . AddWithValue ( "@actype" , Convert . ToInt32 ( cs . AcType ) );
			//						cmd . Parameters . AddWithValue ( "@odate" , Convert . ToDateTime ( cs . ODate ) );
			//						cmd . Parameters . AddWithValue ( "@cdate" , Convert . ToDateTime ( cs . CDate ) );
			//						cmd . ExecuteNonQuery ( );
			//						Debug . WriteLine ( "SQL Update of SecAccounts successful..." );
			//					}
			//					StatusBar . Text = "ALL THREE Databases updated successfully....";
			//					Debug . WriteLine ( "ALL THREE Databases updated successfully...." );
			//				}
			//				catch ( Exception ex )
			//				{
			//					Debug . WriteLine ( $"SQL Error - {ex . Message} Data = {ex . Data}" );
			//#if SHOWSQLERRORMESSAGEBOX
			//						Mouse . OverrideCursor = Cursors . Arrow;
			//						MessageBox . Show ( "SQL error occurred - See Output for details" );
			//#endif
			//				}
			//				finally
			//				{
			//					Mouse . OverrideCursor = Cursors . Arrow;
			//					con . Close ( );
			//				}
			//				Mouse . OverrideCursor = Cursors . Arrow;
			//				// Set the control flags so that we know we have changed data when we notify other windows
			//				Flags . UpdateInProgress = true;
			//				return;
			//			}

			//			// This is the NEW DATA from the current row that we are sending to SQL each update handler to update the DB's
			//			if ( Flags . USECOPYDATA )
			//			{
			//				UpdateSingleBankDb ( ss , DbName );

			//			}
			//			else
			//			{
			//				UpdateSingleBankDb ( ss , "BANKACCOUNT" );
			//				UpdateSingleCustomersDb ( ss , "BANKACCOUNT" );
			//				UpdateSingleDetailsDb ( ss , "BANKACCOUNT" );
			//			}

			//			Mouse . OverrideCursor = Cursors . Arrow;
			//			// Set the control flags so that we know we have changed data when we notify other windows
			//			Flags . UpdateInProgress = false;

			//			return;
		}


		#endregion Update Db methods

		#region Utitlity/Special Methods

		#region Generic Db load
		public static async Task<bool> GetGenericCollectionAsync ( List<string> collection ,
		string SqlCommand = "" ,
		bool Notify = false ,
		string Caller = "" )
		{
			string[] datain = { "","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","",""};  // 40 elements 
			List<string>   DbData = new List<string>();
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			try
			{
				using ( IDbConnection db = new SqlConnection ( ConString ) )
				{
					var Data = db . Query<object>( SqlCommand  ). ToList();
					Console . WriteLine ( $"SQL DAPPER {Data . Count} records successfuly" );
					var dat = Data . Select ( x =>  x.ToString() ) . ToList ( );
					string str = "";
					foreach ( var item in dat )
					{
						str = item . Substring ( 12 );
						collection . Add ( str );
					}
				}
			}
			catch ( Exception ex )
			{
				Console . WriteLine ( $"GENERIC DB ERROR : {ex . Message}" );
				return false;
			}
			//{
			//	EventControl . TriggerCustDataLoaded ( null ,
			//		new LoadedEventArgs
			//		{
			//			CallerType = "SQLSERVER" ,
			//			CallerDb = Caller 
			//			//DataSource = cvmcollection ,
			//			//RowCount = cvmcollection . Count
			//		} );
			return true;// (List<List<string>>)null;
		}

		#endregion Generic Db load

		#region Generic Db load
		public static List<string> GetGenericCollection ( List<string> collection ,
		string SqlCommand = "" ,
		bool Notify = false ,
		string Caller = ""
		)

		{
			string[] datain = { "","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","",""};  // 40 elements 
																										     //Type t = typeof(DbName) ;
																										     //ObservableCollection<typeof(DbName)> collection = new ObservableCollection<DbName>();
			List<string>   DbData = new List<string>();
			//static IEnumerable  List<string> strarray;
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];

			try
			{
				using ( IDbConnection db = new SqlConnection ( ConString ) )
				{
					//object data=null;
					//var parameters= new DynamicParameters(data);
					var Data = db . Query<object>( SqlCommand  ). ToList();
					//var Data = db . Query<dynamic>( SqlCommand  ). ToList();
					Console . WriteLine ( $"SQL DAPPER {Data . Count} records successfuly" );
					var dat = Data . Select ( x =>  x.ToString() ) . ToList ( );
					string str = "";
					foreach ( var item in dat )
					{
						str = item . Substring ( 12 );
						DbData . Add ( str );
					}
				}
			}
			catch ( Exception ex )
			{
				Console . WriteLine ( $"GENERIC DB ERROR : {ex . Message}" );
			}
			//{
			//	EventControl . TriggerCustDataLoaded ( null ,
			//		new LoadedEventArgs
			//		{
			//			CallerType = "SQLSERVER" ,
			//			CallerDb = Caller 
			//			//DataSource = cvmcollection ,
			//			//RowCount = cvmcollection . Count
			//		} );
			return DbData;// (List<List<string>>)null;
		}

		#endregion Generic Db load

		#region Create Copy of any specified Db

		public static bool CreateDbCopy (
			string OriginalDb ,
			string NewDb )
		{
			// All working WELL!!! 28/10/21
			string SqlCommand = "", TestCommand="";
			bool result = false;
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			using ( IDbConnection connection = new SqlConnection ( ConString ) )
			{
				// Check for existence of Db to be created
				try
				{
					TestCommand = $"Select top(1) * from  {NewDb}";
					var  res = connection . QueryFirst<int> ( TestCommand );
					if ( res > 0 )
						return false;
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"Database not found by test call to Db, proceeding with copy operation" );
				}
				// All is well, carry on and Copy Db
				try
				{
					SqlCommand = $"Select * into {NewDb} from {OriginalDb}";

					connection . Execute ( SqlCommand , CommandType . Text );

				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER {NewDb} could NOT be created from {OriginalDb}, Error info : {ex . Message}, {ex . Data}" );
				}
				finally
				{
					Console . WriteLine ( $"SQL DAPPER {NewDb} has been created from {OriginalDb} successfully" );
					result = true;
				}
			}
			return result;
		}
		#endregion Create Copy of any specified Db

		#region	CREATEBANKCOMBINEDASYNC
		public async static Task<bool> CreateBankCombinedAsync ( ObservableCollection<BankCombinedViewModel> collection ,
		string SqlCommand = "" ,
		bool Notify = false )
		{
			//====================================
			// Use STD DAPPER QUERY to load Bank data
			//====================================
			ObservableCollection<BankCombinedViewModel> bvmcollection = new ObservableCollection<BankCombinedViewModel>();
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			//IEnumerable < BankCombinedViewModel> bvmi;
			using ( IDbConnection db = new SqlConnection ( ConString ) )
			{
				try
				{
					// Use DAPPER to to load Bank data using Stored Procedure
					//if ( Flags . USEDAPPERWITHSTOREDPROCEDURE )
					//{
					try
					{
						var Args = new { DbName = "" , Arg = " " , Conditions = "" , SortBy = "" };
						SqlCommand = $"CreateBankCombinedDb";
						// This syntax WORKS CORRECTLY
						var result  = db . Query<BankCombinedViewModel>( SqlCommand , null,null,false, null,CommandType.StoredProcedure).ToList();
						//Console . WriteLine ( result );
						//foreach ( var item in result )
						//{
						//	bvmcollection . Add ( item );
						//}
						Console . WriteLine ( $"SQL DAPPER BANKCOMBINED DB created successfuly" );
						collection = bvmcollection;
					}
					catch ( Exception ex )
					{
						Console . WriteLine ( $"BANK  DB ERROR : {ex . Message}" );
					}
					//}
					//else
					//{
					//	Console . WriteLine ("Flags.USEDAPPERWITHSTOREDPROCEDURE not set !!!!");
					//}
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"Sql Error, {ex . Message}, {ex . Data}" );
					return false;
				}
			}
			return true;
		}
		#endregion	CREATEBASNKCOMBINEDASYNC

		#region	GETBANKCOMBINEDDB

		// NOT ASYNC !!
		public static ObservableCollection<BankCombinedViewModel> GetBankCombinedDb ( ObservableCollection<BankCombinedViewModel> collection ,
		string SqlCommand = "" ,
		string DbNameToLoad = "" ,
		string Orderby = "" ,
		string Conditions = "" ,
		bool wantSort = false ,
		bool Notify = false ,
		string Caller = "" ,
		int [ ] args = null )
		{
			ObservableCollection<BankCombinedViewModel> bvmcollection = new ObservableCollection<BankCombinedViewModel>();
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			//DbNameToLoad = "BankCombined";
			//============================================
			// Use STD DAPPER QUERY to load BankCombined data
			//============================================
			string[] ValidFields=
			{
				"ID",
				"CUSTNO",
				"BANKNO",
				"ACTYPE",
				"INTRATE" ,
				"BALANCE" ,
				"ODATE" ,
				"CDATE",
				"FNAME",
				"LNAME",
				"ADDR1",
				"ADDR2",
				"TOWN",
				"COUNTY",
				"PCODE",
				"PHONE"
				};
			string[] errorcolumns;

			IEnumerable < BankCombinedViewModel> bvmi;
			using ( IDbConnection db = new SqlConnection ( ConString ) )
			{
				if ( ValidateSortConditionColumns ( ValidFields , "BankCmobined" , Orderby , Conditions , out errorcolumns ) == false )
				{
					if ( Orderby . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
					{
						MessageBox . Show ( $"BANKCOMBINED dB\nSorry, but an invalid Column name has been \nidentified in the Sorting Clause provided.\n\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}.\n\nTherefore No Sort will be performed for this Db" );
						Orderby = "";
					}
					else if ( Conditions . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
					{
						MessageBox . Show ( $"BANKCOMBINED dB\nSorry, but an invalid Match clause or Column Name \nhas been identified in the Data Selection Clause.\n\nThe Invalid item identified was :\n\t{errorcolumns [ 0 ]}\n\nTherefore No Data Matching will be performed for this Db" );
						Conditions = "";
					}
					else
					{
						MessageBox . Show ( $"BANKCOMBINED dB\nSorry, but the Loading of the BankCombined Db is being aborted due to \na non existent Column name.\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}" );
						return collection;
					}
				}

				// make sure order by clause is correctly formatted
				if ( Orderby . Trim ( ) != "" )
				{
					if ( Orderby . ToUpper ( ) . Contains ( "ORDER BY " ) == false )
					{
						Orderby = " Order by " + Orderby;
					}
				}
				if ( Conditions != "" )
				{
					if ( Conditions . ToUpper ( ) . Contains ( "WHERE" ) == false )
						Conditions = " Where " + Conditions;
				}
				try
				{
					//====================================
					// Use standard DAPPER code to load Bank data
					//====================================
					if ( Conditions != "" )
					{
						if ( args [ 2 ] > 0 )
							SqlCommand = $" Select top ({args [ 2 ]}) * from {DbNameToLoad} {Conditions} {Orderby}";
						else
							SqlCommand = $" Select * from {DbNameToLoad} {Conditions} {Orderby}";
					}
					else
					{
						if ( Conditions == "" && Orderby == "" && args [ 0 ] == 0 && args [ 1 ] == 0 && args [ 2 ] == 0 )   // we dont even  have args for total records
							SqlCommand = $" Select * from {DbNameToLoad} ";
						else if ( args [ 0 ] != 0 || args [ 1 ] != 0 || args [ 2 ] != 0 )   // we do have args for total records
						{
							if ( args [ 2 ] == 0 )       // no limit on how many records to get
							{
								SqlCommand = $" Select * from {DbNameToLoad} ";
								if ( Conditions != "" )
									SqlCommand += $" {Conditions} ";
								else if ( args [ 1 ] != 0 )
									SqlCommand += $" where CustNo >= { args [ 0 ]} AND CustNo <= { args [ 1 ]} ";
							}
							else if ( args [ 2 ] > 0 && args [ 1 ] == 0 )
								SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} ";
							else if ( args [ 1 ] > 0 )// All 3 args are received
								SqlCommand = $" Select Top ({args [ 2 ]}) * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
							else
								SqlCommand = $" Select * from {DbNameToLoad} where CustNo >= {args [ 0 ]} AND CustNo <= {args [ 1 ]}";
						}
						else if ( Conditions != "" )  // We have conditions
							SqlCommand = $"Select * from {DbNameToLoad} {Conditions} ";
						else if ( args == null || args . Length == 0 )    // No args or conditions, so use generic command
							SqlCommand = $"Select * from {DbNameToLoad}  ";
						// Final Trap to ensure we have a valid command line
						if ( SqlCommand == "" )
							SqlCommand = $" Select * from {DbNameToLoad} ";

						if ( wantSort )
							SqlCommand += $" {Orderby}";
					}
					// Read data via Dapper into list<BVMI> cos Dapper uses Linq, so we cannot get other types returned
					bvmi = db . Query<BankCombinedViewModel> ( SqlCommand );

					foreach ( var item in bvmi )
					{
						bvmcollection . Add ( item );
					}
					collection = bvmcollection;
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER error : {ex . Message}, {ex . Data}" );
				}
				finally
				{
					Console . WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {bvmcollection . Count} records successfuly" );
				}
			}
			return bvmcollection;
		}
		#endregion	GETBANKCOMBINEDDB
		
		#region VALIDATESORTCONDITIONCOLUMNS
		private static bool ValidateSortConditionColumns ( string [ ] validFields , string caller , string orderby , string sortby , out string [ ] errorcolumns )
		{
			string[] errors={"","","","","","","","","","","","","","","","","","","",""};
			string Searchstring="" ;
			int counter=0;
			int errorcount=0;
			string[] FieldsToFind={"","","","","","","","","","","","","","","","","","","","", };
			bool result = true;
			string[] temp;
			char breakchar=' ';

			if ( orderby != "" )
			{
				temp = orderby . Split ( breakchar );
				if ( temp . Length > 0 )
				{
					foreach ( var item in temp )
					{
						string tmp = item . ToUpper ( ) . Trim ( );
						if ( tmp != "ORDER" && tmp != "BY" && tmp . Contains ( "DESC" ) == false && tmp . Contains ( "ASC" ) == false )
						{
							if ( tmp . Contains ( "," ) )
								tmp = tmp . Substring ( 0 , tmp . Length - 1 );
							FieldsToFind [ counter++ ] = tmp;
						}
					}
				}
			}
			if ( sortby != "" )
			{
				temp = sortby . Split ( breakchar );
				if ( temp . Length > 0 )
				{
					bool isnumeric = false;
					string ignorechars="  <   <=    =>    >  >=   ==    =  !=   !  1 2 3 4 5 6 7 8 9 0 ";
					foreach ( var item in temp )
					{
						string tmp = item . ToUpper ( ) . Trim ( );
						if ( tmp != "" )
						{
							try
							{
								double d = double . Parse ( tmp );
								isnumeric = true;
							}
							catch // let it drop thru
							{ }

							if ( tmp != "WHERE" && isnumeric == false )
							{
								// Is it  a maths sign etc 
								bool b =  ( ignorechars . Contains ( tmp ) == true );
								if ( b == false )
								{
									string  c  =tmp [ 0 ].ToString();
									string d  =tmp [ tmp.Length-1].ToString();
									// check if it's a quoted string, if so, let it through
									if ( c != "'" || d != "'" )
									{
										if ( tmp . Contains ( "," ) )
											tmp = tmp . Substring ( 0 , tmp . Length - 1 );
										FieldsToFind [ counter++ ] = tmp;
									}
								}
							}
						}
					}
				}
			}
			foreach ( var item in validFields )
			{
				if ( item != "" )
					Searchstring += item + ",";
			}
			foreach ( var item in FieldsToFind )
			{
				if ( item == "" )
					break;
				if ( validFields . Contains ( item ) == false )
				{
					errors [ errorcount++ ] = item . ToUpper ( );
					result = false;
				}
			}
			errorcolumns = errors;
			return result;
			//		}
		}
		
		#endregion VALIDATESORTCONDITIONCOLUMNS

		#endregion Utitlity/Special Methods
	}
}
