using Newtonsoft . Json . Linq;

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
using System . Windows . Input;
using WPFLibrary2021;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for NewCookie.xaml
	/// </summary>
	public partial class NewCookie : Window
	{
		public NewCookie (Window win=null )
		{
			InitializeComponent ( );
			Library1 . SetupWindowDrag ( this );
			Name . Focus ( );
		}
		private void button_Click ( object sender , RoutedEventArgs e )
{
			string result = "";
			result = Cookies . CreateCookie ( defvars.cookierootname , Key . Text , Value . Text);
			if(result != "")
				MessageBox . Show ( $"Cookie                                                         \n{result }\nkey ={Key.Text}\nvalue = {Value.Text}\nhas been created successfully...                      ", "Cookie Created !");
			defvars . CookieAdded = true;
			this .Close ( );
		}

		private void Cancel_Click ( object sender , RoutedEventArgs e )
		{
			defvars . CookieAdded = false;
			this . Close ( );
		}

		private void Window_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			if ( e . Key == System . Windows . Input . Key . Enter )
			{
				button_Click ( null , null );
				defvars . CookieAdded = true;
			}
			else if ( e . Key == System . Windows . Input . Key . Escape )
			{
				defvars . CookieAdded = false;
				this . Close ( );
			}
		}

		private void Window_PreviewKeyUp ( object sender , KeyEventArgs e )
		{
			if ( e . Key == System . Windows . Input . Key . Enter )
			{
				button_Click ( null , null );
				defvars . CookieAdded = true;
			}
			else if ( e . Key == System . Windows . Input . Key . Escape )
			{
				defvars . CookieAdded = false;
				this . Close ( );
			}
		}
	}
}
