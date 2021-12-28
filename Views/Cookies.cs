using Microsoft . SqlServer . Management . Smo;

using Newtonsoft . Json . Linq;

using System;
using System . Collections;
using System . Collections . Generic;
using System . IO;
using System . Linq;
using System . Net;
using System . Reflection;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Shapes;
//using System . Net;

namespace WPFPages . Views
{
	[Serializable]
	public static class Cookies
	{
		public static Cookie cookie  = new Cookie();
		public static int NextIndex=1;

		//********************************************************************************************************************************************************************************//
		public static Uri CreateNewUri ( string root )
		//********************************************************************************************************************************************************************************//
		{
			string tmp = defvars.cookierootname.ToString();
			tmp = tmp + NextIndex . ToString ( );
			return new Uri ( tmp );
		}
		// Check whether cookie exists
		//********************************************************************************************************************************************************************************//
		public static bool CheckCookie ( string key )
		//********************************************************************************************************************************************************************************//
		{
			key = key . ToUpper ( );
			if ( defvars . Cookiedictionary == null )
			{
				defvars . Cookiedictionary = DeSerialize ( defvars . CookieDictionarypath ) as Dictionary<string , string>;

				if ( defvars . Cookiedictionary == null )
					defvars . Cookiedictionary = new Dictionary<string , string> ( );
			}
			else
			{
				if ( defvars . Cookiedictionary . ContainsKey ( key ) )
					return true;
			}
			return false;
		}
		//********************************************************************************************************************************************************************************//
		public static bool CheckforUri ( Uri uri )
		//********************************************************************************************************************************************************************************//
		{
			foreach ( var item in defvars . Cookiecollection )
			{
				Cookie ck = item as Cookie;
				if ( ck . Path == uri . ToString ( ) )
					return true;
			}
			return false;
		}
		//********************************************************************************************************************************************************************************//
		// Create new cookie
		public static string CreateCookie ( Uri cookiename , string key , string value )
		//********************************************************************************************************************************************************************************//
		{
			string exists = "";
			Uri OriginalUri = cookiename;
			key = key . ToUpper ( );

			// See if it is in our Dictionary 1st.using the key value
			if ( defvars . Cookiedictionary == null )
				defvars . Cookiedictionary = new Dictionary<string , string> ( );
			defvars . Cookiedictionary . TryGetValue ( key , out exists );

			if ( exists == null || exists . Length == 0 )
			{
				// No, so create unique Uri address
				string tmp = cookiename.ToString();
				tmp = tmp + defvars . NextCookieIndex . ToString ( );
				cookiename = new Uri ( tmp );
				defvars . NextCookieIndex++;
				// Ensure we are not duplicating the Uri
				if ( CheckforUri ( cookiename ))
				{
					do
					{
						tmp = OriginalUri.ToString();
						tmp = tmp + defvars . NextCookieIndex . ToString ( );
						cookiename = new Uri ( tmp );
						defvars . NextCookieIndex++;
						if ( CheckforUri ( cookiename ) == false )
							break;
						} while ( true );
				}
				var ck = SetCookie ( cookiename , key + "=" + value ) ;
				if ( ck == "" )
					Console . WriteLine ( $"Failed to create Cookie {cookiename}" );
				else
				{
					Console . WriteLine ( $"Cookie {cookiename} created OK." );
					defvars . Cookiedictionary . Add ( key , value );
					// This hold all THREE data items, so we can search it anytime for cookies
					AddToCookieCollection ( cookiename , key , value );
					Console . WriteLine ( $"{cookiename . PathAndQuery}" );
				}
				Console . WriteLine ( "COOKIECOLLECTION :" );
				foreach ( var item in defvars . Cookiecollection )
				{
					Cookie ckc = item as Cookie;
					ckc . Comment = $"Cookie created : {DateTime . Now}";
					Console . WriteLine ( $"Path= {ckc . Path}, Key= [{ckc . Name}], Value = [{ckc . Value}]" );
				}
				//Save the data to disk
				Cookies . Serialize ( defvars . CookieDictionarypath , defvars . Cookiedictionary );
				Cookies . Serialize ( defvars . CookieCollectionpath , defvars . Cookiecollection );
				defvars . CookieAdded = true;
				return cookiename . ToString ( );
			}
			return "";
		}
		//********************************************************************************************************************************************************************************//
		public static string ReadCookie ( string key , Uri uri = null )
		//********************************************************************************************************************************************************************************//
		{
			key = key . ToUpper ( );

			if ( defvars . Cookiedictionary == null )
			{
				defvars . Cookiedictionary = new Dictionary<string , string> ( );
				defvars . Cookiedictionary = DeSerialize ( defvars . CookieDictionarypath ) as Dictionary<string , string>;
			}
			if ( defvars . Cookiedictionary . ContainsKey ( key . ToUpper ( ) ) )
			{
				// Returns  the value from the dictionary entry whose key is 'key'
				string str = defvars.Cookiedictionary[key];
				if ( uri != null )
				{
					Uri URI = new Uri(uri.ToString());
					str = Application . GetCookie ( URI );
				}
				// Update cookie with LAST READ time in it's comment
				SetLastReadTime ( key );
				return str;
			}
			else
				return "";
		}
		//********************************************************************************************************************************************************************************//
		public static void SetLastReadTime ( string key )
		//********************************************************************************************************************************************************************************//
		{
			// Update cookie with LAST READ time in it's comment
			foreach ( var item in defvars . Cookiecollection )
			{
				Cookie cookie = item as Cookie;
				if ( cookie . Name == key )
				{
					cookie . Comment = $"Cookie last read {DateTime . Now}";
					break;
				}
			}
		}

