using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Globalization;
using System . Security . Principal;
using System . Windows . Data;

using WPFPages . Views;

namespace WPFPages . ViewModels
{
	/// <summary>
	/// Base class for all standard classes with a constructor
	///  Author : ianch
	/// Created : 10/14/2021 10:29:36 AM
	/// </summary>
	public class NWSupplier : IComparable
	{
		//Object Declarations
		public  static  NWSupplier NWsupplier = new NWSupplier();

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

		#region IComparable implementation
		//implement IComparable for the class = (Calls other comparer classes )
		int IComparable.CompareTo ( object obj )
		{
			nworder nwo1 = ( nworder ) obj;
			return string . Compare ( this . SupplierId . ToString ( ) , nwo1 . OrderId . ToString ( ) );
		}

		public static IComparer SortOrderIdAscending ( )
		{
			return ( IComparer ) new SortOrderIdsAscending ( );
		}
		public static IComparer SortOrderIddescending ( )
		{
			return ( IComparer ) new SortOrderIdsdescending ( );
		}
		#endregion

		#region IComparer classes

		public class SortOrderIdsAscending : IComparer<nworder>
		{

			public int Compare ( nworder x , nworder y )
			{
				if ( x . OrderId > y . OrderId )
					return 1;
				else if ( x . OrderId < y . OrderId )
					return -1;
				else
					return 0;
			}
		}
		public class SortOrderIdsdescending : IComparer<nworder>
		{
			public int Compare ( nworder x , nworder y )
			{
				if ( x . OrderId < y . OrderId )
					return 1;
				else if ( x . OrderId > y . OrderId )
					return -1;
				else
					return 0;
			}
		}
		#endregion

		#region Class Declarations

		private int supplierId;
		public int SupplierId
		{
			get { return supplierId; }
			set { supplierId = value; OnPropertyChanged ( nameof ( SupplierId )); }
		}
		private string companyName;
		public string CompanyName
		{
			get { return companyName; }
			set { companyName = value; OnPropertyChanged ( nameof ( CompanyName)); }
		}
		private string contactName ;
		public string ContactName
		{
			get { return contactName; }
			set { contactName = value; OnPropertyChanged ( nameof ( ContactName )); }
		}
		private string contactTitle;
		public string ContactTitle
		{
			get { return contactTitle; }
			set { contactTitle = value; OnPropertyChanged ( nameof ( ContactTitle )); }
		}
		private string address;
		public string Address
		{
			get { return address; }
			set { address = value; OnPropertyChanged ( nameof ( Address)); }
		}
		private string city;
		public string City
		{
			get { return city; }
			set { city = value; OnPropertyChanged ( nameof ( City)); }
		}
		private string region;
		public string Region
		{
			get { return region; }
			set { region = value; OnPropertyChanged ( nameof ( Region)); }
		}
		private string postalCode;
		public string PostalCode
		{
			get { return postalCode; }
			set { postalCode = value; OnPropertyChanged ( nameof ( PostalCode)); }
		}
		private string country;
		public string Country
		{
			get { return country; }
			set { country = value; OnPropertyChanged ( nameof ( Country)); }
		}
		private string phone;
		public string Phone
		{
			get { return phone; }
			set { phone = value; OnPropertyChanged ( nameof ( Phone)); }
		}
		private string fax;
		public string Fax
		{
			get { return fax; }
			set { fax = value; OnPropertyChanged ( nameof ( Fax)); }
		}
		private string homepage;
		public string HomePage
		{
			get { return homepage; }
			set { homepage = value; OnPropertyChanged ( nameof ( HomePage)); }
		}

		#endregion Class Declarations

		public NWSupplier ( )
		{

		}
	}
}
