using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Linq;
using System . Linq . Expressions;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Controls;
using System . Windows . Input;

using WPFPages . Views;

namespace WPFPages . ViewModels
{

	public class LinqSupport< T, U, V>
	{
		// declare Delegates
		public delegate bool LinqDelegate<T, U> ( T s1 , U s2 ) ;
		public delegate bool LinqDelegate1<T, U> ( T s1 , U s2 );
		public delegate bool LinqDelegate2 <T>( T s1 );
		public delegate bool LinqDelegate3<T, U, V> ( T s1 , U s2 , V s3);

		public static DataGrid dg = new DataGrid();
		public static Func <T, T, bool> Querystr2str;
		public static Func <string, int, bool> Querystr2int;
		public static Func <int, int, bool> Queryint2int;
		private string  str1 = "a";
		private string  str2 = "b";
		//private static ObservableCollection<int> intcollection = new   ObservableCollection<int>();


		// EG MyFunc = (str, i) => str . Contains(i . ToString ( ) );
		//		ObservableCollection<S> bvm = new  ObservableCollection<S>();


		//public static ObservableCollection<GenericClass> linq1 ( ObservableCollection<GenericClass> generics , LinqDelegate dl, string Orderby )
		//{
		//	GenericClass[]  gcc = new GenericClass[generics.Count()];
		//	int x = 0;
		//	foreach ( var row in generics )
		//	{
		//		GenericClass  gc = new GenericClass();
		//		gc = row as GenericClass;
		//		gcc [x++] = gc ;

		//	}
		//	x = 0;
		//	string s1="", s2="";
		//	//s1 = items . field1
			//s2 = items . field2;
			//var accounts = from items in  generics
			//	.where( items.field1 == items . field2 )
			//	.select items;
			//		   //orderby Orderby
			////generics . Clear ( );
			//foreach ( var itm in accounts )
			//{
			//	if(dl(cond1, cond2))
			//		generics . Add ( itm);
			//}
		//	return generics;
		//	//let variablename=item
		//}
		public bool teststr2strequal ( T arg1 , T arg2 )
		{
			bool b = arg1. Equals ( arg2);
			return b;
		}
		public bool teststr2strnotequal ( T arg1 , T arg2 )
		{
			bool b = arg1. Equals ( arg2);
			return b;
		}

		//public static ObservableCollection<GenericClass> Test ( ObservableCollection<GenericClass> Generics , LinqDelegate ld, T s1, T s2, string orderby)
		//{
			
		//	return linq1 (  Generics,ld, s1, s2,orderby);
		//}