		//********************************************************************************************************************************************************************************//
		public static void LoadCookiesToCombo ( ComboBox cb , out List<string> CookiesList )
		//********************************************************************************************************************************************************************************//
		{
			// Load cookies combo
			CookiesList = new List<string> ( );
			ComboBoxItem cbi = new ComboBoxItem();
			cbi = cb . SelectedItem as ComboBoxItem;

			string[] tmp;
			cb . ItemsSource = null;
			cb . Items . Clear ( );
			List<string> lst = new  List<string>();
			lst = Cookies . ListAllCookies ( );
			foreach ( var item in lst )
			{
				tmp = item . Split ( '^' );
				CookiesList . Add ( tmp [ 0 ] + "\t\tKey='" + tmp [ 1 ] + "'" );
				//				cb . Items . Add ( tmp [ 0 ] + "\t\tKey='" + tmp [ 1 ] + "'" );
				//SqlServerCommands . cookieKey = tmp [ 0 ];
				//SqlServerCommands . cookieValue= tmp [ 1 ];
			}
			cb . ItemsSource = CookiesList;
			cb . SelectedIndex = 0;
			cb . SelectedItem = 0;
		}

		//********************************************************************************************************************************************************************************//
		public static string SetCookie ( Uri cookiename , string value )
		//********************************************************************************************************************************************************************************//
		{
			//Uri name= new Uri(key);
			Application . SetCookie ( cookiename , value );
			return Application . GetCookie ( cookiename );
		}
		//********************************************************************************************************************************************************************************//
		public static void Serialize ( string path , object dict )
		//********************************************************************************************************************************************************************************//
		{
			try
			{
				var f_fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
				var f_binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				f_binaryFormatter . Serialize ( f_fileStream , dict );
				f_fileStream . Close ( );
			}
			catch ( Exception ex )
			{
				;
			}
		}
		//********************************************************************************************************************************************************************************//
		public static object DeSerialize ( string path )
		//********************************************************************************************************************************************************************************//
		{
			try
			{
				var f_fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
				var f_binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				var v = f_binaryFormatter.Deserialize( f_fileStream);
				f_fileStream . Close ( );
				return v;
			}
			catch ( Exception ex )
			{; }
			return null;
		}

