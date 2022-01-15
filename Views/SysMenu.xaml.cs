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


namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for SysMenu.xaml
	/// </summary>
	public partial class SysMenu : Window
	{
		public SysMenu ( )
		{
			InitializeComponent ( );
			this . Width = Menu1 . Width;
			this . Left = System . Windows . SystemParameters . PrimaryScreenWidth - this.Width;
			var v = System . Windows . SystemParameters .VirtualScreenHeight;
			this . Top = 25;
			MainWindow . sysmenu = this;
		}

		private void Exit_Click ( object sender , RoutedEventArgs e )
		{
			MainWindow . sysmenu = null;
			this . Close ( );
			Application . Current . Shutdown ( );
		}
		private void DragDrop_Click ( object sender , RoutedEventArgs e )
		{
			DragDropClient ddc = new DragDropClient ( );
			ddc . Show ( );
		}

		private void ReloadText_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void SaveData_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void ClearGrid_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void ClearText_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void LoadDragClient_Click ( object sender , RoutedEventArgs e )
		{
			DragDropClient ddc = new DragDropClient ( );
			e . Handled = true;
			//ddc . Show ( );
		}

		private void xxxt_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void LoadDragDrop ( object sender , RoutedEventArgs e )
		{
			if ( Flags . DragDropViewer == null )
			{
				DragDropClient ddc = new DragDropClient ( );
				ddc . Show ( );
				Flags . DragDropViewer . RemoteLoadGrid ( );
			}
			else
				Flags . DragDropViewer . RemoteLoadGrid ( );
		}

		private void LoadBankDbView_Click ( object sender , RoutedEventArgs e )
		{
			BankDbView bdv = new BankDbView ( );
			bdv . Show ( );

		}

		private void LoadCustDbView_Click ( object sender , RoutedEventArgs e )
		{
			CustDbView cdv = new CustDbView ( );
			cdv . Show ( );
		}

		private void LoadDetailsDbView_Click ( object sender , RoutedEventArgs e )
		{
			DetailsDbView dbv = new DetailsDbView ( );
			dbv . Show ( );
		}

		private void LoadMultiView_Click ( object sender , RoutedEventArgs e )
		{
			MultiViewer mv = new MultiViewer ( );
			mv . Show ( );
		}

		private void Paths_Click ( object sender , RoutedEventArgs e )
		{
			if ( Flags . ExecuteViewer != null )
			{
				Flags . ExecuteViewer . BringIntoView ( );
				Flags . ExecuteViewer . Focus ( );
				return;
			}
			RunSearchPaths rsp = new RunSearchPaths ( );
			rsp . Show ( );
		}
		private void Execute_Click ( object sender , RoutedEventArgs e )
		{
			//ToggleEnable ( false );
			//ExecuteFile . Visibility = Visibility . Visible;
			//ExecuteFile . BringIntoView ( );
			//execName . Focus ( );
		}

		private void Exec_Click ( object sender , RoutedEventArgs e )
		{
			//			SupportMethods . ProcessExecuteRequest ( this, null, null, execName . Text );
		}

		private void scratch_Click ( object sender , RoutedEventArgs e )
		{
			//ToggleEnable ( true );
			//ExecuteFile . Visibility = Visibility . Collapsed;
		}

		private void ContextClose_Click ( object sender , RoutedEventArgs e )
		{
			Close ( );
		}

		private void ContextSave_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void ContextEdit_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void ContextSettings_Click ( object sender , RoutedEventArgs e )
		{
			Setup setup = new Setup ( );
			setup . Show ( );
			setup . BringIntoView ( );
			setup . Topmost = true;
			this . Focus ( );
		}

		private void ContextDisplayJsonData_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void ContextShowJson_Click ( object sender , RoutedEventArgs e )
		{

		}

	
		
		private void UserListbox_Click ( object sender , RoutedEventArgs e )
		{
			MultiviewListBoxViewers dblw = new MultiviewListBoxViewers( );
			dblw . Show ( );
		}

		private void Colors_Click ( object sender , RoutedEventArgs e )
		{
			ColorsSelector cs = new ColorsSelector ( "");
			cs . Show ( );
		}

		private void TreeView_Click ( object sender , RoutedEventArgs e )
		{
			TreeView1 tv1 = new TreeView1 ( );
			tv1 . Show ( );
		}

		private void LoadFullNorthwind_Click ( object sender , RoutedEventArgs e )
		{
			NorthwindFullData nwg = new NorthwindFullData ( );
			nwg . Show ( );
		}
		private void LoadSelectedNorthwind_Click ( object sender , RoutedEventArgs e )
		{
			SelectedNwDetails nwg = new SelectedNwDetails ( );
			nwg . Show ( );
		}
		private void LoadBasicNorthwind_Click ( object sender , RoutedEventArgs e )
		{
			//			NorthWindGrid nwg = new NorthWindGrid ( );

			//			nwg . Show ( );
		}

		private void sysColors_Click ( object sender , RoutedEventArgs e )
		{
			SysColors sc = new SysColors ( );
			sc . Show ( );
		}


		private void Animate_Click ( object sender , RoutedEventArgs e )
		{
			AnimationTest at = new AnimationTest ( );
			at . Show ( );
			//e . Handled = true;
		}

		private void AnimMaster_Click ( object sender , RoutedEventArgs e )
		{
			ThreeDeeBtnControl tdbc = new ThreeDeeBtnControl()  ;
			//				tdbc . Show ( );
			//e . Handled = true;
		}

		private void ButtonTesting_Click ( object sender , RoutedEventArgs e )
		{
			ButtonTesting btest = new ButtonTesting ( );
			btest . Show ( );
		}

		private void MoreTesting_Click ( object sender , RoutedEventArgs e )
		{
			MoreTesting tst = new MoreTesting ( );
			tst . Show ( );
		}

		private void MenuItem_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void StoryBoard_Click ( object sender , RoutedEventArgs e )
		{
			Stylingtest test = new Stylingtest ( );
			test . Show ( );

		}

		private void Attached_Click ( object sender , RoutedEventArgs e )
		{
			//                        ListBoxWindow lbw = new ListBoxWindow ( );
			//                      lbw . Show ( );
		}

		private void Styled_Click ( object sender , RoutedEventArgs e )
		{

		}

		private void CustomMsgbox_Click ( object sender , RoutedEventArgs e )
		{
			AboutBox cm = new AboutBox ();
			cm . Show ( );
		}

		private void Bankaccount_Click ( object sender , RoutedEventArgs e )
		{
			Bankaccount ba = new Bankaccount();
			ba . Show ( );
		}
	
		private void ItemsControl_Click ( object sender , RoutedEventArgs e )
		{
			ItemsControlDemo id = new ItemsControlDemo();
			id . Show ( );
		}

		private void Grouping_Click ( object sender , RoutedEventArgs e )
		{
			GroupedAccounts ga = new GroupedAccounts();
			ga . Show ( );
		}

		private void SQL_Click ( object sender , RoutedEventArgs e )
		{
			SqlServerCommands sqlc = new SqlServerCommands();
			sqlc . Show ( );
		}

		private void Dapper_Testing_Click ( object sender , RoutedEventArgs e )
		{
			DapperTesting dpt = new DapperTesting();
			dpt . Show ( );
		}


		private void Msgbox_Click ( object sender , RoutedEventArgs e )
		{
			Window w = new MsgboxSetup();
			w . Show ( );
		}

		private void MsgboxAPs_Click ( object sender , RoutedEventArgs e )
		{
			MainWindow . ShowAPDatatoConsole ( );

		}

		private void CreateCookie_Click ( object sender , RoutedEventArgs e )
		{
			NewCookie  nc = new NewCookie ( );
			nc . ShowDialog ( );
		}

		private void ListCookieData_Click ( object sender , RoutedEventArgs e )
		{
			Cookies . ShowAllCookieData ( out int total , "" );
		}

		private void NewCookie_Click ( object sender , RoutedEventArgs e )
		{
			Utils . NewCookie_Click ( null , null );
		}

		private void sqlselectorbtn_Select ( object sender , RoutedEventArgs e )
		{
			SqlServerCommands sqlc = new SqlServerCommands();
			sqlc . Show ( );
		}

		private void Smb_Click ( object sender , RoutedEventArgs e )
		{
			// shortmsg  box

			Mouse . OverrideCursor = Cursors . Arrow;
			Utils . Mbox (
				this ,
					string1: "This is  the Main Row of text that will contain the relevant information that has caused this dialog to be spawned to display to the End User...." ,
					string2: "This is the 2nd row of text to contain additional info or to provide a prompt to your user..." ,
					caption: "*** This is the Message Box Caption Bar ***" ,
					iconstring: "\\icons\\Information.png" ,
					Btn1: MB . YES ,
					Btn2: MB . NO ,
					defButton: MB . NO );
		}

		private void fmb_Click ( object sender , RoutedEventArgs e )
		{
			// full msg  box
			Utils . Mssg (
				caption: "This is the Message Box Caption Bar" ,
				string1: $"Main row of text of reasonable length to provide the information the dialog needs to provide to your user to let them take the best possible decision."
				 + "This line follows the double 'newline) characters at the end of the sentence above, and a single \\n is right here.\nIt is possible to have up to a maximum of 7 lines of text inside this Row 1 area, & then 2 further rows below it ..." ,
				string2: "This 2nd row of Text can contain some additional info to help or direct your user's decision.," ,
				string3:"This 3rd row of text can prompt your user to take the best decision" ,
				title: "" ,
				iconstring: "\\icons\\exclaim2.png" ,
				defButton: 2 ,
				Btn1: 1 ,
				Btn2: 2 ,
				Btn3: 3 ,
				Btn4: 4 ,
				btn1Text: "Ok" ,
				btn2Text: "Get on with it" ,
				btn3Text: "Bale out quick" ,
				btn4Text: "Run !!!"
			     );
		}

		private void Smallmb_Click ( object sender , RoutedEventArgs e )
		{
			Mouse . OverrideCursor = Cursors . Arrow;
			
			Utils . Mbox (
				this ,
					string1: "This is  the Main Row of text that will contain the relevant information that has caused this dialog to be spawned to display to the end user..." ,
					string2: "This is the 2nd row of text to contain additional info ." ,
					caption: "*** This is the Message Box Caption Bar ***" ,
					iconstring: "\\icons\\Information.png" ,
					Btn1: MB . OK,
					Btn2: MB . CANCEL,
					0,0,
					defButton: MB . OK , true );
		}

		private void mbsettings_Click ( object sender , RoutedEventArgs e )
		{
			MsgboxSetup sm = new MsgboxSetup ( );
		}

		private void Startwin_Click ( object sender , RoutedEventArgs e )
		{
			if(MainWindow . dbs == null)
				return;
			if ( MainWindow . dbs? .Visibility == Visibility .Visible)
				MainWindow . dbs . Visibility = Visibility . Hidden;
			else
				MainWindow . dbs. Visibility = Visibility . Visible;
		}

		private void SQLServer_Click ( object sender , RoutedEventArgs e )
		{
			SqlServerCommands sql = new SqlServerCommands();
			sql . Show ( );
		}

		private void Window_Closed ( object sender , EventArgs e )
		{
			MainWindow . sysmenu = null;

		}
	}
}