		//static Func<T , bool> GetEqualsExp<T> ( string nameOfParameter , string valueToCompare )
		//{
		//	var parameter = Expression.Parameter(typeof(T));
		//	Expression predicate = Expression.Constant(true);
		//	Expression property = Expression.Property(parameter, nameOfParameter);
		//	Expression equal = Expression.Equal(property, Expression.Constant(valueToCompare));
		//	predicate = Expression . AndAlso ( predicate , equal );
		//	return Expression . Lambda<Func<T , bool>> ( predicate , parameter ) . Compile ( );
		//}
		//public static void GetxViaLinq ( LinqDelegate LDelegate , ObservableCollection<T> collection )
		//{
		//	//select items;
		//	Mouse . OverrideCursor = Cursors . Wait;
		//	var accounts = from items in collection
		//			   where s
		//			   orderby Orderby
		//			   select items;
		//	Dgrid . ItemsSource = accounts;
		//	Mouse . OverrideCursor = Cursors . Arrow;
		//}
		//		private void Linq2_Click ( object sender , RoutedEventArgs e )
		//		{
		//			//select items;
		//			Mouse . OverrideCursor = Cursors . Wait;
		//			var accounts = from items in SqlBankAccounts
		//					   where ( items . AcType == 2 )
		//					   orderby items . CustNo
		//					   select items;
		//			this . BankGrid . ItemsSource = accounts;
		//			var accounts1 = from items in SqlCustAccounts
		//					    where ( items . AcType == 2 )
		//					    orderby items . CustNo
		//					    select items;
		//			this . CustomerGrid . ItemsSource = accounts1;
		//			var accounts2 = from items in SqlDetAccounts
		//					    where ( items . AcType == 2 )
		//					    orderby items . CustNo
		//					    select items;
		//			this . DetailsGrid . ItemsSource = accounts2;
		//			StatusBar . Text = "Only Records matching Account Type = 2 are shown above";
		//BankCount . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid , this . BankGrid . SelectedIndex )}";
		//			CustCount . Text = $"{Utils . GetPrettyGridStatistics ( this . CustomerGrid , this . CustomerGrid . SelectedIndex )}";
		//			DetCount . Text = $"{Utils . GetPrettyGridStatistics ( this . DetailsGrid , this . DetailsGrid . SelectedIndex )}";
		//			Mouse . OverrideCursor = Cursors . Arrow;
		//		}
		//		private void Linq3_Click ( object sender , RoutedEventArgs e )
		//		{
		//			//select items;
		//			Mouse . OverrideCursor = Cursors . Wait;
		//			var accounts = from items in SqlBankAccounts
		//					   where ( items . AcType == 3 )
		//					   orderby items . CustNo
		//					   select items;
		//			this . BankGrid . ItemsSource = accounts;
		//			var accounts1 = from items in SqlCustAccounts
		//					    where ( items . AcType == 3 )
		//					    orderby items . CustNo
		//					    select items;
		//			this . CustomerGrid . ItemsSource = accounts1;
		//			var accounts2 = from items in SqlDetAccounts
		//					    where ( items . AcType == 3 )
		//					    orderby items . CustNo
		//					    select items;
		//			this . DetailsGrid . ItemsSource = accounts2;
		//			StatusBar . Text = "Only Records matching Account Type = 3 are shown above";
		//			BankCount . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid , this . BankGrid . SelectedIndex )}";
		//			CustCount . Text = $"{Utils . GetPrettyGridStatistics ( this . CustomerGrid , this . CustomerGrid . SelectedIndex )}";
		//			DetCount . Text = $"{Utils . GetPrettyGridStatistics ( this . DetailsGrid , this . DetailsGrid . SelectedIndex )}";
		//			Mouse . OverrideCursor = Cursors . Arrow;
		//		}
		//		private void Linq4_Click ( object sender , RoutedEventArgs e )
		//		{
		//			//select items;
		//			Mouse . OverrideCursor = Cursors . Wait;
		//			var accounts = from items in SqlBankAccounts
		//					   where ( items . AcType == 4 )
		//					   orderby items . CustNo
		//					   select items;
		//			this . BankGrid . ItemsSource = accounts;
		//			var accounts1 = from items in SqlCustAccounts
		//					    where ( items . AcType == 4 )
		//					    orderby items . CustNo
		//					    select items;
		//			this . CustomerGrid . ItemsSource = accounts1;
		//			var accounts2 = from items in SqlDetAccounts
		//					    where ( items . AcType == 4 )
		//					    orderby items . CustNo
		//					    select items;
		//			this . DetailsGrid . ItemsSource = accounts2;
		//			StatusBar . Text = "Only Records matching Account Type = 4 are shown above";
		//			BankCount . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid , this . BankGrid . SelectedIndex )}";
		//			CustCount . Text = $"{Utils . GetPrettyGridStatistics ( this . CustomerGrid , this . CustomerGrid . SelectedIndex )}";
		//			DetCount . Text = $"{Utils . GetPrettyGridStatistics ( this . DetailsGrid , this . DetailsGrid . SelectedIndex )}";
		//			Mouse . OverrideCursor = Cursors . Arrow;
		//		}
		//		private void Linq5_Click ( object sender , RoutedEventArgs e )
		//		{
		//			int q = 1;
		//			//select All the items first;
		//			Mouse . OverrideCursor = Cursors . Wait;
		//			if ( q == 1 )
		//			{
		//				var accounts = from items in SqlBankAccounts orderby items . CustNo, items . BankNo select items;
		//				//Next Group BankAccountViewModel collection on Custno
		//				var grouped = accounts . GroupBy ( b => b . CustNo );
		//				//Now filter content down to only those a/c's with multiple Bank A/c's
		//				var sel = from g in grouped where g . Count ( ) > 1 select g;
		//				// Finally, iterate thru the list of grouped CustNo's matching to CustNo in the full accounts data
		//				// giving us ONLY the full records for any recordss that have > 1 Bank accounts
		//				List<BankAccountViewModel> output = new List<BankAccountViewModel> ( );
		//				foreach ( var item1 in sel )
		//				{
		//					foreach ( var item2 in accounts )
		//					{
		//						if ( item2 . CustNo . ToString ( ) == item1 . Key )
		//						{ output . Add ( item2 ); }
		//					}
		//				}
		//				this . BankGrid . ItemsSource = output;
		//			}
		//			if ( q == 1 )
		//			{
		//				var accounts = from items in SqlCustAccounts orderby items . CustNo, items . BankNo select items;
		//				//Next Group  collection on Custno
		//				var grouped = accounts . GroupBy ( b => b . CustNo );
		//				//Now filter content down to only those a/c's with multiple Bank A/c's
		//				var sel = from g in grouped where g . Count ( ) > 1 select g;
		//				// Finally, iterate thru the list of grouped CustNo's matching to CustNo in the full accounts data
		//				// giving us ONLY the full records for any recordss that have > 1 Bank accounts
		//				List<CustomerViewModel> output = new List<CustomerViewModel> ( );
		//				foreach ( var item1 in sel )
		//				{
		//					foreach ( var item2 in accounts )
		//					{
		//						if ( item2 . CustNo . ToString ( ) == item1 . Key )
		//						{ output . Add ( item2 ); }
		//					}
		//				}
		//				this . CustomerGrid . ItemsSource = output;
		//			}
		//			if ( q == 1 )
		//			{
		//				var accounts = from items in SqlDetAccounts orderby items . CustNo, items . BankNo select items;
		//				//Next Group  collection on Custno
		//				var grouped = accounts . GroupBy ( b => b . CustNo );
		//				//Now filter content down to only those a/c's with multiple Bank A/c's
		//				var sel = from g in grouped where g . Count ( ) > 1 select g;
		//				// Finally, iterate thru the list of grouped CustNo's matching to CustNo in the full accounts data
		//				// giving us ONLY the full records for any recordss that have > 1 Bank accounts
		//				List<DetailsViewModel> output = new List<DetailsViewModel> ( );

