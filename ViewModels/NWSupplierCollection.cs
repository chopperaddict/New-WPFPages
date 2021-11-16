using System;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data . SqlClient;
using System . Data;
using System . Diagnostics;
using System . Globalization;
using System . Windows;
using System . Windows . Data;

using WPFPages . Views;

namespace WPFPages . ViewModels
{
	/// <summary>
	/// Base class for NorthWind Suppliers ObservableCollection
	///  Author : ianch
	/// Created : 10/14/2021 11:14:27 AM
	/// </summary>
	public class NWSupplierCollection
	{
		public  static ObservableCollection<NWSupplier> NwSuppliers = new ObservableCollection<NWSupplier> ( );

		#region PropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged ( string propertyName )
		{
			//PropertyChanged?.Invoke ( this, new PropertyChangedEventArgs ( propertyName ) );
			//			this . VerifyPropertyName ( propertyName );

			if ( this . PropertyChanged != null )
			{
				var e = new PropertyChangedEventArgs ( propertyName );
				this . PropertyChanged ( this , e );
			}
		}

		#endregion PropertyChanged

		public ObservableCollection<NWSupplier> LoadNWSuppliers ( int arg = -1 )
		{
			return LoadSupplierDetails ( arg );
		}
		public NWSupplierCollection ( )
		{

		}

		#region Suppliers Data Loaders
		public ObservableCollection<NWSupplier> LoadSupplierDetails ( int arg = -1 )
		{
			DataTable dt = new DataTable ( "SelectedSuppliers" );
			string ConString = ( string ) Properties . Settings . Default [ "NorthwindConnectionString" ];

			string CmdString = string . Empty;
			try
			{
				using ( SqlConnection con = new SqlConnection ( ConString ) )
				{
					if ( arg != -1 )
						CmdString = $"SELECT *  FROM [Suppliers] where SupplierId = {arg}";
					else
						CmdString = $"SELECT *  FROM [Suppliers]";
					SqlCommand cmd = new SqlCommand ( CmdString, con );
					SqlDataAdapter sda = new SqlDataAdapter ( cmd );
					sda . Fill ( dt );
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"Data={ex . Data}, {ex . Message}\n[{CmdString}]" );
			}
			return CreateSuppliersCollection ( dt );
		}
		public ObservableCollection<NWSupplier> CreateSuppliersCollection ( DataTable dt )
		{
			int count = 0;
			try
			{
				for ( int i = 0 ; i < dt . Rows . Count ; i++ )
				{
					NwSuppliers . Add ( new NWSupplier
					{
						SupplierId = Convert . ToInt32 ( dt . Rows [ i ] [ 0 ] ) ,
						CompanyName = dt . Rows [ i ] [ 1 ] . ToString ( ) ,
						ContactName = dt . Rows [ i ] [ 2 ] . ToString ( ) ,
						ContactTitle = dt . Rows [ i ] [ 3 ] . ToString ( ) ,
						Address = dt . Rows [ i ] [ 4 ] . ToString ( ) ,
						City = dt . Rows [ i ] [ 5 ] . ToString ( ) ,
						Region = dt . Rows [ i ] [ 6 ] . ToString ( ) ,
						PostalCode = dt . Rows [ i ] [ 7 ] . ToString ( ) ,
						Country = dt . Rows [ i ] [ 8 ] . ToString ( ) ,
						Phone = dt . Rows [ i ] [ 9 ] . ToString ( ) ,
						Fax = dt . Rows [ i ] [ 10 ] . ToString ( ) ,
						HomePage = dt . Rows [ i ] [ 11 ] . ToString ( )
					} );
					count = i;
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"SUPPLIER: ERROR in  CreateSuppliersCollection() : loading data into ObservableCollection : [{ex . Message}] : {ex . Data} ...." );
				MessageBox . Show ( $"SUPPLIER : ERROR in  CreateSuppliersCollection() : loading data into ObservableCollection : [{ex . Message}] : {ex . Data} ...." );
				return null;
			}
			finally
			{
			}
			return NwSuppliers;
		}
		#endregion Suppliers Data Loaders


	}
}