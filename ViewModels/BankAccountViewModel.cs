// if set, Datatable is cleared and reloaded, otherwise it is not reloaded
//#define PERSISTENTDATA
#define USETASK
#undef USETASK

using Dapper;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Threading . Tasks;
using System . Windows . Controls;
using System . Windows . Data;

using WPFPages . Views;

/// <summary>
///  this is a mirror image of the original BankAccount.cs file
/// </summary>
namespace WPFPages . ViewModels
{
	[Serializable]
	public partial class BankAccountViewModel : INotifyPropertyChanged
	{

		#region PropertyChanged

		new public event PropertyChangedEventHandler PropertyChanged;

		new private void OnPropertyChanged ( string propertyName )
		{
			if ( Flags . SqlBankActive == false )
				//				this . VerifyPropertyName ( propertyName );

				if ( this . PropertyChanged != null )
				{
					var e = new PropertyChangedEventArgs ( propertyName );
					this . PropertyChanged ( this , e );
				}
		}
		/// <summary>
		/// Warns the developer if this object does not have
		/// a public property with the specified name. This
		/// method does not exist in a Release build.
		/// </summary>
		[Conditional ( "DEBUG" )]
		[DebuggerStepThrough]
		public virtual void VerifyPropertyName ( string propertyName )
		{
			// Verify that the property name matches a real,
			// public, instance property on this object.
			if ( TypeDescriptor . GetProperties ( this ) [ propertyName ] == null )
			{
				string msg = "Invalid property name: " + propertyName;

				if ( this . ThrowOnInvalidPropertyName )
					throw new Exception ( msg );
				else
					Debug . Fail ( msg );
			}
		}

		/// <summary>
		/// Returns whether an exception is thrown, or if a Debug.Fail() is used
		/// when an invalid property name is passed to the VerifyPropertyName method.
		/// The default value is false, but subclasses used by unit tests might
		/// override this property's getter to return true.
		/// </summary>
		protected virtual bool ThrowOnInvalidPropertyName
		{
			get; private set;
		}

		#endregion PropertyChanged

		// Create a Collection that can be added to View Collection ?
		public static ObservableCollection<BankAccountViewModel> BankViewObservableCollection { get; set; }
		public static ICollectionView BankCollectionView;

		#region CONSTRUCTOR

		public BankAccountViewModel ( )
		{
			BankCollectionView = CollectionViewSource . GetDefaultView ( BankViewObservableCollection );
		}
		#endregion CONSTRUCTOR

		public static DataGrid ActiveEditDbViewer = null;

		#region STANDARD CLASS PROPERTIES SETUP

