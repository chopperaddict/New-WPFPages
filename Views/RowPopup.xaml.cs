﻿using System;
using System . Diagnostics;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;
using WPFPages . ViewModels;
namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for RowInfoPopup.xaml
	/// </summary>
	public partial class RowInfoPopup : Window
	{
		private string CurrentDb = "";
		public bool IsDirty = false;
		private DataGrid ParentGrid = null;
		public DataGridRow dgr = null;
		//BankCollection bc = new BankCollection ( );
		//public BankCollection Bankcollection;// = bc.Bankcollection;
		//public CustCollection Custcollection = CustCollection . Custcollection;
		//public DetCollection Detcollection = DetCollection.Detcollection;
		BankAccountViewModel bvm = new BankAccountViewModel ( );
		CustomerViewModel cvm = new CustomerViewModel ( );
		DetailsViewModel dvm = new DetailsViewModel ( );

		public RowInfoPopup ( string callerType, DataGrid parentGrid)
		{
			ParentGrid = parentGrid;
			try
			{
				//store the tyoe of Db we are working with
				CurrentDb = callerType;
				if ( callerType == "" )
					return;
				InitializeComponent ( );
				if ( callerType == "BANKACCOUNT" )
				{
					bvm = parentGrid . SelectedItem as BankAccountViewModel;
					BankLabels . Visibility = Visibility . Visible;
					BankData . Visibility = Visibility . Visible;
					CustLabels . Visibility = Visibility . Hidden;
					CustData . Visibility = Visibility . Hidden;
					LeftCustBorder . Visibility = Visibility . Hidden;
					LeftBankBorder . Visibility = Visibility . Visible;
					//					BankAccountViewModel bvm = new BankAccountViewModel();
					BankData.DataContext = bvm;
					this . Height = 400;
				}

				if ( callerType == "CUSTOMER" )
				{
					cvm = parentGrid . SelectedItem as CustomerViewModel;
					CustData . Visibility = Visibility . Visible;
					CustLabels . Visibility = Visibility . Visible;
					BankLabels . Visibility = Visibility . Hidden;
					BankData . Visibility = Visibility . Hidden;
					LeftCustBorder . Visibility = Visibility . Visible;
					LeftBankBorder . Visibility = Visibility . Hidden;
					//					CustomerViewModel cvm = new CustomerViewModel ( );
					CustData . DataContext = cvm;
					this . Height = 597;
				}

				if ( callerType == "DETAILS" )
				{
					dvm = parentGrid . SelectedItem as DetailsViewModel;
					BankLabels . Visibility = Visibility . Visible;
					BankData . Visibility = Visibility . Visible;
					CustData . Visibility = Visibility . Hidden;
					CustLabels . Visibility = Visibility . Hidden;
					LeftCustBorder . Visibility = Visibility . Hidden;
					LeftBankBorder . Visibility = Visibility . Visible;
					//					DetailsViewModel dvm = new DetailsViewModel ( );
					BankData . DataContext = dvm;
					this . Height = 400;
				}
				//			this . MouseDown += delegate { DoDragMove ( ); };
				Utils.SetupWindowDrag(this);
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"General Exception : {ex . Message}, {ex . Data}" );
			}

			if ( IsDirty )
				SaveBtn . Visibility = Visibility . Visible;
		}
		private void DoDragMove ( )
		{
			//Handle the button NOT being the left mouse button
			// which will crash the DragMove Fn.....
			try
			{
				this . DragMove ( );
			}
			catch
			{
				return;
			}
		}

		private void ButtonBase_OnClick ( object sender, RoutedEventArgs e )
		{ this . Close ( ); }

		private void Save_Click ( object sender, RoutedEventArgs e )
		{
			SQLHandlers sqlh = new SQLHandlers ( );
			if ( CurrentDb != "CUSTOMER" )
			{
				if ( CurrentDb == "BANKACCOUNT" )
				{
					bvm . BankNo = bankno1 . Text;
					bvm . CustNo = custno1 . Text;
					bvm . AcType = Convert . ToInt32 ( actype1 . Text );
					bvm . IntRate = Convert . ToDecimal( intrate1 . Text );
					bvm . Balance = Convert . ToDecimal ( balance1 . Text );
					bvm . CDate = Convert . ToDateTime ( cdate1 . Text );
					sqlh . UpdateDbRow ( CurrentDb, bvm );
				}
				else
				{
					dvm . BankNo = bankno1 . Text;
					dvm . CustNo = custno1 . Text;
					dvm . AcType = Convert . ToInt32 ( actype1 . Text );
					dvm . IntRate = Convert . ToDecimal( intrate1 . Text );
					dvm . Balance = Convert . ToDecimal (balance1.Text );
					dvm . CDate = Convert . ToDateTime ( cdate1 . Text );
					sqlh . UpdateDbRow ( CurrentDb, dvm );
				}
			}
			else
			{
				cvm . BankNo = bankno . Text;
				cvm . CustNo = custno . Text;
				cvm . AcType = Convert . ToInt32 ( actype1 . Text );
				cvm . Addr1 = addr1 . Text;
				cvm . Addr2 = addr2 . Text;
				cvm . FName = fname . Text;
				cvm . LName = lname . Text;
				cvm . Town = town . Text;
				cvm . County = county . Text;
				cvm . PCode = pcode . Text;
				cvm . Phone = phone . Text;
				cvm . Mobile = mobile . Text;
				cvm . CDate = Convert . ToDateTime ( cdate . Text );
				sqlh . UpdateDbRow ( CurrentDb, cvm );
			}
			SaveBtn . Visibility = Visibility . Hidden;
			Close ( );
		}

		private void Window_KeyDown ( object sender, KeyEventArgs e )
		{
			if ( e . Key == Key . Escape )
			{
				Close ( );
			}
		}

		#region Bank/Details field dirty flag setters

		private void actype_LostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }
		private void intrate_LostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }
		private void balance_LostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }
		private void cdate_LostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }

		#endregion Bank/Details field dirty flag setters

		#region Customer field dirty flag setters
		private void ODateLostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }
		private void AcTypeLostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }

		private void CDateLostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }

		private void MobileLostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }

		private void PhoneLostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }

		private void PCodeLostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }

		private void CountyLostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }

		private void TownLostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }

		private void Addr2LostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }

		private void Addr1LostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }

		private void LNameLostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }

		private void FNameLostFocus ( object sender, RoutedEventArgs e )
		{ UpdateCollection ( ); }

		#endregion Customer field dirty flag setters

		private void UpdateCollection ( )
		{
			IsDirty = true;
			SaveBtn . IsEnabled = true;
			SaveBtn . Visibility = Visibility . Visible;
			return;
		}

		private void New_Click ( object sender, RoutedEventArgs e )
		{
			// Create and add as a new record
//			int x = 0;
			if ( CurrentDb == "BANKACCOUNT" )
			{
				BankAccountViewModel bvm = new BankAccountViewModel ( );
				bvm . CustNo = custno1.Text;
				bvm . BankNo = bankno1 . Text;
				bvm . AcType= int.Parse(actype1 . Text);
				bvm . IntRate = decimal . Parse ( intrate1 . Text);
				bvm . Balance = decimal. Parse ( balance1 . Text);
				bvm . ODate = DateTime. Parse ( odate1 . Text);
				bvm . CDate = DateTime . Parse ( cdate1 . Text);
				//MUST update BOTH bank style Db's'
				SQLDbSupport . AddNewRecord ( CurrentDb, bvm );
				SQLDbSupport . AddNewRecord ( "DETAILS", dvm );
				EventControl . TriggerViewerDataUpdated ( null, new LoadedEventArgs
				{
					CallerDb = CurrentDb,
					Custno = bvm.CustNo,
					SenderGuid = this.Tag.ToString(),
					Bankno = bvm . BankNo,
				});
				EventControl.TriggerGlobalDataChanged(this, new GlobalEventArgs
				{
					CallerType = "ROWPOPUP",
					AccountType = "BANKACCOUNT",
					SenderGuid = this.Tag?.ToString()
				});
			}
			else if ( CurrentDb == "CUSTOMER" )
			{
				CustomerViewModel cvm = new CustomerViewModel ( );
				cvm . CustNo = custno . Text;
				cvm . BankNo = bankno . Text;
				cvm . AcType = int . Parse ( actype . Text );
				cvm . FName = fname . Text;
				cvm .LName = lname. Text;
				cvm . Addr1= addr1. Text;
				cvm . Addr2= addr2. Text;
				cvm . Town= town. Text;
				cvm . County=county . Text;
				cvm . PCode= pcode. Text;
				cvm . Phone= phone. Text;
				cvm . Mobile= mobile. Text;
				cvm . Dob= DateTime . Parse ( dob. Text );
				cvm . ODate = DateTime . Parse ( odate . Text );
				cvm . CDate = DateTime . Parse ( cdate . Text );
				SQLDbSupport . AddNewRecord ( CurrentDb, cvm );
				EventControl . TriggerViewerDataUpdated ( null, new LoadedEventArgs
				{
					CallerDb = CurrentDb,
					Custno = cvm . CustNo,
					SenderGuid = this.Tag.ToString(),
					Bankno = cvm . BankNo,
				});
				EventControl.TriggerGlobalDataChanged(this, new GlobalEventArgs
				{
					CallerType = "ROWPOPUP",
					AccountType = "CUSTOMER",
					SenderGuid = this.Tag?.ToString()
				});
			}
			else if ( CurrentDb == "DETAILS" )
			{
				DetailsViewModel dvm = new DetailsViewModel ( );
				dvm . CustNo = custno1 . Text;
				dvm . BankNo = bankno1 . Text;
				dvm . AcType = int . Parse ( actype1 . Text );
				dvm . IntRate = decimal . Parse ( intrate1 . Text );
				dvm . Balance = decimal . Parse ( balance1 . Text );
				dvm . ODate = DateTime . Parse ( odate1 . Text );
				dvm . CDate = DateTime . Parse ( cdate1 . Text );
				//MUST update BOTH bank style Db's'
				SQLDbSupport . AddNewRecord ( CurrentDb, dvm );
				SQLDbSupport . AddNewRecord ( "BANKACCOUNT", dvm );
				EventControl . TriggerViewerDataUpdated ( null, new LoadedEventArgs
				{
					CallerDb = CurrentDb,
					Custno = dvm . CustNo,
					SenderGuid = this.Tag.ToString(),
					Bankno = dvm.BankNo,				
				});
				EventControl.TriggerGlobalDataChanged(this, new GlobalEventArgs
				{
					CallerType = "ROWPOPUP",
					AccountType = "DETAILS",
					SenderGuid = this.Tag?.ToString()
				});
			}
			Close ( );
		}

		private void Bankno_LostFocus ( object sender, RoutedEventArgs e )
		{

		}
	}
}