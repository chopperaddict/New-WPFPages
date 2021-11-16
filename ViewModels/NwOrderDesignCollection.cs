
using System;
using System . Data . SqlClient;
using System . Data;
using System . Diagnostics;
using System . Collections . ObjectModel;
using System . Windows;

namespace WPFPages . Views
{
        public class NwOrderDesignCollection : ObservableCollection<nworder>
	{
                public nworder NwOrder = new nworder ( );
                public NwOrderDesignCollection ( )
                {
                
                }
                public NwOrderDesignCollection ( string arg )
		{
			if ( arg == "" )
				LoadOrders ( arg );
			else
				StdLoadOrders ( arg );
		}
		public NwOrderDesignCollection StdLoadOrders ( string arg )
		{
			DataTable dt = new DataTable ( "SelectedOrders" );
			string ConString = ( string ) Properties . Settings . Default [ "NorthwindConnectionString" ];

			string CmdString = string . Empty;
			try
			{
				using ( SqlConnection con = new SqlConnection ( ConString ) )
				{
					if ( arg != "" )
						CmdString = $"SELECT *  FROM [Orders] where CustomerId = '{arg}'";
					else
						CmdString = $"SELECT *  FROM [Orders]";
					SqlCommand cmd = new SqlCommand ( CmdString, con );
					SqlDataAdapter sda = new SqlDataAdapter ( cmd );
					sda . Fill ( dt );
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"Data={ex . Data}, {ex . Message}\n[{CmdString}]" );
			}
			return CreateOrdersCollection ( dt );
		}
		public void LoadOrders ( string arg )
		{
			DataTable dt = new DataTable ( "SelectedOrders" );
			string ConString = ( string ) Properties . Settings . Default [ "NorthwindConnectionString" ];
			// Define ADO.NET objects.
			SqlConnection conn = new SqlConnection ( ConString );
			SqlCommand cmd = new SqlCommand ( );
			SqlDataReader dr;

			// Open connection, and retrieve dataset.
			conn . Open ( );

			// Define Command object.
				cmd . CommandText = "Select * From [Orders]";
			cmd . CommandType = CommandType . Text;
			cmd . Connection = conn;

			// Retrieve data reader.
			dr = cmd . ExecuteReader ( );

			int fieldCount = dr . FieldCount;
			object [ ] fieldValues = new object [ fieldCount ];
			string [ ] headers = new string [ fieldCount ];

			// Get names of fields.
			for ( int ctr = 0 ; ctr < fieldCount ; ctr++ )
				headers [ ctr ] = dr . GetName ( ctr );

			// Get data, replace missing values with dummy date as in this case the ShippedDate data is BAD "01/01/2000", and display it.
			while ( dr . Read ( ) )
			{
				dr . GetValues ( fieldValues );

				for ( int fieldCounter = 0 ; fieldCounter < fieldCount ; fieldCounter++ )
				{
					if ( Convert . IsDBNull ( fieldValues [ fieldCounter ] ) )
						fieldValues [ fieldCounter ] = "01/01/2000";
				}
				try
				{
					this . Add ( new nworder
					{
						OrderId = Convert . ToInt32 ( fieldValues [ 0 ] ),
						CustomerId = fieldValues [ 1 ] . ToString ( ),
						EmployeeId = Convert . ToInt32 ( fieldValues [ 2 ] ),
						OrderDate = Convert . ToDateTime ( fieldValues [ 3 ] ),
						RequiredDate = Convert . ToDateTime ( fieldValues [ 4 ] ),
						ShippedDate = Convert . ToDateTime ( fieldValues [ 5 ] ),
						ShipVia = Convert . ToInt32 ( fieldValues [ 6 ] ),
						Freight = Convert . ToDecimal ( fieldValues [ 7 ] ),
						ShipName = fieldValues [ 8 ] . ToString ( ),
						ShipAddress = fieldValues [ 9 ] . ToString ( ),
						ShipCity = fieldValues [ 10 ] . ToString ( ),
						ShipRegion = fieldValues [ 11 ] . ToString ( ),
						ShipPostalCode = fieldValues [ 12 ] . ToString ( ),
						ShipCountry = fieldValues [ 13 ] . ToString ( )
					} );
				}
				catch ( Exception ex )
				{
					Debug . WriteLine ( $"\nNWORDERS : DBNull ERROR  " +
					$"\n={ex . Message} , {ex . Data}");
				}
			}
			dr . Close ( );
//			string Output = sb . ToString ( );

		}
		public NwOrderDesignCollection CreateOrdersCollection ( DataTable dt )
		{
			int count = 0;
			try
			{
				for ( int i = 0 ; i < dt . Rows . Count - 1 ; i++ )
				{
					try
					{

						this . Add ( new nworder
						{
							OrderId = Convert . ToInt32 ( dt . Rows [ i ] [ 0 ] ),
							CustomerId = dt . Rows [ i ] [ 1 ] . ToString ( ),
							EmployeeId = Convert . ToInt32 ( dt . Rows [ i ] [ 2 ] ),
							OrderDate = Convert . ToDateTime ( dt . Rows [ i ] [ 3 ] ),
							RequiredDate = Convert . ToDateTime ( dt . Rows [ i ] [ 4 ] ),
							ShippedDate = Convert . ToDateTime ( dt . Rows [ i ] [ 5 ] ),
							ShipVia = Convert . ToInt32 ( dt . Rows [ i ] [ 6 ] ),
							Freight = Convert . ToDecimal ( dt . Rows [ i ] [ 7 ] ),
							ShipName = dt . Rows [ i ] [ 8 ] . ToString ( ),
							ShipAddress = dt . Rows [ i ] [ 9 ] . ToString ( ),
							ShipCity = dt . Rows [ i ] [ 10 ] . ToString ( ),
							ShipRegion = dt . Rows [ i ] [ 11 ] . ToString ( ),
							ShipPostalCode = dt . Rows [ i ] [ 12 ] . ToString ( ),
							ShipCountry = dt . Rows [ i ] [ 13 ] . ToString ( )
						} );
					}
					catch ( Exception ex )
					{
						Debug . WriteLine ( $"\nNWORDERS : DBNull ERROR  " +
						$"\nOrderId={dt . Rows [ count ] [ 0 ]}" +
						$"\nCustomerId={dt . Rows [ count ] [ 1 ]}" +
						$"\nEmployeeId={dt . Rows [ count ] [ 2 ]}" +
						$"\nOrderDate={dt . Rows [ count ] [ 3 ]}" +
						$"\nRequiredDate={dt . Rows [ count ] [ 4 ]}" +
						$"\nShippedDate={dt . Rows [ count ] [ 5 ]}" +
						$"\nShipVia={dt . Rows [ count ] [ 6 ]}" +
						$"\nFreight={dt . Rows [ count ] [ 7 ]}" +
						$"\nShipName={dt . Rows [ count ] [ 8 ]}" +
						$"\nShipAddress={dt . Rows [ count ] [ 9 ]}" +
						$"\nShipCity={dt . Rows [ count ] [ 10 ]}" +
						$"\nShipRegion={dt . Rows [ count ] [ 11 ]}" +
						$"\nShipPostalCode={dt . Rows [ count ] [ 12 ]}" +
						$"\nShipCountry={dt . Rows [ count ] [ 13 ]} \n{ex . Message} , {ex . Data}, {ex.Source}, {ex.StackTrace}"
							);
					}
					count = i;
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"NWORDERS : ERROR in  CreateOrdersCollection() : loading data into ObservableCollection : [{ex . Message}] : {ex . Data} ...." );
				MessageBox . Show ( $"NWORDERS: ERROR in  Collection() : loading data into ObservableCollection : [{ex . Message}] : {ex . Data} ...." );
				return null;
			}
			finally
			{
			}
			return this;
		}

	}
}