		private int id { get; set; }
		private string bankno { get; set; }
		private string custno { get; set; }
		private int actype { get; set; }
		private decimal balance { get; set; }
		private decimal intrate { get; set; }
		private DateTime odate { get; set; }
		private DateTime cdate { get; set; }

		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
				OnPropertyChanged ( Id . ToString ( ) );
			}
		}

		public string BankNo
		{
			get
			{
				return bankno;
			}
			set
			{
				bankno = value;
				OnPropertyChanged ( BankNo );
			}
		}

		public string CustNo
		{
			get
			{
				return custno;
			}
			set
			{
				custno = value;
				OnPropertyChanged ( CustNo );
			}
		}

		public int AcType
		{
			get
			{
				return actype;
			}

			set
			{
				actype = value;
				OnPropertyChanged ( AcType . ToString ( ) );
			}
		}

		public decimal Balance
		{
			get
			{
				return balance;
			}

			set
			{
				balance = value;
				OnPropertyChanged ( Balance . ToString ( ) );
			}
		}

		public decimal IntRate
		{
			get
			{
				return intrate;
			}
			set
			{
				intrate = value;
				OnPropertyChanged ( IntRate . ToString ( ) );
			}
		}

		public DateTime ODate
		{
			get
			{
				return odate;
			}
			set
			{
				odate = value;
				OnPropertyChanged ( ODate . ToString ( ) );
			}
		}

		public DateTime CDate
		{
			get
			{
				return cdate;
			}
			set
			{
				cdate = value;
				OnPropertyChanged ( CDate . ToString ( ) );
			}
		}

		public string ToString ( bool full = false )
		{
			//			if ( full )
			//				return CustNo + ", " + BankNo + ", " + AcType + ", " + IntRate + ", " + Balance + ", " + ODate + ", " + CDate;
			//			else
			return base . ToString ( );
		}
		public override string ToString ( )
		{
			return base . ToString ( );
		}
		#endregion STANDARD CLASS PROPERTIES SETUP

		#region  DAPPER data methods for BankAccount
		public static List<BankAccountViewModel> GetBankDataAsList ( string SqlCommand = "" , bool Notify = false )
		{
			List<BankAccountViewModel> bvmlist = new List<BankAccountViewModel>();
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			using ( IDbConnection db = new SqlConnection ( ConString ) )
			{
				try
				{
					if ( SqlCommand == "" )
						bvmlist = db . Query<BankAccountViewModel> ( "Select * From BankAccount" ) . ToList ( );
					else
						bvmlist = db . Query<BankAccountViewModel> ( SqlCommand ) . ToList ( );
				}
				catch ( Exception Ex )
				{
					Console . WriteLine ( $"GETBANKDATAASLIST : DAPPER data load error - {Ex . Message}, {Ex . Data}" );
				}
				if ( Notify )
				{
					//					collection = bvmcollection;
					EventControl . TriggerBankListDataLoaded ( null ,
						new LoadedEventArgs
						{
							CallerType = "SQLSERVER" ,
							CallerDb = "" ,
							DataSource = bvmlist ,
							RowCount = bvmlist . Count
						} );
				}
				return bvmlist;
			}
		}
		public static ObservableCollection<BankAccountViewModel> GetBankObsCollection ( ObservableCollection<BankAccountViewModel> collection , string SqlCommand = "" , bool Notify = false , string Caller = "" )
		{
			object  Bankcollection = new object();
			ObservableCollection<BankAccountViewModel> bvmcollection = new ObservableCollection<BankAccountViewModel>();
			bvmcollection = collection;
			IDictionary <int, string> BankDict = new Dictionary<int, string>();
			List<BankAccountViewModel> bvmlist = new List<BankAccountViewModel>();
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			using ( IDbConnection db = new SqlConnection ( ConString ) )
			{
				try
				{
					if ( SqlCommand == "" )
						bvmlist = db . Query<BankAccountViewModel> ( "Select * From BankAccount" ) . ToList ( );// ( "Select * From BankAccount" ) as ObservableCollection<BankAccountViewModel>;
																				    //						bvmcollection = db . Query<BankAccountViewModel> ( "Select * From BankAccount" ) as ObservableCollection<BankAccountViewModel>;
					else
						//						bvmcollection = db . Query<BankAccountViewModel> ( SqlCommand ) as ObservableCollection<BankAccountViewModel>;
						bvmlist = db . Query<BankAccountViewModel> ( SqlCommand ) . ToList ( );// as ObservableCollection<BankAccountViewModel>;

					//db . Query<BankAccountViewModel> ( SqlCommand, 1  ) . ToDictionary (
					//	    row => ( string ) row . CustNo , row => ( int ) row . Id );
					if ( bvmlist . Count > 0 )
					{
						foreach ( var item in bvmlist )
						{
							bvmcollection . Add ( item );
							Console . WriteLine ( $"SQL DAPPER Dictionary : Adding {item.BankNo} " );
							if(BankDict.ContainsKey(int.Parse(item.BankNo)) == false)
								BankDict . Add (int.Parse(item . BankNo ), item . Balance . ToString ( ) );
						}
					}
//					Console . WriteLine ( $"SQL DAPPER has loaded : {bvmcollection . Count} BankAccount Records" );
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER error : {ex . Message}, {ex . Data}" );
				}
			}
			if ( Notify )
			{
				collection = bvmcollection;
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
		public static async Task<ObservableCollection<BankAccountViewModel>> GetBankDataAsObsCollectionAsync ( ObservableCollection<BankAccountViewModel> collection , string SqlCommand = "" , bool Notify = true , string Caller = "" )
		{
			//			object  Bankcollection = new object();
			ObservableCollection<BankAccountViewModel> bvmcollection =collection;
			//			List<BankAccountViewModel> bvmlist = new List<BankAccountViewModel>();
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];

			using ( IDbConnection db = new SqlConnection ( ConString ) )
			{
				try
				{
					if ( SqlCommand == "" )
						bvmcollection = await db . QueryAsync<BankAccountViewModel> ( "Select * From BankAccount" ) . ConfigureAwait ( false ) as ObservableCollection<BankAccountViewModel>;
					else
						bvmcollection = await db . QueryAsync<BankAccountViewModel> ( SqlCommand ) . ConfigureAwait ( false ) as ObservableCollection<BankAccountViewModel>;
				}
				catch ( Exception ex )
				{
					Console . WriteLine ( $"SQL DAPPER error : {ex . Message}, {ex . Data}" );
				}
			}
			if ( Notify )
			{
				collection = bvmcollection;
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

			#endregion  DAPPER data methods for BankAccount
		}
	}
}

/*
 *
 #if USETASK
			{
				int? taskid = Task.CurrentId;
				DateTime start = DateTime.Now;
				Task<bool> DataLoader = FillBankAccountDataGrid ();
				DataLoader.ContinueWith
				(
					task =>
					{
						LoadBankAccountIntoList (dtBank);
					},
					TaskScheduler.FromCurrentSynchronizationContext ()
				);
				Console.WriteLine ($"Completed AWAITED task to load BankAccount  Data via Sql\n" +
					$"task =Id is [ {taskid}], Completed status  [{DataLoader.IsCompleted}] in {(DateTime.Now - start)} Ticks\n");
			}
#else
			{
* */
