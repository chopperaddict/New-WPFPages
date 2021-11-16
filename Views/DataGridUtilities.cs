using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Controls;
using System . Windows;

namespace WPFPages . Views
{
        public static class DataGridUtilities
        {
                public static void LoadDataGridColumns ( object sender, string ColumnsTemplate )
                {
                        //This works just fine
                        // Load  the BANKACCOUNT columns definitions from the Resourcefile "DataGridColumns.xaml"
                        DataGrid dg = sender as DataGrid;
                        if ( dg . Columns . Count == 0 )
                        {
                                // Get columns setup from an x:Array stored in Resources somewhere.
                                DataGridColumn [ ] columns = dg . FindResource ( ColumnsTemplate ) as DataGridColumn [ ];
                                dg . Columns . Clear ( );
                                try
                                {
                                        foreach ( var item in columns )
                                        {
                                                dg . Columns . Add ( item );
                                        }
                                }
                                catch
                                {

                                }
                        }
                }
		public static void LoadDataGridTextColumns ( object sender , string TextColumnsTemplate )
		{
			//This works just fine
			// Load  the CUSTOMER columns definitions from the Resourcefile "DataGridColumns.xaml"
			DataGrid dg2 = sender as DataGrid;
			//if ( dg2 . Columns . Count == 0 )
			//{
			//	dg2 . Columns . Clear ( );
			//	dg2 . ItemsSource = null;
			//	dg2 . Items . Clear ( );
			//}
			//// Get columns setup from an x:Array stored in Resources somewhere.
			DataGridTextColumn [ ] columns = dg2 . FindResource ( TextColumnsTemplate ) as DataGridTextColumn [ ];
			//dg2 . Columns . Clear ( );
			try
			{
				foreach ( var item2 in columns )
				{
					dg2 . Columns . Add ( item2 );
				}
			}
			catch
			{

			}
		}
	}
}
