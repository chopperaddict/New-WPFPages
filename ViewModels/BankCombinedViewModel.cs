using System;
using System . Collections . Generic;
using System . Linq;
using System . Security . Permissions;
using System . Text;
using System . Threading . Tasks;

namespace WPFPages . ViewModels
{
	// combination of BankAccount with personal Customer details added
	public class BankCombinedViewModel : BankAccountViewModel
	{
		public BankCombinedViewModel ( ){}

		private string lName;
		public string LName
		{
			get { return lName; }
			set { lName = value; OnPropertyChanged ( LName. ToString ( ) ); }
		}
		private string fName;
		public string FName
		{
			get { return fName; }
			set { fName = value; OnPropertyChanged ( FName. ToString ( ) ); }
		}
		private string addr1;
		public string Addr1
		{
			get { return addr1; }
			set { addr1 = value; OnPropertyChanged ( Addr1. ToString ( ) ); }
		}
		private string addr2;
		public string Addr2
		{
			get { return addr2; }
			set { addr2 = value; OnPropertyChanged ( Addr2. ToString ( ) ); }
		}
		private string town;
		public string Town
		{
			get { return town; }
			set { town = value; OnPropertyChanged ( Town . ToString ( ) ); }
		}
		private string county;
		public string County
		{
			get { return county; }
			set { county = value; OnPropertyChanged ( County. ToString ( ) ); }
		}
		private string pcode;
		public string PCode
		{
			get { return pcode; }
			set { pcode = value; OnPropertyChanged ( PCode. ToString ( ) ); }
		}
		private string phone;
		public string Phone
		{
			get { return phone; }
			set { phone = value; OnPropertyChanged ( Phone. ToString ( ) ); }
		}

	}
}