		//				//				System . Diagnostics . PresentationTraceSources . SetTraceLevel ( DetailsGrid . ItemContainerGenerator, System . Diagnostics . PresentationTraceLevel . High );

		//				foreach ( var item1 in sel )
		//				{
		//					foreach ( var item2 in accounts )
		//					{
		//						if ( item2 . CustNo . ToString ( ) == item1 . Key )
		//						{ output . Add ( item2 ); }
		//					}
		//				}
		//				this . DetailsGrid . ItemsSource = output;
		//			}
		//			StatusBar . Text = "Only Records of Customers with multiple Bank Accounts are shown above";
		//			BankCount . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid , this . BankGrid . SelectedIndex )}";
		//			CustCount . Text = $"{Utils . GetPrettyGridStatistics ( this . CustomerGrid , this . CustomerGrid . SelectedIndex )}";
		//			DetCount . Text = $"{Utils . GetPrettyGridStatistics ( this . DetailsGrid , this . DetailsGrid . SelectedIndex )}";
		//			Mouse . OverrideCursor = Cursors . Arrow;
		//		}
		//		private void Linq6_Click ( object sender , RoutedEventArgs e )
		//		{
		//			Mouse . OverrideCursor = Cursors . Wait;
		//			var accounts = from items in SqlBankAccounts orderby items . CustNo, items . AcType select items;
		//			var accounts1 = from items in SqlCustAccounts orderby items . CustNo, items . AcType select items;
		//			var accounts2 = from items in SqlDetAccounts orderby items . CustNo, items . AcType select items;
		//			this . BankGrid . ItemsSource = accounts;
		//			this . CustomerGrid . ItemsSource = accounts1;
		//			this . DetailsGrid . ItemsSource = accounts2;
		//			StatusBar . Text = "All available Records are shown above in all three grids";
		//			BankCount . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid , this . BankGrid . SelectedIndex )}";
		//			CustCount . Text = $"{Utils . GetPrettyGridStatistics ( this . CustomerGrid , this . CustomerGrid . SelectedIndex )}";
		//			DetCount . Text = $"{Utils . GetPrettyGridStatistics ( this . DetailsGrid , this . DetailsGrid . SelectedIndex )}";
		//			Mouse . OverrideCursor = Cursors . Arrow;
		//		}

		//		private void bankjoin_Click ( object sender , RoutedEventArgs e )
		//		{
		//			List<DetailsViewModel> output = new List<DetailsViewModel> ( );
		//			List<int> joinData = new List<int> ( );

		//			// create 2 lists first
		//			var bank = from item1 in SqlBankAccounts select item1;
		//			var detail = from item2 in SqlDetAccounts select item2;

		//			//select All the items first;				
		//			var accounts = from alldata in bank . Join (
		//				detail,
		//				bank => bank . CustNo,
		//				detail => detail . CustNo,
		//				( bank, detail ) => new
		//				{
		//					bank1 = bank . BankNo,
		//					bank2 = detail . BankNo,
		//					custno1 = bank . CustNo,
		//					custno2 = detail . CustNo,
		//					actype1 = detail . AcType,
		//					actype2 = bank . AcType,
		//					Balance1 = detail . Balance,
		//					Balance2 = detail . Balance,
		//				} )
		//					   select alldata;
		//			//accounts.So
		//			// Finally, iterate though the list of grouped CustNo's matching to CustNo in the full BankAccount data
		//			// giving us ONLY the full records for any records that have > 1 Bank accounts
		//			//foreach ( var item1 in sel )
		//			//{
		//			//	foreach ( var item2 in accounts )
		//			//	{
		//			//		if ( item2 . CustNo . ToString ( ) == item1 . Key )
		//			//		{ output . Add ( item2 ); }
		//			//	}
		//			//}
		//			DetailsGrid . ItemsSource = accounts;
		//			StatusBar . Text = $"Filtering completed, {output . Count} Multi Account records match";
		//			BankCount . Text = $"{Utils . GetPrettyGridStatistics ( this . BankGrid , this . BankGrid . SelectedIndex )}";
		//			CustCount . Text = $"{Utils . GetPrettyGridStatistics ( this . CustomerGrid , this . CustomerGrid . SelectedIndex )}";
		//			DetCount . Text = $"{Utils . GetPrettyGridStatistics ( this . DetailsGrid , this . DetailsGrid . SelectedIndex )}";
		//		}

	}
}