		//********************************************************************************************************************************************************************************//
		private static void AddToCookieCollection ( Uri Uri , string key , string value )
		//********************************************************************************************************************************************************************************//
		{
			// Add to a cookie collection
			key = key . ToUpper ( );

			Cookie cookie  = new Cookie();
			cookie . Name = key . ToUpper ( );
			cookie . Path = Uri . ToString ( );
			cookie . Value = value;
			defvars . Cookiecollection . Add ( cookie );
		}
		//********************************************************************************************************************************************************************************//
		public static void CreateTestCookies ( )
		//********************************************************************************************************************************************************************************//
		{
			// This WORKS - creates Cookie etc ....
			string Cookiename = "";
			if ( Cookies . CheckCookie ( "Ian" ) == false )
			{
				// These CREATE calls add the new Cookie to Dictionary and Collection
				// CookieName is the root name, and  the library add a number to the end of it
				Cookiename = Cookies . CreateCookie ( defvars . cookierootname , "Ian" , "is the best" );
				if ( Cookiename != "" )
					Console . WriteLine ( $"New Cookie named [{Cookiename}]  Key='Ian', Value='is the best' created  successfully" );
				Cookiename = Cookies . CreateCookie ( defvars . cookierootname , "Ian1" , "wow multi values ?" );
				if ( Cookiename != "" )
					Console . WriteLine ( $"New Cookie named [{Cookiename}] 'Ian1', 'wow multi values ?'  created  successfully" );
				Cookiename = Cookies . CreateCookie ( defvars . cookierootname , "Ian2" , "Maybe, or not" );
				if ( Cookiename != "" )
					Console . WriteLine ( $"New Cookie named [{Cookiename}]  ' Ian2' , 'Maybe, or not'  created  successfully" );
			}
			else
			{
				Cookies . ReadCookie ( "Ian" . ToUpper ( ) );
				Console . WriteLine ( $"Cookie for key of [Ian] already exists" );
			}
			if ( Cookies . CheckCookie ( "Olwen" . ToUpper ( ) ) == false )
			{
				// These CREATE calls add the new Cookie to Dictionary and Collection
				Cookiename = Cookies . CreateCookie ( defvars . cookierootname , "Olwen" . ToUpper ( ) , "is the best" );
				if ( Cookiename != "" )
					Console . WriteLine ( $"New Cookie named [{Cookiename}]  Key='Olwen', Value='is the best' created  successfully" );
				//				MessageBox . Show ( "Cookie added ....\nKey=[Ian], value = [" + Cookies . ReadCookie ( uri , "Ian" )+ "]\n" );
			}
			else
			{
				Cookies . ReadCookie ( "Olwen" . ToUpper ( ) );
				Console . WriteLine ( $"Cookie for key of [Olwen] already exists" );
			}
			Cookies . Serialize ( defvars . CookieDictionarypath , defvars . Cookiedictionary );
			Cookies . Serialize ( defvars . CookieCollectionpath , defvars . Cookiecollection );

		}
		//********************************************************************************************************************************************************************************//
		public static List<string> ListAllCookies ( )
		//********************************************************************************************************************************************************************************//
		{
			List<string> lst = new List<string>();
			foreach ( var item in defvars . Cookiecollection )
			{
				Cookie ck = item as Cookie;
				lst . Add ( ck . Path + "^" + ck . Name + "^" + ck . Value + "\n" );
			}
			return lst;
		}
		//********************************************************************************************************************************************************************************//
		public static string ShowAllCookieData ( out int total , string key = "" )
		//********************************************************************************************************************************************************************************//
		{
			string tmp="";
			total = 0;
			foreach ( Cookie cook in defvars . Cookiecollection )
			{
				if ( key != "" )
				{
					tmp = cook . Name . ToUpper ( );
					if ( tmp . ToUpper ( ) != key . ToUpper ( ) )
						continue;
				}
				tmp += $"\nPath: {cook . Path}\n";
				Console . WriteLine ( $"Path: {cook . Path}" );
				Console . WriteLine ( "Cookie:" );
				tmp += $"Name  = {cook . Name}\nValue = {cook . Value}\n";
				Console . WriteLine ( $"Name  = {cook . Name}\nValue = {cook . Value}" );
				tmp += $"Domain: {cook . Domain}\n";
				Console . WriteLine ( $"Domain: {cook . Domain}" );
				tmp += $"Port: {cook . Port}\n";
				Console . WriteLine ( $"Port: {cook . Port}" );
				tmp += $"Secure: {cook . Secure}\n";
				Console . WriteLine ( $"Secure: {cook . Secure}" );
				tmp += $"When issued: {cook . TimeStamp}\n";
				Console . WriteLine ( $"When issued: {cook . TimeStamp}" );
				tmp += $"Expires: {cook . Expires} (expired? {cook . Expired})\n";
				Console . WriteLine ( $"Expires: {cook . Expires} (expired? {cook . Expired})" );
				tmp += $"Don't save: {cook . Discard}\n";
				Console . WriteLine ( $"Don't save: {cook . Discard}" );
				tmp += $"Comment: {cook . Comment}\n";
				Console . WriteLine ( $"Comment: {cook . Comment}" );
				tmp += $"Uri for comments: {cook . CommentUri}\n";
				Console . WriteLine ( $"Uri for comments: {cook . CommentUri}" );
				tmp += $"Version: RFC {( cook . Version == 1 ? 2109 : 2965 )}\n";
				Console . WriteLine ( $"Version: RFC {( cook . Version == 1 ? 2109 : 2965 )}" );
				tmp += $"String: {cook}\n";
				// Show the string representation of the cookie.
				Console . WriteLine ( $"String: {cook}" );
				total++;
				if ( key != "" )
					break;
			}
			return tmp;
		}
	}
}
